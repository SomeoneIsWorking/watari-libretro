using System.Diagnostics;
using watari_libretro;

class RetroRunner
{
    private readonly RetroWrapper retro;
    private readonly TaskCompletionSource tsc = new();
    private bool running;

    public RetroRunner(RetroWrapper retro)
    {
        this.retro = retro;
    }

    public void Start()
    {
        running = true;
        Stopwatch stopwatch = new();
        stopwatch.Start();
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
            tsc.SetResult();
        });
    }

    public Task Stop()
    {
        running = false;
        return tsc.Task;
    }

    internal bool LoadGame(retro_game_info gameInfo)
    {
        return retro.LoadGame(gameInfo);
    }
}