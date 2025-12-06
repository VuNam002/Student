"use client";

import { useState } from 'react';
import { useRouter } from 'next/navigation';
import { Input } from '@/components/ui/input';
import { Button } from '@/components/ui/button';
import { Card, CardContent, CardHeader, CardTitle, CardFooter } from '@/components/ui/card';
import { ArrowLeft } from 'lucide-react';
import { fetchPermissionCreate } from '@/app/lib/api';

export default function PermissionCreatePage() {
  const router = useRouter();
  const [formData, setFormData] = useState({
    PermissionName: '',
    Module: '',
    Description: '',
  });
  const [errors, setErrors] = useState({
    PermissionName: '',
    Module: '',
  });

  const validateForm = (): boolean => {
    let valid = true;
    const newErrors = { PermissionName: '', Module: '' };

    if (!formData.PermissionName.trim()) {
      newErrors.PermissionName = "Tên quyền là bắt buộc";
      valid = false;
    }
    if (!formData.Module.trim()) {
      newErrors.Module = "Tên module là bắt buộc";
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
      const result = await fetchPermissionCreate(formData);
      if (result) {
        alert("Tạo quyền thành công!");
        router.push('/admin/permission');
      } else {
        alert("Tạo quyền thất bại. Tên quyền có thể đã tồn tại.");
      }
    } catch (err) {
      console.error("Create error:", err);
      alert("Đã xảy ra lỗi khi tạo quyền.");
    }
  };

  return (
    <>
      <div className="text-2xl font-bold py-2 px-6">
        Tạo quyền mới
      </div>
      <div className="p-4 md:p-6">
        <Button variant="outline" onClick={() => router.push('/admin/permission')} className="mb-4">
          <ArrowLeft className="mr-2 h-4 w-4" /> Quay lại danh sách
        </Button>
        <Card>
          <CardHeader>
            <CardTitle>Thông tin quyền mới</CardTitle>
          </CardHeader>
          <CardContent className="space-y-6">
            <div className="grid grid-cols-1 md:grid-cols-2 gap-6">
              <div className="space-y-2">
                <label htmlFor="PermissionName" className="font-medium">
                  Tên quyền <span className="text-red-500">*</span>
                </label>
                <Input
                  id="PermissionName"
                  name="PermissionName"
                  value={formData.PermissionName}
                  onChange={handleChange}
                  className={errors.PermissionName ? 'border-red-500' : ''}
                />
                {errors.PermissionName && <p className="text-red-500 text-sm">{errors.PermissionName}</p>}
              </div>
              <div className="space-y-2">
                <label htmlFor="Module" className="font-medium">
                  Module <span className="text-red-500">*</span>
                </label>
                <Input
                  id="Module"
                  name="Module"
                  value={formData.Module}
                  onChange={handleChange}
                  className={errors.Module ? 'border-red-500' : ''}
                />
                {errors.Module && <p className="text-red-500 text-sm">{errors.Module}</p>}
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
              Tạo quyền
            </Button>
          </CardFooter>
        </Card>
      </div>
    </>
  );
}