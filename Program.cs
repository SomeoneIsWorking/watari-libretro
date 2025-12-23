using Watari;

new FrameworkBuilder()
    .SetDevPort(8831)
    .Expose<watari_libretro.LibretroApplication>()
    .SetServerPort(8836)
    .SetFrontendPath("frontend")
    .Build().Run(args);
