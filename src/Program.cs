using Watari;
using watari_libretro;

if (args.Length > 0 && args[0] == "audiotest")
{
    AudioTest.Main();
    return 0;
}

if (args.Length > 1 && args[0] == "--runner")
{
    string handlerType = args[1];
    switch (handlerType)
    {
        case "LibretroHandler":
            if (RunnerProcess<LibretroHandler>.Get(args) is RunnerProcess<LibretroHandler> runner)
            {
                var task = runner.Run();
                task.Wait();
                return task.Result;
            }
            break;
        // Add more handler types here as needed
        default:
            Console.Error.WriteLine($"Unknown handler type: {handlerType}");
            return 1;
    }
}

new FrameworkBuilder()
    .SetDevPort(8831)
    .Expose<watari_libretro.LibretroApplication>()
    .SetServerPort(8836)
    .SetFrontendPath("frontend")
    .Build().Run(args);

return 0;
