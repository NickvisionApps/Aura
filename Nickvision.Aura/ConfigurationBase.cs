using System;
using System.IO;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;

namespace Nickvision.Aura;

/// <summary>
/// Base class for configuration files
/// </summary>
public abstract class ConfigurationBase
{
    /// <summary>
    /// The key of the Configuration object
    /// </summary>
    internal string? Key { get; set; }

    /// <summary>
    /// Occurs when the configuration object is saved
    /// </summary>
    public event EventHandler<EventArgs>? Saved;

    /// <summary>
    /// Saves the configuration file
    /// </summary>
    public void Save()
    {
        if(string.IsNullOrWhiteSpace(Key))
        {
            throw new ArgumentException("ConfigurationBase.Key must not be empty");
        }
        File.WriteAllText($"{UserDirectories.ApplicationConfig}{Path.DirectorySeparatorChar}{Key}.json", JsonSerializer.Serialize((object)this, new JsonSerializerOptions { WriteIndented = true, Encoder = JavaScriptEncoder.Create(UnicodeRanges.All) }));
        Saved?.Invoke(this, EventArgs.Empty);
    }
}