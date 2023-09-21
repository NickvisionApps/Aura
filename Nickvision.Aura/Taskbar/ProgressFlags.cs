namespace Nickvision.Aura.Taskbar;

/// <summary>
/// Flags that control the current state of the progress button.
/// Specify only one of the following flags; all states are mutually exclusive of all others.
/// </summary>
public enum ProgressFlags {
    NoProgress = 0,
    Indeterminate = 0x1,
    Normal = 0x2,
    Error = 0x4,
    Paused = 0x8
}