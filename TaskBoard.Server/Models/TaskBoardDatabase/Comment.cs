namespace TaskBoard.Server.Models.TaskBoardDatabase
{
    public class Comment
    {
        public int Id { get; set; }
        public string Text { get; set; } = "";
        public DateTime Created { get; set; }
        public DateTime? Edited { get; set; }
        public int UserId { get; set; }
        public User User { get; set; } = null!;
        public int CardId { get; set; }
        public Card Card { get; set; } = null!;
    }
}
