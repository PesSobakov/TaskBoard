import { ListDto } from "./ListDto"
import { Privatness } from "./Privatness"

export interface BoardDto
{
    id: number,
    lists: ListDto[],
    name: string
    description: string
    privatness: Privatness
}