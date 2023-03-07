using AspNetCoreIdentiyApp.Web.Models.Entity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace AspNetCoreIdentiyApp.Web.Models
{
    //string yazdığımızda otomatik olarak random guid olarak atar
    public class AppDbContext : IdentityDbContext<AppUser, AppRole, string>
    {

        //ConnectionStringi Programcs den almak için DcContextOptions sınıfını kullanırız.
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }



    }
}

