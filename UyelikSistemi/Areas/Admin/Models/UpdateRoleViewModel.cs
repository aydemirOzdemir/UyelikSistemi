using System.ComponentModel.DataAnnotations;

namespace UyelikSistemi.Areas.Admin.Models
{
    public class UpdateRoleViewModel
    {
        //update işlemi için idsi ve ismi olan bir view model oluşturuyoruz.
        public string Id { get; set; } = null!;

        [Required(ErrorMessage = "Role Alanı boş bırakılamaz")]
        [Display(Name = "Role Adı:")]
        public string Name { get; set; } = null!;
    }
}
