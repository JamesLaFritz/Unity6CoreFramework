using UnityEngine;

namespace CoreFramework
{
    /// <summary>
    /// Abstract base class for game asset configurations used in the Flyweight pattern.
    /// Stores the reusable data, such as prefab references, that can be shared between game objects.
    /// Inherits from <see cref="ScriptableObject"/> to allow easy creation and management of shared data assets in Unity.
    /// Implements <see cref="ISharedData"/> to mark it as compatible with shared data interfaces.
    /// </summary>
    public abstract class GameAssetConfig : ScriptableObject, ISharedData
    {
        /// <summary>
        /// The prefab associated with this game asset configuration.
        /// This is a shared resource used to instantiate game objects with the same characteristics.
        /// </summary>
        public GameObject prefab;
    }
}