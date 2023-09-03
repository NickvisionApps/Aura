using System;

namespace Nickvision.Aura;

/// <summary>
/// Aura standard exception
/// </summary>
public class AuraException : Exception
{
    /// <summary>
    /// Construct Aura Exception
    /// </summary>
    public AuraException(string message) : base(message)
    {
    }
}