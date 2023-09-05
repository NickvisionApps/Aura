using Xunit.Abstractions;

namespace Nickvision.Aura.Tests;

public class KeyringTest
{
    private readonly ITestOutputHelper _output;

    public KeyringTest(ITestOutputHelper output)
    {
        _output = output;
    }
    
    [Fact]
    public void CredentialManagerTest()
    {
        var keyring = Keyring.Keyring.Access("org.nickvision.test");
        Assert.True(keyring != null);
    }
    
    [Theory]
    [InlineData(4)]
    [InlineData(16)]
    [InlineData(42)]
    public void PasswordGeneratorTest(int length)
    {
        var gen = new Keyring.PasswordGenerator();
        var password = gen.Next(length);
        _output.WriteLine(password);
        Assert.True(password.Length == length);
    }
}