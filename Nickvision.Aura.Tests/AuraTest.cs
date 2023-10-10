namespace Nickvision.Aura.Tests;

public class AuraTest
{
    [Fact]
    public void Init()
    {
        Aura.Init("org.nickvision.Aura.Tests", "Nickvision Aura Tests");
        Assert.IsType<Aura>(Aura.Active);
    }
}