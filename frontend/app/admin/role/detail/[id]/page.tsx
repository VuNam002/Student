"use client";

import { useEffect, useState, useMemo } from "react";
import { useParams, useRouter } from "next/navigation";
import { fetchDetailRole, fetchAllPermissions, assignPermissionsToRole } from "@/app/lib/api";
import { RoleDto, PermissionGroupDto, PermissionItemDto } from "@/app/lib/types";
import { Button } from "@/components/ui/button";
import { Card, CardContent, CardHeader, CardTitle, CardFooter } from "@/components/ui/card";
import { Skeleton } from "@/components/ui/skeleton";
import { Checkbox } from "@/components/ui/checkbox";
import { Collapsible, CollapsibleContent, CollapsibleTrigger } from "@/components/ui/collapsible";
import { FileText, Calendar, Code, Shield, ChevronDown } from "lucide-react";


export default function RoleDetailPage() {
  const params = useParams();
  const router = useRouter();
  const id = params.id ? parseInt(params.id as string, 10) : null;

  const [role, setRole] = useState<RoleDto | null>(null);
  const [allPermissions, setAllPermissions] = useState<PermissionGroupDto[]>([]);
  const [selectedPermissions, setSelectedPermissions] = useState<number[]>([]);
  const [loading, setLoading] = useState(true);
  const [isSaving, setIsSaving] = useState(false);

  useEffect(() => {
    if (id) {
      const loadData = async () => {
        try {
          setLoading(true);
          const [roleData, permissionsData] = await Promise.all([
            fetchDetailRole(id),
            fetchAllPermissions()
          ]);
          
          if (roleData) {
            setRole(roleData);
            // Assuming roleData.permissions is an array of permission IDs
            setSelectedPermissions(roleData.permissions || []);
          }

          if (permissionsData) {
            setAllPermissions(permissionsData);
          }
          
        } catch (error) {
          console.error("Failed to fetch data", error);
        } finally {
          setLoading(false);
        }
      };
      loadData();
    }
  }, [id]);

  const handlePermissionChange = (permissionId: number, checked: boolean) => {
    setSelectedPermissions(prev => 
      checked ? [...prev, permissionId] : prev.filter(id => id !== permissionId)
    );
  };

  const handleSelectAllModule = (modulePermissions: PermissionItemDto[], checked: boolean) => {
    const modulePermissionIds = modulePermissions.map(p => p.PermissionID);
    if (checked) {
      setSelectedPermissions(prev => [...new Set([...prev, ...modulePermissionIds])]);
    } else {
      setSelectedPermissions(prev => prev.filter(id => !modulePermissionIds.includes(id)));
    }
  };

  const handleSave = async () => {
    if (!id) return;
    setIsSaving(true);
    try {
      await assignPermissionsToRole(id, selectedPermissions);
      alert("Cập nhật quyền thành công!");
      router.refresh();
    } catch (error) {
      console.error("Failed to save permissions", error);
      alert("Đã xảy ra lỗi khi cập nhật quyền.");
    } finally {
      setIsSaving(false);
    }
  };

  const renderInfoCard = () => (
    <Card>
      <CardHeader>
        <CardTitle>Thông tin vai trò</CardTitle>
      </CardHeader>
      <CardContent>
        {role ? (
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
  );

  const renderPermissionsCard = () => {
    if (allPermissions.length === 0) return null;

    return (
      <Card>
        <CardHeader>
          <CardTitle className="flex items-center">
            <Shield className="mr-2" />
            Phân Quyền cho Vai trò
          </CardTitle>
        </CardHeader>
        <CardContent className="space-y-4">
          {allPermissions.map(moduleGroup => {
            const modulePermissions = moduleGroup.Permissions || [];
            const allInModuleSelected = modulePermissions.every(p => selectedPermissions.includes(p.PermissionID));
            const someInModuleSelected = modulePermissions.some(p => selectedPermissions.includes(p.PermissionID));

            return (
              <Collapsible key={moduleGroup.Module} className="border rounded-lg p-4">
                <CollapsibleTrigger className="w-full">
                    <div className="flex items-center justify-between">
                        <div className="flex items-center gap-2">
                            <h3 className="text-xl font-semibold capitalize">{moduleGroup.Module.toLowerCase().replace('_', ' ')}</h3>
                            <ChevronDown className="h-5 w-5 transition-transform [&[data-state=open]]:rotate-180" />
                        </div>
                        <div className="flex items-center space-x-2" onClick={(e) => e.stopPropagation()}>
                            <Checkbox
                                id={`select-all-${moduleGroup.Module}`}
                                checked={allInModuleSelected ? true : someInModuleSelected ? 'indeterminate' : false}
                                onCheckedChange={(checked) => handleSelectAllModule(modulePermissions, !!checked)}
                            />
                            <label htmlFor={`select-all-${moduleGroup.Module}`} className="font-medium">
                                Chọn tất cả
                            </label>
                        </div>
                    </div>
                </CollapsibleTrigger>
                <CollapsibleContent>
                    <div className="grid grid-cols-1 sm:grid-cols-2 md:grid-cols-3 lg:grid-cols-4 gap-4 mt-4 pt-4 border-t">
                    {modulePermissions.map(permission => (
                        <div key={permission.PermissionID} className="flex items-center space-x-2">
                        <Checkbox 
                            id={`perm-${permission.PermissionID}`}
                            checked={selectedPermissions.includes(permission.PermissionID)}
                            onCheckedChange={(checked) => handlePermissionChange(permission.PermissionID, !!checked)}
                        />
                        <label htmlFor={`perm-${permission.PermissionID}`} className="text-sm font-normal cursor-pointer">
                            {permission.PermissionName}
                        </label>
                        </div>
                    ))}
                    </div>
                </CollapsibleContent>
              </Collapsible>
            );
          })}
        </CardContent>
        <CardFooter className="flex justify-end">
          <Button onClick={handleSave} disabled={isSaving}>
            {isSaving ? "Đang lưu..." : "Lưu thay đổi"}
          </Button>
        </CardFooter>
      </Card>
    );
  };

  if (loading) {
    return <div className="p-8"><Skeleton className="h-96 w-full" /></div>;
  }

  return (
    <div className="p-4 md:p-8 -mt-6 space-y-6">
      <div className="flex items-center justify-between">
        <h1 className="text-3xl font-bold">Chi tiết vai trò</h1>
        <Button onClick={() => router.back()} variant="outline">
          &larr; Quay lại danh sách
        </Button>
      </div>
      {renderInfoCard()}
      {renderPermissionsCard()}
    </div>
  );
}