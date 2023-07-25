using System;
using System.IO;
using System.Text.Json;

namespace Nickvision.Aura;

/// <summary>
/// Loader for JSON configuration files
/// </summary>
public static class ConfigLoader
{
    /// <summary>
    /// Configuration files directory
    /// </summary>
    public static readonly string ConfigDir = $"{Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)}{Path.DirectorySeparatorChar}{Aura.Active.AppInfo.Name}";
    
    /// <summary>
    /// Load object from JSON file
    /// </summary>
    /// <typeparam name="T">Type of the object to deserialize</typeparam>
    /// <param name="key">File name</param>
    /// <returns>Loaded or new object</returns>
    internal static T Load<T>(string key)
    {
        if (!File.Exists($"{ConfigDir}{Path.DirectorySeparatorChar}{key}.json"))
        {
            Directory.CreateDirectory(ConfigDir);
            Save(new object(), key);
        }
        return JsonSerializer.Deserialize<T>(File.ReadAllText($"{ConfigDir}{Path.DirectorySeparatorChar}{key}.json"))!;
    }
    
    /// <summary>
    /// Save object to JSON file
    /// </summary>
    /// <param name="obj">Object to save</param>
    /// <param name="key">File name</param>
    internal static void Save(object obj, string key) => File.WriteAllText($"{ConfigDir}{Path.DirectorySeparatorChar}{key}.json", JsonSerializer.Serialize(obj, new JsonSerializerOptions { WriteIndented = true }));
}