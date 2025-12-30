using System.Net.Http.Json;

namespace watari_libretro;

public class SteamGridDBApi
{
    private readonly HttpClient _httpClient;
    private const string BaseUrl = "https://www.steamgriddb.com/api/v2";

    public SteamGridDBApi(string apiKey)
    {
        _httpClient = new HttpClient();
        _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");
        _httpClient.DefaultRequestHeaders.Add("User-Agent", "Watari-Libretro/1.0");
    }

    public async Task<List<CoverOption>> Search(string query)
    {
        var url = $"{BaseUrl}/search/autocomplete/{Uri.EscapeDataString(query)}";
        var result = await _httpClient.GetFromJsonAsync<SearchResponse>(url)
            ?? throw new Exception("Invalid response from SteamGridDB");
        var searchResults = result.Data;
        if (searchResults.Count == 0)
        {
            throw new Exception("No games found for the search term");
        }

        var gameId = searchResults[0].Id;
        var gridsUrl = $"{BaseUrl}/grids/game/{gameId}?dimensions=600x900,660x930";
        var gridsResult = await _httpClient.GetFromJsonAsync<GridsResponse>(gridsUrl) 
            ?? throw new Exception("Invalid response from SteamGridDB");
        var grids = gridsResult.Data;

        return grids.Select(g => new CoverOption
        {
            Id = g.Id.ToString(),
            ThumbUrl = g.Thumb,
            FullUrl = g.Url,
            Name = searchResults[0].Name
        }).ToList();
    }
}

public class SearchResponse
{
    public List<GameSearchResult> Data { get; set; } = new();
}

public class GameSearchResult
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public List<string> Types { get; set; } = new();
    public bool Verified { get; set; }
}

public class GridsResponse
{
    public List<Grid> Data { get; set; } = new();
}

public class CoverOption
{
    public string Id { get; set; } = "";
    public string ThumbUrl { get; set; } = "";
    public string FullUrl { get; set; } = "";
    public string Name { get; set; } = "";
}

public class Grid
{
    public int Id { get; set; }
    public int Score { get; set; }
    public string Style { get; set; } = "";
    public string Url { get; set; } = "";
    public string Thumb { get; set; } = "";
    public int Width { get; set; }
    public int Height { get; set; }
    // Other fields omitted
}