import type {Address, ObjectId, UserResponse} from "../../api/openApi/model";
import {UserRoles} from "./userRoles.ts";

// It can also be a type instead of a class, we will see how it will look in the future
export default class AppUser {
    public id: ObjectId;
    public name: string;
    public surname: string;
    public email: string;
    public address: Address;
    public roles: number[];

    constructor(
        id: ObjectId,
        name: string,
        surname: string,
        email: string,
        address: Address,
        roles: number[],
    ) {
        this.roles = roles;
        this.address = address;
        this.email = email;
        this.surname = surname;
        this.name = name;
        this.id = id;
    }

    get isAdmin(): boolean {
        return this.roles.includes(UserRoles.Admin)
    }
}

// export type AppUser = {
//     id: ObjectId;
//     name: string;
//     surname: string;
//     email: string;
//     address: Address;
//     roles: number[];
// }

export function mapResponseToUser(response: UserResponse, roles: number[]): AppUser {
    return new AppUser(
        response.id,
        response.name,
        response.surname,
        response.email,
        response.address,
        roles
    );
}