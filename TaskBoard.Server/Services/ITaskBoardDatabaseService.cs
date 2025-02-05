using TaskBoard.Server.Models.DTOs;
using TaskBoard.Server.Models.DTOs.TaskBoard;
using TaskBoard.Server.Models.TaskBoardDatabase;

namespace TaskBoard.Server.Services
{
    public interface ITaskBoardDatabaseService
    {
        public Task<ServiceResponse<bool>> IsUserExist(string login);
        public Task<ServiceResponse<User>> Register(RegisterDto registerDto);
        public Task<ServiceResponse<User>> Login(LoginDto loginDto);
        public Task<ServiceResponse<User>> GetUser(string login);
        public Task<ServiceResponse> DeleteAccount(string login);

        public Task<ServiceResponse> CreateBoard(string login, CreateBoardDto createBoardDto);
        public Task<ServiceResponse<List<Board>>> GetBoards(string login);
        public Task<ServiceResponse<Board>> GetBoard(string login,int id);
        public Task<ServiceResponse> EditBoard(string login,int id, EditBoardDto editBoardDto);
        public Task<ServiceResponse> DeleteBoard(string login,int id);

        public Task<ServiceResponse> CreateList(string login, CreateListDto createListDto);
        public Task<ServiceResponse> EditList(string login, int id, EditListDto createListDto);
        public Task<ServiceResponse> ChangeListOrder(string login, int id, ChangeListOrderDto createListDto);
        public Task<ServiceResponse> DeleteList(string login, int id);

        public Task<ServiceResponse> CreateCard(string login, CreateCardDto createCardDto);
        public Task<ServiceResponse<Card>> GetCard(string login, int id);
        public Task<ServiceResponse> EditCard(string login, int id, EditCardDto editCardDto);
        public Task<ServiceResponse> MoveCard(string login, int id, MoveCardDto moveCardDto);
        public Task<ServiceResponse> ChangeCardOrder(string login, int id, ChangeCardOrderDto changeCardOrderDto);
        public Task<ServiceResponse> DeleteCard(string login, int id);

        public Task<ServiceResponse> CreateComment(string login, CreateCommentDto createCommentDto);
        public Task<ServiceResponse> EditComment(string login, int id, EditCommentDto editCommentDto);
        public Task<ServiceResponse> DeleteComment(string login, int id);

        public Task<ServiceResponse> SetBoardPrivate(string login, int id);
        public Task<ServiceResponse> SetBoardPublic(string login, int id);

        public Task<ServiceResponse> GiveAccess(string login, int id, GiveAccessDto giveAccessDto);
        public Task<ServiceResponse> RemoveAccess(string login, int id, RemoveAccessDto removeAccessDto);

        public Task<ServiceResponse> Seed();
    }
}
