using System.ComponentModel.DataAnnotations;
using UyelikSistemi.Models;

namespace UyelikSistemi.ViewModels
{
    public class UserEditViewModel
    {
        [Required(ErrorMessage = "Kullanıcı Alanı boş bırakılamaz")]
        [Display(Name = "Kullanıcı Adı:")]
        public string? UserName { get; set; }




        [Required(ErrorMessage = "Email alanı boş bırakılamaz")]
        [EmailAddress(ErrorMessage = "Email formatı yanlıştır")]
        [Display(Name = "Email:")]
        public string? Email { get; set; }



        [Required(ErrorMessage = "Telefon Alanı boş bırakılamaz")]
        [Display(Name = "Telefon Numarası:")]
        public string? Phone { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Doğum Tarihi:")]
        public DateTime? BirthDay { get; set; }
      
        [Display(Name = "Şehir:")]
        public string? City { get; set; }
      
        [Display(Name = "Profil Resim:")]
        public IFormFile? Picture { get; set; }
      
         [Display(Name = "Cinsiyet:")]
        public Gender? Gender { get; set; }
      



       
    }
}
