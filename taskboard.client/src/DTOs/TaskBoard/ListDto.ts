import { CardDto } from "./CardDto";

export interface ListDto
{
    id: number,
    order: number,
    name: string,
    cards: CardDto[]
}