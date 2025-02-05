namespace TaskBoard.Server.Models.TaskBoardDatabase
{
    public class User
    {
        public int Id { get; set; }
        public string Login { get; set; }
        public string PasswordHash { get; set; }
        public List<Board> Boards { get; set; }
        public List<Access> Accesses { get; set; }
    }
}
