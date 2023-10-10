using System;
using System.Linq;
using System.Runtime.InteropServices;

namespace Nickvision.Aura.Tests;

public class SystemDirectoriesTest
{
    [Fact]
    public void Path()
    {
        var path = new[] { "test0", "test1" };
        Environment.SetEnvironmentVariable("PATH", string.Join(RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? ";" : ":", path));
        Assert.True(SystemDirectories.Path.SequenceEqual(path));
    }

    [Fact]
    public void Config()
    {
        if (!RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        {
            Assert.True(SystemDirectories.Config.Length == 0);
        }
        var config = new[] { "test0", "test1" };
        Environment.SetEnvironmentVariable("XDG_CONFIG_DIRS", string.Join(":", config));
        Assert.True(SystemDirectories.Config.SequenceEqual(config));
    }

    [Fact]
    public void Data()
    {
        if (!RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        {
            Assert.True(SystemDirectories.Data.Length == 0);
        }
        var data = new[] { "test0", "test1" };
        Environment.SetEnvironmentVariable("XDG_DATA_DIRS", string.Join(":", data));
        Assert.True(SystemDirectories.Data.SequenceEqual(data));
    }
}