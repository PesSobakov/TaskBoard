using static System.Runtime.InteropServices.JavaScript.JSType;

namespace TaskBoard.Server.Services
{
    public class ServiceResponse<T>
    {
        public T? Data { get; set; }
        public ResponseStatus Status { get; set; }
        public static implicit operator ServiceResponse<T>(ServiceResponse serviceResponse) => new ServiceResponse<T>() { Status = serviceResponse.Status };
    }
    public class ServiceResponse
    {
        public ResponseStatus Status { get; set; }
    }
}
