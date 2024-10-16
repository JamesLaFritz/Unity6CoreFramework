#region Header
// VersionProjectSettings.cs
// Author: James LaFritz
// Manages project-specific settings related to versions and backward compatibility.
// Handles serialization of version information and provides functionality to update these settings.
// This scriptable object is intended to be used as a singleton through the VersionProjectSettings.instance property.
// FilePath attribute specifies the location of the serialized asset in the ProjectFolder.
#endregion

using System;
using UnityEditor;
using UnityEngine;

namespace CoreFramework.Settings
{
    /// <summary>
    /// Represents the project settings for managing versions and backward compatibility.
    /// </summary>
    [FilePath("ProjectSettings/VersionProjectSettings.asset", FilePathAttribute.Location.ProjectFolder)]
    public class VersionProjectSettings : ScriptableSingleton<VersionProjectSettings>
    {
        #region Version

        /// <summary>
        /// The major version component (when you make incompatible API changes).
        /// </summary>
        [SerializeField] private int _major;

        /// <summary>
        /// The minor version component (when you add functionality in a backward compatible manner).
        /// </summary>
        [SerializeField] private int _minor;

        /// <summary>
        /// The build version component (version when you make backward compatible bug fixes).
        /// </summary>
        [SerializeField] private int _build;

        /// <summary>
        /// The revision version (Pre-release).
        /// </summary>
        [SerializeField] private int _revision = 1;

        /// <summary>
        /// Represents the latest version. This should be incremented whenever 
        /// there's a change in the structure or data stored in save files.
        /// Major(when you make incompatible API changes), Minor(when you add functionality in a backward compatible manner, i.e. new feature),
        /// Build(version when you make backward compatible bug fixes), Revision(Pre-release number i.e. internal testing)
        /// </summary>
        public Version CurrentFileVersion
        {
            get => new(_major, _minor, _build, _revision);
            set
            {
                PlayerSettings.bundleVersion = value.ToString();
                VersionControl.CurrentFileVersion = value;
                _major = value.Major;
                _minor = value.Minor;
                _build = value.Build;
                _revision = value.Revision;
                Save(true);
            }
        }

        #endregion

        #region Min Version

        // Serialization fields for the minimum supported save file version
        /// <summary>
        /// The major version component of the minimum supported save file format.
        /// </summary>
        [SerializeField] private int _minMajor;

        /// <summary>
        /// The minor version component of the minimum supported save file format.
        /// </summary>
        [SerializeField] private int _minMinor;

        /// <summary>
        /// The build version component of the minimum supported save file format.
        /// </summary>
        [SerializeField] private int _minBuild;

        /// <summary>
        /// The revision version component of the minimum supported save file format.
        /// </summary>
        [SerializeField] private int _minRevision;

        /// <summary>
        /// Denotes the earliest version of the save file that the game still supports. This allows the game 
        /// to handle older save files by potentially migrating or upgrading them to the current format.
        /// </summary>
        public Version MinFileVersion
        {
            get => new(_minMajor, _minMinor, _minBuild, _minRevision);
            set
            {
                VersionControl.MinFileVersion = value;
                _minMajor = value.Major;
                _minMinor = value.Minor;
                _minBuild = value.Build;
                _minRevision = value.Revision;
                Save(true);
            }
        }

        #endregion
    }
}