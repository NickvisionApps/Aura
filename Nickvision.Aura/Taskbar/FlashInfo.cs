using System.Runtime.InteropServices;

namespace Nickvision.Aura.Taskbar;

[StructLayout(LayoutKind.Sequential)]
internal struct FlashInfo
{
    public uint cbSize;
    public nint hwnd;
    public uint dwFlags;
    public uint uCount;
    public uint dwTimeout;
}