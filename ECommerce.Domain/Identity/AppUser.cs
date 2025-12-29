using Microsoft.AspNetCore.Identity;

namespace ECommerce.Domain.Identity;

public class AppUser : IdentityUser
{
    public string FullName { get; set; } = string.Empty;
}
