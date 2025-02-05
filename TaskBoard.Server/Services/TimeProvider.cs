namespace TaskBoard.Server.Services
{
    public class TimeProvider:ITimeProvider
    {
        public DateTime UtcNow()
        {
            return DateTime.UtcNow;
        }
    }
}
