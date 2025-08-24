import { request } from "@umijs/max";

export async function listRole(params: any) {
    return request(`/role/list`, {
        params
    })
}