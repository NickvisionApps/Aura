using System.Runtime.InteropServices;
using Xunit.Abstractions;

namespace Nickvision.Aura.Tests;

public class SystemDirectoriesTest
{
    private readonly ITestOutputHelper _output;

    public SystemDirectoriesTest(ITestOutputHelper output)
    {
        _output = output;
    }

    [Fact]
    public void Path()
    {
        _output.WriteLine($"Path: {string.Join(", ", SystemDirectories.Path)}");
        Assert.True(SystemDirectories.Path.Length > 0);
    }

    [SkippableFact]
    public void Config()
    {
        Skip.IfNot(RuntimeInformation.IsOSPlatform(OSPlatform.Linux));
        _output.WriteLine($"Config: {string.Join(", ", SystemDirectories.Config)}");
        Assert.True(SystemDirectories.Config.Length > 0);
    }

    [SkippableFact]
    public void Data()
    {
        Skip.IfNot(RuntimeInformation.IsOSPlatform(OSPlatform.Linux));
        _output.WriteLine($"Data: {string.Join(", ", SystemDirectories.Data)}");
        Assert.True(SystemDirectories.Data.Length > 0);
    }
}