using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace Nickvision.Aura.Keyring;

[Flags]
public enum PasswordContent
{
    Numeric = 1,
    Uppercase = 2,
    Lowercase = 4,
    Special = 8
}

/// <summary>
/// Random password generator
/// </summary>
public class PasswordGenerator
{
    private readonly List<char> _chars;
    private PasswordContent _contentFlags;
    
    /// <summary>
    /// Construct password generator
    /// </summary>
    /// <param name="contentFlags">Flags to determine possible characters in password</param>
    public PasswordGenerator(PasswordContent contentFlags = PasswordContent.Numeric | PasswordContent.Uppercase | PasswordContent.Lowercase | PasswordContent.Special)
    {
        _chars = new();
        SetContentFlags(contentFlags);
    }
    
    /// <summary>
    /// Set possible characters for already existing PasswordGenerator
    /// </summary>
    /// <param name="contentFlags">Flags to determine possible characters in password</param>
    public void SetContentFlags(PasswordContent contentFlags)
    {
        _contentFlags = contentFlags;
        if (_contentFlags.HasFlag(PasswordContent.Numeric))
        {
            _chars.AddRange(new []{ '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' });
        }
        if (_contentFlags.HasFlag(PasswordContent.Uppercase))
        {
            _chars.AddRange(new []
            {
                'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M',
                'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z'
            });
        }
        if (_contentFlags.HasFlag(PasswordContent.Lowercase))
        {
            _chars.AddRange(new []
            {
                'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm',
                'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z'
            });
        }
        if (_contentFlags.HasFlag(PasswordContent.Special))
        {
            _chars.AddRange(new []
            {
                '!', '"', '#', '$', '%', '&', '\'', '(', ')', '*', '+', ',', '-', '.', '/', ':',
                ';', '<', '=', '>', '?', '@', '[', '\\', ']', '^', '_', '`', '{', '|', '}', '~'
            });
        }
    }
    
    /// <summary>
    /// Generate new password
    /// </summary>
    /// <param name="length">Password length</param>
    /// <returns>A new random password</returns>
    public string Next(int length = 16)
    {
        var result = new StringBuilder();
        while (result.Length < length)
        {
            result.Append(_chars[RandomNumberGenerator.GetInt32(_chars.Count)]);
        }
        return result.ToString();
    }
}