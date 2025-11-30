"use client";

import { useEffect, useState, Suspense } from 'react';
import { useRouter, useSearchParams } from 'next/navigation';
import { fetchAccountById, fetchAccountEdit } from '../../../lib/api';
import { Input } from '@/components/ui/input';
import { Button } from '@/components/ui/button';
import { Card, CardContent, CardHeader, CardTitle, CardFooter } from '@/components/ui/card';
import { ArrowLeft } from 'lucide-react';

// Re-defining enum and config for self-containment, ideally this should be in a shared file.
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
  id: number;
  email: string;
  roleID: string;
  avatar: string;
  trangThai: number;
  tenHienThi: string;
  hoTen: string;
  sdt: string;
  matKhau: string; // Added for password changes
}

function EditAccountForm() {
  const router = useRouter();
  const searchParams = useSearchParams();
  const accountId = searchParams.get('id');

  const [formData, setFormData] = useState<Partial<FormData>>({});
  const [isLoading, setIsLoading] = useState(true);
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
            id: data.ID,
            email: data.Email || '',
            roleID: data.RoleID || '',
            avatar: data.Avatar || '',
            trangThai: typeof data.TrangThai === 'boolean' ? (data.TrangThai ? 1 : 0) : data.TrangThai,
            tenHienThi: data.TenHienThi || '',
            hoTen: data.HoTen || '',
            sdt: data.SDT || '',
            matKhau: '', 
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
    setFormData(prev => ({ ...prev, [name]: value }));
  };

  const handleSave = async (e: React.FormEvent) => {
    e.preventDefault();
    if (!accountId) return;

    // Construct the payload, excluding the password if it's empty
    const payload: any = {
      Email: formData.email,
      RoleID: formData.roleID,
      Avatar: formData.avatar,
      TrangThai: Number(formData.trangThai),
      TenHienThi: formData.tenHienThi,
      HoTen: formData.hoTen,
      SDT: formData.sdt,
    };

    if (formData.matKhau) {
      payload.MatKhau = formData.matKhau;
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
                <label htmlFor="hoTen" className="font-medium">Họ và Tên</label>
                <Input id="hoTen" name="hoTen" value={formData.hoTen || ''} onChange={handleChange} />
              </div>
              <div className="space-y-2">
                <label htmlFor="tenHienThi" className="font-medium">Tên hiển thị</label>
                <Input id="tenHienThi" name="tenHienThi" value={formData.tenHienThi || ''} onChange={handleChange} />
              </div>
            </div>
            
            <div className="grid grid-cols-1 md:grid-cols-2 gap-6">
              <div className="space-y-2">
                <label htmlFor="email" className="font-medium">Email</label>
                <Input id="email" name="email" type="email" value={formData.email || ''} onChange={handleChange} />
              </div>
              <div className="space-y-2">
                <label htmlFor="sdt" className="font-medium">Số điện thoại</label>
                <Input id="sdt" name="sdt" value={formData.sdt || ''} onChange={handleChange} />
              </div>
            </div>

            <div className="space-y-2">
              <label htmlFor="matKhau" className="font-medium">Mật khẩu mới</label>
              <Input id="matKhau" name="matKhau" type="password" placeholder="Để trống nếu không muốn thay đổi" onChange={handleChange} />
            </div>

            <div className="grid grid-cols-1 md:grid-cols-2 gap-6">
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
               <div className="space-y-2">
                <label htmlFor="roleID" className="font-medium">Vai trò</label>
                <Input id="roleID" name="roleID" value={formData.roleID || ''} onChange={handleChange} placeholder="VD: Admin, User,..." />
              </div>
            </div>

            <div className="space-y-2">
              <label htmlFor="avatar" className="font-medium">URL Avatar</label>
              <div className="flex items-center gap-4">
                  <Input id="avatar" name="avatar" value={formData.avatar || ''} onChange={handleChange} className="flex-grow"/>
                  {formData.avatar && <img src={formData.avatar} alt="Avatar" className="w-12 h-12 rounded-full object-cover"/>}
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


// Use Suspense to handle client-side data fetching with searchParams
export default function EditAccountPage() {
    return (
        <Suspense fallback={<div>Đang tải...</div>}>
            <EditAccountForm />
        </Suspense>
    );
}
