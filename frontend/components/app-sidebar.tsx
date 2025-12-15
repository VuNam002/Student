"use client"

import * as React from "react"
import {
  Shield ,
  Command,
  Frame,
  LifeBuoy,
  Map,
  PieChart,
  Send,
  Settings2,
  LayoutDashboard,
  CircleUserRound,
  SquareUser,
  School  ,
} from "lucide-react"

import { NavMain } from "../components/nav-main"
import { NavProjects } from "../components/nav-projects"
import { NavSecondary } from "../components/nav-secondary"
import { NavUser } from "../components/nav-user"
import {
  Sidebar,
  SidebarContent,
  SidebarFooter,
  SidebarHeader,
  SidebarMenu,
  SidebarMenuButton,
  SidebarMenuItem,
} from "../components/ui/sidebar"
import { fetchAccountMe } from "../app/lib/api"
import { AccountDetail } from "@/app/lib/types";
import { useEffect, useState } from "react";

const navMain = [
  {
    title:" Dashboard",
    url: "/admin/dashboard",
    icon: LayoutDashboard ,
  },
  {
    title: "Tài Khoản",
    url: "#",
    icon: CircleUserRound ,
    items: [
      {
        title: "Danh sách tài khoản",
        url: "/admin/account",
      },
      {
        title: "Thêm tài khoản",
        url: "/admin/account/created",
      }
    ],
  },
  {
    title: "Quyền",
    url: "#",
    icon: Shield ,
    items: [
      {
        title: "Danh sách quyền",
        url: "/admin/role",
      },
      {
        title: "Thêm quyền",
        url: "/admin/role/created",
      },
      {
        title: "Phân quyền",
        url: "/admin/permission",
      },
      {
        title: "Thêm phân quyền",
        url: "/admin/permission/created",
      },
    ],
  },
  {
    title:"Sinh viên",
    url: "#",
    icon: SquareUser,
    items: [
      {
        title: "Danh sách sinh viên",
        url: "/admin/student",
      },
      {
        title: "Thêm sinh viên",
        url: "/admin/student/created",
      } 
    ],
  },
  {
    title: "Lớp",
    url: "#",
    icon: School ,
    items: [
      {
        title: "Danh sách lớp",
        url: "/admin/class",
      },
      {
        title: "Thêm lớp",
        url: "/admin/class/created",
      }
    ]
  },
  {
    title: "Settings",
    url: "#",
    icon: Settings2,
    items: [
      {
        title: "General",
        url: "#",
      },
      {
        title: "Team",
        url: "#",
      },
      {
        title: "Billing",
        url: "#",
      },
      {
        title: "Limits",
        url: "#",
      },
    ],
  },
]
const navSecondary = [
  {
    title: "Support",
    url: "#",
    icon: LifeBuoy,
  },
  {
    title: "Feedback",
    url: "#",
    icon: Send,
  },
]

const projects = [
  {
    name: "Design Engineering",
    url: "#",
    icon: Frame,
  },
  {
    name: "Sales & Marketing",
    url: "#",
    icon: PieChart,
  },
  {
    name: "Travel",
    url: "#",
    icon: Map,
  },
]

export function AppSidebar({ ...props }: React.ComponentProps<typeof Sidebar>) {
  const [user, setUser] = useState<AccountDetail | null>(null);
  const [loading, setLoading] = useState(true);
  useEffect(() => {
    const getAccountMe = async () => {
      const account = await fetchAccountMe();
      setUser(account);
      setLoading(false);
    };
    getAccountMe();
  }, []);
  return (
    <Sidebar variant="inset" {...props}>
      <SidebarHeader>
        <SidebarMenu>
          <SidebarMenuItem>
            <SidebarMenuButton size="lg" asChild>
              <a href="/admin/dashboard" className="flex items-center gap-3">
                <div className="bg-sidebar-primary text-sidebar-primary-foreground flex aspect-square size-8 items-center justify-center rounded-lg">
                  <Command className="size-4" />
                </div>
                <div className="grid flex-1 text-left text-sm leading-tight">
                  <span className="truncate font-medium">Quản lý sinh viên</span>
                  <span className="truncate text-xs">School</span>
                </div>
              </a>
            </SidebarMenuButton>
          </SidebarMenuItem>
        </SidebarMenu>
      </SidebarHeader>
      <SidebarContent>
        <NavMain items={navMain} />
        <NavProjects projects={projects} />
        <NavSecondary items={navSecondary} className="mt-auto" />
      </SidebarContent>
      <SidebarFooter>
        {loading ? (
          <div>Loading...</div>
        ) : user ? (
          <NavUser user={{ name: user.RoleName??null, email: user.Email, avatar: user.Avatar ?? null }} />
        ) : (
          <div>No user</div>
        )}
      </SidebarFooter>
    </Sidebar>
  )
}