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
  Status: number;
  RoleName?: string | null;
  CreatedAt?: string | null;
  FullName?: string | null;
  PhoneNumber?: string | null;
  Password?: string;
}

export interface AccountDto {
  ID: number;
  Email: string;
  RoleID: number;
  Avatar: string | null;
  Status: number;
  RoleName: string | null;
  FullName: string | null;
  CreatedAt: string;
  PhoneNumber: string | null;
  IsDeleted: boolean;
}

export interface Pagination {
  items: AccountDto[];
  totalPages: number;
  pageNumber: number;
  pageSize: number;
}
