using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Threading.Tasks;

namespace Nickvision.Aura.Keyring;

/// <summary>
/// A store object for credentials. Backed by SQLCipher
/// </summary>
internal class Store : IDisposable
{
    /// <summary>
    /// The directory to store Stores
    /// </summary>
    public static readonly string StoreDir = $"{UserDirectories.Config}{Path.DirectorySeparatorChar}Nickvision{Path.DirectorySeparatorChar}Keyring";

    private bool _disposed;
    private readonly SqliteConnection _database;

    /// <summary>
    /// The name of the Store
    /// </summary>
    public string Name { get; init; }

    /// <summary>
    /// The location of the Store on disk
    /// </summary>
    public string Location => $"{StoreDir}{Path.DirectorySeparatorChar}{Name}.nring";

    /// <summary>
    /// Constructs a Store
    /// </summary>
    /// <param name="name">The name of the Store</param>
    /// <param name="database">The database connection for the store</param>
    private Store(string name, SqliteConnection database)
    {
        Name = name;
        _disposed = false;
        _database = database;
        _database.Open();
        using var cmdTableCredentials = _database!.CreateCommand();
        cmdTableCredentials.CommandText = "CREATE TABLE IF NOT EXISTS credentials (id TEXT PRIMARY KEY, name TEXT, uri TEXT, username TEXT, password TEXT)";
        cmdTableCredentials.ExecuteNonQuery();
        _database.Close();
    }

    /// <summary>
    /// Creates a new Store
    /// </summary>
    /// <param name="name">The name of the Store</param>
    /// <param name="password">The password to use to encrypt the Store</param>
    /// <param name="overwrite">Whether or not to overwrite an existing Store at the path</param>
    /// <exception cref="ArgumentException">Thrown if the name or password is empty</exception>
    /// <exception cref="IOException">Thrown if a Store exists at the provided path and overwrite is false</exception>
    /// <returns>The new Store object</returns>
    public static Store Create(string name, string password, bool overwrite)
    {
        Directory.CreateDirectory(StoreDir);
        if (string.IsNullOrEmpty(name))
        {
            throw new ArgumentException("The name to create a Store must not be empty.");
        }
        if (string.IsNullOrEmpty(password))
        {
            throw new ArgumentException("The password to create a Store must not be empty.");
        }
        var path = $"{StoreDir}{Path.DirectorySeparatorChar}{name}.nring";
        if (File.Exists(path))
        {
            if (overwrite)
            {
                File.Delete(path);
            }
            else
            {
                throw new IOException("A Store already exists with the provided name.");
            }
        }
        return new Store(name, new SqliteConnection(new SqliteConnectionStringBuilder()
        {
            DataSource = path,
            Mode = SqliteOpenMode.ReadWriteCreate,
            Pooling = false,
            Password = password
        }.ConnectionString));
    }

    /// <summary>
    /// Loads an existing Store
    /// </summary>
    /// <param name="name">The name of the Store</param>
    /// <param name="password">The password to use to access the Store</param>
    /// <exception cref="ArgumentException">Thrown if the name or password is empty</exception>
    /// <exception cref="FileNotFoundException">Thrown if a Store does not exist at the provided path</exception>
    /// <exception cref="IOException">Thrown if the Store connection cannot be established</exception>
    /// <returns>The loaded Store object</returns>
    public static Store Load(string name, string password)
    {
        if (string.IsNullOrEmpty(name))
        {
            throw new ArgumentException("The name to create a Store must not be empty.");
        }
        if (string.IsNullOrEmpty(password))
        {
            throw new ArgumentException("The password to create a Store must not be empty.");
        }
        var path = $"{StoreDir}{Path.DirectorySeparatorChar}{name}.nring";
        if (!File.Exists(path))
        {
            throw new FileNotFoundException("A Store is not found with the provided name.");
        }
        try
        {
            return new Store(name, new SqliteConnection(new SqliteConnectionStringBuilder()
            {
                DataSource = path,
                Mode = SqliteOpenMode.ReadWrite,
                Pooling = false,
                Password = password
            }.ConnectionString));
        }
        catch
        {
            throw new IOException("Unable to access the Store. Make sure the password is correct.");
        }
    }

    /// <summary>
    /// Gets whether or not a Store exists
    /// </summary>
    /// <param name="name">The name of the Store</param>
    /// <returns>True if exists, else false</returns>
    public static bool Exists(string name) => File.Exists($"{StoreDir}{Path.DirectorySeparatorChar}{name}.nring");

    /// <summary>
    /// Destroys a Store by name
    /// </summary>
    /// <param name="name">The name of the Store</param>
    /// <returns>True if destroyed, else false</returns>
    public static bool Destroy(string name)
    {
        if (File.Exists($"{StoreDir}{Path.DirectorySeparatorChar}{name}.nring"))
        {
            File.Delete($"{StoreDir}{Path.DirectorySeparatorChar}{name}.nring");
            return true;
        }
        return false;
    }

    /// <summary>
    /// Frees resources used by the Store object
    /// </summary>
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Frees resources used by the Store object
    /// </summary>
    protected virtual void Dispose(bool disposing)
    {
        if (_disposed)
        {
            return;
        }
        if (disposing)
        {
            if (_database.State == ConnectionState.Open)
            {
                _database.Close();
            }
            _database.Dispose();
        }
        _disposed = true;
    }

    /// <summary>
    /// Destroys the Store and all its data from disk. Once this method is called, this object should not be used anymore.
    /// </summary>
    /// <returns>True if successful, else false</returns>
    public bool Destroy()
    {
        Dispose();
        try
        {
            File.Delete(Location);
            return true;
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// Gets all credentials from the Store
    /// </summary>
    /// <returns>The list of Credential objects</returns>
    public async Task<List<Credential>> GetAllCredentialsAsync()
    {
        await _database.OpenAsync();
        using var cmdQueryCredentials = _database.CreateCommand();
        cmdQueryCredentials.CommandText = "SELECT * FROM credentials";
        using var readQueryCredentials = await cmdQueryCredentials.ExecuteReaderAsync();
        var credentials = new List<Credential>();
        while (await readQueryCredentials.ReadAsync())
        {
            var id = readQueryCredentials.GetInt32(0);
            var name = readQueryCredentials.GetString(1);
            var uri = readQueryCredentials.IsDBNull(2) ? null : string.IsNullOrEmpty(readQueryCredentials.GetString(2)) ? null : new Uri(readQueryCredentials.GetString(2));
            var username = readQueryCredentials.IsDBNull(3) ? "" : readQueryCredentials.GetString(3);
            var password = readQueryCredentials.IsDBNull(4) ? "" : readQueryCredentials.GetString(4);
            credentials.Add(new Credential(id, name, uri, username, password));
        }
        await _database.CloseAsync();
        return credentials;
    }

    /// <summary>
    /// Lookups a credential by id
    /// </summary>
    /// <param name="id">The id of the credential</param>
    /// <returns>The Credential object if found, else null</returns>
    public async Task<Credential?> LookupCredentialAsync(int id)
    {
        await _database.OpenAsync();
        using var cmdQueryCredential = _database.CreateCommand();
        cmdQueryCredential.CommandText = "SELECT * FROM credentials where id = $id";
        cmdQueryCredential.Parameters.AddWithValue("$id", id);
        using var readQueryCredential = await cmdQueryCredential.ExecuteReaderAsync();
        Credential? credential = null;
        if (readQueryCredential.HasRows)
        {
            await readQueryCredential.ReadAsync();
            var i = readQueryCredential.GetInt32(0);
            var name = readQueryCredential.GetString(1);
            var uri = readQueryCredential.IsDBNull(2) ? null : string.IsNullOrEmpty(readQueryCredential.GetString(2)) ? null : new Uri(readQueryCredential.GetString(2));
            var username = readQueryCredential.IsDBNull(3) ? "" : readQueryCredential.GetString(3);
            var password = readQueryCredential.IsDBNull(4) ? "" : readQueryCredential.GetString(4);
            credential = new Credential(i, name, uri, username, password);
        }
        await _database.CloseAsync();
        return credential;
    }

    /// <summary>
    /// Lookups credentials by name
    /// </summary>
    /// <param name="name">The name of the credentials to find</param>
    /// <returns>The list of Credential objects found</returns>
    public async Task<List<Credential>> LookupCredentialsAsync(string name)
    {
        await _database.OpenAsync();
        using var cmdQueryCredentials = _database.CreateCommand();
        cmdQueryCredentials.CommandText = "SELECT * FROM credentials where name = $name";
        cmdQueryCredentials.Parameters.AddWithValue("$name", name);
        using var readQueryCredentials = await cmdQueryCredentials.ExecuteReaderAsync();
        var credentials = new List<Credential>();
        while (await readQueryCredentials.ReadAsync())
        {
            var id = readQueryCredentials.GetInt32(0);
            var n = readQueryCredentials.GetString(1);
            var uri = readQueryCredentials.IsDBNull(2) ? null : string.IsNullOrEmpty(readQueryCredentials.GetString(2)) ? null : new Uri(readQueryCredentials.GetString(2));
            var username = readQueryCredentials.IsDBNull(3) ? "" : readQueryCredentials.GetString(3);
            var password = readQueryCredentials.IsDBNull(4) ? "" : readQueryCredentials.GetString(4);
            credentials.Add(new Credential(id, n, uri, username, password));
        }
        await _database.CloseAsync();
        return credentials;
    }

    /// <summary>
    /// Adds a Credential to the store
    /// </summary>
    /// <param name="credential">The Credential object to add</param>
    /// <returns>True if successful, else false</returns>
    public async Task<bool> AddCredentialAsync(Credential credential)
    {
        await _database.OpenAsync();
        using var cmdAddCredential = _database.CreateCommand();
        cmdAddCredential.CommandText = "INSERT INTO credentials (id, name, uri, username, password) VALUES ($id, $name, $uri, $username, $password)";
        cmdAddCredential.Parameters.AddWithValue("$id", credential.Id);
        cmdAddCredential.Parameters.AddWithValue("$name", credential.Name);
        cmdAddCredential.Parameters.AddWithValue("$uri", credential.Uri == null ? "" : credential.Uri.ToString());
        cmdAddCredential.Parameters.AddWithValue("$username", credential.Username);
        cmdAddCredential.Parameters.AddWithValue("$password", credential.Password);
        var result = await cmdAddCredential.ExecuteNonQueryAsync() > 0;
        await _database.CloseAsync();
        return result;
    }

    /// <summary>
    /// Updates a Credential in the Store
    /// </summary>
    /// <param name="credential">The Credential object to update</param>
    /// <returns>True if successful, else false</returns>
    public async Task<bool> UpdateCredentialAsync(Credential credential)
    {
        await _database.OpenAsync();
        using var cmdUpdateCredential = _database.CreateCommand();
        cmdUpdateCredential.CommandText = "UPDATE credentials SET name = $name, uri = $uri, username = $username, password = $password where id = $id";
        cmdUpdateCredential.Parameters.AddWithValue("$name", credential.Name);
        cmdUpdateCredential.Parameters.AddWithValue("$uri", credential.Uri == null ? "" : credential.Uri.ToString());
        cmdUpdateCredential.Parameters.AddWithValue("$username", credential.Username);
        cmdUpdateCredential.Parameters.AddWithValue("$password", credential.Password);
        cmdUpdateCredential.Parameters.AddWithValue("$id", credential.Id);
        var result = await cmdUpdateCredential.ExecuteNonQueryAsync() > 0;
        await _database.CloseAsync();
        return true;
    }

    /// <summary>
    /// Removes a Credential from the store
    /// </summary>
    /// <param name="id">The id of the credential to remove</param>
    /// <returns>True if successful, else false</returns>
    public async Task<bool> DeleteCredentialAsync(int id)
    {
        await _database.OpenAsync();
        using var cmdDeleteCredential = _database.CreateCommand();
        cmdDeleteCredential.CommandText = "DELETE FROM credentials WHERE id = $id";
        cmdDeleteCredential.Parameters.AddWithValue("$id", id);
        var result = await cmdDeleteCredential.ExecuteNonQueryAsync() > 0;
        await _database.CloseAsync();
        return result;
    }
}