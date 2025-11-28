"use client";

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
import { fetchlogin } from "../app/lib/api";
import { useRouter } from "next/navigation";

export function LoginForm({
  className,
  ...props
}: React.ComponentProps<"div">) {
  const router = useRouter();
  const [email, setEmail] = useState("");
  const [password, setPassword] = useState("");
  const [error, setError] = useState<string | null>(null);
  const [success, setSuccess] = useState<string | null>(null);

  const handleSubmit = async (e: React.FormEvent<HTMLFormElement>) => {
    e.preventDefault();
    setError(null);
    setSuccess(null);
    try {
      const data = await fetchlogin(email, password);
      if (data.token) {
        localStorage.setItem("token", data.token);
        setSuccess("Đăng nhập thành công!");
        setTimeout(() => router.push("/"), 1000);
      } else {
        setError(data.message || "Login failed. Please try again.");
      }
    } catch {
      setError("An unexpected error occurred.");
    }
  };

  return (
    <div className="flex min-h-screen flex-col items-center justify-center p-2">
      <div className={cn("w-full max-w-md", className)} {...props}>
        <Card className="w-full">
          <CardContent className="p-8">
            <form onSubmit={handleSubmit}>
              <FieldGroup>
                <div className="flex flex-col items-center gap-2 text-center mb-6">
                  <h1 className="text-2xl font-bold">Đăng nhập</h1>
                  <p className="text-sm text-muted-foreground">
                    Quản lý sinh viên
                  </p>
                  {success && (
                    <p className="text-sm text-green-500 mt-2">{success}</p>
                  )}
                  {error && <p className="text-sm text-red-500 mt-2">{error}</p>}
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