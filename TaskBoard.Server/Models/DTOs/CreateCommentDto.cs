using System.ComponentModel.DataAnnotations;
using TaskBoard.Server.Models.TaskBoardDatabase;

namespace TaskBoard.Server.Models.DTOs
{
    public class CreateCommentDto
    {
        [Required]
        public int CardId { get; set; }
        [Required]
        public string Text { get; set; }
    }
}
