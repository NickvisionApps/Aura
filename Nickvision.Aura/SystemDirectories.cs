using System;
using System.Linq;
using System.Runtime.InteropServices;

namespace Nickvision.Aura;

/// <summary>
/// System directories paths
/// </summary>
public class SystemDirectories
{
    private static string[]? _path;
    private static string[]? _config;
    private static string[]? _data;

    /// <summary>
    /// Array of paths from PATH variable
    /// </summary>
    public static string[] Path
    {
        get
        {
            _path ??= Environment.GetEnvironmentVariable("PATH")?.Split(RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? ';' : ':').ToArray() ?? Array.Empty<string>();
            return _path;
        }
    }

    /// <summary>
    /// Array of paths from XDG_CONFIG_DIRS
    /// </summary>
    /// <remarks>Returns empty array when accessed not on Linux</remarks>
    public static string[] Config
    {
        get
        {
            if (!RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                return Array.Empty<string>();
            }
            _config ??= Environment.GetEnvironmentVariable("XDG_CONFIG_DIRS")?.Split(':').ToArray() ?? Array.Empty<string>();
            return _config;
        }
    }

    /// <summary>
    /// Array of paths from XDG_DATA_DIRS
    /// </summary>
    /// <remarks>Returns empty array when accessed not on Linux</remarks>
    public static string[] Data
    {
        get
        {
            if (!RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                return Array.Empty<string>();
            }
            _data ??= Environment.GetEnvironmentVariable("XDG_DATA_DIRS")?.Split(':').ToArray() ?? Array.Empty<string>();
            return _data;
        }
    }
}