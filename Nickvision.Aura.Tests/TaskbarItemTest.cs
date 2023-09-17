using System.Threading.Tasks;
using Xunit.Abstractions;

namespace Nickvision.Aura.Tests;

public class TaskbarItemTest
{
    [Fact]
    public async Task ConnectTest()
    {
        using var t = await Taskbar.TaskbarItem.Connect("test.desktop");
        Assert.NotNull(t);
    }
}