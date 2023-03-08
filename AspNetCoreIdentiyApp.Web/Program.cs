using AspNetCoreIdentiyApp.Web.Models;
using Microsoft.EntityFrameworkCore;
using AspNetCoreIdentiyApp.Web.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

//Sql Ba�lant�s� Sa�lar...
builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("SqlCon"));
});

//Identiy K�t�phanesi ��in Ekledik...
builder.Services.AddIdentityWithExt();


var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

//Area ekleyince buray� da eklemek gerekiyor,zate olu�turunca otomatik kendisi bu yap�y� ekle �eklinde sayfa g�steriyor.Biz bunu yeniversiyon g�re uyarlad�...
app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
