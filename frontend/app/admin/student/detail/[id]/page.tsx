"use client";
import { useEffect, useState } from "react";
import { useParams, useRouter } from "next/navigation";
import { fetchDetailStudent } from "@/app/lib/api";
import { StudentDetail } from "@/app/lib/types";
import { Button } from "@/components/ui/button";
import { Card, CardContent } from "@/components/ui/card";
import { Skeleton } from "@/components/ui/skeleton";
import {
  CheckCircle,
  AlertCircle,
  Lock,
  Calendar,
  Ban,
} from "lucide-react";

const statusConfig = {
  1: {
    label: "Đang học",
    icon: CheckCircle,
    color: "text-emerald-600",
    bg: "bg-emerald-50",
    border: "border-emerald-200",
  },
  2: {
    label: "Đã tốt nghiệp",
    icon: AlertCircle,
    color: "text-orange-600",
    bg: "bg-orange-50",
    border: "border-orange-200",
  },
  3: {
    label: "Bị đình chỉ",
    icon: Lock,
    color: "text-red-600",
    bg: "bg-red-50",
    border: "border-red-200",
  },
  4: {
    label: "Hết hạn",
    icon: Calendar,
    color: "text-gray-600",
    bg: "bg-gray-50",
    border: "border-gray-200",
  },
  5: {
    label: "Bảo lưu",
    icon: Ban,
    color: "text-purple-600",
    bg: "bg-purple-50",
    border: "border-purple-200",
  },
};

export default function DetailStudent() {
  const params = useParams();
  const router = useRouter();
  const id = params.id ? parseInt(params.id as string, 10) : null;

  const [student, setStudent] = useState<StudentDetail | null>(null);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    if (id) {
      const getStudent = async () => {
        try {
          setLoading(true);
          const data = await fetchDetailStudent(id);
          setStudent(data);
        } catch (error) {
          console.error("Failed to fetch student detail", error);
          setStudent(null);
        } finally {
          setLoading(false);
        }
      };
      getStudent();
    }
  }, [id]);

  const getStatusDisplay = (status: number) => {
    const config = statusConfig[status as keyof typeof statusConfig];
    if (!config) {
      return {
        label: "Đang học",
        icon: CheckCircle,
        color: "text-emerald-600",
        bg: "bg-emerald-50",
        border: "border-emerald-200",
      };
    }
    return config;
  };

  const formatDate = (dateString: string | null) => {
    if (!dateString) return "N/A";
    return new Date(dateString).toLocaleDateString("vi-VN");
  };

  const formatGender = (gender: string) => {
    return gender === "MALE" ? "Nam" : gender === "FEMALE" ? "Nữ" : "Khác";
  };

  return (
    <div className="p-4 md:p-8 -mt-6">
      <div>
        <h1 className="text-3xl font-bold mb-4">Chi tiết sinh viên</h1>
      </div>
      <Button onClick={() => router.back()} variant="outline" className="mb-4">
        &larr; Quay lại
      </Button>
      <Card>
        <CardContent className="pt-6">
          {loading ? (
            <div className="space-y-4">
              <div className="flex items-center space-x-4">
                <Skeleton className="h-24 w-24 rounded-full" />
                <div className="space-y-2">
                  <Skeleton className="h-6 w-[250px]" />
                  <Skeleton className="h-5 w-[200px]" />
                </div>
              </div>
              <div className="grid grid-cols-1 md:grid-cols-2 gap-4 pt-4">
                {[...Array(6)].map((_, i) => (
                  <div key={i} className="space-y-2">
                    <Skeleton className="h-4 w-[100px]" />
                    <Skeleton className="h-8 w-full" />
                  </div>
                ))}
              </div>
            </div>
          ) : student ? (
            <div>
              <div className="flex items-center space-x-4 mb-6">
                
                <div>
                  <h2 className="text-2xl font-bold"> Họ tên: {student.Person.FullName}</h2>
                </div>
              </div>
              <div className="grid grid-cols-1 md:grid-cols-2 gap-x-8 gap-y-4">
                <div className="flex flex-col">
                  <span className="text-sm font-medium text-muted-foreground">Mã sinh viên:</span>
                  <p className="text-lg font-medium">{student.StudentCode}</p>
                </div>
                <div className="flex flex-col">
                  <span className="text-sm font-medium text-muted-foreground">Lớp:</span>
                  <p className="text-lg font-medium">{student.ClassName || "N/A"}</p>
                </div>
                <div className="flex flex-col">
                  <span className="text-sm font-medium text-muted-foreground">Email:</span>
                  <p className="text-lg">{student.Person.Email || "N/A"}</p>
                </div>
                <div className="flex flex-col">
                  <span className="text-sm font-medium text-muted-foreground">Số điện thoại:</span>
                  <p className="text-lg">{student.Person.PhoneNumber || "N/A"}</p>
                </div>
                <div className="flex flex-col">
                  <span className="text-sm font-medium text-muted-foreground">Ngày sinh:</span>
                  <p className="text-lg">{formatDate(student.Person.DateOfBirth)}</p>
                </div>
                <div className="flex flex-col">
                  <span className="text-sm font-medium text-muted-foreground">Giới tính:</span>
                  <p className="text-lg">{formatGender(student.Person.Gender)}</p>
                </div>
                <div className="flex flex-col">
                  <span className="text-sm font-medium text-muted-foreground">Địa chỉ:</span>
                  <p className="text-lg">{student.Person.Address || "N/A"}</p>
                </div>
                <div className="flex flex-col">
                  <span className="text-sm font-medium text-muted-foreground">Ngày nhập học:</span>
                  <p className="text-lg">{formatDate(student.EnrollmentDate)}</p>
                </div>
                <div className="flex flex-col">
                  <span className="text-sm font-medium text-muted-foreground">Trạng thái:</span>
                  <div className="mt-1">
                    {(() => {
                      const statusDisplay = getStatusDisplay(student.Status);
                      const StatusIcon = statusDisplay.icon;
                      return (
                        <span
                          className={`inline-flex items-center gap-1.5 px-3 py-1.5 rounded-full text-sm font-medium ${statusDisplay.bg} ${statusDisplay.color} ${statusDisplay.border} border`}
                        >
                          <StatusIcon className="w-4 h-4" />
                          {statusDisplay.label}
                        </span>
                      );
                    })()}
                  </div>
                </div>
                <div className="flex flex-col">
                  <span className="text-sm font-medium text-muted-foreground">Ngày tạo:</span>
                  <p className="text-lg">{formatDate(student.CreatedAt)}</p>
                </div>
                <div className="flex flex-col">
                  <span className="text-sm font-medium text-muted-foreground">Cập nhật lần cuối:</span>
                  <p className="text-lg">{formatDate(student.UpdatedAt)}</p>
                </div>
              </div>
            </div>
          ) : (
            <div className="text-center py-8">
              <p className="text-muted-foreground">Không tìm thấy thông tin sinh viên</p>
            </div>
          )}
        </CardContent>
      </Card>
    </div>
  );
}