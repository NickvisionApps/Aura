using Nickvision.Aura.Network;
using Octokit;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
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
    private readonly string _repoOwner;
    private readonly string _repoName;
    private readonly GitHubClient _github;
    private int? _latestReleaseId;

    /// <summary>
    /// Constructs an Updater
    /// </summary>
    internal Updater()
    {
        var repoFields = Aura.Active.AppInfo.SourceRepo.ToString().Split('/');
        _repoOwner = repoFields[3];
        _repoName = repoFields[4];
        _github = new GitHubClient(new ProductHeaderValue("Nickvision.Aura"));
        _latestReleaseId = null;
    }

    /// <summary>
    /// Create and setup new Updater
    /// </summary>
    /// <returns>Updater</returns>
    public static async Task<Updater?> NewAsync()
    {
        var valid = await Aura.Active.AppInfo.SourceRepo.GetIsValidWebsiteAsync();
        if (!valid)
        {
            throw new ArgumentException("The SourceRepo of the active AppInfo is invalid.");
        }
        if (Aura.Active.AppInfo.SourceRepo.Host.ToLower() != "github.com")
        {
            throw new ArgumentException("The Updater only supports GitHub repos.");
        }
        return new Updater();
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
    /// Downloads and installs an application update for Windows
    /// </summary>
    /// <param name="versionType">VersionType</param>
    /// <returns>True if successful, else false</returns>
    /// <remarks>GetCurrentStableVersionAsync or GetCurrentPreviewVersionAsync should be called first before running this method</remarks>
    /// <remarks>Will force quit the current running app to install the update</remarks>
    public async Task<bool> WindowsUpdateAsync(VersionType versionType)
    {
        if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows) || _latestReleaseId == null)
        {
            return false;
        }
        var release = (await _github.Repository.Release.GetAll(_repoOwner, _repoName)).FirstOrDefault(x => x.Id == _latestReleaseId);
        if (release != null)
        {
            ReleaseAsset? asset = null;
            foreach (var a in release.Assets)
            {
                if (a.Name.ToLower().EndsWith("setup.exe"))
                {
                    asset = a;
                    break;
                }
            }
            if (asset != null)
            {
                var path = $"{UserDirectories.ApplicationCache}{Path.DirectorySeparatorChar}{asset.Name}";
                if (await WebHelpers.Client.GetFileAsync(asset.BrowserDownloadUrl, path))
                {
                    Process.Start(path);
                    Environment.Exit(0);
                    return true;
                }
            }
        }
        return false;
    }

    /// <summary>
    /// Gets the current type version from the source repo
    /// </summary>
    /// <param name="versionType">VersionType</param>
    /// <returns>The Version object representing the current type version. Null if error</returns>
    private async Task<Version?> GetCurrentVersionAsync(VersionType versionType)
    {
        _latestReleaseId = null;
        var releases = await _github.Repository.Release.GetAll(_repoOwner, _repoName);
        Release? latest = null;
        foreach (var release in releases)
        {
            if ((versionType == VersionType.Stable && !release.Prerelease) || versionType == VersionType.Preview && release.Prerelease)
            {
                if (latest == null || (latest != null && release.CreatedAt > latest.CreatedAt))
                {
                    latest = release;
                }
            }
        }
        if (latest != null)
        {
            try
            {
                _latestReleaseId = latest.Id;
                return new Version(latest.TagName);
            }
            catch
            {
                _latestReleaseId = null;
                return null;
            }
        }
        return null;
    }
}
