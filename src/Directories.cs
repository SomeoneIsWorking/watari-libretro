namespace watari_libretro;

public static class Directories
{
    public static string ConfigBaseDir => EnsureDir(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "watari-libretro"));

    public static string CoresDir => EnsureDir(Path.Combine(ConfigBaseDir, "cores"));

    public static string ManifestsDir => EnsureDir(Path.Combine(ConfigBaseDir, "manifests"));

    public static string CoversDir => EnsureDir(Path.Combine(ConfigBaseDir, "covers"));

    public static string CoreOptionsDir => EnsureDir(Path.Combine(ConfigBaseDir, "core-options"));

    public static string SystemDir => EnsureDir(Path.Combine(ConfigBaseDir, "system"));

    public static string SaveDir => EnsureDir(Path.Combine(ConfigBaseDir, "save"));

    private static string EnsureDir(string path)
    {
        Directory.CreateDirectory(path);
        return path;
    }
}