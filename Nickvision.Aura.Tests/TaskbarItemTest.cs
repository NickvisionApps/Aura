using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace Nickvision.Aura.Tests;

public class TaskbarItemTest
{
    [SkippableFact]
    public async Task ConnectTest()
    {
        Skip.IfNot(RuntimeInformation.IsOSPlatform(OSPlatform.Linux));
        using var t = await Taskbar.TaskbarItem.ConnectAsync("test.desktop");
        Assert.NotNull(t);
    }
}