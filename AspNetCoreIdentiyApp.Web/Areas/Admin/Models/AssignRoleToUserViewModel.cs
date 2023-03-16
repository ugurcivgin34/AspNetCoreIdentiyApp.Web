namespace AspNetCoreIdentiyApp.Web.Areas.Admin.Models
{
    public class AssignRoleToUserViewModel
    {
        public string Id { get; set; } = null!;
        public string Name { get; set; } = null!;
        public bool Exist { get; set; } //Bu rol ilgili kullanıcıda var mı yok mu diye bakmak için bu property'i tanımladık
    }
}
