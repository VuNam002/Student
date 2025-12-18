import { LoginResponse, AccountDetail, PermissionGroupDto, RoleDto, PermissionDto, Student, ClassPagination } from "./types";

const API_URL = "http://localhost:5262/api";

async function api<T>(url: string, options: RequestInit = {}): Promise<T> {
  const token = localStorage.getItem("token");

  const headers = {
    "Content-Type": "application/json",
    ...options.headers,
  };

  if (token) {
    headers["Authorization"] = `Bearer ${token}`;
  }

  const response = await fetch(url, { ...options, headers });

  if (response.status === 401) {
    localStorage.removeItem("token");
    localStorage.removeItem("user");
    if (!window.location.pathname.toLowerCase().includes("/login")) {
      window.location.href = "/login";
    }
    throw new Error("Unauthorized");
  }

  if (!response.ok) {
    const errorData = await response.text();
    throw new Error(
      errorData || `Request failed with status ${response.status}`
    );
  }

  if (
    response.headers.get("Content-Length") === "0" ||
    response.status === 204
  ) {
    return null as T;
  }

  if (response.headers.get("Content-Type")?.includes("application/json")) {
    return response.json() as Promise<T>;
  }

  return response.text() as Promise<T>;
}

export async function fetchlogin(
  Email: string,
  Password: string
): Promise<LoginResponse> {
  try {
    const data = await api<any>(`${API_URL}/Account/login`, {
      method: "POST",
      body: JSON.stringify({ Email, Password }),
    });

    const token = typeof data === "string" ? data : data?.token || data?.Token;

    if (token) {
      localStorage.setItem("token", token);
      return { token };
    }
    return { message: "Received an empty token." };
  } catch (error: unknown) {
    console.error("Login API error:", error);
    const errorMessage = error instanceof Error ? error.message : "An unexpected error occurred.";
    return { message: errorMessage };
  }
}

export async function fetchUserFromToken(): Promise<AccountDetail | null> {
  try {
    const token = localStorage.getItem("token");
    if (!token) return null;

    return await fetchAccountMe();
  } catch (error) {
    console.warn("Failed to fetch user from token", error);
    return null;
  }
}

export async function fetchAccount(
  Page: number = 1,
  pageSize: number = 5,
  Keyword: string = "",
  Status?: boolean
) {
  const params = new URLSearchParams({
    page: Page.toString(),
    pageSize: pageSize.toString(),
  });

  if (Keyword) {
    params.append("Keyword", Keyword);
  }

  if (Status !== undefined) {
    params.append("TrangThai", Status.toString());
  }

  const url = `${API_URL}/Account/paginated?${params.toString()}`;

  try {
    const response = await api<any>(url, { method: "GET" });
    const data = response?.data || response;
    const accounts = data?.Account || [];

    return {
      items: accounts.map((account: any) => ({
        ID: account.ID,
        Email: account.Email,
        roleId: account.RoleID,
        Avatar: account.Avatar,
        Status: account.Status,
        RoleName: account.RoleName,
        CreatedAt: account.CreatedAt,
        FullName: account.FullName,
        PhoneNumber: account.PhoneNumber,
      })),
      totalPages: data?.TotalPages || 0,
      currentPage: data?.Page || 1,
      totalCount: data?.TotalCount || 0,
      pageSize: data?.PageSize || pageSize,
      hasPrevious: data?.HasPrevious || false,
      hasNext: data?.HasNext || false,
    };
  } catch (error) {
    console.error(error);
    throw error;
  }
}

export async function fetchAccountById(id: number): Promise<AccountDetail | null> {
  try {
    const response = await api<any>(`${API_URL}/Account/${id}`, { method: "GET" });
    return response?.data || response;
  } catch (error) {
    console.error("Fetch account by ID API error:", error);
    return null;
  }
}

export async function fetchAccountEdit(id: number, updateAccount: any) {
  try {
    const response = await api<any>(`${API_URL}/Account/${id}`, {
      method: "PATCH",
      body: JSON.stringify(updateAccount),
    });
    return response?.data || response;
  } catch (error) {
    console.error("Fetch account edit API error:", error);
    if (error instanceof Error) {
      try {
        return JSON.parse(error.message);
      } catch {}
    }
    return null;
  }
}

export async function fetchAccountCreat(newAccount: any) {
  try {
    const response = await api<any>(`${API_URL}/Account/create`, {
      method: "POST",
      body: JSON.stringify(newAccount),
    });
    return response?.data || response;
  } catch (error) {
    console.error("Fetch account creat API error:", error);
    return null;
  }
}

export async function fetchAccountDeleted(id: number): Promise<boolean | null> {
  try {
    await api<unknown>(`${API_URL}/Account/${id}`, {
      method: "DELETE",
    });
    return true;
  } catch (error) {
    console.error("Delete account API error:", error);
    return null;
  }
}

export async function fetchAccountStatus(id: number, Status: number) {
  try {
    const response = await api<any>(`${API_URL}/Account/${id}/status`, {
      method: "PATCH",
       body: JSON.stringify({ Status }),
    });
    return response?.data || response;
  } catch (error) {
    console.error("Fetch account status API error:", error);
    return null;
  }
}

export async function fetchAccountMe(): Promise<AccountDetail | null> {
  try {
    const response = await api<any>(`${API_URL}/Account/me`, { method: "GET" });
    return response?.data || response;
  } catch (error) {
    console.error("Fetch account me API error:", error);
    return null;
  }
}

export async function logout() {
  try {
    await api<unknown>(`${API_URL}/Account/logout`, { method: "POST" });
  } catch (error) {
    console.error("Logout API error:", error);
  } finally {
    localStorage.removeItem("token");
    localStorage.removeItem("user");
    window.location.href = "/login";
  }
}

export async function fetchRole(): Promise<RoleDto[] | null> {
  try {
    const response = await api<any>(`${API_URL}/Role`, { method: "GET" });
    return response?.data || response;
  } catch (error) {
    console.error("Fetch role API error:", error);
    return null;
  }
}

export async function fetchRoleById(id: number) {
  try {
    const response = await api<any>(`${API_URL}/Role/${id}`, { method: "GET" });
    return response?.data || response;
  } catch (error) {
    console.error("Fetch role by ID API error:", error);
    return null;
  }
}

export async function fetchRoleCreate(newRole: any) {
  try {
    const response = await api<any>(`${API_URL}/Role`, {
      method: "POST",
      body: JSON.stringify(newRole),
    });
    return response?.data || response;
  } catch (error) {
    console.error("Fetch role create API error:", error);
    return null;
  }
}

export async function fetchRoleEdit(id: number, updateRole: any) {
  try {
    const response = await api<any>(`${API_URL}/Role/${id}`, {
      method: "PATCH",
      body: JSON.stringify(updateRole),
    });
    return response?.data || response;
  } catch (error) {
    console.error("Fetch role edit API error:", error);
    return null;
  }
}

export async function fetchRoleDeleted(id: number): Promise<boolean | null> {
  try {
    await api<unknown>(`${API_URL}/Role/${id}`, {
      method: "DELETE",
    });
    return true;
  } catch (error) {
    console.error("Delete role API error:", error);
    return null;
  }
}

export async function assignPermissionsToRole(
  roleId: number,
  permissionIds: number[]
): Promise<any> {
  try {
    const body = { permissionIds };
    return await api<unknown>(`${API_URL}/Role/${roleId}/permissions`, {
      method: "POST",
      body: JSON.stringify(body),
    });
  } catch (error) {
    console.error(`Assign permissions to role ${roleId} API error:`, error);
    throw error;
  }
}

export async function fetchDetailRole(roleId: number) {
  try {
    const response = await api<any>(`${API_URL}/Role/${roleId}`, {
      method: "GET",
    });
    return response?.data || response;
  } catch (error) {
    console.error("Fetch detail role API error:", error);
    return null;
  }
}

export async function fetchPermissions(roleId: number): Promise<PermissionDto[] | null> {
  try {
    const url = `${API_URL}/Role/${roleId}/permissions`;
    const response = await api<any>(url, { method: "GET" });
    return response?.data || response;
  } catch (error) {
    console.error(`Fetch permissions for role ${roleId} API error:`, error);
    return null;
  }
}

export async function fetchAllPermissions(
  module?: string
): Promise<PermissionGroupDto[] | null> {
  try {
    const params = new URLSearchParams();
    if (module) {
      params.append("module", module);
    }
    const url = `${API_URL}/permissions?${params.toString()}`;
    const response = await api<any>(url, { method: "GET" });
    return response?.data || response;
  } catch (error) {
    console.error("Fetch all permissions API error:", error);
    return null;
  }
}

export async function deletePermission(permissionId: number): Promise<boolean> {
  try {
    await api<unknown>(`${API_URL}/permissions/${permissionId}`, {
      method: "DELETE",
    });
    return true;
  } catch (error) {
    console.error("Delete permission API error:", error);
    throw error;
  }
}

export async function fetchPermissionCreate(newPermission: any) {
  try {
    const response = await api<any>(`${API_URL}/permissions`, {
      method: "POST",
      body: JSON.stringify(newPermission),
    });
    return response?.data || response;
  } catch (error) {
    console.error("Fetch permission create API error:", error);
    return null;
  }
}

export async function fetchAllStudent(
  Page: number = 1,
  pageSize: number = 5,
  Keyword: string = "",
  Status?: number | null
) {
  const params = new URLSearchParams({
    page: Page.toString(),
    pageSize: pageSize.toString(),
  });

  if (Keyword) {
    params.append("Keyword", Keyword);
  }

  if (Status !== undefined && Status !== null) {
    params.append("Status", Status.toString());
  }

  const url = `${API_URL}/Student/paginated?${params.toString()}`;

  try {
    const data = await api<{
      Student: Student[];
      TotalPages: number;
      Page: number;
      TotalCount: number;
      PageSize: number;
      HasPrevious: boolean;
      HasNext: boolean;
    }>(url, { method: "GET" });
    return {
      items: data.Student,
      totalPages: data.TotalPages,
      currentPage: data.Page,
      totalCount: data.TotalCount,
      pageSize: data.PageSize,
      hasPrevious: data.HasPrevious,
      hasNext: data.HasNext,
    };
  } catch (error) {
    console.error("Fetch all student API error:", error);
    throw error;
  }
}

export async function fetchStudentStatus(id: number, Status: number) {
  try {
    const response = await api<any>(`${API_URL}/Student/${id}/status`, {
      method: "PATCH",
      body: JSON.stringify(Status),
    });
    return response?.data || response;
  } catch (error) {
    console.error("Fetch student status API error:", error);
    return null;
  }
}

export async function fetchDetailStudent(StudentID:number) {
  try {
    const response = await api<any>(`${API_URL}/Student/${StudentID}`, {
      method: "GET",
    });
    return response?.data || response;
  } catch (error) {
    console.error("Fetch detail student API error:", error);
    return null;
  }
}

export async function fetchUpdateStudent(id:number, updateStudent: any) {
  try {
    const response = await api<any>(`${API_URL}/Student/${id}`, {
      method: "PATCH",
      body: JSON.stringify(updateStudent),
    });
    return response?.data || response;
  } catch (error) {
    console.error("Fetch update student API error:", error);
    return null;
  }
}

export async function fetchCreateStudent(newStudent:any) {
  try {
    const response = await api<any>(`${API_URL}/Student`, {
      method: "POST",
      body: JSON.stringify(newStudent),
    });
    return response?.data || response;
  } catch (error) {
    console.error("Fetch create student API error:", error);
    return null;
  }
}

export async function fetchClasses() {
  try {
    const response = await api<ClassPagination>(`${API_URL}/Class/pagination?PageSize=100`, { method: "GET" });
    return response;
  } catch (error) {
    console.error("Fetch classes API error:", error);
    return null;
  }
}

export async function fetchClassesPaginated(
  Page: number = 1,
  pageSize: number = 10,
  Keyword: string = ""
) {
  const params = new URLSearchParams({
    Page: Page.toString(),
    PageSize: pageSize.toString(),
  });

  if (Keyword) {
    params.append("Keyword", Keyword);
  }

  try {
    const response = await api<ClassPagination>(`${API_URL}/Class/pagination?${params.toString()}`, { method: "GET" });
    return response;
  } catch (error) {
    console.error("Fetch classes paginated API error:", error);
    return null;
  }
}

export async function fetchClassById(id: number) {
  try {
    const response = await api<any>(`${API_URL}/Class/${id}`, { method: "GET" });
    return response?.data || response;
  } catch (error) {
    console.error("Fetch class by ID API error:", error);
    return null;
  }
}

export async function fetchCreateClass(newClass:any) {
  try {
    const response = await api<any>(`${API_URL}/Class`, {
      method: "POST",
      body: JSON.stringify(newClass),
    });
    return response?.data || response;
  } catch (error) {
    console.error("Fetch create class API error:", error);
    return null;
  }
}

export async function fetchEditClass(id:number,editClass:any ) {
  try {
    const response = await api<any>(`${API_URL}/Class/${id}`, {
      method: "PATCH",
      body: JSON.stringify(editClass),
    });
    return response?.data || response;
  } catch (error) {
    console.error("Fetch edit class API error:", error);
    return null;
  }
}

export async function fetchExportStudentByClass(classId: number) {
  try {
    const token = localStorage.getItem("token");
    // Endpoint xuất Excel danh sách sinh viên theo lớp (Class Controller)
    const response = await fetch(`${API_URL}/Class/${classId}/export-students`, {
      method: "GET",
      headers: {
        "Authorization": `Bearer ${token}`,
      },
    });

    if (!response.ok) throw new Error("Export failed");

    const blob = await response.blob();
    const url = window.URL.createObjectURL(blob);
    const a = document.createElement("a");
    a.href = url;
    a.download = `DanhSachSinhVien_Lop_${classId}.xlsx`;
    document.body.appendChild(a);
    a.click();
    a.remove();
    return true;
  } catch (error) {
    console.error("Export student by class API error:", error);
    return false;
  }
}

export async function fetchStudentByClass(id: number) {
  try {
    const response = await api<any>(
      `${API_URL}/Student/by-class/${id}`,
      { method: "GET" }
    );

    return response?.data ?? response;
  } catch (error) {
    console.error("Fetch student by class API error:", error);
    return null;
  }
}
