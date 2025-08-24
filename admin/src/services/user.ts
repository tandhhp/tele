import { request } from '@umijs/max';

export async function login(data: any) {
  return request(`user/password-sign-in`, {
    method: 'POST',
    data,
  });
}

export async function queryCurrentUser() {
  return request(`user`);
}

export async function getUser(id: string | undefined) {
  return request(`user/${id}`);
}

export async function apiChangeAvatar(data: any) {
  return request(`user/change-avatar`, {
    data,
    method: 'POST',
    headers: {
      'Content-Type': 'multipart/form-data'
    }
  })
}

export const apiGetStatisticsUser = (id: string) => request(`user/card-holder/statistics/${id}`);

export const apiGetUserInfo = (id: string) => request(`user/info/${id}`);

export const apiUpdateUserInfo = (data: any) => request(`user/info/save`, {
  method: 'POST',
  data
})

export async function createCardHolder(data: any) {
  return request(`user/create-card-holder`, {
    method: 'POST',
    data,
  });
}

export async function createEmployee(data: any) {
  return request(`user/create-member`, {
    method: 'POST',
    data,
  });
}


export async function apiUserUpdate(data: any) {
  return request(`user/update`, {
    method: 'POST',
    data,
  });
}

export async function apiUserList(params: any) {
  return request(`user/list`, { params });
}

export async function listTrainerUser(params: any) {
  return request(`user/trainer/list-user`, { params });
}

export async function changePassword(data: any) {
  return request(`user/change-password`, {
    method: 'POST',
    data,
  });
}

export async function deleteUser(id?: string) {
  return request(`user/delete/${id}`, {
    method: 'POST'
  })
}

export const apiLockUser = (id: string) => request(`user/leave/${id}`, {
  method: 'POST'
})

export const apiUnLockUser = (id: string) => request(`user/unlock/${id}`, {
  method: 'POST'
})


export async function addToRole(data: any) {
  return request(`user/add-to-role`, {
    method: 'POST',
    data
  });
}

export async function getUserInRoles(params: any, roleName?: string | string[]) {
  return request(`user/users-in-role/${roleName}`, { params });
}

export async function apiConfirmEmail(id?: string | string[]) {
  return request(`user/confirm-email/${id}`, {
    method: 'POST'
  });
}

export const apiUpdateLoyalty = (data: any) => request(`user/loyalty/save`, {
  method: 'POST',
  data
})

export async function apiGetMyTransactions(userId?: string) {
  return request(`user/transactions/${userId}`);
}

export const apiListCard = (params: any) => request(`user/card/list`, { params });

export const apiAddCard = (data: any) => request(`user/card/add`, {
  method: 'POST',
  data
});

export const apiUpdateCard = (data: any) => request(`user/card/update`, {
  method: 'POST',
  data
});

export const apiCardOptions = () => request(`user/card/options`);

export const apiDeleteCard = (id?: string) => request(`user/card/delete/${id}`, { method: 'POST' });

export const apiSubUserList = (id?: string) => request(`user/sub-user/list/${id}`);

export const apiSubUserAdd = (data: any) => request(`user/sub-user/add`, {
  method: 'POST',
  data
});

export const apiSubUserUpdate = (data: any) => request(`user/sub-user/update`, {
  method: 'POST',
  data
});

export const apiSubUserDelete = (id: string) => request(`user/sub-user/delete/${id}`, {
  method: 'POST'
})

export const apiRoleOptions = () => request(`user/role/options`);

export const apiDosOptions = () => request(`user/dos/options`);

export const apiSmOptions = (id?: string) => request(`user/sm/options/${id}`);

export const apiUserByRoleOptions = (name: string) => request(`role/user-options-in-role/${name}`);

export const apiTmOptions = () => request(`user/telesale-manager/options`);

export const apiLotaltyApproveList = (params: any) => request(`user/loyalty/list-approve`, { params });

export const apiApproveDeposit = (id: string) => request(`user/loyalty/approve/${id}`, {
  method: 'POST'
})

export const apiRejectDeposit = (id: string) => request(`user/loyalty/reject/${id}`, {
  method: 'POST'
})

export const apiGetSalesOptions = () => request(`user/sales/options`);

export const apiSetSaller = (data: any) => request(`user/sales/set-seller`, {
  method: 'POST',
  data
});

export const apiAchievementList = () => request(`user/achievements`);

export const apiAchievementListById = (id: string) => request(`user/achievements-by-user/${id}`);

export const apiExportCardHolder = (params: any) => request(`user/export`, {
  params,
  responseType: 'blob'
});

export const apiExportLead = (data: any) => request(`contact/export-lead`, {
  responseType: 'blob',
  method: 'POST',
  data
});

export const apiSalesOptions = () => request(`user/sales/options`);

export const apiSalesWithSmOptions = () => request(`user/sales-with-sm-options`);

export const apiTeleWithTmOptions = () => request(`user/tele-with-tm-options`);

export const apiSendEmails = (data: any) => request(`user/send-emails`, {
  method: 'POST',
  data
})

export const apiCardHolderOptions = () => request(`user/card-holder/options`);

export const apiListNewCardHolder = (params: any) => request(`user/list-new-card-holder`, { params });

export const apiListTopSales = (params: any) => request(`user/list-top-sales`, { params });

export const apiTopUp = (data: any) => request(`user/topup`, {
  method: 'POST',
  data
});

export const apiListTopup = (params: any) => request(`user/list-topup`, { params });

export const apiApproveTopup = (data: any) => request(`user/approve-topup`, {
  method: 'POST',
  data
});

export const apiAmountReport = () => request(`report/amounts`);

export const apiSaleChartData = () => request(`user/sale-chart`);

export const apiSmChartData = () => request(`user/sm-chart`);

export const apiSalesListReport = (year: number) => request(`report/sales`, {
  params: { year }
});

export const apiUserChangeList = (params: any) => request(`user/changes-list`, { params });

export const apiUserChangeApprove = (id: string) => request(`user/approve-changes/${id}`, {
  method: 'POST'
});

export const apiAchievementAdd = (data: any) => request(`user/achievement/add`, {
  method: 'POST',
  data
});

export const apiListApproveAchievent = (params: any) => request(`user/achievement/list-approve`, { params });

export const apiAchieventApprove = (id: string) => request(`user/achievement/approve/${id}`, {
  method: 'POST'
});

export const apiAchieventRemove = (id: string) => request(`user/achievement/delete/${id}`, {
  method: 'POST'
});

export const apiLeadFeedback = (params: any) => request(`contact/feedback/list`, { params });

export const apiKeyinByTelesale = (params: any) => request(`report/keyin-by-telesale`, { params });

export async function apiSetPassword(data: any) {
  return request(`user/set-password`, {
    method: 'POST',
    data
  })
}

export async function apiUpdateContractCode(data: any) {
  return request(`user/change-contract-code`, {
    method: 'POST',
    data
  })
}

export async function apiSaleRevenueReport() {
  return request(`user/my-revenue-total`)
}

export async function apiMyTeam(params: any) {
  return request(`user/team`, { params })
}

export async function apiUserPointByCardHolder(params: any) {
  return request(`user/points`, { params });
}

export async function apiUserProfileUpdate(data: any) {
  return request(`user/update-profile`, {
    method: 'POST',
    data
  });
}

export async function apiLoanPoint(data: any) {
  return request(`user/loan-point`, {
    method: 'POST',
    data
  });
}