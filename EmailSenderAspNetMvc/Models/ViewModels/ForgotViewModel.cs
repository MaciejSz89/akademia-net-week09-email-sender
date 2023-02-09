using System.ComponentModel.DataAnnotations;

namespace EmailSenderAspNetMvc.Models
{
    public class ForgotViewModel
    {
        [Required]
        [Display(Name = "Email")]
        public string Email { get; set; }
    }
}
