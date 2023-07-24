using System;

namespace Nickvision.Aura;

/// <summary>
/// Main Aura class
/// </summary>
public class Aura
{
    private static Aura? _instance;
    
    /// <summary>
    /// The AppInfo object
    /// </summary>
    public AppInfo AppInfo { get; init; }

    public Dictionary<string, object> ConfigFiles;

    public event EventHandler<Dictionary<string, string>>? StartArgumentsReceived;

    /// <summary>
    /// Occurs when the configuration is saved to disk
    /// </summary>
    public static event EventHandler<string>? ConfigSaved;
    
    /// <summary>
    /// Construct Aura
    /// </summary>
    /// <param name="id">ID for AppInfo</param>
    /// <param name="name">Name for AppInfo</param>
    /// <param name="shortName">ShortName for AppInfo</param>
    /// <param name="description">Description for AppInfo</param>
    public Aura(string id, string name, string shortName, string description)
    {
        AppInfo = new AppInfo()
        {
            ID = id,
            Name = name,
            ShortName = shortName,
            Description = description
        };
        _instance = this;
        ConfigFiles = new Dictionary<string, object>();
    }
    
    /// <summary>
    /// Get currently active instance of Aura
    /// </summary>
    /// <exception>Thrown if Aura wasn't created</exception>
    public static Aura Active
    {
        get
        {
            if (_instance == null)
            {
                throw new Exception("Failed to get active Aura. Are you sure it's created?");
            }
            return _instance;
        }
    }
    
    /// <summary>
    /// Process command-line arguments
    /// </summary>
    /// <remarks>Don't use this if using Communicate, IPCServer will call it on start too</remarks>
    internal void ProcessCommandLine(string[] args) => StartArgumentsReceived?.Invoke(this, CommandLine.Parse(args));
    
    /// <summary>
    /// Start IPCServer or send command to a running one and quit
    /// </summary>
    /// <param name="args">Command-line arguments to process/send</param>
    /// <returns>New IPCServer</returns>
    /// <remarks>If using Communicate, don't use ProcessCommandLine, IPCServer will process arguments on start too</remarks>
    public IPCServer Communicate(string[] args)
    {
        var server = new IPCServer();
        var started = server.Communicate(args);
        if (!started)
        {
            Environment.Exit(0);
        }
        return server;
    }

    /// <summary>
    /// Set config to be loaded from JSON file
    /// </summary>
    /// <typeparam name="T">Object type</typeparam>
    /// <param name="key">File name</param>
    public void SetConfig<T>(string key)
    {
        ConfigFiles[key] = ConfigLoader.Load<T>(key)!;
    }

    /// <summary>
    /// Save config to JSON file
    /// </summary>
    /// <param name="key">File name</param>
    public void SaveConfig(string key)
    {
        if (!ConfigFiles.ContainsKey(key))
        {
            throw new Exception($"Configuration file \"{key}\" was not set.");
        }
        ConfigLoader.Save(ConfigFiles[key], key);
        ConfigSaved?.Invoke(this, key);
    }
}
