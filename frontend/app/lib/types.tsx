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

export interface AccountDto {
  id: number;
  email: string;
  roleID: number;
  avatar: string | null;
  trangThai: boolean;
  tenHienThi: string | null;
  ngayTao: string;
}

export interface Pagination {
  items: AccountDto[];
  totalPages: number;
  pageNumber: number;
  pageSize: number;
}
