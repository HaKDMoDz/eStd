using System;

namespace System.Serialisation
{
    /// <summary>
    ///   All labeled with that Attribute object properties are ignored during the serialization. See PropertyProvider
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, Inherited = true, AllowMultiple = false)]
    public sealed class ExcludeFromSerializationAttribute : Attribute
    {
    }
}