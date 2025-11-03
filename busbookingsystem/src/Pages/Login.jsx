// ============================================================================
// Project Name : BusBookingSystem
// File Name    : LoginPage.jsx
// Created By   : Kaviraj M
// Created On   : 22/09/2025
// Modified By  : Kaviraj M
// Modified On  : 28/10/2025
// Description  : React component that manages user authentication for the 
//                Bus Booking System. It connects to the backend API for 
//                login validation, decodes JWT tokens, stores user data, and 
//                performs role-based navigation for Admins and Users.
// ============================================================================

import React, { useState } from "react";
import { Form, Button, Card } from "react-bootstrap";
import { FaBus } from "react-icons/fa";
import { useFormik } from "formik";
import * as Yup from "yup";
import { useNavigate, Link } from "react-router-dom";
import axios from "axios";
import "./Auth.css";

function LoginPage() {
  const navigate = useNavigate(); // Navigation hook for redirecting user
  const [serverError, setServerError] = useState(""); // Stores backend or login error messages

  // ============================================================================
  // Function Name : decodeJwt
  // Description   : Decodes the JWT token to extract user details like role, 
  //                 email, and user ID from the backend authentication response.
  // Parameters    : token (string) - The JWT token string from backend.
  // Return Type   : object | null - Returns decoded payload or null on failure.
  // ============================================================================
  const decodeJwt = (token) => {
    try {
      const base64Url = token.split(".")[1];
      const base64 = base64Url.replace(/-/g, "+").replace(/_/g, "/");
      const jsonPayload = decodeURIComponent(
        atob(base64)
          .split("")
          .map((c) => "%" + ("00" + c.charCodeAt(0).toString(16)).slice(-2))
          .join("")
      );
      return JSON.parse(jsonPayload);
    } catch {
      return null;
    }
  };

  // ============================================================================
  // Function Name : loginFormFormik
  // Description   : Handles login form state, input validation, and API call 
  //                 for authenticating the user. Stores token & user data in 
  //                 localStorage and navigates based on user role.
  // ============================================================================
  const loginFormFormik = useFormik({
    // Initial form values
    initialValues: { email: "", password: "" },

    // Validation schema using Yup
    validationSchema: Yup.object({
      email: Yup.string().required("Email is required"),
      password: Yup.string().required("Password is required"),
    }),

    // ------------------------------------------------------------------------
    // Function Name : onSubmit
    // Description   : Executes API login request, validates JWT, and redirects 
    //                 user to the respective dashboard.
    // Parameters    : values - Form data (email, password)
    // ------------------------------------------------------------------------
    onSubmit: async (values, { setSubmitting }) => {
      setServerError(""); // Reset previous error messages
      try {
        const res = await axios.post("https://localhost:7272/api/Auth/login", {
          email: values.email.trim(),
          password: values.password.trim(),
        });

        if (res.data.token) {
          const decoded = decodeJwt(res.data.token);
          if (!decoded) {
            setServerError("Failed to decode token. Contact admin.");
            return;
          }

          // Extract claims from token
          const role =
            decoded.role ||
            decoded["http://schemas.microsoft.com/ws/2008/06/identity/claims/role"];
          const email =
            decoded.name ||
            decoded["http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name"];
          const userid = decoded.userid || decoded.sub;

          // Store authentication data in localStorage
          localStorage.setItem("token", res.data.token);
          localStorage.setItem("userid", userid);
        localStorage.setItem("username", res.data.userName);  
          localStorage.setItem("role", role);
          localStorage.setItem("email", email);

          // Role-based navigation
          if (role === "1") navigate("/Admindashboard");
          else if (role === "2") navigate("/Home");
          else setServerError("Invalid role. Contact admin.");
        } else {
          setServerError("Invalid email or password");
        }
      } catch (err) {
        if (err.response?.status === 401)
          setServerError("Invalid email or password");
        else setServerError("Something went wrong. Please try again.");
      } finally {
        setSubmitting(false); // Disable loading state
      }
    },
  });

  // ============================================================================
  // UI SECTION : Login Form with Bootstrap Components
  // Description : Renders form fields, validations, and login button
  // ============================================================================
  return (
    <div className="auth-container">
      {/* Left Side Background */}
      <div className="auth-bg float"></div>

      {/* Login Card */}
      <Card className="login_card p-4 shadow-lg">
        <div className="text-center mb-4">
          <FaBus size={40} color="#007bff" />
          <h3 className="mt-2">Bus Booking Login</h3>
        </div>

        {/* Login Form */}
        <Form onSubmit={loginFormFormik.handleSubmit}>
          {/* Email Field */}
          <Form.Group className="mb-3" controlId="loginEmail">
            <Form.Label>Email</Form.Label>
            <Form.Control
              type="email"
              placeholder="Enter email"
              name="email"
              value={loginFormFormik.values.email}
              onChange={loginFormFormik.handleChange}
              onBlur={loginFormFormik.handleBlur}
              isInvalid={
                loginFormFormik.touched.email && !!loginFormFormik.errors.email
              }
              autoComplete="email"
            />
            <Form.Control.Feedback type="invalid">
              {loginFormFormik.errors.email}
            </Form.Control.Feedback>
          </Form.Group>

          {/* Password Field */}
          <Form.Group className="mb-3" controlId="loginPassword">
            <Form.Label>Password</Form.Label>
            <Form.Control
              type="password"
              placeholder="Enter password"
              name="password"
              value={loginFormFormik.values.password}
              onChange={loginFormFormik.handleChange}
              onBlur={loginFormFormik.handleBlur}
              isInvalid={
                loginFormFormik.touched.password &&
                !!loginFormFormik.errors.password
              }
              autoComplete="password"
            />
            <Form.Control.Feedback type="invalid">
              {loginFormFormik.errors.password}
            </Form.Control.Feedback>
          </Form.Group>

          {/* Server Error Display */}
          {serverError && (
            <p className="text-danger text-center">{serverError}</p>
          )}

          {/* Submit Button */}
          <Button
            variant="primary"
            type="submit"
            className="w-100"
            disabled={loginFormFormik.isSubmitting}
          >
            {loginFormFormik.isSubmitting ? "Logging in..." : "Login"}
          </Button>
        </Form>

        {/* Redirect to Sign Up */}
        <p className="text-center mt-3">
          Don’t have an account? <Link to="/signup">Sign Up</Link>
        </p>
      </Card>
    </div>
  );
}

export default LoginPage;
