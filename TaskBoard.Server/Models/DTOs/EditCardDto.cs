using System.ComponentModel.DataAnnotations;
using TaskBoard.Server.Models.TaskBoardDatabase;

namespace TaskBoard.Server.Models.DTOs
{
    public class EditCardDto
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string Status { get; set; }
        public DateOnly DueDate { get; set; }
    }
}
