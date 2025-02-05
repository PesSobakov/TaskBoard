using System.ComponentModel.DataAnnotations;

namespace TaskBoard.Server.Models.DTOs
{
    public class CreateBoardDto
    {
        [Required]
        public string Name { get; set; } = "";
        public string Description { get; set; } = "";
    }
}
