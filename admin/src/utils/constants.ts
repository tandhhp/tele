export enum FormType {
  Holtel,
  PrivateStay,
  Flight,
  Activity,
  AirportTransfer,
  Tour,
  Healthcare,
}

export enum LeadStatus {
  Pending,
  Approved,
  Checkin,
  LeadAccept,
  LeadReject,
  Done,
  AccountantApproved,
  DosApproved,
  ReInvite,
}

export enum TopupType {
  Topup,
  Debt,
}

export enum Tier {
  STANDARD,
  ELITE,
  ROYAL,
  PREMIER,
}

export const SOURCES = [
  'PRIVATE',
  'TELE OPC',
  'TELE INHOUSE',
  'MKT',
  'OUTSIDE',
  'NO SOURCE'
];

export enum UserStatus {
  Working,
  Leave
}

export enum Branch {
  South,
  North
}

export const BRANCH_OPTIONS = [
  { label: 'Tất cả', value: null },
  { label: 'Hồ Chí Minh', value: Branch.South },
  { label: 'Hà Nội', value: Branch.North }
];

export enum TransactionType
{
    Default,
    Bonus,
    Loan
}

export const SETTING_NAME = {
  CHATGPT: 'ChatGPT'
};