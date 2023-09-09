using DBus.Services.Secrets;
using Meziantou.Framework.Win32;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Nickvision.Aura.Keyring;

/// <summary>
/// Object to access system credential manager
/// </summary>
/// <remarks>Uses Windows Credential Manager on Windows and DBus Secret Service on Linux</remarks>
internal static class SystemCredentialManager
{
    private static SecretService? _service;
    private static Collection? _collection;

    /// <summary>
    /// Gets keyring's password from credential manager
    /// </summary>
    /// <param name="name">Keyring name</param>
    /// <returns>Keyring password or null if failed to get password</returns>
    public static async Task<string?> GetPasswordAsync(string name)
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            return CredentialManager.ReadCredential(name)?.Password ?? null;
        }
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        {
            var items = await GetDBusKeyringItems(name);
            if (items.Length > 0)
            {
                return Encoding.UTF8.GetString(await items[0].GetSecretAsync());
            }
            return null;
        }
        throw new PlatformNotSupportedException();
    }

    /// <summary>
    /// Sets random password for a keyring in credential manager
    /// </summary>
    /// <param name="name">Keyring name</param>
    /// <returns>Keyring password or null if failed to set password</returns>
    public static async Task<string?> SetPasswordAsync(string name)
    {
        var password = new PasswordGenerator().Next();
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            CredentialManager.WriteCredential(name, "NickvisionKeyring", password, CredentialPersistence.LocalMachine);
            return password;
        }
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        {
            var items = await GetDBusKeyringItems(name);
            if (items.Length > 0)
            {
                await items[0].SetSecret(Encoding.UTF8.GetBytes(password), "text/plain; charset=utf8");
                return password;
            }
            var lookupAttributes = new Dictionary<string, string> {{ "application", name.ToLower() }};
            await _collection!.CreateItemAsync(name, lookupAttributes, Encoding.UTF8.GetBytes(password), "text/plain; charset=utf8", false);
            return password;
        }
        throw new PlatformNotSupportedException();
    }

    /// <summary>
    /// Deletes keyring's password from credential manager
    /// </summary>
    /// <param name="name">Keyring name</param>
    public static async Task DeletePasswordAsync(string name)
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            CredentialManager.DeleteCredential(name);
            return;
        }
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        {
            var items = await GetDBusKeyringItems(name);
            if (items.Length > 0)
            {
                await items[0].SetSecret(Array.Empty<byte>(), "text/plain; charset=utf8");
            }
            return;
        }
        throw new PlatformNotSupportedException();
    }

    private static async Task<Item[]> GetDBusKeyringItems(string name)
    {
        if (_service == null)
        {
            _service = await SecretService.ConnectAsync(EncryptionType.Dh);
            _collection = await _service.GetDefaultCollectionAsync() ?? await _service.CreateCollectionAsync("Default keyring", "default");
        }
        if (_collection == null)
        {
            throw new AuraException("Failed to get or create default collection in system keyring.");
        }
        await _collection.UnlockAsync();
        var lookupAttributes = new Dictionary<string, string> {{ "application", name.ToLower() }};
        return await _collection.SearchItemsAsync(lookupAttributes);
    }
}