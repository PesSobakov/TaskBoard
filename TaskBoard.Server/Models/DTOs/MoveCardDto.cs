using System.ComponentModel.DataAnnotations;
using TaskBoard.Server.Models.TaskBoardDatabase;

namespace TaskBoard.Server.Models.DTOs
{
    public class MoveCardDto
    {
        [Required]
        public int ListId { get; set; }
    }
}
