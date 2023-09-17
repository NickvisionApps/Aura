using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Tmds.DBus;

namespace Nickvision.Aura.Taskbar;

/// <summary>
/// An item on the taskbar
/// </summary>
public class TaskbarItem : IDisposable
{
    private bool _disposed;
    private Connection? _dbusConnection;
    private readonly LauncherEntry? _unityLauncher;
    
    /// <summary>
    /// Constructs TaskbarItem
    /// </summary>
    private TaskbarItem(string desktopFile)
    {
        _unityLauncher = new LauncherEntry(desktopFile);
        _disposed = false;
    }

    /// <summary>
    /// Finalizes the TaskbarItem
    /// </summary>
    ~TaskbarItem() => Dispose(false);
    
    /// <summary>
    /// Frees resources used by the TaskbarItem object
    /// </summary>
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Frees resources used by the TaskbarItem object
    /// </summary>
    protected virtual void Dispose(bool disposing)
    {
        if (_disposed)
        {
            return;
        }
        if (_unityLauncher != null)
        {
            _unityLauncher.CountVisible = false;
            _unityLauncher.ProgressVisible = false;
            _unityLauncher.Urgent = false;
        }
        _dbusConnection?.Dispose();
        _disposed = true;
    }
    
    /// <summary>
    /// Connects to an item on the taskbar
    /// </summary>
    /// <param name="desktopFile">Desktop file name including extension</param>
    /// <exception cref="PlatformNotSupportedException">Thrown if called not on Linux</exception>
    public static async Task<TaskbarItem?> Connect(string desktopFile)
    {
        if (!RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        {
            throw new PlatformNotSupportedException();
        }
        try
        {
            var t = new TaskbarItem(desktopFile);
            t._dbusConnection = new Connection(Address.Session);
            await t._dbusConnection.ConnectAsync();
            await t._dbusConnection.RegisterObjectAsync(t._unityLauncher);
            return t;
        }
        catch
        {
            return null;
        }
    }
    
    /// <summary>
    /// Sets progress bar value
    /// </summary>
    /// <param name="progress">A number between 0 and 1</param>
    public void SetProgress(double progress)
    {
        if (_unityLauncher != null)
        {
            _unityLauncher.Progress = progress;
            if (!_unityLauncher.ProgressVisible)
            {
                _unityLauncher.ProgressVisible = true;
            }
        }
    }
    
    /// <summary>
    /// Tells the launcher to get the user's attention
    /// </summary>
    /// <param name="urgent">Whether to enable urgent state</param>
    public void SetUrgent(bool urgent)
    {
        if (_unityLauncher != null)
        {
            _unityLauncher.Urgent = urgent;
        }
    }
}