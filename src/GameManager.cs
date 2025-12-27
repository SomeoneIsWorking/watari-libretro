using System.Text.Json;
using Watari;

namespace watari_libretro;

public record GameInfo(string Name, string Path, string SystemId, string? Cover);
public record SystemInfo(string Name, string DefaultCore, List<string> Extensions);

public class GameManager(WatariContext context)
{
    private readonly string libraryPath = context.PathCombine("config", "library.json");

    public void SaveLibrary(List<GameInfo> games)
    {
        Directory.CreateDirectory(Path.GetDirectoryName(libraryPath)!);
        var json = JsonSerializer.Serialize(games);
        File.WriteAllText(libraryPath, json);
    }

    public List<GameInfo> LoadLibrary()
    {
        if (!File.Exists(libraryPath))
        {
            return [];
        }
        var json = File.ReadAllText(libraryPath);
        return JsonSerializer.Deserialize<List<GameInfo>>(json) ?? [];
    }

    public void AddGame(GameInfo game)
    {
        var library = LoadLibrary();
        library.RemoveAll(g => g.Path == game.Path);
        library.Add(game);
        SaveLibrary(library);
    }

    public void RemoveGame(string path)
    {
        var library = LoadLibrary();
        library.RemoveAll(g => g.Path == path);
        SaveLibrary(library);
    }
}