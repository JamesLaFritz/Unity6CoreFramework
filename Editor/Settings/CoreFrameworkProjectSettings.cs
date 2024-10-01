#region Header
// CoreFrameworkProjectSettings.cs
// Author: James LaFritz
// Description: ScriptableSingleton for managing Core Framework project-specific settings.
#endregion

using System;
using UnityEditor;
using UnityEngine;

namespace CoreFramework.Settings
{
    /// <summary>
    /// ScriptableSingleton for managing Core Framework project-specific settings.
    /// </summary>
    [FilePath("ProjectSettings/CoreFrameworkProjectSettings.asset",
        FilePathAttribute.Location.ProjectFolder)]
    public class CoreFrameworkProjectSettings : ScriptableSingleton<CoreFrameworkProjectSettings>
    {
        #region Fields
        
        /// <summary>
        /// The name of the initial scene to start the application.
        /// </summary>
        [SerializeField] private string _startScene;
        
        /// <summary>
        /// The name of the bootstrapper scene that initializes the application.
        /// </summary>
        [SerializeField] private string _bootScene;

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the name of the initial scene to start the application.
        /// </summary>
        public string startScene
        {
            get => _startScene;
            set
            {
                _startScene = value;
                CoreFrameworkSettings.StartScene = value;
                Save(true);
            }
        }

        /// <summary>
        /// Gets or sets the name of the bootstrapper scene that initializes the application.
        /// </summary>
        public string bootScene
        {
            get => _bootScene;
            set
            {
                _bootScene = value;
                CoreFrameworkSettings.BootScene = value;
                Save(true);
            }
        }

        #endregion
    }
}