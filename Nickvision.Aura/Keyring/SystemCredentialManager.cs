using Meziantou.Framework.Win32;
using System;
using System.Runtime.InteropServices;

namespace Nickvision.Aura.Keyring;

/// <summary>
/// Object to access system credential manager
/// </summary>
/// <remarks>Uses Windows Credential Manager on Windows and LibSecret on Linux</remarks>
public static partial class SystemCredentialManager
{
    [LibraryImport("libsecret-1.so.0", StringMarshalling = StringMarshalling.Utf8)]
    private static partial nint secret_schema_new(string name, int flags, nint args);
    [LibraryImport("libsecret-1.so.0", StringMarshalling = StringMarshalling.Utf8)]
    private static partial string secret_password_lookup_sync(nint schema, nint cancellable, nint error, nint args);
    [LibraryImport("libsecret-1.so.0", StringMarshalling = StringMarshalling.Utf8)]
    [return:MarshalAs(UnmanagedType.I1)]
    private static partial bool secret_password_store_sync(nint schema, nint collection, string label, string password, nint cancellable, nint error, nint args);
    [LibraryImport("libsecret-1.so.0", StringMarshalling = StringMarshalling.Utf8)]
    [return:MarshalAs(UnmanagedType.I1)]
    private static partial bool secret_password_clear_sync(nint schema, nint cancellable, nint error, nint args);
    [LibraryImport("libsecret-1.so.0")]
    private static partial void secret_schema_unref(nint schema);

    private const int SECRET_SCHEMA_NONE = 0;

    /// <summary>
    /// Get keyring's password from credential manager
    /// </summary>
    /// <param name="name">Keyring name</param>
    /// <returns>Keyring password or null if failed to get password</returns>
    public static string? GetPassword(string name)
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            return CredentialManager.ReadCredential(name)?.Password ?? null;
        }
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        {
            var schema = secret_schema_new(name, SECRET_SCHEMA_NONE, IntPtr.Zero);
            var password = secret_password_lookup_sync(schema, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero);
            secret_schema_unref(schema);
            return password;
        }
        throw new PlatformNotSupportedException();
    }

    /// <summary>
    /// Set random password for a keyring in credential manager
    /// </summary>
    /// <param name="name">Keyring name</param>
    /// <returns>Keyring password or null if failed to set password</returns>
    public static string? SetPassword(string name)
    {
        var password = new PasswordGenerator().Next();
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            CredentialManager.WriteCredential(name, "NickvisionKeyring", password, CredentialPersistence.LocalMachine);
            return password;
        }
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        {
            var schema = secret_schema_new(name, SECRET_SCHEMA_NONE, IntPtr.Zero);
            var success = secret_password_store_sync(schema, IntPtr.Zero, name, password, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero);
            secret_schema_unref(schema);
            if (!success)
            {
                return null;
            }
            return password;
        }
        throw new PlatformNotSupportedException();
    }

    /// <summary>
    /// Delete keyring's password from credential manager
    /// </summary>
    /// <param name="name">Keyring name</param>
    public static void DeletePassword(string name)
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            CredentialManager.DeleteCredential(name);
            return;
        }
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        {
            var schema = secret_schema_new(name, SECRET_SCHEMA_NONE, IntPtr.Zero);
            secret_password_clear_sync(schema, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero);
            secret_schema_unref(schema);
            return;
        }
        throw new PlatformNotSupportedException();
    }
}