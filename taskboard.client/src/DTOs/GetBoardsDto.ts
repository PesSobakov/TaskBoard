import { Privatness } from "./TaskBoard/Privatness";

export interface GetBoardsDto
{
    name: string,
    description: string,
    privatness: Privatness
}