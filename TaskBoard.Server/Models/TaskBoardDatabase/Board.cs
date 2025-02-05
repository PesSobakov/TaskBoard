namespace TaskBoard.Server.Models.TaskBoardDatabase
{
    public class Board
    {
        public int Id { get; set; }
        public List<List> Lists { get; set; } = null!;
        public string Name { get; set; } = "";
        public string Description { get; set; } = "";
        public User User { get; set; } = null!;
        public int UserId { get; set; }
        public Privatness Privatness { get; set; } = Privatness.Private;
        public List<Access> Accesses { get; set; } = null!;

    }
}
