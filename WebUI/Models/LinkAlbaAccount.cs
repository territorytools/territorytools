using System.ComponentModel.DataAnnotations;

namespace WebUI.Models
{
    public class LinkAlbaAccount
    {
        [Required]
        [EmailAddress]
        [Display(Name ="Territory User Email")]
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

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Confirm Password")]
        public string ConfirmPassword { get; set; }
    }
}
