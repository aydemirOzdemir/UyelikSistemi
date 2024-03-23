using System.ComponentModel.DataAnnotations;

namespace UyelikSistemi.Areas.Admin.Models
{
    public class RoleAddViewModel
    {
        //Role eklemek için bir view model oluşturuyoruz bununda bir name propu var
        [Required(ErrorMessage = "Role Alanı boş bırakılamaz")]
        [Display(Name = "Role Adı:")]
        public string Name { get; set; }
    }
}
