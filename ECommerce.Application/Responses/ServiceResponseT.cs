namespace ECommerce.Application.Responses;

public class ServiceResponse<T>
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public T? Data { get; set; }

    public static ServiceResponse<T> Ok(T data, string message = "Success")
        => new() { Success = true, Message = message, Data = data };

    public static ServiceResponse<T> Fail(string message)
        => new() { Success = false, Message = message, Data = default };
}
