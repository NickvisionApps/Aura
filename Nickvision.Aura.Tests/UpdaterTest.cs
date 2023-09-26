using Nickvision.Aura.Update;
using System;
using System.Threading.Tasks;

namespace Nickvision.Aura.Tests;

public class UpdaterTest
{
    [Fact]
    public async Task LatestUpdateTest()
    {
        var currentVersion = new Version("2023.8.0");
        Aura.Init("org.nickvision.test", "Nickvision Test");
        Aura.Active.AppInfo.SourceRepo = new Uri("https://github.com/NickvisionApps/Denaro");
        var updater = new Updater();
        var latestUpdate = await updater.GetCurrentStableVersionAsync();
        Assert.NotNull(latestUpdate);
        Assert.True(latestUpdate > currentVersion);
    }
}
