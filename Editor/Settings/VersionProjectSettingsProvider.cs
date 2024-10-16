#region Header
// VersionProjectSettingsProvider.cs
// Author: James LaFritz
// Description: A custom settings provider for managing version settings in the Project Settings.
//              This provider allows users to configure versioning and backward compatibility.
//              It provides an interface for users to set the current and minimum save file versions,
//              with the option to enable backward compatibility support.
//              The settings are accessible in the Project Settings window at the specified path.
#endregion

using UnityEditor;
using UnityEngine;

namespace CoreFramework.Settings
{
    /// <summary>
    /// A custom settings provider for managing version settings in the Project Settings.
    /// This provider allows users to configure versioning and backward compatibility.
    /// </summary>
    public class VersionProjectSettingsProvider : SettingsProvider
    {
        // The path of the settings provider in the Project Settings window.
        private const string SettingsPath = "Project/Core Framework/Version Settings";

        // The informative text displayed in a HelpBox to guide users.
        private const string HelpBoxText =
            "Every Save File has a Version Number." +
            "\nWhen trying to load a save, only files with the current version (or the minimum legacy version) will be valid." +
            "\nChanging the version number here will overwrite the Project->Player->Version.";

        // Flag indicating whether to enable backward compatibility support.
        private bool _legacySupport;

        /// <summary>
        /// Creates a new instance of the VersionProjectSettingsProvider.
        /// </summary>
        /// <param name="path">The path of the settings provider, e.g., "Project/Core Framework/Version Settings".</param>
        /// <param name="scopes">The scope of the settings provider (default is SettingsScope.Project).</param>
        private VersionProjectSettingsProvider(string path, SettingsScope scopes = SettingsScope.Project)
            : base(path, scopes) { }
        
        #region Overrides of SettingsProvider

        /// <inheritdoc />
        public override void OnGUI(string searchContext)
        {
            // Help Box
            EditorGUILayout.HelpBox(HelpBoxText, MessageType.Info);
            EditorGUILayout.Space();

            // Retrieving current version information
            var currentVersion = VersionControl.CurrentFileVersion;
            var major = currentVersion.Major;
            var minor = currentVersion.Minor;
            var build = currentVersion.Build;
            var revision = currentVersion.Revision;

            using (var versionChangeCheckScope = new EditorGUI.ChangeCheckScope())
            {
                using (new EditorGUILayout.HorizontalScope())
                {
                    EditorGUILayout.LabelField("Current Version.", GUILayout.MaxWidth(150));

                    // Display version components for editing
                    using (new EditorGUILayout.VerticalScope(GUILayout.MaxWidth(105)))
                    {
                        EditorGUILayout.LabelField("Major", GUILayout.MaxWidth(100));
                        major = EditorGUILayout.IntField(major, GUILayout.MaxWidth(100));
                    }

                    using (new EditorGUILayout.VerticalScope(GUILayout.MaxWidth(105)))
                    {
                        EditorGUILayout.LabelField("Minor", GUILayout.MaxWidth(100));
                        minor = EditorGUILayout.IntField(minor, GUILayout.MaxWidth(100));
                    }

                    using (new EditorGUILayout.VerticalScope(GUILayout.MaxWidth(105)))
                    {
                        EditorGUILayout.LabelField("Build", GUILayout.MaxWidth(100));
                        build = EditorGUILayout.IntField(build, GUILayout.MaxWidth(100));
                    }

                    using (new EditorGUILayout.VerticalScope(GUILayout.MaxWidth(105)))
                    {
                        EditorGUILayout.LabelField("Revision", GUILayout.MaxWidth(100));
                        revision = EditorGUILayout.IntField(revision, GUILayout.MaxWidth(100));
                    }
                }

                // Toggle for legacy support
                _legacySupport = EditorGUILayout.ToggleLeft("Backwards Compatibility", _legacySupport);

                if (versionChangeCheckScope.changed)
                {
                    // Update the current file version based on user input
                    VersionProjectSettings.instance.CurrentFileVersion = new(major, minor, build, revision);
                    if (_legacySupport)
                        VersionProjectSettings.instance.MinFileVersion = VersionProjectSettings.instance.CurrentFileVersion;
                }
            }

            GUI.enabled = !_legacySupport;

            // Retrieving minimum version information
            var minVersion = VersionControl.MinFileVersion;
            var minMajor = minVersion.Major;
            var minMinor = minVersion.Minor;
            var minBuild = minVersion.Build;
            var minRevision = minVersion.Revision;

            using (var versionChangeCheckScope = new EditorGUI.ChangeCheckScope())
            {
                using (new EditorGUILayout.HorizontalScope())
                {
                    EditorGUILayout.LabelField("Min Version.", GUILayout.MaxWidth(150));

                    // Display minimum version components for editing
                    using (new EditorGUILayout.VerticalScope(GUILayout.MaxWidth(105)))
                    {
                        EditorGUILayout.LabelField("Major", GUILayout.MaxWidth(100));
                        minMajor = EditorGUILayout.IntField(minMajor, GUILayout.MaxWidth(100));
                    }

                    using (new EditorGUILayout.VerticalScope(GUILayout.MaxWidth(105)))
                    {
                        EditorGUILayout.LabelField("Minor", GUILayout.MaxWidth(100));
                        minMinor = EditorGUILayout.IntField(minMinor, GUILayout.MaxWidth(100));
                    }

                    using (new EditorGUILayout.VerticalScope(GUILayout.MaxWidth(105)))
                    {
                        EditorGUILayout.LabelField("Build", GUILayout.MaxWidth(100));
                        minBuild = EditorGUILayout.IntField(minBuild, GUILayout.MaxWidth(100));
                    }

                    using (new EditorGUILayout.VerticalScope(GUILayout.MaxWidth(105)))
                    {
                        EditorGUILayout.LabelField("Revision", GUILayout.MaxWidth(100));
                        minRevision = EditorGUILayout.IntField(minRevision, GUILayout.MaxWidth(100));
                    }
                }

                if (versionChangeCheckScope.changed)
                    // Update the minimum file version based on user input
                    VersionProjectSettings.instance.MinFileVersion = new(minMajor, minMinor, minBuild, minRevision);
            }
        }

        #endregion

        /// <summary>
        /// Creates an instance of the SaveProjectSettingsProvider and registers it with the [SettingsProvider] attribute.
        /// </summary>
        /// <returns>The created SaveProjectSettingsProvider instance.</returns>
        [SettingsProvider]
        public static SettingsProvider CreateSaveSettingsProvider() => new VersionProjectSettingsProvider(SettingsPath);

        /// <summary>
        /// Opens the Project Settings window at the specified path when the menu item is selected.
        /// </summary>
        [MenuItem("Core Framework/Version Settings")]
        private static void ProjectSettingsMenuItem() => SettingsService.OpenProjectSettings(SettingsPath);
    }
}