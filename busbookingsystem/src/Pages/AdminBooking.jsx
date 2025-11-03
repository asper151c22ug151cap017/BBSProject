// ============================================================================
// Project Name : BusBookingSystem
// File Name    : AdminBooking.jsx
// Created By   : Kaviraj M
// Created On   : 23/09/2025
// Modified By  : Kaviraj M
// Modified On  : 28/10/2025
// Description  : Admin interface for managing bus bookings with CRUD operations,
//                filtering, and Excel export functionality.
// ============================================================================

import React, { useState, useEffect } from "react";
import {
  Button,
  Offcanvas,
  Form,
  Card,
  Row,
  Col,
  Badge,
  Spinner,
  InputGroup,
} from "react-bootstrap";
import DataTable, { createTheme } from "react-data-table-component";
import { ToastContainer, toast } from "react-toastify";
import {
  FaEdit,
  FaTrash,
  FaPlus,
  FaSearch,
  FaSyncAlt,
  FaCalendarAlt,
  FaFileExcel,
} from "react-icons/fa";
import "react-toastify/dist/ReactToastify.css";
import * as XLSX from "xlsx";
import { saveAs } from "file-saver";
import axios from "axios";
import "./Admindashboard.css";

/**
 * Axios instance configuration with base URL and authentication
 * @constant {Object}
 */
const token = localStorage.getItem("token");
const api = axios.create({
  baseURL: "https://localhost:7272/api/Booking",
  headers: { Authorization: `Bearer ${token}` },
});

/**
 * Custom theme configuration for the DataTable component
 * @constant {Object}
 */
createTheme("bookingTheme", {
  text: { primary: "#2c3e50", secondary: "#7f8c8d" },
  background: { default: "#ffffff" },
  divider: { default: "#EDEDED" },
  button: { default: "#e74c3c", hover: "#c0392b" },
});

/**
 * AdminBookings Component
 * 
 * Provides a comprehensive interface for administrators to manage bus bookings.
 * Features include:
 * - Viewing all bookings with sorting and filtering
 * - Adding, editing, and deleting bookings
 * - Filtering by status, date range, and search term
 * - Exporting booking data to Excel
 * 
 * @component
 * @returns {JSX.Element} The admin bookings management interface
 */
const AdminBookings = () => {
  // ==========================================================================
  // STATE MANAGEMENT
  // ==========================================================================
  
  /** @type {Array} List of all bookings */
  const [bookings, setBookings] = useState([]);
  const [loading, setLoading] = useState(true);
  const [refreshing, setRefreshing] = useState(false);
  const [showModal, setShowModal] = useState(false);
  const [modalType, setModalType] = useState("add");
  const [currentBooking, setCurrentBooking] = useState({});
  const [searchTerm, setSearchTerm] = useState("");
  const [statusFilter, setStatusFilter] = useState("all");
  const [fromDate, setFromDate] = useState("");
  const [toDate, setToDate] = useState("");

  // ==========================================================================
  // API FUNCTIONS
  // ==========================================================================
  
  /**
   * Fetches all bookings from the API
   * Updates the bookings state with the response data
   */
  const fetchBookings = async () => {
    try {
      setLoading(true);
      const res = await api.get("/Getallbookings");
      if (Array.isArray(res.data)) setBookings(res.data);
      else setBookings([]);
    } catch (err) {
      toast.error("Failed to fetch bookings");
    } finally {
      setLoading(false);
      setRefreshing(false);
    }
  };

  // ==========================================================================
  // EFFECTS
  // ==========================================================================
  
  /**
   * Fetches bookings when the component mounts
   */
  useEffect(() => {
    fetchBookings();
  }, []);

  // ==========================================================================
  // DATA PROCESSING
  // ==========================================================================
  
  /**
   * Filters bookings based on search term, status, and date range
   * @type {Array}
   */
  const filteredBookings = bookings.filter((b) => {
    const matchSearch =
      b.name?.toLowerCase().includes(searchTerm.toLowerCase()) ||
      b.busName?.toLowerCase().includes(searchTerm.toLowerCase()) ||
      b.source?.toLowerCase().includes(searchTerm.toLowerCase()) ||
      b.destination?.toLowerCase().includes(searchTerm.toLowerCase());

    const matchStatus =
      statusFilter === "all" ||
      b.status?.toLowerCase() === statusFilter.toLowerCase();

    const bookingDate = new Date(b.bookingDate);
    const from = fromDate ? new Date(fromDate) : null;
    const to = toDate ? new Date(toDate) : null;
    const matchDate =
      (!from || bookingDate >= from) && (!to || bookingDate <= to);

    return matchSearch && matchStatus && matchDate;
  });

  // ==========================================================================
  // EVENT HANDLERS
  // ==========================================================================
  
  /**
   * Handles booking deletion after confirmation
   * @param {number} id - The ID of the booking to delete
   */
  const handleDelete = async (id) => {
  if (!window.confirm("Delete this booking?")) return;
  try {
    await api.delete(`/DeleteBus?Bookingid=${id}`); // ✅ corrected endpoint
    toast.success("Booking deleted");
    fetchBookings();
  } catch {
    toast.error("Delete failed");
  }
};


  /**
   * Handles form submission for adding or updating a booking
   * @param {Event} e - The form submission event
   */
  const handleSubmit = async (e) => {
    e.preventDefault();
    try {
      if (modalType === "add") await api.post("/AddBooking", currentBooking);
      else await api.put("/UpdateBooking", currentBooking);
      toast.success("Booking saved");
      fetchBookings();
      setShowModal(false);
    } catch {
      toast.error("Operation failed");
    }
  };

  /**
   * Exports the filtered bookings data to an Excel file
   * Uses the xlsx and file-saver libraries
   */
  const exportToExcel = () => {
    if (filteredBookings.length === 0) {
      toast.info("No data to export");
      return;
    }

    const exportData = filteredBookings.map((b) => ({
      BookingID: b.bookingId,
      Name: b.name,
      Phone: b.phone,
      Source: b.source,
      Destination: b.destination,
      Bus: b.busName,
      Seats: b.seatNumbers?.join(", "),
      Fare: b.totalFare,
      Date: new Date(b.bookingDate).toLocaleDateString(),
      Status: b.status,
    }));

    const worksheet = XLSX.utils.json_to_sheet(exportData);
    const workbook = XLSX.utils.book_new();
    XLSX.utils.book_append_sheet(workbook, worksheet, "Bookings");
    const excelBuffer = XLSX.write(workbook, { type: "array", bookType: "xlsx" });
    const blob = new Blob([excelBuffer], {
      type: "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
    });
    saveAs(blob, `Bookings_${new Date().toISOString().slice(0, 10)}.xlsx`);
  };

  // ==========================================================================
  // TABLE CONFIGURATION
  // ==========================================================================
  
  /**
   * Column configuration for the DataTable component
   * Defines column headers, cell rendering, and sorting behavior
   */
  const columns = [
    { name: "ID", selector: (row) => row.bookingId, width: "70px" },
    {
      name: "Customer",
      selector: (row) => row.name,
      sortable: true,
      cell: (row) => (
        <div>
          <div className="fw-bold">{row.name}</div>
          <div className="text-muted" style={{ fontSize: "12px" }}>
            {row.phone}
          </div>
        </div>
      ),
    },
    { name: "From", selector: (row) => row.source },
    { name: "To", selector: (row) => row.destination },
    { name: "Bus", selector: (row) => row.busName },
    {
      name: "Seats",
      selector: (row) => row.seatNumbers?.join(", "),
      cell: (row) => (
        <div>
          {row.seatNumbers?.slice(0, 3).map((s, i) => (
            <Badge key={i} bg="secondary" className="me-1">
              {s}
            </Badge>
          ))}
        </div>
      ),
    },
    {
      name: "Fare",
      selector: (row) => row.totalFare,
      sortable: true,
      cell: (row) => <strong>₹{row.totalFare}</strong>,
    },
    {
      name: "Date",
      selector: (row) => row.bookingDate,
      sortable: true,
      cell: (row) => (
        <div>
          <FaCalendarAlt className="text-muted me-1" />
          {new Date(row.bookingDate).toLocaleDateString()}
        </div>
      ),
    },
    { 
    name: "Status", 
    selector: row => row.status,
    cell: row => (
      <span
        style={{
          padding: "0.3em 0.6em",
          textAlign:"center",
          borderRadius: "0.80rem",
          color: row.isActive ? "#196825ff" : "#721c24",
          backgroundColor: row.isActive ? "#cbffc2ff" : "#f8d7da",
          fontWeight: 500,
          width:"70px",
          fontSize:11
        }}
      >
        {row.isActive ? "Confirmed" : "Cancelled"}
      </span>
    )
  },
    {
      name: "Action",
      width: "130px",
      cell: (row) => (
        <div className="d-flex justify-content-center gap-2">
          <Button
            size="sm"
            variant="outline-primary"
            onClick={() => {
              setModalType("edit");
              setCurrentBooking(row);
              setShowModal(true);
            }}
          >
            <FaEdit />
          </Button>
          <Button
            size="sm"
            variant="outline-danger"
            onClick={() => handleDelete(row.bookingId)}
          >
            <FaTrash />
          </Button>
        </div>
      ),
    },
  ];

  // ==========================================================================
  // RENDER
  // ==========================================================================
  
  return (
    <div className="admin-container">
      {/* Header */}
      <div className="d-flex justify-content-between align-items-center mb-4">
        <h3 className="fw-bold text-dark mb-0">Booking Management</h3>
        <div className="d-flex gap-2">
          <Button
            variant="outline-success"
            onClick={exportToExcel}
            title="Export to Excel"
          >
            <FaFileExcel className="me-1" /> Export
          </Button>
          <Button
            variant="outline-secondary"
            onClick={() => {
              setRefreshing(true);
              fetchBookings();
            }}
          >
            {refreshing ? <Spinner size="sm" /> : <FaSyncAlt />}
          </Button>
          <Button
            variant="primary"
            onClick={() => {
              setModalType("add");
              setShowModal(true);
            }}
          >
            <FaPlus className="me-1" /> Add Booking
          </Button>
        </div>
      </div>

      {/* ====================================================================
         FILTER CONTROLS
         ==================================================================== */}
      <Card className="p-3 border-0 shadow-sm mb-3">
        <Row className="g-3 align-items-center">
          <Col md={4}>
            <InputGroup>
              <InputGroup.Text>
                <FaSearch />
              </InputGroup.Text>
              <Form.Control
                type="text"
                placeholder="Search bookings..."
                value={searchTerm}
                onChange={(e) => setSearchTerm(e.target.value)}
              />
            </InputGroup>
          </Col>
          <Col md={2}>
            <Form.Select
              value={statusFilter}
              onChange={(e) => setStatusFilter(e.target.value)}
            >
              <option value="all">All Status</option>
              <option value="confirmed">Confirmed</option>
              <option value="cancelled">Cancelled</option>
              <option value="pending">Pending</option>
            </Form.Select>
          </Col>
          <Col md={3}>
            <Form.Control
              type="date"
              value={fromDate}
              onChange={(e) => setFromDate(e.target.value)}
              placeholder="From Date"
            />
          </Col>
          <Col md={3}>
            <Form.Control
              type="date"
              value={toDate}
              onChange={(e) => setToDate(e.target.value)}
              placeholder="To Date"
            />
          </Col>
        </Row>
      </Card>

      {/* ====================================================================
         DATATABLE
         ==================================================================== */}
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

      {/* ====================================================================
   ADD/EDIT OFFCANVAS PANEL
   ==================================================================== */}
<Offcanvas
  show={showModal}
  onHide={() => setShowModal(false)}
  placement="end"
  backdrop={true}
  style={{
    width: "20vw", // 👈 20% of viewport width
    minWidth: "300px", // ensures readable on small screens
    backgroundColor: "#ffffff",
    boxShadow: "-2px 0 10px rgba(0,0,0,0.1)",
  }}
>
  <Offcanvas.Header closeButton>
    <Offcanvas.Title>
      {modalType === "add" ? "Add Booking" : "Edit Booking"}
    </Offcanvas.Title>
  </Offcanvas.Header>
  <Offcanvas.Body>
    <Form onSubmit={handleSubmit}>
      <Row className="g-3">
        <Col md={6}>
          <Form.Group>
            <Form.Label>User ID</Form.Label>
            <Form.Control
              type="number"
              value={currentBooking.userId || ""}
              onChange={(e) =>
                setCurrentBooking({
                  ...currentBooking,
                  userId: e.target.value,
                })
              }
              required
            />
          </Form.Group>
        </Col>
        <Col md={6}>
          <Form.Group>
            <Form.Label>Bus ID</Form.Label>
            <Form.Control
              type="number"
              value={currentBooking.busId || ""}
              onChange={(e) =>
                setCurrentBooking({
                  ...currentBooking,
                  busId: e.target.value,
                })
              }
              required
            />
          </Form.Group>
        </Col>
      </Row>

      <Row className="g-3 mt-2">
        <Col md={6}>
          <Form.Group>
            <Form.Label>Seat Number</Form.Label>
            <Form.Control
              type="text"
              value={currentBooking.seatNumber || ""}
              onChange={(e) =>
                setCurrentBooking({
                  ...currentBooking,
                  seatNumber: e.target.value,
                })
              }
            />
          </Form.Group>
        </Col>
        <Col md={6}>
          <Form.Group>
            <Form.Label>Date</Form.Label>
            <Form.Control
              type="datetime-local"
              value={
                currentBooking.date
                  ? new Date(currentBooking.date).toISOString().slice(0, 16)
                  : ""
              }
              onChange={(e) =>
                setCurrentBooking({
                  ...currentBooking,
                  date: e.target.value,
                })
              }
              required
            />
          </Form.Group>
        </Col>
      </Row>

      <div className="text-end mt-4">
        <Button
          variant="secondary"
          className="me-2"
          onClick={() => setShowModal(false)}
        >
          Cancel
        </Button>
        <Button type="submit" variant="primary">
          {modalType === "add" ? "Add" : "Update"}
        </Button>
      </div>
    </Form>
  </Offcanvas.Body>
</Offcanvas>


      <ToastContainer position="top-right" autoClose={2500} hideProgressBar />
    </div>
  );
};

export default AdminBookings;
