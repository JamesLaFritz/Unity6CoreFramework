#region Header
// Bootstrapper.cs
// Author: James LaFritz
// Description: Handles the initial bootstrapping of the application, ensuring that
// essential settings are applied and that the initial scene is loaded.
#endregion

using System;
using CoreFramework.Settings;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace CoreFramework
{
    /// <summary>
    /// Handles the initial bootstrapping of the application, ensuring that
    /// essential settings are applied and that the initial scene is loaded.
    /// </summary>
    [HelpURL("https://jameslafritz.github.io/CoreFramework2022/Manual/Bootstrapper.html")]
    public class Bootstrapper : MonoBehaviour
    {
        /// <summary>
        /// Asynchronously initializes necessary services and loads the starting scene
        /// when the application begins.
        /// </summary>
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Init()
        {
#if UNITY_EDITOR
            var currentlyLoadedEditorScene = SceneManager.GetActiveScene();
#endif
            if (string.IsNullOrWhiteSpace(CoreFrameworkSettings.BootScene) ||
                string.Compare(CoreFrameworkSettings.BootScene, "None", StringComparison.Ordinal) == 0) return;

            // Load the designated boot scene if it's not already loaded.
            if (SceneManager.GetSceneByName(CoreFrameworkSettings.BootScene).isLoaded != true)
            {
#if UNITY_EDITOR
                SceneManager.UnloadSceneAsync(currentlyLoadedEditorScene);
#endif
                SceneManager.LoadScene(CoreFrameworkSettings.BootScene);
            }

#if UNITY_EDITOR
            SceneManager.LoadSceneAsync(currentlyLoadedEditorScene.name, LoadSceneMode.Additive);
#else
            if (string.IsNullOrWhiteSpace(CoreFrameworkSettings.StartScene) ||
                string.Compare(CoreFrameworkSettings.StartScene, "None", StringComparison.Ordinal) == 0 ||
                !SceneManager.GetSceneByName(CoreFrameworkSettings.StartScene).isLoaded) return;
            // Load the start scene additively in a built game.
            SceneManager.LoadSceneAsync(CoreFrameworkSettings.StartScene, LoadSceneMode.Additive);
#endif
        }
    }
}