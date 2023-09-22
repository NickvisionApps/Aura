using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace Nickvision.Aura.Tests;

public class TaskbarTest
{
    [SkippableFact]
    public async Task ItemConnectTest()
    {
        Skip.IfNot(RuntimeInformation.IsOSPlatform(OSPlatform.Linux));
        Skip.IfNot(string.IsNullOrEmpty(Environment.GetEnvironmentVariable("GITHUB_ACTIONS")));
        using var t = await Taskbar.TaskbarItem.ConnectLinuxAsync("test.desktop");
        Assert.NotNull(t);
    }
}