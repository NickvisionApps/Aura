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
    public long Count
    {
        get
        {
            _properties.TryGetValue("count", out var count);
            return (long?)count ?? 0;
        }
        
        set
        {
            _properties.TryGetValue("count", out var current);
            if ((long?)current != value)
            {
                _properties["count"] = value;
                OnUpdate?.Invoke((_appUri, new Dictionary<string, object> { {"count", _properties["count"]} }));
            }
        }
    }

    /// <summary>
    /// Whether or not the count is visible
    /// </summary>
    public bool CountVisible
    {
        get
        {
            _properties.TryGetValue("count-visible", out var countVisible);
            return (bool?)countVisible ?? false;
        }

        set
        {
            _properties.TryGetValue("count-visible", out var current);
            if ((bool?)current != value)
            {
                _properties["count-visible"] = value;
                OnUpdate?.Invoke((_appUri, new Dictionary<string, object> { {"count-visible", _properties["count-visible"]} }));
            }
        }
    }

    /// <summary>
    /// Value for the progress bar on the launcher icon
    /// </summary>
    /// <remarks>A number between 0 and 1</remarks>
    public double Progress
    {
        get
        {
            _properties.TryGetValue("progress", out var progress);
            return (double?)progress ?? 0.0;
        }
    
        set
        {
            _properties.TryGetValue("progress", out var current);
            if (current == null || Math.Abs((double)current - value) >= 0.01)
            {
                _properties["progress"] = value;
                OnUpdate?.Invoke((_appUri, new Dictionary<string, object> { {"progress", _properties["progress"]} }));
            }
        }
    }

    /// <summary>
    /// Whether or not the progress bar is visible
    /// </summary>
    public bool ProgressVisible
    {
        get
        {
            _properties.TryGetValue("progress-visible", out var progressVisible);
            return (bool?)progressVisible ?? false;
        }
    
        set
        {
            _properties.TryGetValue("progress-visible", out var current);
            if ((bool?) current != value)
            {
                _properties["progress-visible"] = value;
                OnUpdate?.Invoke((_appUri, new Dictionary<string, object> { {"progress-visible", _properties["progress-visible"]} }));
            }
        }
    }

    /// <summary>
    /// Tells the launcher to get the user's attention
    /// </summary>
    public bool Urgent
    {
        get
        {
            _properties.TryGetValue("urgemt", out var urgent);
            return (bool?)urgent ?? false;
        }

        set
        {
            _properties.TryGetValue("urgent", out var current);
            if ((bool?)current != value)
            {
                _properties["urgent"] = value;
                OnUpdate?.Invoke((_appUri, new Dictionary<string, object> { {"urgent", _properties["urgent"]} }));
            }
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
        var hash = 5381UL;
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