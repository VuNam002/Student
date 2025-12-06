"use client";

import { useEffect, useState } from 'react';
import { Button } from "@/components/ui/button";
import {
  Card,
  CardContent,
  CardDescription,
  CardHeader,
  CardTitle,
} from "@/components/ui/card";
import {
  Table,
  TableBody,
  TableCell,
  TableHead,
  TableHeader,
  TableRow,
} from "@/components/ui/table";
import { Checkbox } from "@/components/ui/checkbox";
import { MoreHorizontal, PlusCircle, Trash2 } from "lucide-react";
import {
    Dialog,
    DialogContent,
    DialogDescription,
    DialogFooter,
    DialogHeader,
    DialogTitle,
    DialogTrigger,
  } from "@/components/ui/dialog"
import { Input } from "@/components/ui/input"
import { Label } from "@/components/ui/label"
import { PermissionGroupDto, PermissionItemDto } from '@/app/lib/types';
import { fetchAllPermissions, deletePermission, fetchPermissionCreate } from '@/app/lib/api';

export default function PermissionsPage() {
    const [permissionModules, setPermissionModules] = useState<PermissionGroupDto[]>([]);
    const [loading, setLoading] = useState(true);
    const [selectedPermissions, setSelectedPermissions] = useState<number[]>([]);
    const [isDialogOpen, setIsDialogOpen] = useState(false);
    const [newPermission, setNewPermission] = useState({
        PermissionName: '',
        PermissionCode: '',
        Module: '',
        Description: '',
    });

    const fetchPermissions = async () => {
        try {
            setLoading(true);
            const data = await fetchAllPermissions();
            if (data) {
                setPermissionModules(data);
            }
        } catch (error) {
            console.error("Failed to fetch permissions", error);
        } finally {
            setLoading(false);
        }
    };

    useEffect(() => {
        fetchPermissions();
    }, []);

    const handlePermissionChange = (permissionId: number, checked: boolean) => {
        setSelectedPermissions(prev => 
          checked ? [...prev, permissionId] : prev.filter(id => id !== permissionId)
        );
    };
    
    const handleSelectAllModule = (modulePermissions: PermissionItemDto[], checked: boolean) => {
        const modulePermissionIds = modulePermissions.map(p => p.PermissionID);
        if (checked) {
          setSelectedPermissions(prev => [...new Set([...prev, ...modulePermissionIds])]);
        } else {
          setSelectedPermissions(prev => prev.filter(id => !modulePermissionIds.includes(id)));
        }
    };

    const handleDeleteSelected = async () => {
        if (selectedPermissions.length === 0) return;
        if (!confirm(`Bạn có chắc chắn muốn xoá ${selectedPermissions.length} quyền đã chọn không?`)) return;

        try {
            await Promise.all(selectedPermissions.map(id => deletePermission(id)));
            alert("Đã xoá các quyền thành công!");
            setSelectedPermissions([]);
            fetchPermissions(); // Refresh the list
        } catch (error) {
            console.error("Failed to delete permissions", error);
            alert("Đã xảy ra lỗi khi xoá quyền.");
        }
    };

    const handleInputChange = (e: React.ChangeEvent<HTMLInputElement>) => {
        const { id, value } = e.target;
        setNewPermission(prev => ({ ...prev, [id]: value }));
    };

    const handleSubmit = async () => {
        if (!newPermission.PermissionName || !newPermission.PermissionCode || !newPermission.Module) {
            alert('Vui lòng điền các trường bắt buộc: Tên quyền, Mã quyền, Module.');
            return;
        }
        try {
            await fetchPermissionCreate(newPermission);
            alert('Thêm quyền mới thành công!');
            setIsDialogOpen(false);
            fetchPermissions();
            setNewPermission({ PermissionName: '', PermissionCode: '', Module: '', Description: '' });
        } catch (error) {
            console.error("Failed to create permission", error);
            alert('Đã có lỗi xảy ra khi thêm quyền.');
        }
    };

    return (
        <main className="grid flex-1 items-start gap-4 sm:px-6 sm:py-0 md:gap-8">
            <div className="flex items-center">
                <h1 className="text-2xl font-bold">Quản lý Quyền</h1>
                <div className="ml-auto flex items-center gap-2">
                    {selectedPermissions.length > 0 && (
                        <Button variant="destructive" size="sm" className="h-8 gap-1" onClick={handleDeleteSelected}>
                            <Trash2 className="h-3.5 w-3.5" />
                            <span className="sr-only sm:not-sr-only sm:whitespace-nowrap">
                                Xoá ({selectedPermissions.length})
                            </span>
                        </Button>
                    )}
                    <Dialog open={isDialogOpen} onOpenChange={setIsDialogOpen}>
                        <DialogTrigger asChild>
                            <Button size="sm" className="h-8 gap-1">
                                <PlusCircle className="h-3.5 w-3.5" />
                                <span className="sr-only sm:not-sr-only sm:whitespace-nowrap">
                                    Thêm quyền mới
                                </span>
                            </Button>
                        </DialogTrigger>
                        <DialogContent className="sm:max-w-[425px]">
                            <DialogHeader>
                                <DialogTitle>Thêm quyền mới</DialogTitle>
                                <DialogDescription>
                                    Điền thông tin chi tiết cho quyền mới tại đây.
                                </DialogDescription>
                            </DialogHeader>
                            <div className="grid gap-4 py-4">
                               <div className="grid grid-cols-4 items-center gap-4">
                                    <Label htmlFor="PermissionName" className="text-right">Tên quyền</Label>
                                    <Input id="PermissionName" value={newPermission.PermissionName} onChange={handleInputChange} className="col-span-3" />
                                </div>
                                <div className="grid grid-cols-4 items-center gap-4">
                                    <Label htmlFor="PermissionCode" className="text-right">Mã quyền</Label>
                                    <Input id="PermissionCode" value={newPermission.PermissionCode} onChange={handleInputChange} className="col-span-3" />
                                </div>
                                <div className="grid grid-cols-4 items-center gap-4">
                                    <Label htmlFor="Module" className="text-right">Module</Label>
                                    <Input id="Module" value={newPermission.Module} onChange={handleInputChange} className="col-span-3" />
                                </div>
                                <div className="grid grid-cols-4 items-center gap-4">
                                    <Label htmlFor="Description" className="text-right">Mô tả</Label>
                                    <Input id="Description" value={newPermission.Description} onChange={handleInputChange} className="col-span-3" />
                                </div>
                            </div>
                            <DialogFooter>
                                <Button onClick={handleSubmit}>Lưu lại</Button>
                            </DialogFooter>
                        </DialogContent>
                    </Dialog>
                </div>
            </div>
            {loading ? (
                <div className="text-center">Đang tải...</div>
            ) : (
                permissionModules.map((moduleGroup) => {
                    const modulePermissions = moduleGroup.Permissions || [];
                    const allInModuleSelected = modulePermissions.length > 0 && modulePermissions.every(p => selectedPermissions.includes(p.PermissionID));
                    const someInModuleSelected = modulePermissions.some(p => selectedPermissions.includes(p.PermissionID));

                    return (
                        <Card key={moduleGroup.Module}>
                            <CardHeader>
                                <div className="flex items-center justify-between">
                                    <CardTitle>{moduleGroup.Module}</CardTitle>
                                    <div className="flex items-center space-x-2">
                                        <Checkbox
                                            id={`select-all-${moduleGroup.Module}`}
                                            checked={allInModuleSelected ? true : someInModuleSelected ? 'indeterminate' : false}
                                            onCheckedChange={(checked) => handleSelectAllModule(modulePermissions, !!checked)}
                                        />
                                        <label htmlFor={`select-all-${moduleGroup.Module}`} className="font-medium text-sm">
                                            Chọn tất cả
                                        </label>
                                    </div>
                                </div>
                                <CardDescription>
                                    Các quyền liên quan đến module {moduleGroup.Module}.
                                </CardDescription>
                            </CardHeader>
                            <CardContent>
                                <Table>
                                    <TableHeader>
                                        <TableRow>
                                            <TableHead className="w-[50px]"></TableHead>
                                            <TableHead>Tên quyền</TableHead>
                                            <TableHead>Mã quyền</TableHead>
                                            <TableHead>Mô tả</TableHead>
                                            <TableHead>
                                                <span className="sr-only">Actions</span>
                                            </TableHead>
                                        </TableRow>
                                    </TableHeader>
                                    <TableBody>
                                        {modulePermissions.map((permission) => (
                                            <TableRow key={permission.PermissionID}>
                                                <TableCell>
                                                    <Checkbox
                                                        id={`perm-${permission.PermissionID}`}
                                                        checked={selectedPermissions.includes(permission.PermissionID)}
                                                        onCheckedChange={(checked) => handlePermissionChange(permission.PermissionID, !!checked)}
                                                    />
                                                </TableCell>
                                                <TableCell className="font-medium">{permission.PermissionName}</TableCell>
                                                <TableCell>{permission.PermissionCode}</TableCell>
                                                <TableCell>{permission.Description}</TableCell>
                                                <TableCell>
                                                    <Button aria-haspopup="true" size="icon" variant="ghost">
                                                        <MoreHorizontal className="h-4 w-4" />
                                                        <span className="sr-only">Toggle menu</span>
                                                    </Button>
                                                </TableCell>
                                            </TableRow>
                                        ))}
                                    </TableBody>
                                </Table>
                            </CardContent>
                        </Card>
                    );
                })
            )}
        </main>
    );
}
