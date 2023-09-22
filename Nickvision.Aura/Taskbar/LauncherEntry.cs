using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Tmds.DBus;

namespace Nickvision.Aura.Taskbar;

/// <summary>
/// Unity LauncherEntry
/// </summary>
internal class LauncherEntry : ILauncherEntry
{
    private readonly string _appUri;
    private readonly Dictionary<string, object> _properties;
    
    /// <summary>
    /// DBus object path
    /// </summary>
    public ObjectPath ObjectPath { get; init; }

    /// <summary>
    /// A number to display on the launcher icon
    /// </summary>
    public int Count
    {
        get => (int)_properties["count"];
        
        set
        {
            _properties["count"] = value;
            OnUpdate?.Invoke((_appUri, new Dictionary<string, object> { {"count", _properties["count"]} }));
        }
    }

    /// <summary>
    /// Whether or not the count is visible
    /// </summary>
    public bool CountVisible
    {
        get => (bool)_properties["count-visible"];
    
        set
        {
            _properties["count-visible"] = value;
            OnUpdate?.Invoke((_appUri, new Dictionary<string, object> { {"count-visible", _properties["count-visible"]} }));
        }
    }

    /// <summary>
    /// Value for the progress bar on the launcher icon
    /// </summary>
    /// <remarks>A number between 0 and 1</remarks>
    public double Progress
    {
        get => (double)_properties["progress"];
    
        set
        {
            _properties["progress"] = value;
            OnUpdate?.Invoke((_appUri, new Dictionary<string, object> { {"progress", _properties["progress"]} }));
        }
    }

    /// <summary>
    /// Whether or not the progress bar is visible
    /// </summary>
    public bool ProgressVisible
    {
        get => (bool)_properties["progress-visible"];
    
        set
        {
            _properties["progress-visible"] = value;
            OnUpdate?.Invoke((_appUri, new Dictionary<string, object> { {"progress-visible", _properties["progress-visible"]} }));
        }
    }

    /// <summary>
    /// Tells the launcher to get the user's attention
    /// </summary>
    public bool Urgent
    {
        get => (bool)_properties["urgent"];

        set
        {
            _properties["urgent"] = value;
            OnUpdate?.Invoke((_appUri, new Dictionary<string, object> { {"urgent", _properties["urgent"]} }));
        }
    }
    
    public event Action<(string appUri, IDictionary<string, object> properties)>? OnUpdate;
    
    /// <summary>
    /// Constructs LauncherEntry
    /// </summary>
    public LauncherEntry(string desktopFile)
    {
        _appUri = $"application://{desktopFile}";
        // DJB hash
        var hash = 5318;
        foreach (var c in _appUri)
        {
            hash = hash * 33 + (byte)c;
        }
        ObjectPath = $"/com/canonical/unity/launcherentry/{hash}";
        _properties = new Dictionary<string, object>();
    }
    
    /// <summary>
    /// Adds Update DBus signal
    /// </summary>
    /// <param name="handler">Action to be invoked</param>
    public Task<IDisposable> WatchUpdateAsync(Action<(string appUri, IDictionary<string, object> properties)> handler)
    {
        return SignalWatcher.AddAsync(this, nameof(OnUpdate), handler);
    }
    
    /// <summary>
    /// Returns result in Query DBus method call
    /// </summary>
    public Task<IDictionary<string, object>> QueryAsync() => Task.FromResult((IDictionary<string, object>)_properties);
}