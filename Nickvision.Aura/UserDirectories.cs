using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;

namespace Nickvision.Aura;

/// <summary>
/// User directories paths
/// </summary>
public static class UserDirectories
{
    [DllImport("shell32.dll")]
    private static extern int SHGetKnownFolderPath([MarshalAs(UnmanagedType.LPStruct)] Guid id, int flags, nint token, [MarshalAs(UnmanagedType.LPWStr)] out string path);
    
    private static Guid WindowsDownloadsFolderGuid = new ("374DE290-123F-4565-9164-39C4925E467B");
    private static Dictionary<string, string> _xdgDirectories = new ();
    
    /// <summary>
    /// Main user directory
    /// </summary>
    public static string Home
    {
        get
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                return string.IsNullOrEmpty(Environment.GetEnvironmentVariable("HOME")) ? $"/home/{Environment.UserName}" : Environment.GetEnvironmentVariable("HOME")!;
            }
            return Environment.GetFolderPath(Environment.SpecialFolder.Personal);
        }
    }

    /// <summary>
    /// Config directory
    /// </summary>
    public static string Config
    {
        get
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                return string.IsNullOrEmpty(Environment.GetEnvironmentVariable("XDG_CONFIG_HOME")) ? $"{Home}/.config" : Environment.GetEnvironmentVariable("XDG_CONFIG_HOME")!;
            }
            return Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        }
    }

    /// <summary>
    /// Config subdirectory for the application
    /// </summary>
    public static string ApplicationConfig
    {
        get
        {
            var path = $"{Config}{Path.DirectorySeparatorChar}{Aura.Active.AppInfo.Name}";
            Directory.CreateDirectory(path);
            return path;
        }
    }

    /// <summary>
    /// Directory for cached files
    /// </summary>
    public static string Cache
    {
        get
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                return string.IsNullOrEmpty(Environment.GetEnvironmentVariable("XDG_CACHE_HOME")) ? $"{Home}/.cache" : Environment.GetEnvironmentVariable("XDG_CACHE_HOME")!;
            }
            return Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        }
    }

    /// <summary>
    /// The application's subdirectory for cached files
    /// </summary>
    public static string ApplicationCache
    {
        get
        {
            var path = $"{Cache}{Path.DirectorySeparatorChar}{Aura.Active.AppInfo.Name}";
            Directory.CreateDirectory(path);
            return path;
        }
    }
    
    /// <summary>
    /// Directory for local data
    /// </summary>
    public static string LocalData
    {
        get
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                return string.IsNullOrEmpty(Environment.GetEnvironmentVariable("XDG_DATA_HOME")) ? $"{Home}/.local/share" : Environment.GetEnvironmentVariable("XDG_DATA_HOME")!;
            }
            return Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        }
    }

    /// <summary>
    /// The application's subdirectory for local data
    /// </summary>
    public static string ApplicationLocalData
    {
        get
        {
            var path = $"{LocalData}{Path.DirectorySeparatorChar}{Aura.Active.AppInfo.Name}";
            Directory.CreateDirectory(path);
            return path;
        }
    }

    /// <summary>
    /// Runtime files directory (Linux only)
    /// </summary>
    public static string Runtime
    {
        get
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                return Environment.GetEnvironmentVariable("XDG_RUNTIME_DIR") ?? $"/run/user/{Environment.GetEnvironmentVariable("UID")}";
            }
            throw new PlatformNotSupportedException();
        }
    }

    /// <summary>
    /// Desktop directory
    /// </summary>
    public static string Desktop
    {
        get
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                return GetXDGUserDirectory("XDG_DESKTOP_DIR");
            }
            return Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
        }
    }

    /// <summary>
    /// Documents directory
    /// </summary>
    public static string Documents
    {
        get
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                return GetXDGUserDirectory("XDG_DOCUMENTS_DIR");
            }
            return Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        }
    }

    /// <summary>
    /// Downloads directory
    /// </summary>
    public static string Downloads
    {
        get
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                return GetXDGUserDirectory("XDG_DOWNLOAD_DIR");
            }
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows) && Environment.OSVersion.Version.Major > 5) // Windows Vista or later
            {
                SHGetKnownFolderPath(WindowsDownloadsFolderGuid, 0, IntPtr.Zero, out var path);
                return path;
            }
            throw new PlatformNotSupportedException();
        }
    }

    /// <summary>
    /// Music directory
    /// </summary>
    public static string Music
    {
        get
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                return GetXDGUserDirectory("XDG_MUSIC_DIR");
            }
            return Environment.GetFolderPath(Environment.SpecialFolder.MyMusic);
        }
    }

    /// <summary>
    /// Pictures directory
    /// </summary>
    public static string Pictures
    {
        get
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                return GetXDGUserDirectory("XDG_PICTURES_DIR");
            }
            return Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
        }
    }

    /// <summary>
    /// Publicly shared files directory
    /// </summary>
    public static string PublicShare
    {
        get
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                return GetXDGUserDirectory("XDG_PUBLICSHARE_DIR");
            }
            return Environment.GetFolderPath(Environment.SpecialFolder.CommonDocuments);
        }
    }

    /// <summary>
    /// Templates directory
    /// </summary>
    public static string Templates
    {
        get
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                return GetXDGUserDirectory("XDG_TEMPLATES_DIR");
            }
            return Environment.GetFolderPath(Environment.SpecialFolder.Templates);
        }
    }

    /// <summary>
    /// Videos directory
    /// </summary>
    public static string Videos
    {
        get
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                return GetXDGUserDirectory("XDG_VIDEOS_DIR");
            }
            return Environment.GetFolderPath(Environment.SpecialFolder.MyVideos);
        }
    }

    /// <summary>
    /// Parse XDG config file with user directories paths
    /// </summary>
    private static void ParseXDGConfig()
    {
        if (_xdgDirectories.Count > 0)
        {
            return;
        }
        var lines = File.ReadLines($"{Config}/user-dirs.dirs");
        foreach (var line in lines)
        {
            if (line.StartsWith("#") || string.IsNullOrWhiteSpace(line))
            {
                continue;
            }
            var pair = line.Split("=");
            _xdgDirectories[pair[0]] = pair[1].Replace("$HOME", Home).Trim('"');
        }
    }

    /// <summary>
    /// Get XDG user directory path for specified key
    /// </summary>
    /// <param name="key">XDG key</param>
    /// <returns>Path string</returns>
    private static string GetXDGUserDirectory(string key)
    {
        if (!string.IsNullOrEmpty(Environment.GetEnvironmentVariable(key)))
        {
            return Environment.GetEnvironmentVariable(key)!;
        }
        ParseXDGConfig();
        return _xdgDirectories[key];
    }
}