using System.ComponentModel.DataAnnotations;
using TaskBoard.Server.Models.TaskBoardDatabase;

namespace TaskBoard.Server.Models.DTOs
{
    public class CreateListDto
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public int BoardId { get; set; }

    }
}
