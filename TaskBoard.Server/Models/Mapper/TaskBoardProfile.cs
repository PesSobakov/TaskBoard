using AutoMapper;
using TaskBoard.Server.Models.DTOs;
using TaskBoard.Server.Models.DTOs.TaskBoard;
using TaskBoard.Server.Models.TaskBoardDatabase;

namespace TaskBoard.Server.Models.Mapper
{
    public class TaskBoardProfile:Profile
    {
        public TaskBoardProfile()
        {
            CreateMap<Board, GetBoardsDto> ();
            CreateMap<Board, BoardDto> ();
            CreateMap<List, ListDto> ();
            CreateMap<Card, CardDto>();
            CreateMap<Comment, CommentDto> ();
            CreateMap<User, UserDto> ();
        }
    }
}
