namespace CoreFramework
{
    /// <summary>
    /// Defines a generic factory for creating instances of type <typeparamref name="T"/> 
    /// using configuration data of type <typeparamref name="TC"/>.
    /// </summary>
    /// <typeparam name="T">The type of object to be created.</typeparam>
    /// <typeparam name="TC">The type of configuration data used to create the object.</typeparam>
    public interface IFactory<out T, in TC>
    {
        /// <summary>
        /// Creates an instance of type <typeparamref name="T"/> using the provided configuration.
        /// </summary>
        /// <param name="config">The configuration data used to create the object.</param>
        /// <returns>A new instance of type <typeparamref name="T"/>.</returns>
        T Create(TC config);
    }
}