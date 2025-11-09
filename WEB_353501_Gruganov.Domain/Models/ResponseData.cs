using System.Runtime.CompilerServices;

namespace WEB_353501_Gruganov.Domain.Models;

public class ResponseData<T>
{
    public T? Data { get; set; }
    public bool Successfull { get; set; } = true;
    public string? Message { get; set; }

    public static ResponseData<T> Success(T data)
    {
        return new ResponseData<T> { Data = data };
    }

    public static ResponseData<T> Success(string successMessage, T? data = default)
    {
        return new ResponseData<T>
        {
            Data = data,
            Successfull = true,
            Message = successMessage
        };
    }

    public static ResponseData<T> Error(string? errorMessage, T? data = default)
    {
        return new ResponseData<T>
        {
            Data = data,
            Successfull = false,
            Message = errorMessage
        };
    }
}