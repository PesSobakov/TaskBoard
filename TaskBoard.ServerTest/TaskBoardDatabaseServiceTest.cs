using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;
using System.Collections.Generic;
using System.Reflection.Metadata;
using System.Text;
using TaskBoard.Server.Models.DTOs;
using TaskBoard.Server.Models.TaskBoardDatabase;
using TaskBoard.Server.Services;
using static System.Net.Mime.MediaTypeNames;

namespace TaskBoard.ServerTest
{
    public class TaskBoardDatabaseServiceTest : IDisposable
    {
        SqliteConnection _connection;
        DbContextOptions<TaskBoardContext> _contextOptions;
        ITaskBoardDatabaseService _taskBoardDatabaseService;
        TestTimeProvider _testTimeProvider;
        public TaskBoardDatabaseServiceTest()
        {
            _connection = new SqliteConnection("Filename=:memory:");
            _connection.Open();
            _contextOptions = new DbContextOptionsBuilder<TaskBoardContext>()
                .UseSqlite(_connection)
                .Options;

            using var context = new TaskBoardContext(_contextOptions);
            context.Database.EnsureCreated();

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
                                            DueDate = new DateTime(2025,1,1,1,1,1),
                                            Order=0,
                                        },
                                        new Card(){
                                            Name="Card 2",
                                            Description="Card description",
                                            Status = "Status",
                                            DueDate = new DateTime(2025,1,1,1,1,1),
                                            Order=1,
                                        },
                                        new Card(){
                                            Name="Card 3",
                                            Description="Card description",
                                            Status = "Status",
                                            DueDate = new DateTime(2025,1,1,1,1,1),
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
                                            DueDate = new DateTime(2025,1,1,1,1,1),
                                            Order=0,
                                        },
                                        new Card(){
                                            Name="Card 5",
                                            Description="Card description",
                                            Status = "Status",
                                            DueDate = new DateTime(2025,1,1,1,1,1),
                                            Order=1,
                                        },
                                        new Card(){
                                            Name="Card 6",
                                            Description="Card description",
                                            Status = "Status",
                                            DueDate = new DateTime(2025,1,1,1,1,1),
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
                                            DueDate = new DateTime(2025,1,1,1,1,1),
                                            Order=0,
                                        },
                                        new Card(){
                                            Name="Card 8",
                                            Description="Card description",
                                            Status = "Status",
                                            DueDate = new DateTime(2025,1,1,1,1,1),
                                            Order=1,
                                        },
                                        new Card(){
                                            Name="Card 9",
                                            Description="Card description",
                                            Status = "Status",
                                            DueDate = new DateTime(2025,1,1,1,1,1),
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
                                            DueDate = new DateTime(2025,1,1,1,1,1),
                                            Order=0,
                                        },
                                        new Card(){
                                            Name="Card 2",
                                            Description="Card description",
                                            Status = "Status",
                                            DueDate = new DateTime(2025,1,1,1,1,1),
                                            Order=1,
                                        },
                                        new Card(){
                                            Name="Card 3",
                                            Description="Card description",
                                            Status = "Status",
                                            DueDate = new DateTime(2025,1,1,1,1,1),
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
                                            DueDate = new DateTime(2025,1,1,1,1,1),
                                            Order=0,
                                        },
                                        new Card(){
                                            Name="Card 5",
                                            Description="Card description",
                                            Status = "Status",
                                            DueDate = new DateTime(2025,1,1,1,1,1),
                                            Order=1,
                                        },
                                        new Card(){
                                            Name="Card 6",
                                            Description="Card description",
                                            Status = "Status",
                                            DueDate = new DateTime(2025,1,1,1,1,1),
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
                                            DueDate = new DateTime(2025,1,1,1,1,1),
                                            Order=0,
                                        },
                                        new Card(){
                                            Name="Card 8",
                                            Description="Card description",
                                            Status = "Status",
                                            DueDate = new DateTime(2025,1,1,1,1,1),
                                            Order=1,
                                        },
                                        new Card(){
                                            Name="Card 9",
                                            Description="Card description",
                                            Status = "Status",
                                            DueDate = new DateTime(2025,1,1,1,1,1),
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
            context.AddRange(users);
            context.AddRange(comments);
            context.AddRange(accesses);
            context.SaveChanges();

            _testTimeProvider = new TestTimeProvider();
            _testTimeProvider.SetTime(new DateTime(2026, 1, 1, 1, 1, 1));

            _taskBoardDatabaseService = new TaskBoardDatabaseService(CreateContext(), _testTimeProvider);
        }
        TaskBoardContext CreateContext() => new TaskBoardContext(_contextOptions);
        public void Dispose() => _connection.Dispose();

        [Fact]
        public async Task IsUserExist()
        {
            var response = await _taskBoardDatabaseService.IsUserExist("user@example.com");

            Assert.Equal(ResponseStatus.Ok, response.Status);
            Assert.True(response.Data);
        }
        [Fact]
        public async Task Register()
        {
            var context = CreateContext();
            RegisterDto registerDto = new()
            {
                Login = "Test Login",
                Password = "Test Password"
            };

            var response = await _taskBoardDatabaseService.Register(registerDto);

            Assert.Equal(ResponseStatus.Ok, response.Status);
            Assert.True(
                context.Users
                .Where(user => user.Login == "Test Login")
                .Where(user => user.PasswordHash == Convert.ToBase64String(System.Security.Cryptography.SHA256.HashData(Encoding.UTF8.GetBytes("Test Password"))))
                .Any()
            );
        }
        [Fact]
        public async Task Login()
        {
            var context = CreateContext();
            LoginDto loginDto = new()
            {
                Login = "user@example.com",
                Password = "password"
            };

            var response = await _taskBoardDatabaseService.Login(loginDto);

            Assert.Equal(ResponseStatus.Ok, response.Status);
            Assert.NotNull(response.Data);
            Assert.Equal("user@example.com",response.Data.Login);
            Assert.Equal(Convert.ToBase64String(System.Security.Cryptography.SHA256.HashData(Encoding.UTF8.GetBytes("password"))), response.Data.PasswordHash);
        }
        [Fact]
        public async Task GetUser()
        {
            var response = await _taskBoardDatabaseService.GetUser("user@example.com");

            Assert.Equal(ResponseStatus.Ok, response.Status);
            Assert.NotNull(response.Data);
            Assert.Equal("user@example.com", response.Data.Login);
            Assert.Equal(Convert.ToBase64String(System.Security.Cryptography.SHA256.HashData(Encoding.UTF8.GetBytes("password"))), response.Data.PasswordHash);
        }
        [Fact]
        public async Task DeleteAccount()
        {
            var context = CreateContext();
            var response = await _taskBoardDatabaseService.DeleteAccount("user@example.com");

            Assert.Equal(ResponseStatus.Ok, response.Status);
            Assert.False(
                context.Users
                .Where(user => user.Login == "user@example.com")
                .Any()
            );
        }

        [Fact]
        public async Task CreateBoard()
        {
            var context = CreateContext();
            CreateBoardDto createBoardDto = new()
            {
                Name = "Test Board",
                Description = "Test Description"
            };

            var response = await _taskBoardDatabaseService.CreateBoard("user@example.com", createBoardDto);

            Assert.Equal(ResponseStatus.Ok, response.Status);
            Assert.True(
                context.Boards
                .Where(board => board.Name == "Test Board")
                .Where(board => board.Description == "Test Description")
                .Where(board => board.User.Login == "user@example.com")
                .Any()
            );
        }

        [Fact]
        public async Task GetBoards()
        {
            var response = await _taskBoardDatabaseService.GetBoards("user@example.com");

            Assert.Equal(ResponseStatus.Ok, response.Status);
            var context = CreateContext();
            Assert.NotNull(response.Data);
            Assert.Equal(2, response.Data.Count);
            Assert.True(
                response.Data
                .Where(board => board.Name == "Board 1")
                .Where(board => board.Description == "Board description 1")
                .Where(board => board.Privatness == Privatness.Public)
                .Any()
            );
            Assert.True(
                response.Data
                .Where(board => board.Name == "Board 2")
                .Where(board => board.Description == "Board description 2")
                .Where(board => board.Privatness == Privatness.Private)
                .Any()
            );
        }

        [Fact]
        public async Task GetBoard()
        {
            var context = CreateContext();
            var board = context.Boards.Where(board => board.Name == "Board 1").First();

            var response = await _taskBoardDatabaseService.GetBoard("user@example.com", board.Id);

            Assert.Equal(ResponseStatus.Ok, response.Status);
            Assert.NotNull(response.Data);
            Assert.Equal("Board 1", response.Data.Name);
            Assert.Equal("Board description 1", response.Data.Description);
            Assert.Equal(Privatness.Public, response.Data.Privatness);
            Assert.Equal(3, response.Data.Lists.Count);
            Assert.True(
                response.Data.Lists
                .Where(list => list.Name == "List 1")
                .Where(list => list.Order == 0)
                .Any()
            );
            Assert.True(
                response.Data.Lists.Where(list => list.Order == 0).First().Cards
                .Where(card => card.Name == "Card 1")
                .Where(card => card.Description == "Card description")
                .Where(card => card.Status == "Status")
                .Where(card => card.DueDate == new DateTime(2025, 1, 1, 1, 1, 1))
                .Where(card => card.Order == 0)
                .Any()
            );
            Assert.True(
                response.Data.Lists.Where(list => list.Order == 0).First().Cards
                .Where(card => card.Name == "Card 2")
                .Where(card => card.Description == "Card description")
                .Where(card => card.Status == "Status")
                .Where(card => card.DueDate == new DateTime(2025, 1, 1, 1, 1, 1))
                .Where(card => card.Order == 1)
                .Any()
            );
            Assert.True(
                response.Data.Lists.Where(list => list.Order == 0).First().Cards
                .Where(card => card.Name == "Card 3")
                .Where(card => card.Description == "Card description")
                .Where(card => card.Status == "Status")
                .Where(card => card.DueDate == new DateTime(2025, 1, 1, 1, 1, 1))
                .Where(card => card.Order == 2)
                .Any()
            );
            Assert.True(
                response.Data.Lists.Where(list => list.Order == 0).First().Cards.Where(card => card.Order == 0).First().Comments
                .Where(comment => comment.Text == "Comment text 1")
                .Where(comment => comment.User.Login == "user@example.com")
                .Where(comment => comment.Created == new DateTime(2024, 1, 1, 1, 1, 1))
                .Where(comment => comment.Edited == null)
                .Any()
            );
            Assert.True(
                response.Data.Lists.Where(list => list.Order == 0).First().Cards.Where(card => card.Order == 0).First().Comments
                .Where(comment => comment.Text == "Comment text 2")
                .Where(comment => comment.User.Login == "user2@example.com")
                .Where(comment => comment.Created == new DateTime(2024, 1, 1, 1, 1, 2))
                .Where(comment => comment.Edited == new DateTime(2024, 1, 1, 1, 1, 3))
                .Any()
            );
            Assert.True(
                response.Data.Lists
                .Where(list => list.Name == "List 2")
                .Where(list => list.Order == 1)
                .Any()
            );
            Assert.True(
                response.Data.Lists.Where(list => list.Order == 1).First().Cards
                .Where(card => card.Name == "Card 4")
                .Where(card => card.Description == "Card description")
                .Where(card => card.Status == "Status")
                .Where(card => card.DueDate == new DateTime(2025, 1, 1, 1, 1, 1))
                .Where(card => card.Order == 0)
                .Any()
            );
            Assert.True(
                response.Data.Lists.Where(list => list.Order == 1).First().Cards
                .Where(card => card.Name == "Card 5")
                .Where(card => card.Description == "Card description")
                .Where(card => card.Status == "Status")
                .Where(card => card.DueDate == new DateTime(2025, 1, 1, 1, 1, 1))
                .Where(card => card.Order == 1)
                .Any()
            );
            Assert.True(
                response.Data.Lists.Where(list => list.Order == 1).First().Cards
                .Where(card => card.Name == "Card 6")
                .Where(card => card.Description == "Card description")
                .Where(card => card.Status == "Status")
                .Where(card => card.DueDate == new DateTime(2025, 1, 1, 1, 1, 1))
                .Where(card => card.Order == 2)
                .Any()
            );
            Assert.True(
                response.Data.Lists
                .Where(list => list.Name == "List 3")
                .Where(list => list.Order == 2)
                .Any()
            );
            Assert.True(
                response.Data.Lists.Where(list => list.Order == 2).First().Cards
                .Where(card => card.Name == "Card 7")
                .Where(card => card.Description == "Card description")
                .Where(card => card.Status == "Status")
                .Where(card => card.DueDate == new DateTime(2025, 1, 1, 1, 1, 1))
                .Where(card => card.Order == 0)
                .Any()
            );
            Assert.True(
                response.Data.Lists.Where(list => list.Order == 2).First().Cards
                .Where(card => card.Name == "Card 8")
                .Where(card => card.Description == "Card description")
                .Where(card => card.Status == "Status")
                .Where(card => card.DueDate == new DateTime(2025, 1, 1, 1, 1, 1))
                .Where(card => card.Order == 1)
                .Any()
            );
            Assert.True(
                response.Data.Lists.Where(list => list.Order == 2).First().Cards
                .Where(card => card.Name == "Card 9")
                .Where(card => card.Description == "Card description")
                .Where(card => card.Status == "Status")
                .Where(card => card.DueDate == new DateTime(2025, 1, 1, 1, 1, 1))
                .Where(card => card.Order == 2)
                .Any()
            );
        }

        [Fact]
        public async Task EditBoard()
        {
            var context = CreateContext();
            int boardId = context.Boards.Where(board => board.Name == "Board 1").Select(board => board.Id).First();
            EditBoardDto editBoardDto = new()
            {
                Name = "Test Board",
                Description = "Test Description"
            };

            var response = await _taskBoardDatabaseService.EditBoard("user@example.com", boardId, editBoardDto);

            Assert.Equal(ResponseStatus.Ok, response.Status);
            Assert.True(
                context.Boards
                .Where(board => board.Id == boardId)
                .Where(board => board.Name == "Test Board")
                .Where(board => board.Description == "Test Description")
                .Where(board => board.User.Login == "user@example.com")
                .Any()
            );
        }

        [Fact]
        public async Task DeleteBoard()
        {
            var context = CreateContext();
            int boardId = context.Boards.Where(board => board.Name == "Board 1").Select(board => board.Id).First();
            List<int> listIds = context.Lists.Where(list => list.Board.Id == boardId).Select(list => list.Id).ToList();
            List<int> cardIds = context.Cards.Where(card => card.List.Board.Id == boardId).Select(card => card.Id).ToList();
            List<int> commentIds = context.Comments.Where(comment => comment.Card.List.Board.Id == boardId).Select(comment => comment.Id).ToList();

            var response = await _taskBoardDatabaseService.DeleteBoard("user@example.com", boardId);

            Assert.Equal(ResponseStatus.Ok, response.Status);
            Assert.False(
                context.Boards.Where(board => board.Id == boardId).Any()
            );
            foreach (var listId in listIds)
            {
                Assert.False(
                    context.Lists
                    .Where(list => list.Id == listId)
                    .Any()
                );
            }
            foreach (var cardId in cardIds)
            {
                Assert.False(
                    context.Cards
                    .Where(card => card.Id == cardId)
                    .Any()
                );
            }
            foreach (var commentId in commentIds)
            {
                Assert.False(
                    context.Comments
                    .Where(comment => comment.Id == commentId)
                    .Any()
                );
            }
        }

        [Fact]
        public async Task CreateList()
        {
            var context = CreateContext();
            int boardId = context.Boards.Where(board => board.Name == "Board 1").Select(board => board.Id).First();
            CreateListDto createListDto = new()
            {
                Name = "Test List",
                BoardId = boardId
            };

            var response = await _taskBoardDatabaseService.CreateList("user@example.com", createListDto);

            Assert.Equal(ResponseStatus.Ok, response.Status);
            Assert.True(
                context.Lists
                .Where(list => list.Name == "Test List")
                .Where(list => list.Board.Id == boardId)
                .Where(list => list.Order == 3)
                .Any()
            );
        }

        [Fact]
        public async Task EditList()
        {
            var context = CreateContext();
            int listId = context.Lists.Where(list => list.Name == "List 1").Select(list => list.Id).First();
            EditListDto editListDto = new()
            {
                Name = "Test List"
            };

            var response = await _taskBoardDatabaseService.EditList("user@example.com", listId, editListDto);

            Assert.Equal(ResponseStatus.Ok, response.Status);
            Assert.True(
                context.Lists
                .Where(list => list.Id == listId)
                .Where(list => list.Name == "Test List")
                .Any()
            );
        }

        [Fact]
        public async Task ChangeListOrder()
        {
            var context = CreateContext();
            int list0Id = context.Lists.Where(list => list.Order == 0).Select(list => list.Id).First();
            int list1Id = context.Lists.Where(list => list.Order == 1).Select(list => list.Id).First();
            ChangeListOrderDto changeListOrderDto = new()
            {
                Order = 1
            };

            var response = await _taskBoardDatabaseService.ChangeListOrder("user@example.com", list0Id, changeListOrderDto);

            Assert.Equal(ResponseStatus.Ok, response.Status);
            Assert.True(
                context.Lists
                .Where(list => list.Id == list0Id)
                .Where(list => list.Order == 1)
                .Any()
            );
            Assert.True(
                context.Lists
                .Where(list => list.Id == list1Id)
                .Where(list => list.Order == 0)
                .Any()
            );
        }

        [Fact]
        public async Task DeleteList()
        {
            var context = CreateContext();
            int listId = context.Lists.Where(list => list.Name == "List 1").Select(list => list.Id).First();
            List<int> cardIds = context.Cards.Where(card => card.List.Id == listId).Select(card => card.Id).ToList();
            List<int> commentIds = context.Comments.Where(comment => comment.Card.List.Id == listId).Select(comment => comment.Id).ToList();

            var response = await _taskBoardDatabaseService.DeleteList("user@example.com", listId);

            Assert.Equal(ResponseStatus.Ok, response.Status);
            Assert.False(
                context.Lists
                .Where(list => list.Id == listId)
                .Any()
            );
            foreach (var cardId in cardIds)
            {
                Assert.False(
                    context.Cards
                    .Where(card => card.Id == cardId)
                    .Any()
                );
            }
            foreach (var commentId in commentIds)
            {
                Assert.False(
                    context.Comments
                    .Where(comment => comment.Id == commentId)
                    .Any()
                );
            }
        }

        [Fact]
        public async Task CreateCard()
        {
            var context = CreateContext();
            int listId = context.Lists.Where(list => list.Name == "List 1").Select(list => list.Id).First();
            CreateCardDto createCardDto = new()
            {
                ListId = listId,
                Name = "Test Card",
                Description = "Test Description",
                Status = "Test Status",
                DueDate = new DateTime(2026, 1, 1, 1, 1, 1),
            };

            var response = await _taskBoardDatabaseService.CreateCard("user@example.com", createCardDto);

            Assert.Equal(ResponseStatus.Ok, response.Status);
            Assert.True(
                context.Cards
                .Where(card => card.List.Id == listId)
                .Where(card => card.Name == "Test Card")
                .Where(card => card.Description == "Test Description")
                .Where(card => card.Status == "Test Status")
                .Where(card => card.DueDate == new DateTime(2026, 1, 1, 1, 1, 1))
                .Where(card => card.Order == 3)
                .Any()
            );
        }

        [Fact]
        public async Task GetCard()
        {
            var context = CreateContext();
            var cardId = context.Cards.Where(card => card.Name == "Card 1").Select(card => card.Id).First();

            var response = await _taskBoardDatabaseService.GetCard("user@example.com", cardId);

            Assert.Equal(ResponseStatus.Ok, response.Status);
            Assert.NotNull(response.Data);
            Assert.Equal("Card 1", response.Data.Name);
            Assert.Equal("Card description", response.Data.Description);
            Assert.Equal("Status", response.Data.Status);
            Assert.Equal(new DateTime(2025, 1, 1, 1, 1, 1), response.Data.DueDate);
            Assert.Equal(0, response.Data.Order);
            Assert.True(
                response.Data.Comments
                .Where(comment => comment.Text == "Comment text 1")
                .Where(comment => comment.User.Login == "user@example.com")
                .Where(comment => comment.Created == new DateTime(2024, 1, 1, 1, 1, 1))
                .Where(comment => comment.Edited == null)
                .Any()
            );
            Assert.True(
                response.Data.Comments
                .Where(comment => comment.Text == "Comment text 2")
                .Where(comment => comment.User.Login == "user2@example.com")
                .Where(comment => comment.Created == new DateTime(2024, 1, 1, 1, 1, 2))
                .Where(comment => comment.Edited == new DateTime(2024, 1, 1, 1, 1, 3))
                .Any()
            );
        }

        [Fact]
        public async Task EditCard()
        {
            var context = CreateContext();
            int cardId = context.Cards.Where(card => card.Name == "Card 1").Select(card => card.Id).First();
            EditCardDto editCardDto = new()
            {
                Name = "Test Card",
                Description = "Test Description",
                Status = "Test Status",
                DueDate = new DateTime(2026, 1, 1, 1, 1, 1),
            };

            var response = await _taskBoardDatabaseService.EditCard("user@example.com", cardId, editCardDto);

            Assert.Equal(ResponseStatus.Ok, response.Status);
            Assert.True(
                context.Cards
                .Where(card => card.Name == "Test Card")
                .Where(card => card.Description == "Test Description")
                .Where(card => card.Status == "Test Status")
                .Where(card => card.DueDate == new DateTime(2026, 1, 1, 1, 1, 1))
                .Any()
            );
        }

        [Fact]
        public async Task MoveCard()
        {
            var context = CreateContext();
            int cardId = context.Lists.Where(list => list.Order == 0).Include(list=>list.Cards).First().Cards.Where(card => card.Order == 0).Select(card => card.Id).First();
            int List1Id = context.Lists.Where(list => list.Order == 1).Select(list => list.Id).First();
            MoveCardDto moveCardDto = new()
            {
                ListId = List1Id
            };

            var response = await _taskBoardDatabaseService.MoveCard("user@example.com", cardId, moveCardDto);

            Assert.Equal(ResponseStatus.Ok, response.Status);
            Assert.True(
                context.Cards
                .Where(card => card.Id == cardId)
                .Where(card => card.List.Id == List1Id)
                .Where(card => card.Order == 3)
                .Any()
            );
        }

        [Fact]
        public async Task ChangeCardOrder()
        {
            var context = CreateContext();
            int card0Id = context.Lists.Where(list => list.Name == "List 1").Include(list=>list.Cards).First().Cards.Where(card => card.Order == 0).Select(card => card.Id).First();
            int card1Id = context.Lists.Where(list => list.Name == "List 1").Include(list => list.Cards).First().Cards.Where(card => card.Order == 1).Select(card => card.Id).First();
            int ListId = context.Lists.Where(list => list.Name == "List 1").Select(list => list.Id).First();
            ChangeCardOrderDto changeCardOrderDto = new()
            {
                Order = 1
            };

            var response = await _taskBoardDatabaseService.ChangeCardOrder("user@example.com", card0Id, changeCardOrderDto);

            Assert.Equal(ResponseStatus.Ok, response.Status);
            Assert.True(
                context.Cards
                .Where(card => card.Id == card0Id)
                .Where(card => card.Order == 1)
                .Any()
            );
            Assert.True(
                context.Cards
                .Where(card => card.Id == card1Id)
                .Where(card => card.Order == 0)
                .Any()
            );
        }

        [Fact]
        public async Task DeleteCard()
        {
            var context = CreateContext();
            int cardId = context.Cards.Where(card => card.Name == "Card 1").Select(card => card.Id).First();
            List<int> commentIds = context.Comments.Where(comment => comment.Card.Id == cardId).Select(comment => comment.Id).ToList();
            //card
            var response = await _taskBoardDatabaseService.DeleteCard("user@example.com", cardId);

            Assert.Equal(ResponseStatus.Ok, response.Status);
            Assert.False(
                context.Cards
                .Where(card => card.Id == cardId)
                .Any()
            );
            foreach (var commentId in commentIds)
            {
                Assert.False(
                    context.Comments
                    .Where(comment => comment.Id == commentId)
                    .Any()
                );
            }
        }
        [Fact]
        public async Task CreateComment()
        {
            var context = CreateContext();
            int cardId = context.Cards.Where(card => card.Name == "Card 1").Select(card => card.Id).First();
            _testTimeProvider.SetTime(new DateTime(2025,1,1,1,1,1));
            CreateCommentDto createCommentDto = new()
            {
                CardId = cardId,
                Text = "Test Text"
            };

            var response = await _taskBoardDatabaseService.CreateComment("user@example.com", createCommentDto);

            Assert.Equal(ResponseStatus.Ok, response.Status);
            Assert.True(
                context.Comments
                .Where(comment => comment.Card.Id == cardId)
                .Where(comment => comment.Text == "Test Text")
                .Where(comment => comment.Created == new DateTime(2025, 1, 1, 1, 1, 1))
                .Where(comment => comment.Edited == null)
                .Where(comment => comment.User.Login == "user@example.com")
                .Any()
            );
        }

        [Fact]
        public async Task EditComment()
        {
            var context = CreateContext();
            int commentId = context.Comments.Where(comment => comment.Text == "Comment text 1").Select(comment => comment.Id).First();
            _testTimeProvider.SetTime(new DateTime(2025, 1, 1, 1, 1, 1));
            EditCommentDto editCommentDto = new()
            {
                Text = "Test Text"
            };

            var response = await _taskBoardDatabaseService.EditComment("user@example.com", commentId, editCommentDto);

            Assert.Equal(ResponseStatus.Ok, response.Status);
            Assert.True(
                context.Comments
                .Where(comment => comment.Id == commentId)
                .Where(comment => comment.Text == "Test Text")
                .Where(comment => comment.Edited == new DateTime(2025, 1, 1, 1, 1, 1))
                .Where(comment => comment.User.Login == "user@example.com")
                .Any()
            );
        }

        [Fact]
        public async Task DeleteComment()
        {
            var context = CreateContext();
            int commentId = context.Comments.Where(comment => comment.Text == "Comment text 1").Select(comment => comment.Id).First();

            var response = await _taskBoardDatabaseService.DeleteComment("user@example.com", commentId);

            Assert.Equal(ResponseStatus.Ok, response.Status);
            Assert.False(
                context.Comments
                .Where(comment => comment.Id == commentId)
                .Any()
            );
        }

        [Fact]
        public async Task SetBoardPrivate()
        {
            var context = CreateContext();
            int BoardId = context.Boards.Where(board => board.Name == "Board 1").Select(board => board.Id).First();

            var response = await _taskBoardDatabaseService.SetBoardPrivate("user@example.com", BoardId);

            Assert.Equal(ResponseStatus.Ok, response.Status);
            Assert.True(
                context.Boards
                .Where(board => board.Id == BoardId)
                .Where(board => board.Privatness == Privatness.Private)
                .Any()
            );
        }

        [Fact]
        public async Task SetBoardPublic()
        {
            var context = CreateContext();
            int BoardId = context.Boards.Where(board => board.Name == "Board 1").Select(board => board.Id).First();

            var response = await _taskBoardDatabaseService.SetBoardPublic("user@example.com", BoardId);

            Assert.Equal(ResponseStatus.Ok, response.Status);
            Assert.True(
                context.Boards
                .Where(board => board.Id == BoardId)
                .Where(board => board.Privatness == Privatness.Public)
                .Any()
            );
        }

        [Fact]
        public async Task GiveAccess()
        {
            var context = CreateContext();
            int BoardId = context.Boards.Where(board => board.Name == "Board 1").Select(board => board.Id).First();
            GiveAccessDto giveAccessDto = new()
            {
                User = "user5@example.com",
                Permission = Permission.Read
            };

            var response = await _taskBoardDatabaseService.GiveAccess("user@example.com", BoardId, giveAccessDto);

            Assert.Equal(ResponseStatus.Ok, response.Status);
            Assert.True(
                context.Accesses
                .Where(access => access.Board.Id == BoardId)
                .Where(access => access.User.Login == "user5@example.com")
                .Where(access => access.Permission == Permission.Read)
                .Any()
            );
        }
        [Fact]
        public async Task RemoveAccess()
        {
            var context = CreateContext();
            int boardId = context.Boards.Where(board => board.Name == "Board 2").Select(board => board.Id).First();
            int accessId = context.Accesses
                 .Where(access=> access.Board.Id == boardId)
                 .Where(access => access.User.Login == "user2@example.com")
                 .Select(access => access.Id)
                 .First();
            RemoveAccessDto removeAccessDto = new()
            {
                User = "user2@example.com"
            };
            var response = await _taskBoardDatabaseService.RemoveAccess("user@example.com", boardId, removeAccessDto);

            Assert.Equal(ResponseStatus.Ok, response.Status);
            Assert.False(
                context.Accesses
                .Where(access => access.Id == accessId)
                .Any()
            );
        }
    }
}
