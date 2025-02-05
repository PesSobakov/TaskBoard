using System.ComponentModel.DataAnnotations;

namespace TaskBoard.Server.Models.DTOs
{
    public class EditBoardDto
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
    }
}
