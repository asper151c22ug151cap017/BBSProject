// ============================================================================
// Project Name : BusBookingSystem
// File Name    : Confirmbooking.jsx
// Created By   : Kaviraj M
// Created On   : 22/09/2025
// Modified By  : Kaviraj M
// Modified On  : 28/10/2025
// Description  : React component that handles the final booking confirmation
//                process, including passenger details collection and payment
//                processing.
// ============================================================================

import React, { useState, useEffect, Fragment } from "react";
import { useLocation, useNavigate, useParams } from "react-router-dom";
import { Container, Card, Row, Col, Button, Spinner, Form, Alert, Badge } from "react-bootstrap";
import axios from "axios";
import BusNavbar from "./Navbar";
import { FaBus, FaMapMarkerAlt, FaTicketAlt, FaUser, FaCheckCircle, FaExclamationTriangle } from "react-icons/fa";

const API_BASE = "https://localhost:7272/api/Confirmbooking";

/**
 * BookingConfirmation Component
 * 
 * Handles the final booking confirmation process, including:
 * - Displaying booking summary
 * - Collecting passenger details
 * - Processing payment
 * - Confirming booking with the backend
 * - Displaying booking confirmation
 * 
 * @component
 * @returns {JSX.Element} The booking confirmation interface
 */
function BookingConfirmation() {
  // ==========================================================================
  // HOOKS & STATE MANAGEMENT
  // ==========================================================================
  const location = useLocation();
  const navigate = useNavigate();
  const params = useParams();

  // Data passed from Seatspage
  const { busId: stateBusId, fare, selectedSeats, seatIds } = location.state || {};
  const busId = stateBusId || params.busId;
  const totalFare = (fare || 0) * (selectedSeats?.length || 0);

  const [busDetails, setBusDetails] = useState(null);
  const [loading, setLoading] = useState(true);
  const [bookingLoading, setBookingLoading] = useState(false);
  const [bookingConfirmed, setBookingConfirmed] = useState(false);
  const [bookingId, setBookingId] = useState(null);

  const [passengers, setPassengers] = useState(
    selectedSeats?.map(() => ({ name: "", age: "", gender: "", errors: {} })) || []
  );

  // ==========================================================================
  // EFFECTS
  // ==========================================================================
  
  /**
   * Fetches additional bus information from the API
   * Used to display bus details in the confirmation page
   */
  useEffect(() => {
    const fetchBus = async () => {
      try {
        const res = await axios.get(`${API_BASE}/Getbusesbyid?busId=${busId}`);
        if (!res.data || res.data.length === 0) {
          setBusDetails(null);
        } else {
          const bus = res.data[0];
          const route = bus.routes?.[0];
          setBusDetails({
            busId: bus.busId,
            busName: bus.busName,
            busNumber: bus.busNumber,
            busType: bus.busType,
            routeId: route?.routeId || null,
            source: route?.source,
            destination: route?.destination,
          });
        }
      } catch (err) {
        console.error(err);
        setBusDetails(null);
      } finally {
        setLoading(false);
      }
    };
    fetchBus();
  }, [busId]);

  // ==========================================================================
  // VALIDATION FUNCTIONS
  // ==========================================================================
  
  /**
   * Validates passenger information
   * @param {Object} p - Passenger object containing name, age, and gender
   * @returns {Object} Object containing validation errors (if any)
   */
  const validatePassenger = (p) => {
    const errors = {};
    if (!p.name?.trim()) errors.name = "Name is required";
    if (!p.age || isNaN(p.age) || p.age <= 0) errors.age = "Enter valid age";
    if (!p.gender) errors.gender = "Select gender";
    return errors;
  };

  // ==========================================================================
  // EVENT HANDLERS
  // ==========================================================================
  
  /**
   * Handles changes to passenger input fields
   * @param {number} index - Index of the passenger in the passengers array
   * @param {string} field - Field name being updated (name, age, gender)
   * @param {string} value - New value for the field
   */
  const handlePassengerChange = (index, field, value) => {
    const updated = [...passengers];
    updated[index][field] = value;
    updated[index].errors = validatePassenger(updated[index]);
    setPassengers(updated);
  };

  /**
   * Handles the booking confirmation process
   * - Validates user session
   * - Validates passenger information
   * - Sends booking request to the server
   * - Handles success/error responses
   */
  const handleConfirmBooking = async () => {
  const loggedInUserId = localStorage.getItem("userid");
  const token = localStorage.getItem("token");

  // 🔒 Validate login and token
  if (!loggedInUserId || !token) {
    alert("⚠️ Please log in again — your session has expired.");
    navigate("/login");
    return;
  }

  // 🧍‍♂️ Validate passenger info
  const invalidPassengers = passengers.map((p) => ({
    ...p,
    errors: validatePassenger(p),
  }));

  if (invalidPassengers.some((p) => Object.keys(p.errors).length > 0)) {
    alert("Please fix passenger details before confirming.");
    return;
  }

  // 💺 Validate seat selection
  if (!selectedSeats || selectedSeats.length !== passengers.length) {
    alert(`Number of passengers must match selected seats (${selectedSeats?.length || 0}).`);
    return;
  }

  if (!busDetails?.busId || !busDetails?.routeId) {
    alert("Bus details are missing. Please refresh and try again.");
    return;
  }

  if (!seatIds || seatIds.length === 0) {
    alert("Seat IDs are missing. Cannot proceed with booking.");
    return;
  }

  setBookingLoading(true);

  try {
    // 🧾 Build payload for backend
    const payload = {
      userId: parseInt(loggedInUserId, 10),
      busId: parseInt(busDetails.busId, 10),
      routeId: parseInt(busDetails.routeId, 10),
      bookingDate: new Date().toISOString(),
      totalFare: parseFloat(totalFare) || 0,
      seatIds: seatIds.map((id) => parseInt(id, 10)), // backend expects int[]
      seatNumbers: selectedSeats,                      // backend expects list of seat numbers
      passangers: passengers.map((p) => ({
        PassengerName: p.name,
        PassengerAge: parseInt(p.age, 10),
        PassengerGender: p.gender,
      })),
    };

    console.log("Booking payload:", payload);

    // 🚀 Send request with Authorization header
    const res = await axios.post(
      "https://localhost:7272/api/Confirmbooking/AddBooking",
      payload,
      {
        headers: {
          "Content-Type": "application/json",
          Authorization: `Bearer ${token}`,
        },
      }
    );

    const data = res.data;

    // ✅ Handle success response
    if (data?.bookingId && data.bookingId > 0) {
      setBookingId(data.bookingId);
      setBookingConfirmed(true);

      alert(
        `${data.message || "Booking confirmed successfully"}\nSeats: ${
          data.SeatNumbers?.join(", ") || selectedSeats.join(", ")
        }`
      );
      return;
    }

    // ⚠️ Handle partial success
    if (data?.SeatNumbers?.length > 0) {
      alert(`Booking partially successful. Seats already booked: ${data.SeatNumbers.join(", ")}`);
      setBookingConfirmed(true);
      return;
    }

    alert(data?.message || "Booking confirmed but no ID returned");
    setBookingConfirmed(true);
  } catch (err) {
    console.error("Booking failed:", err);

    if (err.response?.status === 401) {
      alert("🚫 Unauthorized! Please log in again.");
      localStorage.removeItem("token");
      navigate("/login");
    } else {
      alert(err.response?.data?.message || err.message || "Booking failed. Try again.");
    }
  } finally {
    setBookingLoading(false);
  }
};


  // ==========================================================================
  // RENDER
  // ==========================================================================
  
  // Show loading state while fetching bus details
  if (loading)
    return (
      <div className="text-center mt-5">
        <Spinner animation="border" variant="danger" size="lg" />
        <p className="mt-3">Loading bus details...</p>
      </div>
    );

  // Show error state if bus details are not available
  if (!busDetails)
    return (
      <Container className="mt-5">
        <Alert variant="danger" className="text-center">
          <FaExclamationTriangle className="me-2" />
          Bus details not found. Please try again or contact support.
        </Alert>
        <div className="text-center">
          <Button variant="outline-danger" onClick={() => navigate("/home")}>
            Back to Home
          </Button>
        </div>
      </Container>
    );

  // Main component render
  return (
    <Fragment>
      <BusNavbar />
      <Container className="mt-5">
        {!bookingConfirmed ? (
          <Card className="p-4 shadow-sm border-0">
            <Card.Header className="bg-danger text-white text-center">
              <h3 className="mb-0 d-flex align-items-center justify-content-center">
                <FaTicketAlt className="me-2" />
                Booking Confirmation
              </h3>
            </Card.Header>
            <Card.Body>
              <Row className="mb-4">
                <Col>
                  <Card className="h-100 border-0 shadow-sm">
                    <Card.Body className="text-center">
                      <FaBus size={40} className="text-danger mb-2" />
                      <h5>{busDetails.busName}</h5>
                      <p className="text-muted mb-1">Bus Number: {busDetails.busNumber}</p>
                      <Badge bg="secondary">{busDetails.busType}</Badge>
                    </Card.Body>
                  </Card>
                </Col>
                <Col>
                  <Card className="h-100 border-0 shadow-sm">
                    <Card.Body className="text-center">
                      <FaMapMarkerAlt size={40} className="text-danger mb-2" />
                      <h6>Route</h6>
                      <p className="mb-1"><strong>From:</strong> {busDetails.source}</p>
                      <p className="mb-0"><strong>To:</strong> {busDetails.destination}</p>
                    </Card.Body>
                  </Card>
                </Col>
                <Col>
                  <Card className="h-100 border-0 shadow-sm">
                    <Card.Body className="text-center">
                      <FaUser size={40} className="text-danger mb-2" />
                      <h6>Seats & Fare</h6>
                      <p className="mb-1"><strong>Seats:</strong> {selectedSeats?.join(", ")}</p>
                      <p className="mb-0"><strong>Total Fare:</strong> ₹{totalFare.toFixed(2)}</p>
                    </Card.Body>
                  </Card>
                </Col>
              </Row>

            </Card.Body>
            <Card.Body>
              <h5 className="mb-3">Passenger Details</h5>
              {passengers.map((p, index) => (
                <Row key={index} className="mb-3 align-items-end">
                  <Col md={3}>
                    <Form.Group>
                      <Form.Label>Name</Form.Label>
                      <Form.Control
                        type="text"
                        value={p.name}
                        onChange={(e) => handlePassengerChange(index, "name", e.target.value)}
                        isInvalid={!!p.errors.name}
                      />
                      <Form.Control.Feedback type="invalid">{p.errors.name}</Form.Control.Feedback>
                    </Form.Group>
                  </Col>

                  <Col md={2}>
                    <Form.Group>
                      <Form.Label>Age</Form.Label>
                      <Form.Control
                        type="number"
                        value={p.age}
                        onChange={(e) => handlePassengerChange(index, "age", e.target.value)}
                        isInvalid={!!p.errors.age}
                      />
                      <Form.Control.Feedback type="invalid">{p.errors.age}</Form.Control.Feedback>
                    </Form.Group>
                  </Col>

                  <Col md={3}>
                    <Form.Group>
                      <Form.Label>Gender</Form.Label>
                      <Form.Select
                        value={p.gender}
                        onChange={(e) => handlePassengerChange(index, "gender", e.target.value)}
                        isInvalid={!!p.errors.gender}
                      >
                        <option value="">Select</option>
                        <option value="Male">Male</option>
                        <option value="Female">Female</option>
                        <option value="Other">Other</option>
                      </Form.Select>
                      <Form.Control.Feedback type="invalid">{p.errors.gender}</Form.Control.Feedback>
                    </Form.Group>
                  </Col>
                </Row>
              ))}

              <div className="text-center mt-4">
                <Button
                  variant="success"
                  size="lg"
                  onClick={handleConfirmBooking}
                  disabled={bookingLoading}
                >
                  {bookingLoading ? <Spinner size="sm" animation="border" /> : "Confirm Booking"}
                </Button>
              </div>
            </Card.Body>
          </Card>
        ) : (
          <Card className="p-4 shadow-sm border-0 text-center">
            <Card.Body>
              <FaCheckCircle size={60} className="text-success mb-3" />
              <h3 className="text-success">Booking Confirmed!</h3>
              <p>Your Booking ID is:</p>
              <h4 className="text-danger">{bookingId}</h4>
              <Row className="mt-3">
                <Col md={4}>
                  <strong>Seats:</strong> {selectedSeats?.join(", ")}
                </Col>
                <Col md={4}>
                  <strong>Total Fare:</strong> ₹{totalFare.toFixed(2)}
                </Col>
                <Col md={4}>
                  <strong>Passengers:</strong> {passengers.length}
                </Col>
              </Row>
              <div className="mt-4">
                <Button variant="primary" onClick={() => navigate("/Bookings")}>
                  View Bookings
                </Button>
                <Button variant="outline-secondary" className="ms-2" onClick={() => navigate("/home")}>
                  Book Again
                </Button>
              </div>
            </Card.Body>
          </Card>
        )}
      </Container>
    </Fragment>
  );
}

export default BookingConfirmation;
