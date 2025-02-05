using System.ComponentModel.DataAnnotations;
using TaskBoard.Server.Models.TaskBoardDatabase;

namespace TaskBoard.Server.Models.DTOs
{
    public class ChangeCardOrderDto
    {
        [Required]
        [Range(0, int.MaxValue)]
        public int Order { get; set; }
    }
}
