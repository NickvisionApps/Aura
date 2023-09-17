using Tmds.DBus;

namespace Nickvision.Aura.Taskbar;

/// <summary>
/// Object containing properties of LauncherEntry, marshalled as DBus dictionary of a{sv}
/// </summary>
[Dictionary]
internal class LauncherProperties
{
    /// <summary>
    /// A number to display on the launcher icon
    /// </summary>
    public int Count;
    /// <summary>
    /// Whether or not the count is visible
    /// </summary>
    public bool CountVisible;
    /// <summary>
    /// Value for the progress bar on the launcher icon
    /// </summary>
    /// <remarks>A number between 0 and 1</remarks>
    public double Progress;
    /// <summary>
    /// Whether or not the progress bar is visible
    /// </summary>
    public bool ProgressVisible;
    /// <summary>
    /// Tells the launcher to get the user's attention
    /// </summary>
    public bool Urgent;
    /// <summary>
    /// The object path to a DbusmenuServer instance on the emitting process
    /// </summary>
    public string Quicklist = "";
}