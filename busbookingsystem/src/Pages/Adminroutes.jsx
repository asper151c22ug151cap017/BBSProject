// ============================================================================
// Project Name : BusBookingSystem
// File Name    : Adminroutes.jsx
// Created By   : Kaviraj M
// Created On   : 24/10/2025
// Modified By  : Kaviraj M
// Modified On  : 28/10/2025
// Description  : Admin interface for managing bus routes in the Bus Booking System.
//                Allows administrators to view, add, edit, and delete routes,
//                and assign buses to specific routes with scheduling information.
// ============================================================================

import React, { useEffect, useState } from "react";
import axios from "axios";
import DataTable from "react-data-table-component";
import { Button, Offcanvas, Form } from "react-bootstrap";
import { FaEdit, FaTrash, FaSearch } from "react-icons/fa";

// API endpoints
const API_BASE = "https://localhost:7272/api/Routes";
const BUS_API = "https://localhost:7272/api/Bus/GetAllBuses";

// ============================================================================
// Component Name: AdminRoutesPage
// Description  : Main component for managing bus routes in the admin panel.
//                Handles CRUD operations for routes and integrates with the backend API.
// State        :
//                - routes (array): List of all routes
//                - filteredRoutes (array): Routes filtered by search term
//                - buses (array): List of available buses
//                - showForm (boolean): Controls form visibility
//                - search (string): Search term for filtering routes
//                - editingRoute (object|null): Currently edited route data
//                - formData (object): Form data for adding/editing routes
// ============================================================================
function AdminRoutesPage() {
  // State for managing routes and UI
  const [routes, setRoutes] = useState([]);
  const [filteredRoutes, setFilteredRoutes] = useState([]);
  const [buses, setBuses] = useState([]);
  const [showForm, setShowForm] = useState(false);
  const [search, setSearch] = useState("");
  const [editingRoute, setEditingRoute] = useState(null);
  
  // Form state for adding/editing routes
  const [formData, setFormData] = useState({
    busType: "",
    busName: "",
    fare: "",
    source: "",
    destination: "",
    journeydate: "",
    departuretime: "",
    arrivaltime: "",
    busId: 0,
  });

  const token = localStorage.getItem("token");
  const config = { headers: { Authorization: `Bearer ${token}` } };

  // Fetch routes
  // ==========================================================================
  // Function: fetchRoutes
  // Description: Fetches all routes from the API and updates the state
  // ==========================================================================
  const fetchRoutes = async () => {
    try {
      const response = await axios.get(`${API_BASE}/GetAllRoutes`, config);
      setRoutes(response.data || []);
      setFilteredRoutes(response.data || []);
    } catch (error) {
      console.error("Error fetching routes:", error);
      setRoutes([]);
      setFilteredRoutes([]);
    }
  };

  // Fetch buses
  // ==========================================================================
  // Function: fetchBuses
  // Description: Fetches all available buses from the API
  // ==========================================================================
  const fetchBuses = async () => {
    try {
      const response = await axios.get(BUS_API, config);
      setBuses(response.data || []);
    } catch (error) {
      console.error("Error fetching buses:", error);
      setBuses([]);
    }
  };

  // ==========================================================================
  // Function: useEffect
  // Description: Fetches routes and buses data when component mounts
  // Dependencies: Empty array (runs once on mount)
  // ==========================================================================
  useEffect(() => {
    fetchRoutes();
    fetchBuses();
  }, []);

  // 🔍 Filter routes live as user types
  useEffect(() => {
    const result = routes.filter((route) =>
      Object.values(route)
        .join(" ")
        .toLowerCase()
        .includes(search.toLowerCase())
    );
    setFilteredRoutes(result);
  }, [search, routes]);

  // ==========================================================================
  // Function: handleSearch
  // Description: Filters routes based on search input
  // Parameters: e (Event) - The input change event
  // ==========================================================================
  const handleSearch = (e) => {
    const value = e.target.value.toLowerCase();
    setSearch(value);
    const filtered = routes.filter(
      (route) =>
        route.source.toLowerCase().includes(value) ||
        route.destination.toLowerCase().includes(value) ||
        route.busName.toLowerCase().includes(value)
    );
    setFilteredRoutes(filtered);
  };

  const handleChange = (e) =>
    setFormData({ ...formData, [e.target.name]: e.target.value });

  // ==========================================================================
  // Function: handleBusSelect
  // Description: Handles bus selection and updates form data
  // Parameters: e (Event) - The select change event
  // ==========================================================================
  const handleBusSelect = (e) => {
    const selectedBusName = e.target.value;
    const selectedBus = buses.find((bus) => bus.busName === selectedBusName);
    if (selectedBus) {
      setFormData({
        ...formData,
        busName: selectedBus.busName,
        busType: selectedBus.busType,
        busId: selectedBus.busId,
      });
    }
  };

  // ==========================================================================
  // Function: handleAdd
  // Description: Prepares the form for adding a new route
  // ==========================================================================
  const handleAdd = () => {
    setEditingRoute(null);
    setFormData({
      busType: "",
      busName: "",
      fare: "",
      source: "",
      destination: "",
      journeydate: "",
      departuretime: "",
      arrivaltime: "",
      busId: 0,
    });
    setShowForm(true);
  };

  // ==========================================================================
  // Function: handleEdit
  // Description: Prepares the form for editing an existing route
  // Parameters: route (Object) - The route object to be edited
  // ==========================================================================
  const handleEdit = (route) => {
    setEditingRoute(route);
    setFormData({
      busType: route.busType,
      busName: route.busName,
      fare: route.fare,
      source: route.source,
      destination: route.destination,
      journeydate: route.journeydate,
      departuretime: route.departuretime,
      arrivaltime: route.arrivaltime,
      busId: route.busId,
    });
    setShowForm(true);
  };

  // ==========================================================================
  // Function: handleSave
  // Description: Saves the route data to the API
  // ==========================================================================
  const handleSave = async () => {
    try {
      const user = JSON.parse(localStorage.getItem("user"));
      const createdBy = user?.name || "Admin";

      const payload = {
        Busid: formData.busId,
        BusName: formData.busName,
        BusType: formData.busType,
        Fare: parseFloat(formData.fare) || 0,
        Source: formData.source,
        Destination: formData.destination,
        Departuretime: formData.departuretime,
        Arrivaltime: formData.arrivaltime,
        createdBy,
      };

      if (editingRoute) {
        await axios.put(`${API_BASE}/UpdateRoutes`, payload, config);
        alert("Route updated successfully!");
      } else {
        await axios.post(`${API_BASE}/AddRoutes`, payload, config);
        alert("Route added successfully!");
      }

      setShowForm(false);
      setEditingRoute(null);
      fetchRoutes();
    } catch (err) {
      console.error("Error saving route:", err.response?.data || err.message);
      alert("Failed to save route.");
    }
  };

  // ==========================================================================
  // Function: handleDelete
  // Description: Handles deletion of a route after confirmation
  // Parameters: id (number) - The ID of the route to be deleted
  // ==========================================================================
  const handleDelete = async (id) => {
    if (window.confirm("Are you sure you want to delete this route?")) {
      try {
        await axios.delete(`${API_BASE}/DeleteRoutes?routeId=${id}`, config);
        alert("Route deleted successfully!");
        fetchRoutes();
      } catch (error) {
        console.error("Error deleting route:", error);
      }
    }
  };

  const columns = [
    { name: "Bus Type", selector: (row) => row.busType, sortable: true },
    { name: "Bus Name", selector: (row) => row.busName, sortable: true },
    { name: "Source", selector: (row) => row.source, sortable: true },
    { name: "Destination", selector: (row) => row.destination, sortable: true },
    { name: "Departure", selector: (row) => row.departuretime },
    { name: "Arrival", selector: (row) => row.arrivaltime },
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

  return (
    <div className="container mt-4">
      <h2 className="mb-4 text-center">🚌 Routes Management</h2>

{/* 🔍 Search Bar + Add Route Button in Same Row */}
<div className="d-flex justify-content-between align-items-center mt-3 mb-3 flex-wrap">
  <div className="input-group shadow-sm rounded" style={{ maxWidth: "600px" }}>
    <span className="input-group-text">
      <FaSearch />
    </span>
    <input
      type="text"
      className="form-control"
      placeholder="Search routes by name, source, or destination..."
      value={search}
      onChange={(e) => setSearch(e.target.value)}
    />
  </div>

  <Button onClick={handleAdd} className="ms-3 mt-2 mt-md-0">
    ➕ Add Route
  </Button>
</div>


      <DataTable
        columns={columns}
        data={filteredRoutes}
        pagination
        highlightOnHover
        striped
        responsive
      />

      {/* Add/Edit Form (Offcanvas) */}
      <Offcanvas show={showForm} onHide={() => setShowForm(false)} placement="end">
        <Offcanvas.Header closeButton>
          <Offcanvas.Title>{editingRoute ? "Edit Route" : "Add Route"}</Offcanvas.Title>
        </Offcanvas.Header>
        <Offcanvas.Body>
          <Form>
            {/* Bus Name Dropdown */}
            <Form.Group className="mb-3">
              <Form.Label>Bus Name</Form.Label>
              <Form.Select value={formData.busName} onChange={handleBusSelect}>
                <option value="">Select Bus</option>
                {buses.map((bus) => (
                  <option key={bus.busId} value={bus.busName}>
                    {bus.busName} ({bus.busType})
                  </option>
                ))}
              </Form.Select>
            </Form.Group>

            {/* Auto-filled fields */}
            <Form.Group className="mb-3">
              <Form.Label>Bus Type</Form.Label>
              <Form.Control value={formData.busType} readOnly />
            </Form.Group>

            <Form.Group className="mb-3">
              <Form.Label>Bus ID</Form.Label>
              <Form.Control value={formData.busId} readOnly />
            </Form.Group>

            {/* Other inputs */}
            {[
              { name: "source", label: "Source" },
              { name: "destination", label: "Destination" },
              { name: "journeydate", label: "Journey Date", type: "date" },
              { name: "departuretime", label: "Departure Time", type: "time" },
              { name: "arrivaltime", label: "Arrival Time", type: "time" },
              { name: "fare", label: "Fare", type: "number" },
            ].map((f) => (
              <Form.Group className="mb-3" key={f.name}>
                <Form.Label>{f.label}</Form.Label>
                <Form.Control
                  type={f.type || "text"}
                  name={f.name}
                  value={formData[f.name]}
                  onChange={handleChange}
                />
              </Form.Group>
            ))}
          </Form>

          <div className="d-flex justify-content-end">
            <Button
              variant="secondary"
              className="me-2"
              onClick={() => setShowForm(false)}
            >
              Cancel
            </Button>
            <Button variant="primary" onClick={handleSave}>
              {editingRoute ? "Update" : "Save"}
            </Button>
          </div>
        </Offcanvas.Body>
      </Offcanvas>
    </div>
  );
}

export default AdminRoutesPage;
