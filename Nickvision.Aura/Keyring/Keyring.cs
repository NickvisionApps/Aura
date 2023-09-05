using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Nickvision.Aura.Keyring;

/// <summary>
/// The Keyring object
/// </summary>
public class Keyring : IDisposable
{
    private bool _disposed;
    private readonly Store _store;
    
    /// <summary>
    /// The name of the Keyring
    /// </summary>
    public string Name => _store.Name;

    /// <summary>
    /// Constructs a Keyring
    /// </summary>
    /// <param name="store">The Keyring's store</param>
    private Keyring(Store store)
    {
        _disposed = false;
        _store = store;
    }

    /// <summary>
    /// Accesses a Keyring. The Keyring will first attempt to load the Store. If the Store doesn't exist, it will create a new Store.
    /// </summary>
    /// <param name="name">The name of the Store</param>
    /// <param name="password">The password of the Store, or null to use password from system's credential manager</param>
    /// <returns>The Keyring. If the Keyring exists and cannot be loaded, null will be returned</returns>
    /// <remarks>If password is null, Windows Credential Manager will be used on Windows or LibSecret will be used on
    /// Linux to get password for the keyring. If a new Store will be created, it will be encrypted with random password.</remarks>
    public static Keyring? Access(string name, string? password = null)
    {
        password ??= SystemCredentialManager.GetPassword(name);
        if (password != null)
        {
            try
            {
                return new Keyring(Store.Load(name, password));
            }
            catch (FileNotFoundException) { }
        }
        try
        {
            password ??= SystemCredentialManager.SetPassword(name);
            return new Keyring(Store.Create(name, password, false));
        }
        catch
        {
            return null;
        }
    }

    /// <summary>
    /// Gets whether or not a Keyring exists
    /// </summary>
    /// <param name="name">The name of the Keyring</param>
    /// <returns>True if exists, else false</returns>
    public static bool Exists(string name) => Store.Exists(name);

    /// <summary>
    /// Destroys a Keyring by name
    /// </summary>
    /// <param name="name">The name of the Keyring</param>
    /// <returns>True if destroyed, else false</returns>
    public static bool Destroy(string name) => Store.Destroy(name);

    /// <summary>
    /// Frees resources used by the Keyring object
    /// </summary>
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Frees resources used by the Keyring object
    /// </summary>
    protected virtual void Dispose(bool disposing)
    {
        if (_disposed)
        {
            return;
        }
        if (disposing)
        {
            _store.Dispose();
        }
        _disposed = true;
    }

    /// <summary>
    /// Destroys the Keyring and all its data, including the store. Once this method is called, this object should not be used anymore.
    /// </summary>
    /// <returns>True if successful, else false</returns>
    public bool Destroy()
    {
        if(_store.Destroy())
        {
            Dispose();
            return true;
        }
        return false;
    }

    /// <summary>
    /// Gets all credentials from the Keyring
    /// </summary>
    /// <returns>The list of Credential objects</returns>
    public async Task<List<Credential>> GetAllCredentialsAsync() => await _store.GetAllCredentialsAsync();

    /// <summary>
    /// Lookups a credential by id
    /// </summary>
    /// <param name="id">The id of the credential</param>
    /// <returns>The Credential object if found, else null</returns>
    public async Task<Credential?> LookupCredentialAsync(int id) => await _store.LookupCredentialAsync(id);

    /// <summary>
    /// Lookups credentials by name
    /// </summary>
    /// <param name="name">The name of the credentials to find</param>
    /// <returns>The list of Credential objects found</returns>
    public async Task<List<Credential>> LookupCredentialsAsync(string name) => await _store.LookupCredentialsAsync(name);

    /// <summary>
    /// Adds a Credential to the Keyring
    /// </summary>
    /// <param name="credential">The Credential object to add</param>
    /// <returns>True if successful, else false</returns>
    public async Task<bool> AddCredentialAsync(Credential credential) => await _store.AddCredentialAsync(credential);

    /// <summary>
    /// Updates a Credential in the Keyring
    /// </summary>
    /// <param name="credential">The Credential object to update</param>
    /// <returns>True if successful, else false</returns>
    public async Task<bool> UpdateCredentialAsync(Credential credential) => await _store.UpdateCredentialAsync(credential);

    /// <summary>
    /// Removes a Credential from the Keyring
    /// </summary>
    /// <param name="id">The id of the credential to remove</param>
    /// <returns>True if successful, else false</returns>
    public async Task<bool> DeleteCredentialAsync(int id) => await _store.DeleteCredentialAsync(id);
}