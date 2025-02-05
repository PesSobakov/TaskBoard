using AutoMapper;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;
using Moq;
using System.Collections.Generic;
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.Intrinsics.Arm;
using System.Security.Claims;
using System.Security.Cryptography.Xml;
using System.Text;
using System.Xml.Linq;
using TaskBoard.Server.Controllers;
using TaskBoard.Server.Models.DTOs;
using TaskBoard.Server.Models.DTOs.TaskBoard;
using TaskBoard.Server.Models.Mapper;
using TaskBoard.Server.Models.TaskBoardDatabase;
using TaskBoard.Server.Services;
using static System.Net.Mime.MediaTypeNames;

namespace TaskBoard.ServerTest
{
    public class TaskBoardControllerTest
    {
        private TaskBoardController CreateTestController(ITaskBoardDatabaseService taskBoardDatabaseService, string? username)
        {
            IMapper mapper = new Mapper(new MapperConfiguration(configuration => configuration.AddProfile(new TaskBoardProfile())));
            var authServiceMock = new Mock<IAuthenticationService>();
            authServiceMock
                .Setup(_ => _.SignInAsync(It.IsAny<HttpContext>(), It.IsAny<string>(), It.IsAny<ClaimsPrincipal>(), It.IsAny<AuthenticationProperties>()))
                .Returns(Task.FromResult((object)null!));
            authServiceMock
                .Setup(_ => _.SignOutAsync(It.IsAny<HttpContext>(), It.IsAny<string>(), It.IsAny<AuthenticationProperties>()))
                .Returns(Task.FromResult((object)null!));
            var serviceProviderMock = new Mock<IServiceProvider>();
            serviceProviderMock
                .Setup(_ => _.GetService(typeof(IAuthenticationService)))
            .Returns(authServiceMock.Object);
            ClaimsPrincipal? claimsPrincipal = null;
            if (username != null)
            {
                var claims = new List<Claim> { new Claim(ClaimsIdentity.DefaultNameClaimType, username) };
                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
            }
            TaskBoardController controller = new(taskBoardDatabaseService, mapper)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = new DefaultHttpContext
                    {
                        RequestServices = serviceProviderMock.Object,
                        User = claimsPrincipal ?? new ClaimsPrincipal()
                    }
                }
            };
            return controller;
        }

        [Fact]
        public async Task GetBoards()
        {
            List<Board> boards = [
                new Board(){
                    Name = "Board 1",
                    Description = "Board description 1",
                    Privatness = Privatness.Public
                },
                new Board(){
                    Name = "Board 2",
                    Description = "Board description 2",
                    Privatness = Privatness.Private,
                }
            ];
            ServiceResponse<List<Board>> response = new ServiceResponse<List<Board>>() { Status = ResponseStatus.Ok, Data = boards };
            ITaskBoardDatabaseService taskBoardDatabaseService = Mock.Of<ITaskBoardDatabaseService>(x => x.GetBoards("user@example.com") == Task.FromResult(response));
            var controller = CreateTestController(taskBoardDatabaseService, "user@example.com");

            var actionResult = await controller.GetBoards();

            OkObjectResult okResult = Assert.IsAssignableFrom<OkObjectResult>(actionResult);
            Assert.Equal(200, okResult.StatusCode);
            List<GetBoardsDto> boardDtos = Assert.IsAssignableFrom<List<GetBoardsDto>>(okResult.Value);
            Assert.Equal(2, boardDtos.Count);
            Assert.Equal("Board 1", boardDtos[0].Name);
            Assert.Equal("Board description 1", boardDtos[0].Description);
            Assert.Equal(Privatness.Public, boardDtos[0].Privatness);
            Assert.Equal("Board 2", boardDtos[1].Name);
            Assert.Equal("Board description 2", boardDtos[1].Description);
            Assert.Equal(Privatness.Private, boardDtos[1].Privatness);
        }

        [Fact]
        public async Task GetBoard()
        {
            Board board = new()
            {
                Id = 1,
                Name = "Board 1",
                Description = "Board description 1",
                Privatness = Privatness.Public,
                Lists = [
                    new List(){
                        Id = 1,
                        Name = "List 1",
                        Order = 0,
                        Cards = [
                            new Card(){
                                Id = 1,
                                Name="Card 1",
                                Description="Card description",
                                Status = "Status",
                                DueDate = new DateTime(2025,1,1,1,1,1),
                                Order=0,
                                Comments=[
                                    new Comment(){
                                        Id = 1,
                                        Text = "Comment text 1",
                                        Created = new DateTime(2024,1,1,1,1,1),
                                        Edited = null,
                                        User = new User(){
                                            Id = 1,
                                            Login = "user@example.com",
                                            PasswordHash = Convert.ToBase64String(System.Security.Cryptography.SHA256.HashData(Encoding.UTF8.GetBytes("password"))),
                                        },
                                    },
                                    new Comment(){
                                        Id = 2,
                                        Text = "Comment text 2",
                                        Created = new DateTime(2024,1,1,1,1,2),
                                        Edited = new DateTime(2024,1,1,1,1,3),
                                        User = new User(){
                                            Id = 2,
                                            Login = "user2@example.com",
                                            PasswordHash = Convert.ToBase64String(System.Security.Cryptography.SHA256.HashData(Encoding.UTF8.GetBytes("password2"))),
                                        },

                                    }
                                ]
                            },
                            new Card(){
                                Id = 2,
                                Name="Card 2",
                                Description="Card description",
                                Status = "Status",
                                DueDate = new DateTime(2025,1,1,1,1,1),
                                Order=1,
                            },
                        ]
                    },
                    new List(){
                        Id = 2,
                        Name = "List 2",
                        Order = 1,
                    },
                ]
            };
            ServiceResponse<Board> response = new() { Status = ResponseStatus.Ok, Data = board };
            ITaskBoardDatabaseService taskBoardDatabaseService = Mock.Of<ITaskBoardDatabaseService>(x => x.GetBoard("user@example.com", 1) == Task.FromResult(response));
            var controller = CreateTestController(taskBoardDatabaseService, "user@example.com");

            var actionResult = await controller.GetBoard(1);

            OkObjectResult okResult = Assert.IsAssignableFrom<OkObjectResult>(actionResult);
            Assert.Equal(200, okResult.StatusCode);
            BoardDto boardDto = Assert.IsAssignableFrom<BoardDto>(okResult.Value);
            Assert.Equal(1, boardDto.Id);
            Assert.Equal("Board 1", boardDto.Name);
            Assert.Equal("Board description 1", boardDto.Description);
            Assert.Equal(Privatness.Public, boardDto.Privatness);
            Assert.Equal(2, boardDto.Lists.Count);
            Assert.Equal(1, boardDto.Lists[0].Id);
            Assert.Equal("List 1", boardDto.Lists[0].Name);
            Assert.Equal(0, boardDto.Lists[0].Order);
            Assert.Equal(2, boardDto.Lists[1].Id);
            Assert.Equal("List 2", boardDto.Lists[1].Name);
            Assert.Equal(1, boardDto.Lists[1].Order);
            Assert.Equal(2, boardDto.Lists[0].Cards.Count);
            Assert.Equal(1, boardDto.Lists[0].Cards[0].Id);
            Assert.Equal("Card 1", boardDto.Lists[0].Cards[0].Name);
            Assert.Equal("Card description", boardDto.Lists[0].Cards[0].Description);
            Assert.Equal("Status", boardDto.Lists[0].Cards[0].Status);
            Assert.Equal(new DateTime(2025, 1, 1, 1, 1, 1), boardDto.Lists[0].Cards[0].DueDate);
            Assert.Equal(0, boardDto.Lists[0].Cards[0].Order);
            Assert.Equal(2, boardDto.Lists[0].Cards[1].Id);
            Assert.Equal("Card 2", boardDto.Lists[0].Cards[1].Name);
            Assert.Equal("Card description", boardDto.Lists[0].Cards[1].Description);
            Assert.Equal("Status", boardDto.Lists[0].Cards[1].Status);
            Assert.Equal(new DateTime(2025, 1, 1, 1, 1, 1), boardDto.Lists[0].Cards[1].DueDate);
            Assert.Equal(1, boardDto.Lists[0].Cards[1].Order);
            Assert.Equal(2, boardDto.Lists[0].Cards[0].Comments.Count);
            Assert.Equal(1, boardDto.Lists[0].Cards[0].Comments[0].Id);
            Assert.Equal("Comment text 1", boardDto.Lists[0].Cards[0].Comments[0].Text);
            Assert.Equal(new DateTime(2024, 1, 1, 1, 1, 1), boardDto.Lists[0].Cards[0].Comments[0].Created);
            Assert.Null(boardDto.Lists[0].Cards[0].Comments[0].Edited);
            Assert.Equal(2, boardDto.Lists[0].Cards[0].Comments[1].Id);
            Assert.Equal("Comment text 2", boardDto.Lists[0].Cards[0].Comments[1].Text);
            Assert.Equal(new DateTime(2024, 1, 1, 1, 1, 2), boardDto.Lists[0].Cards[0].Comments[1].Created);
            Assert.Equal(new DateTime(2024, 1, 1, 1, 1, 3), boardDto.Lists[0].Cards[0].Comments[1].Edited);
            Assert.Equal(1, boardDto.Lists[0].Cards[0].Comments[0].User.Id);
            Assert.Equal("user@example.com", boardDto.Lists[0].Cards[0].Comments[0].User.Login);
            Assert.Equal(2, boardDto.Lists[0].Cards[0].Comments[1].User.Id);
            Assert.Equal("user2@example.com", boardDto.Lists[0].Cards[0].Comments[1].User.Login);
        }

        [Fact]
        public async Task GetCard()
        {
            Card card = new()
            {
                Id = 1,
                Name = "Card 1",
                Description = "Card description",
                Status = "Status",
                DueDate = new DateTime(2025, 1, 1, 1, 1, 1),
                Order = 0,
                Comments = [
                    new Comment(){
                        Id = 1,
                        Text = "Comment text 1",
                        Created = new DateTime(2024,1,1,1,1,1),
                        Edited = null,
                        User = new User(){
                            Id = 1,
                            Login = "user@example.com",
                            PasswordHash = Convert.ToBase64String(System.Security.Cryptography.SHA256.HashData(Encoding.UTF8.GetBytes("password"))),
                        },
                    },
                    new Comment(){
                        Id = 2,
                        Text = "Comment text 2",
                        Created = new DateTime(2024,1,1,1,1,2),
                        Edited = new DateTime(2024,1,1,1,1,3),
                        User = new User(){
                            Id = 2,
                            Login = "user2@example.com",
                            PasswordHash = Convert.ToBase64String(System.Security.Cryptography.SHA256.HashData(Encoding.UTF8.GetBytes("password2"))),
                        },

                    }
                ]
            };
            ServiceResponse<Card> response = new() { Status = ResponseStatus.Ok, Data = card };
            ITaskBoardDatabaseService taskBoardDatabaseService = Mock.Of<ITaskBoardDatabaseService>(x => x.GetCard("user@example.com", 1) == Task.FromResult(response));
            var controller = CreateTestController(taskBoardDatabaseService, "user@example.com");

            var actionResult = await controller.GetCard(1);

            OkObjectResult okResult = Assert.IsAssignableFrom<OkObjectResult>(actionResult);
            Assert.Equal(200, okResult.StatusCode);
            CardDto cardDto = Assert.IsAssignableFrom<CardDto>(okResult.Value);
            Assert.Equal(1, cardDto.Id);
            Assert.Equal("Card 1", cardDto.Name);
            Assert.Equal("Card description", cardDto.Description);
            Assert.Equal("Status", cardDto.Status);
            Assert.Equal(new DateTime(2025, 1, 1, 1, 1, 1), cardDto.DueDate);
            Assert.Equal(0, cardDto.Order);
            Assert.Equal(1, cardDto.Comments[0].Id);
            Assert.Equal("Comment text 1", cardDto.Comments[0].Text);
            Assert.Equal(new DateTime(2024, 1, 1, 1, 1, 1), cardDto.Comments[0].Created);
            Assert.Null(cardDto.Comments[0].Edited);
            Assert.Equal(2, cardDto.Comments[1].Id);
            Assert.Equal("Comment text 2", cardDto.Comments[1].Text);
            Assert.Equal(new DateTime(2024, 1, 1, 1, 1, 2), cardDto.Comments[1].Created);
            Assert.Equal(new DateTime(2024, 1, 1, 1, 1, 3), cardDto.Comments[1].Edited);
            Assert.Equal(1, cardDto.Comments[0].User.Id);
            Assert.Equal("user@example.com", cardDto.Comments[0].User.Login);
            Assert.Equal(2, cardDto.Comments[1].User.Id);
            Assert.Equal("user2@example.com", cardDto.Comments[1].User.Login);
        }
    }
}
