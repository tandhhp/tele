import { request } from '@umijs/max';

export async function apiPlasmaCheckInList(params: any) {
    return request('/plasmaCheckIn/list', {
      method: 'GET',
      params,
    });
}

export function apiPlasmaUser() {
  return request(`plasmaCheckIn/plasmaUser`);
}

export function apiCreatePlasmaCheckIn(data: any) {
    return request('plasmaCheckIn/create', {
        method: 'POST',
        data, 
    });
  }
  
export function apiUpdatePlasmaCheckIn(data: any) {
  return request(`plasmaCheckIn/update`, {
      method: 'POST',
      data,
  });
}

export function apiDeletePlasmaCheckIn(id: string) {
  return request(`plasmaCheckIn/delete/${id}`, {
      method: 'POST',
  });
}
  