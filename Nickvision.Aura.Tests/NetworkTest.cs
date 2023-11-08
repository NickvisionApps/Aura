using System;
using System.Threading.Tasks;

using Nickvision.Aura.Network;

namespace Nickvision.Aura.Tests;

public class NetworkTest
{
    [SkippableFact]
    public async Task NetworkAccessTest()
    {
        Environment.SetEnvironmentVariable("AURA_DISABLE_NETCHECK", "true");
        var netmon = await NetworkMonitor.NewAsync();
        Assert.True(await netmon.GetStateAsync());
    }

    [Fact]
    public async Task ValidWebsiteTest() => Assert.True(await WebHelpers.GetIsValidWebsiteAsync(new Uri("https://example.com/")));
}
