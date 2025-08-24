import { request } from '@umijs/max';

export async function apiPlasmaCount() {
  return request('/plasma/count');
}

export async function apiPlasmaList(params: any) {
  return request('plasma/list', {
    method: 'GET',
    params,
  });
}

export function apiCreatePlasma(data: any) {
  return request('plasma/create', {
      method: 'POST',
      data, 
  });
}

export function apiUpdatePlasma(data: any) {
  return request(`plasma/update`, {
      method: 'POST',
      data,
  });
}

export function apiDeletePlasma(id: string) {
  return request(`plasma/delete/${id}`, {
      method: 'POST',
  });
}

export function apiTechicianOptions() {
  return request(`plasma/technician-options`);
}
