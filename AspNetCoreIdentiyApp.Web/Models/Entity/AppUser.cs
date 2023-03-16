using AspNetCoreIdentiyApp.Web.Models.Enum;
using Microsoft.AspNetCore.Identity;

namespace AspNetCoreIdentiyApp.Web.Models.Entity
{
    public class AppUser:IdentityUser
    {
        public string? City { get; set; }
        public string? Picture { get; set; }
        public DateTime? BirthDate { get; set; }
        public Gender? Gender { get; set; }
    }
}
