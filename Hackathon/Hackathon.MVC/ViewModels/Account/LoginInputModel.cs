using System.ComponentModel.DataAnnotations;

namespace Hackathon.MVC.ViewModels.Account
{
    public class LoginInputModel
    {
        [Required(ErrorMessage = "E-mail je obvezen.")]
        [EmailAddress(ErrorMessage = "Vnesen ni veljaven e-mail naslov.")]
        [Display(Name = "E-mail")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Geslo je obvezno.")]
        [StringLength(100, ErrorMessage = "{0} mora biti vsaj {2}.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Geslo")]
        public string Password { get; set; }

        [Display(Name = "Zapomni se me")] public bool RememberMe { get; set; }
    }
}