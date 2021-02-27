using System.ComponentModel.DataAnnotations;

namespace Hackathon.MVC.ViewModels.Account
{
    public class RegisterInputModel
    {
        [Required(ErrorMessage = "Ime je obvezno.")]
        [Display(Name = "Ime")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Priimek je obvezno.")]
        [Display(Name = "Priimek")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "E-mail je obvezen.")]
        [EmailAddress(ErrorMessage = "Vnesen ni veljaven e-mail naslov.")]
        [Display(Name = "E-mail")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Podjetje je obvezno.")]
        [Display(Name = "Podjetje")]
        public string Company { get; set; }

        [Required(ErrorMessage = "Geslo je obvezno.")]
        [StringLength(100, ErrorMessage = "{0} mora biti vsaj {2}.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Geslo")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Ponovno vnesite geslo")]
        [Compare("Password", ErrorMessage = "Geslo in potrditveno geslo se ne ujemata.")]
        public string ConfirmPassword { get; set; }
    }
}