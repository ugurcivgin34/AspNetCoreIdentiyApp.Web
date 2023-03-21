using AspNetCoreIdentiyApp.Web.Models.Entity;
using AspNetCoreIdentiyApp.Web.Models;
using AspNetCoreIdentiyApp.Web.CustomValidations;
using AspNetCoreIdentiyApp.Web.Localizations;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using AspNetCoreIdentiyApp.Web.Requirements;

namespace AspNetCoreIdentiyApp.Web.Extensions
{
    public static class StartupExtensions
    {
        public static void AddIdentityWithExt(this IServiceCollection services)
        {

            services.Configure<DataProtectionTokenProviderOptions>(opt =>
            {
                opt.TokenLifespan = TimeSpan.FromHours(2); //Üretilen tokenin süresi
            });

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

                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(3);//Default 3 dk kitlenmesini sağlıyoruz
                options.Lockout.MaxFailedAccessAttempts = 3;//Kaç hatalı girişte kitlensin

            }).AddPasswordValidator<PasswordValidator>()
            .AddUserValidator<UserValidator>() //Passwordvalidatoru da buraya ekledik
            .AddErrorDescriber<LocalizationIdentityErrorDescriber>()  //İdendityError hatalarını elle ezerek türkçe yaptık.Bunun extensinu tanımladık
            .AddDefaultTokenProviders() //İdentity Serverin kendi tokenini üretmek için bunu kullandık
            .AddEntityFrameworkStores<AppDbContext>();

            services.ConfigureApplicationCookie(opt =>
            {
                // CookieBuilder sınıfı, ASP.NET Core uygulamasında kullanılan çerezlerin özelliklerini belirlemek için kullanılır.
                var cookieBuilder = new CookieBuilder();

                // CookieBuilder'ın Name özelliği ile çerez adı belirlenir.
                cookieBuilder.Name = "UdemyAppCookie";

                //kullanıcının yetkisiz olarak erişmeye çalıştığı bir sayfaya erişmek istediğinde yönlendirileceği giriş sayfasının yolunu belirtir.
                opt.LoginPath = new PathString("/Home/SignIn");

                //bu kod parçası "/Member/logout" URL'sinin, kullanıcının oturumunu sonlandırmak için kullanılacağını belirtir.bir kullanıcının oturumunu sonlandırmak için kullanılan URL'nin yolunu belirtir. 
                opt.LogoutPath = new PathString("/Member/logout");

                opt.AccessDeniedPath = new PathString("/Member/AccessDenied");

                // ConfigureApplicationCookie metodu, cookie ayarlarını yapılandırmak için kullanılır.
                opt.Cookie = cookieBuilder;

                // ExpireTimeSpan özelliği, çerezin ne kadar süreyle geçerli olduğunu belirler.
                opt.ExpireTimeSpan = TimeSpan.FromMinutes(60);

                // SlidingExpiration özelliği, kullanıcı herhangi bir işlem yapmadan oturum süresinin dolmasını önlemek için kullanılır.
                opt.SlidingExpiration = true;
            });


            //Bu kod, ASP.NET Core uygulamalarında, Authorization (yetkilendirme) işlemleri için kullanılan AddAuthorization metodu aracılığıyla, AnkaraPolicy adında bir politika ekler.
            //AddPolicy metodu kullanılarak, AnkaraPolicy adlı bir politika tanımlanır.Bu politika, kullanıcının kimlik bilgileri arasında city tipinde bir Claim bulunuyorsa, bu bilginin değerinin "ankara" olması koşulunu sağlar.
            //Bu politika, belirli bir işlem veya sayfa için yetkilendirme yapılması gerektiğinde kullanılabilir.Örneğin, Ankara şehrindeki kullanıcıların sadece Ankara ile ilgili içeriklere erişim sağlamasını gerektiren bir senaryoda, AnkaraPolicy adlı politika bu işlem için kullanılabilir.
            //Ayrıca, options nesnesinin bir diğer kullanımı da, farklı politika ve gereksinimler tanımlamak için kullanılan özelleştirilebilir bir yapı sunar. Bu sayede, farklı senaryolara göre özelleştirilmiş yetkilendirme politikaları tanımlanabilir.
            services.AddAuthorization(options =>
            {
                options.AddPolicy("AnkaraPolicy", policy =>
                {
                    policy.RequireClaim("city", "ankara");
                });

                options.AddPolicy("ExchangePolicy", policy =>
                {
                    policy.AddRequirements(new ExchangeExpireRequirement());
                });

				options.AddPolicy("ViolencePolicy", policy =>
				{
					policy.AddRequirements(new ViolenceRequirement() { ThresholdAge = 18 });


				});

			});

        }
    }
}
