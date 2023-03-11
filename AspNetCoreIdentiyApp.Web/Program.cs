using AspNetCoreIdentiyApp.Web.Models;
using Microsoft.EntityFrameworkCore;
using AspNetCoreIdentiyApp.Web.Extensions;
using AspNetCoreIdentiyApp.Web.Models.OptionsEntity;
using AspNetCoreIdentiyApp.Web.Services;

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



//Request response ya�am d�ng�s� oldu�u i�in response d�nd��� anda bu emailservice memoryden gitsin,her request ile beraber olu�sun her defas�nda 
builder.Services.AddScoped<IEmailService, EmailService>();

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
