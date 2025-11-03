import { Routes, Route, Navigate } from "react-router-dom";
import LoginPage from "./Pages/Login";
import SignUpPage from "./Pages/Signup";
import HomePage from "./Pages/Home";
import SearchResults from "./Pages/searchresult";
import AdminDashboard from "./Pages/Admindashboard";
import Layout from "./Pages/layout";
import AdminHomePage from "./Pages/AdminHome";
import LandingPage from "./Pages/landingpage";
import BookingConfirmation from "./Pages/Confirmbooking";
import AdminBusPage from "./Pages/AdminBuspage";
import BookedPage from "./Pages/Bookedpage";
import ProfilePage from "./Pages/Userprofile";
import UserBookings from "./Pages/Navbooking";
import Seatspage from "./Pages/Seatspage";
import HelpPage from "./Pages/Navhelp";
import AdminBookings from "./Pages/AdminBooking";
import AdminRoutesPage from "./Pages/Adminroutes";
import AdminUserPage from "./Pages/Adminuserpage";
function App() {
  return (
    <Routes>
      {/* Public Routes - No Navbar */}
      <Route path="/" element={<LandingPage />} />
      <Route path="/login" element={<LoginPage />} />
      <Route path="/signup" element={<SignUpPage />} />

      {/* Protected Routes - With Navbar */}
      <Route element={<Layout />}>
        <Route path="/home" element={<HomePage />} />

      </Route>
      <Route path="/searchresult" element={<SearchResults />} />
      <Route path="/Seatspage/:busId" element={<Seatspage />} />
      <Route path="/confirmbooking/:busId" element={<BookingConfirmation/>}/>
      <Route path="/Booked/:bookingId" element={<BookedPage/>}/>
       <Route path="profile" element={<ProfilePage/>}/>
       <Route path="/Bookings" element={<UserBookings/>}/>
      <Route path="/help" element={<HelpPage/>}/>


      {/* Admin Dashboard */}
      <Route path="/Admindashboard" element={<AdminDashboard />}>
        <Route index element={<AdminHomePage />} /> {/* default = home page */}
        <Route path="home" element={<AdminHomePage />} />
        <Route path="users" element={<AdminUserPage />} />
        <Route path="route" element={<AdminRoutesPage />} />
        <Route path="buses" element={<AdminBusPage />} />
       <Route path="bookings" element={<AdminBookings/>}/>
        {/* Add RoutePage, BookingPage, BusesPage here later */}
      </Route>



      {/* Redirect unknown routes */}
      <Route path="*" element={<Navigate to="/login" replace />} />
    </Routes>
  );
}

export default App;
