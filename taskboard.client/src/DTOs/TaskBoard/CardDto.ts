import { CommentDto } from "./CommentDto";

export interface CardDto
{
    id: number,
    name: string,
    description: string,
    status: string,
    order: number,
    dueDate: Date,
    comments: CommentDto[]
}