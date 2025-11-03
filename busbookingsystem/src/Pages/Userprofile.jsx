// ============================================================================
// Project Name : BusBookingSystem
// File Name    : Userprofile.jsx
// Created By   : Kaviraj M
// Created On   : 24/10/2025
// Modified By  : Kaviraj M
// Modified On  : 30/10/2025
// Description  : Displays and updates user profile information (used inside HomePage Offcanvas)
// ============================================================================

import React, { useEffect, useState } from "react";
import { Card, Spinner, Alert, Button, Form, Container } from "react-bootstrap";
import axios from "axios";
import {
  FaUserCircle,
  FaEnvelope,
  FaPhone,
  FaEdit,
  FaSave,
  FaTimes,
  FaExclamationTriangle,
} from "react-icons/fa";
import { useNavigate } from "react-router-dom";

const API_GET = "https://localhost:7272/api/User/Getuserbyid";
const API_UPDATE = "https://localhost:7272/api/User/UpdateUserProfile";

function ProfilePage() {
  const [user, setUser] = useState(null);
  const [loading, setLoading] = useState(true);
  const [isEditing, setIsEditing] = useState(false);
  const [formData, setFormData] = useState({ name: "", email: "", phone: "" });
  const [updating, setUpdating] = useState(false);
  const [updateMessage, setUpdateMessage] = useState("");
  const [error, setError] = useState("");

  const navigate = useNavigate();
  const loggedInUserId = localStorage.getItem("userid");

  useEffect(() => {
    if (!loggedInUserId) {
      navigate("/login");
      return;
    }

    const fetchUser = async () => {
      try {
        const res = await axios.get(`${API_GET}?Userid=${loggedInUserId}`);
        if (Array.isArray(res.data) && res.data.length > 0) {
          const userData = res.data[0];
          setUser(userData);
          setFormData({
            name: userData.name || "",
            email: userData.email || "",
            phone: userData.phone || "",
          });
        } else {
          setError("User not found.");
        }
      } catch (err) {
        console.error(err);
        setError("Failed to load user details.");
      } finally {
        setLoading(false);
      }
    };

    fetchUser();
  }, [loggedInUserId, navigate]);

  const handleEdit = () => {
    setIsEditing(true);
    setUpdateMessage("");
  };

  const handleCancel = () => {
    setIsEditing(false);
    setFormData({
      name: user.name,
      email: user.email,
      phone: user.phone,
    });
  };

  const handleChange = (e) => {
    setFormData((prev) => ({ ...prev, [e.target.name]: e.target.value }));
  };

  const handleSave = async () => {
    try {
      setUpdating(true);
      const payload = {
        userId: parseInt(loggedInUserId),
        ...formData,
      };

      const res = await axios.put(API_UPDATE, payload);
      setUpdateMessage(res.data || "Profile updated successfully!");
      setUser({ ...user, ...formData });
      setIsEditing(false);
    } catch (err) {
      console.error(err);
      setUpdateMessage("Failed to update profile. Try again.");
    } finally {
      setUpdating(false);
    }
  };

  if (loading)
    return (
      <Container className="mt-5 text-center">
        <Spinner animation="border" variant="danger" />
        <p className="mt-3">Loading profile...</p>
      </Container>
    );

  if (error)
    return (
      <Container className="mt-5 text-center">
        <Alert variant="danger">
          <FaExclamationTriangle className="me-2" />
          {error}
        </Alert>
      </Container>
    );

  return (
    <div className="text-center mt-3">
      <Card className="border-0 shadow-sm">
        <Card.Body>
          <FaUserCircle size={80} className="text-danger mb-3" />

          {isEditing ? (
            <Form>
              <Form.Group className="mb-3 text-start">
                <Form.Label>Name</Form.Label>
                <Form.Control
                  type="text"
                  name="name"
                  value={formData.name}
                  onChange={handleChange}
                />
              </Form.Group>

              <Form.Group className="mb-3 text-start">
                <Form.Label>Email</Form.Label>
                <Form.Control
                  type="email"
                  name="email"
                  value={formData.email}
                  onChange={handleChange}
                />
              </Form.Group>

              <Form.Group className="mb-3 text-start">
                <Form.Label>Phone</Form.Label>
                <Form.Control
                  type="text"
                  name="phone"
                  value={formData.phone}
                  onChange={handleChange}
                />
              </Form.Group>

              {updateMessage && (
                <Alert
                  variant={
                    updateMessage.toLowerCase().includes("success")
                      ? "success"
                      : "danger"
                  }
                  className="text-center"
                >
                  {updateMessage}
                </Alert>
              )}

              <div className="d-grid gap-2 mt-3">
                <Button
                  variant="success"
                  onClick={handleSave}
                  disabled={updating}
                  className="d-flex align-items-center justify-content-center"
                >
                  {updating ? (
                    <>
                      <Spinner animation="border" size="sm" className="me-2" />
                      Saving...
                    </>
                  ) : (
                    <>
                      <FaSave className="me-2" />
                      Save
                    </>
                  )}
                </Button>
                <Button
                  variant="secondary"
                  onClick={handleCancel}
                  className="d-flex align-items-center justify-content-center"
                >
                  <FaTimes className="me-2" />
                  Cancel
                </Button>
              </div>
            </Form>
          ) : (
            <>
              <h4>{user.name}</h4>
              <p className="text-muted">
                <FaEnvelope className="me-1" /> {user.email}
              </p>
              <p>
                <FaPhone className="me-1" /> <strong>{user.phone}</strong>
              </p>

              {updateMessage && (
                <Alert
                  variant={
                    updateMessage.toLowerCase().includes("success")
                      ? "success"
                      : "danger"
                  }
                  className="text-center mt-3"
                >
                  {updateMessage}
                </Alert>
              )}

              <div className="d-grid gap-2 mt-4">
                <Button
                  variant="primary"
                  onClick={handleEdit}
                  className="d-flex align-items-center justify-content-center"
                >
                  <FaEdit className="me-2" />
                  Edit Profile
                </Button>
              </div>
            </>
          )}
        </Card.Body>
      </Card>
    </div>
  );
}

export default ProfilePage;
