using System.Text.Json;

namespace Watari.Libretro;

public class GameManager(WatariContext context)
{
    private readonly string libraryPath = context.PathCombine(Directories.ConfigBaseDir, "library.json");

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
        Console.WriteLine($"Adding game: {game.Name} at {game.Path}");
        var library = LoadLibrary();
        library.RemoveAll(g => g.Path == game.Path);
        library.Add(game);
        Console.WriteLine($"Adding game: {game.Name} at {game.Path}");
        SaveLibrary(library);
    }

    public void RemoveGame(string path)
    {
        var library = LoadLibrary();
        library.RemoveAll(g => g.Path == path);
        SaveLibrary(library);
    }

    public void RenameGame(string path, string newName)
    {
        var library = LoadLibrary();
        var game = library.FirstOrDefault(g => g.Path == path);
        if (game != null)
        {
            var updatedGame = game with { Name = newName };
            library.Remove(game);
            library.Add(updatedGame);
            SaveLibrary(library);
        }
    }
}