// ============================================================================
// Project Name : BusBookingSystem
// File Name    : Bookedpage.jsx
// Created By   : Kaviraj M
// Created On   : 24/10/2025
// Modified By  : Kaviraj M
// Modified On  : 28/10/2025
// Description  : React component that displays detailed booking information
//                for a specific booking ID. Fetches and renders booking
//                details including passenger information and journey details.
// ============================================================================

import React, { useEffect, useState } from "react";
import { Container, Card, Row, Col, Spinner, Alert, Table } from "react-bootstrap";
import { useParams } from "react-router-dom";
import axios from "axios";
import BusNavbar from "./Navbar";

// API base URL for booking operations
const API_BASE = "https://localhost:7272/api/Confirmbooking";

// ============================================================================
// Component Name: BookedPage
// Description  : Displays detailed information about a specific booking including
//                passenger details, journey information, and booking status.
// State        :
//                - booking (object): Holds the booking details
//                - loading (boolean): Indicates if data is being fetched
//                - error (string): Stores any error messages
// ============================================================================
function BookedPage() {
  // Extract booking ID from URL parameters
  const { bookingId } = useParams();
  
  // Component state
  const [booking, setBooking] = useState(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState("");

  // ==========================================================================
  // Effect: Fetch Booking Details
  // Description: Fetches booking details when the component mounts or when bookingId changes
  // Dependencies: bookingId
  // ==========================================================================
  useEffect(() => {
    const fetchBooking = async () => {
      try {
        // API returns List<BookingResponse>
        const res = await axios.get(`${API_BASE}/create?bookingId=${bookingId}`);
        if (!res.data || res.data.length === 0) {
          setError("Booking not found.");
        } else {
          setBooking(res.data[0]);
        }
      } catch (err) {
        console.error(err);
        setError("Failed to fetch booking details.");
      } finally {
        setLoading(false);
      }
    };
    fetchBooking();
  }, [bookingId]);

  // ==========================================================================
  // Loading State
  // Displays a loading spinner while fetching booking details
  // ==========================================================================
  if (loading)
    return (
      <div className="text-center mt-5">
        <Spinner animation="border" /> Loading booking details...
      </div>
    );

  // ==========================================================================
  // Error State
  // Displays an error message if the booking cannot be fetched
  // ==========================================================================
  if (error)
    return (
      <Container className="mt-5">
        <Alert variant="danger">{error}</Alert>
      </Container>
    );

  // ==========================================================================
  // Main Render
  // Displays the booking details in a card layout with passenger information
  // ==========================================================================
  return (
    <>
      <BusNavbar />
      <Container className="mt-5">
        <Card className="p-4 shadow-sm">
          <h3 className="text-center mb-4">Booking Details</h3>
          <Row className="mb-3">
            <Col md={6}><strong>Booking ID:</strong> {booking.bookingId}</Col>
            <Col md={6}><strong>Bus Number:</strong> {booking.busnumber}</Col>
          </Row>
          <Row className="mb-3">
            <Col md={6}><strong>Bus Type:</strong> {booking.busType}</Col>
            <Col md={6}><strong>Source:</strong> {booking.source}</Col>
          </Row>
          <Row className="mb-3">
            <Col md={6}><strong>Destination:</strong> {booking.destination}</Col>
            <Col md={6}><strong>Operator Name:</strong> {booking.operatorName}</Col>
          </Row>
          <Row className="mb-3">
            <Col md={6}><strong>Operator Number:</strong> {booking.operatorNumber}</Col>
          </Row>

          {/* Passenger Details Section */}
          <h5 className="mt-4 mb-3">Passenger Details</h5>
          <Table striped bordered hover>
            <thead>
              <tr>
                <th>#</th>
                <th>Name</th>
                <th>Age</th>
                <th>Gender</th>
              </tr>
            </thead>
            <tbody>
              {booking.passengers?.map((p, index) => (
                <tr key={index}>
                  <td>{index + 1}</td>
                  <td>{p.name}</td>
                  <td>{p.age}</td>
                  <td>{p.gender}</td>
                </tr>
              ))}
            </tbody>
          </Table>
        </Card>
      </Container>
    </>
  );
}

export default BookedPage;
