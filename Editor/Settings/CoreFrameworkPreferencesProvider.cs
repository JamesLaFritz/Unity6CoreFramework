#region Header
// CoreFrameworkPreferencesProvider.cs
// Author: James LaFritz
// Description: Settings provider for managing Core Framework preferences in the Unity Preferences window.
// This provider allows users to customize their debugging experience by toggling the display of debug logs
// and adjusting font sizes for informational, warning, and error log messages.
#endregion

using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace CoreFramework.Settings
{
    /// <summary>
    /// The `CoreFrameworkPreferencesProvider` class is a SettingsProvider responsible for displaying and managing
    /// Core Framework preferences in the Unity Editor. It allows users to toggle debugging logs and adjust log message
    /// font sizes for informational, warning, and error logs.
    /// </summary>
    public class CoreFrameworkPreferencesProvider : SettingsProvider
    {
        /// <summary>
        /// The path of the settings provider in the Preferences window.
        /// </summary>
        private const string KSettingsPath = "Preferences/Core Framework/Settings";

        /// <summary>
        /// Creates an instance of the CoreFrameworkPreferencesProvider.
        /// </summary>
        /// <param name="path">The path of the settings provider, e.g., "Preferences/Core Framework/Settings".</param>
        /// <param name="scopes">The scope of the settings provider (default is SettingsScope.User).</param>
        /// <param name="keywords">Keywords associated with the settings provider.</param>
        private CoreFrameworkPreferencesProvider(string path,
            SettingsScope scopes = SettingsScope.User,
            IEnumerable<string> keywords = null) : base(path, scopes, keywords) {}

        #region Overrides of SettingsProvider

        public override void OnActivate(string searchContext, VisualElement rootElement)
        {
            base.OnActivate(searchContext, rootElement);

            var preferences =  new SerializedObject(CoreFrameworkPreferences.instance);
            
            rootElement.Add(new Toggle("Show Debug")
            {
                bindingPath = preferences.FindProperty("_showDebug").propertyPath
            });
            rootElement.Add(new SliderInt(10, 20)
            {
                label = "Info Size",
                bindingPath = preferences.FindProperty("_infoSize").propertyPath
            });
            rootElement.Add(new SliderInt(10, 20)
            {
                label = "Warning Size",
                bindingPath = preferences.FindProperty("_warningSize").propertyPath
            });
            rootElement.Add(new SliderInt(10, 20)
            {
                label = "Error Size",
                bindingPath = preferences.FindProperty("_errorSize").propertyPath
            });
            
            rootElement.Bind(preferences);
        }

        /// <inheritdoc />
        public override void OnGUI(string searchContext)
        {
            base.OnGUI(searchContext);

            var preferences = CoreFrameworkPreferences.instance;

            preferences.ShowDebug = EditorGUILayout.ToggleLeft("Show Debug", preferences.ShowDebug);
            preferences.InfoSize = EditorGUILayout.IntSlider("Info Size", preferences.InfoSize, 10, 20);
            preferences.WarningSize = EditorGUILayout.IntSlider("Warning Size", preferences.WarningSize, 10, 20);
            preferences.ErrorSize = EditorGUILayout.IntSlider("Error Size", preferences.ErrorSize, 10, 20);
        }

        #endregion

        /// <summary>
        /// Creates an instance of the $CLASS and registers it with the [SettingsProvider] attribute.
        /// </summary>
        /// <returns>The created CoreFrameworkPreferencesProvider instance.</returns>
        [SettingsProvider]
        public static SettingsProvider CreateCoreFrameworkPreferencesProvider() =>
            new CoreFrameworkPreferencesProvider(KSettingsPath);

        /// <summary>
        /// Opens the Preferences window at the specified path when the menu item is selected.
        /// </summary>
        [MenuItem("Core Framework/Preference Settings", priority = 1)]
        private static void ProjectSettingsMenuItem() => SettingsService.OpenUserPreferences(KSettingsPath);
    }
}