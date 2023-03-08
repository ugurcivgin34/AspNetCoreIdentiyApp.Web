using AspNetCoreIdentiyApp.Web.Models;
using Microsoft.EntityFrameworkCore;
using AspNetCoreIdentiyApp.Web.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

//Sql Baðlantýsý Saðlar...
builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("SqlCon"));
});

//Identiy Kütüphanesi Ýçin Ekledik...
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

//Area ekleyince burayý da eklemek gerekiyor,zate oluþturunca otomatik kendisi bu yapýyý ekle þeklinde sayfa gösteriyor.Biz bunu yeniversiyon göre uyarladý...
app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
