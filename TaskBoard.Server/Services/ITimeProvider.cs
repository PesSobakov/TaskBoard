namespace TaskBoard.Server.Services
{
    public interface ITimeProvider
    {
       public DateTime UtcNow();
    }
}
