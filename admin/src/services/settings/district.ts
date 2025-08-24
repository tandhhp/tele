import { request } from "@umijs/max";

export async function apiDistrictOptions(params: any) {
    return request(`district/options`, { params });
}