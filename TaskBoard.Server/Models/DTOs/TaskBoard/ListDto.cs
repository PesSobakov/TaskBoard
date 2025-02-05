using System.ComponentModel.DataAnnotations;
using TaskBoard.Server.Models.TaskBoardDatabase;

namespace TaskBoard.Server.Models.DTOs.TaskBoard
{
    public class ListDto
    {
        public int Id { get; set; }
        public int Order { get; set; }
        public string Name { get; set; } = "";
        public List<CardDto> Cards { get; set; } = null!;
    }
}
