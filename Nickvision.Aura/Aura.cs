using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text.Json;
using Tmds.DBus;

[assembly: InternalsVisibleTo(Connection.DynamicAssemblyName)]

namespace Nickvision.Aura;

/// <summary>
/// Main Aura class
/// </summary>
public class Aura
{
    private static Aura? _instance;

    private readonly Dictionary<string, ConfigurationBase> _configFiles;

    /// <summary>
    /// The AppInfo object
    /// </summary>
    public AppInfo AppInfo { get; init; }

    /// <summary>
    /// Construct Aura
    /// </summary>
    /// <param name="id">ID for AppInfo</param>
    /// <param name="name">Name for AppInfo</param>
    private Aura(string id, string name)
    {
        _configFiles = new Dictionary<string, ConfigurationBase>();
        AppInfo = new AppInfo()
        {
            ID = id,
            Name = name
        };
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
    /// Gets a config object
    /// </summary>
    /// <typeparam name="T">The type of ConfigurationBase</typeparam>
    /// <param name="key">The name of the config object</param>
    /// <returns>The initalized config object</returns>
    public T GetConfig<T>(string key) where T : ConfigurationBase
    {
        if(!_configFiles.ContainsKey(key))
        {
            var path = $"{UserDirectories.ApplicationConfig}{Path.DirectorySeparatorChar}{key}.json";
            try
            {
                _configFiles[key] = JsonSerializer.Deserialize<T>(File.ReadAllText(path))!;
            }
            catch
            {
                if (File.Exists(path))
                {
                    File.Move(path, $"{path}.bak", true);
                }
                File.WriteAllText(path, "{}");
                _configFiles[key] = JsonSerializer.Deserialize<T>("{}")!;
            }
            _configFiles[key].Key = key;
        }
        return (T)_configFiles[key];
    }
}
