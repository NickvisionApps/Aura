using System;
using System.IO;
using System.Runtime.InteropServices;
using Xunit.Abstractions;

namespace Nickvision.Aura.Tests;

public class UserDirectoriesTest
{
    private readonly ITestOutputHelper _output;
    
    public UserDirectoriesTest(ITestOutputHelper output)
    {
        _output = output;
    }

    [Fact]
    public void Home()
    {
        _output.WriteLine($"Home: {UserDirectories.Home}");
        Assert.True(Directory.Exists(UserDirectories.Home));
    }

    [Fact]
    public void Config()
    {
        _output.WriteLine($"Config: {UserDirectories.Config}");
        Assert.True(Directory.Exists(UserDirectories.Config));
    }

    [Fact]
    public void ApplicationConfig()
    {
        try
        {
            Aura.Init("org.nickvision.Aura.Tests", "Nickvision Aura Tests");
        }
        catch (AuraException) { }
        _output.WriteLine($"ApplicationConfig: {UserDirectories.ApplicationConfig}");
        Assert.True(Directory.Exists(UserDirectories.ApplicationConfig));
    }

    [Fact]
    public void Cache()
    {
        _output.WriteLine($"Cache: {UserDirectories.Cache}");
        Assert.True(Directory.Exists(UserDirectories.Cache));
    }

    [Fact]
    public void ApplicationCache()
    {
        try
        {
            Aura.Init("org.nickvision.Aura.Tests", "Nickvision Aura Tests");
        }
        catch (AuraException) { }
        _output.WriteLine($"ApplicationCache: {UserDirectories.ApplicationCache}");
        Assert.True(Directory.Exists(UserDirectories.ApplicationCache));
    }

    [Fact]
    public void LocalData()
    {
        _output.WriteLine($"LocalData: {UserDirectories.LocalData}");
        Assert.True(Directory.Exists(UserDirectories.LocalData));
    }

    [Fact]
    public void ApplicationLocalData()
    {
        try
        {
            Aura.Init("org.nickvision.Aura.Tests", "Nickvision Aura Tests");
        }
        catch (AuraException) { }
        _output.WriteLine($"ApplicationLocalData: {UserDirectories.ApplicationLocalData}");
        Assert.True(Directory.Exists(UserDirectories.ApplicationLocalData));
    }

    [SkippableFact]
    public void Runtime()
    {
        Skip.IfNot(RuntimeInformation.IsOSPlatform(OSPlatform.Linux));
        _output.WriteLine($"Runtime: {UserDirectories.Runtime}");
        Assert.True(Directory.Exists(UserDirectories.Runtime));
    }

    [Fact]
    public void Desktop()
    {
        _output.WriteLine($"Desktop: {UserDirectories.Desktop}");
        Assert.True(Directory.Exists(UserDirectories.Desktop));
    }
    
    [Fact]
    public void Documents()
    {
        _output.WriteLine($"Documents: {UserDirectories.Documents}");
        Assert.True(Directory.Exists(UserDirectories.Documents));
    }
    
    [Fact]
    public void Downloads()
    {
        _output.WriteLine($"Downloads: {UserDirectories.Downloads}");
        Assert.True(Directory.Exists(UserDirectories.Downloads));
    }

    [Fact]
    public void Music()
    {
        _output.WriteLine($"Music: {UserDirectories.Music}");
        Assert.True(Directory.Exists(UserDirectories.Music));
    }
    
    [Fact]
    public void Pictures()
    {
        _output.WriteLine($"Pictures: {UserDirectories.Pictures}");
        Assert.True(Directory.Exists(UserDirectories.Pictures));
    }
    
    [SkippableFact]
    public void PublicShare()
    {
        Skip.IfNot(RuntimeInformation.IsOSPlatform(OSPlatform.Linux));
        _output.WriteLine($"PublicShare: {UserDirectories.PublicShare}");
        Assert.True(Directory.Exists(UserDirectories.PublicShare));
    }
    
    [Fact]
    public void Templates()
    {
        _output.WriteLine($"Templates: {UserDirectories.Templates}");
        Assert.True(Directory.Exists(UserDirectories.Templates));
    }

    [Fact]
    public void Videos()
    {
        _output.WriteLine($"Videos: {UserDirectories.Videos}");
        Assert.True(Directory.Exists(UserDirectories.Videos));
    }
}