import { useState, useEffect } from "react";
import { Plus, Edit, Trash2, RefreshCw, Key, UserCheck, UserX } from "lucide-react";
import { Button } from "@/app/components/ui/button";
import { Card, CardContent, CardHeader, CardTitle } from "@/app/components/ui/card";
import { api, User } from "@/app/lib/api";
import { toast } from "sonner";
import {
  Table,
  TableBody,
  TableCell,
  TableHead,
  TableHeader,
  TableRow,
} from "@/app/components/ui/table";
import {
  Dialog,
  DialogContent,
  DialogHeader,
  DialogTitle,
  DialogFooter,
} from "@/app/components/ui/dialog";
import { Input } from "@/app/components/ui/input";
import { Label } from "@/app/components/ui/label";
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from "@/app/components/ui/select";
import { Theme } from "@/app/App";

export default function UsersPage({ theme }: { theme: Theme }) {
  const [users, setUsers] = useState<User[]>([]);
  const [showDialog, setShowDialog] = useState(false);
  const [editingUser, setEditingUser] = useState<User | null>(null);

  useEffect(() => {
    loadUsers();
  }, []);

  const loadUsers = async () => {
    try {
      const data = await api.getUsers();
      setUsers(data);
    } catch (error) {
      toast.error("Failed to load users");
    }
  };

  const handleDelete = async (id: number) => {
    if (confirm("Are you sure you want to delete this user?")) {
      try {
        await api.deleteUser(id);
        await loadUsers();
        toast.success("User deleted successfully");
      } catch (error) {
        toast.error("Failed to delete user");
      }
    }
  };

  const handleToggleActive = async (user: User) => {
    try {
      await api.updateUser(user.id, { ...user, isActive: !user.isActive });
      await loadUsers();
      toast.success(
        `User ${user.isActive ? "deactivated" : "activated"} successfully`
      );
    } catch (error) {
      toast.error("Failed to update user status");
    }
  };

  const handleResetPassword = async (user: User) => {
    if (confirm(`Reset password for ${user.username}?`)) {
      toast.success("Password reset email sent (mock)");
    }
  };

  return (
    <div className="space-y-6">
      {/* Header */}
      <div className="flex items-center justify-between">
        <div>
          <h1 className={`mb-2 ${theme === "dark" ? "text-white" : "text-gray-900"}`}>Users Management</h1>
          <p className={theme === "dark" ? "text-gray-400" : "text-gray-600"}>Manage system users and their access levels</p>
        </div>
        <div className="flex gap-2">
          <Button variant="outline" onClick={loadUsers}>
            <RefreshCw className="mr-2 h-4 w-4" />
            Refresh
          </Button>
          <Button
            onClick={() => {
              setEditingUser(null);
              setShowDialog(true);
            }}
          >
            <Plus className="mr-2 h-4 w-4" />
            Create User
          </Button>
        </div>
      </div>

      {/* Users Table */}
      <Card className={theme === "dark" ? "border-gray-700 bg-[#1F2937]" : theme === "gray" ? "bg-white" : ""}>
        <CardHeader>
          <CardTitle className={theme === "dark" ? "text-white" : ""}>All Users</CardTitle>
        </CardHeader>
        <CardContent>
          <Table>
            <TableHeader>
              <TableRow className={theme === "dark" ? "border-gray-700" : ""}>
                <TableHead className={theme === "dark" ? "text-gray-300" : ""}>Username</TableHead>
                <TableHead className={theme === "dark" ? "text-gray-300" : ""}>Full Name</TableHead>
                <TableHead className={theme === "dark" ? "text-gray-300" : ""}>Email</TableHead>
                <TableHead className={theme === "dark" ? "text-gray-300" : ""}>Role</TableHead>
                <TableHead className={theme === "dark" ? "text-gray-300" : ""}>Status</TableHead>
                <TableHead className={theme === "dark" ? "text-gray-300" : ""}>Actions</TableHead>
              </TableRow>
            </TableHeader>
            <TableBody>
              {users.map((user) => (
                <TableRow key={user.id} className={theme === "dark" ? "border-gray-700" : ""}>
                  <TableCell className={`font-medium ${theme === "dark" ? "text-gray-300" : ""}`}>{user.username}</TableCell>
                  <TableCell className={theme === "dark" ? "text-gray-300" : ""}>{user.fullName}</TableCell>
                  <TableCell className={theme === "dark" ? "text-gray-300" : ""}>{user.email}</TableCell>
                  <TableCell>
                    <span
                      className={`inline-block rounded-full px-3 py-1 text-xs ${
                        user.role === "Admin"
                          ? "bg-red-100 text-red-800"
                          : user.role === "Manager"
                          ? "bg-blue-100 text-blue-800"
                          : "bg-gray-100 text-gray-800"
                      }`}
                    >
                      {user.role}
                    </span>
                  </TableCell>
                  <TableCell>
                    <span
                      className={`inline-block rounded-full px-3 py-1 text-xs ${
                        user.isActive
                          ? "bg-green-100 text-green-800"
                          : "bg-red-100 text-red-800"
                      }`}
                    >
                      {user.isActive ? "Active" : "Inactive"}
                    </span>
                  </TableCell>
                  <TableCell>
                    <div className="flex gap-2">
                      <Button
                        size="sm"
                        variant="ghost"
                        onClick={() => {
                          setEditingUser(user);
                          setShowDialog(true);
                        }}
                      >
                        <Edit className="h-4 w-4" />
                      </Button>
                      <Button
                        size="sm"
                        variant="ghost"
                        onClick={() => handleToggleActive(user)}
                      >
                        {user.isActive ? (
                          <UserX className="h-4 w-4 text-orange-500" />
                        ) : (
                          <UserCheck className="h-4 w-4 text-green-500" />
                        )}
                      </Button>
                      <Button
                        size="sm"
                        variant="ghost"
                        onClick={() => handleResetPassword(user)}
                      >
                        <Key className="h-4 w-4 text-blue-500" />
                      </Button>
                      <Button
                        size="sm"
                        variant="ghost"
                        onClick={() => handleDelete(user.id)}
                      >
                        <Trash2 className="h-4 w-4 text-red-500" />
                      </Button>
                    </div>
                  </TableCell>
                </TableRow>
              ))}
            </TableBody>
          </Table>
        </CardContent>
      </Card>

      <UserDialog
        open={showDialog}
        onClose={() => setShowDialog(false)}
        user={editingUser}
        onSave={loadUsers}
        theme={theme}
      />
    </div>
  );
}

function UserDialog({
  open,
  onClose,
  user,
  onSave,
  theme,
}: {
  open: boolean;
  onClose: () => void;
  user: User | null;
  onSave: () => void;
  theme: Theme;
}) {
  const [formData, setFormData] = useState({
    username: "",
    fullName: "",
    email: "",
    role: "Cashier" as "Admin" | "Manager" | "Cashier",
    isActive: true,
    password: "",
  });

  useEffect(() => {
    if (user) {
      setFormData({
        username: user.username,
        fullName: user.fullName,
        email: user.email,
        role: user.role,
        isActive: user.isActive,
        password: "",
      });
    } else {
      setFormData({
        username: "",
        fullName: "",
        email: "",
        role: "Cashier",
        isActive: true,
        password: "",
      });
    }
  }, [user, open]);

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    try {
      if (user) {
        await api.updateUser(user.id, formData);
        toast.success("User updated successfully");
      } else {
        await api.createUser(formData);
        toast.success("User created successfully");
      }
      onSave();
      onClose();
    } catch (error) {
      toast.error("Failed to save user");
    }
  };

  return (
    <Dialog open={open} onOpenChange={(isOpen) => !isOpen && onClose()}>
      <DialogContent className={`max-w-md ${theme === "dark" ? "border-gray-700 bg-[#1F2937] text-white" : ""}`}>
        <DialogHeader>
          <DialogTitle className={theme === "dark" ? "text-white" : ""}>{user ? "Edit User" : "Create User"}</DialogTitle>
        </DialogHeader>
        <form onSubmit={handleSubmit} className="space-y-4">
          <div>
            <Label>Username</Label>
            <Input
              value={formData.username}
              onChange={(e) => setFormData({ ...formData, username: e.target.value })}
              required
            />
          </div>
          <div>
            <Label>Full Name</Label>
            <Input
              value={formData.fullName}
              onChange={(e) => setFormData({ ...formData, fullName: e.target.value })}
              required
            />
          </div>
          <div>
            <Label>Email</Label>
            <Input
              type="email"
              value={formData.email}
              onChange={(e) => setFormData({ ...formData, email: e.target.value })}
              required
            />
          </div>
          {!user && (
            <div>
              <Label>Password</Label>
              <Input
                type="password"
                value={formData.password}
                onChange={(e) => setFormData({ ...formData, password: e.target.value })}
                required={!user}
              />
            </div>
          )}
          <div>
            <Label>Role</Label>
            <Select
              value={formData.role}
              onValueChange={(value: any) => setFormData({ ...formData, role: value })}
            >
              <SelectTrigger>
                <SelectValue />
              </SelectTrigger>
              <SelectContent>
                <SelectItem value="Admin">Admin</SelectItem>
                <SelectItem value="Manager">Manager</SelectItem>
                <SelectItem value="Cashier">Cashier</SelectItem>
              </SelectContent>
            </Select>
          </div>
          <div className="flex items-center gap-2">
            <input
              type="checkbox"
              checked={formData.isActive}
              onChange={(e) => setFormData({ ...formData, isActive: e.target.checked })}
              className="h-4 w-4"
            />
            <Label>Active</Label>
          </div>
          <DialogFooter>
            <Button type="button" variant="outline" onClick={onClose}>
              Cancel
            </Button>
            <Button type="submit">Save</Button>
          </DialogFooter>
        </form>
      </DialogContent>
    </Dialog>
  );
}