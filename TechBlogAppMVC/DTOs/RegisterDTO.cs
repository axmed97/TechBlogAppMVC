using System.ComponentModel.DataAnnotations;

namespace TechBlogAppMVC.DTOs
{
    public class RegisterDTO
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public string Surname { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
        [Required]
        [Compare("Password", ErrorMessage = "Password is invalid")]
        public string PasswordConfirm { get; set; }

    }
}
