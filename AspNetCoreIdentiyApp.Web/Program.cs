using AspNetCoreIdentiyApp.Web.Models;
using Microsoft.EntityFrameworkCore;
using AspNetCoreIdentiyApp.Web.Extensions;
using AspNetCoreIdentiyApp.Web.Models.OptionsEntity;
using AspNetCoreIdentiyApp.Web.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.FileProviders;
using Microsoft.AspNetCore.Authentication;
using AspNetCoreIdentiyApp.Web.ClaimProvider;
using Microsoft.AspNetCore.Authorization;
using AspNetCoreIdentiyApp.Web.Requirements;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

//Sql Baðlantýsý Saðlar...
builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("SqlCon"));
});

//Herhangibir classýn constructorýnda IOptions görürsen appsettings den EmailSettings datalarýný al
builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));

//Identiy Kütüphanesi Ýçin Ekledik...
builder.Services.AddIdentityWithExt();



//ConcurrencyStamp, ASP.NET Core Identity tarafýndan kullanýlan bir özelliktir ve veritabanýnda kaydedilen herhangi bir kimlik doðrulama kaydýnýn deðiþtirilip deðiþtirilmediðini takip etmek için kullanýlýr. Bu özellik, kullanýcý hesaplarýný yönetmek için kullanýlan tablolardaki her satýr için benzersiz bir deðerdir.
//ConcurrencyStamp, her kullanýcý hesabý için bir kez oluþturulur ve hesapta herhangi bir deðiþiklik yapýldýðýnda (örneðin, kullanýcý þifresi deðiþtirildiðinde veya hesap silindiðinde), bu deðer deðiþtirilir. Böylece, birden fazla istemcinin ayný anda ayný kullanýcý hesabýný deðiþtirmesi durumunda bile, her bir istemcinin güncellemesi diðerlerinin üzerine yazýlmaz. Farklý cihazlardan deðiþtirme yapýldýðýnda en son yapýlanýn kaydý veritabanýna kaydedilir
//ConcurrencyStamp ayrýca, ASP.NET Core Identity tarafýndan saðlanan bir yetkilendirme mekanizmasý olan Token tabanlý kimlik doðrulamayý kullanýrken de kullanýlýr. Token tabanlý kimlik doðrulama, bir kullanýcýnýn giriþ yapmasýný saðlamak için geçici bir eriþim anahtarý olan bir JWT (JSON Web Token) oluþturur.JWT'nin bir parçasý olarak, kullanýcýnýn ConcurrencyStamp deðeri de içerilir. Bu, JWT'nin geçerliliðini kontrol ederken, kullanýcýnýn hesabý deðiþtirilirse JWT'nin artýk geçerli olmayacaðýndan emin olunmasýna olanak tanýr.



//Security stamp, ASP.NET Identity içinde kullanýlan bir özelliktir. Bu, kullanýcýnýn hesap bilgilerinde yapýlan önemli deðiþikliklerin belirlenmesine yardýmcý olan bir deðerdir.
//ASP.NET Identity, kullanýcý hesap bilgilerinin korunmasý için çeþitli yöntemler saðlar. Bu yöntemler arasýnda, kullanýcýnýn kimliði (user ID), kullanýcýnýn þifresi ve kullanýcýnýn güvenlik damgasý (security stamp) yer alýr. Güvenlik damgasý, kullanýcýnýn hesap bilgilerinde yapýlan önemli bir deðiþiklik olduðunda deðiþtirilir. Bu deðiþiklikler, kullanýcýnýn þifresinin deðiþtirilmesi, e-posta adresinin güncellenmesi veya hesabýn silinmesi gibi iþlemleri kapsayabilir.
//Bir kullanýcýnýn güvenlik damgasý, kullanýcýnýn tarayýcýsýnda depolanýr ve sunucuya her istekte gönderilir. Bu, sunucunun kullanýcýnýn hesap bilgilerindeki önemli deðiþiklikleri tespit etmesini saðlar. Kullanýcýnýn güvenlik damgasý, uygulamanýn güvenliðini artýrmaya ve kullanýcýnýn hesap bilgilerinin kötüye kullanýmýný önlemeye yardýmcý olur. 
//Defaultta 30 dakika da bir kullanýcýnýn SecurityStamp tablosundaki deðer bakýlýr. Kullanýcý hem bilgisayardan hem mobilden giriþ yapýp mobilden þifresini deðiþtirmeye kalktýðýnda kullanýcý bilgisayarda oturum bilgisi halen daha açýk olacaktýr.Fakat Security Stamp olayý ile 30 dakikada bir tabloya baktýðý için deðiþiklik farkedilecek ve ootmatikmen kullanýcýyý logout edecektir.Önemli bilgilerinden herhangi bir deðiþiklik olduðunda securityStamp deðeri otomatik deðiþir.Aþaðýda konfigürasyonu da saðlanabilir.
builder.Services.Configure<SecurityStampValidatorOptions>(options =>
{
    options.ValidationInterval = TimeSpan.FromMinutes(30);
});

//Bu kod sayesinde projede herhangi bir sýnýffta IFilePRovider kullanýlýrsa dosya iþlemleri için referans olarak da wwwroot olacak kullanýr,eriþmiþ olur
builder.Services.AddSingleton<IFileProvider>(new PhysicalFileProvider(Directory.GetCurrentDirectory()));

//Request response yaþam döngüsü olduðu için response döndüðü anda bu emailservice memoryden gitsin,her request ile beraber oluþsun her defasýnda 
builder.Services.AddScoped<IEmailService, EmailService>();

//Coorkiee de claime istediðimi özlliði eklemek için ekledik,Ezdik
builder.Services.AddScoped<IClaimsTransformation, UserClaimProvider>();

//Bir kullanýcýnýn deðiþim yapabileceði son tarihin geçip geçmediðini kontrol etmek için yetkilendirme gereksinimini temsil eder.Bunun enjekte ettik
builder.Services.AddScoped<IAuthorizationHandler, ExchangeExpireRequirementHandler>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

//Area ekleyince burayý da eklemek gerekiyor,zate oluþturunca otomatik kendisi bu yapýyý ekle þeklinde sayfa gösteriyor.Biz bunu yeniversiyon göre uyarladý...
app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
