namespace UyelikSistemi.Areas.Admin.Models
{
    public class UserViewModel
    {
        //userları listelemek için bir view model oluşturuyoruz.
        public string? UserId { get; set; }
        public string? UserName { get; set; }
        public string? UserEmail { get; set; }
    }
}
