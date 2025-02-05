using System.ComponentModel.DataAnnotations;
using TaskBoard.Server.Models.TaskBoardDatabase;

namespace TaskBoard.Server.Models.DTOs
{
    public class EditListDto
    {
        [Required]
        public string Name { get; set; }
    }
}
