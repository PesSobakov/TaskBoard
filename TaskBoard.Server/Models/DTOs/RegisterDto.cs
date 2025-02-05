using System.ComponentModel.DataAnnotations;

namespace TaskBoard.Server.Models.DTOs
{
    public class RegisterDto
    {
        [Required]
        [EmailAddress]
        public string Login { get; set; }
        [Required]
        [MinLength(8)]
        public string Password { get; set; }
    }
}
