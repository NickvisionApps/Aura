using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Tmds.DBus;

namespace Nickvision.Aura.Network;

[DBusInterface("org.freedesktop.portal.NetworkMonitor")]
internal interface INetworkMonitor : IDBusObject
{
    Task<bool> GetAvailableAsync();
    Task<bool> GetMeteredAsync();
    Task<uint> GetConnectivityAsync();
    Task<IDictionary<string, object>> GetStatusAsync();
    Task<bool> CanReachAsync(string Hostname, uint Port);
    Task<IDisposable> WatchchangedAsync(Action handler);
    Task<T> GetAsync<T>(string prop);
    Task<NetworkMonitorProperties> GetAllAsync();
    Task SetAsync(string prop, object val);
    Task<IDisposable> WatchPropertiesAsync(Action<PropertyChanges> handler);
}

[Dictionary]
internal class NetworkMonitorProperties
{
    private uint _version = default(uint);

    public uint Version
    {
        get
        {
            return _version;
        }

        set
        {
            _version = value;
        }
    }
}

internal static class NetworkMonitorExtensions
{
    public static Task<uint> GetVersionAsync(this INetworkMonitor o) => await o.GetAsync<uint>("version");
}