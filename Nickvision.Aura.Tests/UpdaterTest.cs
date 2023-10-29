using Nickvision.Aura.Update;
using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace Nickvision.Aura.Tests;

public class UpdaterTest
{
    [SkippableFact]
    public async Task LatestUpdateTest()
    {
        Skip.IfNot(RuntimeInformation.IsOSPlatform(OSPlatform.Windows));
        var currentVersion = new Version("2023.9.0");
        Aura.Init("org.nickvision.test", "Nickvision Test");
        Aura.Active.AppInfo.SourceRepo = new Uri("https://github.com/NickvisionApps/Tagger");
        var updater = await Updater.NewAsync();
        Assert.NotNull(updater);
        var latestUpdate = await updater.GetCurrentStableVersionAsync();
        Assert.NotNull(latestUpdate);
        Assert.True(latestUpdate > currentVersion, "There is no update available.");
        Assert.True(await updater.WindowsUpdateAsync(VersionType.Stable), "Failed to download the latest update.");
    }
}
