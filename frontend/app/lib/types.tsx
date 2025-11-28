export interface LoginResponse {
  token?: string;
  message?: string;
  user?: AccountDetail;
}

export interface AccountDetail {
  ID: number;
  Email: string;
  RoleID?: string;
  Avatar?: string | null;
  TrangThai?: boolean;
  TenHienThi?: string | null;
  NgayTao?: string | null;
}
