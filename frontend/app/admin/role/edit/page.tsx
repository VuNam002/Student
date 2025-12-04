"use client";

import { useEffect, useState, Suspense } from 'react';
import { useRouter, useSearchParams } from 'next/navigation';
import { fetchDetailRole, fetchRoleEdit } from '@/app/lib/api';
import { RoleDto } from '@/app/lib/types';
import { Input } from '@/components/ui/input';
import { Button } from '@/components/ui/button';
import { Card, CardContent, CardHeader, CardTitle, CardFooter } from '@/components/ui/card';
import { ArrowLeft } from 'lucide-react';
import { Textarea } from '@/components/ui/textarea';

interface RoleFormData {
  RoleName: string;
  RoleCode: string;
  Description: string;
}

function EditRoleForm() {
  const router = useRouter();
  const searchParams = useSearchParams();
  const roleId = searchParams.get('id');

  const [formData, setFormData] = useState<Partial<RoleFormData>>({});
  const [isLoading, setIsLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    if (!roleId) {
      setError("Không tìm thấy ID vai trò.");
      setIsLoading(false);
      return;
    }

    const loadRoleData = async () => {
      try {
        setIsLoading(true);
        const data: RoleDto = await fetchDetailRole(Number(roleId));
        if (data) {
          setFormData({
            RoleName: data.RoleName,
            RoleCode: data.RoleCode,
            Description: data.Description,
          });
        } else {
          setError("Không thể tải dữ liệu vai trò.");
        }
      } catch (err) {
        setError("Đã xảy ra lỗi khi tải dữ liệu.");
        console.error(err);
      } finally {
        setIsLoading(false);
      }
    };

    loadRoleData();
  }, [roleId]);

  const handleChange = (e: React.ChangeEvent<HTMLInputElement | HTMLTextAreaElement>) => {
    const { name, value } = e.target;
    setFormData(prev => ({
      ...prev,
      [name]: value
    }));
  };

  const handleSave = async (e: React.FormEvent) => {
    e.preventDefault();
    if (!roleId) return;

    try {
      const result = await fetchRoleEdit(Number(roleId), formData);
      if (result) {
        alert("Cập nhật vai trò thành công!");
        router.push('/admin/role');
      } else {
        alert("Cập nhật vai trò thất bại.");
      }
    } catch (err) {
      console.error("Save error:", err);
      alert("Đã xảy ra lỗi khi lưu.");
    }
  };

  if (isLoading) {
    return <div className="flex justify-center items-center h-64"><div className="animate-spin rounded-full h-12 w-12 border-b-2 border-gray-900"></div></div>;
  }

  if (error) {
    return <div className="text-center text-red-500 py-10">{error}</div>;
  }

  return (
    <>
      <div className="text-2xl font-bold py-2 px-6">
        Cập nhật vai trò
      </div>
      <div className="p-4 md:p-6">
        <Button variant="outline" onClick={() => router.push('/admin/role')} className="mb-4">
          <ArrowLeft className="mr-2 h-4 w-4" /> Quay lại danh sách
        </Button>
        <Card>
          <CardHeader>
            <CardTitle>Chỉnh sửa thông tin vai trò</CardTitle>
          </CardHeader>
          <form onSubmit={handleSave}>
            <CardContent className="space-y-6">
              <div className="grid grid-cols-1 md:grid-cols-2 gap-6">
                <div className="space-y-2">
                  <label htmlFor="RoleName" className="font-medium">Tên vai trò</label>
                  <Input
                    id="RoleName"
                    name="RoleName"
                    value={formData.RoleName || ''}
                    onChange={handleChange}
                  />
                </div>
                <div className="space-y-2">
                  <label htmlFor="RoleCode" className="font-medium">Mã vai trò</label>
                  <Input
                    id="RoleCode"
                    name="RoleCode"
                    value={formData.RoleCode || ''}
                    onChange={handleChange}
                    className="font-mono"
                  />
                </div>
              </div>

              <div className="space-y-2">
                <label htmlFor="Description" className="font-medium">Mô tả</label>
                <Textarea
                  id="Description"
                  name="Description"
                  value={formData.Description || ''}
                  onChange={handleChange}
                  rows={4}
                />
              </div>

            </CardContent>
            <CardFooter className="flex justify-end gap-2">
              <Button type="button" variant="outline" onClick={() => router.back()}>Hủy</Button>
              <Button type="submit">Lưu thay đổi</Button>
            </CardFooter>
          </form>
        </Card>
      </div>
    </>
  );
}

export default function EditRolePage() {
    return (
        <Suspense fallback={<div className="flex justify-center items-center h-screen"><div className="animate-spin rounded-full h-12 w-12 border-b-2 border-gray-900"></div></div>}>
            <EditRoleForm />
        </Suspense>
    );
}
