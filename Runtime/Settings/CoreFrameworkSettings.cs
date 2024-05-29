#region Header
// CoreFrameworkSettings.cs
#endregion

namespace CoreFramework.Settings
{
    public static class CoreFrameworkSettings
    {

        /// <summary>
        /// The name of the starting scene. Defaults to "SampleScene" if SettingsSO is not set.
        /// </summary>
        public static string StartScene = null;

        /// <summary>
        /// The name of the boot scene. Defaults to "Bootstrapper" if SettingsSO is not set.
        /// </summary>
        public static string BootScene = null;

        /// <summary>
        /// Flag to indicate if debugging information should be displayed.
        /// Defaults to true if SettingsSO is not set.
        /// </summary>
        public static bool ShowDebug;

        /// <summary>
        /// The font size for informational log messages.
        /// Defaults to 14 if SettingsSO is not set.
        /// </summary>
        public static int InfoSize = 14;

        /// <summary>
        /// The font size for warning log messages.
        /// Defaults to 15 if SettingsSO is not set.
        /// </summary>
        public static int WarningSize = 15;

        /// <summary>
        /// The font size for error log messages.
        /// Defaults to 16 if SettingsSO is not set.
        /// </summary>
        public static int ErrorSize = 16;
    }
}