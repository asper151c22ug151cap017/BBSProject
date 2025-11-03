// ============================================================================
// Project Name : BusBookingSystem
// File Name    : Adminuserpage.jsx
// Created By   : Kaviraj M
// Created On   : 23/09/2025
// Modified By  : Kaviraj M
// Modified On  : 28/10/2025
// Description  : Admin interface for managing user accounts with CRUD operations,
//                search, and user status management.
// ============================================================================

import React, { useEffect, useState } from "react";
import axios from "axios";
import DataTable, { createTheme } from "react-data-table-component";
import {
  Button,
  Form,
  Card,
  Collapse,
  Spinner,
  InputGroup,
} from "react-bootstrap";
import { FaEdit, FaTrash, FaPlus, FaSearch } from "react-icons/fa";
import "bootstrap/dist/css/bootstrap.min.css";

/**
 * Base URL for user-related API endpoints
 * @constant {string}
 */
const API_BASE = "https://localhost:7272/api/User";

/**
 * Custom theme configuration for the DataTable component
 */
createTheme("userTheme", {
  text: { primary: "#212529", secondary: "#6c757d" },
  background: { default: "#ffffff" },
  divider: { default: "#e0e0e0" },
  highlightOnHover: { default: "#f1f3f4" },
});

/**
 * AdminUserPage Component
 * 
 * Provides a comprehensive interface for administrators to manage user accounts.
 * Features include:
 * - Viewing all users with sorting and searching
 * - Adding new users
 * - Editing existing user information
 * - Toggling user active status
 * - Deleting users
 * 
 * @component
 * @returns {JSX.Element} The user management interface
 */
function AdminUserPage() {
  // ==========================================================================
  // STATE MANAGEMENT
  // ==========================================================================
  
  /** @type {Array} List of all users */
  const [users, setUsers] = useState([]);
  
  /** @type {boolean} Loading state indicator */
  const [loading, setLoading] = useState(true);
  
  /** @type {string} Search term for filtering users */
  const [searchTerm, setSearchTerm] = useState("");
  
  /** @type {boolean} Controls the visibility of the user form */
  const [showForm, setShowForm] = useState(false);
  
  /** @type {Object|null} Currently selected user for editing */
  const [editingUser, setEditingUser] = useState(null);

  /** @type {Object} Form data for adding/editing users */
  const [formData, setFormData] = useState({
    name: "",
    email: "",
    phone: "",
    age: "",
    password: "",
    isActive: true,
  });

  // ==========================================================================
  // AUTHENTICATION
  // ==========================================================================
  
  /** @type {string} Authentication token from localStorage */
  const token = localStorage.getItem("token");
  
  /** @type {Object} Axios request configuration with auth headers */
  const config = {
    headers: { Authorization: `Bearer ${token}` },
  };

  // ==========================================================================
  // DATA FETCHING
  // ==========================================================================
  const fetchUsers = async () => {
    try {
      setLoading(true);
      const response = await axios.get(`${API_BASE}/Getallusers`, config);
      setUsers(Array.isArray(response.data) ? response.data : []);
    } catch (error) {
      console.error("Error fetching users:", error);
      if (error.response?.status === 401) {
        alert("Unauthorized. Please log in again.");
      } else {
        alert("Failed to fetch users.");
      }
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    fetchUsers();
  }, []);

  // ==========================================================================
  // DATA TRANSFORMATIONS
  // ==========================================================================
  
  /**
   * Filters users based on search term
   * @type {Array}
   */
  const filteredUsers = users.filter((u) => {
    const name = u.name ?? u.Name ?? "";
    const email = u.email ?? u.Email ?? "";
    const phone = u.phone ?? u.Phone ?? "";
    return (
      name.toLowerCase().includes(searchTerm.toLowerCase()) ||
      email.toLowerCase().includes(searchTerm.toLowerCase()) ||
      phone.toLowerCase().includes(searchTerm.toLowerCase())
    );
  });

  // ==========================================================================
  // EVENT HANDLERS
  // ==========================================================================
  const handleChange = (e) => {
    const { name, value, type, checked } = e.target;
    setFormData({
      ...formData,
      [name]: type === "checkbox" ? checked : value,
    });
  };

  /**
   * Resets the form and prepares it for adding a new user
   */
  const handleAdd = () => {
    setEditingUser(null);
    setFormData({
      name: "",
      email: "",
      phone: "",
      age: "",
      password: "",
      isActive: true,
    });
    setShowForm(true);
    window.scrollTo({ top: 0, behavior: "smooth" });
  };

  /**
   * Prepares the form for editing an existing user
   * @param {Object} user - The user object to edit
   */
  const handleEdit = (user) => {
    const normalized = {
      userId: user.userId ?? user.Id,
      name: user.name ?? user.Name ?? "",
      email: user.email ?? user.Email ?? "",
      phone: user.phone ?? user.Phone ?? "",
      age: user.age ?? user.Age ?? "",
      isActive: user.isActive ?? true,
    };
    setEditingUser(normalized);
    setFormData({ ...normalized, password: "" });
    setShowForm(true);
    window.scrollTo({ top: 0, behavior: "smooth" });
  };

  /**
   * Handles form submission for both adding and editing users
   * @param {Event} e - The form submission event
   */
  const handleSave = async () => {
    const payload = {
      userId: editingUser ? editingUser.userId : 0,
      name: formData.name,
      email: formData.email,
      phone: formData.phone,
      age: parseInt(formData.age) || 0,
      password: formData.password || null,
      isActive: formData.isActive,
    };

    try {
      if (editingUser) {
        await axios.put(`${API_BASE}/Updateuser`, payload, config);
        alert("✅ User updated successfully!");
      } else {
        await axios.post(`${API_BASE}/Addusers`, payload, config);
        alert("✅ User added successfully!");
      }

      fetchUsers();
      setShowForm(false);
      setEditingUser(null);
    } catch (error) {
      console.error("Error saving user:", error);
      alert("❌ Failed to save user.");
    }
  };

  /**
   * Deletes a user after confirmation
   * @param {string} userId - The ID of the user to delete
   */
  const handleDelete = async (user) => {
    const id = user.userId ?? user.Id;
    if (!window.confirm(`Are you sure you want to delete ${user.name ?? user.Name}?`)) return;

    try {
      await axios.delete(`${API_BASE}/DeleteUser?userId=${id}`, config);
      alert("✅ User deleted successfully!");
      fetchUsers();
    } catch (error) {
      console.error("Error deleting user:", error);
      alert("❌ Failed to delete user.");
    }
  };

  // ✅ DataTable columns
  const columns = [
    { name: "Name", selector: (row) => row.name ?? row.Name, sortable: true },
    { name: "Email", selector: (row) => row.email ?? row.Email, sortable: true },
    { name: "Phone", selector: (row) => row.phone ?? row.Phone },
    { name: "Age", selector: (row) => row.age ?? row.Age, width: "80px" },
    {
      name: "Active",
      cell: (row) => (
        <span
          style={{
            padding: "0.3em 0.6em",
            borderRadius: "0.8rem",
            color: row.isActive ? "#196825" : "#721c24",
            backgroundColor: row.isActive ? "#cbffc2" : "#f8d7da",
            fontWeight: 500,
            fontSize: 11,
          }}
        >
          {row.isActive ? "Active" : "Inactive"}
        </span>
      ),
    },
    {
      name: "Actions",
      cell: (row) => (
        <>
          <FaEdit
            className="text-primary me-2"
            style={{ cursor: "pointer" }}
            onClick={() => handleEdit(row)}
          />
          <FaTrash
            className="text-danger ms-3"
            style={{ cursor: "pointer" }}
            onClick={() => handleDelete(row)}
          />
        </>
      ),
      ignoreRowClick: true,
    },
  ];

  return (
    <div className="container mt-4">
      <h4 className="fw-bold text-center mb-4">👤 User Management</h4>

      {/* Toolbar */}
      <div className="d-flex justify-content-between mb-3">
        <Button onClick={handleAdd}>
          <FaPlus className="me-2" /> Add User
        </Button>

        <InputGroup className="w-50">
          <InputGroup.Text>
            <FaSearch />
          </InputGroup.Text>
          <Form.Control
            placeholder="Search users..."
            value={searchTerm}
            onChange={(e) => setSearchTerm(e.target.value)}
          />
        </InputGroup>
      </div>

      {/* Collapsible Add/Edit Form */}
      <Collapse in={showForm}>
        <div>
          <Card className="p-4 mb-4 shadow-sm border-0 bg-light">
            <h5 className="mb-3">
              {editingUser ? "✏️ Edit User" : "🆕 Add New User"}
            </h5>
            <Form>
              <div className="row">
                {[
                  { name: "name", label: "Name" },
                  { name: "email", label: "Email", type: "email" },
                  { name: "phone", label: "Phone" },
                  { name: "age", label: "Age", type: "number" },
                  { name: "password", label: "Password", type: "password" },
                ].map((f) => (
                  <div className="col-md-6 mb-3" key={f.name}>
                    <Form.Label>{f.label}</Form.Label>
                    <Form.Control
                      type={f.type || "text"}
                      name={f.name}
                      value={formData[f.name]}
                      onChange={handleChange}
                      placeholder={
                        f.name === "password" && editingUser
                          ? "Leave blank to keep old password"
                          : ""
                      }
                    />
                  </div>
                ))}
                <div className="col-md-6 mb-3 d-flex align-items-center">
                  <Form.Check
                    type="checkbox"
                    label="Active"
                    name="isActive"
                    checked={formData.isActive}
                    onChange={handleChange}
                  />
                </div>
              </div>
            </Form>
            <div className="d-flex justify-content-end">
              <Button
                variant="secondary"
                className="me-2"
                onClick={() => {
                  setShowForm(false);
                  setEditingUser(null);
                }}
              >
                Cancel
              </Button>
              <Button variant="success" onClick={handleSave}>
                {editingUser ? "Update User" : "Add User"}
              </Button>
            </div>
          </Card>
        </div>
      </Collapse>

      {/* ✅ Data Table */}
      <DataTable
        columns={columns}
        data={filteredUsers}
        theme="userTheme"
        pagination
        striped
        highlightOnHover
        progressPending={loading}
        progressComponent={<Spinner animation="border" />}
        noDataComponent={<div className="p-3 text-muted">No users found</div>}
      />
    </div>
  );
}

export default AdminUserPage;
