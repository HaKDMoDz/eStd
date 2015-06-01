using System;

namespace System.Serialisation.Json
{
    /// <summary>
    /// Specifies the state of the <see cref="JsonWriter"/>.
    /// </summary>
    public enum WriteState
    {
        /// <summary>
        /// An exception has been thrown, which has left the <see cref="JsonWriter"/> in an invalid state.
        /// You may call the <see cref="JsonWriter.Close"/> method to put the <see cref="JsonWriter"/> in the <c>Closed</c> state.
        /// Any other <see cref="JsonWriter"/> method calls results in an <see cref="InvalidOperationException"/> being thrown. 
        /// </summary>
        Error = 0,

        /// <summary>
        /// The <see cref="JsonWriter.Close"/> method has been called. 
        /// </summary>
        Closed = 1,

        /// <summary>
        /// An object is being written. 
        /// </summary>
        Object = 2,

        /// <summary>
        /// A array is being written.
        /// </summary>
        Array = 3,

        /// <summary>
        /// A constructor is being written.
        /// </summary>
        Constructor = 4,

        /// <summary>
        /// A property is being written.
        /// </summary>
        Property = 5,

        /// <summary>
        /// A write method has not been called.
        /// </summary>
        Start = 6
    }
}