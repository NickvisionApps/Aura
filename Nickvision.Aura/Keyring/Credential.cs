using System;

namespace Nickvision.Aura.Keyring;
    
/// <summary>
/// A model of a credential stored in the keyring
/// </summary>    
public class Credential : IComparable<Credential>, IEquatable<Credential>
{
    /// <summary>
    /// The id of the credential
    /// </summary>    
    public int Id { get; init; }
    /// <summary>
    /// The name of the credential
    /// </summary>    
    public string Name { get; set; }
    /// <summary>
    /// The uri of the credential
    /// </summary>    
    public Uri? Uri { get; set; }
    /// <summary>
    /// The username of the credential
    /// </summary>    
    public string Username { get; set; }
    /// <summary>
    /// The password of the credential
    /// </summary>    
    public string Password { get; set; }
    
    /// <summary>
    /// Constructs a Credential
    /// </summary>
    /// <param name="name">The name of the credential</param>
    /// <param name="uri">The uri of the credential</param>
    /// <param name="username">The username of the credential</param>
    /// <param name="password">The password of the credential</param>
    public Credential(string name, Uri? uri, string username, string password)
    {
        Id = Guid.NewGuid().GetHashCode();
        Name = name;
        Uri = uri;
        Username = username;
        Password = password;
    }
    
    /// <summary>
    /// Constructs a Credential
    /// </summary>
    /// <param name="id">The id of the credential</param>
    /// <param name="name">The name of the credential</param>
    /// <param name="uri">The uri of the credential</param>
    /// <param name="username">The username of the credential</param>
    /// <param name="password">The password of the credential</param>
    internal Credential(int id, string name, Uri? uri, string username, string password)
    {
        Id = id;
        Uri = uri;
        Name = name;
        Username = username;
        Password = password;
    }

    /// <summary>
    /// Compares this with other
    /// </summary>
    /// <param name="other">The Credential object to compare to</param>
    /// <returns>-1 if this is less than other. 0 if this is equal to other. 1 if this is greater than other</returns>
    /// <exception cref="NullReferenceException">Thrown if other is null</exception>
    public int CompareTo(Credential? other)
    {
        if (other == null)
        {
            throw new NullReferenceException();
        }
        if (this < other)
        {
            return -1;
        }
        else if (this == other)
        {
            return 0;
        }
        else
        {
            return 1;
        }
    }

    /// <summary>
    /// Gets whether or not an object is equal to this Credential
    /// </summary>
    /// <param name="obj">The object to compare</param>
    /// <returns>True if equals, else false</returns>
    public override bool Equals(object? obj)
    {
        if (obj is Credential toCompare)
        {
            return Id == toCompare.Id;
        }
        return false;
    }

    /// <summary>
    /// Gets whether or not an object is equal to this Credential
    /// </summary>
    /// <param name="obj">The Credential? object to compare</param>
    /// <returns>True if equals, else false</returns>
    public bool Equals(Credential? obj) => Equals((object?)obj);

    /// <summary>
    /// Gets a hash code for the object
    /// </summary>
    /// <returns>The hash code for the object</returns>
    public override int GetHashCode() => Id.GetHashCode();

    /// <summary>
    /// Compares two Credential objects by ==
    /// </summary>
    /// <param name="a">The first Credential object</param>
    /// <param name="b">The second Credential object</param>
    /// <returns>True if a == b, else false</returns>
    public static bool operator ==(Credential? a, Credential? b) => a?.Id == b?.Id;

    /// <summary>
    /// Compares two Credential objects by !=
    /// </summary>
    /// <param name="a">The first Credential object</param>
    /// <param name="b">The second Credential object</param>
    /// <returns>True if a != b, else false</returns>
    public static bool operator !=(Credential? a, Credential? b) => a?.Id != b?.Id;

    /// <summary>
    /// Compares two Credential objects by >
    /// </summary>
    /// <param name="a">The first Credential object</param>
    /// <param name="b">The second Credential object</param>
    /// <returns>True if a > b, else false</returns>
    public static bool operator <(Credential? a, Credential? b) => a?.Id < b?.Id;

    /// <summary>
    /// Compares two Credential objects by <
    /// </summary>
    /// <param name="a">The first Credential object</param>
    /// <param name="b">The second Credential object</param>
    /// <returns>True if a < b, else false</returns>
    public static bool operator >(Credential? a, Credential? b) => a?.Id > b?.Id;
}