#region Header
// BuildProcessing.cs
// Author: James LaFritz
// Description: 
// Provides methods for automating build processes in Unity, including version incrementing, build pipeline execution, 
// exporting version files, and optionally creating an installer. This script uses Unity's IPreprocessBuildWithReport 
// and IPostprocessBuildWithReport interfaces to execute build logic before and after the build process.
#endregion

using System.IO;
using CoreFramework.Settings;
using Microsoft.Win32;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;

namespace CoreFramework
{
    /// <summary>
    /// Handles the build process by automating tasks such as version incrementing, executing builds,
    /// exporting version files, and optionally creating installers.
    /// Implements Unity's IPreprocessBuildWithReport and IPostprocessBuildWithReport interfaces.
    /// </summary>
    public class BuildProcessing : IPreprocessBuildWithReport, IPostprocessBuildWithReport
    {
        /// <summary>
        /// Specifies the order in which callbacks should be invoked.
        /// </summary>
        public int callbackOrder => 0;

        // Flags to indicate which version component to increment.
        private static bool _incrementMajor;
        private static bool _incrementMinor;
        private static bool _incrementBuild;
        private static bool _incrementRevision;

        /// <summary>
        /// Initiates a quick build as a hotfix or patch, incrementing the build number.
        /// </summary>
        [MenuItem("Core Framework/Build/0 HotFix or Patch")]
        private static void QuickBuildMenuItem()
        {
            _incrementBuild = true;

            Build();

            _incrementBuild = false;
        }

        /// <summary>
        /// Initiates a quick build for a new feature, incrementing the minor version.
        /// </summary>
        [MenuItem("Core Framework/Build/1 New Feature")]
        private static void QuickBuildMinorMenuItem()
        {
            _incrementMinor = true;

            Build();

            _incrementMinor = false;
        }

        /// <summary>
        /// Initiates a quick build for a breaking API change, incrementing the major version.
        /// </summary>
        [MenuItem("Core Framework/Build/2 Breaking API Change")]
        private static void QuickBuildMajorMenuItem()
        {
            _incrementMajor = true;

            Build();

            _incrementMajor = false;
        }

        /// <summary>
        /// Initiates a quick build for Internal Testing, incrementing the revision number.
        /// </summary>
        [MenuItem("Core Framework/Build/3 Internal Testing")]
        private static void QuickBuildRevisionBuildMenuItem()
        {
            _incrementRevision = true;

            Build();

            _incrementRevision = false;
        }

        /// <summary>
        /// Called before the build process starts. Increments the build version based on selected flags.
        /// </summary>
        /// <param name="report">Provides information about the build.</param>
        public void OnPreprocessBuild(BuildReport report)
        {
            IncrementBuildVersion();
        }

        /// <summary>
        /// Called after the build process finishes. Exports a version file and optionally runs the installer compiler.
        /// </summary>
        /// <param name="report">Provides information about the build.</param>
        public void OnPostprocessBuild(BuildReport report)
        {
            // Skip further processing if the build failed
            if (report.summary.result == BuildResult.Failed) return;

            // Export the version file
            ExportVersionFile(report.summary.outputPath);

            // Check if the platform is standalone and prompt for installer creation
            if (report.summary.platformGroup != BuildTargetGroup.Standalone) return;
            if (EditorUtility.DisplayDialog("Create Installer", "Would you like to create an installer?", "Yes", "No"))
                RunInstallerCompiler();
        }

        /// <summary>
        /// Builds the project based on the current settings.
        /// </summary>
        private static void Build()
        {
            var buildPath = Path.Combine(Application.dataPath, "../Builds/");
            if (!Directory.Exists(buildPath)) Directory.CreateDirectory(buildPath);

            var exePath = Path.Combine(buildPath, Application.productName + ".exe");

            // Build the player using the scenes set in EditorBuildSettings
            BuildPipeline.BuildPlayer(EditorBuildSettings.scenes, exePath, EditorUserBuildSettings.activeBuildTarget,
                BuildOptions.None);
        }

        /// <summary>
        /// Increments the version number based on selected flags.
        /// </summary>
        private static void IncrementBuildVersion()
        {
            var version = VersionProjectSettings.instance.CurrentFileVersion;

            // Increment the version based on the set flags
            if (_incrementRevision)
                version = new(version.Major, version.Minor, version.Build, version.Revision + 1);
            if (_incrementBuild)
                version = new(version.Major, version.Minor, version.Build + 1, 0);
            if (_incrementMinor)
                version = new(version.Major, version.Minor + 1, 0, 0);
            if (_incrementMajor)
                version = new(version.Major + 1, 0, 0, 0);

            // Apply the updated version
            VersionProjectSettings.instance.CurrentFileVersion = version;
        }

        /// <summary>
        /// Exports a version file to the specified build output path.
        /// </summary>
        /// <param name="outputPath">The path to the build output directory.</param>
        private static void ExportVersionFile(string outputPath)
        {
            var buildPath = new FileInfo(outputPath).Directory!.FullName;
            var versionPath = Path.Combine(buildPath, "Version.txt");
            
            // Write the current bundle version to the version file
            File.WriteAllText(versionPath, PlayerSettings.bundleVersion);
        }

        /// <summary>
        /// Runs the installer compiler using Inno Setup.
        /// </summary>
        private static void RunInstallerCompiler()
        {
            if (!TryGetInnoSetupCompilerPath(out var innoCompilerPath)) return;

            var installationPath = Path.Combine(Application.dataPath, "../Installers");
            var installerPath = Path.Combine(installationPath, "installer.iss");

            // Check if both the Inno Setup compiler and installer script exist
            if (!File.Exists(innoCompilerPath) || !File.Exists(installerPath)) return;
            
            // Run the Inno Setup compiler process
            var process = new System.Diagnostics.Process
            {
                StartInfo =
                {
                    FileName = innoCompilerPath,
                    Arguments = $"/cc {installerPath}"
                }
            };
            process.Start();
        }

        /// <summary>
        /// Tries to retrieve the path to the Inno Setup compiler from the registry.
        /// </summary>
        /// <param name="path">The output path to the Inno Setup compiler.</param>
        /// <returns>True if the compiler path was found, otherwise false.</returns>
        private static bool TryGetInnoSetupCompilerPath(out string path)
        {
            var key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Classes\InnoSetupScriptFile\shell\open\command");
            if (key != null)
            {
                var value = key.GetValue("") as string;
                foreach (var entry in value?.Split('\"')!)
                {
                    // Find the executable file path
                    if (!entry.EndsWith(".exe")) continue;
                    path = entry;
                    return true;
                }
            }

            Debug.LogError("Inno Setup Compiler not found!");
            path = null;
            return false;
        }
    }
}