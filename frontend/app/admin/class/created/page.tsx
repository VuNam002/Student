"use client";
 
import { useState } from 'react';

import { useRouter } from 'next/navigation';
import { Input } from '@/components/ui/input';
import { Button } from '@/components/ui/button';
import { Card, CardContent, CardHeader, CardTitle, CardFooter } from '@/components/ui/card';
import {fetchCreateClass} from '@/app/lib/api';
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

  const handleChange = (e: React.ChangeEvent<HTMLInputElement>) => {
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
                  <Input id="DepartmentId" name="DepartmentId" type="number" value={formData.DepartmentId} onChange={handleChange} required />
                </div>
                <div className="space-y-2">
                  <label htmlFor="TeacherId" className="font-medium">Mã giảng viên (Teacher ID)</label>
                  <Input id="TeacherId" name="TeacherId" type="number" value={formData.TeacherId} onChange={handleChange} required />
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
