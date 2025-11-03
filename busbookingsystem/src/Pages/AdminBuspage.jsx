// ============================================================================
// Project Name : BusBookingSystem
// File Name    : AdminBuspage.jsx
// Created By   : Kaviraj M
// Created On   : 23/09/2025
// Modified By  : Kaviraj M
// Modified On  : 28/10/2025
// Description  : Admin interface for managing bus information including
//                adding, editing, and deleting bus records.
// ============================================================================

import React, { useState, useEffect } from "react";
import axios from "axios";
import DataTable from "react-data-table-component";
import { FaEdit, FaTrash, FaSearch } from "react-icons/fa";

/**
 * Base URL for bus-related API endpoints
 * @constant {string}
 */
const API_BASE = "https://localhost:7272/api/Bus";

/**
 * AdminBusPage Component
 * 
 * Provides a comprehensive interface for administrators to manage bus information.
 * Features include:
 * - Viewing a list of all buses with sorting and searching capabilities
 * - Adding new bus records
 * - Editing existing bus information
 * - Deleting bus records
 * 
 * @component
 * @returns {JSX.Element} The admin bus management interface
 */
function AdminBusPage() {
  // ==========================================================================
  // STATE MANAGEMENT
  // ==========================================================================
  
  /** @type {Array} List of all buses */
  const [buses, setBuses] = useState([]);
  const [filteredBuses, setFilteredBuses] = useState([]);
  const [search, setSearch] = useState("");
  const [formData, setFormData] = useState(getInitialFormData());
  const [editingBus, setEditingBus] = useState(null);
  const [showForm, setShowForm] = useState(false);

  /**
   * Returns the initial state for the bus form
   * @returns {Object} Initial form data with empty fields
   */
  function getInitialFormData() {
    return {
      BusNumber: "",
      OperatorName: "",
      OperatorNumber: "",
      BusName: "",
      BusType: "",
      TotalSeats: "",
      Fare: "",
    };
  }

  /**
   * Generates authentication headers with JWT token
   * @returns {Object} Headers object with Authorization token
   */
  const getAuthHeaders = () => ({
    Authorization: `Bearer ${localStorage.getItem("token")}`,
  });

  // ==========================================================================
  // API FUNCTIONS
  // ==========================================================================
  
  /**
   * Fetches all buses from the API and updates the state
   * Handles both successful responses and errors
   */
  const fetchBuses = async () => {
    try {
      const res = await axios.get(`${API_BASE}/GetAllBuses`, {
        headers: getAuthHeaders(),
      });
      setBuses(res.data);
      setFilteredBuses(res.data); // ✅ set filtered data initially
    } catch (err) {
      console.error("Error fetching buses:", err);
      alert("Failed to load buses. Please check your login.");
    }
  };

  // ==========================================================================
  // EFFECTS
  // ==========================================================================
  
  /**
   * Fetches buses when the component mounts
   */
  useEffect(() => {
    fetchBuses();
  }, []);

  /**
   * Filters buses based on search input
   * Updates filteredBuses state whenever search term or buses list changes
   */
  useEffect(() => {
    if (!search) {
      setFilteredBuses(buses);
      return;
    }

    const result = buses.filter((bus) =>
      Object.values(bus)
        .join(" ")
        .toLowerCase()
        .includes(search.toLowerCase())
    );
    setFilteredBuses(result);
  }, [search, buses]);

  // ==========================================================================
  // EVENT HANDLERS
  // ==========================================================================
  
  /**
   * Handles input changes in the bus form
   * @param {Object} e - The event object
   */
  const handleChange = (e) => {
    setFormData({ ...formData, [e.target.name]: e.target.value });
  };

  /**
   * Resets the form to its initial state and hides it
   */
  const resetForm = () => {
    setFormData(getInitialFormData());
    setEditingBus(null);
    setShowForm(false);
  };

  /**
   * Populates the form with bus data for editing
   * @param {Object} bus - The bus object to edit
   */
  const handleEdit = (bus) => {
    setEditingBus(bus);
    setFormData({
      BusNumber: bus.busnumber,
      OperatorName: bus.operatorName,
      OperatorNumber: bus.operatorNumber,
      BusName: bus.busName,
      BusType: bus.busType,
      TotalSeats: bus.totalSeats,
      Fare: bus.fare,
    });
    setShowForm(true);
  };

  /**
   * Handles form submission for both creating and updating bus records
   * Sends data to the appropriate API endpoint based on whether it's an edit or add operation
   */
  const handleSave = async () => {
    try {
      const payload = {
        BusId: editingBus?.busId || 0,
        Busnumber: formData.BusNumber,
        OperatorName: formData.OperatorName,
        OperatorNumber: formData.OperatorNumber,
        BusName: formData.BusName,
        BusType: formData.BusType,
        TotalSeats: parseInt(formData.TotalSeats, 10) || 0,
        Fare: parseFloat(formData.Fare) || 0,
      };

      if (editingBus) {
        await axios.put(`${API_BASE}/UpdateBus`, payload, {
          headers: getAuthHeaders(),
        });
        alert("Bus updated successfully!");
      } else {
        await axios.post(`${API_BASE}/AddBus`, payload, {
          headers: getAuthHeaders(),
        });
        alert("Bus added successfully!");
      }

      resetForm();
      fetchBuses();
    } catch (err) {
      console.error("Error saving bus:", err.response?.data || err.message);
      alert("Failed to save bus. Check console for details.");
    }
  };

  /**
   * Handles bus deletion after confirmation
   * @param {number} busId - The ID of the bus to delete
   */
  const handleDelete = async (busId) => {
    if (!window.confirm("Are you sure you want to delete this bus?")) return;

    try {
      await axios.delete(`${API_BASE}/DeleteBus?busId=${busId}`, {
        headers: getAuthHeaders(),
      });
      alert("Bus deleted successfully!");
      fetchBuses();
    } catch (err) {
      console.error("Error deleting bus:", err.response?.data || err.message);
      alert("Failed to delete bus.");
    }
  };

  // ==========================================================================
  // TABLE CONFIGURATION
  // ==========================================================================
  
  /**
   * Configuration for the DataTable component
   * Defines columns, their headers, and cell rendering logic
   */
  const columns = [
    { name: "Bus Number", selector: (row) => row.busnumber, sortable: true },
    { name: "Operator", selector: (row) => row.operatorName, sortable: true },
    { name: "Operator No", selector: (row) => row.operatorNumber },
    { name: "Name", selector: (row) => row.busName, sortable: true },
    { name: "Type", selector: (row) => row.busType },
    { name: "Seats", selector: (row) => row.totalSeats, sortable: true },
    { name: "Fare", selector: (row) => row.fare, sortable: true },
    {
      name: "Active",
      selector: (row) => row.isActive,
      cell: (row) => (
        <span
          style={{
            padding: "0.3em 0.6em",
            textAlign: "center",
            borderRadius: "0.8rem",
            color: row.isActive ? "#196825ff" : "#721c24",
            backgroundColor: row.isActive ? "#cbffc2ff" : "#f8d7da",
            fontWeight: 500,
            width: "55px",
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
            className="text-primary me-3"
            style={{ cursor: "pointer" }}
            onClick={() => handleEdit(row)}
          />
          <FaTrash
            className="text-danger"
            style={{ cursor: "pointer" }}
            onClick={() => handleDelete(row.busId)}
          />
        </>
      ),
      ignoreRowClick: true,
      allowOverflow: true,
      button: true,
    },
  ];

  // ==========================================================================
  // RENDER
  // ==========================================================================
  
  return (
    <div className="container mt-4">
      <h2>Admin - Manage Buses</h2>

      {/* 🔍 Search Bar */}
      <div className="input-group mb-3 mt-3" style={{ maxWidth: "350px" }}>
        <span className="input-group-text">
          <FaSearch />
        </span>
        <input
          type="text"
          className="form-control"
          placeholder="Search by name, number, or operator..."
          value={search}
          onChange={(e) => setSearch(e.target.value)}
        />
      </div>

      <button
        className="btn btn-primary mb-3"
        onClick={() => {
          resetForm();
          setShowForm(true);
        }}
      >
        + Add Bus
      </button>

      {/* ✅ Use filteredBuses instead of buses */}
      <DataTable
        columns={columns}
        data={filteredBuses}
        pagination
        highlightOnHover
        pointerOnHover
        striped
      />

      {showForm && (
        <div className="card p-3 mt-3">
          <h4>{editingBus ? "Edit Bus" : "Add New Bus"}</h4>
          <div className="row g-2">
            {[
              "BusNumber",
              "OperatorName",
              "OperatorNumber",
              "BusName",
              "BusType",
              "TotalSeats",
              "Fare",
            ].map((field) => (
              <div className="col-md-6" key={field}>
                <input
                  type="text"
                  className="form-control"
                  name={field}
                  placeholder={field}
                  value={formData[field] || ""}
                  onChange={handleChange}
                />
              </div>
            ))}
          </div>
          <div className="mt-3">
            <button className="btn btn-success" onClick={handleSave}>
              Save
            </button>
            <button className="btn btn-secondary ms-2" onClick={resetForm}>
              Cancel
            </button>
          </div>
        </div>
      )}
    </div>
  );
}

export default AdminBusPage;
