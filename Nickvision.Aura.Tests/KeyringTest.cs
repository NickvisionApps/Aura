using Xunit.Abstractions;

namespace Nickvision.Aura.Tests;

public class KeyringTest
{
    private readonly ITestOutputHelper _output;

    public KeyringTest(ITestOutputHelper output)
    {
        _output = output;
    }
    
    [SkippableFact]
    public void AccessTest()
    {
        var keyring = Keyring.Keyring.Access("org.nickvision.aura.test");
        // We want the test to succeed when running locally but skip on GitHub where libsecret keyring is locked
        Skip.If(keyring == null);
        keyring.Destroy();
        keyring.Dispose();
    }

    [Fact]
    public void CredentialManagerTest()
    {
        var setPassword = Keyring.SystemCredentialManager.SetPassword("org.nickvision.aura.test");
        if (setPassword == null)
        {
            _output.WriteLine("Failed to set password, skipping.");
            return;
        }
        Assert.True(setPassword.Length == 16);
        var getPassword = Keyring.SystemCredentialManager.GetPassword("org.nickvision.aura.test");
        Assert.True(setPassword == getPassword);
        Keyring.SystemCredentialManager.DeletePassword("org.nickvision.aura.test");
        Assert.True(Keyring.SystemCredentialManager.GetPassword("org.nickvision.aura.test") == null);
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