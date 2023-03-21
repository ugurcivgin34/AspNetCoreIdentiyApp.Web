using AspNetCoreIdentiyApp.Web.Models.Entity;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace AspNetCoreIdentiyApp.Web.Seeds
{
    //Bu kodlar, bir rol yöneticisi (RoleManager) kullanarak belirli rollerin tanımlanmasını ve bu rollerin belirli alanlara belirli izinlere sahip olmasını sağlar. Özellikle, Seed metodu, BasicRole, AdvancedRole ve AdminRole rollerini kontrol eder ve varsa bunları kullanır veya yoksa yeni roller oluşturur. AddReadPermission, AddUpdateAndCreatePermission ve AddDeletePermission metotları ise belirli bir role belirli izinleri ekler. Bu izinler, "Permission" adlı bir etiketle birlikte belirli bir alanın okunması, güncellenmesi veya silinmesi gibi işlemleri temsil eden bir izin adına sahiptir. Bu şekilde, bir kullanıcı yönetim sistemi oluşturulur ve her bir kullanıcı belirli roller ve izinlerle yönetilir.
    public class PermissionSeed
    {
        // Rol yöneticisi (RoleManager) üzerinden rollerin ve izinlerin tanımlanmasını sağlayan Seed metodu
        public static async Task Seed(RoleManager<AppRole> roleManager)
        {
            // Var olan BasicRole rolü kontrol edilir
            var hasBasicRole = await roleManager.RoleExistsAsync("BasicRole");
            // Var olan AdvancedRole rolü kontrol edilir
            var hasAdvancedRole = await roleManager.RoleExistsAsync("AdvancedRole");
            // Var olan AdminRole rolü kontrol edilir
            var hasAdminRole = await roleManager.RoleExistsAsync("AdminRole");

            // BasicRole yoksa oluşturulur ve bu role okuma izni eklenir
            if (!hasBasicRole)
            {
                await roleManager.CreateAsync(new AppRole() { Name = "BasicRole" });
                var basicRole = (await roleManager.FindByNameAsync("BasicRole"))!;
                await AddReadPermission(basicRole, roleManager);
            }

            // AdvancedRole yoksa oluşturulur ve bu role okuma, güncelleme ve oluşturma izinleri eklenir
            if (!hasAdvancedRole)
            {
                await roleManager.CreateAsync(new AppRole() { Name = "AdvancedRole" });
                var advancedRole = (await roleManager.FindByNameAsync("AdvancedRole"))!;
                await AddReadPermission(advancedRole, roleManager);
                await AddUpdateAndCreatePermission(advancedRole, roleManager);
            }

            // AdminRole yoksa oluşturulur ve bu role okuma, güncelleme, oluşturma ve silme izinleri eklenir
            if (!hasAdminRole)
            {
                await roleManager.CreateAsync(new AppRole() { Name = "AdminRole" });
                var adminRole = (await roleManager.FindByNameAsync("AdminRole"))!;
                await AddReadPermission(adminRole, roleManager);
                await AddUpdateAndCreatePermission(adminRole, roleManager);
                await AddDeletePermission(adminRole, roleManager);
            }
        }

        // Verilen role okuma izni ekleyen AddReadPermission metodu
        public static async Task AddReadPermission(AppRole role, RoleManager<AppRole> roleManager)
        {
            await roleManager.AddClaimAsync(role, new Claim("Permission", Permissions.Permissions.Stock.Read));
            await roleManager.AddClaimAsync(role, new Claim("Permission", Permissions.Permissions.Order.Read));
            await roleManager.AddClaimAsync(role, new Claim("Permission", Permissions.Permissions.Catalog.Read));
        }

        // Verilen role okuma, güncelleme ve oluşturma izinlerini ekleyen AddUpdateAndCreatePermission metodu
        public static async Task AddUpdateAndCreatePermission(AppRole role, RoleManager<AppRole> roleManager)
        {
            await roleManager.AddClaimAsync(role, new Claim("Permission", Permissions.Permissions.Stock.Create));
            await roleManager.AddClaimAsync(role, new Claim("Permission", Permissions.Permissions.Order.Create));
            await roleManager.AddClaimAsync(role, new Claim("Permission", Permissions.Permissions.Catalog.Create));
            await roleManager.AddClaimAsync(role, new Claim("Permission", Permissions.Permissions.Stock.Update));
            await roleManager.AddClaimAsync(role, new Claim("Permission", Permissions.Permissions.Order.Update));
            await roleManager.AddClaimAsync(role, new Claim("Permission", Permissions.Permissions.Catalog.Update));
        }

        // Verilen role silme iznini ekleyen AddDeletePermission metodu
        public static async Task AddDeletePermission(AppRole role, RoleManager<AppRole> roleManager)
        {
            await roleManager.AddClaimAsync(role, new Claim("Permission", Permissions.Permissions.Stock.Delete));
            await roleManager.AddClaimAsync(role, new Claim("Permission", Permissions.Permissions.Order.Delete));
            await roleManager.AddClaimAsync(role, new Claim("Permission", Permissions.Permissions.Catalog.Delete));
        }
    }
}
