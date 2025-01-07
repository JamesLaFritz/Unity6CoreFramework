using UnityEngine;

namespace CoreFramework
{
    /// <summary>
    /// Generic interface for implementing game asset factories in the Factory design pattern.
    /// Provides a mechanism to create Unity <see cref="Component"/> instances of type <typeparamref name="T"/> 
    /// using game asset configurations of type <typeparamref name="TC"/>.
    /// </summary>
    /// <typeparam name="T">The type of Unity component to be created. Must inherit from <see cref="Component"/>.</typeparam>
    /// <typeparam name="TC">The type of game asset configuration used to create the component. 
    /// Must inherit from <see cref="GameAssetConfig"/>.</typeparam>
    public interface IGameAssetFactory<out T, in TC> 
        where T : Component 
        where TC : GameAssetConfig
    {
        /// <summary>
        /// Creates a Unity component of type <typeparamref name="T"/> using the specified game asset configuration.
        /// </summary>
        /// <param name="config">The game asset configuration containing the prefab and other shared data.</param>
        /// <returns>A new instance of <typeparamref name="T"/> initialized based on <paramref name="config"/>.</returns>
        T Create(TC config);
    }
}