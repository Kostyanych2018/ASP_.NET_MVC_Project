namespace GameStore.Application.Common.Constants;

public class CacheConstants
{
    public const string GamesTag = "games";
    public const string GenresTag = "genres";
    public const string AllGenresKey = "genres:all";
    
    public static string GameByIdKey(int id) => $"games:id:{id}";
    public static string GamesListKey(string? genreNormalizedName, int pageNo, int pageSize)
    {
        var genre = string.IsNullOrWhiteSpace(genreNormalizedName) ? "all" : genreNormalizedName;
        return $"games:genre:{genre}:page:{pageNo}:size:{pageSize}";
    }
}