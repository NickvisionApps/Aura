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
    private readonly LauncherProperties _properties;
    
    /// <summary>
    /// DBus object path
    /// </summary>
    public ObjectPath ObjectPath { get; init; }

    /// <summary>
    /// A number to display on the launcher icon
    /// </summary>
    public int Count
    {
        get => _properties.Count;
        
        set
        {
            _properties.Count = value;
            OnUpdate?.Invoke((_appUri, new Dictionary<string, object> { {"count", _properties.Count} }));
        }
    }

    /// <summary>
    /// Whether or not the count is visible
    /// </summary>
    public bool CountVisible
    {
        get => _properties.CountVisible;
    
        set
        {
            _properties.CountVisible = value;
            OnUpdate?.Invoke((_appUri, new Dictionary<string, object> { {"count-visible", _properties.CountVisible} }));
        }
    }

    /// <summary>
    /// Value for the progress bar on the launcher icon
    /// </summary>
    /// <remarks>A number between 0 and 1</remarks>
    public double Progress
    {
        get => _properties.Progress;
    
        set
        {
            _properties.Progress = value;
            OnUpdate?.Invoke((_appUri, new Dictionary<string, object> { {"progress", _properties.Progress} }));
        }
    }

    /// <summary>
    /// Whether or not the progress bar is visible
    /// </summary>
    public bool ProgressVisible
    {
        get => _properties.ProgressVisible;
    
        set
        {
            _properties.ProgressVisible = value;
            OnUpdate?.Invoke((_appUri, new Dictionary<string, object> { {"progress-visible", _properties.ProgressVisible} }));
        }
    }

    /// <summary>
    /// Tells the launcher to get the user's attention
    /// </summary>
    public bool Urgent
    {
        get => _properties.Urgent;

        set
        {
            _properties.Urgent = value;
            OnUpdate?.Invoke((_appUri, new Dictionary<string, object> { {"urgent", _properties.Urgent} }));
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
        _properties = new LauncherProperties();
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
    public Task<LauncherProperties> QueryAsync() => Task.FromResult(_properties);
}