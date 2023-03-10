using AspNetCoreIdentiyApp.Web.Models.Entity;
using AspNetCoreIdentiyApp.Web.Models;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using AspNetCoreIdentiyApp.Web.CustomValidations;
using AspNetCoreIdentiyApp.Web.Localizations;

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


                options.Lockout.DefaultLockoutTimeSpan=TimeSpan.FromMinutes(3);//Default 3 dk kitlenmesini sağlıyoruz
                options.Lockout.MaxFailedAccessAttempts = 3;//Kaç hatalı girişte kitlensin
            
            }).AddPasswordValidator<PasswordValidator>()
            .AddUserValidator<UserValidator>() //Passwordvalidatoru da buraya ekledik
            .AddErrorDescriber<LocalizationIdentityErrorDescriber>()  //İdendityError hatalarını elle ezerek türkçe yaptık.Bunun extensinu tanımladık
            .AddEntityFrameworkStores<AppDbContext>();

            services.ConfigureApplicationCookie(opt =>
            {
                // CookieBuilder sınıfı, ASP.NET Core uygulamasında kullanılan çerezlerin özelliklerini belirlemek için kullanılır.
                var cookieBuilder = new CookieBuilder();
                // CookieBuilder'ın Name özelliği ile çerez adı belirlenir.
                cookieBuilder.Name = "UdemyAppCookie";
                //kullanıcının yetkisiz olarak erişmeye çalıştığı bir sayfaya erişmek istediğinde yönlendirileceği giriş sayfasının yolunu belirtir.
                opt.LoginPath = new PathString("/ Home/SigIn");
                // ConfigureApplicationCookie metodu, cookie ayarlarını yapılandırmak için kullanılır.
                opt.Cookie = cookieBuilder;
                // ExpireTimeSpan özelliği, çerezin ne kadar süreyle geçerli olduğunu belirler.
                opt.ExpireTimeSpan = TimeSpan.FromMinutes(60);
                // SlidingExpiration özelliği, kullanıcı herhangi bir işlem yapmadan oturum süresinin dolmasını önlemek için kullanılır.
                opt.SlidingExpiration = true;
            });

        }


    }
}
