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
    
    public static readonly string[] AllowedImageExtensions = [".jpg", ".jpeg", ".png", ".webp"];
    
    public static class ErrorMessages
    {
        public const string IdRequired = "Идентификатор игры должен быть больше нуля.";
        public const string GameNotFound = "Игра с ID {0} не найдена.";
        public const string NameRequired = "Название игры обязательно для заполнения.";
        public const string NameLength = "Длина названия игры должна быть от {0} до {1} символов.";
        public const string DescriptionLength = "Описание не должно превышать {0} символов.";
        public const string PriceRange = "Цена должна быть в диапазоне от {0} до {1}.";
        public const string GenreRequired = "Идентификатор игры должен быть больше нуля.";
        public const string GenreNotFound = "Указанный жанр с ID {0} не найден.";
        public const string InvalidImageFormat = "Недопустимый формат файла. Разрешены: {0}.";
        public const string InvalidPageNumber = "Номер страницы должен быть больше или равен 1.";
        public const string InvalidPageSize = "Размер страницы должен быть от 1 до {0}.";
        public const string PageNumberExceeded = "Запрошенный номер страницы превышает общее количество страниц.";
    }
}