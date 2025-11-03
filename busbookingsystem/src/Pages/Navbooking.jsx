// ============================================================================
// Project Name : BusBookingSystem
// File Name    : Navbooking.jsx
// Created By   : Kaviraj M 
// Created On   : 24/10/2025
// Modified By  : Kaviraj M 
// Modified On  : 28/10/2025
// Description  : React component that manages user bookings in the Bus Booking System.
//                It handles viewing, filtering, and canceling bookings, along with
//                ticket generation and download functionality. Integrates with the
//                backend API to fetch and update booking data.
// ============================================================================

import React, { useEffect, useState, useRef } from "react";
import axios from "axios";
import BusNavbar from "./Navbar";
import { useNavigate } from "react-router-dom";
import { FaDownload, FaTimes, FaTicketAlt, FaEye, FaSpinner, FaCheckCircle, FaExclamationTriangle, FaBus, FaCalendarAlt, FaMapMarkerAlt, FaUser, FaUsers, FaRupeeSign, FaFilter, FaSearch, FaStar, FaClock, FaArrowRight } from "react-icons/fa";
import { Toast, ToastContainer } from "react-bootstrap";
import jsPDF from "jspdf";
import html2canvas from "html2canvas";
import "./Navbooking.css";
// ============================================================================
// Component Name: CenteredModal
// Description  : A reusable modal component that centers its content and handles
//                accessibility features like keyboard navigation and focus management.
// Props        : 
//                - isOpen (boolean): Controls modal visibility
//                - onClose (function): Callback when modal is closed
//                - title (string): Modal title
//                - children (ReactNode): Modal content
//                - labelledBy (string): ID of the element that labels the modal
// ============================================================================
const CenteredModal = ({ isOpen, onClose, title, children, labelledBy }) => {
  const modalRef = useRef(null);
  const previousFocusRef = useRef(null);

  // Focus management
  useEffect(() => {
    if (isOpen) {
      // Store the previously focused element
      previousFocusRef.current = document.activeElement;
      // Focus the modal
      if (modalRef.current) {
        modalRef.current.focus();
      }
    } else if (previousFocusRef.current) {
      // Return focus to the previously focused element
      previousFocusRef.current.focus();
    }

    return () => {
      // Cleanup focus on unmount
      if (previousFocusRef.current) {
        previousFocusRef.current.focus();
      }
    };
  }, [isOpen]);

  // Handle escape key
  useEffect(() => {
    const handleKeyDown = (event) => {
      if (event.key === 'Escape' && isOpen) {
        onClose();
      }
    };

    if (isOpen) {
      document.addEventListener('keydown', handleKeyDown);
    }

    return () => {
      document.removeEventListener('keydown', handleKeyDown);
    };
  }, [isOpen, onClose]);

  // Handle backdrop click
  const handleBackdropClick = (event) => {
    if (event.target === event.currentTarget) {
      onClose();
    }
  };

  if (!isOpen) return null;

  return (
    <div
      className={`modal-overlay ${isOpen ? 'show' : ''}`}
      onClick={handleBackdropClick}
      role="presentation"
    >
      <div
        className="modal-dialog"
        role="dialog"
        aria-modal="true"
        aria-labelledby={labelledBy}
        ref={modalRef}
        tabIndex="-1"
      >
        <div className="modal-content">
          <div className="modal-header">
            <h5 className="modal-title" id={labelledBy}>{title}</h5>
            <button
              type="button"
              className="modal-close-btn"
              onClick={onClose}
              aria-label="Close modal"
            >
              ×
            </button>
          </div>
          <div className="modal-body">
            {children}
          </div>
        </div>
      </div>
    </div>
  );
};


// ============================================================================
// Component Name: Navbooking
// Description  : Main component for managing user bookings. Handles fetching,
//                displaying, filtering, and canceling bookings. Also provides
//                functionality to view and download booking tickets as PDF.
// State        :
//                - bookings (array): List of user's bookings
//                - loading (boolean): Loading state indicator
//                - toastMessage (string): Message to display in toast notifications
//                - showToast (boolean): Controls toast visibility
//                - selectedBookingList (array): Currently selected booking details
//                - filterStatus (string): Current filter status for bookings
//                - searchTerm (string): Search term for filtering bookings
//                - showModal (boolean): Controls modal visibility
//                - ticketLoading (boolean): Loading state for ticket generation
// ============================================================================
function Navbooking() {
  const [bookings, setBookings] = useState([]);
  const [loading, setLoading] = useState(true);
  const [toastMessage, setToastMessage] = useState("");
  const [showToast, setShowToast] = useState(false);
  const [selectedBookingList, setSelectedBookingList] = useState([]);
  const [filterStatus, setFilterStatus] = useState("");
  const [searchTerm, setSearchTerm] = useState("");
  const [showModal, setShowModal] = useState(false);
  const ticketRef = useRef();
  const [ticketLoading, setTicketLoading] = useState(false);
  const navigate = useNavigate();

  // ==========================================================================
  // Function Name: fetchBookings
  // Description  : Fetches the current user's bookings from the backend API.
  //                Handles authentication and error states.
  // Dependencies : Runs on component mount and when dependencies change
  // ==========================================================================
  useEffect(() => {
  const fetchBookings = async () => {
    try {
      const userId = localStorage.getItem("userid");
      const token = localStorage.getItem("token");

      if (!userId) {
        console.warn("⚠️ No user ID found in localStorage");
        setLoading(false);
        return;
      }

      const config = {
        headers: {
          Authorization: `Bearer ${token}`,
          "Content-Type": "application/json",
        },
      };

      const response = await axios.get(
        `https://localhost:7272/api/Confirmbooking/GetBookingsByUserId?userId=${userId}`,
        config
      );

      setBookings(response.data || []);
    } catch (error) {
      console.error("❌ Error fetching bookings:", error);
    } finally {
      setLoading(false);
    }
  };

  fetchBookings();
}, []);

// ==========================================================================
// Function Name: filteredBookings
// Description  : Filters the bookings array based on the current filter status
//                and search term. Matches against bus name, route details, and booking ID.
// Parameters   : None (uses component state)
// Returns      : Array of filtered bookings
// ==========================================================================
const filteredBookings = bookings.filter((booking) => {
  const matchesStatus =
    filterStatus === "" ||
    booking.status?.toLowerCase() === filterStatus.toLowerCase();

  const matchesSearch =
    searchTerm === "" ||
    booking.tblbuses?.busName?.toLowerCase().includes(searchTerm.toLowerCase()) ||
    booking.tblbuses?.routesinfo?.[0]?.source
      ?.toLowerCase()
      .includes(searchTerm.toLowerCase()) ||
    booking.tblbuses?.routesinfo?.[0]?.destination
      ?.toLowerCase()
      .includes(searchTerm.toLowerCase()) ||
    booking.bookingId?.toString().includes(searchTerm);

  return matchesStatus && matchesSearch;
});

  // ==========================================================================
  // Function Name: handleView
  // Description  : Fetches and displays detailed ticket information for a specific booking.
  //                Handles loading states and error notifications.
  // Parameters   : bookingId (string|number) - ID of the booking to view
  // ==========================================================================
  const handleView = async (bookingId) => {
  try {
    setTicketLoading(true);

    // ✅ Get token from localStorage
    const token = localStorage.getItem("token");

    // ✅ If token is missing, stop and show message
    if (!token) {
      setToastMessage("Unauthorized! Please log in again.");
      setShowToast(true);
      navigate("/login");
      return;
    }

    // ✅ Make authorized request
    const response = await axios.get(
      `https://localhost:7272/api/Confirmbooking/downloadticket?bookingid=${bookingId}`,
      {
        headers: {
          Authorization: `Bearer ${token}`,
        },
      }
    );

    // ✅ Normalize and handle response
    if (response.data && response.data.length > 0) {
      const normalizedData = response.data.map((b) => ({
        bookingId: b.bookingId || b.BookingId,
        status: b.status || b.Status,
        name: b.name || b.Name,
        phone: b.phone || b.Phone,
        bookingDate: b.bookingDate || b.BookingDate,
        source: b.source || b.Source,
        destination: b.destination || b.Destination,
        busName: b.busName || b.BusName,
        busnumber: b.busnumber || b.BusNumber,
        seatNumber: b.seatNumber || b.SeatNumber || [],
        totalFare: b.totalFare || b.TotalFare,
        passangers: b.passangers || b.Passangers || [],
      }));

      setSelectedBookingList(normalizedData);
      setShowModal(true);
    } else {
      setToastMessage("No ticket found for this booking.");
      setShowToast(true);
    }
  } catch (error) {
    console.error("Failed to fetch ticket details:", error);
    if (error.response && error.response.status === 401) {
      setToastMessage("Session expired. Please log in again.");
      navigate("/login");
    } else {
      setToastMessage("Failed to fetch ticket. Try again later.");
    }
    setShowToast(true);
  } finally {
    setTicketLoading(false);
  }
};


  // ==========================================================================
  // Function Name: handleCancel
  // Description  : Cancels a booking after user confirmation. Updates the backend
  //                and local state to reflect the cancellation.
  // Parameters   : bookingId (string|number) - ID of the booking to cancel
  // ==========================================================================
  const handleCancel = async (bookingId) => {
    const userId = localStorage.getItem("userid");
    const userName = localStorage.getItem("username") || "Unknown User";
      const token = localStorage.getItem("token"); 

    if (!window.confirm(`Are you sure you want to cancel this booking?\n\nBooking ID: ${bookingId}\nCancelled by: ${userName}`)) return;

    try {
      const response = await axios.post(
        `https://localhost:7272/api/Confirmbooking/CancelBooking?BookingId=${bookingId}`,
          {}, // empty body (since you're using query param)
      {
        headers: {
          "Authorization": `Bearer ${token}`, // ✅ added header
          "Content-Type": "application/json"
        }
      }
      );

      setToastMessage(`Booking cancelled successfully by ${userName}!`);
      setShowToast(true);

      // Update local state with cancellation details
      setBookings((prev) =>
        prev.map((b) =>
          b.BookingId === bookingId ? {
            ...b,
            Status: "Cancelled",
            cancelledBy: userName,
            cancelledAt: new Date().toISOString()
          } : b
        )
      );
    } catch (error) {
      setToastMessage("Failed to cancel booking. Try again later.");
      setShowToast(true);
    }
  };

  if (loading) {
    return (
      <>
        <BusNavbar />
        <div className="dashboard-layout">
          <div className="dashboard-content">
            <div className="loading-container">
              <div className="loading-spinner"></div>
              <p className="loading-text">Loading your bus bookings...</p>
            </div>
          </div>
        </div>
      </>
    );
  }

  if (bookings.length === 0) {
    return (
      <>
        <BusNavbar />
        <div className="dashboard-layout">
          <div className="dashboard-content">
            <div className="empty-state">
              <div className="empty-state-icon">
                <FaTicketAlt />
              </div>
              <h3 className="empty-state-title">No bookings yet</h3>
              <p className="empty-state-subtitle">You haven't made any bus bookings yet. Start your journey by booking your first ticket!</p>
              <button
                className="btn-primary-large"
                onClick={() => navigate("/home")}
              >
                <FaBus className="btn-icon" />
                Book Your First Ticket
              </button>
            </div>
          </div>
        </div>
      </>
    );
  }

  return (
    <>
      <BusNavbar />
      <div className="dashboard-layout">
        <div className="dashboard-content">

          {/* Personal Booking Header */}
          <div className="booking-headerdiv">
            <div className="booking-title">
              <div className="title-section">
                <FaTicketAlt className="title-icon" />
                <div>
                  <h3 className="booking-main-title">My Bus Bookings</h3>
                  <p className="booking-subtitle">View and manage your ticket reservations</p>
                </div>
              </div>
              <div className="booking-stats">
                <div className="stat-item">
                  <span className="stat-number">{filteredBookings.length}</span>
                  <span className="stat-label">Total</span>
                </div>
                <div className="stat-divider"></div>
                <div className="stat-item">
                  <span className="stat-number">
                    {filteredBookings.filter(b => b.status === 'Confirmed').length}
                  </span>
                  <span className="stat-label">Active</span>
                </div>
                <div className="stat-divider"></div>
                <div className="stat-item">
                  <span className="stat-number">
                    {filteredBookings.filter(b => b.status === 'Cancelled').length}
                  </span>
                  <span className="stat-label">Cancelled</span>
                </div>
              </div>
            </div>

            <div className="booking-filters">
              <div className="search-section">
                <div className="search-input-wrapper">
                  <FaSearch className="search-icon" />
                  <input
                    type="text"
                    className="search-input"
                    placeholder="Search by bus name, route, or booking ID..."
                    value={searchTerm}
                    onChange={(e) => setSearchTerm(e.target.value)}
                  />
                </div>
              </div>

              <div className="filter-section">
                <select
                  className="filter-dropdown"
                  value={filterStatus}
                  onChange={(e) => setFilterStatus(e.target.value)}
                >
                  <option value="">All Bookings</option>
                  <option value="confirmed">Active Bookings</option>
                  <option value="cancelled">Cancelled</option>
                </select>
                <FaFilter className="filter-icon" />
              </div>
            </div>
          </div>

          {/* My Bookings Table */}
          <div className="bookings-container">
            <div className="table-responsive">
              <table className="bookings-table">
                <thead className="table-header">
                  <tr>
                    <th className="booking-id-col">Booking ID</th>
                    <th className="route-col">Route & Bus</th>
                    <th className="passenger-col">Passenger</th>
                    <th className="journey-col">Journey Date</th>
                    <th className="seats-col">Seats</th>
                    <th className="fare-col">Total Fare</th>
                    <th className="status-col">Status</th>
                    <th className="actions-col">Actions</th>
                  </tr>
                </thead>
                <tbody>
                  {filteredBookings.length > 0 ? filteredBookings.map((booking, index) => (
                    <tr key={booking.bookingId} className="booking-row">
                      <td className="booking-id-cell">
                        <div className="booking-id-wrapper">
                          <FaTicketAlt className="booking-id-icon" />
                          <span className="booking-id">#{booking.bookingId}</span>
                        </div>
                      </td>
                      <td className="route-cell">
                        <div className="route-info">
                          <div className="route-path">
                            <span className="source">{booking.tblbuses?.routesinfo?.[0]?.source}</span>
                            <FaBus className="route-arrow" />
                            <span className="destination">{booking.tblbuses?.routesinfo?.[0]?.destination}</span>
                          </div>
                          <div className="bus-details">
                            <span className="bus-name">{booking.tblbuses?.busName}</span>
                            <span className="bus-type">({booking.tblbuses?.busType})</span>
                          </div>
                        </div>
                      </td>
                      <td className="passenger-cell">
                        <div className="passenger-info">
                          <div className="passenger-count">
                            <FaUser className="passenger-icon" />
                            <span>{booking.passengers || booking.seatNumbers?.length || 1} Passenger{(booking.passengers || booking.seatNumbers?.length || 1) > 1 ? 's' : ''}</span>
                          </div>
                        </div>
                      </td>
                      <td className="journey-cell">
                        <div className="journey-info">
                          <div className="journey-date">
                            <FaCalendarAlt className="date-icon" />
                            <span>{new Date(booking.bookingDate).toLocaleDateString('en-IN', {
                              day: '2-digit',
                              month: 'short',
                              year: 'numeric'
                            })}</span>
                          </div>
                        <div className="journey-time">
  <FaClock className="time-icon" />
  <span>
    {new Date(booking.bookingDate).toLocaleTimeString([], {
      hour: '2-digit',
      minute: '2-digit',
    })}
  </span>
</div>

                        </div>
                      </td>
                      <td className="seats-cell">
                        <div className="seats-info">
                          {booking.seatNumbers?.map((seat, seatIndex) => (
                            <span key={seatIndex} className="seat-badge">
                              {seat}
                            </span>
                          ))}
                        </div>
                      </td>
                      <td className="fare-cell">
                        <div className="fare-info">
                          <FaRupeeSign className="rupee-icon" />
                          <span className="fare-amount">{booking.totalFare?.toLocaleString('en-IN')}</span>
                        </div>
                      </td>
                      <td className="status-cell">
                        <div className="status-wrapper">
                          <span className={`status-badge ${booking.status?.toLowerCase()}`}>
                            {booking.status === 'Confirmed' && <FaCheckCircle className="status-icon" />}
                            {booking.status === 'Cancelled' && <FaTimes className="status-icon" />}
                            {booking.status}
                          </span>
                          {booking.status === 'Cancelled' && booking.cancelledBy && (
                            <div className="cancellation-details">
                              <small className="cancelled-by">
                                by {booking.cancelledBy}
                              </small>
                              {booking.cancelledAt && (
                                <small className="cancelled-at">
                                  on {new Date(booking.cancelledAt).toLocaleDateString('en-IN', {
                                    day: '2-digit',
                                    month: 'short',
                                    hour: '2-digit',
                                    minute: '2-digit'
                                  })}
                                </small>
                              )}
                            </div>
                          )}
                        </div>
                      </td>
                      <td className="actions-cell">
                        <div className="action-buttons">
                          <button
                            className="btn-action btn-view"
                            onClick={() => handleView(booking.bookingId)}
                            disabled={ticketLoading}
                            title="View Ticket Details"
                          >
                            <FaEye />
                          </button>
                          {booking.status !== "Cancelled" && (
                            <button
                              className="btn-action btn-cancel"
                              onClick={() => handleCancel(booking.bookingId)}
                              title="Cancel Booking"
                            >
                              <FaTimes />
                            </button>
                          )}
                        </div>
                      </td>
                    </tr>
                  )) : (
                    <tr>
                      <td colSpan="8" className="no-data-cell">
                        <div className="no-data-content">
                          <FaTicketAlt className="no-data-icon" />
                          <h5>No bookings found</h5>
                          <p>Your search or filter didn't match any bookings</p>
                        </div>
                      </td>
                    </tr>
                  )}
                </tbody>
              </table>
            </div>
          </div>

          {/* Custom Centered Modal - No External Dependencies */}
          <CenteredModal
            isOpen={showModal}
            onClose={() => {
              setShowModal(false);
              setSelectedBookingList([]);
            }}
            title={
              <>
                <FaTicketAlt className="me-2" />
                Ticket Details
              </>
            }
            labelledBy="ticket-modal-title"
          >
            {selectedBookingList.map((booking) => (
              <div
                key={booking.bookingId}
                ref={(el) => (ticketRef.current = el)}
                className="ticket-content"
                style={{
                  maxWidth: '210mm',
                  margin: '0 auto',
                  padding: '20px',
                  backgroundColor: '#ffffff',
                  borderRadius: '8px',
                  boxShadow: '0 0 20px rgba(0,0,0,0.1)'
                }}
              >
                {/* Ticket Header */}
                <div style={{
                  background: 'linear-gradient(135deg, #4a6cf7 0%, #2541b2 100%)',
                  color: 'white',
                  padding: '20px',
                  borderRadius: '8px 8px 0 0',
                  marginBottom: '20px',
                  position: 'relative',
                  overflow: 'hidden'
                }}>
                  <div style={{
                    position: 'absolute',
                    right: '20px',
                    top: '20px',
                    background: 'rgba(255,255,255,0.2)',
                    padding: '5px 10px',
                    borderRadius: '20px',
                    fontSize: '12px',
                    fontWeight: 'bold'
                  }}>
                    {booking.status}
                  </div>
                  <div style={{ textAlign: 'center', marginBottom: '10px' }}>
                    <FaBus style={{ fontSize: '2.5rem', marginBottom: '10px' }} />
                    <h2 style={{ margin: '0', fontWeight: '700', letterSpacing: '1px' }}>BUS TICKET</h2>
                    <p style={{ margin: '5px 0 0', opacity: '0.9', fontSize: '14px' }}>Booking ID: {booking.bookingId}</p>
                  </div>
                </div>

                {/* Journey Info */}
                <div style={{
                  background: '#f8f9fa',
                  padding: '20px',
                  borderRadius: '8px',
                  marginBottom: '20px',
                  position: 'relative',
                  border: '1px solid #e9ecef'
                }}>
                  <div style={{
                    display: 'flex',
                    justifyContent: 'space-between',
                    alignItems: 'center',
                    marginBottom: '15px',
                    position: 'relative'
                  }}>
                    <div style={{ textAlign: 'center', flex: 1 }}>
                      <div style={{
                        background: '#4a6cf7',
                        color: 'white',
                        width: '40px',
                        height: '40px',
                        borderRadius: '50%',
                        display: 'flex',
                        alignItems: 'center',
                        justifyContent: 'center',
                        margin: '0 auto 8px'
                      }}>
                        <FaMapMarkerAlt />
                      </div>
                      <div style={{ fontWeight: '600' }}>{booking.source}</div>
                    </div>
                    <div style={{
                      position: 'relative',
                      flex: 2,
                      padding: '0 20px',
                      textAlign: 'center'
                    }}>
                      <div style={{
                        height: '2px',
                        background: '#dee2e6',
                        position: 'relative',
                        margin: '20px 0'
                      }}>
                        <div style={{
                          position: 'absolute',
                          top: '-4px',
                          left: '50%',
                          transform: 'translateX(-50%)',
                          background: '#4a6cf7',
                          width: '10px',
                          height: '10px',
                          borderRadius: '50%'
                        }}></div>
                      </div>
                      <div style={{
                        background: '#4a6cf7',
                        color: 'white',
                        padding: '5px 10px',
                        borderRadius: '20px',
                        display: 'inline-block',
                        fontSize: '12px',
                        fontWeight: '600'
                      }}>
                        {booking.busName} ({booking.busnumber})
                      </div>
                    </div>
                    <div style={{ textAlign: 'center', flex: 1 }}>
                      <div style={{
                        background: '#ff6b6b',
                        color: 'white',
                        width: '40px',
                        height: '40px',
                        borderRadius: '50%',
                        display: 'flex',
                        alignItems: 'center',
                        justifyContent: 'center',
                        margin: '0 auto 8px'
                      }}>
                        <FaMapMarkerAlt />
                      </div>
                      <div style={{ fontWeight: '600' }}>{booking.destination}</div>
                    </div>
                  </div>
                  
                  <div style={{
                    display: 'flex',
                    justifyContent: 'space-between',
                    marginTop: '20px',
                    paddingTop: '15px',
                    borderTop: '1px dashed #dee2e6'
                  }}>
                    <div style={{ textAlign: 'center', flex: 1 }}>
                      <div style={{
                        color: '#6c757d',
                        fontSize: '12px',
                        marginBottom: '5px'
                      }}>Departure</div>
                      <div style={{ fontWeight: '600' }}>
                        {new Date(booking.bookingDate).toLocaleTimeString([], { hour: '2-digit', minute: '2-digit' })}
                      </div>
                      <div style={{
                        fontSize: '12px',
                        color: '#6c757d'
                      }}>
                        {new Date(booking.bookingDate).toLocaleDateString('en-US', { weekday: 'short', year: 'numeric', month: 'short', day: 'numeric' })}
                      </div>
                    </div>
                    <div style={{
                      textAlign: 'center',
                      flex: 1,
                      borderLeft: '1px solid #e9ecef',
                      borderRight: '1px solid #e9ecef'
                    }}>
                      <div style={{
                        color: '#6c757d',
                        fontSize: '12px',
                        marginBottom: '5px'
                      }}>Total Fare</div>
                      <div style={{
                        color: '#28a745',
                        fontWeight: '700',
                        fontSize: '1.5rem'
                      }}>
                        ₹{booking.totalFare?.toLocaleString('en-IN')}
                      </div>
                    </div>
                    <div style={{ textAlign: 'center', flex: 1 }}>
                      <div style={{
                        color: '#6c757d',
                        fontSize: '12px',
                        marginBottom: '5px'
                      }}>Seat{booking.seatNumber?.length > 1 ? 's' : ''}</div>
                      <div style={{
                        display: 'flex',
                        justifyContent: 'center',
                        flexWrap: 'wrap',
                        gap: '5px'
                      }}>
                        {booking.seatNumber?.map((seat, index) => (
                          <span key={index} style={{
                            background: '#4a6cf7',
                            color: 'white',
                            padding: '3px 10px',
                            borderRadius: '4px',
                            fontSize: '14px',
                            fontWeight: '600',
                            minWidth: '35px',
                            textAlign: 'center'
                          }}>
                            {seat}
                          </span>
                        ))}
                      </div>
                    </div>
                  </div>
                </div>

                {/* Passenger Details */}
                <div style={{
                  background: '#f8f9fa',
                  padding: '20px',
                  borderRadius: '8px',
                  marginBottom: '20px',
                  border: '1px solid #e9ecef'
                }}>
                  <h4 style={{
                    margin: '0 0 15px 0',
                    paddingBottom: '10px',
                    borderBottom: '2px solid #4a6cf7',
                    display: 'inline-block',
                    color: '#343a40',
                    fontSize: '1.1rem'
                  }}>
                    <FaUser style={{ marginRight: '8px', color: '#4a6cf7' }} />
                    Passenger Details
                  </h4>
                  
                  <div style={{
                    display: 'flex',
                    justifyContent: 'space-between',
                    marginBottom: '15px',
                    paddingBottom: '15px',
                    borderBottom: '1px dashed #dee2e6'
                  }}>
                    <div>
                      <div style={{
                        color: '#6c757d',
                        fontSize: '13px',
                        marginBottom: '2px'
                      }}>Booked By</div>
                      <div style={{
                        fontWeight: '600',
                        fontSize: '1rem'
                      }}>{booking.name}</div>
                      <div style={{
                        color: '#6c757d',
                        fontSize: '13px',
                        marginTop: '3px'
                      }}>Phone: {booking.phone}</div>
                    </div>
                    <div style={{
                      textAlign: 'right'
                    }}>
                      <div style={{
                        color: '#6c757d',
                        fontSize: '13px',
                        marginBottom: '2px'
                      }}>Booking Date</div>
                      <div style={{
                        fontWeight: '600',
                        fontSize: '0.9rem'
                      }}>
                        {new Date(booking.bookingDate).toLocaleDateString('en-US', { weekday: 'short', year: 'numeric', month: 'short', day: 'numeric' })}
                      </div>
                      <div style={{
                        color: '#6c757d',
                        fontSize: '13px',
                        marginTop: '3px'
                      }}>
                        {new Date(booking.bookingDate).toLocaleTimeString()}
                      </div>
                    </div>
                  </div>

                  {/* Passengers List */}
                  {booking.passangers?.length > 0 && (
                    <div>
                      <h5 style={{
                        margin: '20px 0 10px',
                        color: '#495057',
                        fontSize: '0.95rem',
                        display: 'flex',
                        alignItems: 'center'
                      }}>
                        <FaUsers style={{ marginRight: '8px', color: '#4a6cf7' }} />
                        Passenger{booking.passangers.length > 1 ? 's' : ''} ({booking.passangers.length})
                      </h5>
                      <div style={{
                        display: 'grid',
                        gridTemplateColumns: 'repeat(auto-fill, minmax(250px, 1fr))',
                        gap: '10px',
                        marginTop: '10px'
                      }}>
                        {booking.passangers.map((p, index) => (
                          <div key={p.passengerId} style={{
                            background: 'white',
                            padding: '12px 15px',
                            borderRadius: '6px',
                            border: '1px solid #e9ecef',
                            position: 'relative',
                            boxShadow: '0 2px 4px rgba(0,0,0,0.03)'
                          }}>
                            <div style={{
                              position: 'absolute',
                              top: '-10px',
                              left: '10px',
                              background: '#4a6cf7',
                              color: 'white',
                              width: '20px',
                              height: '20px',
                              borderRadius: '50%',
                              display: 'flex',
                              alignItems: 'center',
                              justifyContent: 'center',
                              fontSize: '10px',
                              fontWeight: 'bold'
                            }}>
                              {index + 1}
                            </div>
                            <div style={{
                              display: 'flex',
                              justifyContent: 'space-between',
                              alignItems: 'center'
                            }}>
                              <div>
                                <div style={{
                                  fontWeight: '600',
                                  marginBottom: '3px',
                                  fontSize: '0.95rem'
                                }}>{p.passengerName}</div>
                                <div style={{
                                  display: 'flex',
                                  gap: '10px',
                                  fontSize: '12px',
                                  color: '#6c757d'
                                }}>
                                  <span>Age: {p.passengerAge} yrs</span>
                                  <span>•</span>
                                  <span>Gender: {p.passengerGender}</span>
                                </div>
                              </div>
                              <div style={{
                                background: '#e9f7ef',
                                color: '#28a745',
                                padding: '3px 8px',
                                borderRadius: '4px',
                                fontSize: '11px',
                                fontWeight: '600',
                                whiteSpace: 'nowrap'
                              }}>
                                {p.seatNumber ? `Seat: ${p.seatNumber}` : 'Seat Assigned'}
                              </div>
                            </div>
                          </div>
                        ))}
                      </div>
                    </div>
                  )}
                </div>

                {/* Footer */}
                <div style={{
                  textAlign: 'center',
                  padding: '15px',
                  color: '#6c757d',
                  fontSize: '12px',
                  borderTop: '1px solid #e9ecef',
                  marginTop: '20px',
                  position: 'relative'
                }}>
                  <div style={{
                    position: 'absolute',
                    top: '-10px',
                    left: '50%',
                    transform: 'translateX(-50%)',
                    background: 'white',
                    padding: '0 15px',
                    color: '#6c757d',
                    fontSize: '12px'
                  }}>
                    <FaBus style={{ marginRight: '5px' }} />
                    {booking.busName || 'Bus Booking System'}
                  </div>
                  <p style={{ margin: '5px 0' }}>
                    Thank you for choosing our service. Have a safe journey!
                  </p>
                  <p style={{ margin: '5px 0 0', fontWeight: '500' }}>
                    For any assistance, please contact: support@busbookingsystem.com
                  </p>
                </div>

                {/* Download Button */}
                <div className="text-center">
                  <button
                    className="btn btn-primary btn-lg download-btn"
                    style={{
                      background: 'linear-gradient(135deg, #4a6cf7 0%, #2541b2 100%)',
                      border: 'none',
                      padding: '12px 30px',
                      borderRadius: '50px',
                      fontWeight: '600',
                      fontSize: '1rem',
                      boxShadow: '0 4px 15px rgba(74, 108, 247, 0.3)',
                      transition: 'all 0.3s ease',
                      display: 'inline-flex',
                      alignItems: 'center',
                      justifyContent: 'center',
                      margin: '0 auto',
                      width: 'auto',
                      minWidth: '220px',
                      height: 'auto',
                      textTransform: 'none',
                      letterSpacing: '0.5px'
                    }}
                    onMouseOver={(e) => {
                      e.currentTarget.style.transform = 'translateY(-2px)';
                      e.currentTarget.style.boxShadow = '0 6px 20px rgba(74, 108, 247, 0.4)';
                    }}
                    onMouseOut={(e) => {
                      e.currentTarget.style.transform = 'translateY(0)';
                      e.currentTarget.style.boxShadow = '0 4px 15px rgba(74, 108, 247, 0.3)';
                    }}
                    onClick={async () => {
                      try {
                        if (ticketRef.current) {
                          setTicketLoading(true);
                          
                          // Set a fixed width for the ticket content
                          const ticketElement = ticketRef.current;
                          const originalWidth = ticketElement.style.width;
                          ticketElement.style.width = '210mm'; // A4 width
                          
                          // Create a clone of the element to avoid affecting the original
                          const clone = ticketElement.cloneNode(true);
                          clone.style.position = 'absolute';
                          clone.style.left = '-9999px';
                          document.body.appendChild(clone);
                          
                          // Add a small delay to ensure proper rendering
                          await new Promise(resolve => setTimeout(resolve, 300));
                          
                          // Generate the canvas with higher quality
                          const canvas = await html2canvas(clone, {
                            scale: 2,
                            backgroundColor: '#ffffff',
                            useCORS: true,
                            logging: false,
                            allowTaint: true,
                            scrollX: 0,
                            scrollY: 0,
                            windowWidth: document.documentElement.offsetWidth,
                            windowHeight: document.documentElement.offsetHeight
                          });
                          
                          // Remove the clone
                          document.body.removeChild(clone);
                          
                          // Restore original width
                          ticketElement.style.width = originalWidth;
                          
                          // Calculate dimensions for PDF
                          const imgData = canvas.toDataURL('image/png');
                          const pdf = new jsPDF({
                            orientation: 'portrait',
                            unit: 'mm',
                            format: 'a4'
                          });
                          
                          // Add margin to the PDF
                          const margin = 10; // 10mm margin
                          const pdfWidth = 210 - (2 * margin);
                          const pageHeight = 297 - (2 * margin);
                          
                          // Calculate the height that maintains aspect ratio
                          const imgHeight = (canvas.height * pdfWidth) / canvas.width;
                          
                          // Add the image to PDF
                          pdf.addImage(imgData, 'PNG', margin, margin, pdfWidth, imgHeight);
                          
                          // Add a second page if the content is too long
                          if (imgHeight > pageHeight) {
                            pdf.addPage();
                            const remainingHeight = imgHeight - (pageHeight - margin);
                            pdf.addImage(
                              imgData,
                              'PNG',
                              margin,
                              -pageHeight + margin,
                              pdfWidth,
                              imgHeight
                            );
                          }
                          
                          // Save the PDF
                          pdf.save(`Ticket_${booking.bookingId}.pdf`);
                          
                          // Show success message
                          setToastMessage('Ticket downloaded successfully!');
                          setShowToast(true);
                        }
                      } catch (error) {
                        console.error('Error generating PDF:', error);
                        setToastMessage('Failed to download ticket. Please try again.');
                        setShowToast(true);
                      } finally {
                        setTicketLoading(false);
                      }
                    }}
                  >
                    <FaDownload className="me-2" />
                    Download Ticket
                  </button>
                </div>
              </div>
            ))}
          </CenteredModal>
        </div>
      </div>

      <ToastContainer position="top-end" className="p-3">
        <Toast
          show={showToast}
          onClose={() => setShowToast(false)}
          delay={3000}
          autohide
        >
          <Toast.Header>
            <strong className="me-auto">Booking Update</strong>
            <small>Just now</small>
          </Toast.Header>
          <Toast.Body>{toastMessage}</Toast.Body>
        </Toast>
      </ToastContainer>
    </>
  );
}

export default Navbooking;
