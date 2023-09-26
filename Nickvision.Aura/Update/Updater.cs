using Nickvision.Aura.Network;
using System;

namespace Nickvision.Aura.Update;

/// <summary>
/// An object to check for application updates
/// </summary>
public class Updater
{
    private AppInfo _appInfo;

    /// <summary>
    /// Constructs an Updater
    /// </summary>
    /// <param name="appInfo">AppInfo</param>
    public Updater(AppInfo appInfo)
    {
        _appInfo = appInfo;
        var task = appInfo.SourceRepo.GetIsValidWebsiteAsync();
        task.Wait();
        if(!task.Result)
        {
            throw new ArgumentException("The SourceRepo of the AppInfo is invalid.");
        }
        if (_appInfo.SourceRepo.Host.ToLower() != "github.com")
        {
            throw new ArgumentException("The Updater only supports GitHub repos.");
        }
    }
}
