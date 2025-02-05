using System.ComponentModel.DataAnnotations;
using TaskBoard.Server.Models.TaskBoardDatabase;

namespace TaskBoard.Server.Models.DTOs
{
    public class UserDto
    {
        public int Id { get; set; }
        public string Login { get; set; } = "";
    }
}
