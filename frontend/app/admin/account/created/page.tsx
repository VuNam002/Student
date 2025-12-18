"use client";

import { useState, useEffect } from 'react';
import { useRouter } from 'next/navigation';
import { Input } from '@/components/ui/input';
import { Button } from '@/components/ui/button';
import { Card, CardContent, CardFooter } from '@/components/ui/card';
import { ArrowLeft, UploadCloud } from 'lucide-react';
import { fetchAccountCreat, fetchRole, fetchClasses, fetchDepartments } from '@/app/lib/api';
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
  // Account info
  Email: string;
  RoleID: string;
  Avatar: string;
  Status: number;
  FullName: string;
  PhoneNumber: string;
  Password: string;
  
  // Person info
  DateOfBirth: string;
  Gender: string;
  Address: string;
  IdentityCard: string;
  
  // Teacher info
  DepartmentID: string;
  Position: string;
  Degree: string;
  Specialization: string;
  
  // Student info
  ClassId: string;
  EnrollmentDate: string;
}

export default function AdminAccountCreatePage() {
  const router = useRouter();
  const [roles, setRoles] = useState<RoleDto[]>([]);
  const [classes, setClasses] = useState<any[]>([]);
  const [departments, setDepartments] = useState<any[]>([]);
  const [selectedRoleName, setSelectedRoleName] = useState<string>('');

  const [formData, setFormData] = useState<FormData>({
    // Account
    Email: '',
    RoleID: '',
    Avatar: '',
    Status: 1,
    FullName: '',
    PhoneNumber: '',
    Password: '',
    
    // Person
    DateOfBirth: '',
    Gender: '',
    Address: '',
    IdentityCard: '',
    
    // Teacher
    DepartmentID: '',
    Position: '',
    Degree: '',
    Specialization: '',
    
    // Student
    ClassId: '',
    EnrollmentDate: '',
  });

  const [isUploading, setIsUploading] = useState(false);
  const [errors, setErrors] = useState<Partial<Record<keyof FormData, string>>>({});

  // Load Roles
  useEffect(() => {
    const getRoles = async () => {
      const rolesData = await fetchRole();
      if (rolesData) {
        setRoles(rolesData);
        if (rolesData.length > 0) {
          setFormData(prev => ({ ...prev, RoleID: rolesData[0].RoleID.toString() }));
          setSelectedRoleName(rolesData[0].RoleName.toLowerCase());
        }
      }
    };
    getRoles();
  }, []);

  // Load Classes (for Student)
  useEffect(() => {
    const getClasses = async () => {
      try {
        const classesData = await fetchClasses();
        if (classesData) {
          setClasses(classesData.Classes);
        }
      } catch (error) {
        console.error('Error loading classes:', error);
      }
    };
    getClasses();
  }, []);

  // Load Departments
  useEffect(() => {
    const getDepartments = async () => {
      const data = await fetchDepartments();
      if (data) {
        setDepartments(data);
      }
    };
    getDepartments();
  }, []);

  const handleChange = (e: React.ChangeEvent<HTMLInputElement | HTMLSelectElement | HTMLTextAreaElement>) => {
    const { name, value } = e.target;
    setFormData(prev => ({ ...prev, [name]: value }));
    if (errors[name as keyof FormData]) {
      setErrors(prev => ({ ...prev, [name]: '' }));
    }
  };

  const handleRoleChange = (e: React.ChangeEvent<HTMLSelectElement>) => {
    const roleId = e.target.value;
    const role = roles.find(r => r.RoleID.toString() === roleId);
    
    setFormData(prev => ({ ...prev, RoleID: roleId }));
    setSelectedRoleName(role?.RoleName.toLowerCase() || '');
    
    if (errors.RoleID) {
      setErrors(prev => ({ ...prev, RoleID: '' }));
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

    // Basic validation
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

    if (selectedRoleName.includes('teacher') || selectedRoleName.includes('giáo viên')) {
      if (!formData.DepartmentID) {
        newErrors.DepartmentID = "ID Khoa là bắt buộc cho giáo viên";
      }
    }

    if (selectedRoleName.includes('student') || selectedRoleName.includes('sinh viên')) {
      if (!formData.ClassId) {
        newErrors.ClassId = "Lớp là bắt buộc cho sinh viên";
      }
    }

    setErrors(newErrors);
    return Object.keys(newErrors).length === 0;
  };

  const handleSubmit = async () => {
    if (!validateForm()) {
      return;
    }

    const payload: any = {
      Email: formData.Email,
      RoleID: parseInt(formData.RoleID),
      Avatar: formData.Avatar,
      Status: Number(formData.Status),
      FullName: formData.FullName,
      PhoneNumber: formData.PhoneNumber,
      Password: formData.Password,
      
      // Person info
      DateOfBirth: formData.DateOfBirth || null,
      Gender: formData.Gender || null,
      Address: formData.Address || null,
      IdentityCard: formData.IdentityCard || null,
    };

    // Add Teacher fields if role is Teacher
    if (selectedRoleName.includes('teacher') || selectedRoleName.includes('giáo viên')) {
      payload.DepartmentID = parseInt(formData.DepartmentID);
      payload.Position = formData.Position || null;
      payload.Degree = formData.Degree || null;
      payload.Specialization = formData.Specialization || null;
    }

    // Add Student fields if role is Student
    if (selectedRoleName.includes('student') || selectedRoleName.includes('sinh viên')) {
      payload.ClassID = parseInt(formData.ClassId);
      payload.EnrollmentDate = formData.EnrollmentDate || null;
    }

    try {
      const result = await fetchAccountCreat(payload);
      if (result) {
        alert("Tạo tài khoản thành công!");
        router.push('/admin/account');
      } else {
        alert("Tạo tài khoản thất bại.");
      }
    } catch (err: any) {
      console.error("Create error:", err);
      
      // Handle validation errors from backend
      if (err.response?.data?.errors) {
        const backendErrors: any = {};
        err.response.data.errors.forEach((error: any) => {
          backendErrors[error.field] = error.message;
        });
        setErrors(backendErrors);
      } else if (err.response?.data?.message) {
        alert(err.response.data.message);
      } else {
        alert("Đã xảy ra lỗi khi tạo tài khoản.");
      }
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
         
          <CardContent className="space-y-6">
            {/* ========== ACCOUNT INFO ========== */}
            <div className="border-b pb-4">
              <h3 className="text-lg font-semibold mb-4">Thông tin tài khoản</h3>
              <div className="grid grid-cols-1 md:grid-cols-2 gap-6">
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

                <div className="space-y-2">
                  <label htmlFor="RoleID" className="font-medium">
                    Vai trò <span className="text-red-500">*</span>
                  </label>
                  <select
                    id="RoleID"
                    name="RoleID"
                    value={formData.RoleID}
                    onChange={handleRoleChange}
                    className={`w-full p-2 border rounded-md ${errors.RoleID ? 'border-red-500' : 'border-gray-300'}`}
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

              <div className="space-y-2 mt-4">
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
            </div>

            {/* ========== PERSON INFO ========== */}
            <div className="border-b pb-4">
              <h3 className="text-lg font-semibold mb-4">Thông tin cá nhân</h3>
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
                  <label htmlFor="DateOfBirth" className="font-medium">Ngày sinh</label>
                  <Input
                    id="DateOfBirth"
                    name="DateOfBirth"
                    type="date"
                    value={formData.DateOfBirth}
                    onChange={handleChange}
                  />
                </div>

                <div className="space-y-2">
                  <label htmlFor="Gender" className="font-medium">Giới tính</label>
                  <select
                    id="Gender"
                    name="Gender"
                    value={formData.Gender}
                    onChange={handleChange}
                    className="w-full p-2 border border-gray-300 rounded-md"
                  >
                    <option value="">-- Chọn --</option>
                    <option value="Nam">Nam</option>
                    <option value="Nữ">Nữ</option>
                    <option value="Khác">Khác</option>
                  </select>
                </div>

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
                  <label htmlFor="IdentityCard" className="font-medium">CMND/CCCD</label>
                  <Input
                    id="IdentityCard"
                    name="IdentityCard"
                    value={formData.IdentityCard}
                    onChange={handleChange}
                  />
                </div>

                <div className="space-y-2 ">
                  <label htmlFor="Address" className="font-medium">Địa chỉ</label>
                  <textarea
                    id="Address"
                    name="Address"
                    value={formData.Address}
                    onChange={handleChange}
                    className="w-full p-2 border border-gray-300 rounded-md"
                  />
                </div>
              </div>
            </div>

            {(selectedRoleName.includes('teacher') || selectedRoleName.includes('giáo viên')) && (
              <div className="border-b pb-4  p-4 rounded-md">
                <h3 className="text-lg font-semibold mb-4 "> Thông tin Giảng viên</h3>
                <div className="grid grid-cols-1 md:grid-cols-2 gap-6">
                  <div className="space-y-2">
                    <label htmlFor="DepartmentID" className="font-medium">
                      ID Khoa <span className="text-red-500">*</span>
                    </label>
                    <select
                      id="DepartmentID"
                      name="DepartmentID"
                      value={formData.DepartmentID}
                      onChange={handleChange}
                      className={`w-full p-2 border rounded-md ${errors.DepartmentID ? 'border-red-500' : 'border-gray-300'}`}
                    >
                      <option value="">-- Chọn Khoa --</option>
                      {departments.map((dept) => (
                        <option key={dept.DepartmentID} value={dept.DepartmentID}>{dept.DepartmentName}</option>
                      ))}
                    </select>
                    {errors.DepartmentID && <p className="text-red-500 text-sm">{errors.DepartmentID}</p>}
                  </div>

                  <div className="space-y-2">
                    <label htmlFor="Position" className="font-medium">Chức vụ</label>
                    <Input
                      id="Position"
                      name="Position"
                      value={formData.Position}
                      onChange={handleChange}
                    />
                  </div>

                  <div className="space-y-2">
                    <label htmlFor="Degree" className="font-medium">Học vị</label>
                    <Input
                      id="Degree"
                      name="Degree"
                      value={formData.Degree}
                      onChange={handleChange}
                    />
                  </div>

                  <div className="space-y-2">
                    <label htmlFor="Specialization" className="font-medium">Chuyên môn</label>
                    <Input
                      id="Specialization"
                      name="Specialization"
                      value={formData.Specialization}
                      onChange={handleChange}
                    />
                  </div>
                </div>
              </div>
            )}

            {(selectedRoleName.includes('student') || selectedRoleName.includes('sinh viên')) && (
              <div className="border-b pb-4  p-4 rounded-md">
                <h3 className="text-lg font-semibold mb-4 "> Thông tin học sinh</h3>
                <div className="grid grid-cols-1 md:grid-cols-2 gap-6">
                  <div className="space-y-2">
                    <label htmlFor="ClassID" className="font-medium">
                      Lớp <span className="text-red-500">*</span>
                    </label>
                    <select
                      id="ClassID"
                      name="ClassId"
                      value={formData.ClassId}
                      onChange={handleChange}
                      className={`w-full p-2 border rounded-md ${errors.ClassId ? 'border-red-500' : 'border-gray-300'}`}
                    >
                      <option value="">-- Chọn lớp --</option>
                      {classes.map(cls => (
                        <option key={cls.ClassId} value={cls.ClassId}>
                          {cls.ClassName}
                        </option>
                      ))}
                    </select>
                    {errors.ClassId && <p className="text-red-500 text-sm">{errors.ClassId}</p>}
                  </div>

                  <div className="space-y-2">
                    <label htmlFor="EnrollmentDate" className="font-medium">Ngày nhập học</label>
                    <Input
                      id="EnrollmentDate"
                      name="EnrollmentDate"
                      type="date"
                      value={formData.EnrollmentDate}
                      onChange={handleChange}
                    />
                  </div>
                </div>
              </div>
            )}
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