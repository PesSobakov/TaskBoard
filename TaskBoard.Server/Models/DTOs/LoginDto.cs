using System.ComponentModel.DataAnnotations;

namespace TaskBoard.Server.Models.DTOs
{
    public class LoginDto
    {
        [Required]
        [EmailAddress]
        public string Login { get; set; }
        [Required]
        public string Password { get; set; }
    }
}
