namespace ECommerce.Application.DTOs.Identity;

public class CreateUser : BaseModel
{
    public string ConfirmPassword { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
}
