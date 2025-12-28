namespace ECommerce.Application.DTOs.Category;

public class UpdateCategoryDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
}
