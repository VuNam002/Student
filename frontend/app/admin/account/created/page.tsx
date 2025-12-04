"use client";

import { useState } from 'react';
import { useRouter } from 'next/navigation';
import { Input } from '@/components/ui/input';
import { Button } from '@/components/ui/button';
import { Card, CardContent, CardHeader, CardTitle, CardFooter } from '@/components/ui/card';
import { ArrowLeft, UploadCloud } from 'lucide-react';
import { fetchAccountCreat } from '@/app/lib/api';

const statusConfig = {
  0: { label: "Chờ kích hoạt" },
  1: { label: "Hoạt động" },
  2: { label: "Tạm khóa" },
  3: { label: "Khóa vĩnh viễn" },
  4: { label: "Hết hạn" },
  5: { label: "Không hoạt động" }
};

interface FormData {
  email: string;
  roleID: string;
  avatar: string;
  trangThai: number;
  tenHienThi: string;
  hoTen: string;
  sdt: string;
  matKhau: string;
}

export default function AdminAccountCreatePage() {
  const router = useRouter();

  const [formData, setFormData] = useState<FormData>({
    email: '',
    roleID: '',
    avatar: '',
    trangThai: 0,
    tenHienThi: '',
    hoTen: '',
    sdt: '',
    matKhau: '',
  });

  const [isUploading, setIsUploading] = useState(false);
  const [errors, setErrors] = useState<Partial<Record<keyof FormData, string>>>({});

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
        setFormData(prev => ({ ...prev, avatar: data.secure_url }));
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

    if (!formData.hoTen.trim()) {
      newErrors.hoTen = "Họ và tên là bắt buộc";
    }

    if (!formData.email.trim()) {
      newErrors.email = "Email là bắt buộc";
    } else if (!/^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(formData.email)) {
      newErrors.email = "Email không hợp lệ";
    }

    if (!formData.matKhau) {
      newErrors.matKhau = "Mật khẩu là bắt buộc";
    } else if (formData.matKhau.length < 6) {
      newErrors.matKhau = "Mật khẩu phải có ít nhất 6 ký tự";
    }

    if (!formData.tenHienThi.trim()) {
      newErrors.tenHienThi = "Tên hiển thị là bắt buộc";
    }

    setErrors(newErrors);
    return Object.keys(newErrors).length === 0;
  };

  const handleSubmit = async () => {
    if (!validateForm()) {
      return;
    }

    const payload = {
      Email: formData.email,
      RoleID: formData.roleID,
      Avatar: formData.avatar,
      Status: Number(formData.trangThai),
      RoleName: formData.tenHienThi,
      FullName: formData.hoTen,
      PhoneNumber: formData.sdt,
      Password: formData.matKhau,
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
                <label htmlFor="hoTen" className="font-medium">
                  Họ và Tên <span className="text-red-500">*</span>
                </label>
                <Input
                  id="hoTen"
                  name="hoTen"
                  value={formData.hoTen}
                  onChange={handleChange}
                  className={errors.hoTen ? 'border-red-500' : ''}
                />
                {errors.hoTen && <p className="text-red-500 text-sm">{errors.hoTen}</p>}
              </div>
              <div className="space-y-2">
                <label htmlFor="tenHienThi" className="font-medium">
                  Tên hiển thị <span className="text-red-500">*</span>
                </label>
                <Input
                  id="tenHienThi"
                  name="tenHienThi"
                  value={formData.tenHienThi}
                  onChange={handleChange}
                  className={errors.tenHienThi ? 'border-red-500' : ''}
                />
                {errors.tenHienThi && <p className="text-red-500 text-sm">{errors.tenHienThi}</p>}
              </div>
            </div>

            <div className="grid grid-cols-1 md:grid-cols-2 gap-6">
              <div className="space-y-2">
                <label htmlFor="email" className="font-medium">
                  Email <span className="text-red-500">*</span>
                </label>
                <Input
                  id="email"
                  name="email"
                  type="email"
                  value={formData.email}
                  onChange={handleChange}
                  className={errors.email ? 'border-red-500' : ''}
                  autoComplete="off"
                />
                {errors.email && <p className="text-red-500 text-sm">{errors.email}</p>}
              </div>
              <div className="space-y-2">
                <label htmlFor="sdt" className="font-medium">Số điện thoại</label>
                <Input
                  id="sdt"
                  name="sdt"
                  value={formData.sdt}
                  onChange={handleChange}
                  autoComplete="off"
                />
              </div>
            </div>

            <div className="space-y-2 w-1/2">
              <label htmlFor="matKhau" className="font-medium">
                Mật khẩu <span className="text-red-500">*</span>
              </label>
              <Input
                id="matKhau"
                name="matKhau"
                type="password"
                value={formData.matKhau}
                onChange={handleChange}
                className={errors.matKhau ? 'border-red-500' : ''}
                placeholder="Nhập mật khẩu (tối thiểu 6 ký tự)"
                autoComplete="new-password"
              />
              {errors.matKhau && <p className="text-red-500 text-sm">{errors.matKhau}</p>}
            </div>

            <div className="grid grid-cols-1 md:grid-cols-2 gap-6">
              <div className="space-y-2">
                <label htmlFor="roleID" className="font-medium">Vai trò</label>
                <Input
                  id="roleID"
                  name="roleID"
                  value={formData.roleID}
                  onChange={handleChange}
                  placeholder="Nhập ID vai trò"
                />
              </div>
              <div className="space-y-2">
                <label htmlFor="trangThai" className="font-medium">Trạng thái</label>
                <select
                  id="trangThai"
                  name="trangThai"
                  value={formData.trangThai}
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
                {formData.avatar && (
                  <img
                    src={formData.avatar}
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