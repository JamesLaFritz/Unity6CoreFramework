using UnityEngine;

namespace CoreFramework
{
    /// <summary>
    /// Represents a visitor in the Visitor design pattern.
    /// Provides a mechanism for defining new operations on objects/> 
    /// without changing their structure or class hierarchy.
    /// </summary>
    public interface IVisitor
    {
        /// <summary>
        /// Visits an object of type <typeparamref name="T"/> to perform an operation defined by the visitor.
        /// </summary>
        /// <typeparam name="T">The type of object being visited. Must inherit from <see cref="Component"/> and implement <see cref="IVisitable"/>.</typeparam>
        /// <param name="visitable">The object being visited.</param>
        void Visit<T>(T visitable) where T : Component, IVisitable;
    }

    /// <summary>
    /// Represents an object that can accept a visitor in the Visitor design pattern.
    /// Defines a mechanism for allowing a visitor to perform operations on this object.
    /// </summary>
    public interface IVisitable
    {
        /// <summary>
        /// Accepts a visitor and allows it to perform an operation on this object.
        /// </summary>
        /// <param name="visitor">The visitor performing the operation.</param>
        void Accept(IVisitor visitor);
    }
}