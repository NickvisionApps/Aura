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
    private readonly nint _hwnd;
    private readonly ITaskbarList3? _taskbarList;
    
    /// <summary>
    /// Constructs TaskbarItem for Linux
    /// </summary>
    /// <param name="desktopFile">Desktop file name with extension</param>
    private TaskbarItem(string desktopFile)
    {
        _unityLauncher = new LauncherEntry(desktopFile);
        _disposed = false;
    }
    
    private TaskbarItem(nint hwnd)
    {
        _hwnd = hwnd;
        _taskbarList = (ITaskbarList3)new CTaskbarList();
        _taskbarList.HrInit();
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
    /// Connects to an item on the taskbar on Linux
    /// </summary>
    /// <param name="desktopFile">Desktop file name with extension</param>
    /// <exception cref="PlatformNotSupportedException">Thrown if called not on Linux</exception>
    public static async Task<TaskbarItem?> ConnectAsync(string desktopFile)
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
    /// Connects to an item on the taskbar on Windows
    /// </summary>
    /// <param name="hwnd">Window handle</param>
    /// <exception cref="PlatformNotSupportedException">Thrown if called not on Windows</exception>
    /// <exception cref="ArgumentException">Thrown if hwnd is IntPtr.Zero</exception>
    public static TaskbarItem? Connect(nint hwnd)
    {
        if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            throw new PlatformNotSupportedException();
        }
        if (hwnd == IntPtr.Zero)
        {
            throw new ArgumentException();
        }
        try
        {
            return new TaskbarItem(hwnd);
        }
        catch
        {
            return null;
        }
    }
    
    /// <summary>
    /// Sets progress bar state
    /// </summary>
    /// <param name="state">Progress bar state flag</param>
    /// <remarks>On Linux, Indeterminate is the same as NoProgress, and Error and Paused are the same as Normal</remarks>
    public void SetProgressState(ProgressFlags state)
    {
        if (_unityLauncher != null)
        {
            _unityLauncher.ProgressVisible = state >= ProgressFlags.Normal;
        }
        else if (_taskbarList != null)
        {
            _taskbarList.SetProgressState(_hwnd, state);
        }
    }
    
    /// <summary>
    /// Sets progress bar value
    /// </summary>
    /// <param name="progress">A number between 0 and 1</param>
    /// <remarks>Changing progress bar value automatically sets progress state to normal</remarks>
    public void SetProgressValue(double progress)
    {
        if (_unityLauncher != null)
        {
            _unityLauncher.Progress = progress;
            if (!_unityLauncher.ProgressVisible)
            {
                _unityLauncher.ProgressVisible = true;
            }
        }
        else if (_taskbarList != null)
        {
            _taskbarList.SetProgressState(_hwnd, ProgressFlags.Normal);
            _taskbarList.SetProgressValue(_hwnd, (ulong)(progress * 100), 100u);
        }
    }
    
    /// <summary>
    /// Sets the urgent state that tells the taskbar item to get the user's attention
    /// </summary>
    /// <param name="urgent">True to set urgent state, else false</param>
    public void SetUrgent(bool urgent)
    {
        if (_unityLauncher != null)
        {
            _unityLauncher.Urgent = urgent;
        }
        else if (_taskbarList != null)
        {
            TaskbarFlash.Change(_hwnd, urgent);
        }
    }
    
    /// <summary>
    /// Sets counter state (Linux-only)
    /// </summary>
    /// <param name="state">True if visible, else false</param>
    public void SetCountState(bool state)
    {
        if (_unityLauncher != null)
        {
            _unityLauncher.CountVisible = state;
        }
    }
    
    /// <summary>
    /// Sets counter value (Linux-only)
    /// </summary>
    /// <param name="count">Counter value</param>
    public void SetCountValue(int count)
    {
        if (_unityLauncher != null)
        {
            _unityLauncher.Count = count;
        }
    }
}