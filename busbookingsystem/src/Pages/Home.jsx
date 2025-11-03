// ============================================================================
// Project Name : BusBookingSystem
// File Name    : HomePage.jsx
// Created By   : Kaviraj M
// Created On   : 23/09/2025
// Modified By  : Kaviraj M
// Modified On  : 28/10/2025
// Description  : Displays the homepage of the Bus Booking System, enabling users 
//                to search bus routes, access their profile, and manage bookings. 
//                Includes animated UI elements, toast notifications, and 
//                responsive layout using Bootstrap.
// ============================================================================

import React, { useState, useEffect, useRef } from "react";
import {
  FaSearch,
  FaMapMarkerAlt,
  FaCalendarAlt,
  FaFacebook,
  FaTwitter,
  FaInstagram,
  FaLinkedin,
  FaExclamationCircle,
  FaUserCircle,
  FaUser,
  FaSignOutAlt,
} from "react-icons/fa";
import {
  Container,
  Row,
  Col,
  Form,
  Button,
  Card,
  Toast,
  ToastContainer,
  Badge,
  Offcanvas,
} from "react-bootstrap";
import { useNavigate } from "react-router-dom";
import axios from "axios";
import ProfilePage from "./Userprofile";
import "./Home.css";

function HomePage() {
  // --------------------------------------------------------------------------
  // 🔹 State Management
  // --------------------------------------------------------------------------
  const [formData, setFormData] = useState({
    fromCity: "",
    toCity: "",
    journeyDate: "",
  });
  const [fromCities, setFromCities] = useState([]);
  const [toCities, setToCities] = useState([]);
  const [errorMessage, setErrorMessage] = useState("");
  const [showToast, setShowToast] = useState(false);

  const navigate = useNavigate();
  const today = new Date().toISOString().split("T")[0];

  // --------------------------------------------------------------------------
  // 🔹 Scroll Animation using Intersection Observer
  // --------------------------------------------------------------------------
  const animatedRefs = useRef([]);
  animatedRefs.current = [];
  const addToRefs = (el) => {
    if (el && !animatedRefs.current.includes(el)) animatedRefs.current.push(el);
  };

  useEffect(() => {
    const observer = new IntersectionObserver(
      (entries) => {
        entries.forEach((entry) => {
          if (entry.isIntersecting) {
            entry.target.classList.add("animate-slideUp");
            observer.unobserve(entry.target);
          }
        });
      },
      { threshold: 0.2 }
    );
    animatedRefs.current.forEach((el) => observer.observe(el));
    return () => observer.disconnect();
  }, []);

  // --------------------------------------------------------------------------
  // 🔹 Fetch Available Routes
  // --------------------------------------------------------------------------
  useEffect(() => {
    const fetchRoutes = async () => {
      try {
        const response = await axios.get("https://localhost:7272/api/Routes/GetAllRoutes");
        const routes = response.data;

        const uniqueFrom = [...new Set(routes.map((r) => r.source))];
        const uniqueTo = [...new Set(routes.map((r) => r.destination))];

        setFromCities(uniqueFrom);
        setToCities(uniqueTo);
      } catch (error) {
        console.error("Failed to fetch routes:", error);
      }
    };
    fetchRoutes();
  }, []);

  // --------------------------------------------------------------------------
  // 🔹 Handle Input Changes
  // --------------------------------------------------------------------------
  const handleChange = (e) => {
    const { name, value } = e.target;
    setFormData((prev) => ({ ...prev, [name]: value }));
  };

  // --------------------------------------------------------------------------
  // 🔹 Handle Search Functionality
  // --------------------------------------------------------------------------
  const handleSearch = async () => {
    const { fromCity, toCity, journeyDate } = formData;
    let missingFields = [];

    if (!fromCity) missingFields.push("From City");
    if (!toCity) missingFields.push("To City");
    if (!journeyDate) missingFields.push("Journey Date");

    if (missingFields.length > 0) {
      setErrorMessage(`Please fill: ${missingFields.join(", ")}`);
      setShowToast(true);
      return;
    }

    const todayDate = new Date();
    todayDate.setHours(0, 0, 0, 0);
    const selectedDate = new Date(journeyDate);

    if (selectedDate < todayDate) {
      setErrorMessage("⚠️ Please select a valid journey date (not in the past).");
      setShowToast(true);
      return;
    }

    try {
      // Format the date to YYYY-MM-DD
      const formattedDate = new Date(journeyDate).toISOString().split('T')[0];
      
      console.log('Searching buses with params:', {
        source: fromCity.trim(),
        destination: toCity.trim(),
        travelDate: formattedDate
      });
      
      const response = await axios.get("https://localhost:7272/api/Routes/FilterRoutes", {
        params: {
          source: fromCity.trim(),
          destination: toCity.trim(),
          travelDate: formattedDate
        },
        headers: {
          'Content-Type': 'application/json',
        },
        paramsSerializer: params => {
          return Object.entries(params)
            .map(([key, value]) => `${encodeURIComponent(key)}=${encodeURIComponent(value)}`)
            .join('&');
        }
      });

      if (response.data && response.data.length > 0) {
        navigate("/searchresult", { 
          state: { 
            ...formData,
            travelDate: formattedDate // Use the already formatted date
          } 
        });
      } else {
        setErrorMessage("😔 No buses found for this route.");
        setShowToast(true);
      }
    } catch (error) {
      console.error("Error fetching routes:", error);
      setErrorMessage("Failed to search buses. Please try again.");
      setShowToast(true);
    }
  };

  // --------------------------------------------------------------------------
  // 🔹 Logout Functionality
  // --------------------------------------------------------------------------
  const handleLogout = () => {
    localStorage.removeItem("token");
    navigate("/login");
  };

  // --------------------------------------------------------------------------
  // 🔹 Component JSX
  // --------------------------------------------------------------------------
  return (
    <>

      {/* 🔹 Hero Section */}
      <section className="hero-section d-flex align-items-center justify-content-center">
        <Container className="text-center text-white py-5">
          <h1 className="searchheadh1 fw-bold mb-4 hero-animate" ref={addToRefs}>
            Book Your Bus Tickets Online
          </h1>
          <p className="searchheadp lead mb-4 hero-animate" ref={addToRefs}>
            Fast, Safe & Easy Bus Tickets Booking
          </p>

          {/* 🔹 Search Card */}
          <Card className="search-card shadow-lg p-4 hero-animate border-0" ref={addToRefs}>
            <Card.Header className="bg-danger text-white text-center mb-3">
              <h5 className="mb-0">Search Buses</h5>
            </Card.Header>
            <Card.Body>
              <Row className="g-3 align-items-end">
                <Col md={4}>
                  <Form.Group>
                    <Form.Label>
                      <FaMapMarkerAlt className="me-1" /> From City
                    </Form.Label>
                    <Form.Control
                      list="fromCitiesList"
                      name="fromCity"
                      value={formData.fromCity}
                      onChange={handleChange}
                      placeholder="Select departure city"
                      className="form-control-lg"
                    />
                    <datalist id="fromCitiesList">
                      {fromCities.map((city, idx) => (
                        <option key={idx} value={city} />
                      ))}
                    </datalist>
                  </Form.Group>
                </Col>

                <Col md={4}>
                  <Form.Group>
                    <Form.Label>
                      <FaMapMarkerAlt className="me-1" /> To City
                    </Form.Label>
                    <Form.Control
                      list="toCitiesList"
                      name="toCity"
                      value={formData.toCity}
                      onChange={handleChange}
                      placeholder="Select destination city"
                      className="form-control-lg"
                    />
                    <datalist id="toCitiesList">
                      {toCities.map((city, idx) => (
                        <option key={idx} value={city} />
                      ))}
                    </datalist>
                  </Form.Group>
                </Col>

                <Col md={3}>
                  <Form.Group>
                    <Form.Label>
                      <FaCalendarAlt className="me-1" /> Journey Date
                    </Form.Label>
                    <Form.Control
                      type="date"
                      name="journeyDate"
                      value={formData.journeyDate}
                      onChange={handleChange}
                      min={today}
                      className="form-control-lg"
                    />
                  </Form.Group>
                </Col>

                <Col md={1} className="text-center">
                  <Button
                    type="button"
                    variant="danger"
                    size="lg"
                    onClick={handleSearch}
                    className="w-100"
                  >
                    <FaSearch />
                  </Button>
                </Col>
              </Row>
            </Card.Body>
          </Card>
        </Container>
      </section>

      {/* 🔹 Toast Notifications */}
      <ToastContainer position="top-end" className="p-3 mt-5">
        <Toast
          show={showToast}
          onClose={() => setShowToast(false)}
          delay={5000}
          autohide
          style={{
            backgroundColor: "#fff9f9ff",
            color: "black",
            borderRadius: "12px",
            minWidth: "300px",
          }}
        >
          <Toast.Header closeButton style={{ backgroundColor: "#d34048ff", color: "#f0f0f0ff" }}>
            <FaExclamationCircle className="me-2" />
            <strong className="me-auto">Attention</strong>
          </Toast.Header>
          <Toast.Body>{errorMessage}</Toast.Body>
        </Toast>
      </ToastContainer>

      {/* 🔹 Marquee */}
      <div className="marquee shadow-sm">
        <div className="marquee-content mt-2 pt-1 pb-1">
          🎉 Welcome to BusBooking – Safe, Fast & Easy Bus Tickets | Enjoy Your Journey &
          Create Unforgettable Memories | 24/7 Support Available 🎉
        </div>
      </div>

      {/* 🔹 Info Section */}
      <section className="ms-3 mt-4">
        <h4 className="fw-bold scroll-animate" ref={addToRefs}>
          BusBooking: Simplifying Bus Booking for Everyone
        </h4>
        <p className="text-muted mt-3 scroll-animate" ref={addToRefs}>
          BusBooking is a leading platform trusted by millions across South India for a seamless
          booking experience.
        </p>
        <p className="text-muted scroll-animate" ref={addToRefs}>
          With 10+ partners and multiple routes available, users can find the best travel options
          easily.
        </p>
        <h5 className="fw-bold mt-4 text-muted scroll-animate" ref={addToRefs}>
          Why Choose BusBooking?
        </h5>
        <p className="text-muted mt-3 scroll-animate" ref={addToRefs}>
          <span className="fw-bold">Easy Cancellation –</span> Cancel booking before 12 hours of
          journey start.
        </p>
        <p className="text-muted scroll-animate" ref={addToRefs}>
          <span className="fw-bold">Student Offers –</span> Get exclusive discounts and quick
          support.
        </p>
        <p className="text-muted mb-4 scroll-animate" ref={addToRefs}>
          <span className="fw-bold">24/7 Support –</span> Always available for your travel queries.
        </p>
      </section>

      {/* 🔹 Footer */}
      <footer className="bg-dark text-white mt-5 pt-4 pb-3">
        <Container>
          <Row>
            <Col md={4}>
              <h5 className="fw-bold text-danger">BusBooking</h5>
              <p className="small text-muted">
                Your trusted platform for safe, fast, and easy bus booking.
              </p>
              <Badge bg="danger" className="mt-2">
                Trusted by 100M+ Users
              </Badge>
            </Col>

            <Col md={4}>
              <h6 className="fw-bold">Quick Links</h6>
              <ul className="list-unstyled text-muted">
                <li>
                  <a href="/home" className="text-white text-decoration-none">
                    Home
                  </a>
                </li>
                <li>
                  <a href="/bookings" className="text-white text-decoration-none">
                    Book Tickets
                  </a>
                </li>
                <li>
                  <a href="/help" className="text-white text-decoration-none">
                    Help & Support
                  </a>
                </li>
                <li>
                  <a href="/profile" className="text-white text-decoration-none">
                    Profile
                  </a>
                </li>
              </ul>
            </Col>

            <Col md={4}>
              <h6 className="fw-bold">Follow Us</h6>
              <div className="d-flex gap-3 mt-2">
                <a href="https://www.facebook.com/" className="text-white fs-5">
                  <FaFacebook />
                </a>
                <a href="https://x.com" className="text-white fs-5">
                  <FaTwitter />
                </a>
                <a href="https://www.instagram.com/" className="text-white fs-5">
                  <FaInstagram />
                </a>
                <a href="https://in.linkedin.com" className="text-white fs-5">
                  <FaLinkedin />
                </a>
              </div>
              <p className="small text-light mt-3">📞 +91 98765 43210</p>
              <p className="small text-light">✉️ support@busbooking.com</p>
            </Col>
          </Row>

          <hr className="border-secondary" />
          <p className="text-center small mb-0">
            © {new Date().getFullYear()} BusBooking. All rights reserved.
          </p>
        </Container>
      </footer>
    </>
  );
}

export default HomePage;
