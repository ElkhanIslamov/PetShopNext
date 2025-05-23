using System.ComponentModel.DataAnnotations;

namespace Pb304PetShop.Models
{
    public class ChangePasswordViewModel
    {
        public required string CurrentPassword { get; set; }
        public required string NewPassword { get; set; }
        [DataType(DataType.Password)]
        [Compare("NewPassword", ErrorMessage = "Passwords do not match.")]
        public required string ConfirmPassword { get; set; }
    }
}
