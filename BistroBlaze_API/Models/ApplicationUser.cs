using Microsoft.AspNetCore.Identity;

namespace BistroBlaze_API.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string Name { get; set; }
    }
}
