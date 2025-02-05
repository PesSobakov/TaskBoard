using System.ComponentModel.DataAnnotations;
using TaskBoard.Server.Models.TaskBoardDatabase;

namespace TaskBoard.Server.Models.DTOs
{
    public class EditCommentDto
    {
        [Required]
        public string Text { get; set; }
    }
}
