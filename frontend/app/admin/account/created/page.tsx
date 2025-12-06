"use client";

import { useState, useEffect } from 'react';
import { useRouter } from 'next/navigation';
import { Input } from '@/components/ui/input';
import { Button } from '@/components/ui/button';
import { Card, CardContent, CardHeader, CardTitle, CardFooter } from '@/components/ui/card';
import { ArrowLeft, UploadCloud } from 'lucide-react';
import { fetchAccountCreat, fetchRole } from '@/app/lib/api';
import { RoleDto } from '@/app/lib/types';

const statusConfig = {
  0: { label: "Chờ kích hoạt" },
  1: { label: "Hoạt động" },
  2: { label: "Tạm khóa" },
  3: { label: "Khóa vĩnh viễn" },
  4: { label: "Hết hạn" },
  5: { label: "Không hoạt động" }
};

interface FormData {
  Email: string;
  RoleID: string;
  Avatar: string;
  Status: number;
  FullName: string;
  PhoneNumber: string;
  Password: string;
}

export default function AdminAccountCreatePage() {
  const router = useRouter();
  const [roles, setRoles] = useState<RoleDto[]>([]);

  const [formData, setFormData] = useState<FormData>({
    Email: '',
    RoleID: '',
    Avatar: '',
    Status: 0,
    FullName: '',
    PhoneNumber: '',
    Password: '',
  });

  const [isUploading, setIsUploading] = useState(false);
  const [errors, setErrors] = useState<Partial<Record<keyof FormData, string>>>({});

  useEffect(() => {
    const getRoles = async () => {
      const rolesData = await fetchRole();
      if (rolesData) {
        setRoles(rolesData);
        if (rolesData.length > 0) {
          setFormData(prev => ({ ...prev, RoleID: rolesData[0].RoleID.toString() }));
        }
      }
    };
    getRoles();
  }, []);

  const handleChange = (e: React.ChangeEvent<HTMLInputElement | HTMLSelectElement>) => {
    const { name, value } = e.target;
    setFormData(prev => ({ ...prev, [name]: value }));
    if (errors[name as keyof FormData]) {
      setErrors(prev => ({ ...prev, [name]: '' }));
    }
  };

  const handleImageUpload = async (e: React.ChangeEvent<HTMLInputElement>) => {
    const file = e.target.files?.[0];
    if (!file) return;

    const CLOUDINARY_CLOUD_NAME = "duzubskpy";
    const CLOUDINARY_UPLOAD_PRESET = "students";

    setIsUploading(true);
    const apiFormData = new FormData();
    apiFormData.append('file', file);
    apiFormData.append('upload_preset', CLOUDINARY_UPLOAD_PRESET);

    try {
      const response = await fetch(`https://api.cloudinary.com/v1_1/${CLOUDINARY_CLOUD_NAME}/image/upload`, {
        method: 'POST',
        body: apiFormData,
      });

      if (response.ok) {
        const data = await response.json();
        setFormData(prev => ({ ...prev, Avatar: data.secure_url }));
        alert("Tải ảnh lên thành công!");
      } else {
        throw new Error('Tải ảnh lên thất bại.');
      }
    } catch (error) {
      console.error("Cloudinary upload error:", error);
      alert("Đã xảy ra lỗi khi tải ảnh lên.");
    } finally {
      setIsUploading(false);
    }
  };

  const validateForm = (): boolean => {
    const newErrors: Partial<Record<keyof FormData, string>> = {};

    if (!formData.FullName.trim()) {
      newErrors.FullName = "Họ và tên là bắt buộc";
    }

    if (!formData.Email.trim()) {
      newErrors.Email = "Email là bắt buộc";
    } else if (!/^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(formData.Email)) {
      newErrors.Email = "Email không hợp lệ";
    }

    if (!formData.Password) {
      newErrors.Password = "Mật khẩu là bắt buộc";
    } else if (formData.Password.length < 6) {
      newErrors.Password = "Mật khẩu phải có ít nhất 6 ký tự";
    }

    if (!formData.RoleID) {
        newErrors.RoleID = "Vai trò là bắt buộc";
    }

    setErrors(newErrors);
    return Object.keys(newErrors).length === 0;
  };

  const handleSubmit = async () => {
    if (!validateForm()) {
      return;
    }

    const payload = {
      Email: formData.Email,
      RoleID: parseInt(formData.RoleID),
      Avatar: formData.Avatar,
      Status: Number(formData.Status),
      FullName: formData.FullName,
      PhoneNumber: formData.PhoneNumber,
      Password: formData.Password,
    };

    try {
      // Replace with your actual API call
      const result = await fetchAccountCreat(payload);
      if (result) {
        alert("Tạo tài khoản thành công!");
        router.push('/admin/account');
      } else {
        alert("Tạo tài khoản thất bại.");
      }
    } catch (err) {
      console.error("Create error:", err);
      alert("Đã xảy ra lỗi khi tạo tài khoản.");
    }
  };

  return (
    <>
      <div className="text-2xl font-bold py-2 px-6">
        Tạo tài khoản mới
      </div>
      <div className="p-4 md:p-6">
        <Button variant="outline" onClick={() => router.push('/admin/account')} className="mb-4">
          <ArrowLeft className="mr-2 h-4 w-4" /> Quay lại danh sách
        </Button>
        <Card>
          <CardHeader>
            <CardTitle>Thông tin tài khoản mới</CardTitle>
          </CardHeader>
          <CardContent className="space-y-6">
            <div className="grid grid-cols-1 md:grid-cols-2 gap-6">
              <div className="space-y-2">
                <label htmlFor="FullName" className="font-medium">
                  Họ và Tên <span className="text-red-500">*</span>
                </label>
                <Input
                  id="FullName"
                  name="FullName"
                  value={formData.FullName}
                  onChange={handleChange}
                  className={errors.FullName ? 'border-red-500' : ''}
                />
                {errors.FullName && <p className="text-red-500 text-sm">{errors.FullName}</p>}
              </div>
              <div className="space-y-2">
                <label htmlFor="Email" className="font-medium">
                  Email <span className="text-red-500">*</span>
                </label>
                <Input
                  id="Email"
                  name="Email"
                  type="email"
                  value={formData.Email}
                  onChange={handleChange}
                  className={errors.Email ? 'border-red-500' : ''}
                  autoComplete="off"
                />
                {errors.Email && <p className="text-red-500 text-sm">{errors.Email}</p>}
              </div>
            </div>

            <div className="grid grid-cols-1 md:grid-cols-2 gap-6">
              
              <div className="space-y-2">
                <label htmlFor="PhoneNumber" className="font-medium">Số điện thoại</label>
                <Input
                  id="PhoneNumber"
                  name="PhoneNumber"
                  value={formData.PhoneNumber}
                  onChange={handleChange}
                  autoComplete="off"
                />
              </div>
               <div className="space-y-2">
                <label htmlFor="Password" className="font-medium">
                  Mật khẩu <span className="text-red-500">*</span>
                </label>
                <Input
                  id="Password"
                  name="Password"
                  type="password"
                  value={formData.Password}
                  onChange={handleChange}
                  className={errors.Password ? 'border-red-500' : ''}
                  placeholder="Nhập mật khẩu (tối thiểu 6 ký tự)"
                  autoComplete="new-password"
                />
                {errors.Password && <p className="text-red-500 text-sm">{errors.Password}</p>}
              </div>
            </div>

            <div className="grid grid-cols-1 md:grid-cols-2 gap-6">
              <div className="space-y-2">
                <label htmlFor="RoleID" className="font-medium">Vai trò</label>
                <select
                  id="RoleID"
                  name="RoleID"
                  value={formData.RoleID}
                  onChange={handleChange}
                  className="w-full p-2 border border-gray-300 rounded-md"
                >
                  <option value="" disabled>Chọn vai trò</option>
                  {roles.map(role => (
                    <option key={role.RoleID} value={role.RoleID}>{role.RoleName}</option>
                  ))}
                </select>
                 {errors.RoleID && <p className="text-red-500 text-sm">{errors.RoleID}</p>}
              </div>
              <div className="space-y-2">
                <label htmlFor="Status" className="font-medium">Trạng thái</label>
                <select
                  id="Status"
                  name="Status"
                  value={formData.Status}
                  onChange={handleChange}
                  className="w-full p-2 border border-gray-300 rounded-md"
                >
                  {Object.entries(statusConfig).map(([key, { label }]) => (
                    <option key={key} value={key}>{label}</option>
                  ))}
                </select>
              </div>
            </div>

            <div className="space-y-2">
              <label className="font-medium">Avatar</label>
              <div className="flex items-center gap-4">
                {formData.Avatar && (
                  <img
                    src={formData.Avatar}
                    alt="Avatar preview"
                    className="w-16 h-16 rounded-full object-cover"
                  />
                )}
                <Input
                  id="avatar-upload"
                  type="file"
                  accept="image/*"
                  onChange={handleImageUpload}
                  className="hidden"
                />
                <Button type="button" variant="outline" asChild disabled={isUploading}>
                  <label htmlFor="avatar-upload" className="cursor-pointer">
                    <UploadCloud className="mr-2 h-4 w-4" />
                    Chọn ảnh
                  </label>
                </Button>
                {isUploading && <div className="text-sm text-gray-500">Đang tải lên...</div>}
              </div>
            </div>
          </CardContent>
          <CardFooter className="flex justify-end gap-2">
            <Button type="button" variant="outline" onClick={() => router.back()}>
              Hủy
            </Button>
            <Button type="button" onClick={handleSubmit}>
              Tạo tài khoản
            </Button>
          </CardFooter>
        </Card>
      </div>
    </>
  );
}