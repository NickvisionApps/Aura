using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Tmds.DBus;

namespace Nickvision.Aura.Taskbar;

/// <summary>
/// Unity LauncherEntry DBus interface
/// </summary>
[DBusInterface("com.canonical.Unity.LauncherEntry")]
internal interface ILauncherEntry : IDBusObject
{
    Task<IDisposable> WatchUpdateAsync(Action<(string appUri, IDictionary<string, object> properties)> handler);
    Task<IDictionary<string, object>> QueryAsync();
}