namespace TaskBoard.Server.Models.TaskBoardDatabase
{
    public class List
    {
        public int Id { get; set; }
        public int BoardId { get; set; }
        public Board Board { get; set; } = null!;
        public int Order { get; set; }
        public string Name { get; set; } = "";
        public List<Card> Cards { get; set; } = null!;

    }
}
