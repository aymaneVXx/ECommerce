namespace ECommerce.Application.Responses;

public class ServiceResponse
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;

    public static ServiceResponse Ok(string message = "Success")
        => new() { Success = true, Message = message };

    public static ServiceResponse Fail(string message)
        => new() { Success = false, Message = message };
}
