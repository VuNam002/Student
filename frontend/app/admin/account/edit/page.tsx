"use client";

import { useEffect, useState, Suspense } from 'react';
import { useRouter, useSearchParams } from 'next/navigation';
import { fetchAccountById, fetchAccountEdit } from '../../../lib/api';
import { Input } from '@/components/ui/input';
import { Button } from '@/components/ui/button';
import { Card, CardContent, CardHeader, CardTitle, CardFooter } from '@/components/ui/card';
import { ArrowLeft, UploadCloud } from 'lucide-react';

enum AccountStatus {
  Pending = 0,
  Active = 1,
  Suspended = 2,
  Locked = 3,
  Expired = 4,
  Inactive = 5
}

const statusConfig = {
  0: { label: "Chờ kích hoạt" },
  1: { label: "Hoạt động" },
  2: { label: "Tạm khóa" },
  3: { label: "Khóa vĩnh viễn" },
  4: { label: "Hết hạn" },
  5: { label: "Không hoạt động" }
};

interface FormData {
  ID: number;
  Email: string;
  RoleID: number;
  Avatar: string | null;
  Status: number;
  FullName: string | null;
  PhoneNumber: string | null;
  Password?: string;
}

function EditAccountForm() {
  const router = useRouter();
  const searchParams = useSearchParams();
  const accountId = searchParams.get('id');

  const [formData, setFormData] = useState<Partial<FormData>>({});
  const [isLoading, setIsLoading] = useState(true);
  const [isUploading, setIsUploading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    if (!accountId) {
      setError("Không tìm thấy ID tài khoản.");
      setIsLoading(false);
      return;
    }

    const loadAccountData = async () => {
      try {
        setIsLoading(true);
        const data = await fetchAccountById(Number(accountId));
        if (data) {
          setFormData({
            ID: data.ID,
            Email: data.Email || '',
            RoleID: data.RoleID || 0,
            Avatar: data.Avatar || null,
            Status: data.Status ?? 0,
            FullName: data.FullName || '',
            PhoneNumber: data.PhoneNumber || '',
            Password: '',
          });
        } else {
          setError("Không thể tải dữ liệu tài khoản.");
        }
      } catch (err) {
        setError("Đã xảy ra lỗi khi tải dữ liệu.");
        console.error(err);
      } finally {
        setIsLoading(false);
      }
    };

    loadAccountData();
  }, [accountId]);

  const handleChange = (e: React.ChangeEvent<HTMLInputElement | HTMLSelectElement>) => {
    const { name, value } = e.target;
    setFormData(prev => ({ 
      ...prev, 
      [name]: name === 'Status' || name === 'RoleID' ? Number(value) : value 
    }));
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

  const handleSave = async (e: React.FormEvent) => {
    e.preventDefault();
    if (!accountId || !formData.ID) return;

    const payload: any = {
      Email: formData.Email,
      RoleID: Number(formData.RoleID),
      Avatar: formData.Avatar,
      Status: Number(formData.Status),
      FullName: formData.FullName,
      PhoneNumber: formData.PhoneNumber,
    };

    if (formData.Password && formData.Password.trim() !== '') {
      payload.Password = formData.Password;
    }

    try {
      const result = await fetchAccountEdit(Number(accountId), payload);
      if (result) {
        alert("Cập nhật tài khoản thành công!");
        router.push('/admin/account');
      } else {
        alert("Cập nhật tài khoản thất bại.");
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
        Cập nhật tài khoản
      </div>
      <div className="p-4 md:p-6">
        <Button variant="outline" onClick={() => router.push('/admin/account')} className="mb-4">
          <ArrowLeft className="mr-2 h-4 w-4" /> Quay lại danh sách
        </Button>
        <Card>
          <CardHeader>
            <CardTitle>Chỉnh sửa thông tin tài khoản</CardTitle>
          </CardHeader>
          <form onSubmit={handleSave}>
            <CardContent className="space-y-6">
              <div className="grid grid-cols-1 md:grid-cols-2 gap-6">
                <div className="space-y-2">
                  <label htmlFor="FullName" className="font-medium">Họ và Tên</label>
                  <Input 
                    id="FullName" 
                    name="FullName" 
                    value={formData.FullName || ''} 
                    onChange={handleChange} 
                  />
                </div>
                <div className="space-y-2">
                  <label htmlFor="RoleID" className="font-medium">Role ID</label>
                  <Input 
                    id="RoleID" 
                    name="RoleID" 
                    type="number"
                    value={formData.RoleID || ''} 
                    onChange={handleChange} 
                  />
                </div>
              </div>

              <div className="grid grid-cols-1 md:grid-cols-2 gap-6">
                <div className="space-y-2">
                  <label htmlFor="Email" className="font-medium">Email</label>
                  <Input 
                    id="Email" 
                    name="Email" 
                    type="email" 
                    value={formData.Email || ''} 
                    onChange={handleChange} 
                  />
                </div>
                <div className="space-y-2">
                  <label htmlFor="PhoneNumber" className="font-medium">Số điện thoại</label>
                  <Input 
                    id="PhoneNumber" 
                    name="PhoneNumber" 
                    value={formData.PhoneNumber || ''} 
                    onChange={handleChange} 
                  />
                </div>
              </div>

              <div className="space-y-2">
                <label htmlFor="Password" className="font-medium">Mật khẩu mới</label>
                <Input 
                  id="Password" 
                  name="Password" 
                  type="password" 
                  placeholder="Để trống nếu không muốn thay đổi" 
                  value={formData.Password || ''}
                  onChange={handleChange} 
                />
              </div>

              <div className="grid grid-cols-1 md:grid-cols-2 gap-6">
                <div className="space-y-2">
                  <label htmlFor="Status" className="font-medium">Trạng thái</label>
                  <select
                    id="Status"
                    name="Status"
                    value={formData.Status ?? 0}
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
                      alt="Avatar" 
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
                  <Button type="button" variant="outline" asChild>
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
              <Button type="button" variant="outline" onClick={() => router.back()}>Hủy</Button>
              <Button type="submit">Lưu thay đổi</Button>
            </CardFooter>
          </form>
        </Card>
      </div>
    </>
  );
}

export default function EditAccountPage() {
    return (
        <Suspense fallback={<div>Đang tải...</div>}>
            <EditAccountForm />
        </Suspense>
    );
}