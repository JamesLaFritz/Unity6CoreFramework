#region Header
// CoreFrameworkProjectSettingsProvider.cs
// Author: James LaFritz
// Description: Custom settings provider for managing Core Framework project settings in the Unity Editor.
#endregion

using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine.UIElements;

namespace CoreFramework.Settings
{
    /// <summary>
    /// The `CoreFrameworkProjectSettingsProvider` class is a SettingsProvider responsible for displaying and managing project settings in the Unity Editor.
    /// </summary>
    public class CoreFrameworkProjectSettingsProvider : SettingsProvider
    {
        /// <summary>
        /// The path of the settings provider in the Preferences window.
        /// </summary>
        private const string KSettingsPath = "Project/Core Framework/Settings";

        /// <summary>
        /// Creates an instance of the CoreFrameworkProjectSettingsProvider.
        /// </summary>
        /// <param name="path">The path of the settings provider, e.g., "Preferences/Core Framework/Settings".</param>
        /// <param name="scopes">The scope of the settings provider (default is SettingsScope.Project).</param>
        /// <param name="keywords">Keywords associated with the settings provider.</param>
        private CoreFrameworkProjectSettingsProvider(string path,
            SettingsScope scopes = SettingsScope.Project,
            IEnumerable<string> keywords = null) : base(path, scopes, keywords)
        {
        }

        #region Overrides of SettingsProvider

        /*public override void OnActivate(string searchContext, VisualElement rootElement)
        {
            // Retrieve available scenes
            var scenes = (from scene in EditorBuildSettings.scenes where scene.enabled select scene.path).ToList();
            //var scenes = SceneAttributePropertyDrawer.GetScenes() ?? new[] { "" };

            // Insert "None" option if more than one scene is available
            if (scenes.Count > 0)
            {
                scenes.Insert(0, "None");
            }
            else
            {
                scenes.Add("None");
            }
            
            // Get current project settings
            var settings = CoreFrameworkProjectSettings.instance;

            var startField = new PopupField<string>
            {
                label = "Start Scene",
                choices = scenes,
                index = scenes.Contains(settings.bootScene) ? scenes.IndexOf(settings.bootScene) : 0
            };
            startField.RegisterValueChangedCallback(StartFieldChanged);
            rootElement.Add(startField);

            var bootField = new PopupField<string>
            {
                label = "Boot Scene",
                choices = scenes,
                index = scenes.Contains(settings.bootScene) ? scenes.IndexOf(settings.bootScene) : 0
            };
            bootField.RegisterValueChangedCallback(BootFieldChanged);
            rootElement.Add(bootField);
        }*/

        /// <inheritdoc />
        public override void OnGUI(string searchContext)
        {
            // This function is never called since UIElements is drawing the UI.

            // Retrieve available scenes
            var scenes = (from scene in EditorBuildSettings.scenes where scene.enabled select scene.path).ToArray();
            //var scenes = SceneAttributePropertyDrawer.GetScenes() ?? new[] { "" };
            var scenesList = scenes.ToList();

            // Insert "None" option if more than one scene is available
            if (scenesList.Count > 0)
                scenesList.Insert(0, "None");
            else
                scenesList.Add("None");
            
            scenes = scenesList.ToArray();

            // Get current project settings
            var settings = CoreFrameworkProjectSettings.instance;

            // Get indices for selected scenes
            var selectedStartSceneIndex =
                scenes.Contains(settings.StartScene) ? scenesList.IndexOf(settings.StartScene) : 0;

            var selectedBootSceneIndex = scenes.Contains(settings.BootScene) ? scenesList.IndexOf(settings.BootScene) : 0;

            // Display dropdowns for selecting Boot Scene and Start Scene
            settings.BootScene = scenesList[EditorGUILayout.Popup("Boot Scene", selectedBootSceneIndex, scenes)];
            settings.StartScene = scenesList[EditorGUILayout.Popup("Start Scene", selectedStartSceneIndex, scenes)];
        }

        #endregion

        private static void StartFieldChanged(ChangeEvent<string> evt)
        {
            CoreFrameworkProjectSettings.instance.StartScene = evt.newValue;
        }
        
        private static void BootFieldChanged(ChangeEvent<string> evt)
        {
            CoreFrameworkProjectSettings.instance.BootScene = evt.newValue;
        }

        /// <summary>
        /// Creates an instance of the $CLASS and registers it with the [SettingsProvider] attribute.
        /// </summary>
        /// <returns>The created CoreFrameworkProjectSettingsProvider instance.</returns>
        [SettingsProvider]
        public static SettingsProvider CreateCoreFrameworkProjectSettingsProvider() =>
            new CoreFrameworkProjectSettingsProvider(KSettingsPath)
            {
                // Populate the search keywords to enable smart search filtering and label highlighting:
                keywords = new HashSet<string>(new[] { "Boot Scene", "Start Scene" }),
            };

        /// <summary>
        /// Opens the Project Settings window at the specified path when the menu item is selected.
        /// </summary>
        [MenuItem("Core Framework/Project Settings", priority = 0)]
        private static void ProjectSettingsMenuItem() =>
            SettingsService.OpenProjectSettings(KSettingsPath);
    }
}