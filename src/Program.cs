using Watari;
using watari_libretro;
using SeparateProcess;
using Microsoft.Extensions.Logging;

if (Spawner.GetRunner(args) is ProcessRunner runner)
{
    runner.Run().Wait();
    return 0;
}

new FrameworkBuilder()
    .SetDevPort(8831)
    .Expose<LibretroApplication>()
    .SetServerPort(8836)
    .SetFrontendPath("frontend")
    .ConfigureLogging(builder => builder.AddConsole())
    .Build().Run(args);

return 0;
