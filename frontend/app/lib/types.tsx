export interface LoginResponse {
  token?: string;
  message?: string;
  user?: AccountDetail;
}

export interface AccountDetail {
  ID: number;
  Email: string;
  RoleID: string | number;
  Avatar: string;
  Status: number;
  FullName: string;
  PhoneNumber: string;
  RoleName?: string;
  DateOfBirth?: string;
  Gender?: string;
  Address?: string;
  IdentityCard?: string;
  DepartmentID?: number;
  Position?: string;
  Degree?: string;
  Specialization?: string;
  ClassID?: number;
  EnrollmentDate?: string;
  CreatedAt?: string;
  UpdatedAt?: string;
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

export interface StudentDetail {
  StudentID: number;
  StudentCode: string;
  PersonID: number;
  ClassName: number;
  EnrollmentDate: string;
  Status: number;
  CreatedAt: string;
  UpdatedAt: string;
  Person: {
    FullName: string;
    DateOfBirth: string;
    Gender: string;
    Email: string;
    PhoneNumber: string;
    Address: string;
  };
}

export interface StudentDto {
  StudentCode: string;
  ClassID: number;
  Status: number;
  EnrollmentDate: string;
  PersonID: {
    FullName: string;
    DateOfBirth: string;
    Gender: string;
    Email: string;
    PhoneNumber: string;
    Address: string;
  };
}

export interface ClassDto {
  ClassId: number;
  ClassCode: string;
  ClassName: string;
  DepartmentId: number;
  DepartmentName: string;
  TeacherId: number;
  TeacherName: string;
  AcademicYear: string;
  Semester: number;
  TotalStudents: number;
  CreatedAt: string;
  UpdatedAt: string;
  IsDeleted: boolean;
}

export interface ClassPagination {
  Classes: ClassDto[];
  TotalCount: number;
  Page: number;
  PageSize: number;
  TotalPages: number;
  HasPrevious: boolean;
  HasNext: boolean;
}

export interface FormData {
  ID: number;
  Email: string;
  RoleID: string;
  Avatar: string;
  Status: number;
  FullName: string;
  PhoneNumber: string;
  Password: string;
  DateOfBirth: string;
  Gender: string;
  Address: string;
  IdentityCard: string;
  DepartmentID: string;
  Position: string;
  Degree: string;
  Specialization: string;
  ClassID: string;
  EnrollmentDate: string;
}