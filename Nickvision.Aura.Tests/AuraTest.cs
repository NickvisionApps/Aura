using Xunit.Abstractions;

namespace Nickvision.Aura.Tests;

public class AuraTest
{
    [Fact]
    public void Init()
    {
        Assert.Throws<AuraException>(() => Aura.Active);
        Aura.Init("org.nickvision.Aura.Tests", "Nickvision Aura Tests");
        Assert.Throws<AuraException>(() => Aura.Init("org.nickvision.Aura.Tests", "Nickvision Aura Tests"));
        Assert.IsType<Aura>(Aura.Active);
    }
}