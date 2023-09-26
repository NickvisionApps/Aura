using Nickvision.Aura.Network;
using Octokit;
using System;
using System.Threading.Tasks;

namespace Nickvision.Aura.Update;

public enum VersionType
{
    Stable,
    Preview
}

/// <summary>
/// An object to check for application updates
/// </summary>
public class Updater
{
    private readonly AppInfo _appInfo;
    private readonly GitHubClient _github;

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
        _github = new GitHubClient(new ProductHeaderValue("Nickvision.Aura"));
    }

    /// <summary>
    /// Gets the current stable version from the source repo
    /// </summary>
    /// <returns>The Version object representing the current stable version. Null if error</returns>
    public async Task<Version?> GetCurrentStableVersionAsync() => await GetCurrentVersionAsync(VersionType.Stable);

    /// <summary>
    /// Gets the current preview version from the source repo
    /// </summary>
    /// <returns>The Version object representing the current preview version. Null if error</returns>
    public async Task<Version?> GetCurrentPreviewVersionAsync() => await GetCurrentVersionAsync(VersionType.Preview);

    /// <summary>
    /// Gets the current type version from the source repo
    /// </summary>
    /// <param name="versionType">VersionType</param>
    /// <returns>The Version object representing the current type version. Null if error</returns>
    private async Task<Version?> GetCurrentVersionAsync(VersionType versionType)
    {
        var repoFields = _appInfo.SourceRepo.ToString().Split('/');
        var owner = repoFields[3];
        var name = repoFields[4];
        var releases = await _github.Repository.Release.GetAll(owner, name);
        Release? latest = null;
        foreach (var release in releases)
        {
            if((versionType == VersionType.Stable && !release.Prerelease) || versionType == VersionType.Preview && release.Prerelease)
            {
                if (latest == null || (latest != null && release.CreatedAt > latest.CreatedAt))
                {
                    latest = release;
                }
            }
        }
        if(latest != null)
        {
            try
            {
                return new Version(latest.TagName);
            }
            catch
            {
                return null;
            }
        }
        return null;
    }
}
