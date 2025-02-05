namespace TaskBoard.Server.Models.TaskBoardDatabase
{
    public class Access
    {
        public int Id { get; set; }
        public int BoardId { get; set; }
        public Board Board { get; set; } = null!;
        public Permission Permission { get; set; }
        public int UserId { get; set; }
        public User User { get; set; } = null!;
    }
}
