import { request } from '@umijs/max';

export async function apiBackToCheckin(data: any) {
    return request(`event/back-to-checkin`, {
        method: 'POST',
        data
    });
}

export async function apiListEventSaleRevenue(params: any) {
    return request(`event/list-sale-revenue`, { params });
}

export async function apiListKeyInRevenue(params: any) {
    return request(`event/list-key-in-revenue`, { params });
}

export async function apiDebtHistoryList(params: any) {
    return request(`event/revenue-history`, { params });
}

export async function apiTopupKeyIn(data: any) {
    return request(`event/add-sale-revenue`, {
        method: 'POST',
        data,
        headers: {
            'Content-Type': 'multipart/form-data',
        },
    });
}