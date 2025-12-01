"use client";

import {
  createContext,
  useContext,
  useState,
  useEffect,
  ReactNode,
} from "react";
import { useRouter, usePathname } from "next/navigation";
import { AccountDetail } from "./types";
import { fetchUserFromToken } from "./api";

interface AuthContextType {
  user: AccountDetail | null;
  login: (userData: AccountDetail, token: string) => void;
  logout: () => void;
  isAuthenticated: boolean;
  isLoading: boolean;
}

const AuthContext = createContext<AuthContextType | undefined>(undefined);

export function AuthProvider({ children }: { children: ReactNode }) {
  const [user, setUser] = useState<AccountDetail | null>(null);
  const [isLoading, setIsLoading] = useState(true);
  const router = useRouter();
  const pathname = usePathname();

  useEffect(() => {
    (async () => {
      try {
        const token = localStorage.getItem("token");
        const userData = localStorage.getItem("user");
        if (token && userData) {
          setUser(JSON.parse(userData));
          setIsLoading(false);
          return;
        }

        if (token && !userData) {
          try {
            const fetched = await fetchUserFromToken();
            if (fetched) {
              localStorage.setItem('user', JSON.stringify(fetched));
              setUser(fetched);
            }
          } catch (e) {
            console.warn('Could not load user from token', e);
          }
        }
      } catch (error) {
        console.error("Failed to parse user data from localStorage", error);
      } finally {
        setIsLoading(false);
      }
    })();
  }, []);

  useEffect(() => {
    if (isLoading) return; 
    if (!user && pathname !== "/login") {
      router.push("/login");
    }

    if (user && (pathname === "/login" || pathname === "/")) {
      router.push("/admin/dashboard");
    }
  }, [isLoading, user, pathname, router]);

  const login = (userData: AccountDetail, token: string) => {
    localStorage.setItem("token", token);
    localStorage.setItem("user", JSON.stringify(userData));
    setUser(userData);
    router.push("/dashboard");
  };

  const logout = () => {
    localStorage.removeItem("token");
    localStorage.removeItem("user");
    setUser(null);
    router.push("/login");
  };

  const isAuthenticated = !!user;

  return (
    <AuthContext.Provider
      value={{ user, login, logout, isAuthenticated, isLoading }}
    >
      {isLoading ? <div>Loading...</div> : children}
    </AuthContext.Provider>
  );
}

export function useAuth() {
  const context = useContext(AuthContext);
  if (context === undefined) {
    throw new Error("useAuth must be used within an AuthProvider");
  }
  return context;
}
