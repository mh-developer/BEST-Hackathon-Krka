using System.ComponentModel.DataAnnotations;

namespace Hackathon.MVC.ViewModels.Account
{
    public class ResetPasswordModel
    {
        [Required(ErrorMessage = "E-mail je obvezen.")]
        [EmailAddress(ErrorMessage = "Vnesen ni veljaven e-mail naslov.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Geslo je obvezno.")]
        [StringLength(100, ErrorMessage = "{0} mora biti vsaj {2}.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Potrdi geslo")]
        [Compare("Password", ErrorMessage = "Geslo in potrditveno geslo se ne ujemata.")]
        public string ConfirmPassword { get; set; }

        public string Code { get; set; }
    }
}