import { request } from "@umijs/max";

export async function apiCalendarData(params: any) {
    return request(`calendar/list`, { params });
}

export async function apiCalendarEvents(params: any) {
    return request(`calendar/events`, { params });
}

export async function apiCalendarPlasma(params: any) {
    return request(`calendar/plasma`, { params });
}