using Nickvision.Aura.Update;
using System;
using System.Threading.Tasks;

namespace Nickvision.Aura.Tests;

public class UpdaterTest
{
    [SkippableFact]
    public async Task LatestUpdateTest()
    {
        var currentVersion = new Version("2023.8.0");
        Aura.Init("org.nickvision.test", "Nickvision Test");
        Aura.Active.AppInfo.SourceRepo = new Uri("https://github.com/NickvisionApps/Denaro");
        var updater = await Updater.NewAsync();
        Assert.NotNull(updater);
        var latestUpdate = await updater.GetCurrentStableVersionAsync();
        Skip.If(latestUpdate == null);
        Assert.True(latestUpdate > currentVersion);
    }
}
