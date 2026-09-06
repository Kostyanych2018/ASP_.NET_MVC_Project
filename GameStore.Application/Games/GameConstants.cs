namespace GameStore.Application.Games;

public static class GameConstants
{
    public const int MinNameLength = 2;
    public const int MaxNameLength = 100;
    public const int MaxDescriptionLength = 1000;
    public const decimal MinPrice = 1m;
    public const decimal MaxPrice = 10_000m;

    public const int DefaultPageNumber = 1;
    public const int DefaultPageSize = 3;
    public const int MaxPageSize = 20;

    public const long MaxImageFileSizeBytes = 2 * 1024 * 1024;

    public static readonly string[] AllowedImageExtensions = [".jpg", ".jpeg", ".png", ".webp"];

    public static bool IsAllowedImageExtension(string? fileName)
    {
        if (string.IsNullOrWhiteSpace(fileName))
        {
            return false;
        }

        var extension = Path.GetExtension(fileName).ToLowerInvariant();
        return AllowedImageExtensions.Contains(extension);
    }

    public static class ErrorMessages
    {
        public const string IdRequired = "Game ID must be greater than zero.";
        public const string GameNotFound = "Game with ID {0} was not found.";
        public const string NameRequired = "Game name is required.";
        public const string NameLength = "Game name must be between {0} and {1} characters.";
        public const string DescriptionLength = "Description must not exceed {0} characters.";
        public const string PriceRange = "Price must be between {0} and {1}.";
        public const string GenreRequired = "Genre ID must be greater than zero.";
        public const string GenreNotFound = "Genre with ID {0} was not found.";
        public const string FileRequired = "No file selected.";
        public const string InvalidImageFormat = "Invalid file format. Allowed: {0}.";
        public const string InvalidImageSize = "File size must not exceed {0} MB.";
        public const string InvalidPageNumber = "Page number must be greater than or equal to 1.";
        public const string InvalidPageSize = "Page size must be between 1 and {0}.";
        public const string PageNumberExceeded = "The requested page number exceeds the total number of pages.";
    }
}
