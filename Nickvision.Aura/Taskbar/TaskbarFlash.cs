using System;
using System.Runtime.InteropServices;

namespace Nickvision.Aura.Taskbar;

/// <summary>
/// Helper to change taskbar icon flashing on Windows
/// </summary>
internal partial class TaskbarFlash
{
    [LibraryImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static partial bool FlashWindowEx(ref FlashInfo pwfi);
    
    //Stop flashing. The system restores the window to its original state. 
    public const UInt32 FLASHW_STOP = 0;
    //Flash the taskbar button. 
    public const UInt32 FLASHW_TRAY = 2; 
    //Flash continuously, until the FLASHW_STOP flag is set.
    public const UInt32 FLASHW_TIMER = 4;
    
    public static void Change(nint hwnd, bool state)
    {
        var fInfo = new FlashInfo
        {
            hwnd = hwnd,
            dwFlags = state ? (FLASHW_TRAY | FLASHW_TIMER) : FLASHW_STOP,
            uCount = UInt32.MaxValue,
            dwTimeout = 0
        };
        fInfo.cbSize = Convert.ToUInt32(Marshal.SizeOf(fInfo));
        FlashWindowEx(ref fInfo);
    }
}