using System.Diagnostics;
using watari_libretro;

class RetroRunner(RetroWrapper retro)
{
    private TaskCompletionSource? tsc;
    private bool running;

    public void Start()
    {
        running = true;
        Stopwatch stopwatch = new();
        stopwatch.Start();
        
        tsc = new TaskCompletionSource();
        Task.Run(() =>
        {
            while (running)
            {
                retro.Run();
                if (stopwatch.ElapsedMilliseconds < 16)
                {
                    Thread.Sleep(16 - (int)stopwatch.ElapsedMilliseconds);
                }
                stopwatch.Restart();
            }
            retro.Dispose();
            tsc?.SetResult();
        });
    }

    public async Task Stop()
    {
        if (!running) return;
        running = false;
        await (tsc?.Task ?? Task.CompletedTask);
        tsc = null;
    }

    internal bool LoadGame(retro_game_info gameInfo)
    {
        return retro.LoadGame(gameInfo);
    }
}