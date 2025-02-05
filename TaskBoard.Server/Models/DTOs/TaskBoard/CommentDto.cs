using System.ComponentModel.DataAnnotations;
using TaskBoard.Server.Models.TaskBoardDatabase;

namespace TaskBoard.Server.Models.DTOs.TaskBoard
{
    public class CommentDto
    {
        public int Id { get; set; }
        public string Text { get; set; } = "";
        public DateTime Created { get; set; }
        public DateTime? Edited { get; set; }
        public UserDto User { get; set; } = null!;
    }
}
