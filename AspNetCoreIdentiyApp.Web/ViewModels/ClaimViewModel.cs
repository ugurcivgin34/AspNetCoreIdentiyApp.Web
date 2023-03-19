namespace AspNetCoreIdentiyApp.Web.ViewModels
{
    public class ClaimViewModel
    {
        public string Issuer { get; set; } = null!; //Nereden girdiğini anlamak için , local ise Local authority yazar,google dan giriş yapıyorsa google authority yazar
        public string Type { get; set; } = null!;
        public string Value { get; set; } = null!;
    }
}
