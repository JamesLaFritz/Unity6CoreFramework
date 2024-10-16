#region Header
// VersionControl.cs
// Author: James LaFritz
// Description: Static class managing the versioning of save files within the Core Framework.
//              It ensures backward compatibility, allowing identification and potential handling of save files from earlier versions.
//              Developers can use version numbers for migration logic to upgrade older save files to the latest version without data loss.
#endregion

using System;

namespace CoreFramework.Settings
{
    /// <summary>
    /// The `VersionControl` static class is designed to manage the versioning of save files within the game.
    /// This ensures backward compatibility, allowing the game to identify and potentially handle save files 
    /// from earlier versions. Developers can use these version numbers to apply migration logic, ensuring that 
    /// older save files can be upgraded to the latest version without loss of data.
    ///
    /// The class exposes two integer fields, `CurrentFileVersion` and `MinFileVersion`, which represent the 
    /// latest version of the save file format and the earliest version that is still supported, respectively.
    /// Developers should update the `CurrentFileVersion` whenever there are changes in the save file format 
    /// and set `MinFileVersion` to the oldest version that the game can still load without issues.
    /// </summary>
    public static class VersionControl
    {
        /// <summary>
        /// Represents the latest version of the save file format. This should be incremented whenever 
        /// there's a change in the structure or data stored in save files.
        /// </summary>
        public static Version CurrentFileVersion = new(0, 0, 0, 1);

        /// <summary>
        /// Denotes the earliest version of the save file that the game still supports. This allows the game 
        /// to handle older save files by potentially migrating or upgrading them to the current format.
        /// </summary>
        public static Version MinFileVersion = new(0, 0, 0, 1);
    }
}