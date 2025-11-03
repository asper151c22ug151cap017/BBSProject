// ============================================================================
// Project Name : BusBookingSystem
// File Name    : SignUpPage.jsx
// Created By   : Kaviraj M
// Created On   : 22/09/2025
// Modified By  : Kaviraj M
// Modified On  : 28/10/2025
// Description  : React component that handles user registration for the 
//                Bus Booking System. It collects user details, validates 
//                passwords, and integrates with the backend API to create 
//                a new account. On successful registration, users are 
//                redirected to the Home page.
// ============================================================================

import React, { useState } from "react";
import { Form, Button, Card, Row, Col } from "react-bootstrap";
import { FaBus } from "react-icons/fa";
import axios from "axios";
import { useNavigate, Link } from "react-router-dom";
import "./Auth.css";


function SignUpPage() {
  const navigate = useNavigate(); // Navigation hook for redirecting after signup

  // ============================================================================
  // State : formData
  // Description : Stores user registration input values.
  // ============================================================================
  const [formData, setFormData] = useState({
    name: "",
    email: "",
    phone: "",
    password: "",
    confirmPassword: "",
    age: "",
    genderId: "", // 1 = Male, 2 = Female, 3 = Transgender
  });
const api = axios.create({
  baseURL: 'https://localhost:7272/api', // ✅ include 'api' if your backend uses it
  headers: { 'Content-Type': 'application/json' }
});

  // ============================================================================
  // State : error
  // Description : Stores validation or API-related error messages.
  // ============================================================================
  const [error, setError] = useState("");

  // ============================================================================
  // Function Name : handleChange
  // Description   : Updates form input values dynamically and clears errors.
  // Parameters    : e (event) - Input change event
  // ============================================================================
  const handleChange = (e) => {
    setFormData({ ...formData, [e.target.name]: e.target.value });
    setError("");
  };

  // ============================================================================
  // Function Name : handleSubmit
  // Description   : Validates form data and sends a POST request to backend API 
  //                 for user registration. Handles success/failure states and 
  //                 navigates to Home upon success.
  // Parameters    : e (event) - Form submit event
  // ============================================================================
  const handleSubmit = async (e) => {
    e.preventDefault();

    // Password validation
    if (formData.password !== formData.confirmPassword) {
      setError("⚠️ Passwords do not match");
      return;
    }

    try {
      const response = await api.post("/User/Adduser", {
        name: formData.name,
        email: formData.email,
        phone: formData.phone,
        password: formData.password,
        age: parseInt(formData.age) || 0,
        genderId: parseInt(formData.genderId),
      });

      // If user registration succeeds
      if (response.status === 200 || response.status === 201) {
        if (response.data.token) {
          localStorage.setItem("token", response.data.token);
        }
        if (response.data.user) {
          localStorage.setItem("user", JSON.stringify(response.data.user));
        }

        navigate("/Home"); // Redirect to homepage after signup
      }
    } catch (err) {
      // Handle backend validation or server errors
      setError(
        err.response?.data?.message ||
          "❌ Registration failed, please try again"
      );
    }
  };

  // ============================================================================
  // UI SECTION : Registration Form Layout
  // Description : Uses Bootstrap layout to create a responsive registration 
  //               form with validation messages and gender selection.
  // ============================================================================
  return (
    <div className="auth-container">
      {/* Left Background Section */}
      <div className="auth-bg"></div>

      {/* Signup Card */}
      <Card className="signup_card p-4 shadow-lg">
        <div className="text-center mb-3">
          <FaBus size={40} color="#28a745" />
          <h4 className="mt-2 fw-bold">Create Account</h4>
        </div>

        <Form onSubmit={handleSubmit}>
          {/* ===========================================================
              Name & Email Fields
          ============================================================ */}
          <Row className="mb-3">
            <Col>
              <Form.Control
                type="text"
                placeholder="Full Name"
                name="name"
                value={formData.name}
                onChange={handleChange}
                autoComplete="off"
                required
              />
            </Col>
            <Col>
              <Form.Control
                type="email"
                placeholder="Email"
                name="email"
                value={formData.email}
                onChange={handleChange}
                autoComplete="off"
                required
              />
            </Col>
          </Row>

          {/* ===========================================================
              Phone & Password Fields
          ============================================================ */}
          <Row className="mb-3">
            <Col>
              <Form.Control
                type="tel"
                placeholder="Phone Number"
                name="phone"
                value={formData.phone}
                onChange={handleChange}
                autoComplete="off"
                required
              />
            </Col>
            <Col>
              <Form.Control
                type="password"
                placeholder="Password"
                name="password"
                value={formData.password}
                onChange={handleChange}
                autoComplete="new-password"
                required
              />
            </Col>
          </Row>

          {/* ===========================================================
              Confirm Password & Age Fields
          ============================================================ */}
          <Row className="mb-3">
            <Col>
              <Form.Control
                type="password"
                placeholder="Confirm Password"
                name="confirmPassword"
                value={formData.confirmPassword}
                onChange={handleChange}
                autoComplete="new-password"
                required
              />
              {error && <p className="text-danger mt-2">{error}</p>}
            </Col>
            <Col md={4}>
              <Form.Control
                type="number"
                placeholder="Age"
                name="age"
                value={formData.age}
                onChange={handleChange}
                min="1"
                required
              />
            </Col>
          </Row>

          {/* ===========================================================
              Gender Selection Section
          ============================================================ */}
          <Row className="mb-3">
            <Col>
              <div className="d-flex align-items-center gap-3">
                <Form.Label className="mb-0 fw-semibold">Gender:</Form.Label>
                <div className="d-flex gap-3">
                  <Form.Check
                    inline
                    type="radio"
                    label="Male"
                    name="genderId"
                    value="1"
                    checked={formData.genderId === "1"}
                    onChange={handleChange}
                    required
                  />
                  <Form.Check
                    inline
                    type="radio"
                    label="Female"
                    name="genderId"
                    value="2"
                    checked={formData.genderId === "2"}
                    onChange={handleChange}
                    required
                  />
                  <Form.Check
                    inline
                    type="radio"
                    label="Transgender"
                    name="genderId"
                    value="3"
                    checked={formData.genderId === "3"}
                    onChange={handleChange}
                    required
                  />
                </div>
              </div>
            </Col>
          </Row>

          {/* ===========================================================
              Submit Button
          ============================================================ */}
          <Button variant="success" type="submit" className="w-100 mb-3">
            Sign Up
          </Button>

          {/* ===========================================================
              Redirect to Login Page
          ============================================================ */}
          <p className="text-center mb-0">
            Already have an account?{" "}
            <Link to="/login" className="fw-bold text-success">
              Login
            </Link>
          </p>
        </Form>
      </Card>
    </div>
  );
}

export default SignUpPage;
