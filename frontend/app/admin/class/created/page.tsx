"use client";
 
import { useState, useEffect } from 'react';

import { useRouter } from 'next/navigation';
import { Input } from '@/components/ui/input';
import { Button } from '@/components/ui/button';
import { Card, CardContent, CardHeader, CardTitle, CardFooter } from '@/components/ui/card';
import { fetchCreateClass, fetchDepartments, fetchAccount } from '@/app/lib/api';
import { ArrowLeft } from 'lucide-react';
import { toast } from 'react-toastify';
import 'react-toastify/dist/ReactToastify.css';


interface FormData {
  ClassCode: string;
  ClassName: string;
  DepartmentId: number;
  TeacherId: number;
  AcademicYear: string;
  Semester: number;
}

export default function CreateClassPage() {
  const router = useRouter();
  const [formData, setFormData] = useState<FormData>({
    ClassCode: "",
    ClassName: "",
    DepartmentId: 0,
    TeacherId: 0,
    AcademicYear: "",
    Semester: 1,
  });

  const [departments, setDepartments] = useState<any[]>([]);
  const [teachers, setTeachers] = useState<any[]>([]);

  useEffect(() => {
    const loadData = async () => {
      const depts = await fetchDepartments();
      if (depts) setDepartments(depts);

      const accs = await fetchAccount(1, 100);
      if (accs && accs.items) {
        const teacherList = accs.items.filter((acc: any) => 
          acc.RoleName?.toLowerCase().includes('teacher') || 
          acc.RoleName?.toLowerCase().includes('giáo viên')
        );
        setTeachers(teacherList);
      }
    };
    loadData();
  }, []);

  const handleChange = (e: React.ChangeEvent<HTMLInputElement | HTMLSelectElement>) => {
    const { name, value } = e.target;
    setFormData((prev) => ({
      ...prev,
      [name]: name === "DepartmentId" || name === "TeacherId" || name === "Semester" ? Number(value) : value,
    }));
  };

  const handleSave = async (e: React.FormEvent) => {
    e.preventDefault();
    try {
      const result = await fetchCreateClass(formData);
      if (result) {
        toast.success("Thêm mới lớp học thành công!");
        router.push("/admin/class");
      } else {
        toast.success("Thêm mới lớp thất bại.");
      }
    } catch (err) {
      console.error("Save error:", err);
      toast.success("Đã xảy ra lỗi khi lưu.");
    }
  };

  return (
    <>
      <div className="text-2xl font-bold py-2 px-6">Thêm mới lớp học</div>
      <div className="p-4 md:p-6">
        <Button variant="outline" onClick={() => router.back()} className="mb-4">
          <ArrowLeft className="mr-2 h-4 w-4" /> Quay lại
        </Button>
        <Card>
          <CardHeader>
            <CardTitle>Nhập thông tin lớp học</CardTitle>
          </CardHeader>
          <form onSubmit={handleSave}>
            <CardContent className="space-y-6">
              <div className="grid grid-cols-1 md:grid-cols-2 gap-6">
                <div className="space-y-2">
                  <label htmlFor="ClassCode" className="font-medium">Mã lớp</label>
                  <Input id="ClassCode" name="ClassCode" value={formData.ClassCode} onChange={handleChange} required />
                </div>
                <div className="space-y-2">
                  <label htmlFor="ClassName" className="font-medium">Tên lớp</label>
                  <Input id="ClassName" name="ClassName" value={formData.ClassName} onChange={handleChange} required  />
                </div>
              </div>
              <div className="grid grid-cols-1 md:grid-cols-2 gap-6">
                <div className="space-y-2">
                  <label htmlFor="DepartmentId" className="font-medium">Mã khoa</label>
                  <select
                    id="DepartmentId"
                    name="DepartmentId"
                    value={formData.DepartmentId}
                    onChange={handleChange}
                    className="flex h-10 w-full rounded-md border border-input bg-background px-3 py-2 text-sm ring-offset-background file:border-0 file:bg-transparent file:text-sm file:font-medium placeholder:text-muted-foreground focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-ring focus-visible:ring-offset-2 disabled:cursor-not-allowed disabled:opacity-50"
                    required
                  >
                    <option value={0}>-- Chọn Khoa --</option>
                    {departments.map((dept) => (
                      <option key={dept.DepartmentID} value={dept.DepartmentID}>{dept.DepartmentName}</option>
                    ))}
                  </select>
                </div>
                <div className="space-y-2">
                  <label htmlFor="TeacherId" className="font-medium">Giảng viên</label>
                  <select id="TeacherId" name="TeacherId" value={formData.TeacherId} onChange={handleChange} className="flex h-10 w-full rounded-md border border-input bg-background px-3 py-2 text-sm ring-offset-background file:border-0 file:bg-transparent file:text-sm file:font-medium placeholder:text-muted-foreground focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-ring focus-visible:ring-offset-2 disabled:cursor-not-allowed disabled:opacity-50" required>
                    <option value={0}>-- Chọn Giảng viên --</option>
                    {teachers.map((teacher) => (
                      <option key={teacher.ID} value={teacher.ID}>{teacher.FullName} ({teacher.Email})</option>
                    ))}
                  </select>
                </div>
              </div>
              <div className="grid grid-cols-1 md:grid-cols-2 gap-6">
                <div className="space-y-2">
                  <label htmlFor="AcademicYear" className="font-medium">Niên khóa</label>
                  <Input id="AcademicYear" name="AcademicYear" value={formData.AcademicYear} onChange={handleChange} required />
                </div>
                <div className="space-y-2">
                  <label htmlFor="Semester" className="font-medium">Học kỳ</label>
                  <Input id="Semester" name="Semester" type="number" value={formData.Semester} onChange={handleChange} required min={1} />
                </div>
              </div>
            </CardContent>
            <CardFooter className="flex justify-end gap-3 mt-4">
              <Button type="button" variant="outline" onClick={() => router.back()}>Hủy</Button>
              <Button type="submit">Thêm mới</Button>
            </CardFooter>
          </form>
        </Card>
      </div>
    </>
  );
}
