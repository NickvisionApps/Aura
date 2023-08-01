using System;

namespace Nickvision.Aura;

/// <summary>
/// Interface for configuration files
/// </summary>
/// <remarks>Only used for type checking</remarks>
public abstract class ConfigurationBase
{
    /// <summary>
    /// Occurs when the configuration object is saved
    /// </summary>
    public event EventHandler<EventArgs>? Saved;

    /// <summary>
    /// Raises the Saved event of the object
    /// </summary>
    internal void RaiseSavedEvent() => Saved?.Invoke(this, EventArgs.Empty);
}