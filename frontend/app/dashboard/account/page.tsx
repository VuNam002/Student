"use client";

import { useState, useEffect } from "react";
import { fetchAccount, fetchAccountDeleted } from "../../lib/api";
import { useRouter } from "next/navigation";
import { CheckCircle, XCircle, Trash2, NotebookPen, Search, X } from "lucide-react";

export default function ItemsPage() {
  const [items, setItems] = useState<any[]>([]);
  const [filteredItems, setFilteredItems] = useState<any[]>([]);
  const [selectedRows, setSelectedRows] = useState<number[]>([]);
  const [currentPage, setCurrentPage] = useState(1);
  const [searchTerm, setSearchTerm] = useState('');
  const [searchInput, setSearchInput] = useState('');
  const [isLoading, setIsLoading] = useState(true);
  const router = useRouter();
  
  const itemsPerPage = 10;

  useEffect(() => {
    async function loadItems() {
      setIsLoading(true);
      try {
        const data = await fetchAccount();
        if (data) {
          setItems(data);
          setFilteredItems(data);
        } else {
          setItems([]);
          setFilteredItems([]);
          const token = localStorage.getItem('token');
          if (!token) {
            router.push('/login');
          }
        }
      } catch (error) {
        console.error("Error fetching items:", error);
        setItems([]);
        setFilteredItems([]);
      } finally {
        setIsLoading(false);
      }
    }
    loadItems();
  }, [router]);

  // Xử lý tìm kiếm và phân trang phía client
  useEffect(() => {
    if (searchTerm) {
      const filtered = items.filter(item => 
        item.name?.toLowerCase().includes(searchTerm.toLowerCase()) ||
        item.description?.toLowerCase().includes(searchTerm.toLowerCase())
      );
      setFilteredItems(filtered);
    } else {
      setFilteredItems(items);
    }
    setCurrentPage(1); // Reset về trang 1 khi search
  }, [searchTerm, items]);

  // Tính toán pagination
  const totalPages = Math.ceil(filteredItems.length / itemsPerPage);
  const startIndex = (currentPage - 1) * itemsPerPage;
  const endIndex = startIndex + itemsPerPage;
  const currentItems = filteredItems.slice(startIndex, endIndex);

  const handleSearch = () => {
    setSearchTerm(searchInput);
  };

  const handleClearSearch = () => {
    setSearchInput('');
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
      const result = await fetchAccountDeleted(id);
      if (result) {
        setItems((currentItems) =>
          currentItems.filter((item) => item.item_id !== id)
        );
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
        selectedRows.map((id) => fetchAccountDeleted(id))
      );
      const allSuccess = results.every(result => result !== null);
      if (allSuccess) {
        setItems((currentItems) =>
          currentItems.filter((item) => !selectedRows.includes(item.item_id))
        );
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
    if (selectedRows.length === currentItems.length) {
      setSelectedRows([]);
    } else {
      setSelectedRows(currentItems.map((item) => item.item_id));
    }
  };

  const StatusDisplay = ({ isActive }: { isActive: boolean }) => {
    return (
      <div className="flex items-center gap-2">
        {isActive ? (
          <CheckCircle size={18} className="text-emerald-600" />
        ) : (
          <XCircle size={18} className="text-rose-600" />
        )}
        <span
          className={isActive ? "text-gray-900 font-medium" : "text-gray-700"}
        >
          {isActive ? "Hoạt động" : "Tạm dừng"}
        </span>
      </div>
    );
  };

  return (
    <>
      <div className="text-2xl font-bold py-2 px-4">
        Danh sách tài khoản
      </div>
      <div className="w-full text-gray-900 p-2 bg-white min-h-screen">
        {/* Search Bar */}
        <div className="mb-4 flex items-center gap-3">
          <div className="flex-1 relative">
            <input
              type="text"
              value={searchInput}
              onChange={(e) => setSearchInput(e.target.value)}
              onKeyPress={handleSearchKeyPress}
              placeholder="Tìm kiếm theo tên sản phẩm..."
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
        </div>
        <div className="mb-4 flex justify-between items-center">
          <div className="text-sm text-gray-600">
            {searchTerm && (
              <span>
                Kết quả tìm kiếm cho: <span className="font-semibold">"{searchTerm}"</span>
                {' '}({filteredItems.length} kết quả)
              </span>
            )}
          </div>
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
        ) : currentItems.length === 0 ? (
          <div className="text-center py-12 text-gray-500">
            <p className="text-lg">Không tìm thấy sản phẩm nào</p>
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
                        selectedRows.length === currentItems.length && currentItems.length > 0
                      }
                      onChange={toggleAll}
                      className="w-4 h-4 cursor-pointer accent-blue-600"
                    />
                  </th>
                  <th className="text-left p-4 font-normal text-gray-600">
                    Avatar
                  </th>
                  <th className="text-left p-4 font-normal text-gray-600">
                    Chức vụ
                  </th>
                  <th className="text-left p-4 font-normal text-gray-600">
                    Email
                  </th>
                  <th className="text-left p-4 font-normal text-gray-600">
                    Trạng thái
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
                {currentItems.map((row, index) => (
                  <tr
                    key={row.item_id}
                    className={`border-b border-gray-100 transition-colors hover:bg-gray-50 ${
                      index % 2 === 0 ? "bg-white" : "bg-gray-50/30"
                    }`}
                  >
                    <td className="p-4">
                      <input
                        type="checkbox"
                        checked={selectedRows.includes(row.item_id)}
                        onChange={() => toggleRow(row.item_id)}
                        className="w-4 h-4 cursor-pointer accent-blue-600"
                      />
                    </td>
                    
                    <td className="p-4 font-medium text-gray-900">
                      <div className="flex items-center gap-3">
                        {row.Avatar && (
                          <img
                            src={row.Avatar}
                            alt={row.Avatar}
                            className="w-10 h-10 rounded-lg object-cover"
                          />
                        )}
                        <span>{row.Avatar}</span>
                      </div>
                    </td>
                    <td className="p-4 text-gray-700 max-w-xs truncate">
                      {row.TenHienThi}
                    </td>
                 
                    <td className="p-4">
                      <span className="px-3 py-1 rounded-lg text-sm text-gray-700">
                        {row.Email}
                      </span>
                    </td>
                    <td className="p-4">
                      <StatusDisplay isActive={row.TrangThai} />
                    </td>
                    <td className="p-4 text-gray-700 text-sm">
                      {new Date(row.NgayTao).toLocaleDateString("vi-VN")}
                    </td>

                    <td className="p-4">
                      <div className="flex gap-2">
                        <button
                          onClick={() => handleDeleteSingle(row.item_id)}
                          className="text-red-600 hover:text-red-800 hover:bg-red-100 p-1 rounded transition-colors font-medium"
                          title="Xóa"
                        >
                          <Trash2 size={18} />
                        </button>
                        <button
                          onClick={() =>
                            router.push(`/product/edit?id=${row.item_id}`)
                          }
                          className="text-green-600 hover:text-green-800 hover:bg-green-100 p-1 rounded transition-colors font-medium"
                          title="Chỉnh sửa"
                        >
                          <NotebookPen size={18} />
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
          <div className="mt-4 flex justify-center gap-2">
            <button
              onClick={() => setCurrentPage(prev => Math.max(1, prev - 1))}
              disabled={currentPage === 1}
              className="px-4 py-2 border border-gray-300 rounded-lg hover:bg-gray-50 disabled:opacity-50 disabled:cursor-not-allowed transition-colors"
            >
              Trước
            </button>
            <span className="px-4 py-2">
              Trang {currentPage} / {totalPages}
            </span>
            <button
              onClick={() => setCurrentPage(prev => Math.min(totalPages, prev + 1))}
              disabled={currentPage === totalPages}
              className="px-4 py-2 border border-gray-300 rounded-lg hover:bg-gray-50 disabled:opacity-50 disabled:cursor-not-allowed transition-colors"
            >
              Sau
            </button>
          </div>
        )}
      </div>
    </>
  );
}
