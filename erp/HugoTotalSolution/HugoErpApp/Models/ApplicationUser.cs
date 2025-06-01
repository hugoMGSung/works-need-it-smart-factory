using Microsoft.AspNetCore.Identity;

namespace HugoErpApp.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string? FullName { get; set; }
        public DateTime RegisteredAt { get; set; } = DateTime.UtcNow;
    }
}
