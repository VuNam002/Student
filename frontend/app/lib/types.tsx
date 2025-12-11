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

export interface RoleDto {
  RoleID: number;
  RoleCode: string;
  RoleName: string;
  Description: string;
  IsDeleted: boolean;
  CreatedAt: string;
  permissions?: PermissionDto[];  
}

export interface RolePermissionDto {
  roleId: number;
  permissionId: number;
}

export interface PermissionDto {
  PermissionID: number;
  PermissionCode: string;
  PermissionName: string;
  Module: string;
  Description: string;
  IsDeleted: boolean;
  CreatedAt: string;
  UpdatedAt: string;
}

export interface PermissionItemDto {
  PermissionID: number;
  PermissionCode: string;
  PermissionName: string;
  Description: string | null;
}

export interface PermissionGroupDto {
  Module: string;
  Permissions: PermissionItemDto[];
}

export interface Student {
  StudentID: number;
  StudentCode: string;
  ClassName: string;
  Status: number;
  Person: {
    FullName: string;
    Email: string;
    PhoneNumber: string;
    Gender: string;
    DateOfBirth: string;
  };
}