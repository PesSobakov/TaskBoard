import { UserDto } from "./UserDto";

export interface CommentDto
{
    id: number,
    text: string,
    created: Date,
    edited: Date|null,
    user: UserDto
}