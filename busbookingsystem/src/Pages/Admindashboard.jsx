// ============================================================================
// Project Name : BusBookingSystem
// File Name    : Admindashboard.jsx
// Created By   : Kaviraj M
// Created On   : 23/09/2025
// Modified By  : Kaviraj M
// Modified On  : 28/10/2025
// Description  : Main layout component for the admin dashboard, providing
//                navigation and a consistent layout for all admin pages.
// ============================================================================

import React from "react";
import { FaHome, FaRoad, FaTicketAlt, FaUsers, FaBus, FaSignOutAlt } from "react-icons/fa";
import { useNavigate, Link, Outlet } from "react-router-dom";
import "bootstrap/dist/css/bootstrap.min.css";

/**
 * AdminDashboard Component
 * 
 * Serves as the main layout for the admin section of the application.
 * Provides navigation between different admin sections and a consistent
 * layout for all admin pages.
 * 
 * @component
 * @returns {JSX.Element} The admin dashboard layout with navigation and content area
 */
function AdminDashboard() {
  const navigate = useNavigate();

  // ==========================================================================
  // EVENT HANDLERS
  // ==========================================================================
  
  /**
   * Handles admin logout
   * Removes user session and redirects to login page
   */
  const handleLogout = () => {
    localStorage.removeItem("user");
    navigate("/login");
  };

  // ==========================================================================
  // RENDER
  // ==========================================================================
  
  return (
    <div className="d-flex flex-column min-vh-100">
      {/* ====================================================================
         TOP NAVIGATION BAR
         ==================================================================== */}
      <nav className="navbar navbar-expand-lg navbar-dark bg-dark px-3">
        <Link className="navbar-brand d-flex align-items-center" to="/admin/home">
          <FaBus className="me-2 text-danger" size={24} />
          <span>Bus Admin</span>
        </Link>

        <button
          className="navbar-toggler"
          type="button"
          data-bs-toggle="collapse"
          data-bs-target="#adminNavbar"
          aria-controls="adminNavbar"
          aria-expanded="false"
          aria-label="Toggle navigation"
        >
          <span className="navbar-toggler-icon"></span>
        </button>

        <div className="collapse navbar-collapse" id="adminNavbar">
          <ul className="navbar-nav me-auto mb-2 mb-lg-0">
            <li className="nav-item">
              <Link to="home" className="nav-link">
                <FaHome className="me-1 mx-1" /> Home
              </Link>
            </li>
            <li className="nav-item">
              <Link to="route" className="nav-link">
                <FaRoad className="me-1 mx-1" /> Route
              </Link>
            </li>
            <li className="nav-item">
              <Link to="bookings" className="nav-link">
                <FaTicketAlt className="me-1 mx1" /> Booking
              </Link>
            </li>
            <li className="nav-item">
              <Link to="users" className="nav-link">
                <FaUsers className="me-1 mx-1" /> User
              </Link>
            </li>
            <li className="nav-item">
              <Link to="buses" className="nav-link">
                <FaBus className="me-1 mx-1" /> Buses
              </Link>
            </li>
            
          </ul>

          <button
            onClick={handleLogout}
            className="btn btn-outline-light d-flex align-items-center"
          >
            <FaSignOutAlt className="me-2" /> Logout
          </button>
        </div>
      </nav>

      {/* ====================================================================
         MAIN CONTENT AREA
         ==================================================================== */}
      <main className="flex-grow-1 p-4 bg-light">
        {/* Nested routes render here */}
        <Outlet />
      </main>
    </div>
  );
}

export default AdminDashboard;
