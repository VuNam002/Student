"use client";

import { AuthProvider } from "./lib/auth";
import { ToastContainer } from "react-toastify";
import "react-toastify/dist/ReactToastify.css";


export function Providers({ children }: { children: React.ReactNode }) {
  return (
    <AuthProvider>
        {children}
        <ToastContainer />
    </AuthProvider>
  );
}
