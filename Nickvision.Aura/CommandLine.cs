namespace Nickvision.Aura;

/// <summary>
/// Helper class to work with command-line arguments
/// </summary>
public static class CommandLine
{
    /// <summary>
    /// Parse command-line arguments to dictionary
    /// </summary>
    /// <param name="args">Command string array</param>
    /// <returns>Dictionary of commands</returns>
    public static Dictionary<string, string> Parse(string[] args)
    {
        var result = new Dictionary<string, string>();
        try
        {
            foreach (var arg in args)
            {
                var command = arg.TrimStart('-').Split("=");
                if (command.Length > 1)
                {
                    result[command[0]] = string.Join("=", command.Skip(1).ToArray());
                }
                else
                {
                    result[command[0]] = "";
                }
            }
            return result;
        }
        catch
        {
            Console.WriteLine("[AURA] Invalid options.");
            return new Dictionary<string, string>();
        }
    }
}