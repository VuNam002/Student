"use client";

import { useEffect, useState } from "react";
import { fetchAllStudent, fetchStudentStatus } from "../../lib/api";
import Link from "next/link";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import {
  Table,
  TableBody,
  TableCell,
  TableHead,
  TableHeader,
  TableRow,
} from "@/components/ui/table";
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
import { Badge } from "@/components/ui/badge";
import { Search, Eye, Loader2, CheckCircle, GraduationCap, AlertCircle, XCircle, MinusCircle } from "lucide-react";
import {
  Pagination,
  PaginationContent,
  PaginationEllipsis,
  PaginationItem,
  PaginationLink,
  PaginationNext,
  PaginationPrevious,
} from "@/components/ui/pagination";
import {Student} from "@/app/lib/types";

export enum StudentStatus {
  Active = 1,
  Graduated = 2,
  Suspended = 3,
  DroppedOut = 4,
  Retired = 5,
}

const studentStatusConfig = {
  1: { label: "Đang học", icon: CheckCircle, color: "text-emerald-600", bg: "bg-emerald-50", border: "border-emerald-200" },
  2: { label: "Đã tốt nghiệp", icon: GraduationCap, color: "text-blue-600", bg: "bg-blue-50", border: "border-blue-200" },
  3: { label: "Đình chỉ", icon: AlertCircle, color: "text-red-600", bg: "bg-red-50", border: "border-red-200" },
  4: { label: "Bỏ học", icon: XCircle, color: "text-orange-600", bg: "bg-orange-50", border: "border-orange-200" },
  5: { label: "Nghỉ học", icon: MinusCircle, color: "text-gray-600", bg: "bg-gray-50", border: "border-gray-200" },
};

export default function StudentPage() {
  const [students, setStudents] = useState<Student[]>([]);
  const [loading, setLoading] = useState(true);
  const [page, setPage] = useState(1);
  const [totalPages, setTotalPages] = useState(1);
  const [totalCount, setTotalCount] = useState(0);
  const [keyword, setKeyword] = useState("");
  const [openDropdown, setOpenDropdown] = useState<number | null>(null);

  const loadStudents = async (currentPage: number, search: string) => {
    setLoading(true);
    try {
      const data = await fetchAllStudent(currentPage, 10, search);
      setStudents(data.items || []);
      setTotalPages(data.totalPages || 1);
      setTotalCount(data.totalCount || 0);
    } catch (error) {
      console.error("Failed to load students", error);
      setStudents([]);
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    loadStudents(page, keyword);
  }, [page]);

  const handleSearch = (e: React.FormEvent) => {
    e.preventDefault();
    setPage(1);
    loadStudents(1, keyword);
  };

  const formatDate = (dateString: string) => {
    if (!dateString) return "N/A";
    const date = new Date(dateString);
    return date.toLocaleDateString("vi-VN");
  };

  const handleStatusChange = async (id: number, newStatus: number) => {
    try {
      const result = await fetchStudentStatus(id, newStatus);
      if (result) {
        setStudents((currentItems) =>
          currentItems.map((item) =>
            item.StudentID === id ? { ...item, Status: newStatus } : item
          )
        );
        setOpenDropdown(null);
      } else {
        alert("Đã xảy ra lỗi khi cập nhật trạng thái.");
      }
    } catch (error) {
      console.error(`Error updating status for item ${id}:`, error);
      alert("Đã xảy ra lỗi khi cập nhật trạng thái.");
    }
  };

  const StatusDisplay = ({ Status, id }: { Status: number; id: number }) => {
    const config = studentStatusConfig[Status as keyof typeof studentStatusConfig] || studentStatusConfig[1];
    const Icon = config.icon;
    const isOpen = openDropdown === id;

    return (
      <div className="relative">
        <button
          onClick={() => setOpenDropdown(isOpen ? null : id)}
          className={`flex items-center gap-2 px-3 py-1.5 rounded-lg border ${config.bg} ${config.border} hover:opacity-80 transition-all`}
        >
          <Icon size={16} className={config.color} />
          <span className={`${config.color} font-medium text-sm`}>
            {config.label}
          </span>
        </button>

        {isOpen && (
          <>
            <div className="fixed inset-0 z-10" onClick={() => setOpenDropdown(null)} />
            <div className="absolute top-full left-0 mt-1 w-56 bg-white rounded-lg shadow-lg border border-gray-200 py-1 z-20">
              {Object.entries(studentStatusConfig).map(([key, value]) => {
                const StatusIcon = value.icon;
                const statusValue = parseInt(key);
                return (
                  <button
                    key={key}
                    onClick={() => handleStatusChange(id, statusValue)}
                    className={`w-full flex items-center gap-3 px-4 py-2 hover:bg-gray-50 transition-colors ${statusValue === Status ? "bg-gray-50" : ""}`}
                  >
                    <StatusIcon size={16} className={value.color} />
                    <span className={`${value.color} text-sm font-medium`}>{value.label}</span>
                    {statusValue === Status && <CheckCircle size={14} className="ml-auto text-blue-600" />}
                  </button>
                );
              })}
            </div>
          </>
        )}
      </div>
    );
  };

  const renderPaginationItems = () => {
    const items = [];
    const maxVisiblePages = 5;

    if (totalPages <= maxVisiblePages) {
      for (let i = 1; i <= totalPages; i++) {
        items.push(
          <PaginationItem key={i}>
            <PaginationLink
              onClick={() => setPage(i)}
              isActive={page === i}
              className="cursor-pointer"
            >
              {i}
            </PaginationLink>
          </PaginationItem>
        );
      }
    } else {
      items.push(
        <PaginationItem key={1}>
          <PaginationLink onClick={() => setPage(1)} isActive={page === 1} className="cursor-pointer">
            1
          </PaginationLink>
        </PaginationItem>
      );

      if (page > 3) {
        items.push(
          <PaginationItem key="ellipsis-start">
            <PaginationEllipsis />
          </PaginationItem>
        );
      }

      const start = Math.max(2, page - 1);
      const end = Math.min(totalPages - 1, page + 1);

      for (let i = start; i <= end; i++) {
        items.push(
          <PaginationItem key={i}>
            <PaginationLink
              onClick={() => setPage(i)}
              isActive={page === i}
              className="cursor-pointer"
            >
              {i}
            </PaginationLink>
          </PaginationItem>
        );
      }

      if (page < totalPages - 2) {
        items.push(
          <PaginationItem key="ellipsis-end">
            <PaginationEllipsis />
          </PaginationItem>
        );
      }

      items.push(
        <PaginationItem key={totalPages}>
          <PaginationLink
            onClick={() => setPage(totalPages)}
            isActive={page === totalPages}
            className="cursor-pointer"
          >
            {totalPages}
          </PaginationLink>
        </PaginationItem>
      );
    }
    return items;
  };

  return (
    <div className="w-full p-6 space-y-2">
      <div className="flex flex-col gap-4 md:flex-row md:items-center md:justify-between">
        <div>
          <h1 className="text-3xl font-bold tracking-tight">Danh sách sinh viên</h1>
        </div>
      </div>  

      <Card>
        <CardContent className="pt-2">
          <form onSubmit={handleSearch} className="flex gap-2">
            <div className="relative flex-1">
              <Search className="absolute left-3 top-1/2 h-4 w-4 -translate-y-1/2 text-muted-foreground" />
              <Input
                type="text"
                placeholder="Tìm kiếm theo mã sinh viên, tên..."
                className="pl-10"
                value={keyword}
                onChange={(e) => setKeyword(e.target.value)}
              />
            </div>
            <Button type="submit">Tìm kiếm</Button>
          </form>
        </CardContent>
      </Card>

      <Card>
        <CardHeader>
          <CardTitle>Danh sách</CardTitle>
        </CardHeader>
        <CardContent>
          {loading ? (
            <div className="flex items-center justify-center py-12">
              <Loader2 className="h-8 w-8 animate-spin text-muted-foreground" />
            </div>
          ) : students.length === 0 ? (
            <div className="text-center py-12 text-muted-foreground">
              <p className="text-lg font-medium">Không tìm thấy sinh viên</p>
              <p className="text-sm mt-1">Thử thay đổi từ khóa tìm kiếm</p>
            </div>
          ) : (
            <>
              <div className="">
                <Table>
                  <TableHeader>
                    <TableRow>
                      <TableHead className="w-[120px]">Mã SV</TableHead>
                      <TableHead>Họ và tên</TableHead>
                      <TableHead>Lớp</TableHead>
                      <TableHead>Giới tính</TableHead>
                      <TableHead>Email</TableHead>
                      <TableHead>Số điện thoại</TableHead>
                      <TableHead>Trạng thái</TableHead>
                      <TableHead className="text-right">Thao tác</TableHead>
                    </TableRow>
                  </TableHeader>
                  <TableBody>
                    {students.map((student) => (
                      <TableRow key={student.StudentID}>
                        <TableCell className="font-medium">
                          {student.StudentCode}
                        </TableCell>
                        <TableCell>
                          <div className="font-medium">{student.Person.FullName}</div>
                          <div className="text-xs text-muted-foreground">
                            {formatDate(student.Person.DateOfBirth)}
                          </div>
                        </TableCell>
                        <TableCell>
                          <Badge variant="outline">{student.ClassName || "N/A"}</Badge>
                        </TableCell>
                        <TableCell>
                          <Badge variant={student.Person.Gender === "MALE" ? "default" : "secondary"}>
                            {student.Person.Gender === "MALE" ? "Nam" : "Nữ"}
                          </Badge>
                        </TableCell>
                        <TableCell className="text-sm text-muted-foreground">
                          {student.Person.Email || "N/A"}
                        </TableCell>
                        <TableCell className="text-sm text-muted-foreground">
                          {student.Person.PhoneNumber || "N/A"}
                        </TableCell>
                        <TableCell>
                          <StatusDisplay Status={student.Status} id={student.StudentID} />
                        </TableCell>
                        <TableCell className="text-right">
                          <Button variant="ghost" size="sm" asChild>
                            <Link href={`/student/${student.StudentID}`}>
                              <Eye className="mr-2 h-4 w-4" />
                            </Link>
                          </Button>
                        </TableCell>
                      </TableRow>
                    ))}
                  </TableBody>
                </Table>
              </div>

              <div className="mt-4">
                <Pagination>
                  <PaginationContent>
                    <PaginationItem>
                      <PaginationPrevious
                        onClick={() => setPage((p) => Math.max(1, p - 1))}
                        className={page === 1 ? "pointer-events-none opacity-50" : "cursor-pointer"}
                      />
                    </PaginationItem>
                    {renderPaginationItems()}
                    <PaginationItem>
                      <PaginationNext
                        onClick={() => setPage((p) => Math.min(totalPages, p + 1))}
                        className={page === totalPages ? "pointer-events-none opacity-50" : "cursor-pointer"}
                      />
                    </PaginationItem>
                  </PaginationContent>
                </Pagination>
              </div>
            </>
          )}
        </CardContent>
      </Card>
    </div>
  );
}