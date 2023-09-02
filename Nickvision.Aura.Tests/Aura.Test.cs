using Xunit.Abstractions;

namespace Nickvision.Aura.Tests;

public class AuraTest
{
    [Fact]
    public void Init()
    {
        Assert.Throws<AuraException>(() => Aura.Active);
        Aura.Init("org.nickvision.Aura.Tests", "Nickvision Aura Tests", "Aura Tests", "Test Aura library");
        Assert.Throws<AuraException>(() => Aura.Init("org.nickvision.Aura.Tests", "Nickvision Aura Tests", "Aura Tests", "Test Aura library"));
        Assert.IsType<Aura>(Aura.Active);
    }
}