"use client";

import { useEffect, useState } from "react";
import { useParams, useRouter } from "next/navigation";
import { fetchDetailRole } from "@/app/lib/api";
import { RoleDto } from "@/app/lib/types";
import { Button } from "@/components/ui/button";
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
import { Skeleton } from "@/components/ui/skeleton";
import { FileText, Calendar, Code } from "lucide-react";

export default function RoleDetailPage() {
  const params = useParams();
  const router = useRouter();
  const id = params.id ? parseInt(params.id as string, 10) : null;

  const [role, setRole] = useState<RoleDto | null>(null);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    if (id) {
      const getRole = async () => {
        try {
          setLoading(true);
          const data = await fetchDetailRole(id);
          setRole(data);
        } catch (error) {
          console.error("Failed to fetch role details", error);
          setRole(null);
        } finally {
          setLoading(false);
        }
      };
      getRole();
    }
  }, [id]);

  return (
    <div className="p-4 md:p-8 -mt-6">
      <div className="flex items-center justify-between mb-4">
        <h1 className="text-3xl font-bold">Chi tiết vai trò</h1>
        <Button onClick={() => router.back()} variant="outline">
          &larr; Quay lại danh sách
        </Button>
      </div>
      <Card>
        <CardHeader>
          <CardTitle>Thông tin vai trò</CardTitle>
        </CardHeader>
        <CardContent>
          {loading ? (
            <div className="space-y-6">
              <div className="space-y-2">
                <Skeleton className="h-4 w-1/4" />
                <Skeleton className="h-8 w-1/2" />
              </div>
              <div className="space-y-2">
                <Skeleton className="h-4 w-1/4" />
                <Skeleton className="h-8 w-1/3" />
              </div>
              <div className="space-y-2">
                <Skeleton className="h-4 w-1/4" />
                <Skeleton className="h-12 w-full" />
              </div>
              <div className="space-y-2">
                <Skeleton className="h-4 w-1/4" />
                <Skeleton className="h-8 w-1/2" />
              </div>
            </div>
          ) : role ? (
            <div className="grid grid-cols-1 md:grid-cols-2 gap-x-8 gap-y-6">
              <div className="flex items-center">
                <FileText className="w-6 h-6 mr-4 text-gray-500" />
                <div>
                  <span className="text-sm font-medium text-muted-foreground">Tên vai trò</span>
                  <p className="text-lg font-semibold">{role.RoleName}</p>
                </div>
              </div>
              <div className="flex items-center">
                <Code className="w-6 h-6 mr-4 text-gray-500" />
                <div>
                  <span className="text-sm font-medium text-muted-foreground">Mã vai trò</span>
                  <p className="text-lg font-mono bg-gray-100 px-2 py-1 rounded">{role.RoleCode}</p>
                </div>
              </div>
              <div className="flex items-start col-span-1 md:col-span-2">
                <FileText className="w-6 h-6 mr-4 text-gray-500 mt-1" />
                <div>
                  <span className="text-sm font-medium text-muted-foreground">Mô tả</span>
                  <p className="text-lg">{role.Description || 'Không có mô tả'}</p>
                </div>
              </div>
              <div className="flex items-center">
                <Calendar className="w-6 h-6 mr-4 text-gray-500" />
                <div>
                  <span className="text-sm font-medium text-muted-foreground">Ngày tạo</span>
                  <p className="text-lg">
                    {role.CreatedAt ? new Date(role.CreatedAt).toLocaleDateString() : 'N/A'}
                  </p>
                </div>
              </div>
            </div>
          ) : (
            <div className="text-center py-10">
              <p className="text-muted-foreground">Không tìm thấy vai trò.</p>
            </div>
          )}
        </CardContent>
      </Card>
    </div>
  );
}