using AspNetCoreIdentiyApp.Web.Models.Entity;
using AspNetCoreIdentiyApp.Web.Models;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace AspNetCoreIdentiyApp.Web.Extensions
{
    public static class StartupExtensions
    {
        public static void AddIdentityWithExt(this IServiceCollection services)
        {
            //Identiy Kütüphanesi İçin Ekledik...
            services.AddIdentity<AppUser, AppRole>(options =>
            {
                options.User.RequireUniqueEmail = true; //Email unique olması ayarı
                options.User.AllowedUserNameCharacters = "abcdefghijklmnoprestuvwxyz1234567890_"; //Kullanıcı adı küçük harflerden istediğimiz harflerden ve rakamlardan ve de alt çizgiden sadece oluşmalı şeklinde ayar verebildik...

                options.Password.RequiredLength = 6;//Şifre 6 karakter olsun
                options.Password.RequireNonAlphanumeric = false; //Şifre de alphanumeci yani yıldız soru işareti gibi karakterler olmasın demiş olduk...
                options.Password.RequireLowercase = true; //Küçük karakter zorunlu olsun
                options.Password.RequireUppercase = false; //Büyük karakter zorunlu değil
                options.Password.RequireDigit = false; //Sayısal karakter de zorunlu değil

            }).AddEntityFrameworkStores<AppDbContext>();
        }
    }
}
