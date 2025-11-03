// ============================================================================
// Project Name : BusBookingSystem
// File Name    : IBBSbooking.cs
// Created By   : Kaviraj M
// Created On   : 20/09/2025
// Modified By  : Kaviraj M
// Modified On  : 28/10/2025
// Description  : Defines the repository interface for managing bookings within
//                the Bus Booking System. Handles operations such as adding,
//                updating, deleting, retrieving, and counting bookings.
// ============================================================================

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusBookingSystem.Application.BookingDtos;

namespace BusBookingSystem.Infrastructure.RepositoryInterface
{
    /// <summary>
    /// Provides a contract for managing bookings in the Bus Booking System.
    /// Defines methods to create, update, delete, and retrieve booking records.
    /// </summary>
    public interface IBBSbooking
    {

        // --------------------------------------------------------------------
        // ✅ GET ALL BOOKINGS
        // --------------------------------------------------------------------
        /// <summary>
        /// Retrieves all bookings including related route, bus, seat, 
        /// and passenger details.
        /// </summary>
        /// <returns>
        /// A list of <see cref="Responsegetbooking"/> representing all bookings.
        /// </returns>
        List<Responsegetbooking> Getallbookings();

        // --------------------------------------------------------------------
        // ✅ ADD BOOKING
        // --------------------------------------------------------------------
        /// <summary>
        /// Creates a new booking with associated passenger and seat information.
        /// </summary>
        /// <param name="addbookings">
        /// The <see cref="RequestAddbookings"/> object containing booking details.
        /// </param>
        /// <returns>
        /// A string message indicating the result of the add operation.
        /// </returns>
        string AddBooking(RequestAddbookings addbookings);

        // --------------------------------------------------------------------
        // ✅ UPDATE BOOKING
        // --------------------------------------------------------------------
        /// <summary>
        /// Updates existing booking details.
        /// </summary>
        /// <param name="updatebooking">
        /// The <see cref="Requestupdatebooking"/> object containing updated booking details.
        /// </param>
        /// <returns>
        /// A string message indicating the success or failure of the update.
        /// </returns>
        string UpdateBooking(Requestupdatebooking updatebooking);

        // --------------------------------------------------------------------
        // ✅ DELETE BOOKING
        // --------------------------------------------------------------------
        /// <summary>
        /// Marks a booking as deleted (soft delete) using its unique booking ID.
        /// </summary>
        /// <param name="Bookingid">The unique ID of the booking.</param>
        /// <returns>
        /// A string message indicating whether the delete was successful.
        /// </returns>
        string DeleteBooking(int Bookingid);

        // --------------------------------------------------------------------
        // ✅ GET BOOKING COUNT
        // --------------------------------------------------------------------
        /// <summary>
        /// Retrieves the total count of bookings in the system.
        /// </summary>
        /// <returns>
        /// The total number of bookings as an integer value.
        /// </returns>
        int Getcountbookings();

        // --------------------------------------------------------------------
        // ✅ GET RECENT BOOKINGS
        // --------------------------------------------------------------------
        /// <summary>
        /// Retrieves bookings made within the current day.
        /// </summary>
        /// <returns>
        /// A list of <see cref="ResponseGetrecentbookings"/> representing recent bookings.
        /// </returns>
        List<ResponseGetrecentbookings> Getrecentbookings();
     
    }
}
