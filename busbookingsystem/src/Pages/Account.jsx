// ============================================================================
// Project Name : BusBookingSystem
// File Name    : Account.jsx
// Created By   : Kaviraj M
// Created On   : 24/10/2025
// Modified By  : Kaviraj M
// Modified On  : 28/10/2025
// Description  : React component that provides a slide-out user account panel
//                for the Bus Booking System. It includes a login form and 
//                placeholder for user profile information, integrated into
//                the main navigation as an offcanvas component.
// ============================================================================

import React, { useState } from "react";
import { Offcanvas, Button } from "react-bootstrap";

/**
 * Account Component
 * @param {Object} props - Component properties
 * @param {boolean} props.show - Controls the visibility of the offcanvas
 * @param {Function} props.handleClose - Callback function to close the offcanvas
 * @returns {JSX.Element} The Account component UI
 */
function Account({ show, handleClose }) {
  // State for form inputs
  const [email, setEmail] = useState("");
  const [password, setPassword] = useState("");

  /**
   * Handles the login form submission
   * @param {Event} e - The form submission event
   */
  const handleLogin = (e) => {
    e.preventDefault();
    console.log({ email, password });
    handleClose(); // Close offcanvas after login
  };

  // ========================================================================
  // Main Render
  // Renders the offcanvas component with login form and profile section
  // ========================================================================
  return (
    <Offcanvas show={show} onHide={handleClose} placement="end">
      <Offcanvas.Header closeButton>
        <Offcanvas.Title>User Account</Offcanvas.Title>
      </Offcanvas.Header>
      <Offcanvas.Body>
        {/* Login Form Section */}
        <h5 className="fw-bold mb-3">Login</h5>
        <form onSubmit={handleLogin}>
          <div className="mb-3">
            <label className="form-label">Email</label>
            <input
              type="email"
              className="form-control"
              placeholder="Enter email"
              value={email}
              onChange={(e) => setEmail(e.target.value)}
              required
            />
          </div>
          <div className="mb-3">
            <label className="form-label">Password</label>
            <input
              type="password"
              className="form-control"
              placeholder="Enter password"
              value={password}
              onChange={(e) => setPassword(e.target.value)}
              required
            />
          </div>
          <Button variant="danger" type="submit" className="w-100 fw-semibold">
            Login
          </Button>
        </form>

        <hr />

        {/* Profile Section */}
        <h6 className="fw-bold">Profile</h6>
        <p className="text-muted">
          Once logged in, your profile details will appear here.
        </p>
      </Offcanvas.Body>
    </Offcanvas>
  );
}

export default Account;
