import { RegisterDto } from "../DTOs/RegisterDto";
import { LoginDto } from "../DTOs/LoginDto";
import { CreateBoardDto } from "../DTOs/CreateBoardDto";
import { EditBoardDto } from "../DTOs/EditBoardDto";
import { BoardDto } from "../DTOs/TaskBoard/BoardDto";
import { CreateListDto } from "../DTOs/CreateListDto";
import { EditListDto } from "../DTOs/EditListDto";
import { ChangeListOrderDto } from "../DTOs/ChangeListOrderDto";
import { CreateCardDto } from "../DTOs/CreateCardDto";
import { CardDto } from "../DTOs/TaskBoard/CardDto";
import { ChangeCardOrderDto } from "../DTOs/ChangeCardOrderDto";
import { MoveCardDto } from "../DTOs/MoveCardDto";
import { EditCardDto } from "../DTOs/EditCardDto";
import { GiveAccessDto } from "../DTOs/GiveAccessDto";
import { RemoveAccessDto } from "../DTOs/RemoveAccessDto";
import { CreateCommentDto } from "../DTOs/CreateCommentDto";
import { EditCommentDto } from "../DTOs/EditCommentDto";
import { StringDto } from "../DTOs/StringDto";

export default class ApiService
{
    private static apiPath: string | undefined = import.meta.env.VITE_SERVER
    private static makeRequest = <T1, T2>(path: string, method: string, body?: T2 | undefined): Promise<[T1 | null, string | null]> =>
    {
        return new Promise((resolve,) =>
        {
            const xhr = new XMLHttpRequest();
            xhr.open(method, this.apiPath + path);
            xhr.withCredentials = true;
            xhr.setRequestHeader("Content-Type", "application/json");
            xhr.onload = function ()
            {
                if (xhr.status >= 200 && xhr.status < 300) {
                    const responseObject = xhr.response == '' ? null : JSON.parse(xhr.response);
                    resolve([responseObject, null]);
                }
                else {
                    switch (xhr.status) {
                        case 400:
                            resolve([null, "BadRequest"]);
                            break;
                        case 401:
                            resolve([null, "Unauthorized"]);
                            break;
                        case 403:
                            resolve([null, "Forbidden"]);
                            break;
                        default:
                            resolve([null, "Unknown"]);
                            break;
                    }
                }
            };
            xhr.onerror = function ()
            {
                resolve([null, "Unknown"]);
                return true;
            };
            xhr.send(JSON.stringify(body));
        });
    }
    public static GetUser() 
    {
        return this.makeRequest<StringDto, null>("/api/auth/getuser", "GET");
    };
    public static Register (dto: RegisterDto) 
    {
        return this.makeRequest<null, RegisterDto>("/api/auth/register", "POST", dto);
    };
    public static Login  (dto: LoginDto)
    {
        return this.makeRequest<null, LoginDto>("/api/auth/login", "POST", dto);
    };
    public static Logout  () 
    {
        return this.makeRequest<null, null>("/api/auth/logout", "GET");
    };
    public static DeleteAccount  () 
    {
        return this.makeRequest<null, null>("/api/auth/deleteaccount", "GET");
    };

    public static CreateBoard  (dto: CreateBoardDto) 
    {
        return this.makeRequest<null, CreateBoardDto>("/api/taskboard/createboard", "POST", dto);
    };
    public static GetBoards  () 
    {
        return this.makeRequest<BoardDto[], null>("/api/taskboard/getboards", "GET");
    };
    public static GetBoard  (id: number) 
    {
        return this.makeRequest<BoardDto, null>(`/api/taskboard/getboard/${id}`, "GET");
    };
    public static EditBoard  (id: number, dto: EditBoardDto) 
    {
        return this.makeRequest<null, EditBoardDto>(`/api/taskboard/editboard/${id}`, "PATCH", dto);
    };
    public static DeleteBoard  (id: number)
    {
        return this.makeRequest<null, null>(`/api/taskboard/deleteboard/${id}`, "DELETE");
    };

    public static CreateList  (dto: CreateListDto) 
    {
        return this.makeRequest<null, CreateListDto>(`/api/taskboard/createlist`, "POST", dto);
    };
    public static EditList  (id: number, dto: EditListDto) 
    {
        return this.makeRequest<null, EditListDto>(`/api/taskboard/editlist/${id}`, "PATCH", dto);
    };
    public static ChangeListOrder  (id: number, dto: ChangeListOrderDto) 
    {
        return this.makeRequest<null, ChangeListOrderDto>(`/api/taskboard/changelistorder/${id}`, "PATCH", dto);
    };
    public static DeleteList  (id: number) 
    {
        return this.makeRequest<null, null>(`/api/taskboard/deletelist/${id}`, "DELETE");
    };

    public static CreateCard  (dto: CreateCardDto) 
    {
        return this.makeRequest<null, CreateCardDto>(`/api/taskboard/createcard`, "POST", dto);
    };
    public static GetCard  (id: number) 
    {
        return this.makeRequest<CardDto, null>(`/api/taskboard/getcard/${id}`, "GET");
    };
    public static EditCard  (id: number, dto: EditCardDto) 
    {
        return this.makeRequest<null, EditCardDto>(`/api/taskboard/editcard/${id}`, "PATCH", dto);
    };
    public static MoveCard  (id: number, dto: MoveCardDto)
    {
        return this.makeRequest<null, MoveCardDto>(`/api/taskboard/movecard/${id}`, "PATCH", dto);
    };
    public static ChangeCardOrder  (id: number, dto: ChangeCardOrderDto) 
    {
        return this.makeRequest<null, ChangeCardOrderDto>(`/api/taskboard/changecardorder/${id}`, "PATCH", dto);
    };
    public static DeleteCard  (id: number)
    {
        return this.makeRequest<null, null>(`/api/taskboard/deletecard/${id}`, "DELETE");
    };

    public static CreateComment  (dto: CreateCommentDto) 
    {
        return this.makeRequest<null, CreateCommentDto>(`/api/taskboard/createcomment`, "POST", dto);
    };
    public static EditComment  (id: number, dto: EditCommentDto) 
    {
        return this.makeRequest<null, EditCommentDto>(`/api/taskboard/editcomment/${id}`, "PATCH", dto);
    };
    public static DeleteComment  (id: number) 
    {
        return this.makeRequest<null, null>(`/api/taskboard/deletecomment/${id}`, "DELETE");
    };

    public static SetBoardPrivate  (id: number)
    {
        return this.makeRequest<null, null>(`/api/taskboard/setboardprivate/${id}`, "PATCH");
    };
    public static SetBoardPublic  (id: number) 
    {
        return this.makeRequest<null, null>(`/api/taskboard/setboardpublic/${id}`, "PATCH");
    };

    public static GiveAccess  (id: number, dto: GiveAccessDto)
    {
        return this.makeRequest<null, GiveAccessDto>(`/api/taskboard/giveaccess/${id}`, "POST", dto);
    };
    public static RemoveAccess  (id: number, dto: RemoveAccessDto) 
    {
        return this.makeRequest<null, RemoveAccessDto>(`/api/taskboard/removeaccess/${id}`, "DELETE", dto);
    };

}