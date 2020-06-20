using System.ComponentModel.DataAnnotations;

namespace WebUI.Models
{
    public class LinkAlbaAccount
    {
        [Required]
        [EmailAddress]
        [Display(Name ="Territory Tools User Email")]
        public string TerritoryUserEmail { get; set; }
        
        [Required]
        public string Host { get; set; }
        
        [Required]
        [Display(Name = "Account Name")]
        public string AccountName { get; set; }
        
        [Required]
        [Display(Name = "User Name")]
        public string UserName { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
    }
}
