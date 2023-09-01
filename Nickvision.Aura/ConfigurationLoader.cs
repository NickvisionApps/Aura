using System;
using System.IO;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;

namespace Nickvision.Aura;

/// <summary>
/// Loader for JSON configuration files
/// </summary>
public static class ConfigurationLoader
{
    /// <summary>
    /// Load object from JSON file
    /// </summary>
    /// <typeparam name="T">Type of the object to deserialize</typeparam>
    /// <param name="key">File name</param>
    /// <returns>Loaded or new object</returns>
    internal static T Load<T>(string key) where T : ConfigurationBase
    {
        var path = $"{UserDirectories.ApplicationConfig}{Path.DirectorySeparatorChar}{key}.json";
        try
        {
            return JsonSerializer.Deserialize<T>(File.ReadAllText(path))!;
        }
        catch
        {
            if (File.Exists(path))
            {
                File.Move(path, $"{path}.bak", true);
            }
            File.WriteAllText(path, "{}");
            return JsonSerializer.Deserialize<T>(File.ReadAllText(path))!;
        }
    }

    /// <summary>
    /// Save object to JSON file
    /// </summary>
    /// <param name="obj">IConfiguration object to save</param>
    /// <param name="key">File name</param>
    internal static void Save(ConfigurationBase obj, string key)
    {
        File.WriteAllText($"{UserDirectories.ApplicationConfig}{Path.DirectorySeparatorChar}{key}.json", JsonSerializer.Serialize((object)obj, new JsonSerializerOptions { WriteIndented = true, Encoder = JavaScriptEncoder.Create(UnicodeRanges.All) }));
        obj.RaiseSavedEvent();
    }
}
