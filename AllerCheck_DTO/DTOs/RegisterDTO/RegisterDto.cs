using System.ComponentModel.DataAnnotations;

namespace AllerCheck.API.DTOs.RegisterDTO
{
    public class RegisterDto
    {
        [Required(ErrorMessage = "Ad alanı zorunludur.")]
        [StringLength(50, ErrorMessage = "Ad en fazla 50 karakter olabilir.")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Soyad alanı zorunludur.")]
        [StringLength(50, ErrorMessage = "Soyad en fazla 50 karakter olabilir.")]
        public string UserSurname { get; set; }

        [Required(ErrorMessage = "E-posta adresi zorunludur.")]
        [EmailAddress(ErrorMessage = "Geçerli bir e-posta adresi giriniz.")]
        public string MailAdress { get; set; }

        [Required(ErrorMessage = "Şifre zorunludur.")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Şifre en az 6 karakter olmalıdır.")]
        public string UserPassword { get; set; }

        [Required(ErrorMessage = "Şifre tekrarı zorunludur.")]
        [Compare("UserPassword", ErrorMessage = "Şifreler eşleşmiyor.")]
        public string UserPasswordConfirm { get; set; }
    }
}
