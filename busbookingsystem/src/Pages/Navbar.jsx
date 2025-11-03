// ============================================================================
// Project Name : BusBookingSystem
// File Name    : Navbar.jsx
// Created By   : Kaviraj M
// Created On   : 22/09/2025
// Modified By  : Kaviraj M
// Modified On  : 30/10/2025
// Description  : React component for the main navigation bar of the Bus 
//                Booking System. Includes responsive design and 
//                authentication-based navigation controls. 
//                (Profile now navigates to UserProfile page.)
// ============================================================================

import React, { useEffect, useState } from "react";
import {
  Navbar,
  Container,
  Nav,
  NavDropdown,
  Button,
  Offcanvas,
  Modal,
} from "react-bootstrap";
import "./Navbar.css";
import {
  FaBus,
  FaQuestionCircle,
  FaHome,
  FaSignOutAlt,
  FaUserCircle,
  FaUserAlt,
  FaTicketAlt,
} from "react-icons/fa";
import UserProfile from "./Userprofile";
import { Link, useNavigate } from "react-router-dom";
import { useForm } from "react-hook-form";

function BusNavbar() {
  const navigate = useNavigate();
  // const { control, setValue, getValues, watch } = useForm({
  //   defaultValues : {
  //     txtNameBox: ''
  //   }
  // });

  // const nameValue = getValues("txtNameBox");
  
  const [showOffcanvas, setShowOffcanvas] = useState(false);
  const [showProfileModal, setShowProfileModal] = useState(false);

  // 🔹 Logout handler
  const handleLogout = () => {
    localStorage.clear();
    sessionStorage.clear();
    navigate("/login");
  };

   const handleProfileClick = () => {
    setShowProfileModal(true);
  };

  // useEffect(() => {

  // }, [showOffcanvas]);

  return (
    <>
      {/* ✅ Navbar */}
      <Navbar expand="lg" sticky="top" className="BBSnavbar shadow-sm bg-light">
        <Container fluid className="px-3">
          <Navbar.Brand
            as={Link}
            to="/Home"
            className="fw-bold text-dark d-flex align-items-center ms-2"
          >
            <FaBus className="me-2 text-danger pe-1" size={32} />
            BusBooking
          </Navbar.Brand>

          {/* ✅ Mobile Menu Button */}
          <Button
            variant="outline-danger"
            className="d-lg-none"
            onClick={() => setShowOffcanvas(true)}
          >
            ☰
          </Button>

          {/* ✅ Desktop Menu */}
          <Navbar.Collapse id="navbarScroll" className="justify-content-end">
            <Nav>
              <Nav.Link
                as={Link}
                to="/Home"
                className="fw-semibold text-dark mx-3 d-flex align-items-center"
              >
                <FaHome className="me-2 text-danger" /> Home
              </Nav.Link>

              <Nav.Link
                as={Link}
                to="/Bookings"
                className="fw-semibold text-dark mx-3 d-flex align-items-center"
              >
                <FaTicketAlt size={20} className="me-2 text-danger" /> Bookings
              </Nav.Link>

              <Nav.Link
                as={Link}
                to="/help"
                className="fw-semibold text-dark mx-3 d-flex align-items-center"
              >
                <FaQuestionCircle className="me-1 text-danger" /> Help
              </Nav.Link>

              <NavDropdown
                title={<FaUserCircle size={28} className="text-danger" />}
                id="account-dropdown"
                align="end"
                className="fw-semibold text-dark mx-3 no-caret"
              >
                <NavDropdown.Item onClick={handleProfileClick}>
                  <FaUserAlt className="me-2 text-danger" size={18} /> Profile
                </NavDropdown.Item>
                <NavDropdown.Item onClick={handleLogout}>
                  <FaSignOutAlt className="me-2 text-danger" size={18} /> Logout
                </NavDropdown.Item>
              </NavDropdown>
            </Nav>
          </Navbar.Collapse>
        </Container>
      </Navbar>

      {/* ✅ Offcanvas (Mobile Menu) */}
      <Offcanvas
        show={showOffcanvas}
        onHide={() => setShowOffcanvas(false)}
        placement="end"
        className="custom-offcanvas"
      >
        <Offcanvas.Header closeButton>
          <Offcanvas.Title>Menu</Offcanvas.Title>
        </Offcanvas.Header>
        <Offcanvas.Body>
          <Nav className="flex-column">
            <Nav.Link
              as={Link}
              to="/Home"
              onClick={() => setShowOffcanvas(false)}
            >
              <FaHome className="me-2 text-danger" /> Home
            </Nav.Link>

            <Nav.Link
              as={Link}
              to="/Bookings"
              onClick={() => setShowOffcanvas(false)}
            >
              <FaTicketAlt className="me-2 text-danger" /> Bookings
            </Nav.Link>

            <Nav.Link
              as={Link}
              to="/help"
              onClick={() => setShowOffcanvas(false)}
            >
              <FaQuestionCircle className="me-2 text-danger" /> Help
            </Nav.Link>

            <Nav.Link
              onClick={() => {
                setShowOffcanvas(false);
                navigate("/UserProfile");
              }}
            >
              <FaUserAlt className="me-2 text-danger" /> Profile
            </Nav.Link>

            <Nav.Link onClick={handleLogout}>
              <FaSignOutAlt className="me-2 text-danger" /> Logout
            </Nav.Link>
          </Nav>
        </Offcanvas.Body>
      </Offcanvas>
       {/* ✅ Profile Modal */}
      <Modal
        show={showProfileModal}
        onHide={() => setShowProfileModal(false)}
        size="lg"
        centered
        fullscreen="md-down"
      >
        <Modal.Header closeButton>
          <Modal.Title>User Profile</Modal.Title>
        </Modal.Header>
        <Modal.Body>
          {/* Render your UserProfile page here */}
          <UserProfile />
        </Modal.Body>
      </Modal>
    </>
  );
}

export default BusNavbar;
