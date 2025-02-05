using System.ComponentModel.DataAnnotations;
using TaskBoard.Server.Models.TaskBoardDatabase;

namespace TaskBoard.Server.Models.DTOs.TaskBoard
{
    public class BoardDto
    {
        public int Id { get; set; }
        public List<ListDto> Lists { get; set; } = null!;
        public string Name { get; set; } = "";
        public string Description { get; set; } = "";
        public Privatness Privatness { get; set; } = Privatness.Private;

    }
}
