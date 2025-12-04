"use client";

import { useState, useEffect } from "react";
import { fetchRole, fetchRoleDeleted } from "../../lib/api";
import { useRouter } from "next/navigation";
import { Trash2, NotebookPen, Search, X, Info } from "lucide-react";
import { RoleDto } from "@/app/lib/types";
import {
  Pagination,
  PaginationContent,
  PaginationItem,
  PaginationLink,
  PaginationNext,
  PaginationPrevious,
} from "@/components/ui/pagination";

export default function RolesPage() {
  const [roles, setRoles] = useState<RoleDto[]>([]);
  const [allRoles, setAllRoles] = useState<RoleDto[]>([]);
  const [selectedRows, setSelectedRows] = useState<number[]>([]);
  const [currentPage, setCurrentPage] = useState(1);
  const [totalPages, setTotalPages] = useState(1);
  const [searchTerm, setSearchTerm] = useState('');
  const [searchInput, setSearchInput] = useState('');
  const [isLoading, setIsLoading] = useState(true);
  const router = useRouter();
  
  const itemsPerPage = 6;

  useEffect(() => {
    async function loadRoles() {
      setIsLoading(true);
      try {
        const data: RoleDto[] = await fetchRole();
        if (data) {
          setAllRoles(data);
        } else {
          setAllRoles([]);
          const token = localStorage.getItem('token');
          if (!token) {
            router.push('/login');
          }
        }
      } catch (error) {
        console.error("Error fetching roles:", error);
        setAllRoles([]);
      } finally {
        setIsLoading(false);
      }
    }
    loadRoles();
  }, [router]);

  useEffect(() => {
    const filteredData = allRoles.filter((role: RoleDto) =>
      role.RoleName.toLowerCase().includes(searchTerm.toLowerCase())
    );
    setTotalPages(Math.ceil(filteredData.length / itemsPerPage));
    setRoles(paginate(filteredData, currentPage, itemsPerPage));
  }, [currentPage, allRoles, searchTerm]);

  const paginate = (items: RoleDto[], page: number, pageSize: number) => {
    const startIndex = (page - 1) * pageSize;
    return items.slice(startIndex, startIndex + pageSize);
  };

  const handleSearch = () => {
    setCurrentPage(1);
    setSearchTerm(searchInput);
  };

  const handleClearSearch = () => {
    setSearchInput('');
    setCurrentPage(1);
    setSearchTerm('');
  };

  const handleSearchKeyPress = (e: React.KeyboardEvent) => {
    if (e.key === 'Enter') {
      handleSearch();
    }
  };

  const handleDeleteSingle = async (id: number) => {
    if (!window.confirm("Bạn có chắc muốn xóa mục này?")) {
      return;
    }
    try {
      const result = await fetchRoleDeleted(id);
      if (result) {
        const updatedRoles = allRoles.filter((role) => role.RoleID !== id);
        setAllRoles(updatedRoles);
        setSelectedRows((prev) => prev.filter((rowId) => rowId !== id));
      } else {
        alert("Đã xảy ra lỗi khi xóa mục này.");
      }
    } catch (error) {
      console.error(`Error deleting item ${id}:`, error);
      alert("Đã xảy ra lỗi khi xóa mục này.");
    }
  };

  const handleDeleteSelected = async () => {
    if (selectedRows.length === 0) return;
    if (
      !window.confirm(
        `Bạn có chắc muốn xóa ${selectedRows.length} mục đã chọn?`
      )
    ) {
      return;
    }
    try {
      const results = await Promise.all(
        selectedRows.map((id) => fetchRoleDeleted(id))
      );
      const allSuccess = results.every(result => result !== null);
      if (allSuccess) {
        const updatedRoles = allRoles.filter(
          (role) => !selectedRows.includes(role.RoleID)
        );
        setAllRoles(updatedRoles);
        setSelectedRows([]);
      } else {
        alert("Một số mục không thể xóa được.");
      }
    } catch (error) {
      console.error("Error deleting selected items:", error);
      alert("Đã xảy ra lỗi khi xóa hàng loạt.");
    }
  };

  const toggleRow = (id: number) => {
    setSelectedRows((prev) =>
      prev.includes(id) ? prev.filter((rowId) => rowId !== id) : [...prev, id]
    );
  };

  const toggleAll = () => {
    if (selectedRows.length === roles.length) {
      setSelectedRows([]);
    } else {
      setSelectedRows(roles.map((role) => role.RoleID));
    }
  };
  
  const getPageNumbers = () => {
    const pages: (number | string)[] = [];
    const maxVisible = 5;
    
    if (totalPages <= maxVisible) {
      for (let i = 1; i <= totalPages; i++) {
        pages.push(i);
      }
    } else {
      if (currentPage <= 3) {
        for (let i = 1; i <= 4; i++) {
          pages.push(i);
        }
        pages.push('ellipsis');
        pages.push(totalPages);
      } else if (currentPage >= totalPages - 2) {
        pages.push(1);
        pages.push('ellipsis');
        for (let i = totalPages - 3; i <= totalPages; i++) {
          pages.push(i);
        }
      } else {
        pages.push(1);
        pages.push('ellipsis');
        for (let i = currentPage - 1; i <= currentPage + 1; i++) {
          pages.push(i);
        }
        pages.push('ellipsis');
        pages.push(totalPages);
      }
    }
    
    return pages;
  };

  return (
    <>
      <div className="text-2xl font-bold py-2 px-4">
        Danh sách vai trò
      </div>
      <div className="w-full text-gray-900 p-2 bg-white min-h-screen">
        <div className="mb-4 flex items-center gap-3">
          <div className="flex-1 relative">
            <input
              type="text"
              value={searchInput}
              onChange={(e) => setSearchInput(e.target.value)}
              onKeyPress={handleSearchKeyPress}
              placeholder="Tìm kiếm vai trò..."
              className="w-full pl-10 pr-10 py-2 border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-gray-600 focus:border-transparent"
            />
            <Search className="absolute left-3 top-1/2 -translate-y-1/2 text-gray-400" size={20} />
            {searchInput && (
              <button
                onClick={handleClearSearch}
                className="absolute right-3 top-1/2 -translate-y-1/2 text-gray-400 hover:text-gray-600"
              >
                <X size={20} />
              </button>
            )}
          </div>
           <button
            onClick={handleSearch}
            className="bg-black hover:bg-gray-800 text-white font-medium py-2 px-6 rounded-lg transition-colors flex items-center gap-2"
          >
            <Search size={18} />
            Tìm kiếm
          </button>
          <button
            onClick={() => router.push('/admin/role/created')}
            className="bg-black hover:bg-gray-800 text-white font-medium py-2 px-6 rounded-lg transition-colors flex items-center gap-2"
          >
            Tạo mới
          </button>
        </div>
        <div className="mb-4 flex justify-between items-center">
          {selectedRows.length > 0 && (
            <button
              onClick={handleDeleteSelected}
              className="flex items-center gap-2 bg-red-600 hover:bg-red-700 text-white font-medium py-2 px-4 rounded-lg transition-colors"
            >
              <Trash2 size={18} />
              Xóa {selectedRows.length} mục đã chọn
            </button>
          )}
        </div>

        {isLoading ? (
          <div className="flex justify-center items-center py-12">
            <div className="animate-spin rounded-full h-12 w-12 border-b-2 border-blue-600"></div>
          </div>
        ) : roles.length === 0 ? (
          <div className="text-center py-12 text-gray-500">
            <p className="text-lg">Không tìm thấy vai trò nào</p>
            {searchTerm && (
              <button
                onClick={handleClearSearch}
                className="mt-4 text-blue-600 hover:text-blue-700 underline"
              >
                Xóa bộ lọc tìm kiếm
              </button>
            )}
          </div>
        ) : (
          <div className="overflow-x-auto rounded-lg border border-gray-200 shadow-sm">
            <table className="w-full border-collapse">
              <thead>
                <tr className="border-b border-gray-200 bg-gray-50">
                  <th className="text-left p-4 font-normal text-gray-600">
                    <input
                      type="checkbox"
                      checked={
                        selectedRows.length === roles.length && roles.length > 0
                      }
                      onChange={toggleAll}
                      className="w-4 h-4 cursor-pointer accent-blue-600"
                    />
                  </th>
                  <th className="text-left p-4 font-normal text-gray-600">
                    Mã vai trò
                  </th>
                  <th className="text-left p-4 font-normal text-gray-600">
                    Tên vai trò
                  </th>
                  <th className="text-left p-4 font-normal text-gray-600">
                    Mô tả
                  </th>
                  <th className="text-left p-4 font-normal text-gray-600">
                    Ngày tạo
                  </th>
                  <th className="text-left p-4 font-normal text-gray-600">
                    Chức năng
                  </th>
                </tr>
              </thead>
              <tbody>
                {roles.map((role) => (
                  <tr
                    key={role.RoleID}
                    className={`border-b border-gray-100 transition-colors hover:bg-gray-50`}
                  >
                    <td className="p-4">
                      <input
                        type="checkbox"
                        checked={selectedRows.includes(role.RoleID)}
                        onChange={() => toggleRow(role.RoleID)}
                        className="w-4 h-4 cursor-pointer accent-blue-600"
                      />
                    </td>
                    <td className="p-4 font-medium text-gray-900">
                      {role.RoleCode}
                    </td>
                    <td className="p-4 font-medium text-gray-900">
                      {role.RoleName}
                    </td>
                    <td className="p-4 font-medium text-gray-900">
                      {role.Description}
                    </td>
                    <td className="p-4 text-gray-700 text-sm">
                      {new Date(role.CreatedAt).toLocaleDateString("vi-VN")}
                    </td>
                    <td className="p-4">
                      <div className="flex gap-2">
                        <button
                          onClick={() =>
                            router.push(`/admin/role/detail/${role.RoleID}`)
                          }
                          className="text-blue-600 hover:text-blue-800 hover:bg-blue-100 p-1 rounded transition-colors font-medium"
                          title="Chi tiết"
                        >
                          <Info size={18} />
                        </button>
                        <button
                          onClick={() =>
                            router.push(`/admin/role/edit?id=${role.RoleID}`)
                          }
                          className="text-green-600 hover:text-green-800 hover:bg-green-100 p-1 rounded transition-colors font-medium"
                          title="Chỉnh sửa"
                        >
                          <NotebookPen size={18} />
                        </button>
                        <button
                          onClick={() => handleDeleteSingle(role.RoleID)}
                          className="text-red-600 hover:text-red-800 hover:bg-red-100 p-1 rounded transition-colors font-medium"
                          title="Xóa"
                        >
                          <Trash2 size={18} />
                        </button>
                      </div>
                    </td>
                  </tr>
                ))}
              </tbody>
            </table>
          </div>
        )}

        {totalPages > 1 && (
          <Pagination className="mt-6">
            <PaginationContent>
              <PaginationItem>
                <PaginationPrevious 
                  href="#"
                  onClick={(e) => {
                    e.preventDefault();
                    if (currentPage > 1) {
                      setCurrentPage(prev => prev - 1);
                    }
                  }}
                  className={currentPage === 1 ? "pointer-events-none opacity-50" : "cursor-pointer"}
                />
              </PaginationItem>
              
              {getPageNumbers().map((page, index) => (
                <PaginationItem key={index}>
                  {page === 'ellipsis' ? (
                    <span className="px-4 py-2">...</span>
                  ) : (
                    <PaginationLink
                      href="#"
                      onClick={(e) => {
                        e.preventDefault();
                        setCurrentPage(page as number);
                      }}
                      isActive={currentPage === page}
                      className="cursor-pointer"
                    >
                      {page}
                    </PaginationLink>
                  )}
                </PaginationItem>
              ))}
              
              <PaginationItem>
                <PaginationNext 
                  href="#"
                  onClick={(e) => {
                    e.preventDefault();
                    if (currentPage < totalPages) {
                      setCurrentPage(prev => prev + 1);
                    }
                  }}
                  className={currentPage === totalPages ? "pointer-events-none opacity-50" : "cursor-pointer"}
                />
              </PaginationItem>
            </PaginationContent>
          </Pagination>
        )}
      </div>
    </>
  );
}
