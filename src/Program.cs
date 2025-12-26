using Watari;
using watari_libretro;
using SeparateProcess;

if (ProcessRunner.Get(args) is ProcessRunner runner)
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
