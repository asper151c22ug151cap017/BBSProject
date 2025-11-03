// ============================================================================
// Project Name : BusBookingSystem
// File Name    : Seatspage.jsx
// Created By   : Kaviraj M
// Created On   : 23/09/2025
// Modified By  : Kaviraj M
// Modified On  : 28/10/2025
// Description  : React component that handles seat selection for bus booking.
//                Displays interactive seat layout, handles seat selection,
//                and provides booking summary with fare calculation.
// ============================================================================

import React, { useState, useEffect, useMemo } from "react";
import { useParams, useLocation, useNavigate } from "react-router-dom";
import axios from "axios";
import BusNavbar from "./Navbar";
import {
  Alert,
  Button,
  Card,
  Col,
  Container,
  Row,
  Spinner,
  Badge,
  ProgressBar,
} from "react-bootstrap";
import {
  FaBus,
  FaCheckCircle,
  FaTimesCircle,
  FaArrowRight,
  FaChair,
  FaRupeeSign,
  FaMapMarkerAlt,
  FaUser,
  FaInfoCircle,
  FaEye,
  FaBed,
} from "react-icons/fa";

import "./seatspage.css";
import { FaCircleCheck, FaDiagramSuccessor } from "react-icons/fa6";

/**
 * Seatspage Component
 * 
 * Handles the seat selection process for bus booking. Displays an interactive
 * seat layout, allows seat selection, and shows booking summary with fare details.
 * 
 * @component
 * @returns {JSX.Element} The seat selection interface
 */
function Seatspage() {
  // ==========================================================================
  // HOOKS & STATE MANAGEMENT
  // ==========================================================================
  const location = useLocation();
  const navigate = useNavigate();

  // Destructure all necessary properties from location.state with defaults
  const {
    busId: stateBusId,
    busName = '',
    busType = 'Standard',
    fare = 0,
    routeId = '',
    routeName = '',
    source = '',
    destination = '',
    travelDate: navTravelDate = new Date().toISOString().split('T')[0]
  } = location.state || {};

  // ✅ Safely determine final busId
  const { busId: paramBusId } = useParams();
  const busId = stateBusId || paramBusId || 1; // Default to 1 if not provided

  console.log("BusId sources:", { stateBusId, paramBusId, finalBusId: busId });

  // Handle case where busId is missing or invalid
  if (!busId || busId === 'undefined' || busId === 'null' || isNaN(busId)) {
    console.error("Invalid busId detected:", busId);
    return (
      <div>
        <BusNavbar />
        <Container className="mt-4">
          {/* Date Selection Card */}
          <Card className="mb-4 shadow-sm">
            <Card.Body>
              <div className="d-flex justify-content-between align-items-center flex-wrap">
                <div>
                  <h5 className="mb-2">Change Travel Date</h5>
                  <div className="d-flex align-items-center">
                    <Form.Control
                      type="date"
                      value={apiFormattedDate}
                      min={new Date().toISOString().split('T')[0]}
                      onChange={(e) => handleDateChange(e.target.value)}
                      className="me-2"
                      style={{ width: '200px' }}
                      disabled={loading}
                    />
                    {loading && (
                      <div className="position-absolute" style={{ right: '10px', top: '50%', transform: 'translateY(-50%)' }}>
                        <div className="spinner-border spinner-border-sm text-primary" role="status">
                          <span className="visually-hidden">Loading...</span>
                        </div>
                      </div>
                    )}
                    <Button 
                      variant="primary" 
                      onClick={() => handleDateChange(selectedDate)}
                    >
                      <FaSearch className="me-1" /> Update
                    </Button>
                  </div>
                </div>
                <div className="mt-2 mt-md-0">
                  <div className="d-flex align-items-center">
                    <FaCalendarAlt className="text-primary me-2" />
                    <div>
                      <div className="text-muted small">Showing Seats For</div>
                      <div className="fw-bold">{formattedDate}</div>
                    </div>
                  </div>
                </div>
              </div>
            </Card.Body>
          </Card>
          {/* Date and Route Information */}
          <Card className="mb-4 shadow-sm">
            <Card.Body className="p-3">
              <div className="d-flex justify-content-between align-items-center flex-wrap">
                <div>
                  <h4 className="mb-1">{finalBusName}</h4>
                  <p className="mb-1 text-muted">
                    <FaMapMarkerAlt className="text-danger me-1" />
                    <strong>From:</strong> {finalSource} <FaArrowRight className="mx-2" />
                    <strong>To:</strong> {finalDestination}
                  </p>
                </div>
                <div className="mt-2 mt-md-0">
                  <div className="d-flex align-items-center">
                    <FaCalendarAlt className="text-primary me-2" />
                    <div>
                      <div className="text-muted small">Travel Date</div>
                      <div className="fw-bold">{formattedDate}</div>
                    </div>
                  </div>
                </div>
              </div>
            </Card.Body>
          </Card>
          <Alert variant="danger" className="text-center shadow-sm">
            <FaBus className="me-2" size={24} />
            <Alert.Heading>Invalid Bus Selection</Alert.Heading>
            <p>Bus ID is missing or invalid. Please go back and select a bus again.</p>
            <Button variant="danger" size="lg" onClick={() => navigate("/home")}>
              Back to Home
            </Button>
          </Alert>
        </Container>
      </div>
    );
  }

  const [seats, setSeats] = useState([]);
  const [selectedSeats, setSelectedSeats] = useState([]);
  const [selectedSeatIds, setSelectedSeatIds] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState("");
  const [isUsingMockData, setIsUsingMockData] = useState(false);
  // Initialize selectedDate from navigation state or use current date
  // Initialize selectedDate from navigation state
  const [selectedDate, setSelectedDate] = useState(() => {
    try {
      // Parse the date from navTravelDate, or use current date if not available
      const date = navTravelDate ? new Date(navTravelDate) : new Date();
      // Ensure it's a valid date
      return isNaN(date.getTime()) ? new Date() : date;
    } catch (e) {
      console.error('Error parsing date, using current date:', e);
      return new Date();
    }
  });

  // Set defaults if not provided
  const finalSource = source || (busDetails?.source || "Delhi");
  const finalDestination = destination || (busDetails?.destination || "Mumbai");
  const finalBusName = busName || (busDetails?.busName || `Bus ${busId}`);
  const finalBusType = busType || (busDetails?.busType || "Standard");
  const finalFare = fare || (busDetails?.fare || 500);
  
  // Format date for display and API requests
  const formattedDate = useMemo(() => {
    try {
      return selectedDate.toLocaleDateString('en-US', {
        weekday: 'long',
        year: 'numeric',
        month: 'long',
        day: 'numeric'
      });
    } catch (e) {
      console.error('Error formatting date:', e);
      return new Date().toLocaleDateString('en-US', {
        weekday: 'long',
        year: 'numeric',
        month: 'long',
        day: 'numeric'
      });
    }
  }, [selectedDate]);
  
  // Format date for API requests (YYYY-MM-DD)
  const apiFormattedDate = useMemo(() => {
    try {
      return selectedDate.toISOString().split('T')[0];
    } catch (e) {
      console.error('Error formatting date for API:', e);
      return new Date().toISOString().split('T')[0];
    }
  }, [selectedDate]);

  // Handle date change and fetch seats for the new date
  const handleDateChange = async (newDate) => {
    if (!newDate) return;
    
    try {
      setLoading(true);
      setError('');
      
      let date;
      try {
        date = new Date(newDate);
        // Validate the date
        if (isNaN(date.getTime())) {
          throw new Error('Invalid date');
        }
      } catch (e) {
        console.error('Invalid date selected:', newDate, e);
        setError('Please select a valid date');
        return;
      }
      
      // Update local state immediately for better UX
      setSelectedDate(date);
      setSeats([]);
      setSelectedSeats([]);
      setSelectedSeatIds([]);
      
      // Format date for API
      const formattedApiDate = date.toISOString().split('T')[0];
      
      // Update URL and navigation state
      navigate(`/seats/${busId}`, {
        state: {
          ...location.state,
          travelDate: formattedApiDate
        },
        replace: true
      });
      
      // Clear previous data
      setSeats([]);
      setSelectedSeats([]);
      setSelectedSeatIds([]);
      setError('');
      
    } catch (error) {
      console.error('Error changing date:', error);
      setError('Failed to change date. Please try again.');
    } finally {
      setLoading(false); // Always stop spinner
    }
  };

  // Determine bus layout type
  const isSleeper = finalBusType.toLowerCase().includes('sleeper');

  /**
   * Fetches additional bus details if not provided in navigation state
   */
  useEffect(() => {
    const fetchBusDetails = async () => {
      // Only fetch if we're missing critical information
      if (!busName || !source || !destination || !fare) {
        console.log("Missing bus details in state, attempting to fetch from API");
        try {
          const numericBusId = parseInt(busId);

          // Use the suggested URL pattern
          const response = await axios.get(
            `https://localhost:7272/api/Buses/${numericBusId}`,
            { timeout: 5000 }
          );

          if (response.data) {
            console.log("Successfully fetched bus details from API:", response.data);
            setBusDetails(response.data);
          } else {
            throw new Error("No data received");
          }
        } catch (err) {
          console.log("Bus details API not available, all data should come from navigation state");
          // Don't set fallback here since state should have the data
        }
      } else {
        console.log("Complete bus details available from navigation state");
      }
    };

    fetchBusDetails();
  }, [busId, busName, source, destination, fare]);

    // ==========================================================================
  // EFFECTS
  // ==========================================================================
  
  /**
   * Fetches seat data for the selected bus from the API
   * Falls back to mock data if API is unavailable
   */
useEffect(() => {
  const fetchSeats = async () => {
    try {
      setLoading(true); // Show spinner before API call

      // Format the selected date
      const travelDate = selectedDate.toISOString().split("T")[0];
      
      // Get stored date from localStorage if it exists
      const storedDate = localStorage.getItem("journeyDate");
      
      // Store the travel date for future reference if we have a navTravelDate
      // and either there's no stored date or it's different from the current one
      if (navTravelDate && (!storedDate || storedDate !== travelDate)) {
        localStorage.setItem("journeyDate", travelDate);
      }

      // Call backend API with busId + travelDate
      const response = await axios.get(
        "https://localhost:7272/api/Seats/GetParticularBusSeats",
        {
          params: { 
            busId: Number(busId), // Ensure busId is a number
            travelDate: apiFormattedDate
          },
          headers: {
            'Content-Type': 'application/json',
            'Cache-Control': 'no-cache',
            'Pragma': 'no-cache'
          },
          timeout: 10000, // 10 second timeout
          withCredentials: true
        }
      );
      
      console.log('Seats API response:', {
        busId: Number(busId),
        travelDate: apiFormattedDate,
        response: response.data
      });

      // Process the response data
      if (Array.isArray(response.data)) {
        const mappedSeats = response.data.map((seat) => ({
          ...seat,
          isAvailable: seat.isBooked === false, // mark seat available if not booked
          deck: seat.seatNumber
            ? seat.seatNumber.endsWith("U")
              ? "upper"
              : seat.seatNumber.endsWith("R")
              ? "right"
              : seat.seatNumber.endsWith("L")
              ? "lower"
              : "left"
            : "lower",
        }));

        setSeats(mappedSeats);
      } else {
        setSeats([]);
      }
    } catch (error) {
      console.error("❌ Error fetching seats:", error);
      setSeats([]);
    } finally {
      setLoading(false); // Always stop spinner
    }
  };

  // Trigger fetch only when busId and selectedDate are valid
  if (busId && selectedDate) {
    console.log('Fetching seats for:', {
      busId,
      selectedDate: selectedDate.toISOString().split('T')[0]
    });
    fetchSeats();
  }
}, [busId, selectedDate]); // Runs when busId or selectedDate changes


  /**
 * Creates mock seat data for demo purposes when API is unavailable
 * @returns {Array} Array of seat objects with mock data
 */
const createMockSeatsData = () => {
    const totalSeats = isSleeper ? 20 : 40; // 10 lower + 10 upper for sleeper, 20 per side for seater
    const baseFare = finalFare;
    const numericBusId = parseInt(busId) || 1;
    const seats = [];

    for (let i = 1; i <= totalSeats; i++) {
      const row = Math.ceil(i / 2); // 2 seats per row
      const seatInRow = ((i - 1) % 2) + 1; // 1 or 2
      const isLowerDeck = i <= totalSeats / 2;
      const isAvailable = Math.random() > 0.25; // 75% availability

      // Seat type based on position (1+1 layout: window-aisle)
      let seatType = 'regular';
      if (seatInRow === 1) {
        seatType = 'window';
      } else if (seatInRow === 2) {
        seatType = 'aisle';
      }

      // Dynamic pricing
      let priceMultiplier = 1;
      if (seatType === 'window') priceMultiplier = 1.1; // Window premium
      if (row <= 2) priceMultiplier *= 1.2; // Front row premium
      if (!isLowerDeck) priceMultiplier *= 0.95; // Upper deck discount

      // Generate seat number based on bus type
      let seatNumber;
      if (isSleeper) {
        seatNumber = `${row}${isLowerDeck ? 'L' : 'U'}`;
      } else {
        // For seater: left side first 20 seats, right side next 20
        const isLeftSide = i <= totalSeats / 2;
        const sideRow = isLeftSide ? i : i - totalSeats / 2;
        seatNumber = `${sideRow}${isLeftSide ? 'L' : 'R'}`;
      }

      seats.push({
        seatId: i,
        seatNumber: seatNumber,
        isAvailable: isAvailable,
        seatType: seatType,
        price: Math.round(baseFare * priceMultiplier),
        row: row,
        column: seatInRow,
        deck: isSleeper ? (isLowerDeck ? 'lower' : 'upper') : (seatNumber.endsWith('L') ? 'left' : 'right'),
        userId: 0,
        routeId: 0,
        busId: numericBusId
      });
    }

    return seats;
  };

  /**
   * Groups seats into rows (2 seats per row) for display
   * @param {Array} deckSeats - Array of seat objects for a specific deck
   * @returns {Array} 2D array of seats grouped by rows
   */
  const getSeatLayoutForDeck = (deckSeats) => {
    const rows = [];
    const seatsPerRow = 2;

    const sortedSeats = deckSeats.sort((a, b) => {
      const numA = parseInt(a.seatNumber.slice(0, -1));
      const numB = parseInt(b.seatNumber.slice(0, -1));
      return numA - numB;
    });

    for (let i = 0; i < sortedSeats.length; i += seatsPerRow) {
      rows.push(sortedSeats.slice(i, i + seatsPerRow));
    }
    return rows;
  };

  /**
   * Renders an individual seat component with appropriate styling
   * @param {Object} seat - Seat object containing seat details
   * @returns {JSX.Element} Rendered seat component
   */
  const renderSeat = (seat) => {
    if (!seat || !seat.seatNumber) {
      return null;
    }

    const isBooked = !seat.isAvailable;
    const isSelected = selectedSeats.includes(seat.seatNumber);
    const isUpperDeck = seat.deck === 'upper' || seat.deck === 'right';
    const seatNum = parseInt(seat.seatNumber.slice(0, -1)) || 1;

    // Seat type detection (1+1 layout: window-aisle)
    let seatType = 'regular';
    if (seatNum % 2 === 1) {
      seatType = 'window';
    } else {
      seatType = 'aisle';
    }

    let seatClass = `seat ${isBooked ? 'booked' : ''} ${isSelected ? 'selected' : ''}`;
    seatClass += (seatType === 'window') ? ' window-seat' : ' aisle-seat';
    seatClass += isUpperDeck ? ' upper-deck' : ' lower-deck';

    return (
      <div
        key={seat.seatId}
        className={seatClass}
        onClick={() => toggleSeat(seat)}
        title={
          isBooked
            ? `Seat ${seat.seatNumber} - Not Available`
            : isSelected
            ? `Seat ${seat.seatNumber} - Selected - Click to deselect`
            : `Seat ${seat.seatNumber} - Available - Click to select`
        }
      >
        <div className="seat-inner">
          {isBooked ? (
            <FaTimesCircle className="seat-icon unavailable" />
          ) : isSelected ? (
            <>
              <FaCheckCircle className="seat-icon selected" />
              <div className="seat-price">₹{getSeatFare(seat)}</div>
            </>
          ) : (
            <>
              <span className="seat-number">{seatNum}</span>
              <div className="seat-price">₹{getSeatFare(seat)}</div>
            </>
          )}
        </div>
        <div className="seat-label">
        </div>
      </div>
    );
  };

  // ==========================================================================
  // EVENT HANDLERS
  // ==========================================================================
  
  /**
   * Handles the booking confirmation process
   * Validates seat selection and navigates to the payment page
   */
  const handleConfirmBooking = () => {
  if (selectedSeats.length === 0) {
    alert("Please select at least one seat!");
    return;
  }

  if (selectedSeats.length > 4) {
    alert("You can book a maximum of 4 seats only.");
    return;
  }

  navigate(`/confirmbooking/${busId}`, {
    state: {
      fare: finalFare,
      selectedSeats,
      seatIds: selectedSeatIds,
    },
  });
};

/**
 * Toggles seat selection state
 * @param {Object} seat - The seat object to toggle
 */
const toggleSeat = (seat) => {
  if (!seat.isAvailable) return;

  // If the seat is already selected, deselect it
  if (selectedSeats.includes(seat.seatNumber)) {
    setSelectedSeats(selectedSeats.filter((s) => s !== seat.seatNumber));
    setSelectedSeatIds(selectedSeatIds.filter((id) => id !== seat.seatId));
  } else {
    // If user already selected 4 seats, prevent adding more
    if (selectedSeats.length >= 4) {
      alert("You can book a maximum of 4 seats per bus.");
      return;
    }

    // Otherwise, add the new seat
    setSelectedSeats([...selectedSeats, seat.seatNumber]);
    setSelectedSeatIds([...selectedSeatIds, seat.seatId]);
  }
};

  /**
   * Calculates the fare for a specific seat based on its properties
   * @param {Object} seat - The seat object
   * @returns {number} The calculated fare for the seat
   */
  const getSeatFare = (seat) => {
    try {
      // If seat has price from API, use it
      if (seat && seat.price && typeof seat.price === 'number' && seat.price > 0) {
        return seat.price;
      }

      // Otherwise calculate based on seat properties
      if (!seat || !seat.seatNumber) {
        return finalFare || 500;
      }

      const baseFare = Math.max(finalFare || 500, 100); // Ensure minimum fare
      const seatNum = parseInt(seat.seatNumber.slice(0, -1)) || 1;
      const isUpperDeck = seat.deck === 'upper' || seat.deck === 'right';

      let priceMultiplier = 1;

      // Window seats (odd numbers) are slightly more expensive
      if (seatNum % 2 === 1) {
        priceMultiplier = 1.1;
      }

      // Front seats more expensive
      if (seatNum <= 4) {
        priceMultiplier *= 1.2;
      }

      // Upper deck slightly cheaper
      if (isUpperDeck) {
        priceMultiplier *= 0.95;
      }

      return Math.round(baseFare * priceMultiplier);
    } catch (err) {
      console.error("Error calculating seat fare:", err, seat);
      return finalFare || 500;
    }
  };

  // ==========================================================================
  // DERIVED STATE & MEMOIZED VALUES
  // ==========================================================================
  
  // Seat statistics
  const availableSeats = seats.filter(s => s.isAvailable).length;
  const totalSeats = seats.length;

  const totalFare = useMemo(() => {
    try {
      return selectedSeats.reduce((total, seatNum) => {
        const seat = seats.find(s => s && s.seatNumber === seatNum);
        if (seat && seat.seatNumber) {
          return total + getSeatFare(seat);
        }
        return total;
      }, 0);
    } catch (err) {
      console.error("Error calculating total fare:", err);
      return 0;
    }
  }, [selectedSeats, seats, finalFare]);

  // Filter seats by deck (computed after seats are loaded)
  const lowerDeckSeats = seats.length > 0 ? seats.filter(seat => seat && seat.deck === 'lower' || seat.deck === 'left').sort((a, b) => {
    const numA = parseInt(a.seatNumber.slice(0, -1)) || 0;
    const numB = parseInt(b.seatNumber.slice(0, -1)) || 0;
    return numA - numB;
  }) : [];
  const upperDeckSeats = seats.length > 0 ? seats.filter(seat => seat && seat.deck === 'upper' || seat.deck === 'right').sort((a, b) => {
    const numA = parseInt(a.seatNumber.slice(0, -1)) || 0;
    const numB = parseInt(b.seatNumber.slice(0, -1)) || 0;
    return numA - numB;
  }) : [];

  // ==========================================================================
  // RENDER
  // ==========================================================================
  
  return (
    <>
      <BusNavbar />
      <Container fluid className="bg-gradient-light py-4 min-vh-100">
        <Container>
        <div className="d-flex justify-content-center align-items-center mb-4">
  <label htmlFor="travelDate" className="me-3 fw-bold">Select Travel Date:</label>
  <input
    type="date"
    id="travelDate"
    className="form-control w-auto"
    value={selectedDate.toISOString().split('T')[0]}
    min={new Date().toISOString().split('T')[0]} // ⛔ disables previous dates
    onChange={(e) => setSelectedDate(new Date(e.target.value))}
  />
</div>


          {/* Professional Header */}
          <div className="text-center mb-5 position-relative">
            <div className="header-badge">
              <FaBus className="me-2" />
              <span>Select Your Perfect Seat</span>
            </div>
            <div className="bus-info-card mt-4">
              <h2 className="bus-title mb-3">
                {finalBusName}
              </h2>
              <div className="d-flex justify-content-center align-items-center gap-4 mb-3">
                <Badge className="bus-type-badge">{finalBusType}</Badge>
                <div className="route-info">
                  <span className="route-text text-dark">
                    <FaMapMarkerAlt className="me-2" />
                    {finalSource}
                    <FaArrowRight className="mx-3" />
                    {finalDestination}
                  </span>
                </div>
              </div>
              <div className="d-flex justify-content-center gap-4 mb-2">
                <div className="info-item">
                  <FaRupeeSign className="info-icon" />
                  <span>Starting from ₹{finalFare}</span>
                </div>
                <div className="info-item">
                  <FaUser className="info-icon" />
                  <span>{selectedSeats.length} selected</span>
                </div>
                <div className="info-item">
                  <FaEye className="info-icon" />
                  <span>{availableSeats} available</span>
                </div>
              </div>
            </div>
          </div>

          {error && (
            <Alert variant="danger" className="error-card shadow-sm">
              <FaTimesCircle className="me-2" />
              <Alert.Heading>Error Loading Seats</Alert.Heading>
              <p className="mb-3">{error}</p>
              <Button variant="outline-danger" onClick={() => window.location.reload()}>
                <FaArrowRight className="me-2" />
                Retry
              </Button>
            </Alert>
          )}

          {loading ? (
            <div className="loading-container">
              <div className="loading-spinner">
                <Spinner animation="border" variant="danger" size="lg" />
              </div>
              <p className="loading-text">Loading seat layout...</p>
            </div>
          ) : seats.length === 0 ? (
            <Alert variant="warning" className="no-seats-card">
              <FaInfoCircle className="me-2" />
              <Alert.Heading>No Seats Available</Alert.Heading>
              <p>No seats found for this bus. Please try selecting a different bus or date.</p>
              <Button variant="outline-warning" onClick={() => navigate("/home")}>
                <FaArrowRight className="me-2" />
                Back to Search
              </Button>
            </Alert>
          ) : (
            <Row className="g-4">
              {/* Professional Seat Layout */}
              <Col lg={8}>
                <Card className="seat-layout-card">
                  <Card.Header className="seat-layout-header">
                    <h5 className="mb-0 d-flex align-items-center justify-content-center">
                      <FaChair className="me-2" />
                      Bus Seat Layout
                    </h5>
                  </Card.Header>
                  <Card.Body className="p-4">
                    {/* Clean Legend - Available/Selected/Unavailable/Lower/Upper */}
                    <div className="legend-section">
                      <div className="legend-row">
                        <div className="legend-item">
                          <div className="legend-color available"></div>
                          <small>Available</small>
                        </div>
                        <div className="legend-item">
                          <div className="legend-color selected"></div>
                          <small>Selected</small>
                        </div>
                        <div className="legend-item">
                          <div className="legend-color unavailable"></div>
                          <small>Not Available</small>
                        </div>
                      </div>
                      <div className="legend-row">
                        <div className="legend-item">
                          <div className="legend-color lower-deck"></div>
                          <small>{isSleeper ? 'Lower Deck' : 'Left Side'}</small>
                        </div>
                        <div className="legend-item">
                          <div className="legend-color upper-deck"></div>
                          <small>{isSleeper ? 'Upper Deck' : 'Right Side'}</small>
                        </div>
                      </div>
                    </div>

                    {/* Perfect Bus Layout - Side by Side Decks */}
                    <div className="bus-layout-container">
                      {/* Driver Area */}
                      <div className="driver-area">
                        <div className="driver-seat">
                          <FaBus size={20} />
                          <small className="driver-label">Driver</small>
                        </div>
                      </div>

                      {/* Decks Side by Side */}
                      <div className="decks-side-by-side">
                        {/* Deck A (Lower) */}
                        <div className={`deck-section ${isSleeper ? 'lower-deck' : 'left'}`}>
                          <div className="deck-header lower-deck-header">
                            <h6 className="deck-title">
                             
                              {isSleeper ? 'Lower Deck' : 'Left Side'}
                            </h6>
                          </div>
                          <div className="seats-grid">
                            {getSeatLayoutForDeck(lowerDeckSeats).map((row, rowIndex) => (
                              <div key={`lower-${rowIndex}`} className="seat-row">
                                {row.map((seat) => (
                                  <div key={seat.seatId} className="seat-wrapper">
                                    {renderSeat(seat)}
                                  </div>
                                ))}
                              </div>
                            ))}
                          </div>
                        </div>

                        {/* Deck B (Upper) */}
                        <div className={`deck-section ${isSleeper ? 'upper-deck' : 'right'}`}>
                          <div className="deck-header upper-deck-header">
                            <h6 className="deck-title">
                             
                              {isSleeper ? 'Upper Deck' : 'Right Side'}
                            </h6>
                          </div>
                          <div className="seats-grid">
                            {getSeatLayoutForDeck(upperDeckSeats).map((row, rowIndex) => (
                              <div key={`upper-${rowIndex}`} className="seat-row">
                                {row.map((seat) => (
                                  <div key={seat.seatId} className="seat-wrapper">
                                    {renderSeat(seat)}
                                  </div>
                                ))}
                              </div>
                            ))}
                          </div>
                        </div>
                      </div>

                      {/* Emergency Exit */}
                      <div className="emergency-exit">
                        <div className="emergency-badge">
                          <FaInfoCircle />
                          <small>Emergency Exit</small>
                        </div>
                      </div>
                    </div>
                  </Card.Body>
                </Card>
              </Col>

              {/* Professional Booking Summary */}
              <Col lg={4}>
                <Card className="booking-summary-card">
                  <Card.Header className="booking-header">
                    <h5 className="mb-0">Booking Summary</h5>
                  </Card.Header>
                  <Card.Body className="booking-body">
                    {/* Trip Details */}
                    <div className="trip-details-section">
                      <h6 className="section-title">Trip Details</h6>
                      <div className="detail-row">
                        <span className="detail-label">From:</span>
                        <strong className="detail-value">{finalSource}</strong>
                      </div>
                      <div className="detail-row">
                        <span className="detail-label">To:</span>
                        <strong className="detail-value">{finalDestination}</strong>
                      </div>
                      <div className="detail-row">
                        <span className="detail-label">Bus:</span>
                        <strong className="detail-value">{finalBusName}</strong>
                      </div>
                      {finalBusType && (
                        <div className="detail-row">
                          <span className="detail-label">Type:</span>
                          <Badge className="bus-type-badge">{finalBusType}</Badge>
                        </div>
                      )}
                    </div>

                    <hr className="section-divider" />

                    {/* Selected Seats */}
                    <div className="selected-seats-section">
                      <h6 className="section-title">Selected Seats ({selectedSeats.length})</h6>
                      {selectedSeats.length > 0 ? (
                        <div>
                          <div className="selected-seats-list">
                            {selectedSeats.map((seatNum, index) => {
                              const seat = seats.find(s => s && s.seatNumber === seatNum);
                              const seatFare = seat ? getSeatFare(seat) : (finalFare || 500);
                              const isUpperDeck = seat && seat.deck ? (seat.deck === 'upper' || seat.deck === 'right') : false;

                              return (
                                <div key={index} className="selected-seat-item">
                                  <div className="d-flex justify-content-between align-items-center">
                                    <div className="d-flex align-items-center">
                                      <Badge className={`seat-badge ${isUpperDeck ? 'upper' : 'lower'}`}>
                                        {seat && seat.seatNumber ? parseInt(seat.seatNumber.slice(0, -1)) : parseInt(seatNum.slice(0, -1))}
                                      </Badge>
                                      <small className="deck-label">
                                        {isSleeper ? 'Deck' : 'Side'}: {isUpperDeck ? (isSleeper ? 'Upper' : 'Right') : (isSleeper ? 'Lower' : 'Left')}
                                      </small>
                                    </div>
                                    <span className="fare-price">₹{seatFare}</span>
                                  </div>
                                </div>
                              );
                            })}
                          </div>
                          <div className="fare-breakdown">
                            <div className="fare-row">
                              <span>Subtotal:</span>
                              <span>₹{totalFare}</span>
                            </div>
                            <div className="fare-row">
                              <span>Convenience Fee:</span>
                              <span>₹20</span>
                            </div>
                          </div>
                          <hr className="fare-divider" />
                          <div className="total-amount">
                            <div className="total-row">
                              <span className="total-label">Total Amount:</span>
                              <span className="total-value">₹{totalFare + 20}</span>
                            </div>
                          </div>
                        </div>
                      ) : (
                        <div className="no-selection">
                          <p className="no-selection-text">No seats selected</p>
                          <small className="selection-hint">Click on available seats to select</small>
                        </div>
                      )}
                    </div>

                    {/* Selection Progress */}
                    {selectedSeats.length > 0 && (
                      <div className="progress-section">
                        <div className="progress-header">
                          <small className="progress-label">Selection Progress</small>
                          <small className="progress-count">{selectedSeats.length} seat(s)</small>
                        </div>
                        <ProgressBar
                          now={(selectedSeats.length / totalSeats) * 100}
                          className="selection-progress"
                        />
                      </div>
                    )}

                    {/* Action Buttons */}
                    <div className="action-buttons">
                      <Button
                        variant="success"
                        size="lg"
                        onClick={handleConfirmBooking}
                        disabled={selectedSeats.length === 0}
                        className="continue-button"
                      >
                        <FaCircleCheck className="me-2" />
                        Click to Confirm Booking
                      </Button>
                      <Button
                        variant="outline-secondary"
                        onClick={() => navigate("/home")}
                        className="back-button"
                      >
                        <FaArrowRight className="me-2" />
                        Back to Search
                      </Button>
                    </div>

                    {/* Terms and Conditions */}
                    <div className="terms-section">
                      <small className="terms-text">
                        By proceeding, you agree to our terms and conditions
                      </small>
                    </div>
                  </Card.Body>
                </Card>
              </Col>
            </Row>
          )}
        </Container>
      </Container>
    </>
  );
}

export default Seatspage;
