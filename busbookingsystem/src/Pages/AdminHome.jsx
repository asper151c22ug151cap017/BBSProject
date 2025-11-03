// ============================================================================
// Project Name : BusBookingSystem
// File Name    : AdminHome.jsx
// Created By   : Kaviraj M
// Created On   : 22/09/2025
// Modified By  : Kaviraj M
// Modified On  : 28/10/2025
// Description  : Admin dashboard home page displaying key metrics and recent bookings
//                with filtering and export capabilities.
// ============================================================================

import React, { useEffect, useState, useMemo } from "react";
import { FaBus, FaUsers, FaTicketAlt, FaRoad, FaExclamationTriangle } from "react-icons/fa";
import { Card, Row, Col, Badge, Form, Button, Alert } from "react-bootstrap";
import DataTable from "react-data-table-component";
import { Spinner } from "react-bootstrap";
import api from "../apiclients";
import * as XLSX from "xlsx";
import { saveAs } from "file-saver";
import "./Admindashboard.css";

/**
 * AdminHomePage Component
 * 
 * Main dashboard component for administrators displaying:
 * - Key statistics (buses, bookings, users, routes)
 * - Recent bookings with filtering and search
 * - Export functionality to Excel
 * 
 * @component
 * @returns {JSX.Element} The admin dashboard interface
 */
function AdminHomePage() {
  // ==========================================================================
  // STATE MANAGEMENT
  // ==========================================================================
  
  /** @type {Object} Statistics data object */
  const [stats, setStats] = useState({ buses: 0, bookings: 0, users: 0, routes: 0 });
  
  /** @type {Array} List of recent bookings */
  const [recentBookings, setRecentBookings] = useState([]);
  
  /** @type {boolean} Loading state indicator */
  const [loading, setLoading] = useState(true);
  
  /** @type {string} Search term for filtering bookings */
  const [searchTerm, setSearchTerm] = useState("");
  
  /** @type {string} Start date for date range filter */
  const [dateFrom, setDateFrom] = useState("");
  
  /** @type {string} End date for date range filter */
  const [dateTo, setDateTo] = useState("");
  
  /** @type {string|null} Error message to display */
  const [error, setError] = useState(null);

  // ==========================================================================
  // DATA FETCHING
  // ==========================================================================
  useEffect(() => {
    const fetchStats = async () => {
      try {
        setLoading(true);
        setError(null);
        
        const [busRes, bookingRes, userRes, routeRes, recentBookingRes] = await Promise.all([
          api.get("/Bus/GetBusescount"),
          api.get("/Booking/Getbokingcounts"),
          api.get("/User/GetUsercount"),
          api.get("/Routes/GetRoutescount"),
          api.get("/Booking/Getrecentbookings"),
        ]);

        setStats({
          buses: Number(busRes.data) || 0,
          bookings: Number(bookingRes.data) || 0,
          users: Number(userRes.data) || 0,
          routes: Number(routeRes.data) || 0,
        });

        setRecentBookings(Array.isArray(recentBookingRes.data) ? recentBookingRes.data : []);
      } catch (error) {
        console.error("Error fetching stats:", error);
        setError("Failed to load dashboard data. Please check your connection and try again.");
        setStats({ buses: 0, bookings: 0, users: 0, routes: 0 });
        setRecentBookings([]);
      } finally {
        setLoading(false);
      }
    };
    
    fetchStats();
    
      // Set up auto-refresh every 5 minutes
    const interval = setInterval(fetchStats, 5 * 60 * 1000);
    
    // Cleanup interval on component unmount
    return () => clearInterval(interval);
  }, []);

  // ==========================================================================
  // DATA TRANSFORMATIONS
  // ==========================================================================

  const cards = [
    { title: "Total Buses", count: stats.buses, icon: <FaBus />, bg: "primary" },
    { title: "Total Bookings", count: stats.bookings, icon: <FaTicketAlt />, bg: "success" },
    { title: "Total Users", count: stats.users, icon: <FaUsers />, bg: "info" },
    { title: "Total Routes", count: stats.routes, icon: <FaRoad />, bg: "warning" },
  ];

  const filteredBookings = useMemo(() => {
    if (!Array.isArray(recentBookings)) return [];

    return recentBookings.filter((b) => {
      const busName = b.tblbuses?.busName || "";
      const source = b.tblbuses?.routesinfo?.[0]?.source || "";
      const destination = b.tblbuses?.routesinfo?.[0]?.destination || "";
      const bookingDate = new Date(b.bookingDate);

      const matchesSearch =
        busName.toLowerCase().includes(searchTerm.toLowerCase()) ||
        source.toLowerCase().includes(searchTerm.toLowerCase()) ||
        destination.toLowerCase().includes(searchTerm.toLowerCase()) ||
        b.bookingId?.toString().includes(searchTerm);

      const matchesDateFrom = !dateFrom || bookingDate >= new Date(dateFrom);
      const matchesDateTo = !dateTo || bookingDate <= new Date(dateTo);

      return matchesSearch && matchesDateFrom && matchesDateTo;
    });
  }, [recentBookings, searchTerm, dateFrom, dateTo]);

  const clearFilters = () => {
    setSearchTerm("");
    setDateFrom("");
    setDateTo("");
  };

  // DataTable columns
  const columns = [
    { name: "Booking ID", selector: row => row.bookingId, sortable: true, width: "120px" },
    { name: "Customer", selector: row => row.name, sortable: true, style: { minWidth: "180px",  } },
    { name: "Source", selector: row => row.source, sortable: true, style: { minWidth: "120px" } },
    { name: "Destination", selector: row => row.destination, sortable: true, style: { minWidth: "120px" } },
    { name: "Bus Name", selector: row => row.busName, sortable: true },
    { name: "Seats", selector: row => row.seatNumbers?.join(", ") || "" },
    { name: "Passengers", selector: row => row.passangers?.length || 0 },
    { name: "Fare", selector: row => `₹${row.totalFare?.toLocaleString() || 0}` },
    { name: "Date", selector: row => new Date(row.bookingDate).toLocaleString() },
    {
      name: "Status",
      cell: row => (
        <Badge bg={row.status === "Confirmed" ? "success" : row.status === "Cancelled" ? "danger" : "warning"}>
          {row.status}
        </Badge>
      ),
    },
    {
      name: "Active",
      cell: row => (
        <Badge bg={row.isActive ? "success" : "secondary"}>
          {row.isActive ? "Active" : "Inactive"}
        </Badge>
      ),
    },
  ];

  // ==========================================================================
  // EVENT HANDLERS
  // ==========================================================================
  
  /**
   * Exports the filtered booking data to an Excel file
   * @returns {void}
   */
  const exportToExcel = () => {
    try {
      if (!filteredBookings.length) {
        setError("No booking data available to export.");
        return;
      }

      const exportData = filteredBookings.map(row => ({
        "Booking ID": row.bookingId,
        "Customer Name": row.name,
        "Phone": row.phone,
        "Bus Name": row.busName,
        "Bus Number": row.busnumber,
        "Source": row.source,
        "Destination": row.destination,
        "Seat Numbers": row.seatNumbers?.join(", ") || "",
        "Passengers": row.passangers?.length || 0,
        "Total Fare": row.totalFare,
        "Booking Date": new Date(row.bookingDate).toLocaleDateString(),
        "Booking Time": new Date(row.bookingDate).toLocaleTimeString(),
        "Status": row.status,
        "Active": row.isActive ? "Yes" : "No",
      }));

      const worksheet = XLSX.utils.json_to_sheet(exportData);
      const workbook = XLSX.utils.book_new();
      XLSX.utils.book_append_sheet(workbook, worksheet, "Recent Bookings");
      
      // Auto-size columns
      const wscols = [
        { wch: 12 }, // Booking ID
        { wch: 20 }, // Customer Name
        { wch: 15 }, // Phone
        { wch: 20 }, // Bus Name
        { wch: 12 }, // Bus Number
        { wch: 15 }, // Source
        { wch: 15 }, // Destination
        { wch: 20 }, // Seat Numbers
        { wch: 12 }, // Passengers
        { wch: 12 }, // Total Fare
        { wch: 15 }, // Booking Date
        { wch: 15 }, // Booking Time
        { wch: 12 }, // Status
        { wch: 10 }, // Active
      ];
      worksheet['!cols'] = wscols;

      const excelBuffer = XLSX.write(workbook, { bookType: "xlsx", type: "array" });
      saveAs(
        new Blob([excelBuffer], { type: "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet" }),
        `Bookings_${new Date().toISOString().slice(0, 10)}.xlsx`
      );
    } catch (err) {
      console.error("Error exporting to Excel:", err);
      setError("Failed to export data. Please try again.");
    }
  };

  return (
    <div className="dashboard-container">
      <div className="page-header mb-4">
        <h1 className="page-title"><FaTicketAlt /> Admin Dashboard</h1>
        <p className="page-subtitle">Overview of your bus management system</p>
      </div>

      {error && (
        <Alert variant="danger" onClose={() => setError(null)} dismissible className="mb-4">
          <FaExclamationTriangle className="me-2" />
          {error}
        </Alert>
      )}

      {/* Stats Cards */}
      <Row className="mb-4">
        {cards.map((c, idx) => (
          <Col key={idx} lg={3} md={6} sm={12} className="mb-3">
            <Card
              className={`text-center border-${c.bg} dashboard-card`}
              style={{ animationDelay: `${idx * 0.15}s` }}
            >
              <Card.Body>
                <div className={`text-${c.bg} fs-3 mb-2`}>{c.icon}</div>
                <Card.Title>{c.title}</Card.Title>
                <Card.Text className="fs-4 fw-bold">{c.count.toLocaleString()}</Card.Text>
              </Card.Body>
            </Card>
          </Col>
        ))}
      </Row>

      {/* Filters */}
      <Card className=" p-3 filter-card" style={{backgroundColor:"#ebf1fbff", boxShadow:"lg", marginBottom:"60px"}}>
        <Row className="align-items-end g-3">
          <Col md={4}>
            <Form.Label>Search</Form.Label>
            <Form.Control
              type="text"
              placeholder="Search by bus, source, destination, or ID"
              value={searchTerm}
              onChange={e => setSearchTerm(e.target.value)}
            />
          </Col>
          <Col md={3}>
            <Form.Label>From Date</Form.Label>
            <Form.Control type="date" value={dateFrom} onChange={e => setDateFrom(e.target.value)} />
          </Col>
          <Col md={3}>
            <Form.Label>To Date</Form.Label>
            <Form.Control type="date" value={dateTo} onChange={e => setDateTo(e.target.value)} />
          </Col>
          <Col md={2}>
            <Button variant="outline-primary" className="w-100 mb-2" onClick={clearFilters}>Clear</Button>
            <Button variant="success" className="w-100" onClick={exportToExcel}>Export Excel</Button>
          </Col>
        </Row>
      </Card>

      {/* DataTable */}
       {/* Data Table */}
      <Card className="border-0 shadow-sm">
        <DataTable
          columns={columns}
          data={filteredBookings}
          theme="bookingTheme"
          pagination
          highlightOnHover
          striped
          progressPending={loading}
          progressComponent={<Spinner animation="border" variant="danger" />}
          noDataComponent={
            <div className="p-4 text-center text-muted">No bookings found</div>
          }
        />
      </Card>

    </div>
  );
}

export default AdminHomePage;
