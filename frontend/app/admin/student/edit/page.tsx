"use client";

import { useEffect, useState, Suspense } from "react";
import { useRouter, useSearchParams } from "next/navigation";
import { fetchDetailStudent, fetchUpdateStudent } from "../../../lib/api";
import { Input } from "@/components/ui/input";
import { Button } from "@/components/ui/button";
import {
  Card,
  CardContent,
  CardHeader,
  CardTitle,
  CardFooter,
} from "@/components/ui/card";
import { ArrowLeft } from "lucide-react";


const statusConfig = {
    1 : { label: "Đang học"},
    2 : { label: "Đã tốt nghiệp"},
    3 : { label: "Bị đình chỉ"},
    4 : { label: "Thôi học"},
    5 : { label: "Bảo lưu"},
}

interface FormData {
  StudentCode: string;
  ClassID: number;
  Status: number;
  EnrollmentDate: string;
  Person: {
    FullName: string;
    DateOfBirth: string;
    Gender: string;
    Email: string;
    PhoneNumber: string;
    Address: string;
  };
}

function EditStudentForm() {
  const router = useRouter();
  const searchParams = useSearchParams();
  const studentId = searchParams.get("id");

  const [formData, setFormData] = useState<FormData>({
    StudentCode: "",
    ClassID: 0,
    Status: 1,
    EnrollmentDate: "",
    Person: {
      FullName: "",
      DateOfBirth: "",
      Gender: "MALE",
      Email: "",
      PhoneNumber: "",
      Address: "",
    },
  });
  const [isLoading, setIsLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  const formatDateForInput = (dateString: string | null | undefined) => {
    if (!dateString) return "";
    const date = new Date(dateString);
    if (isNaN(date.getTime())) return "";
    return date.toISOString().split("T")[0];
  };

  useEffect(() => {
    if (!studentId) {
      setError("Không tìm thấy ID sinh viên.");
      setIsLoading(false);
      return;
    }

    const loadData = async () => {
      try {
        setIsLoading(true);
        const data = await fetchDetailStudent(Number(studentId));
        if (data) {
          setFormData({
            StudentCode: data.StudentCode || "",
            ClassID: data.ClassID || 0,
            Status: data.Status || 1,
            EnrollmentDate: data.EnrollmentDate || "",
            Person: {
              FullName: data.Person?.FullName || "",
              DateOfBirth: data.Person?.DateOfBirth || "",
              Gender: data.Person?.Gender || "MALE",
              Email: data.Person?.Email || "",
              PhoneNumber: data.Person?.PhoneNumber || "",
              Address: data.Person?.Address || "",
            },
          });
        } else {
          setError("Không thể tải dữ liệu sinh viên.");
        }
      } catch (err) {
        setError("Đã xảy ra lỗi khi tải dữ liệu.");
        console.error(err);
      } finally {
        setIsLoading(false);
      }
    };

    loadData();
  }, [studentId]);

  const handleChange = (
    e: React.ChangeEvent<HTMLInputElement | HTMLSelectElement>
  ) => {
    const { name, value } = e.target;

    if (name.startsWith("Person.")) {
      const personField = name.split(".")[1];
      setFormData((prev) => ({
        ...prev,
        Person: {
          ...prev.Person,
          [personField]: value,
        },
      }));
    } else {
      setFormData((prev) => ({
        ...prev,
        [name]: name === "ClassID" || name === "Status" ? Number(value) : value,
      }));
    }
  };

  const handleSave = async (e: React.FormEvent) => {
    e.preventDefault();
    if (!studentId) return;

    try {
      const payload = {
        ...formData,
        EnrollmentDate: formData.EnrollmentDate ? new Date(formData.EnrollmentDate).toISOString() : new Date().toISOString(),
        Person: {
            ...formData.Person,
            DateOfBirth: formData.Person.DateOfBirth ? new Date(formData.Person.DateOfBirth).toISOString() : new Date().toISOString(),
        }
      };

      const result = await fetchUpdateStudent(Number(studentId), payload);

      if (result) {
        alert("Cập nhật sinh viên thành công!");
        router.push("/admin/student");
      } else {
        alert("Cập nhật sinh viên thất bại.");
      }
    } catch (err) {
      console.error("Save error:", err);
      alert("Đã xảy ra lỗi khi lưu.");
    }
  };

  if (isLoading) {
    return (
      <div className="flex justify-center items-center h-64">
        <div className="animate-spin rounded-full h-12 w-12 border-b-2 border-gray-900"></div>
      </div>
    );
  }

  if (error) {
    return <div className="text-center text-red-500 py-10">{error}</div>;
  }

  return (
    <>
      <div className="text-2xl font-bold py-2 px-6">Cập nhật sinh viên</div>
      <div className="p-4 md:p-6">
        <Button
          variant="outline"
          onClick={() => router.back()}
          className="mb-4"
        >
          <ArrowLeft className="mr-2 h-4 w-4" /> Quay lại
        </Button>
        <Card>
          <CardHeader>
            <CardTitle>Chỉnh sửa thông tin sinh viên</CardTitle>
          </CardHeader>
          <form onSubmit={handleSave}>
            <CardContent className="space-y-6">
              <div className="grid grid-cols-1 md:grid-cols-2 gap-6">
                <div className="space-y-2">
                  <label htmlFor="StudentCode" className="font-medium">Mã sinh viên</label>
                  <Input id="StudentCode" name="StudentCode" value={formData.StudentCode} onChange={handleChange} required />
                </div>
                <div className="space-y-2">
                  <label htmlFor="ClassID" className="font-medium">Mã lớp (ID)</label>
                  <Input id="ClassID" name="ClassID" type="number" value={formData.ClassID} onChange={handleChange} required />
                </div>
              </div>
              <div className="grid grid-cols-1 md:grid-cols-2 gap-6">
                <div className="space-y-2">
                  <label htmlFor="Status" className="font-medium">Trạng thái</label>
                  <select id="Status" name="Status" value={formData.Status} onChange={handleChange} className="w-full p-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500">
                    {Object.entries(statusConfig).map(([key, { label }]) => (
                      <option key={key} value={key}>{label}</option>
                    ))}
                  </select>
                </div>
                <div className="space-y-2">
                  <label htmlFor="EnrollmentDate" className="font-medium">Ngày nhập học</label>
                  <Input id="EnrollmentDate" name="EnrollmentDate" type="date" value={formatDateForInput(formData.EnrollmentDate)} onChange={handleChange} required />
                </div>
              </div>
              <hr className="my-4" />
              <h3 className="text-lg font-semibold">Thông tin cá nhân</h3>
              <div className="grid grid-cols-1 md:grid-cols-2 gap-6">
                <div className="space-y-2">
                  <label htmlFor="Person.FullName" className="font-medium">Họ và Tên</label>
                  <Input id="Person.FullName" name="Person.FullName" value={formData.Person.FullName} onChange={handleChange} required />
                </div>
                <div className="space-y-2">
                  <label htmlFor="Person.Gender" className="font-medium">Giới tính</label>
                  <select id="Person.Gender" name="Person.Gender" value={formData.Person.Gender} onChange={handleChange} className="w-full p-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500">
                    <option value="MALE">Nam</option>
                    <option value="FEMALE">Nữ</option>
                    <option value="OTHER">Khác</option>
                  </select>
                </div>
              </div>
              <div className="grid grid-cols-1 md:grid-cols-2 gap-6">
                <div className="space-y-2">
                  <label htmlFor="Person.Email" className="font-medium">Email</label>
                  <Input id="Person.Email" name="Person.Email" type="email" value={formData.Person.Email} onChange={handleChange} required />
                </div>
                <div className="space-y-2">
                  <label htmlFor="Person.PhoneNumber" className="font-medium">Số điện thoại</label>
                  <Input id="Person.PhoneNumber" name="Person.PhoneNumber" value={formData.Person.PhoneNumber} onChange={handleChange} />
                </div>
              </div>
              <div className="grid grid-cols-1 md:grid-cols-2 gap-6">
                <div className="space-y-2">
                  <label htmlFor="Person.DateOfBirth" className="font-medium">Ngày sinh</label>
                  <Input id="Person.DateOfBirth" name="Person.DateOfBirth" type="date" value={formatDateForInput(formData.Person.DateOfBirth)} onChange={handleChange} required />
                </div>
                <div className="space-y-2">
                  <label htmlFor="Person.Address" className="font-medium">Địa chỉ</label>
                  <Input id="Person.Address" name="Person.Address" value={formData.Person.Address} onChange={handleChange} />
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

export default function EditStudentPage() {
  return (
    <Suspense
      fallback={
        <div className="flex justify-center items-center h-screen">
          <div className="animate-spin rounded-full h-12 w-12 border-b-2 border-gray-900"></div>
        </div>
      }
    >
      <EditStudentForm />
    </Suspense>
  );
}