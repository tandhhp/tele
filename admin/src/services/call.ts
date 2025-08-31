import { request } from "@umijs/max";

export async function apiCallOptions(params: any) {
    return await request('call/status/options', { params });
}