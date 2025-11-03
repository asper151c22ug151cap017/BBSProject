// ============================================================================
// Project Name : BusBookingSystem
// File Name    : layout.jsx
// Created By   : [Your Name]
// Created On   : [Current Date]
// Modified By  : [Your Name]
// Modified On  : [Current Date]
// Description  : Main layout component that provides the application structure
//                including the navigation bar and content area.
// ============================================================================

import { Outlet } from "react-router";
import BusNavbar from "./Navbar";
import HomePage from "./Home";

/**
 * Layout Component
 * 
 * Provides the main layout structure for the application including:
 * - Navigation bar at the top
 * - Content area that renders child routes
 * - Consistent styling and structure across all pages
 * 
 * @component
 * @returns {JSX.Element} The application layout with navigation and content area
 */
function Layout() {

  return (
    <>
    <BusNavbar/>
   
    <Outlet/>
    
    </>
  );
}

export default Layout;
