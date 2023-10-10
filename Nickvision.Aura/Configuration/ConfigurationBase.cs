using System;

namespace Nickvision.Aura.Configuration;

/// <summary>
/// Base class for configuration files
/// </summary>
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