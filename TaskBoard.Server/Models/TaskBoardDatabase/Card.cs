namespace TaskBoard.Server.Models.TaskBoardDatabase
{
    public class Card
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; } = "";
        public string? Status { get; set; }
        public int Order { get; set; }
        public DateOnly? DueDate { get; set; }
        public List<Comment> Comments { get; set; } = null!;
        public int  ListId { get;set; }
        public List List { get; set; } = null!;
    }
}
