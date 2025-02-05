using System.ComponentModel.DataAnnotations;
using TaskBoard.Server.Models.TaskBoardDatabase;

namespace TaskBoard.Server.Models.DTOs.TaskBoard
{
    public class CardDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Status { get; set; }
        public int Order { get; set; }
        public DateOnly DueDate { get; set; }
        public List<CommentDto> Comments { get; set; } = null!;
    }
}
