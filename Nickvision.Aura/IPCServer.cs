using System;
using System.IO;
using System.IO.Pipes;
using System.Text;

namespace Nickvision.Aura;

/// <summary>
/// Inter-process communication server
/// </summary>
public class IPCServer
{
    private readonly string _id;
    private bool _running;
    
    /// <summary>
    /// Occurs when a command is received
    /// </summary>
    public EventHandler<string[]>? CommandReceived;
    
    /// <summary>
    /// Construct IPCServer
    /// </summary>
    public IPCServer()
    {
        _id = Aura.Active.AppInfo.ID;
        _running = false;
    }
    
    /// <summary>
    /// Enable IPCServer.
    /// First it will be checked if the server is already running
    /// (by another instance of the same application), and if yes
    /// then command-line arguments will be sent to running
    /// server; otherwise a server will be started.
    /// </summary>
    /// <param name="args">Command-line arguments</param>
    /// <returns>Whether or not a new server was started</returns>
    public bool Communicate(string[] args)
    {
        using var client = new NamedPipeClientStream(".", _id, PipeDirection.Out);
        try
        {
            client.Connect(100);
            using var writer = new BinaryWriter(client, Encoding.UTF8);
            writer.Write(args.Length);
            foreach (var arg in args)
            {
                writer.Write(arg);
            }
            Console.WriteLine("[AURA] Sent a command to the running instance.");
        }
        catch (TimeoutException)
        {
            _running = true;
            Task.Run(StartListeningServer);
        }
        return _running;
    }
    
    /// <summary>
    /// Triggers the server to start listening for commands
    /// </summary>
    public void StartListeningServer()
    {
        while (_running)
        {
            using var server = new NamedPipeServerStream(_id, PipeDirection.In);
            Console.WriteLine("[AURA] Listening to commands.");
            try
            {
                server.WaitForConnection();
                using var reader = new BinaryReader(server, Encoding.UTF8);
                var argc = reader.ReadInt32();
                var args = new string[argc];
                for (int i = 0; i < argc; i++)
                {
                    args[i] = reader.ReadString();
                }
                Console.WriteLine("[AURA] Command received.");
                CommandReceived?.Invoke(this, args);
            }
            catch (Exception ex)
            {
                Console.WriteLine("[AURA] IPCServer terminated with exception!");
                Console.WriteLine(ex);
                _running = false;
            }
        }
    }
}