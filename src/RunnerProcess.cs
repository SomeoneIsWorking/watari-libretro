using System.IO.Pipes;
using System.Text.Json;
using Microsoft.Extensions.Logging;

namespace watari_libretro;

public class RunnerProcess<THandler> where THandler : IRunnerHandler
{
    public static RunnerProcess<THandler>? Get(string[] args)
    {
        if (args.Length > 1 && args[0] == "--runner")
        {
            return new RunnerProcess<THandler>(args);
        }
        return null;
    }

    private readonly string pipeName;
    private IRunnerHandler? handler;
    private NamedPipeClientStream? pipeClient;
    private readonly object pipeLock = new();
    private BinaryWriter? writer;

    private RunnerProcess(string[] args)
    {
        // Parse args: --runner <type> --pipe <name>
        pipeName = "";
        for (int i = 2; i < args.Length; i++)
        {
            if (args[i] == "--pipe" && i + 1 < args.Length)
            {
                pipeName = args[i + 1];
            }
        }
    }

    public async Task<int> Run()
    {
        try
        {
            Log("Starting runner");
            pipeClient = new NamedPipeClientStream(".", pipeName, PipeDirection.InOut);
            Log("Connecting to pipe");
            pipeClient.Connect();

            writer = new BinaryWriter(pipeClient, System.Text.Encoding.UTF8, leaveOpen: true);

            // Set up logging
            var ipcLogger = new IPCLogger((level, msg) => SendLog(pipeClient, level, msg));
            handler = (IRunnerHandler)Activator.CreateInstance(typeof(THandler), ipcLogger)!;

            // Set up event handlers
            handler.OnFrame += data => SendEvent(pipeClient, "OnFrame", data);
            handler.OnAudio += data => SendEvent(pipeClient, "OnAudio", data);

            // Start listening for messages
            var listenTask = Task.Run(() => ListenForMessages(pipeClient));

            Log("Runner initialized");

            // Wait for stop
            await listenTask;

            Log("Runner stopped");
            return 0;
        }
        catch (Exception ex)
        {
            LogError("Runner error", ex);
            return 1;
        }
    }

    private async Task ListenForMessages(NamedPipeClientStream pipe)
    {
        using var reader = new BinaryReader(pipe, System.Text.Encoding.UTF8, leaveOpen: true);
        while (true)
        {
            try
            {
                await ProcessMessage(reader);
            }
            catch (Exception ex)
            {
                LogError("Error reading message", ex);
                break;
            }
        }
    }

    private async Task ProcessMessage(BinaryReader reader)
    {
        var type = reader.ReadString();
        if (type == "call")
        {
            var id = reader.ReadInt32();
            var method = reader.ReadString();
            var dataJson = reader.ReadString();
            var data = JsonDocument.Parse(dataJson).RootElement;
            Log($"Handling {method} with id {id}");
            try
            {
                var result = await handler!.HandleMessage(method, data);
                SendBinaryMessage(pipeClient!, "response", id, "success", result);
            }
            catch (Exception ex)
            {
                LogError($"Error handling message {method}", ex);
                SendBinaryMessage(pipeClient!, "response", id, "error", ex.Message);
            }
        }
    }

    private void SendBinaryMessage(NamedPipeClientStream pipe, string type, int id, string status, object? result = null)
    {
        lock (pipeLock)
        {
            writer!.Write(type);
            writer.Write(id);
            writer.Write(status);
            var resultJson = result != null ? JsonSerializer.Serialize(result) : "";
            writer.Write(resultJson);
            pipe.Flush();
        }
    }

    private void SendEvent(NamedPipeClientStream pipe, string eventName, object data)
    {
        lock (pipeLock)
        {
            writer!.Write("event");
            writer.Write(eventName);
            var dataJson = JsonSerializer.Serialize(data);
            writer.Write(dataJson);
            pipe.Flush();
        }
    }

    private void SendLog(NamedPipeClientStream pipe, LogLevel level, string message)
    {
        lock (pipeLock)
        {
            writer!.Write("log");
            writer.Write(level.ToString());
            writer.Write(message);
            pipe.Flush();
        }
    }

    private void Log(string message)
    {
        Console.WriteLine($"[Runner] {message}");
    }

    private void LogError(string message, Exception ex)
    {
        Console.Error.WriteLine($"[Runner Error] {message}: {ex}");
    }
}