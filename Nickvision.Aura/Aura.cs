using System;
using System.Collections.Generic;

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

    /// <summary>
    /// Dictionary of configuration files that were set
    /// </summary>
    public Dictionary<string, ConfigurationBase> ConfigFiles { get; init; }

    /// <summary>
    /// Construct Aura
    /// </summary>
    /// <param name="id">ID for AppInfo</param>
    /// <param name="name">Name for AppInfo</param>
    private Aura(string id, string name)
    {
        AppInfo = new AppInfo()
        {
            ID = id,
            Name = name
        };
        ConfigFiles = new Dictionary<string, ConfigurationBase>();
    }
    
    /// <summary>
    /// Initialize Aura
    /// </summary>
    /// <param name="id">ID for AppInfo</param>
    /// <param name="name">Name for AppInfo</param>
    public static void Init(string id, string name)
    {
        if (_instance != null)
        {
            Console.WriteLine("[AURA] Warning: Aura was already initialized.");
            return;
        }
        _instance = new Aura(id, name);
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
                throw new AuraException("Failed to get active Aura. Are you sure it's initialized?");
            }
            return _instance;
        }
    }
    
    /// <summary>
    /// Start IPCServer or send command to a running one and quit
    /// </summary>
    /// <param name="args">Command-line arguments to send</param>
    /// <returns>New IPCServer</returns>
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
    public void SetConfig<T>(string key) where T : ConfigurationBase => ConfigFiles[key] = ConfigurationLoader.Load<T>(key)!;

    /// <summary>
    /// Save config to JSON file
    /// </summary>
    /// <param name="key">File name</param>
    public void SaveConfig(string key)
    {
        if (!ConfigFiles.ContainsKey(key))
        {
            throw new AuraException($"Configuration file \"{key}\" was not set.");
        }
        ConfigurationLoader.Save(ConfigFiles[key], key);
    }
}
