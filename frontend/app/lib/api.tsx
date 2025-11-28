import { LoginResponse } from "./types";

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
