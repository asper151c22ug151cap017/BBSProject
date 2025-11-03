// ============================================================================
// Project Name : BusBookingSystem
// File Name    : landingpage.jsx
// Created By   : Kaviraj M
// Created On   : 22/09/2025
// Modified By  : Kaviraj M
// Modified On  : 28/10/2025
// Description  : Landing page component that serves as the main entry point for
//                the application, featuring hero section, features, and call-to-action.
// ============================================================================

import React, { useEffect } from "react";
import { Container, Row, Col, Button, Card, Badge, Alert } from "react-bootstrap";
import { FaBus, FaSignInAlt, FaUserAlt, FaShieldAlt, FaBolt, FaThumbsUp, FaArrowRight, FaStar, FaUsers, FaRoute, FaTicketAlt, FaFacebook, FaTwitter, FaInstagram } from "react-icons/fa";
import { useNavigate } from "react-router-dom";
import "./landingpage.css";

/**
 * LandingPage Component
 * 
 * Main landing page component that includes:
 * - Hero section with call-to-action
 * - Key features and benefits
 * - Testimonials and social proof
 * - Responsive design for all screen sizes
 * 
 * @component
 * @returns {JSX.Element} The landing page interface
 */
function LandingPage() {
  const navigate = useNavigate();

  // ==========================================================================
  // EFFECTS
  // ==========================================================================
  
  /**
   * Sets up intersection observer for scroll animations
   * @effect
   */
  useEffect(() => {
    const sections = document.querySelectorAll(".fold-section");

    const observer = new IntersectionObserver(
      (entries) => {
        entries.forEach((entry) => {
          if (entry.isIntersecting) {
            entry.target.classList.add("visible");
          }
        });
      },
      { threshold: 0.25 }
    );

    sections.forEach((section) => observer.observe(section));

    return () => observer.disconnect();
  }, []);

  // ==========================================================================
  // RENDER
  // ==========================================================================
  
  return (
    <div className="min-vh-100 d-flex flex-column bg-light">
      {/* Top Bar */}
      <nav className="navbar navbar-expand-lg navbar-light bg-white border-bottom px-3 px-md-4 shadow-sm" style={{position:"sticky", top:"0px", zIndex:"1000"}}>
        <a className="navbar-brand d-flex align-items-center" href="#">
          <FaBus className="me-2 text-danger" size={32} />
          <span className="fw-bold fs-4">BusBooking</span>
        </a>
        <div className="ms-auto d-flex align-items-center gap-2">
          <Button variant="outline-secondary" className="d-flex align-items-center" onClick={() => navigate("/login")}>
            <FaSignInAlt className="me-2" /> Login
          </Button>
          <Button variant="danger" className="d-flex align-items-center" onClick={() => navigate("/signup")}>
            <FaUserAlt className="me-2" /> Signup
          </Button>
        </div>
      </nav>

      {/* Hero Section */}
      <header className="simple-hero">
        <div className="floating-buses">
          <FaBus className="floating-bus bus-1" />
          <FaBus className="floating-bus bus-2" />
          <FaBus className="floating-bus bus-3" />
        </div>
        <Container>
          <Row className="justify-content-center">
            <Col md={8} lg={6}>
              <Card className="hero-minicard border-0">
                <Card.Body className="p-4 p-md-5">
                  <Badge bg="danger" className="rounded-pill brand-kicker mb-2">BusBooking</Badge>
                  <h1 className="fw-bold mb-2">Modern bus booking for everyone</h1>
                  <p className="text-muted mb-4">Our platform helps travelers find reliable routes, compare options, and checkout securely. Built for speed, trust and simplicity.</p>
                  <ul className="list-unstyled mb-4 info-bullets">
                    <li className="d-flex align-items-start mb-2">
                      <span className="text-danger me-2"><FaBolt /></span>
                      <span>Instant results with real-time seat visibility</span>
                    </li>
                    <li className="d-flex align-items-start mb-2">
                      <span className="text-danger me-2"><FaShieldAlt /></span>
                      <span>Secure payments and verified bus operators</span>
                    </li>
                    <li className="d-flex align-items-start">
                      <span className="text-danger me-2"><FaThumbsUp /></span>
                      <span>Simple experience across web and mobile</span>
                    </li>
                  </ul>
                  <div className="d-flex gap-2">
                    <Button size="lg" variant="danger" onClick={() => navigate("/signup")}>
                      Get Started <FaArrowRight className="ms-2" />
                    </Button>
                    <Button size="lg" variant="outline-secondary" onClick={() => navigate("/login")}>
                      Login
                    </Button>
                  </div>
                </Card.Body>
              </Card>
            </Col>
          </Row>
        </Container>
      </header>

      {/* About */}
      <section className="py-5 bg-white border-top fold-section">
        <Container>
          <Row className="justify-content-center text-center mb-4">
            <Col md={8}>
              <h2 className="fw-bold">About the application</h2>
              <p className="text-muted">BusBooking streamlines intercity travel with a fast search experience, transparent pricing, and a secure checkout. Operators are vetted to ensure quality and reliability.</p>
            </Col>
          </Row>
        </Container>
      </section>

      {/* Features */}
      <section className="py-5 fold-section">
        <Container>
          <Row className="text-center mb-4">
            <Col>
              <h3 className="fw-bold">Core features</h3>
              <p className="text-muted">Everything you need to plan better trips</p>
            </Col>
          </Row>
          <Row className="g-4">
            <Col md={4}>
              <Card className="h-100 border-0 shadow-sm">
                <Card.Body className="p-4 text-center">
                  <div className="text-danger mb-2"><FaBolt size={22} /></div>
                  <h5 className="fw-bold">Fast discovery</h5>
                  <p className="text-muted mb-0">Quickly view schedules, seating and prices across operators.</p>
                </Card.Body>
              </Card>
            </Col>
            <Col md={4}>
              <Card className="h-100 border-0 shadow-sm">
                <Card.Body className="p-4 text-center">
                  <div className="text-danger mb-2"><FaShieldAlt size={22} /></div>
                  <h5 className="fw-bold">Secure checkout</h5>
                  <p className="text-muted mb-0">Protected payments and instant ticket confirmation.</p>
                </Card.Body>
              </Card>
            </Col>
            <Col md={4}>
              <Card className="h-100 border-0 shadow-sm">
                <Card.Body className="p-4 text-center">
                  <div className="text-danger mb-2"><FaThumbsUp size={22} /></div>
                  <h5 className="fw-bold">Trusted experience</h5>
                  <p className="text-muted mb-0">Vetted operators with ratings and consistent service quality.</p>
                </Card.Body>
              </Card>
            </Col>
          </Row>
        </Container>
      </section>

      {/* Benefits */}
      <section className="py-5 bg-light fold-section">
        <Container>
          <Row className="text-center g-4">
            <Col md={4}>
              <Card className="h-100 border-0 shadow-sm">
                <Card.Body className="text-center">
                  <FaRoute size={40} className="text-danger mb-3" />
                  <h3 className="m-0 text-danger">10,000+</h3>
                  <p className="text-muted">Routes Supported</p>
                </Card.Body>
              </Card>
            </Col>
            <Col md={4}>
              <Card className="h-100 border-0 shadow-sm">
                <Card.Body className="text-center">
                  <FaUsers size={40} className="text-danger mb-3" />
                  <h3 className="m-0 text-danger">50,000+</h3>
                  <p className="text-muted">Happy Users</p>
                </Card.Body>
              </Card>
            </Col>
            <Col md={4}>
              <Card className="h-100 border-0 shadow-sm">
                <Card.Body className="text-center">
                  <FaTicketAlt size={40} className="text-danger mb-3" />
                  <h3 className="m-0 text-danger">150,000+</h3>
                  <p className="text-muted">Bookings Completed</p>
                </Card.Body>
              </Card>
            </Col>
          </Row>
        </Container>
      </section>

      {/* Footer */}
      <footer className="bg-dark text-white text-center py-4 mt-auto">
        <Container>
          <Row>
            <Col md={4}>
              <h5 className="fw-bold text-danger">BusBooking</h5>
              <p className="small text-muted">Travel better with our platform.</p>
            </Col>
            <Col md={4}>
              <h6 className="fw-bold">Quick Links</h6>
              <p className="small text-muted">
                <a href="/home" className="text-white text-decoration-none me-2">Home</a>
                <a href="/help" className="text-white text-decoration-none">Help</a>
              </p>
            </Col>
            <Col md={4}>
              <h6 className="fw-bold">Follow Us</h6>
              <div className="d-flex justify-content-center gap-3 mt-2">
                <a href="https://www.facebook.com/" className="text-white fs-5"><FaFacebook /></a>
                <a href="https://x.com/?lang=en-in" className="text-white fs-5"><FaTwitter /></a>
                <a href="https://www.instagram.com/" className="text-white fs-5"><FaInstagram /></a>
              </div>
            </Col>
          </Row>
          <hr className="border-secondary" />
          <p className="small mb-0">© {new Date().getFullYear()} BusBooking. All rights reserved.</p>
        </Container>
      </footer>
    </div>
  );
}

export default LandingPage;
