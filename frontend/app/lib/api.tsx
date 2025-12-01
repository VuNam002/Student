import { LoginResponse, AccountDetail } from "./types";

const API_URL = 'http://localhost:5262/api';

async function api<T>(url: string, options: RequestInit = {}): Promise<T> {
    const token = localStorage.getItem('token');

    const headers = {
        'Content-Type': 'application/json',
        ...options.headers,
    };

    if (token) {
        headers['Authorization'] = `Bearer ${token}`;
    }

    const response = await fetch(url, { ...options, headers });

    if (response.status === 401) {
        localStorage.removeItem('token');
        localStorage.removeItem('user');
        window.location.href = '/login'; 
        throw new Error('Unauthorized');
    }

    if (!response.ok) {
        const errorData = await response.text();
        throw new Error(errorData || `Request failed with status ${response.status}`);
    }

    if (response.headers.get('Content-Length') === '0' || response.status === 204) {
        return null as T;
    }
    
    if (response.headers.get('Content-Type')?.includes('application/json')) {
        return response.json() as Promise<T>;
    }

    return response.text() as Promise<T>;
}


export async function fetchlogin(Email: string, MatKhau: string): Promise<LoginResponse> {
    try {
        const token = await api<string>(`${API_URL}/Account/login`, {
            method: 'POST',
            body: JSON.stringify({ Email, MatKhau }),
        });

        if (token) {
            localStorage.setItem('token', token);
            return { token };
        }
        return { message: 'Received an empty token.' };

    } catch (error: any) {
        console.error('Login API error:', error);
        return { message: error.message || 'An unexpected error occurred.' };
    }
}

export async function fetchUserFromToken(): Promise<AccountDetail | null> {
    try {
        const token = localStorage.getItem('token');
        if (!token) return null;
        
        return await fetchAccountMe();
    } catch (error) {
        console.warn('Failed to fetch user from token', error);
        return null;
    }
}

export async function fetchAccount(page: number = 1, pageSize: number = 5, Keyword: string = '', trangThai?: boolean) {
    const params = new URLSearchParams({
        page: page.toString(),
        pageSize: pageSize.toString(),
    });

    if (Keyword) {
        params.append('Keyword', Keyword);
    }

    if (trangThai !== undefined) {
        params.append('TrangThai', trangThai.toString());
    }

    const url = `${API_URL}/Account/paginated?${params.toString()}`;

    try {
        const data = await api<any>(url, { method: 'GET' });
        return {
            items: data.Account.map((account: any) => ({
                id: account.ID,
                email: account.Email,
                roleId: account.RoleID,
                avatar: account.Avatar,
                trangThai: account.TrangThai,
                tenHienThi: account.TenHienThi,
                ngayTao: account.NgayTao,
                HoTen: account.HoTen,
                SDT: account.SDT
            })),
            totalPages: data.TotalPages,
            currentPage: data.Page,
            totalCount: data.TotalCount,
            pageSize: data.PageSize,
            hasPrevious: data.HasPrevious,
            hasNext: data.HasNext
        };
    } catch (error) {
        console.error(error);
        throw error;
    }
}

export async function fetchAccountById(id: number): Promise<any | null> {
    try {
        return await api<any>(`${API_URL}/Account/${id}`, { method: 'GET' });
    } catch (error) {
        console.error('Fetch account by ID API error:', error);
        return null;
    }
}

export async function fetchAccountEdit(id: number, updateAccount: any) {
    try {
        return await api<any>(`${API_URL}/Account/${id}`, {
            method: 'PATCH',
            body: JSON.stringify(updateAccount)
        });
    } catch (error) {
        console.error('Fetch account edit API error:', error);
        return null;
    }
}

export async function fetchAccountCreat(newAccount: any) {
    try {
        return await api<any>(`${API_URL}/Account`, {
            method: 'POST',
            body: JSON.stringify(newAccount)
        });
    } catch (error) {
        console.error('Fetch account creat API error:', error);
        return null;
    }
}

export async function fetchAccountDeleted(id: number): Promise<boolean | null> {
    try {
        await api<any>(`${API_URL}/Account/${id}`, {
            method: 'DELETE',
        });
        return true;
    } catch (error) {
        console.error('Delete item API error:', error);
        return null;
    }
}

export async function fetchAccountStatus(id: number, trangThai: number) {
    try {
        return await api<any>(`${API_URL}/Account/${id}/status`, {
            method: 'PATCH',
            body: JSON.stringify(trangThai)
        });
    } catch (error) {
        console.error('Fetch account status API error:', error);
        return null;
    }
}

export async function fetchAccountMe(): Promise<AccountDetail | null> {
    try {
        return await api<AccountDetail>(`${API_URL}/Account/me`, { method: 'GET' });
    } catch (error) {
        console.error('Fetch account me API error:', error);
        return null;
    }
}

export function logout() {
    localStorage.removeItem('token');
    localStorage.removeItem('user');
    window.location.href = '/login';
}