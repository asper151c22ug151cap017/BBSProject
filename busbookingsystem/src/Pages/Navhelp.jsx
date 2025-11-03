// ============================================================================
// Project Name : BusBookingSystem
// File Name    : Navhelp.jsx
// Created By   : Kaviraj M
// Created On   : 24/10/2025
// Modified By  : Kaviraj M
// Modified On  : 28/10/2025
// Description  : React component that provides a help and support interface for
//                users to find assistance with bus bookings, ticket management,
//                and frequently asked questions.
// ============================================================================

import React from "react";
import { Container, Row, Col, Card, Button, Alert } from "react-bootstrap";
import { FaBus, FaTicketAlt, FaQuestionCircle, FaInfoCircle, FaPhoneAlt, FaEnvelope, FaClock } from "react-icons/fa";
import BusNavbar from "./Navbar";
import "./Help.css"

// ============================================================================
// Component Name: HelpPage
// Description  : Main component for the Help & Support page. Displays various
//                help topics in a responsive card layout with relevant icons
//                and navigation options.
// State        : None (Stateless functional component)
// ============================================================================
function HelpPage() {
  // ==========================================================================
  // Constant: helpTopics
  // Description: Array of help topic objects containing information for each
  //              help section including icons, titles, descriptions, and actions
  // ==========================================================================
  const helpTopics = [
    {
      icon: <FaBus size={30} className="text-danger" />,
      title: "Booking a Bus",
      description: "Learn how to search and book buses quickly. Select source, destination, date, and seats.",
      link: "/home",
      linkText: "Start Booking",
    },
    {
      icon: <FaTicketAlt size={30} className="text-primary" />,
      title: "Ticket Management",
      description: "View, download, and cancel your bookings easily. Keep track of your tickets.",
      link: "/Bookings",
      linkText: "View Bookings",
    },
    {
      icon: <FaQuestionCircle size={30} className="text-warning" />,
      title: "FAQs",
      description: "Find answers to common questions about bookings, payments, and bus services.",
    },
    {
      icon: <FaInfoCircle size={30} className="text-info" />,
      title: "System Info",
      description: "Understand how the system works, including backend services and user interface features.",
    },
    {
      icon: <FaPhoneAlt size={30} className="text-success" />,
      title: "Contact Support",
      description: "Need help? Contact our support team via phone or email for immediate assistance.",
      contact: "📞 +91 98765 43210 | ✉️ support@busbooking.com",
    },
  ];

  // ==========================================================================
  // Render Method
  // Returns the JSX for the Help & Support page layout with all help topics
  // ==========================================================================
  return (
    <>
      <BusNavbar />
      <Container className="mt-5 py-4">
        {/* Page Header Section */}
        <div className="text-center mb-5">
          <h2 className="text-danger">🚌 Help & Support</h2>
          <p className="text-muted">We're here to assist you with all your bus booking needs.</p>
        </div>

        {/* Help Topics Grid */}
        <Row className="g-4">
          {helpTopics.map((topic, index) => (
            <Col md={6} lg={4} key={index}>
              <Card className="h-100 shadow-lg border-0 help-card animate-card">
                <Card.Body className="text-center">
                  <div className="mb-3">{topic.icon}</div>
                  <Card.Title className="mb-3">{topic.title}</Card.Title>
                  <Card.Text className="mb-3">{topic.description}</Card.Text>
                  {topic.link && (
                    <Button variant="outline-primary" href={topic.link}>
                      {topic.linkText}
                    </Button>
                  )}
                  {topic.contact && (
                    <Alert variant="light" className="mt-3">
                      {topic.contact}
                    </Alert>
                  )}
                </Card.Body>
              </Card>
            </Col>
          ))}
        </Row>

        {/* Support Hours Information */}
        <div className="text-center mt-5">
          <Alert variant="info">
            <FaClock className="me-2" />
            Support Hours: 24/7 | Response Time: Within 1 hour
          </Alert>
        </div>
      </Container>
    </>
  );
}

export default HelpPage;
