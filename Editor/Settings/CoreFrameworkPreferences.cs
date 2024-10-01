#region Header
// CoreFrameworkPreferences.cs
// Author: James LaFritz
// Description: ScriptableSingleton managing Core Framework preferences for debugging logs and log message font sizes.
// This class provides fields and properties for toggling debugging logs and adjusting font sizes 
// for informational, warning, and error log messages.
#endregion

namespace CoreFramework.Settings
{
    /// <summary>
    /// The `CoreFrameworkPreferences` class is a ScriptableSingleton managing Core Framework preferences for debugging logs 
    /// and log message font sizes. It provides fields and properties for toggling debugging logs and adjusting font sizes 
    /// for informational, warning, and error log messages.
    /// </summary>
    [UnityEditor.FilePath("CoreFrameworkPreferences.asset", UnityEditor.FilePathAttribute.Location.PreferencesFolder)]
    public class CoreFrameworkPreferences : UnityEditor.ScriptableSingleton<CoreFrameworkPreferences>
    {
        #region Fields

        /// <summary>
        /// A flag indicating whether debugging logs should be shown.
        /// </summary>
        [UnityEngine.SerializeField] private bool _showDebug = true;

        /// <summary>
        /// The font size for informational log messages.
        /// </summary>
        [UnityEngine.SerializeField] private int _infoSize = 14;

        /// <summary>
        /// The font size for warning log messages.
        /// </summary>
        [UnityEngine.SerializeField] private int _warningSize = 15;

        /// <summary>
        /// The font size for error log messages.
        /// </summary>
        [UnityEngine.SerializeField] private int _errorSize = 16;

        #endregion

        #region Properties

        /// <summary>
        /// A flag indicating whether debugging logs should be shown.
        /// </summary>
        public bool showDebug
        {
            get => _showDebug;
            set
            {
                _showDebug = value;
                CoreFrameworkSettings.ShowDebug = value;
                Save(true);
            }
        }

        /// <summary>
        /// The font size for informational log messages.
        /// </summary>
        public int infoSize
        {
            get => _infoSize;
            set
            {
                _infoSize = value;
                CoreFrameworkSettings.InfoSize = value;
                Save(true);
            }
        }

        /// <summary>
        /// The font size for warning log messages.
        /// </summary>
        public int warningSize
        {
            get => _warningSize;
            set
            {
                _warningSize = value;
                CoreFrameworkSettings.WarningSize = value;
                Save(true);
            }
        }

        /// <summary>
        /// The font size for error log messages.
        /// </summary>
        public int errorSize
        {
            get => _errorSize;
            set
            {
                _errorSize = value;
                CoreFrameworkSettings.ErrorSize = value;
                Save(true);
            }
        }

        #endregion
    }
}