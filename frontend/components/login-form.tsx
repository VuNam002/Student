"use client";

import { ToastContainer, toast } from 'react-toastify';
import 'react-toastify/dist/ReactToastify.css';
import { cn } from "../app/lib/utils";
import { useState } from "react";
import { Button } from "@/components/ui/button";
import { Card, CardContent } from "@/components/ui/card";
import {
  Field,
  FieldGroup,
  FieldLabel,
} from "../components/ui/field";
import { Input } from "@/components/ui/input";
import { fetchlogin } from "@/app/lib/api";
import { useRouter } from "next/navigation";

export function LoginForm({
  className,
  ...props
}: React.ComponentProps<"div">) {
  const router = useRouter();
  const [email, setEmail] = useState("");
  const [password, setPassword] = useState("");

  const handleSubmit = async (e: React.FormEvent<HTMLFormElement>) => {
    e.preventDefault();
    try {
      const data = await fetchlogin(email, password);
      if (data.token) {
        localStorage.setItem("token", data.token);
        toast.success("Đăng nhập thành công!");
        setTimeout(() => router.push("/dashboard"), 1000);
      } else {
        toast.error(data.message || "Login failed. Please try again.");
      }
    } catch {
      toast.error("An unexpected error occurred.");
    }
  };

  return (
    <div className="flex w-full h-full flex-col items-center justify-center ">
      <div className={cn("w-full max-w-md", className)} {...props}>
        <Card className="w-full">
          <ToastContainer />
          <CardContent className="p-8">
            <form onSubmit={handleSubmit}>
              <FieldGroup>
                <div className="flex flex-col items-center gap-2 text-center mb-6">
                  <h1 className="text-2xl font-bold">Đăng nhập</h1>
                  <p className="text-sm text-muted-foreground">
                    Quản lý sinh viên
                  </p>
                </div>

                <Field>
                  <FieldLabel htmlFor="email">Email</FieldLabel>
                  <Input
                    id="email"
                    type="email"
                    placeholder="m@example.com"
                    required
                    className="mb-"
                    value={email}
                    onChange={(e) => setEmail(e.target.value)}
                  />
                </Field>

                <Field>
                  <FieldLabel htmlFor="password">Password</FieldLabel>
                  <Input
                    id="password"
                    type="password"
                    required
                    value={password}
                    onChange={(e) => setPassword(e.target.value)}
                  />
                </Field>

                <Field>
                  <Button type="submit" className="w-full mt-6">
                    Login
                  </Button>
                </Field>
              </FieldGroup>
            </form>
          </CardContent>
        </Card>
      </div>
    </div>
  );
}