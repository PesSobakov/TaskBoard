import { Permission } from "./TaskBoard/Permission";

export interface GiveAccessDto
{
    permission: Permission,
    user: string
}