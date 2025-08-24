export default (initialState: { currentUser?: API.User }) => {
  const { currentUser } = initialState ?? {};
  const canAdmin = currentUser && currentUser.roles.includes('Admin');
  
  const canCX = currentUser && (currentUser.roles.includes('cx') || currentUser.roles.includes('cxtp') || canAdmin);

  const canAccountant = currentUser && (currentUser.roles.includes('accountant') || currentUser.roles.includes('ChiefAccountant') || currentUser.roles.includes('admin'));

  const canCXM = currentUser && (currentUser.roles.includes('cxtp') || currentUser.roles.includes('admin'));
  const canPlasma = currentUser && (currentUser.roles.includes('plasma') || currentUser.roles.includes('admin'));

  const cxm = currentUser && currentUser.roles.includes('cxtp');
  const cx = currentUser && currentUser.roles.includes('cx');

  // Quản lý nhân viên
  const canHR = currentUser && (currentUser.roles.includes('hr') 
  || currentUser.roles.includes('dos')
  || currentUser.roles.includes('admin'));

  const canComment = currentUser && (currentUser.roles.includes('cx') 
  || currentUser.roles.includes('cxtp')
  || currentUser.roles.includes('admin'));

  // Quản lý form đăng ký
  const canForm = currentUser && (currentUser.roles.includes('cx')
  || currentUser.roles.includes('accountant')
  || currentUser.roles.includes('ChiefAccountant')
  || currentUser.roles.includes('cxtp')
  || currentUser.roles.includes('admin'));

  // Quản lý điểm
  const canDeposit = currentUser && (currentUser.roles.includes('cx')
  || currentUser.roles.includes('cxtp'));
  // Quản lý chủ thẻ
  const canCardHolder = currentUser && (currentUser.roles.includes('cx')
  || currentUser.roles.includes('cxtp')
  || currentUser.roles.includes('sales')
  || currentUser.roles.includes('accountant')
  || currentUser.roles.includes('ChiefAccountant')
  || currentUser.roles.includes('dos')
  || currentUser.roles.includes('sm')
  || currentUser.roles.includes('admin'));
  // Thêm sửa, xóa chủ thẻ
  const canCRUDCardHolder = currentUser && (currentUser.roles.includes('cx')
  || currentUser.roles.includes('cxtp'));
  const canApproveComment = currentUser && (currentUser.roles.includes('cxtp')
  || currentUser.roles.includes('admin'));

  const canSales = currentUser && (currentUser.roles.includes('sales')
  || currentUser.roles.includes('sm')
  || currentUser.roles.includes('dos')
  || currentUser.roles.includes('admin'));

  const canCreateEmployee = currentUser && (currentUser.roles.includes('hr')
  || currentUser.roles.includes('admin'));

  const canDos = currentUser && (currentUser.roles.includes('dos') || currentUser.roles.includes('admin'));

  const canSm = currentUser && (currentUser.roles.includes('sm')
  || currentUser.roles.includes('admin'));

  const canDosAccountant = currentUser && (currentUser.roles.includes('dos')
  || currentUser.roles.includes('ChiefAccountant')
  || currentUser.roles.includes('accountant')
  || currentUser.roles.includes('admin'));

  const canUserChange = currentUser && (currentUser.roles.includes('hr') || currentUser.roles.includes('dos') || currentUser.roles.includes('admin'));

  const canViewChart = currentUser && (currentUser.roles.includes('accountant')
  || currentUser.roles.includes('ChiefAccountant')
  || currentUser.roles.includes('sm')
  || currentUser.roles.includes('dos')
  || currentUser.roles.includes('admin'));

  const sales = currentUser && currentUser.roles.includes('sales');
  const sm = currentUser && currentUser.roles.includes('sm');
  const dos = currentUser && currentUser.roles.includes('dos');
  const event = currentUser && currentUser.roles.includes('event');
  const telesale = currentUser && currentUser.roles.includes('Telesale');
  const telesaleManager = currentUser && currentUser.roles.includes('TelesaleManager');
  const dot = currentUser && currentUser.roles.includes('dot');
  const hr = currentUser && currentUser.roles.includes('hr');
  const trainer = currentUser && currentUser.roles.includes('Trainer');
  const plasma = currentUser && currentUser.roles.includes('plasma');
  const accountant = currentUser && currentUser.roles.includes('accountant');
  const chiefAccountant = currentUser && currentUser.roles.includes('ChiefAccountant');

  const canEvent = currentUser && (sales || currentUser.roles.includes('event') || canCX || currentUser.roles.includes('sm') 
  || currentUser.roles.includes('Telesale')
  || currentUser.roles.includes('TelesaleManager')
  || currentUser.roles.includes('dos')
  || currentUser.roles.includes('admin')
  || dot);

  const canLead = canSales ||
   (currentUser && (currentUser.roles.includes('Telesale') || currentUser.roles.includes('TelesaleManager') || currentUser.roles.includes('event') 
    || dot))
  
  const canCardHolderQueue = currentUser && (currentUser.roles.includes('ChiefAccountant')
  || currentUser.roles.includes('cx')
  || currentUser.roles.includes('cxtp')
  || currentUser.roles.includes('dos')
  || currentUser.roles.includes('admin')
  || currentUser.roles.includes('event')
  || currentUser.roles.includes('sales')
  || currentUser.roles.includes('sm')
  || currentUser.roles.includes('accountant'));

  return {
    canAdmin,
    canCX,
    canCXM,
    canAccountant,
    canHR,
    canComment,
    canForm,
    canDeposit,
    canCardHolder,
    canCRUDCardHolder,
    canApproveComment,
    canSales,
    canCreateEmployee,
    canDos,
    canSm,
    canPlasma,
    canDosAccountant,
    canUserChange,
    canViewChart,
    canEvent,
    sales,
    event,
    canCardHolderQueue,
    cx,
    cxm,
    dos,
    canLead,
    telesale,
    telesaleManager,
    hr,
    trainer,
    dot,
    plasma,
    accountant,
    chiefAccountant,
    sm
  };
};
