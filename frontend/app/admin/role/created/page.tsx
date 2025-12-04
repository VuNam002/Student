"use client";

import { useState } from 'react';
import { useRouter } from 'next/navigation';
import { Input } from '@/components/ui/input';
import { Button } from '@/components/ui/button';
import { Card, CardContent, CardHeader, CardTitle, CardFooter } from '@/components/ui/card';
import { ArrowLeft } from 'lucide-react';
import { fetchRoleCreate } from '@/app/lib/api';

export default function AdminRoleCreatePage() {
  const router = useRouter();
  const [formData, setFormData] = useState({
    RoleName: '',
    RoleCode: '',
    Description: '',
  });
  const [errors, setErrors] = useState({
    RoleName: '',
    RoleCode: '',
  });

  const validateForm = (): boolean => {
    let valid = true;
    const newErrors = { RoleName: '', RoleCode: '' };

    if (!formData.RoleName.trim()) {
      newErrors.RoleName = "Tên vai trò là bắt buộc";
      valid = false;
    }
    if (!formData.RoleCode.trim()) {
      newErrors.RoleCode = "Mã vai trò là bắt buộc";
      valid = false;
    }

    setErrors(newErrors);
    return valid;
  };

  const handleChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    const { name, value } = e.target;
    setFormData(prev => ({ ...prev, [name]: value }));
    if (errors[name as keyof typeof errors]) {
      setErrors(prev => ({ ...prev, [name]: '' }));
    }
  };

  const handleSubmit = async () => {
    if (!validateForm()) {
      return;
    }

    try {
      const result = await fetchRoleCreate(formData);
      if (result) {
        alert("Tạo vai trò thành công!");
        router.push('/admin/role');
      } else {
        alert("Tạo vai trò thất bại. Mã vai trò có thể đã tồn tại.");
      }
    } catch (err) {
      console.error("Create error:", err);
      alert("Đã xảy ra lỗi khi tạo vai trò.");
    }
  };

  return (
    <>
      <div className="text-2xl font-bold py-2 px-6">
        Tạo vai trò mới
      </div>
      <div className="p-4 md:p-6">
        <Button variant="outline" onClick={() => router.push('/admin/role')} className="mb-4">
          <ArrowLeft className="mr-2 h-4 w-4" /> Quay lại danh sách
        </Button>
        <Card>
          <CardHeader>
            <CardTitle>Thông tin vai trò mới</CardTitle>
          </CardHeader>
          <CardContent className="space-y-6">
            <div className="grid grid-cols-1 md:grid-cols-2 gap-6">
              <div className="space-y-2">
                <label htmlFor="RoleName" className="font-medium">
                  Tên vai trò <span className="text-red-500">*</span>
                </label>
                <Input
                  id="RoleName"
                  name="RoleName"
                  value={formData.RoleName}
                  onChange={handleChange}
                  className={errors.RoleName ? 'border-red-500' : ''}
                />
                {errors.RoleName && <p className="text-red-500 text-sm">{errors.RoleName}</p>}
              </div>
              <div className="space-y-2">
                <label htmlFor="RoleCode" className="font-medium">
                  Mã vai trò <span className="text-red-500">*</span>
                </label>
                <Input
                  id="RoleCode"
                  name="RoleCode"
                  value={formData.RoleCode}
                  onChange={handleChange}
                  className={errors.RoleCode ? 'border-red-500' : ''}
                />
                {errors.RoleCode && <p className="text-red-500 text-sm">{errors.RoleCode}</p>}
              </div>
            </div>
            <div className="space-y-2">
              <label htmlFor="Description" className="font-medium">
                Mô tả
              </label>
              <Input
                id="Description"
                name="Description"
                value={formData.Description}
                onChange={handleChange}
              />
            </div>
          </CardContent>
          <CardFooter className="flex justify-end gap-2">
            <Button type="button" variant="outline" onClick={() => router.back()}>
              Hủy
            </Button>
            <Button type="button" onClick={handleSubmit}>
              Tạo vai trò
            </Button>
          </CardFooter>
        </Card>
      </div>
    </>
  );
}