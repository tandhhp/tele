import { request } from "@umijs/max";

export async function apiCallOptions(params: any) {
    return await request('call/status/options', { params });
}

export async function apiCallComplete(data: any) {
    return await request('call/complete', {
        method: 'POST',
        data
    });
}

export async function apiCallHistories(params: any) {
    return await request('call/histories', { params });
}

export async function apiCallStatistics() {
    return await request('call/statistics');
}