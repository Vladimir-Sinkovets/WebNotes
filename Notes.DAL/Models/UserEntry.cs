using Microsoft.AspNetCore.Identity;

namespace Notes.DAL.Models
{
    public class UserEntry : IdentityUser
    {
        public string? Nickname { get; set; }
    }
}
