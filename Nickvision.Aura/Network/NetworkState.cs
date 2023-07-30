namespace Nickvision.Aura.Network;

/// <summary>
/// Network state, as returned by NetworkManager
/// </summary>
public enum NetworkState : uint
{
    Unknown = 0,
    Asleep = 10,
    Disconnect = 20,
    Disconnecting = 30,
    Connecting = 40,
    ConnectedLocal = 50,
    ConnectedSite = 60,
    ConnectedGlobal = 70
}