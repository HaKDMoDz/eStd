﻿namespace Creek.Extensibility.Plugins
{
    /// <summary>
    /// This interface is implemented all plugins.
    /// PlugIns does not directly implement this interface, but they implement by inheriting PlugIn class.
    /// </summary>
    public interface IPlugIn : IPluggable
    {
        /// <summary>
        /// Name of the plugin.
        /// </summary>
        string Name { get; }
    }
}
