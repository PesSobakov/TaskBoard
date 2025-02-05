using System.ComponentModel.DataAnnotations;
using TaskBoard.Server.Models.TaskBoardDatabase;

namespace TaskBoard.Server.Models.DTOs
{
    public class RemoveAccessDto
    {
        [Required]
        public string User { get; set; }
    }
}
