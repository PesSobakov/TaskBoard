namespace TaskBoard.Server.Services
{
    public enum ResponseStatus
    {
        Ok,
        Unauthorized,
        BadRequest,
        Forbidden,
        UserExists,
        UserNotExists,
        WrongPassword,
    }
}
