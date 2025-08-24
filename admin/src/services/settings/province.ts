import { request } from "@umijs/max";

export async function apiProvinceList(params: any) {
    return request(`province/list`, {
        params
    })
}

export async function apiProvinceOptions(params: any) {
    return request(`province/options`, { params });
}