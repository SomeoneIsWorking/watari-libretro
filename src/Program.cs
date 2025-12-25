using Watari;
using watari_libretro;

if (RunnerProcess.Get(args) is RunnerProcess runner)
{
    runner.Run().Wait();
    return 0;
}

new FrameworkBuilder()
    .SetDevPort(8831)
    .Expose<LibretroApplication>()
    .SetServerPort(8836)
    .SetFrontendPath("frontend")
    .Build().Run(args);

return 0;
