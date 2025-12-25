using System.Diagnostics;
using System.IO.Pipes;
using System.Text.Json;

namespace watari_libretro;

public class RunnerManager<THandler> where THandler : IRunnerHandler
{
    private struct RunnerCall
    {
        public string Method { get; set; }
        public int Id { get; set; }
        public JsonElement Data { get; set; }
    }

    private struct RunnerResponse
    {
        public int Id { get; set; }
        public string Status { get; set; }
        public object? Result { get; set; }
    }

    private struct RunnerEvent
    {
        public string Event { get; set; }
        public JsonElement Data { get; set; }
    }
    private Process? runnerProcess;
    private NamedPipeServerStream? pipeServer;
    private readonly string pipeName = $"w{Guid.NewGuid().ToString("N")[..8]}";
    private RunnerProxy? runnerProxy;
    private readonly Dictionary<int, TaskCompletionSource<object?>> pendingRequests = new();
    private readonly object pipeLock = new();

    public async Task StartRunner()
    {
        Console.WriteLine("[Main] Starting runner process");
        // Start runner process
        runnerProcess = new Process();
        runnerProcess.StartInfo.FileName = Environment.ProcessPath!;
        runnerProcess.StartInfo.Arguments = $"--runner {typeof(THandler).Name} --pipe {pipeName}";
        runnerProcess.StartInfo.UseShellExecute = false;
        runnerProcess.StartInfo.RedirectStandardOutput = true;
        runnerProcess.StartInfo.RedirectStandardError = true;
        runnerProcess.Start();

        // Start pipe server
        pipeServer = new NamedPipeServerStream(pipeName, PipeDirection.InOut, 1, PipeTransmissionMode.Byte, PipeOptions.None, 4 * 1024 * 1024, 4 * 1024 * 1024);
        Console.WriteLine("[Main] Waiting for pipe connection");
        await pipeServer.WaitForConnectionAsync();

        // Check if process exited
        if (runnerProcess.HasExited)
        {
            throw new Exception($"Runner process exited early with code {runnerProcess.ExitCode}");
        }

        // Create proxy
        runnerProxy = new RunnerProxy(pipeServer, pendingRequests);
        Console.WriteLine("[Main] Runner started successfully");
    }

    public async Task StopRunner()
    {
        if (runnerProxy != null)
        {
            await runnerProxy.Stop();
            runnerProxy = null;
        }
        if (pipeServer != null)
        {
            pipeServer.Close();
            pipeServer = null;
        }
        if (runnerProcess != null)
        {
            if (!runnerProcess.HasExited)
            {
                runnerProcess.Kill();
            }
            await runnerProcess.WaitForExitAsync();
            runnerProcess = null;
        }
    }

    public RunnerProxy? Proxy => runnerProxy;

    public void ListenForMessages(Action<string, JsonElement> onEvent)
    {
        if (pipeServer == null) return;
        using var reader = new BinaryReader(pipeServer, System.Text.Encoding.UTF8, leaveOpen: true);
        while (true)
        {
            try
            {
                HandleMessage(onEvent, reader);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Main] Error reading message: {ex}");
                break;
            }
        }
    }

    private void HandleMessage(Action<string, JsonElement> onEvent, BinaryReader reader)
    {
        lock (pipeLock)
        {
            var type = reader.ReadString();
            if (type == "event")
            {
                var eventName = reader.ReadString();
                var dataJson = reader.ReadString();
                var data = JsonDocument.Parse(dataJson).RootElement;
                onEvent(eventName, data);
            }
            else if (type == "log")
            {
                var level = reader.ReadString();
                var msg = reader.ReadString();
                Console.WriteLine($"[{level}] {msg}");
            }
            else if (type == "response")
            {
                var id = reader.ReadInt32();
                var status = reader.ReadString();
                var resultJson = reader.ReadString();
                var result = !string.IsNullOrEmpty(resultJson) ? JsonSerializer.Deserialize<object>(resultJson) : null;
                HandleResponse(id, status, result);
            }
        }
    }

    private void HandleResponse(int id, string status, object? result)
    {
        if (pendingRequests.TryGetValue(id, out var tcs))
        {
            pendingRequests.Remove(id);
            if (status == "success")
            {
                tcs.SetResult(result);
            }
            else if (status == "error")
            {
                tcs.SetException(new Exception(result?.ToString() ?? "Unknown error"));
            }
        }
    }
}
