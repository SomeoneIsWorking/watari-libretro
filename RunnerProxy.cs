using System.IO.Pipes;
using System.Text.Json;

namespace watari_libretro;

public class RunnerProxy(NamedPipeServerStream pipe, Dictionary<int, TaskCompletionSource<object?>> pendingRequests)
{
    private readonly NamedPipeServerStream pipe = pipe;
    private readonly Dictionary<int, TaskCompletionSource<object?>> pendingRequests = pendingRequests;
    private int nextId = 0;
    private readonly object pipeLock = new();
    private readonly BinaryWriter writer = new(pipe, System.Text.Encoding.UTF8, leaveOpen: true);

    public async Task LoadCore(string corePath)
    {
        await SendMessageAndWait("LoadCore", new { corePath });
    }

    public async Task LoadGame(string gamePath)
    {
        await SendMessageAndWait("LoadGame", new { gamePath });
    }

    public async Task Run()
    {
        await SendMessageAndWait("Run", null);
    }

    public async Task Stop()
    {
        await SendMessageAndWait("Stop", null);
    }

    public async Task SetInput(string key, bool down)
    {
        await SendMessageAndWait("SetInput", new { key, down });
    }

    private async Task<object?> SendMessageAndWait(string method, object? data)
    {
        int id = Interlocked.Increment(ref nextId);
        var tcs = new TaskCompletionSource<object?>();
        pendingRequests[id] = tcs;
        lock (pipeLock)
        {
            writer.Write("call");
            writer.Write(id);
            writer.Write(method);
            var dataJson = data == null ? "{}" : JsonSerializer.Serialize(data);
            writer.Write(dataJson);
        }
        await pipe.FlushAsync();
        Console.WriteLine($"[Main] Sending {method} with id {id}");
        var result = await tcs.Task;
        Console.WriteLine($"[Main] {method} completed");
        return result;
    }
}