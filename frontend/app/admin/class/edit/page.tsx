
"use client";
 
import { useEffect, useState, Suspense } from 'react';
import { useRouter, useSearchParams } from 'next/navigation';
import { Input } from '@/components/ui/input';
import { Button } from '@/components/ui/button';
import { Card, CardContent, CardHeader, CardTitle, CardFooter } from '@/components/ui/card';
import { ArrowLeft, FileDown } from 'lucide-react';
import { fetchClassById, fetchEditClass, fetchExportStudentByClass } from '@/app/lib/api';
import { toast } from 'react-toastify';
import 'react-toastify/dist/ReactToastify.css';

interface FormData {
  ClassId: number;
  ClassCode: string;
  ClassName: string;
  DepartmentId: number;
  TeacherId: number;
  AcademicYear: string;
  Semester: number;
}

function EditClassForm() {
  const router = useRouter();
  const searchParams = useSearchParams();
  const classId = searchParams.get('id');

  const [formData, setFormData] = useState<FormData>({
    ClassId: 0,
    ClassCode: "",
    ClassName: "",
    DepartmentId: 0,
    TeacherId: 0,
    AcademicYear: "",
    Semester: 1,
  });
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    if (!classId) return;

    const loadData = async () => {
      try {
        const data = await fetchClassById(Number(classId));
        console.log("Class data loaded:", data);
        if (data) {
          setFormData({
            ClassId: data.ClassId || data.classId || 0,
            ClassCode: data.ClassCode || data.classCode || "",
            ClassName: data.ClassName || data.className || "",
            DepartmentId: data.DepartmentId || data.departmentId || 0,
            TeacherId: data.TeacherId || data.teacherId || 0,
            AcademicYear: data.AcademicYear || data.academicYear || "",
            Semester: data.Semester || data.semester || 1,
          });
        } else {
          toast.error("Không tìm thấy dữ liệu lớp học.");
        }
      } catch (error) {
        console.error("Failed to load class", error);
        toast.error("Không thể tải thông tin lớp học.");
      } finally {
        setLoading(false);
      }
    };
    loadData();
  }, [classId]);

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
      const result = await fetchEditClass(formData.ClassId, formData);
      if (result) {
        toast.success("Cập nhật lớp học thành công!");
        router.push("/admin/class");
      } else {
        toast.error("Cập nhật lớp học thất bại.");
      }
    } catch (err) {
      console.error("Save error:", err);
      toast.error("Đã xảy ra lỗi khi lưu.");
    }
  };

  const handleExport = async () => {
    if (!formData.ClassId) return;
    toast.info("Đang tải xuống danh sách...");
    const success = await fetchExportStudentByClass(formData.ClassId);
    if (!success) {
      toast.error("Xuất file thất bại.");
    }
  };

  if (loading) {
    return <div className="p-6 flex justify-center">
      <div className="animate-spin rounded-full h-8 w-8 border-b-2 border-gray-900"></div>
    </div>;
  }

  return (
    <>
      <div className="text-2xl font-bold py-2 px-6">Cập nhật lớp học</div>
      <div className="p-4 md:p-6">
        <Button variant="outline" onClick={() => router.back()} className="mb-4">
          <ArrowLeft className="mr-2 h-4 w-4" /> Quay lại
        </Button>
        <Card>
          <CardHeader>
            <CardTitle>Chỉnh sửa thông tin lớp học</CardTitle>
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
                  <Input id="ClassName" name="ClassName" value={formData.ClassName} onChange={handleChange} required />
                </div>
              </div>
              <div className="grid grid-cols-1 md:grid-cols-2 gap-6">
                <div className="space-y-2">
                  <label htmlFor="DepartmentId" className="font-medium">Mã khoa (Department ID)</label>
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
            <CardFooter className="flex justify-between mt-4">
              <Button type="button" variant="secondary" onClick={handleExport} className="flex items-center gap-2">
                <FileDown className="h-4 w-4" /> Xuất danh sách SV
              </Button>
              <div className="flex gap-3">
                <Button type="button" variant="outline" onClick={() => router.back()}>Hủy</Button>
                <Button type="submit">Lưu thay đổi</Button>
              </div>
            </CardFooter>
          </form>
        </Card>
      </div>
    </>
  );
}

export default function EditClassPage() {
  return (
    <Suspense fallback={
      <div className="flex justify-center items-center h-screen">
        <div className="animate-spin rounded-full h-12 w-12 border-b-2 border-gray-900"></div>
      </div>
    }>
      <EditClassForm />
    </Suspense>
  );
}
