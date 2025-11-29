import { LoginResponse, AccountDetail } from "./types";

const API_URL = 'http://localhost:5262/api';

export async function fetchlogin(Email: string, MatKhau: string): Promise<LoginResponse> {
    try {
        const res = await fetch(`${API_URL}/Account/login`, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
            },
            body: JSON.stringify({ Email, MatKhau }),
        });

        if (res.ok) {
            const token = await res.text();
            if (token) {
                localStorage.setItem('token', token);
                return { token: token };
            } 
            return { message: 'Received an empty token.' };
        }
        const errorMessage = await res.text();
        return { message: errorMessage || 'Invalid credentials' };
    } catch (error) {
        console.error('Login API error:', error);
        return { message: 'An unexpected error occurred.' };
    }
}


export async function fetchUserFromToken(): Promise<AccountDetail | null> {
    try {
        const token = localStorage.getItem('token');
        if (!token) return null;

        const parts = token.split('.');
        if (parts.length < 2) return null;
        const payload = parts[1].replace(/-/g, '+').replace(/_/g, '/');
        const jsonPayload = decodeURIComponent(atob(payload).split('').map(function(c) {
            return '%' + ('00' + c.charCodeAt(0).toString(16)).slice(-2);
        }).join(''));

        const parsed = JSON.parse(jsonPayload);

        const user: AccountDetail = {
            ID: parsed.ID ?? parsed.id ?? parsed.sub ?? 0,
            Email: parsed.Email ?? parsed.email ?? '',
            RoleID: parsed.RoleID ?? parsed.role ?? parsed.roleId,
            Avatar: parsed.Avatar ?? parsed.avatar ?? null,
            TrangThai: parsed.TrangThai ?? parsed.trangThai ?? undefined,
            TenHienThi: parsed.TenHienThi ?? parsed.name ?? null,
            NgayTao: parsed.NgayTao ?? parsed.iat ?? null,
        };

        return user;
    } catch (error) {
        console.warn('Failed to decode token for user info', error);
        return null;
    }
}

export async function fetchAccount(): Promise<AccountDetail | null> {
    try {
        const res = await fetch(`${API_URL}/Account`, {
            method: 'GET',
            headers: {
                'Content-Type': 'application/json',
                'Authorization': `Bearer ${localStorage.getItem('token')}`
            },
        })
        if (res.ok) {
            const data = await res.json();
            return data;
        } else {
            console.error('Fetch account API error:', res.statusText);
            return null;
        }
    } catch (error) {
        console.error('Fetch account API error:', error);
        return null;
    }
}

export async function fetchAccountById(id: number): Promise<any|null >{
    try {
        const res = await fetch(`${API_URL}/Account/${id}`, {
            method: 'GET',
            headers: {
                'Content-Type': 'application/json',
                'Authorization': `Bearer ${localStorage.getItem('token')}`
            },
        });
        if(!res.ok) {
            return null;
        }
        const data = await res.json();
        return data;
    } catch (error) {
        console.error('Fetch account by ID API error:', error);
        return null;
    }
}

export async function fetchAccountEdit(id:number, updateAccount: any) {
    try {
        const res = await fetch(`${API_URL}/Account/${id}`, {
            method: 'PATCh',
            headers: {
                'Content-Type': 'application/json',
                'Authorization': `Bearer ${localStorage.getItem('token')}`
            },
            body: JSON.stringify(updateAccount)
        });
        if (!res.ok) {
            return null;
        }
        const data = await res.json();
        return data;
    } catch (error) {
        console.error('Fetch account edit API error:', error);
        return null;
    }
}

export async function fetchAccountCreat(newAccount: any) {
    try {
        const res = await fetch(`${API_URL}/Account`, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
                'Authorization': `Bearer ${localStorage.getItem('token')}`
            },
            body: JSON.stringify(newAccount)
        });
        if (!res.ok) {
            return null;
        }
        const data = await res.json();
        return data
    } catch (error) {
        console.error('Fetch account creat API error:', error);
        return null;
    }
}

export async function fetchAccountDeleted(id: number): Promise<boolean | null> {
    try {
        const res = await fetch(`${API_URL}/Account/${id}`, {
            method: 'DELETE',
            headers: {
                'Content-Type': 'application/json',
                'Authorization': `Bearer ${localStorage.getItem('token')}`
            },
        });

        if (!res.ok) {
            return null;
        }
        return true;
    } catch (error) {
        console.error('Delete item API error:', error);
        return null;
    }
}

export function logout() {
    localStorage.removeItem('token');
}