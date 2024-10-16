#region Header
// VersionInitialization.cs
// Author: James LaFritz
// Description: Initializes version-related settings upon the editor's load, ensuring consistency with project settings.
// This class sets the current and minimum file versions, checks and updates the PlayerSettings bundle version.
#endregion

using System;
using UnityEditor;

namespace CoreFramework.Settings
{
    /// <summary>
    /// Handles the initialization of save-related settings upon the editor's load.
    /// </summary>
    [InitializeOnLoad]
    public class VersionInitialization
    {
        static VersionInitialization()
        {
            // Retrieve the SaveProjectSettings instance
            var settings = VersionProjectSettings.instance;

            // Set the current and minimum save file versions from the SaveProjectSettings
            VersionControl.CurrentFileVersion = settings.CurrentFileVersion;
            VersionControl.MinFileVersion = settings.MinFileVersion;

            // Check if the PlayerSettings bundle version can be parsed as a Version
            if (Version.TryParse(PlayerSettings.bundleVersion, out var result) && VersionControl.CurrentFileVersion < result)
                // If successful and the parsed version is newer, update the current save file version
                settings.CurrentFileVersion = result;
            else
                // Otherwise, set the PlayerSettings bundle version to the current save file version
                PlayerSettings.bundleVersion = settings.CurrentFileVersion.ToString();
        }
    }
}