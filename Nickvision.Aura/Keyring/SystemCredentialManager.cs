using Meziantou.Framework.Win32;
using System;
using System.Runtime.InteropServices;

namespace Nickvision.Aura.Keyring;

/// <summary>
/// Object to access system credential manager
/// </summary>
/// <remarks>Uses Windows Credential Manager on Windows and LibSecret on Linux</remarks>
public static class SystemCredentialManager
{
    /// <summary>
    /// Get password from credential manager
    /// </summary>
    /// <param name="name">Keyring name</param>
    /// <returns>Keyring password or null</returns>
    public static string? GetPassword(string name)
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            return CredentialManager.ReadCredential(name)?.Password ?? null;
        }
        // TODO: Linux
        throw new PlatformNotSupportedException();
    }

    /// <summary>
    /// Set random password for a keyring in credential manager
    /// </summary>
    /// <param name="name">Keyring name</param>
    /// <returns>Keyring password or null</returns>
    public static string SetPassword(string name)
    {
        var password = new PasswordGenerator().Next();
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            CredentialManager.WriteCredential(name, "NickvisionKeyring", password, CredentialPersistence.LocalMachine);
            return password;
        }
        // TODO: Linux
        throw new PlatformNotSupportedException();
    }
}