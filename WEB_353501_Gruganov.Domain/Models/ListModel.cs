namespace WEB_353501_Gruganov.Domain.Models;

public class ListModel<T>
{
    public List<T> Items { get; set; } = [];
    public int CurrentPage { get; set; } = 1;
    public int TotalPages { get; set; } = 1;
}