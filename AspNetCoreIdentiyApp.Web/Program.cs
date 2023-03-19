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

//Sql Ba�lant�s� Sa�lar...
builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("SqlCon"));
});

//Herhangibir class�n constructor�nda IOptions g�r�rsen appsettings den EmailSettings datalar�n� al
builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));

//Identiy K�t�phanesi ��in Ekledik...
builder.Services.AddIdentityWithExt();



//ConcurrencyStamp, ASP.NET Core Identity taraf�ndan kullan�lan bir �zelliktir ve veritaban�nda kaydedilen herhangi bir kimlik do�rulama kayd�n�n de�i�tirilip de�i�tirilmedi�ini takip etmek i�in kullan�l�r. Bu �zellik, kullan�c� hesaplar�n� y�netmek i�in kullan�lan tablolardaki her sat�r i�in benzersiz bir de�erdir.
//ConcurrencyStamp, her kullan�c� hesab� i�in bir kez olu�turulur ve hesapta herhangi bir de�i�iklik yap�ld���nda (�rne�in, kullan�c� �ifresi de�i�tirildi�inde veya hesap silindi�inde), bu de�er de�i�tirilir. B�ylece, birden fazla istemcinin ayn� anda ayn� kullan�c� hesab�n� de�i�tirmesi durumunda bile, her bir istemcinin g�ncellemesi di�erlerinin �zerine yaz�lmaz. Farkl� cihazlardan de�i�tirme yap�ld���nda en son yap�lan�n kayd� veritaban�na kaydedilir
//ConcurrencyStamp ayr�ca, ASP.NET Core Identity taraf�ndan sa�lanan bir yetkilendirme mekanizmas� olan Token tabanl� kimlik do�rulamay� kullan�rken de kullan�l�r. Token tabanl� kimlik do�rulama, bir kullan�c�n�n giri� yapmas�n� sa�lamak i�in ge�ici bir eri�im anahtar� olan bir JWT (JSON Web Token) olu�turur.JWT'nin bir par�as� olarak, kullan�c�n�n ConcurrencyStamp de�eri de i�erilir. Bu, JWT'nin ge�erlili�ini kontrol ederken, kullan�c�n�n hesab� de�i�tirilirse JWT'nin art�k ge�erli olmayaca��ndan emin olunmas�na olanak tan�r.



//Security stamp, ASP.NET Identity i�inde kullan�lan bir �zelliktir. Bu, kullan�c�n�n hesap bilgilerinde yap�lan �nemli de�i�ikliklerin belirlenmesine yard�mc� olan bir de�erdir.
//ASP.NET Identity, kullan�c� hesap bilgilerinin korunmas� i�in �e�itli y�ntemler sa�lar. Bu y�ntemler aras�nda, kullan�c�n�n kimli�i (user ID), kullan�c�n�n �ifresi ve kullan�c�n�n g�venlik damgas� (security stamp) yer al�r. G�venlik damgas�, kullan�c�n�n hesap bilgilerinde yap�lan �nemli bir de�i�iklik oldu�unda de�i�tirilir. Bu de�i�iklikler, kullan�c�n�n �ifresinin de�i�tirilmesi, e-posta adresinin g�ncellenmesi veya hesab�n silinmesi gibi i�lemleri kapsayabilir.
//Bir kullan�c�n�n g�venlik damgas�, kullan�c�n�n taray�c�s�nda depolan�r ve sunucuya her istekte g�nderilir. Bu, sunucunun kullan�c�n�n hesap bilgilerindeki �nemli de�i�iklikleri tespit etmesini sa�lar. Kullan�c�n�n g�venlik damgas�, uygulaman�n g�venli�ini art�rmaya ve kullan�c�n�n hesap bilgilerinin k�t�ye kullan�m�n� �nlemeye yard�mc� olur. 
//Defaultta 30 dakika da bir kullan�c�n�n SecurityStamp tablosundaki de�er bak�l�r. Kullan�c� hem bilgisayardan hem mobilden giri� yap�p mobilden �ifresini de�i�tirmeye kalkt���nda kullan�c� bilgisayarda oturum bilgisi halen daha a��k olacakt�r.Fakat Security Stamp olay� ile 30 dakikada bir tabloya bakt��� i�in de�i�iklik farkedilecek ve ootmatikmen kullan�c�y� logout edecektir.�nemli bilgilerinden herhangi bir de�i�iklik oldu�unda securityStamp de�eri otomatik de�i�ir.A�a��da konfig�rasyonu da sa�lanabilir.
builder.Services.Configure<SecurityStampValidatorOptions>(options =>
{
    options.ValidationInterval = TimeSpan.FromMinutes(30);
});

//Bu kod sayesinde projede herhangi bir s�n�ffta IFilePRovider kullan�l�rsa dosya i�lemleri i�in referans olarak da wwwroot olacak kullan�r,eri�mi� olur
builder.Services.AddSingleton<IFileProvider>(new PhysicalFileProvider(Directory.GetCurrentDirectory()));

//Request response ya�am d�ng�s� oldu�u i�in response d�nd��� anda bu emailservice memoryden gitsin,her request ile beraber olu�sun her defas�nda 
builder.Services.AddScoped<IEmailService, EmailService>();

//Coorkiee de claime istedi�imi �zlli�i eklemek i�in ekledik,Ezdik
builder.Services.AddScoped<IClaimsTransformation, UserClaimProvider>();

//Bir kullan�c�n�n de�i�im yapabilece�i son tarihin ge�ip ge�medi�ini kontrol etmek i�in yetkilendirme gereksinimini temsil eder.Bunun enjekte ettik
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

//Area ekleyince buray� da eklemek gerekiyor,zate olu�turunca otomatik kendisi bu yap�y� ekle �eklinde sayfa g�steriyor.Biz bunu yeniversiyon g�re uyarlad�...
app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
