#region Header
// CoreFrameworkSettingInitialization.cs
// Author: James LaFritz
// Description: Initialization script for Core Framework settings in the Unity Editor.
//              This script ensures that the preferences and project settings are loaded
//              on editor startup, synchronizing them with the CoreFrameworkSettings.
#endregion

using UnityEditor;

namespace CoreFramework.Settings
{
    /// <summary>
    /// Initialization script for Core Framework settings in the Unity Editor.
    /// This script ensures that the preferences and project settings are loaded
    /// on editor startup, synchronizing them with the CoreFrameworkSettings.
    /// </summary>
    [InitializeOnLoad]
    public static class CoreFrameworkSettingInitialization
    {
        
        /// <summary>
        /// Static constructor that runs when the class is loaded.
        /// </summary>
        static CoreFrameworkSettingInitialization()
        {
            // Load preferences
            var preferences = CoreFrameworkPreferences.instance;
            CoreFrameworkSettings.ShowDebug = preferences.ShowDebug;
            CoreFrameworkSettings.InfoSize = preferences.InfoSize;
            CoreFrameworkSettings.WarningSize = preferences.WarningSize;
            CoreFrameworkSettings.ErrorSize = preferences.ErrorSize;

            // Load project settings
            var projectSettings = CoreFrameworkProjectSettings.instance;
            CoreFrameworkSettings.StartScene = projectSettings.StartScene;
            CoreFrameworkSettings.BootScene = projectSettings.BootScene;
        }
    }
}