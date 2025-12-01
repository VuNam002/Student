"use client";

import { useEffect, useState } from "react";
import { useParams, useRouter } from "next/navigation";
import { fetchAccountById } from "@/app/lib/api";
import { AccountDetail } from "@/app/lib/types";
import { Avatar, AvatarFallback, AvatarImage } from "@/components/ui/avatar";
import { Button } from "@/components/ui/button";
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
import { Skeleton } from "@/components/ui/skeleton";
import { Clock, CheckCircle, AlertCircle, Lock, Calendar, Ban } from "lucide-react";

const statusConfig = {
  0: { label: "Chờ kích hoạt", icon: Clock, color: "text-yellow-600", bg: "bg-yellow-50", border: "border-yellow-200" },
  1: { label: "Hoạt động", icon: CheckCircle, color: "text-emerald-600", bg: "bg-emerald-50", border: "border-emerald-200" },
  2: { label: "Tạm khóa", icon: AlertCircle, color: "text-orange-600", bg: "bg-orange-50", border: "border-orange-200" },
  3: { label: "Khóa vĩnh viễn", icon: Lock, color: "text-red-600", bg: "bg-red-50", border: "border-red-200" },
  4: { label: "Hết hạn", icon: Calendar, color: "text-gray-600", bg: "bg-gray-50", border: "border-gray-200" },
  5: { label: "Không hoạt động", icon: Ban, color: "text-slate-600", bg: "bg-slate-50", border: "border-slate-200" }
};

export default function AccountDetailPage() {
  const params = useParams();
  const router = useRouter();
  const id = params.id ? parseInt(params.id as string, 10) : null;

  const [account, setAccount] = useState<AccountDetail | null>(null);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    if (id) {
      const getAccount = async () => {
        try {
          setLoading(true);
          const data = await fetchAccountById(id);
          setAccount(data);
        } catch (error) {
          console.error("Failed to fetch account details", error);
          setAccount(null);
        } finally {
          setLoading(false);
        }
      };
      getAccount();
    }
  }, [id]);

  const getInitials = (name?: string | null) => {
    return name
      ? name
          .split(" ")
          .map((n) => n[0])
          .join("")
      : "AD";
  };

  const getStatusDisplay = (status: number) => {
    const config = statusConfig[status as keyof typeof statusConfig];
    if (!config) {
      return {
        label: "Không xác định",
        icon: AlertCircle,
        color: "text-gray-600",
        bg: "bg-gray-50",
        border: "border-gray-200"
      };
    }
    return config;
  };

  return (
    <div className="p-4 md:p-8">
      <Button onClick={() => router.back()} variant="outline" className="mb-4">
        &larr; Back to Accounts
      </Button>
      <Card>
        <CardHeader>
          <CardTitle>Account Details</CardTitle>
        </CardHeader>
        <CardContent>
          {loading ? (
            <div className="space-y-4">
              <div className="flex items-center space-x-4">
                <Skeleton className="h-24 w-24 rounded-full" />
                <div className="space-y-2">
                  <Skeleton className="h-6 w-[250px]" />
                  <Skeleton className="h-5 w-[200px]" />
                </div>
              </div>
              <div className="grid grid-cols-1 md:grid-cols-2 gap-4 pt-4">
                <div className="space-y-2">
                  <Skeleton className="h-4 w-[100px]" />
                  <Skeleton className="h-8 w-full" />
                </div>
                <div className="space-y-2">
                  <Skeleton className="h-4 w-[100px]" />
                  <Skeleton className="h-8 w-full" />
                </div>
                <div className="space-y-2">
                  <Skeleton className="h-4 w-[100px]" />
                  <Skeleton className="h-8 w-full" />
                </div>
                <div className="space-y-2">
                  <Skeleton className="h-4 w-[100px]" />
                  <Skeleton className="h-8 w-full" />
                </div>
              </div>
            </div>
          ) : account ? (
            <div>
              <div className="flex items-center space-x-4 mb-6">
                <Avatar className="h-24 w-24">
                  <AvatarImage src={account.Avatar || ""} alt={account.TenHienThi || "Avatar"} />
                  <AvatarFallback>{getInitials(account.TenHienThi)}</AvatarFallback>
                </Avatar>
                <div>
                  <h2 className="text-2xl font-bold">{account.HoTen}</h2>
                  <p className="text-muted-foreground">{account.Email}</p>
                </div>
              </div>
              <div className="grid grid-cols-1 md:grid-cols-2 gap-x-8 gap-y-4">
                <div className="flex flex-col">
                  <span className="text-sm font-medium text-muted-foreground">Chức vụ</span>
                  <p className="text-lg">{account.TenHienThi}</p>
                </div>
                <div className="flex flex-col">
                  <span className="text-sm font-medium text-muted-foreground">SDT</span>
                  <p className="text-lg">{account.SDT || 'N/A'}</p>
                </div>
                <div className="flex flex-col">
                  <span className="text-sm font-medium text-muted-foreground">Trạng thái</span>
                  <p className="text-lg">
                    {(() => {
                      const statusDisplay = getStatusDisplay(account.TrangThai);
                      const StatusIcon = statusDisplay.icon;
                      return (
                        <span
                          className={`inline-flex items-center gap-1.5 px-3 py-1.5 rounded-full text-sm font-medium ${statusDisplay.bg} ${statusDisplay.color} ${statusDisplay.border} border`}
                        >
                          <StatusIcon className="w-4 h-4" />
                          {statusDisplay.label}
                        </span>
                      );
                    })()}
                  </p>
                </div>
                <div className="flex flex-col">
                  <span className="text-sm font-medium text-muted-foreground">Date Created</span>
                  <p className="text-lg">
                    {account.NgayTao ? new Date(account.NgayTao).toLocaleDateString() : 'N/A'}
                  </p>
                </div>
              </div>
            </div>
          ) : (
            <div className="text-center py-10">
              <p className="text-muted-foreground">Account not found.</p>
            </div>
          )}
        </CardContent>
      </Card>
    </div>
  );
}