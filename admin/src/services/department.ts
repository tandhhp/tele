import { request } from "@umijs/max";

export async function apiDepartmentList(params: any) {
    return request(`department/list`, { params })
}

export async function apiDepartmentCreate(data: any) {
    return request(`department/create`, {
        method: 'POST',
        data
    })
}

export async function apiDepartmentUpdate(data: any) {
    return request(`department/update`, {
        method: 'POST',
        data
    })
}

export async function apiDepartmentDelete(id: string) {
    return request(`department/delete/${id}`, {
        method: 'POST'
    })
}