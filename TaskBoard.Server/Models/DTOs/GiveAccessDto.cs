using System.ComponentModel.DataAnnotations;
using TaskBoard.Server.Models.TaskBoardDatabase;

namespace TaskBoard.Server.Models.DTOs
{
    public class GiveAccessDto
    {
        [Required]
        public Permission Permission { get; set; }
        [Required]
        public string User { get; set; }
    }
}
