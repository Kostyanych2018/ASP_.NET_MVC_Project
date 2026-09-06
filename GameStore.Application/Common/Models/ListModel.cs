namespace GameStore.Application.Common.Models;

public class ListModel<T>
{
    public List<T> Items { get; set; } = [];
    public int CurrentPage { get; set; } = 1;
    public int TotalPages { get; set; } = 1;
}