using System;
using System.Threading.Tasks;
using Tmds.DBus;

namespace Nickvision.Aura.Network;

/// <summary>
/// NetworkManager DBus interface
/// </summary>
[DBusInterface("org.freedesktop.NetworkManager")]
internal interface INetworkManager : IDBusObject
{
    public Task<T> GetAsync<T>(string prop);
    public Task<IDisposable> WatchStateChangedAsync(Action<NetworkState> handler);
}