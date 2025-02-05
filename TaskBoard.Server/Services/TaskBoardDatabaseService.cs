using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TaskBoard.Server.Models.DTOs;
using TaskBoard.Server.Models.DTOs.TaskBoard;
using TaskBoard.Server.Models.TaskBoardDatabase;

namespace TaskBoard.Server.Services
{
    public class TaskBoardDatabaseService : ITaskBoardDatabaseService
    {
        private readonly TaskBoardContext _taskBoardContext;
        private readonly ITimeProvider _timeProvider;
        public TaskBoardDatabaseService(TaskBoardContext context, ITimeProvider timeProvider)
        {
            _taskBoardContext = context;
            _timeProvider = timeProvider;
        }

        private string HashString(string inputString)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(inputString);
            byte[] hashedBytes = System.Security.Cryptography.SHA256.HashData(bytes);
            return Convert.ToBase64String(hashedBytes);
        }

        public async Task<ServiceResponse<bool>> IsUserExist(string login)
        {
            return new ServiceResponse<bool>()
            {
                Data = await _taskBoardContext.Users.Where(user => user.Login == login).AnyAsync(),
                Status = ResponseStatus.Ok
            };
        }
        public async Task<ServiceResponse<User>> Register(RegisterDto registerDto)
        {
            if (await _taskBoardContext.Users.Where(user => user.Login == registerDto.Login).AnyAsync())
            {
                return new ServiceResponse() { Status = ResponseStatus.UserExists };
            }
            string passwordHash = HashString(registerDto.Password);
            User user = new User()
            {
                Login = registerDto.Login,
                PasswordHash = passwordHash
            };
            _taskBoardContext.Add(user);
            await _taskBoardContext.SaveChangesAsync();

            return new ServiceResponse<User>()
            {
                Data = user,
                Status = ResponseStatus.Ok
            };
        }
        public async Task<ServiceResponse<User>> Login(LoginDto loginDto)
        {
            User? user = await _taskBoardContext.Users.Where(user => user.Login == loginDto.Login).FirstOrDefaultAsync();
            if (user == null) { return new ServiceResponse() { Status = ResponseStatus.UserNotExists }; }

            string passwordHash = HashString(loginDto.Password);
            if (passwordHash == user.PasswordHash)
            {
                return new ServiceResponse<User>()
                {
                    Data = user,
                    Status = ResponseStatus.Ok
                };
            }
            else
            {
                return new ServiceResponse() { Status = ResponseStatus.WrongPassword };
            }
        }
        public async Task<ServiceResponse<User>> GetUser(string login)
        {
            User? user = await _taskBoardContext.Users.Where(user => user.Login == login).FirstOrDefaultAsync();
            if (user == null) { return new ServiceResponse() { Status = ResponseStatus.BadRequest }; }

            return new ServiceResponse<User>()
            {
                Data = user,
                Status = ResponseStatus.Ok
            };
        }
        public async Task<ServiceResponse> DeleteAccount(string login)
        {
            User? user = await _taskBoardContext.Users.Where(user => user.Login == login).FirstOrDefaultAsync();
            if (user == null) { return new ServiceResponse() { Status = ResponseStatus.BadRequest }; }

            _taskBoardContext.Remove(user);
            await _taskBoardContext.SaveChangesAsync();
            return new ServiceResponse() { Status = ResponseStatus.Ok };
        }

        public async Task<ServiceResponse> CreateBoard(string login, CreateBoardDto createBoardDto)
        {
            User? user = await _taskBoardContext.Users.Where(user => user.Login == login).FirstOrDefaultAsync();
            if (user == null) { return new ServiceResponse() { Status = ResponseStatus.Unauthorized }; }

            Board board = new Board()
            {
                Name = createBoardDto.Name,
                Description = createBoardDto.Description,
                User = user
            };
            _taskBoardContext.Add(board);
            await _taskBoardContext.SaveChangesAsync();
            return new ServiceResponse() { Status = ResponseStatus.Ok };
        }
        public async Task<ServiceResponse<List<Board>>> GetBoards(string login)
        {
            User? user = await _taskBoardContext.Users.Where(user => user.Login == login)
                .Include(user => user.Boards)
                .AsSplitQuery()
                .FirstOrDefaultAsync();
            if (user == null) { return new ServiceResponse() { Status = ResponseStatus.Unauthorized }; }

            return new ServiceResponse<List<Board>>()
            {
                Data = user.Boards,
                Status = ResponseStatus.Ok
            };
        }
        public async Task<ServiceResponse<Board>> GetBoard(string login, int id)
        {
            User? user = await _taskBoardContext.Users.Where(user => user.Login == login).FirstOrDefaultAsync();
            if (user == null) { return new ServiceResponse() { Status = ResponseStatus.Unauthorized }; }

            Board? board = await _taskBoardContext.Boards.Where(board => board.Id == id)
                .Include(board => board.Lists).ThenInclude(list => list.Cards).ThenInclude(card => card.Comments).ThenInclude(comment => comment.User)
                .AsSplitQuery()
                .FirstOrDefaultAsync();
            if (board == null) { return new ServiceResponse() { Status = ResponseStatus.BadRequest }; }

            if (board.UserId != user.Id && board.Privatness == Privatness.Private)
            {
                Access? access = await _taskBoardContext.Accesses
                     .Where(access => access.UserId == user.Id)
                     .Where(access => access.BoardId == board.Id)
                     .FirstOrDefaultAsync();
                if (access == null) { return new ServiceResponse() { Status = ResponseStatus.Forbidden }; }
            }

            return new ServiceResponse<Board>()
            {
                Data = board,
                Status = ResponseStatus.Ok
            };
        }
        public async Task<ServiceResponse> EditBoard(string login, int id, EditBoardDto editBoardDto)
        {
            User? user = await _taskBoardContext.Users.Where(user => user.Login == login).FirstOrDefaultAsync();
            if (user == null) { return new ServiceResponse() { Status = ResponseStatus.Unauthorized }; }

            Board? board = await _taskBoardContext.Boards
                .Where(board => board.Id == id)
                 .FirstOrDefaultAsync();
            if (board == null) { return new ServiceResponse() { Status = ResponseStatus.BadRequest }; }

            if (board.UserId != user.Id)
            {
                Access? access = await _taskBoardContext.Accesses
                     .Where(access => access.UserId == user.Id)
                     .Where(access => access.BoardId == board.Id)
                     .FirstOrDefaultAsync();
                if (!(access?.Permission == Permission.Edit)) { return new ServiceResponse() { Status = ResponseStatus.Forbidden }; }
            }

            if (editBoardDto.Name != null)
            {
                board.Name = editBoardDto.Name;
            }
            if (editBoardDto.Description != null)
            {
                board.Description = editBoardDto.Description;
            }
            await _taskBoardContext.SaveChangesAsync();
            return new ServiceResponse() { Status = ResponseStatus.Ok };
        }
        public async Task<ServiceResponse> DeleteBoard(string login, int id)
        {
            User? user = await _taskBoardContext.Users.Where(user => user.Login == login).FirstOrDefaultAsync();
            if (user == null) { return new ServiceResponse() { Status = ResponseStatus.Unauthorized }; }

            Board? board = await _taskBoardContext.Boards.Where(board => board.Id == id)
              .Include(board => board.Lists).ThenInclude(list => list.Cards).ThenInclude(card => card.Comments)
              .Include(board=>board.Accesses)  
              .AsSplitQuery()
                .FirstOrDefaultAsync();
            if (board == null) { return new ServiceResponse() { Status = ResponseStatus.BadRequest }; }

            if (board.UserId != user.Id)
            {
                Access? access = await _taskBoardContext.Accesses
                     .Where(access => access.UserId == user.Id)
                     .Where(access => access.BoardId == board.Id)
                     .FirstOrDefaultAsync();
                if (!(access?.Permission == Permission.Edit)) { return new ServiceResponse() { Status = ResponseStatus.Forbidden }; }
            }

            _taskBoardContext.Remove(board);
            await _taskBoardContext.SaveChangesAsync();
            return new ServiceResponse() { Status = ResponseStatus.Ok };
        }

        public async Task<ServiceResponse> CreateList(string login, CreateListDto createListDto)
        {
            User? user = await _taskBoardContext.Users.Where(user => user.Login == login).FirstOrDefaultAsync();
            if (user == null) { return new ServiceResponse() { Status = ResponseStatus.Unauthorized }; }

            Board? board = await _taskBoardContext.Boards.Where(board => board.Id == createListDto.BoardId).FirstOrDefaultAsync();
            if (board == null) { return new ServiceResponse() { Status = ResponseStatus.BadRequest }; }

            if (board.UserId != user.Id)
            {
                Access? access = await _taskBoardContext.Accesses
                     .Where(access => access.UserId == user.Id)
                     .Where(access => access.BoardId == board.Id)
                     .FirstOrDefaultAsync();
                if (!(access?.Permission == Permission.Edit)) { return new ServiceResponse() { Status = ResponseStatus.Forbidden }; }
            }

            int order;
            if (!await _taskBoardContext.Lists.Where(list => list.BoardId == board.Id).AnyAsync())
            {
                order = 0;
            }
            else
            {
                order = await _taskBoardContext.Lists.Where(list => list.BoardId == board.Id).MaxAsync(list => list.Order) + 1;
            }
            List list = new List()
            {
                Name = createListDto.Name,
                Board = board,
                Order = order
            };
            _taskBoardContext.Add(list);
            await _taskBoardContext.SaveChangesAsync();
            return new ServiceResponse() { Status = ResponseStatus.Ok };
        }
        public async Task<ServiceResponse> EditList(string login, int id, EditListDto editListDto)
        {
            User? user = await _taskBoardContext.Users.Where(user => user.Login == login).FirstOrDefaultAsync();
            if (user == null) { return new ServiceResponse() { Status = ResponseStatus.Unauthorized }; }

            List? list = await _taskBoardContext.Lists
                .Where(list => list.Id == id)
                .Include(list => list.Board)
                .FirstOrDefaultAsync();
            if (list == null) { return new ServiceResponse() { Status = ResponseStatus.BadRequest }; }

            if (list.Board.UserId != user.Id)
            {
                Access? access = await _taskBoardContext.Accesses
                     .Where(access => access.UserId == user.Id)
                     .Where(access => access.BoardId == list.Board.Id)
                     .FirstOrDefaultAsync();
                if (!(access?.Permission == Permission.Edit)) { return new ServiceResponse() { Status = ResponseStatus.Forbidden }; }
            }


            list.Name = editListDto.Name;
            await _taskBoardContext.SaveChangesAsync();
            return new ServiceResponse() { Status = ResponseStatus.Ok };
        }
        public async Task<ServiceResponse> ChangeListOrder(string login, int id, ChangeListOrderDto changeListOrderDto)
        {
            User? user = await _taskBoardContext.Users.Where(user => user.Login == login).FirstOrDefaultAsync();
            if (user == null) { return new ServiceResponse() { Status = ResponseStatus.Unauthorized }; }

            List? list = await _taskBoardContext.Lists
                .Where(list => list.Id == id)
                .Include(list => list.Board)
                .FirstOrDefaultAsync();
            if (list == null) { return new ServiceResponse() { Status = ResponseStatus.BadRequest }; }

            if (list.Board.UserId != user.Id)
            {
                Access? access = await _taskBoardContext.Accesses
                     .Where(access => access.UserId == user.Id)
                     .Where(access => access.BoardId == list.Board.Id)
                     .FirstOrDefaultAsync();
                if (!(access?.Permission == Permission.Edit)) { return new ServiceResponse() { Status = ResponseStatus.Forbidden }; }
            }

            int boardId = list.Board.Id;
            int oldOrder = list.Order;
            int maxOrder = await _taskBoardContext.Lists.Where(list => list.Board.Id == boardId).MaxAsync(list => list.Order);
            int newOrder = changeListOrderDto.Order < maxOrder ? changeListOrderDto.Order : maxOrder;
            int from;
            int to;
            if (oldOrder > newOrder)
            {
                from = newOrder;
                to = oldOrder - 1;
                await _taskBoardContext.Lists.Where(list => list.Board.Id == boardId).Where(list => list.Order >= from && list.Order <= to).ExecuteUpdateAsync(s => s.SetProperty(list => list.Order, list => list.Order + 1));
            }
            else
            {
                from = oldOrder + 1;
                to = newOrder;
                await _taskBoardContext.Lists.Where(list => list.Board.Id == boardId).Where(list => list.Order >= from && list.Order <= to).ExecuteUpdateAsync(s => s.SetProperty(list => list.Order, list => list.Order - 1));
            }
            list.Order = newOrder;
            await _taskBoardContext.SaveChangesAsync();
            return new ServiceResponse() { Status = ResponseStatus.Ok };
        }
        public async Task<ServiceResponse> DeleteList(string login, int id)
        {
            User? user = await _taskBoardContext.Users.Where(user => user.Login == login).FirstOrDefaultAsync();
            if (user == null) { return new ServiceResponse() { Status = ResponseStatus.Unauthorized }; }

            List? list = await _taskBoardContext.Lists.Where(list => list.Id == id)
                .Include(list => list.Cards).ThenInclude(card => card.Comments)
                .Include(list => list.Board)
                .AsSplitQuery()
                .FirstOrDefaultAsync();
            if (list == null) { return new ServiceResponse() { Status = ResponseStatus.BadRequest }; }

            if (list.Board.UserId != user.Id)
            {
                Access? access = await _taskBoardContext.Accesses
                     .Where(access => access.UserId == user.Id)
                     .Where(access => access.BoardId == list.Board.Id)
                     .FirstOrDefaultAsync();
                if (!(access?.Permission == Permission.Edit)) { return new ServiceResponse() { Status = ResponseStatus.Forbidden }; }
            }

            int boardId = list.Board.Id;
            int oldOrder = list.Order;
            int maxOrder = await _taskBoardContext.Lists.Where(list => list.Board.Id == boardId).MaxAsync(list => list.Order);
            await _taskBoardContext.Lists.Where(list => list.Board.Id == boardId).Where(list => list.Order > oldOrder && list.Order <= maxOrder).ExecuteUpdateAsync(s => s.SetProperty(list => list.Order, list => list.Order - 1));


            _taskBoardContext.Remove(list);
            await _taskBoardContext.SaveChangesAsync();
            return new ServiceResponse() { Status = ResponseStatus.Ok };
        }

        public async Task<ServiceResponse> CreateCard(string login, CreateCardDto createCardDto)
        {
            User? user = await _taskBoardContext.Users.Where(user => user.Login == login).FirstOrDefaultAsync();
            if (user == null) { return new ServiceResponse() { Status = ResponseStatus.Unauthorized }; }

            List? list = await _taskBoardContext.Lists
                .Where(list => list.Id == createCardDto.ListId)
                .Include(list => list.Board)
                .FirstOrDefaultAsync();
            if (list == null) { return new ServiceResponse() { Status = ResponseStatus.BadRequest }; }

            if (list.Board.UserId != user.Id)
            {
                Access? access = await _taskBoardContext.Accesses
                     .Where(access => access.UserId == user.Id)
                     .Where(access => access.BoardId == list.Board.Id)
                     .FirstOrDefaultAsync();
                if (!(access?.Permission == Permission.Edit)) { return new ServiceResponse() { Status = ResponseStatus.Forbidden }; }
            }

            int order;
            if (!await _taskBoardContext.Cards.Where(Card => Card.ListId == list.Id).AnyAsync())
            {
                order = 0;
            }
            else
            {
                order = await _taskBoardContext.Cards.Where(card => card.ListId == list.Id).MaxAsync(card => card.Order) + 1;
            }
            Card card = new Card()
            {
                List = list,
                Name = createCardDto.Name,
                Description = createCardDto.Description,
                Status = createCardDto.Status,
                DueDate = createCardDto.DueDate,
                Order = order,
            };
            _taskBoardContext.Add(card);
            await _taskBoardContext.SaveChangesAsync();
            return new ServiceResponse() { Status = ResponseStatus.Ok };
        }
        public async Task<ServiceResponse<Card>> GetCard(string login, int id)
        {
            User? user = await _taskBoardContext.Users.Where(user => user.Login == login).FirstOrDefaultAsync();
            if (user == null) { return new ServiceResponse() { Status = ResponseStatus.Unauthorized }; }

            Card? card = await _taskBoardContext.Cards
                .Where(card => card.Id == id)
                .Include(card => card.Comments).ThenInclude(comment => comment.User)
                .Include(card => card.List).ThenInclude(list => list.Board)
                .AsSplitQuery()
                .FirstOrDefaultAsync();
            if (card == null) { return new ServiceResponse() { Status = ResponseStatus.BadRequest }; }

            if (card.List.Board.UserId != user.Id && card.List.Board.Privatness == Privatness.Private)
            {
                Access? access = await _taskBoardContext.Accesses
                     .Where(access => access.UserId == user.Id)
                     .Where(access => access.BoardId == card.List.Board.Id)
                     .FirstOrDefaultAsync();
                if (access == null) { return new ServiceResponse() { Status = ResponseStatus.Forbidden }; }
            }

            return new ServiceResponse<Card>()
            {
                Data = card,
                Status = ResponseStatus.Ok
            };
        }
        public async Task<ServiceResponse> EditCard(string login, int id, EditCardDto editCardDto)
        {
            User? user = await _taskBoardContext.Users.Where(user => user.Login == login).FirstOrDefaultAsync();
            if (user == null) { return new ServiceResponse() { Status = ResponseStatus.Unauthorized }; }

            Card? card = await _taskBoardContext.Cards.Where(card => card.Id == id)
                .Include(card => card.List).ThenInclude(list => list.Board)
                .FirstOrDefaultAsync();
            if (card == null) { return new ServiceResponse() { Status = ResponseStatus.BadRequest }; }

            if (card.List.Board.UserId != user.Id)
            {
                Access? access = await _taskBoardContext.Accesses
                     .Where(access => access.UserId == user.Id)
                     .Where(access => access.BoardId == card.List.Board.Id)
                     .FirstOrDefaultAsync();
                if (!(access?.Permission == Permission.Edit)) { return new ServiceResponse() { Status = ResponseStatus.Forbidden }; }
            }

            if (editCardDto.Name != null)
            {
                card.Name = editCardDto.Name;
            }
            if (editCardDto.Description != null)
            {
                card.Description = editCardDto.Description;
            }
            if (editCardDto.Status != null)
            {
                card.Status = editCardDto.Status;
            }
            if (editCardDto.DueDate != null)
            {
                card.DueDate = editCardDto.DueDate;
            }
            await _taskBoardContext.SaveChangesAsync();
            return new ServiceResponse() { Status = ResponseStatus.Ok };
        }
        public async Task<ServiceResponse> MoveCard(string login, int id, MoveCardDto moveCardDto)
        {
            User? user = await _taskBoardContext.Users.Where(user => user.Login == login).FirstOrDefaultAsync();
            if (user == null) { return new ServiceResponse() { Status = ResponseStatus.Unauthorized }; }

            Card? card = await _taskBoardContext.Cards
                .Where(card => card.Id == id)
                .Include(card => card.List).ThenInclude(list => list.Board)
                .FirstOrDefaultAsync();
            if (card == null) { return new ServiceResponse() { Status = ResponseStatus.BadRequest }; }

            List? list = await _taskBoardContext.Lists.Where(list => list.Id == moveCardDto.ListId).FirstOrDefaultAsync();
            if (list == null) { return new ServiceResponse() { Status = ResponseStatus.BadRequest }; }

            if (card.List.Board.UserId != user.Id)
            {
                Access? access = await _taskBoardContext.Accesses
                     .Where(access => access.UserId == user.Id)
                     .Where(access => access.BoardId == card.List.Board.Id)
                     .FirstOrDefaultAsync();
                if (!(access?.Permission == Permission.Edit)) { return new ServiceResponse() { Status = ResponseStatus.Forbidden }; }
            }

            int oldListId = card.List.Id;
            int oldOrder = card.Order;
            int maxOrder = await _taskBoardContext.Cards.Where(card => card.List.Id == oldListId).MaxAsync(card => card.Order);
            await _taskBoardContext.Cards.Where(card=>card.List.Id== oldListId).Where(card => card.Order > oldOrder && card.Order <= maxOrder).ExecuteUpdateAsync(s => s.SetProperty(card => card.Order, card => card.Order - 1));

            int newOrder;
            if (await _taskBoardContext.Cards.Where(card => card.List.Id == list.Id).AnyAsync()) 
            {
                newOrder = (await _taskBoardContext.Cards.Where(card => card.List.Id == list.Id).MaxAsync(card => card.Order)) + 1;
            }
            else
            {
                newOrder = 0;
            }
             
            card.Order = newOrder;
            card.List = list;
            await _taskBoardContext.SaveChangesAsync();
            return new ServiceResponse() { Status = ResponseStatus.Ok };
        }
        public async Task<ServiceResponse> ChangeCardOrder(string login, int id, ChangeCardOrderDto changeCardOrderDto)
        {
            User? user = await _taskBoardContext.Users.Where(user => user.Login == login).FirstOrDefaultAsync();
            if (user == null) { return new ServiceResponse() { Status = ResponseStatus.Unauthorized }; }

            Card? card = await _taskBoardContext.Cards
                .Where(card => card.Id == id)
                .Include(card => card.List).ThenInclude(list => list.Board)
                .FirstOrDefaultAsync();
            if (card == null) { return new ServiceResponse() { Status = ResponseStatus.BadRequest }; }

            if (card.List.Board.UserId != user.Id)
            {
                Access? access = await _taskBoardContext.Accesses
                     .Where(access => access.UserId == user.Id)
                     .Where(access => access.BoardId == card.List.Board.Id)
                     .FirstOrDefaultAsync();
                if (!(access?.Permission == Permission.Edit)) { return new ServiceResponse() { Status = ResponseStatus.Forbidden }; }
            }

            int listId = card.List.Id;
            int oldOrder = card.Order;
            int maxOrder = await _taskBoardContext.Cards.Where(card => card.List.Id == listId).MaxAsync(card => card.Order);
            int newOrder = changeCardOrderDto.Order < maxOrder ? changeCardOrderDto.Order : maxOrder;
            int from;
            int to;
            if (oldOrder > newOrder)
            {
                from = newOrder;
                to = oldOrder - 1;
                await _taskBoardContext.Cards.Where(card => card.List.Id == listId).Where(card => card.Order >= from && card.Order <= to).ExecuteUpdateAsync(s => s.SetProperty(card => card.Order, card => card.Order + 1));
            }
            else
            {
                from = oldOrder + 1;
                to = newOrder;
                await _taskBoardContext.Cards.Where(card => card.List.Id == listId).Where(card => card.Order >= from && card.Order <= to).ExecuteUpdateAsync(s => s.SetProperty(card => card.Order, card => card.Order - 1));
            }
            card.Order = newOrder;
            await _taskBoardContext.SaveChangesAsync();
            return new ServiceResponse() { Status = ResponseStatus.Ok };
        }
        public async Task<ServiceResponse> DeleteCard(string login, int id)
        {
            User? user = await _taskBoardContext.Users.Where(user => user.Login == login).FirstOrDefaultAsync();
            if (user == null) { return new ServiceResponse() { Status = ResponseStatus.Unauthorized }; }

            Card? card = await _taskBoardContext.Cards.Where(card => card.Id == id)
                .Include(card => card.Comments)
                .Include(card => card.List).ThenInclude(list => list.Board)
                .AsSplitQuery()
                .FirstOrDefaultAsync();
            if (card == null) { return new ServiceResponse() { Status = ResponseStatus.BadRequest }; }

            if (card.List.Board.UserId != user.Id)
            {
                Access? access = await _taskBoardContext.Accesses
                     .Where(access => access.UserId == user.Id)
                     .Where(access => access.BoardId == card.List.Board.Id)
                     .FirstOrDefaultAsync();
                if (!(access?.Permission == Permission.Edit)) { return new ServiceResponse() { Status = ResponseStatus.Forbidden }; }
            }

            int oldListId = card.List.Id;
            int oldOrder = card.Order;
            int maxOrder = await _taskBoardContext.Cards.Where(card => card.List.Id == oldListId).MaxAsync(card => card.Order);
            await _taskBoardContext.Cards.Where(card => card.List.Id == oldListId).Where(card => card.Order > oldOrder && card.Order <= maxOrder).ExecuteUpdateAsync(s => s.SetProperty(card => card.Order, card => card.Order - 1));

            _taskBoardContext.Remove(card);
            await _taskBoardContext.SaveChangesAsync();
            return new ServiceResponse() { Status = ResponseStatus.Ok };
        }

        public async Task<ServiceResponse> CreateComment(string login, CreateCommentDto createCommentDto)
        {
            User? user = await _taskBoardContext.Users.Where(user => user.Login == login).FirstOrDefaultAsync();
            if (user == null) { return new ServiceResponse() { Status = ResponseStatus.Unauthorized }; }

            Card? card = await _taskBoardContext.Cards
                .Where(card => card.Id == createCommentDto.CardId)
                .Include(card => card.List).ThenInclude(list => list.Board)
                .FirstOrDefaultAsync();
            if (card == null) { return new ServiceResponse() { Status = ResponseStatus.BadRequest }; }

            if (card.List.Board.UserId != user.Id && card.List.Board.Privatness == Privatness.Private)
            {
                Access? access = await _taskBoardContext.Accesses
                     .Where(access => access.UserId == user.Id)
                     .Where(access => access.BoardId == card.List.Board.Id)
                     .FirstOrDefaultAsync();
                if (access == null) { return new ServiceResponse() { Status = ResponseStatus.Forbidden }; }
            }

            Comment comment = new Comment()
            {
                Text = createCommentDto.Text,
                Created = _timeProvider.UtcNow(),
                Edited = null,
                User = user,
                Card = card
            };
            _taskBoardContext.Add(comment);
            await _taskBoardContext.SaveChangesAsync();
            return new ServiceResponse() { Status = ResponseStatus.Ok };

        }
        public async Task<ServiceResponse> EditComment(string login, int id, EditCommentDto editCommentDto)
        {
            User? user = await _taskBoardContext.Users.Where(user => user.Login == login).FirstOrDefaultAsync();
            if (user == null) { return new ServiceResponse() { Status = ResponseStatus.Unauthorized }; }

            Comment? comment = await _taskBoardContext.Comments
                .Where(comment => comment.Id == id)
                .Include(comment => comment.Card).ThenInclude(card => card.List).ThenInclude(list => list.Board)
                .FirstOrDefaultAsync();
            if (comment == null) { return new ServiceResponse() { Status = ResponseStatus.BadRequest }; }

            if (comment.Card.List.Board.UserId != user.Id && comment.Card.List.Board.Privatness == Privatness.Private)
            {
                Access? access = await _taskBoardContext.Accesses
                     .Where(access => access.UserId == user.Id)
                     .Where(access => access.BoardId == comment.Card.List.Board.Id)
                     .FirstOrDefaultAsync();
                if (access == null) { return new ServiceResponse() { Status = ResponseStatus.Forbidden }; }
            }
            if (comment.UserId != user.Id) { return new ServiceResponse() { Status = ResponseStatus.Forbidden }; }

            comment.Text = editCommentDto.Text;
            comment.Edited = _timeProvider.UtcNow();
            await _taskBoardContext.SaveChangesAsync();
            return new ServiceResponse() { Status = ResponseStatus.Ok };
        }
        public async Task<ServiceResponse> DeleteComment(string login, int id)
        {
            User? user = await _taskBoardContext.Users.Where(user => user.Login == login).FirstOrDefaultAsync();
            if (user == null) { return new ServiceResponse() { Status = ResponseStatus.Unauthorized }; }

            Comment? comment = await _taskBoardContext.Comments
                .Where(comment => comment.Id == id)
                .Include(comment => comment.Card).ThenInclude(card => card.List).ThenInclude(list => list.Board)
                .FirstOrDefaultAsync();
            if (comment == null) { return new ServiceResponse() { Status = ResponseStatus.BadRequest }; }

            if (comment.Card.List.Board.UserId != user.Id && comment.Card.List.Board.Privatness == Privatness.Private)
            {
                Access? access = await _taskBoardContext.Accesses
                     .Where(access => access.UserId == user.Id)
                     .Where(access => access.BoardId == comment.Card.List.Board.Id)
                     .FirstOrDefaultAsync();
                if (access == null) { return new ServiceResponse() { Status = ResponseStatus.Forbidden }; }
            }
            if (comment.UserId != user.Id) { return new ServiceResponse() { Status = ResponseStatus.Forbidden }; }

            _taskBoardContext.Remove(comment);
            await _taskBoardContext.SaveChangesAsync();
            return new ServiceResponse() { Status = ResponseStatus.Ok };
        }

        public async Task<ServiceResponse> SetBoardPrivate(string login, int id)
        {
            User? user = await _taskBoardContext.Users.Where(user => user.Login == login).FirstOrDefaultAsync();
            if (user == null) { return new ServiceResponse() { Status = ResponseStatus.Unauthorized }; }

            Board? board = await _taskBoardContext.Boards.Where(board => board.Id == id).FirstOrDefaultAsync();
            if (board == null) { return new ServiceResponse() { Status = ResponseStatus.BadRequest }; }

            if (board.UserId != user.Id) { return new ServiceResponse() { Status = ResponseStatus.Forbidden }; }

            board.Privatness = Privatness.Private;
            await _taskBoardContext.SaveChangesAsync();
            return new ServiceResponse() { Status = ResponseStatus.Ok };
        }
        public async Task<ServiceResponse> SetBoardPublic(string login, int id)
        {
            User? user = await _taskBoardContext.Users.Where(user => user.Login == login).FirstOrDefaultAsync();
            if (user == null) { return new ServiceResponse() { Status = ResponseStatus.Unauthorized }; }

            Board? board = await _taskBoardContext.Boards.Where(board => board.Id == id).FirstOrDefaultAsync();
            if (board == null) { return new ServiceResponse() { Status = ResponseStatus.BadRequest }; }

            if (board.UserId != user.Id) { return new ServiceResponse() { Status = ResponseStatus.Forbidden }; }

            board.Privatness = Privatness.Public;
            await _taskBoardContext.SaveChangesAsync();
            return new ServiceResponse() { Status = ResponseStatus.Ok };
        }

        public async Task<ServiceResponse> GiveAccess(string login, int id, GiveAccessDto giveAccessDto)
        {
            User? user = await _taskBoardContext.Users.Where(user => user.Login == login).FirstOrDefaultAsync();
            if (user == null) { return new ServiceResponse() { Status = ResponseStatus.Unauthorized }; }

            Board? board = await _taskBoardContext.Boards.Where(board => board.Id == id).FirstOrDefaultAsync();
            if (board == null) { return new ServiceResponse() { Status = ResponseStatus.BadRequest }; }

            if (board.UserId != user.Id) { return new ServiceResponse() { Status = ResponseStatus.Forbidden }; }

            User? accessUser = await _taskBoardContext.Users.Where(user => user.Login == giveAccessDto.User).FirstOrDefaultAsync();
            if (accessUser == null) { return new ServiceResponse() { Status = ResponseStatus.BadRequest }; }

            Access? access = await _taskBoardContext.Accesses
                 .Where(access => access.UserId == accessUser.Id)
                 .Where(access => access.BoardId == board.Id)
                 .FirstOrDefaultAsync();
            if (access == null)
            {
                access = new()
                {
                    User = accessUser,
                    Board = board,
                    Permission = giveAccessDto.Permission
                };
                _taskBoardContext.Add(access);
            }
            else
            {
                access.Permission = giveAccessDto.Permission;
            }
            await _taskBoardContext.SaveChangesAsync();
            return new ServiceResponse() { Status = ResponseStatus.Ok };
        }
        public async Task<ServiceResponse> RemoveAccess(string login, int id, RemoveAccessDto removeAccessDto)
        {
            User? user = await _taskBoardContext.Users.Where(user => user.Login == login).FirstOrDefaultAsync();
            if (user == null) { return new ServiceResponse() { Status = ResponseStatus.Unauthorized }; }

            Board? board = await _taskBoardContext.Boards.Where(board => board.Id == id).FirstOrDefaultAsync();
            if (board == null) { return new ServiceResponse() { Status = ResponseStatus.BadRequest }; }

            if (board.UserId != user.Id) { return new ServiceResponse() { Status = ResponseStatus.Forbidden }; }

            User? accessUser = await _taskBoardContext.Users.Where(user => user.Login == removeAccessDto.User).FirstOrDefaultAsync();
            if (accessUser == null) { return new ServiceResponse() { Status = ResponseStatus.BadRequest }; }

            Access? access = await _taskBoardContext.Accesses
                 .Where(access => access.UserId == accessUser.Id)
                 .Where(access => access.BoardId == board.Id)
                 .FirstOrDefaultAsync();
            if (access == null)
            {
                return new ServiceResponse() { Status = ResponseStatus.Ok };
            }
            else
            {
                _taskBoardContext.Remove(access);
                await _taskBoardContext.SaveChangesAsync();
            }
            await _taskBoardContext.SaveChangesAsync();
            return new ServiceResponse() { Status = ResponseStatus.Ok };
        }

        public async Task<ServiceResponse> Seed()
        {
           await _taskBoardContext.Database.EnsureDeletedAsync();
           await _taskBoardContext.Database.EnsureCreatedAsync();

            List<User> users =
            [
                new User
                {
                    Login = "user@example.com",
                    PasswordHash = Convert.ToBase64String(System.Security.Cryptography.SHA256.HashData(Encoding.UTF8.GetBytes("password"))),
                    Boards =
                    [
                        new Board(){
                            Name = "Board 1",
                            Description = "Board description 1",
                            Privatness = Privatness.Public,
                            Lists = [
                                new List(){
                                    Name = "List 1",
                                    Order = 0,
                                    Cards = [
                                        new Card(){
                                            Name="Card 1",
                                            Description="Card description",
                                            Status = "Status",
                                            DueDate = new DateOnly(2025,1,1),
                                            Order=0,
                                        },
                                        new Card(){
                                            Name="Card 2",
                                            Description="Card description",
                                            Status = "Status",
                                            DueDate = new DateOnly(2025,1,1),
                                            Order=1,
                                        },
                                        new Card(){
                                            Name="Card 3",
                                            Description="Card description",
                                            Status = "Status",
                                            DueDate = new DateOnly(2025,1,1),
                                            Order=2,
                                        },
                                    ]
                                },
                                new List(){
                                    Name = "List 2",
                                    Order = 1,
                                    Cards = [
                                        new Card(){
                                            Name="Card 4",
                                            Description="Card description",
                                            Status = "Status",
                                            DueDate = new DateOnly(2025,1,1),
                                            Order=0,
                                        },
                                        new Card(){
                                            Name="Card 5",
                                            Description="Card description",
                                            Status = "Status",
                                            DueDate = new DateOnly(2025,1,1),
                                            Order=1,
                                        },
                                        new Card(){
                                            Name="Card 6",
                                            Description="Card description",
                                            Status = "Status",
                                            DueDate = new DateOnly(2025,1,1),
                                            Order=2,
                                        },
                                    ]
                                },
                                new List(){
                                    Name = "List 3",
                                    Order = 2,
                                    Cards = [
                                        new Card(){
                                            Name="Card 7",
                                            Description="Card description",
                                            Status = "Status",
                                            DueDate = new DateOnly(2025,1,1),
                                            Order=0,
                                        },
                                        new Card(){
                                            Name="Card 8",
                                            Description="Card description",
                                            Status = "Status",
                                            DueDate = new DateOnly(2025,1,1),
                                            Order=1,
                                        },
                                        new Card(){
                                            Name="Card 9",
                                            Description="Card description",
                                            Status = "Status",
                                            DueDate = new DateOnly(2025,1,1),
                                            Order=2,
                                        },
                                    ]
                                },

                            ]
                        },
                        new Board(){
                            Name = "Board 2",
                            Description = "Board description 2",
                            Privatness = Privatness.Private,
                            Lists = [
                                new List(){
                                    Name = "List 4",
                                    Order = 0,
                                    Cards = [
                                        new Card(){
                                            Name="Card 1",
                                            Description="Card description",
                                            Status = "Status",
                                            DueDate =new DateOnly(2025,1,1),
                                            Order=0,
                                        },
                                        new Card(){
                                            Name="Card 2",
                                            Description="Card description",
                                            Status = "Status",
                                            DueDate = new DateOnly(2025,1,1),
                                            Order=1,
                                        },
                                        new Card(){
                                            Name="Card 3",
                                            Description="Card description",
                                            Status = "Status",
                                            DueDate = new DateOnly(2025,1,1),
                                            Order=2,
                                        },
                                    ]
                                },
                                new List(){
                                    Name = "List 5",
                                    Order = 1,
                                    Cards = [
                                        new Card(){
                                            Name="Card 4",
                                            Description="Card description",
                                            Status = "Status",
                                            DueDate = new DateOnly(2025,1,1),
                                            Order=0,
                                        },
                                        new Card(){
                                            Name="Card 5",
                                            Description="Card description",
                                            Status = "Status",
                                            DueDate =new DateOnly(2025,1,1),
                                            Order=1,
                                        },
                                        new Card(){
                                            Name="Card 6",
                                            Description="Card description",
                                            Status = "Status",
                                            DueDate = new DateOnly(2025,1,1),
                                            Order=2,
                                        },
                                    ]
                                },
                                new List(){
                                    Name = "List 6",
                                    Order = 2,
                                    Cards = [
                                        new Card(){
                                            Name="Card 7",
                                            Description="Card description",
                                            Status = "Status",
                                            DueDate = new DateOnly(2025,1,1),
                                            Order=0,
                                        },
                                        new Card(){
                                            Name="Card 8",
                                            Description="Card description",
                                            Status = "Status",
                                            DueDate = new DateOnly(2025,1,1),
                                            Order=1,
                                        },
                                        new Card(){
                                            Name="Card 9",
                                            Description="Card description",
                                            Status = "Status",
                                            DueDate = new DateOnly(2025,1,1),
                                            Order=2,
                                        },
                                    ]
                                }
                            ]
                        }
                    ]
                },
                new User{
                    Login = "user2@example.com",
                    PasswordHash = Convert.ToBase64String(System.Security.Cryptography.SHA256.HashData(Encoding.UTF8.GetBytes("password2")))
                },
                new User{
                    Login = "user3@example.com",
                    PasswordHash = Convert.ToBase64String(System.Security.Cryptography.SHA256.HashData(Encoding.UTF8.GetBytes("password3")))
                },
                new User{
                    Login = "user4@example.com",
                    PasswordHash = Convert.ToBase64String(System.Security.Cryptography.SHA256.HashData(Encoding.UTF8.GetBytes("password4")))
                },
                new User{
                    Login = "user5@example.com",
                    PasswordHash = Convert.ToBase64String(System.Security.Cryptography.SHA256.HashData(Encoding.UTF8.GetBytes("password5")))
                },
            ];
            List<Comment> comments = [
                new Comment(){
                    Text = "Comment text 1",
                    Card = users[0].Boards[0].Lists[0].Cards[0],
                    User = users[0],
                    Created = new DateTime(2024,1,1,1,1,1),
                    Edited = null
                },
                new Comment(){
                    Text = "Comment text 2",
                    Card = users[0].Boards[0].Lists[0].Cards[0],
                    User = users[1],
                    Created = new DateTime(2024,1,1,1,1,2),
                    Edited = new DateTime(2024,1,1,1,1,3)
                }
            ];
            List<Access> accesses = [
                new Access(){
                    User =users[1],
                    Board = users[0].Boards[1],
                    Permission = Permission.Read
                },
                new Access(){
                    User =users[1],
                    Board = users[0].Boards[1],
                    Permission = Permission.Edit
                }
            ];
            _taskBoardContext.AddRange(users);
            _taskBoardContext.AddRange(comments);
            _taskBoardContext.AddRange(accesses);
            await _taskBoardContext.SaveChangesAsync();

            return new ServiceResponse() { Status = ResponseStatus.Ok };
        }
    }
}
