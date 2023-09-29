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
    private readonly Connection? _dbusConnection;
    private readonly LauncherEntry? _unityLauncher;
    private readonly nint _hwnd;
    private readonly ITaskbarList3? _taskbarList;
    private System.Drawing.Bitmap? _countIconWindows;
    private ProgressFlags _progressState;
    private double _progress;
    private bool _urgent;
    private bool _countVisible;
    private long _count;

    /// <summary>
    /// Constructs a generic TaskbarItem
    /// </summary>
    private TaskbarItem()
    {
        _disposed = false;
        _dbusConnection = null;
        _unityLauncher = null;
        _hwnd = IntPtr.Zero;
        _taskbarList = null;
        _progressState = ProgressFlags.NoProgress;
        _progress = 0;
        _urgent = false;
        _countVisible = false;
        _count = 0;
    }

    /// <summary>
    /// Constructs a TaskbarItem for Linux
    /// </summary>
    /// <param name="desktopFile">Desktop file name with extension</param>
    /// <param name="dbus">The dbus connection object</param>
    private TaskbarItem(string desktopFile, Connection? dbus) : this()
    {
        _unityLauncher = new LauncherEntry(desktopFile);
        _dbusConnection = dbus;
    }

    /// <summary>
    /// Constructs a TaskbarItem for Windows
    /// </summary>
    /// <param name="hwnd">Window handle</param>
    private TaskbarItem(nint hwnd) : this()
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
    /// Progress bar state
    /// </summary>
    /// <remarks>>On Linux, Indeterminate is the same as NoProgress, and Error and Paused are the same as Normal</remarks>
    public ProgressFlags ProgressState
    {
        get => _progressState;

        set
        {
            _progressState = value;
            if (_unityLauncher != null)
            {
                _unityLauncher.ProgressVisible = _progressState >= ProgressFlags.Normal;
            }
            if (_taskbarList != null)
            {
                _taskbarList.SetProgressState(_hwnd, _progressState);
            }
        }
    }

    /// <summary>
    /// Progress bar value
    /// </summary>
    /// <remarks>Changing progress bar value automatically sets progress state to normal</remarks>
    public double Progress
    {
        get => _progress;

        set
        {
            _progress = value;
            if (_unityLauncher != null)
            {
                _unityLauncher.Progress = _progress;
            }
            else if (_taskbarList != null)
            {
                _taskbarList.SetProgressValue(_hwnd, (ulong)(_progress * 100), 100u);
            }
            ProgressState = ProgressFlags.Normal;
        }
    }

    /// <summary>
    /// Whether or not the taskbar icon is shown as urgent
    /// </summary>
    public bool Urgent
    {
        get => _urgent;

        set
        {
            _urgent = value;
            if (_unityLauncher != null)
            {
                _unityLauncher.Urgent = _urgent;
            }
            if (_taskbarList != null)
            {
                TaskbarFlash.Change(_hwnd, _urgent);
            }
        }
    }

    /// <summary>
    /// Whether or not the taskbar icon has a count visble
    /// </summary>
    public bool CountVisible
    {
        get => _countVisible;

        set
        {
            _countVisible = value;
            if (_unityLauncher != null)
            {
                _unityLauncher.CountVisible = _countVisible;
            }
            if (_taskbarList != null)
            {
                _countIconWindows?.Dispose();
                _countIconWindows = null;
                if (!_countVisible)
                {
                    _taskbarList.SetOverlayIcon(_hwnd, IntPtr.Zero, "");
                }
                else
                {
                    _countIconWindows = new System.Drawing.Bitmap(16, 16);
                    var g = System.Drawing.Graphics.FromImage(_countIconWindows);
                    g.FillEllipse(System.Drawing.Brushes.Black, new System.Drawing.Rectangle(0, 0, _countIconWindows.Width, _countIconWindows.Height));
                    var s = Count > 99 ? "99+" : Count.ToString();
                    var font = new System.Drawing.Font(System.Drawing.SystemFonts.DefaultFont.Name, Count <= 99 ? (Count > 9 ? 7.5f : 8.0f) : 7.0f);
                    var stringSize = g.MeasureString(s, font);
                    g.DrawString(s, font, System.Drawing.Brushes.White, new System.Drawing.Point(Convert.ToInt32((_countIconWindows.Width - stringSize.Width) / 2), Convert.ToInt32((_countIconWindows.Height - stringSize.Height) / 2)));
                    _taskbarList.SetOverlayIcon(_hwnd, _countIconWindows.GetHicon(), Count.ToString());
                }
            }
        }
    }

    /// <summary>
    /// Count value
    /// </summary>
    /// <remarks>Changing count value automatically sets count visible to true</remarks>
    public long Count
    {
        get => _count;

        set
        {
            _count = value;
            if (_unityLauncher != null)
            {
                _unityLauncher.Count = _count;
            }
            CountVisible = _count >= 0;
        }
    }

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
        CountVisible = false;
        ProgressState = ProgressFlags.NoProgress;
        Urgent = false;
        _dbusConnection?.Dispose();
        _countIconWindows?.Dispose();
        _disposed = true;
    }

    /// <summary>
    /// Connects to an item on the taskbar on Linux
    /// </summary>
    /// <param name="desktopFile">Desktop file name with extension</param>
    /// <exception cref="PlatformNotSupportedException">Thrown if called not on Linux</exception>
    public static async Task<TaskbarItem?> ConnectLinuxAsync(string desktopFile)
    {
        if (!RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        {
            throw new PlatformNotSupportedException();
        }
        try
        {
            var t = new TaskbarItem(desktopFile, new Connection(Address.Session));
            await t._dbusConnection!.ConnectAsync();
            await t._dbusConnection.RegisterObjectAsync(t._unityLauncher);
            return t;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return null;
        }
    }

    /// <summary>
    /// Connects to an item on the taskbar on Windows
    /// </summary>
    /// <param name="hwnd">Window handle</param>
    /// <exception cref="PlatformNotSupportedException">Thrown if called not on Windows</exception>
    /// <exception cref="ArgumentException">Thrown if hwnd is IntPtr.Zero</exception>
    public static TaskbarItem? ConnectWindows(nint hwnd)
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
        catch (Exception e)
        {
            Console.WriteLine(e);
            return null;
        }
    }
}