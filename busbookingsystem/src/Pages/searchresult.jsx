// ============================================================================
// Project Name : BusBookingSystem
// File Name    : searchresult.jsx
// Created By   : Kaviraj M
// Created On   : 22/09/2025
// Modified By  : Kaviraj M
// Modified On  : 28/10/2025
// Description  : React component that displays search results for available buses
//                based on user's search criteria. Includes filtering, sorting,
//                and booking functionality with a responsive UI.
// ============================================================================

import React, { useState, useEffect } from "react";
import { Container, Row, Col, Card, Button, Spinner, Alert, Badge, Form, InputGroup } from "react-bootstrap";
import { useLocation, useNavigate, useParams } from "react-router-dom";
import axios from "axios";
import { FaBus, FaClock, FaMapMarkerAlt, FaStar, FaCalendarAlt, FaArrowRight, FaRoute, FaRupeeSign, FaChair, FaFilter, FaSort, FaHeart, FaSearch } from "react-icons/fa";
import BusNavbar from "./Navbar";
import "./searchresult.css";

/**
 * SearchResult Component
 * 
 * Displays a list of available buses based on search criteria with filtering and sorting options.
 * Fetches real-time bus data from the API with fallback to mock data when needed.
 * 
 * @component
 * @returns {JSX.Element} The rendered search results page
 */
function SearchResult() {
  // ==========================================================================
  // HOOKS & STATE MANAGEMENT
  // ==========================================================================
  const location = useLocation();
  const navigate = useNavigate();
  const { busId: stateBusId, ...searchParams } = location.state || {};
  const { busId: paramBusId } = useParams();
  const busId = stateBusId || paramBusId;

  // Format the date from search params or use today's date
  const formatDate = (dateString) => {
    if (!dateString) return new Date().toISOString().split('T')[0];
    try {
      const date = new Date(dateString);
      return date.toISOString().split('T')[0];
    } catch (e) {
      return new Date().toISOString().split('T')[0];
    }
  };

  // Define default initialData safely
  const initialData = searchParams || {
    fromCity: "",
    toCity: "",
    travelDate: formatDate()
  };

  const [formData, setFormData] = useState({
    fromCity: initialData.fromCity || "",
    toCity: initialData.toCity || "",
    travelDate: formatDate(initialData.travelDate || initialData.journeyDate)
  });

  const [selectedDate, setSelectedDate] = useState(() => {
    // Initialize selectedDate from formData.travelDate if it exists
    return formData.travelDate || formatDate();
  });
  const [buses, setBuses] = useState([]);
  const [filteredBuses, setFilteredBuses] = useState([]);
  const [sortBy, setSortBy] = useState("departure");
  const [filterType, setFilterType] = useState("all");
  const [priceRange, setPriceRange] = useState([0, 5000]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState("");
  const [isUsingMockData, setIsUsingMockData] = useState(false);
  const [seatAvailability, setSeatAvailability] = useState({});

  /**
   * Capitalizes the first letter of a string
   * @param {string} str - The string to capitalize
   * @returns {string} The capitalized string
   */
  const capitalize = (str = "") => str.charAt(0).toUpperCase() + str.slice(1).toLowerCase();

  /**
   * Determines the color of the rating badge based on the rating value
   * @param {number} rating - The bus rating (0-5)
   * @returns {string} The color code for the rating badge
   */
  const getRatingColor = (rating) => {
    /**
     * Returns a color code based on the rating value:
     * - Green (#28a745) for ratings 4.5 and above
     * - Orange (#ffc107) for ratings between 4.0 and 4.4
     * - Yellow (#fd7e14) for ratings between 3.5 and 3.9
     * - Red (#dc3545) for ratings below 3.5
     */
    if (rating >= 4.5) return "#28a745";
    if (rating >= 4.0) return "#ffc107";
    if (rating >= 3.5) return "#fd7e14";
    return "#dc3545";
  };

  /**
   * Default date for travel if none provided
   */
  const defaultDate = new Date().toISOString().split("T")[0];
  const travelDate = formData?.travelDate || defaultDate;

  /**
   * Fetches bus data from the API or uses mock data if API fails
   * @async
   */
  // Fetch seat availability for a specific bus and date
  const fetchSeatAvailability = async (busId, travelDate) => {
    try {
      const response = await axios.get(
        'https://localhost:7272/api/Seats/GetParticularBusSeats',
        {
          params: { busId, travelDate }
        }
      );
      
      if (response.data && Array.isArray(response.data)) {
        const availableSeats = response.data.filter(seat => seat.isBooked === false).length;
        const totalSeats = response.data.length;
        
        setSeatAvailability(prev => ({
          ...prev,
          [busId]: {
            available: availableSeats,
            total: totalSeats,
            booked: totalSeats - availableSeats
          }
        }));
      }
    } catch (error) {
      console.error(`Error fetching seat availability for bus ${busId}:`, error);
    }
  };

  // Fetch buses and their seat availability
  useEffect(() => {
    const fetchBuses = async () => {
      if (!formData) {
        setError("No search data found. Please search first.");
        setLoading(false);
        return;
      }

      setLoading(true);
      setError("");

      try {
        console.log("Fetching buses from API...");

        // Get and validate the travel date
        const travelDate = (() => {
          try {
            // If no date provided, use today
            if (!formData.travelDate) return new Date().toISOString().split('T')[0];
            
            // If it's already in YYYY-MM-DD format, use it as is
            if (/^\d{4}-\d{2}-\d{2}$/.test(formData.travelDate)) {
              return formData.travelDate;
            }
            
            // Try to parse as a date
            const date = new Date(formData.travelDate);
            if (!isNaN(date.getTime())) {
              return date.toISOString().split('T')[0];
            }
            
            throw new Error('Invalid date format');
          } catch (error) {
            console.warn('Error parsing travel date, using today\'s date:', error);
            return new Date().toISOString().split('T')[0];
          }
        })();
        
        console.log('Making API request with:', {
          source: formData.fromCity,
          destination: formData.toCity,
          travelDate
        });
        
        const response = await axios.get(
          "https://localhost:7272/api/Routes/FilterRoutes",
          {
            params: {
              source: formData.fromCity?.trim() || '',
              destination: formData.toCity?.trim() || '',
              travelDate: travelDate,
            },
            headers: {
              'Content-Type': 'application/json',
            },
            paramsSerializer: params => {
              return Object.entries(params)
                .map(([key, value]) => `${encodeURIComponent(key)}=${encodeURIComponent(value)}`)
                .join('&');
            }
          }
        );

        console.log("API Response:", response.data);

        if (response.data && response.data.length > 0) {
          const formattedData = response.data.map((bus) => ({
            busId: bus.busid ?? bus.busId,
            busName: bus.busName ?? bus.busname,
            busType: bus.busType ?? bus.bustype,
            departureTime: bus.departuretime ?? bus.departureTime,
            arrivalTime: bus.arrivaltime ?? bus.arrivalTime,
            fare: bus.fare ?? 0,
            ratingValue: bus.ratingValue ?? bus.rating ?? 4.0,
            source: bus.source,
            destination: bus.destination,
            routeId: bus.routeId ?? 1,
            journeyDate: formData.travelDate, // ✅ ensure date is carried forward
            totalSeats: bus.totalSeats ?? 0,
            bookedSeats: bus.bookedSeats ?? 0,
            availableSeats: bus.availableSeats ?? 0,
            bookedSeatNumbers: bus.bookedSeatNumbers ?? [],
            busOperator: bus.busOperator || bus.operator || "Premium Bus Service",
          }));

          setBuses(formattedData);
          setFilteredBuses(formattedData);
          setIsUsingMockData(false);
          setError("");
          
          // Fetch seat availability for each bus
          formattedData.forEach(bus => {
            if (bus.busId && bus.journeyDate) {
              fetchSeatAvailability(bus.busId, bus.journeyDate);
            }
          });
        } else {
          console.log("No buses found in API");
          // Set empty array instead of using mock data
          setBuses([]);
          setFilteredBuses([]);
          setIsUsingMockData(false);
          setError("No buses found for the selected route and date.");
        }
      } catch (err) {
        console.error("API Error:", err.message);
        // Fallback to empty array instead of undefined mockBuses
        const mockData = [];
        setBuses(mockData);
        setFilteredBuses(mockData);
        setIsUsingMockData(true);
        setError("Unable to connect to server. Please try again later.");
      } finally {
        setLoading(false);
      }
    };

    fetchBuses();
  }, [formData]);

  // ==========================================================================
  // FILTERING & SORTING EFFECT
  // ==========================================================================
  useEffect(() => {
    if (buses.length === 0) return; // Don't run if no buses loaded yet

    let filtered = [...buses];

    // Filter by bus type
    if (filterType !== "all") {
      filtered = filtered.filter(bus => bus.busType.toLowerCase().includes(filterType.toLowerCase()));
    }

    // Filter by price range (₹0 - ₹5000)
    filtered = filtered.filter(bus => bus.fare >= priceRange[0] && bus.fare <= priceRange[1]);
    
    // Filter by selected date
    filtered = filtered.filter(bus => bus.journeyDate === selectedDate);

    // Sort buses
    filtered.sort((a, b) => {
      switch (sortBy) {
        case "departure":
          return a.departureTime.localeCompare(b.departureTime);
        case "arrival":
          return a.arrivalTime.localeCompare(b.arrivalTime);
        case "price":
          return a.fare - b.fare;
        case "rating":
          return b.ratingValue - a.ratingValue;
        default:
          return 0;
      }
    });

    setFilteredBuses(filtered);
  }, [buses, sortBy, filterType, priceRange, selectedDate]);

  const createMockBuses = () => {
    const source = formData ? capitalize(formData.fromCity) : "Chennai";
    const destination = formData ? capitalize(formData.toCity) : "Bangalore";

    return mockBuses.filter(bus => bus.source === source && bus.destination === destination);
  };

  if (!formData) {
    return (
      <div>
        <BusNavbar />
        <Container className="mt-5 py-5">
          <Alert variant="warning" className="text-center shadow-sm">
            <FaBus className="me-2" size={24} />
            No search data found. Please go back and search again.
          </Alert>
          <div className="text-center">
            <Button variant="danger" size="lg" onClick={() => navigate("/home")}>
              Back to Home
            </Button>
          </div>
        </Container>
      </div>
    );
  }

  // ==========================================================================
  // EVENT HANDLERS
  // ==========================================================================
  
  /**
   * Handles the "Book Now" button click
   * Navigates to the seat selection page with bus details
   * @param {Object} bus - The selected bus object
   */
  const handleBookNow = (bus) => {
  // ✅ Save selected journey date globally before navigating
  localStorage.setItem("journeyDate", formData.travelDate);

  navigate(`/Seatspage/${bus.busId}`, {
    state: {
      busId: bus.busId,
      busName: bus.busName,
      busType: bus.busType,
      fare: bus.fare,
      source: bus.source,
      destination: bus.destination,
      routeId: bus.routeId,
      routeName: `${bus.source} to ${bus.destination}`,
      travelDate: formData.travelDate, // Pass the selected travel date
    },
  });
};

  const handleSeatSelection = (bus) => {
    // Ensure the date is in the correct format (YYYY-MM-DD)
    const formattedDate = formData.travelDate 
      ? new Date(formData.travelDate).toISOString().split('T')[0]
      : new Date().toISOString().split('T')[0];

    navigate(`/seats/${bus.busId}`, {
      state: {
        ...bus,
        travelDate: formattedDate,
      },
    });
  };



  // Handle date change and automatically trigger search
  const handleDateChange = async (newDate) => {
    if (!newDate) return;
    
    try {
      setLoading(true);
      setError('');
      
      const formattedDate = new Date(newDate).toISOString().split('T')[0];
      
      // Update form data with new date
      const updatedFormData = {
        ...formData,
        travelDate: formattedDate
      };
      
      setFormData(updatedFormData);
      setSelectedDate(formattedDate); // Update selectedDate state
      
      // Only proceed with search if we have both source and destination
      if (!updatedFormData.fromCity || !updatedFormData.toCity) {
        setLoading(false);
        return;
      }
      
      // Show loading state
      setBuses([]);
      setFilteredBuses([]);
      
      // Call search with new date
      const response = await axios.get("https://localhost:7272/api/Routes/FilterRoutes", {
        params: {
          source: updatedFormData.fromCity.trim(),
          destination: updatedFormData.toCity.trim(),
          travelDate: formattedDate
        },
        headers: {
          'Content-Type': 'application/json',
        },
        paramsSerializer: params => {
          return Object.entries(params)
            .map(([key, value]) => `${encodeURIComponent(key)}=${encodeURIComponent(value)}`)
            .join('&');
        },
        timeout: 10000 // 10 second timeout
      });

      if (response.data && response.data.length > 0) {
        // Process the response data
        const formattedData = response.data.map((bus) => ({
          busId: bus.busid ?? bus.busId,
          busName: bus.busName ?? bus.busname,
          busType: bus.busType ?? bus.bustype,
          departureTime: bus.departuretime ?? bus.departureTime,
          arrivalTime: bus.arrivaltime ?? bus.arrivalTime,
          fare: bus.fare ?? 0,
          source: bus.source,
          destination: bus.destination,
          availableSeats: bus.availableSeats ?? bus.availableseats,
          totalSeats: bus.totalSeats ?? bus.totalseats,
          routeId: bus.routeId ?? bus.routeid,
          routeName: bus.routeName ?? `${bus.source} to ${bus.destination}`
        }));
        
        setBuses(formattedData);
        setFilteredBuses(formattedData);
        setError('');
      } else {
        setBuses([]);
        setFilteredBuses([]);
        setError("No buses found for the selected date.");
      }
    } catch (error) {
      console.error("Error updating search:", error);
      setError(error.response?.data?.message || "Failed to update search. Please try again.");
      setBuses([]);
      setFilteredBuses([]);
    } finally {
      setLoading(false);
    }
  };

  return (
    <>
      <BusNavbar />
      <Container fluid className="search-results-container">
        {/* Date Selection Card */}
        <Card className="mb-4 shadow-sm">
          <Card.Body>
            <div className="d-flex justify-content-between align-items-center flex-wrap">
              <div className="change-date">
                <h5 className="mb-2">Change Travel Date</h5>
                <div className="d-flex align-items-center">
                  <div className="position-relative">
                    <Form.Control
                      type="date"
                      value={selectedDate}
                      min={new Date().toISOString().split('T')[0]}
                      onChange={(e) => {
                        const newDate = e.target.value;
                        setSelectedDate(newDate); // Update UI immediately
                        handleDateChange(newDate); // Trigger search
                      }}
                      className="pe-5"
                      style={{ width: '200px', paddingRight: '2.5rem' }}
                      disabled={loading}
                    />
                    {loading ? (
                      <div className="position-absolute" style={{ right: '10px', top: '50%', transform: 'translateY(-50%)' }}>
                        <div className="spinner-border spinner-border-sm text-primary" role="status">
                          <span className="visually-hidden">Loading...</span>
                        </div>
                      </div>
                    ) : (
                      <FaCalendarAlt 
                        className="position-absolute" 
                        style={{ 
                          right: '10px', 
                          top: '50%', 
                          transform: 'translateY(-50%)',
                          color: '#6c757d',
                          pointerEvents: 'none'
                        }} 
                      />
                    )}
                  </div>
                </div>
              </div>
              <div className="mt-2 mt-md-0">
                <div className="d-flex align-items-center">
                  <FaCalendarAlt className="text-primary me-2" />
                  <div>
                    <div className="text-muted small">Showing Results For</div>
                    <div className="fw-bold">
                      {new Date(formData.travelDate).toLocaleDateString('en-US', {
                        weekday: 'long',
                        year: 'numeric',
                        month: 'long',
                        day: 'numeric'
                      })}
                    </div>
                  </div>
                </div>
              </div>
            </div>
          </Card.Body>
        </Card>
        <Container className="py-4">
          {/* Professional Header */}
          <div className="search-header">
            <div className="route-info-card">
              <div className="route-path">
                <div className="route-point">
                  <FaMapMarkerAlt className="route-icon text-success" />
                  <div>
                    <div className="route-city">{capitalize(formData.fromCity)}</div>
                    <small className="route-label">From</small>
                  </div>
                </div>
                <div className="route-arrow">
                  <FaArrowRight  className="ms-5" size={20}/>
                </div>
                <div className="route-point">
                  <FaMapMarkerAlt className="route-icon" />
                  <div>
                    <div className="route-city">{capitalize(formData.toCity)}</div>
                    <small className="route-label">To</small>
                  </div>
                </div>
              </div>
              <div className="journey-info">
                <div className="journey-date">
                  <FaCalendarAlt />
                  <span>{new Date(travelDate).toLocaleDateString('en-IN', {
                    weekday: 'long',
                    year: 'numeric',
                    month: 'long',
                    day: 'numeric'
                  })}</span>
                </div>
                <Badge className="bus-count-badge">
                  {filteredBuses.length} bus{filteredBuses.length !== 1 ? 'es' : ''} found
                </Badge>
                {isUsingMockData && (
                  <Badge className="demo-data-badge">
                    Demo Data
                  </Badge>
                )}
              </div>
            </div>
          </div>

          <div className="d-flex justify-content-between align-items-center mb-4">
            <h2>Available Buses</h2>
                       
          </div>

          {isUsingMockData && (
            <Alert variant="warning" className="mb-4">
              <i className="fas fa-exclamation-triangle me-2"></i>
              Using demo data. Some features may be limited.
            </Alert>
          )}

          {/* Filters and Sorting */}
          <Row className="mb-4">
            <Col lg={3}>
              <Card className="filter-card">
                <Card.Header className="filter-header">
                  <FaFilter className="me-2" />
                  Filters
                </Card.Header>
                <Card.Body>
                  <Form.Group className="mb-3">
                    <Form.Label>Bus Type</Form.Label>
                    <Form.Select value={filterType} onChange={(e) => setFilterType(e.target.value)}>
                      <option value="all">All Types</option>
                      <option value="sleeper">Sleeper</option>
                      <option value="seater">Seater</option>
                      <option value="ac">AC Buses</option>
                    </Form.Select>
                  </Form.Group>

                  <Form.Group className="mb-3">
                    <Form.Label>Price Range: ₹{priceRange[0]} - ₹{priceRange[1]}</Form.Label>
                    <Form.Range
                      min={0}
                      max={5000}
                      value={priceRange[1]}
                      onChange={(e) => setPriceRange([0, parseInt(e.target.value)])}
                    />
                  </Form.Group>
                </Card.Body>
              </Card>
            </Col>

            <Col lg={9}>
              <div className="sort-controls">
                <Form.Select value={sortBy} onChange={(e) => setSortBy(e.target.value)}>
                  <option value="departure">Sort by Departure</option>
                  <option value="arrival">Sort by Arrival</option>
                  <option value="price">Sort by Price (Low to High)</option>
                  <option value="rating">Sort by Rating</option>
                </Form.Select>
              </div>

              {/* Loading State */}
              {loading && (
                <div className="loading-container">
                  <Spinner animation="border" variant="danger" size="lg" />
                  <h4 className="mt-3">Finding the best buses for you...</h4>
                </div>
              )}

              {/* Error State */}
              {error && !loading && (
                <Alert variant="warning" className="error-alert">
                  <FaBus className="me-2" size={24} />
                  <Alert.Heading>Connection Issue</Alert.Heading>
                  <p>{error}</p>
                  <p className="mb-0"><strong>Don't worry! Demo buses are shown below for preview.</strong></p>
                  <Button variant="outline-warning" onClick={() => window.location.reload()}>
                    Retry Connection
                  </Button>
                </Alert>
              )}

              {/* Bus Cards */}
              <Row className="bus-cards-container">
                {!loading && filteredBuses.map((bus) => (
                  <Col lg={12} key={bus.busId} className="mb-3">
                    <Card className="bus-card">
                      <Card.Body className="p-0">
                        <Row className="g-0">
                          {/* Bus Info Section */}
                          <Col md={8}>
                            <div className="bus-info-section">
                              <div className="bus-header">
                                <div className="bus-name-section">
                                  <h5 className="bus-name">{bus.busName}</h5>
                                  <div className="bus-operator">{bus.busOperator}</div>
                                  <Badge className="bus-type-badge">{bus.busType}</Badge>
                                </div>
                                <div className="bus-rating">
                                  <div className="rating-badge" style={{ backgroundColor: getRatingColor(bus.ratingValue) }}>
                                    <FaStar />
                                    <span>{Number(bus.ratingValue).toFixed(1)}</span>
                                  </div>
                                  <div className="rating-details">
                                    <small className="rating-text">Excellent</small>
                                    <small className="rating-count">({Math.floor(Math.random() * 500) + 100} reviews)</small>
                                  </div>
                                </div>
                              </div>

                              {/* From and To Route Display */}
                              <div className="route-display-card">
                                <div className="route-from-to">
                                  <div className="route-stop">
                                    <FaMapMarkerAlt className="route-stop-icon text-success" />
                                    <div className="route-stop-details">
                                      <div className="route-stop-city">{bus.source}</div>
                                      <small className="route-stop-label">From</small>
                                    </div>
                                  </div>
                                  <div className="route-connector">
                                    <FaArrowRight className="route-arrow" />
                                  </div>
                                  <div className="route-stop">
                                    <FaMapMarkerAlt className="route-stop-icon" />
                                    <div className="route-stop-details">
                                      <div className="route-stop-city">{bus.destination}</div>
                                      <small className="route-stop-label">To</small>
                                    </div>
                                  </div>
                                </div>
                              </div>

                              <div className="bus-route">
                                <div className="route-display">
                                  <span className="departure-time">{bus.departureTime}</span>
                                  <span className="route-separator">───</span>
                                  <span className="arrival-time">{bus.arrivalTime}</span>
                                </div>
                              
                              </div>

                            </div>
                          </Col>

                          {/* Price and Booking Section */}
                          <Col md={4}>
                            <div className="booking-section">
                              <div className="price-info">
                                <div className="fare-amount">
                                  <FaRupeeSign />
                                  <span className="amount">{bus.fare}</span>
                                </div>
                                <div className="fare-label">Starting from</div>
                                <div className="seats-availabilitys ">
                                  <div className={`seats-remaining ${bus.availableSeats <= 5 ? 'seats-low' : bus.availableSeats <= 15 ? 'seats-medium' : 'seats-high'}`}>
                                    <FaChair className="me-1" />
                                    {seatAvailability[bus.busId] ? (
                                      <span>
                                        <span className="available">{seatAvailability[bus.busId]?.available || 0} available</span>
                                        <small className="text-muted ms-2">({seatAvailability[bus.busId]?.booked || 0} booked)</small>
                                      </span>
                                    ) : (
                                      <span>{bus.availableSeats} seats left</span>
                                    )}
                                  </div>
                                </div>
                              </div>

                              <Button
                                variant="danger"
                                size="lg"
                                className="book-now-btn"
                                onClick={() => handleBookNow(bus)}
                              >
                                <FaChair className="me-2" />
                                Select Seats
                              </Button>

                              <div className="cancellation-policy">
                                <small>Free cancellation till 2 hours before departure</small>
                              </div>
                            </div>
                          </Col>
                        </Row>
                      </Card.Body>
                    </Card>
                  </Col>
                ))}
              </Row>
            </Col>
          </Row>
        </Container>
      </Container>
    </>
  );
}

/**
 * Calculates the duration between departure and arrival times
 * @param {string} departure - Departure time in HH:MM format
 * @param {string} arrival - Arrival time in HH:MM format
 * @returns {string} Formatted duration string (e.g., "5h 30m")
 */
const calculateDuration = (departure, arrival) => {
  const depTime = new Date(`2000-01-01T${departure}:00`);
  const arrTime = new Date(`2000-01-01T${arrival}:00`);

  if (arrTime < depTime) {
    arrTime.setDate(arrTime.getDate() + 1);
  }

  const diffMs = arrTime - depTime;
  const hours = Math.floor(diffMs / (1000 * 60 * 60));
  const minutes = Math.floor((diffMs % (1000 * 60 * 60)) / (1000 * 60));

  return `${hours}h ${minutes}m`;
};

export default SearchResult;
