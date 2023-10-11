using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;

namespace Nickvision.Aura;

/// <summary>
/// Methods for working with system dependencies
/// </summary>
public static class DependencyLocator
{
    private static readonly Dictionary<string, string?> _locations;

    /// <summary>
    /// Constructs a static DependencyLocator
    /// </summary>
    static DependencyLocator()
    {
        _locations = new Dictionary<string, string?>();
    }

    /// <summary>
    /// Find the path of a given dependency
    /// </summary>
    /// <param name="dependency">The name of the dependency to find</param>
    /// <returns>The path of the dependency if found, else null</returns>
    public static string? Find(string dependency)
    {
        dependency = RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? $"{dependency}.exe" : dependency;
        if (_locations.ContainsKey(dependency) && !string.IsNullOrEmpty(_locations[dependency]) && File.Exists(_locations[dependency]))
        {
            return _locations[dependency];
        }
        _locations[dependency] = null;
        var assemblyPath = Path.GetFullPath(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!);
        if (File.Exists($"{assemblyPath}{Path.DirectorySeparatorChar}{dependency}"))
        {
            _locations[dependency] = $"{assemblyPath}{Path.DirectorySeparatorChar}{dependency}";
        }
        else
        {
            foreach (var dir in SystemDirectories.Path)
            {
                if (File.Exists($"{dir}{Path.DirectorySeparatorChar}{dependency}") && !dir.Contains("AppData\\Local\\Microsoft\\WindowsApps"))
                {
                    _locations[dependency] = $"{dir}{Path.DirectorySeparatorChar}{dependency}";
                    break;
                }
            }
        }
        return _locations[dependency];
    }
}
