// ============================================================================
// Project Name : BusBookingSystem
// File Name    : IBBSbooking.cs
// Created By   : Kaviraj M
// Created On   : 20/09/2025
// Modified By  : Kaviraj M
// Modified On  : 05/11/2025
// Description  : Defines the asynchronous repository interface for managing 
//                bookings within the Bus Booking System. Handles operations 
//                such as adding, updating, deleting, retrieving, and counting 
//                bookings asynchronously.
// ============================================================================

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BusBookingSystem.Application.BookingDtos;

namespace BusBookingSystem.Infrastructure.RepositoryInterface
{
    /// <summary>
    /// Provides an asynchronous contract for managing bookings in the Bus Booking System.
    /// Defines async methods to create, update, delete, and retrieve booking records.
    /// </summary>
    public interface IBBSbooking
    {
        // --------------------------------------------------------------------
        // ✅ GET ALL BOOKINGS (ASYNC)
        // --------------------------------------------------------------------
        /// <summary>
        /// Asynchronously retrieves all bookings including related route, bus, 
        /// seat, and passenger details.
        /// </summary>
        /// <returns>
        /// A task representing the asynchronous operation. The task result 
        /// contains a list of <see cref="Responsegetbooking"/> representing all bookings.
        /// </returns>
        Task<List<Responsegetbooking>> GetAllBookingsAsync();

        // --------------------------------------------------------------------
        // ✅ ADD BOOKING (ASYNC)
        // --------------------------------------------------------------------
        /// <summary>
        /// Asynchronously creates a new booking with associated passenger and seat information.
        /// </summary>
        /// <param name="addbookings">
        /// The <see cref="RequestAddbookings"/> object containing booking details.
        /// </param>
        /// <returns>
        /// A task representing the asynchronous operation. The task result contains 
        /// a string message indicating the result of the add operation.
        /// </returns>
        Task<string> AddBookingAsync(RequestAddbookings addbookings);

        // --------------------------------------------------------------------
        // ✅ UPDATE BOOKING (ASYNC)
        // --------------------------------------------------------------------
        /// <summary>
        /// Asynchronously updates existing booking details.
        /// </summary>
        /// <param name="updatebooking">
        /// The <see cref="Requestupdatebooking"/> object containing updated booking details.
        /// </param>
        /// <returns>
        /// A task representing the asynchronous operation. The task result contains 
        /// a string message indicating the success or failure of the update.
        /// </returns>
        Task<string> UpdateBookingAsync(Requestupdatebooking updatebooking);

        // --------------------------------------------------------------------
        // ✅ DELETE BOOKING (ASYNC)
        // --------------------------------------------------------------------
        /// <summary>
        /// Asynchronously marks a booking as deleted (soft delete) using its unique booking ID.
        /// </summary>
        /// <param name="Bookingid">The unique ID of the booking.</param>
        /// <returns>
        /// A task representing the asynchronous operation. The task result contains 
        /// a string message indicating whether the delete was successful.
        /// </returns>
        Task<string> DeleteBookingAsync(int Bookingid);

        // --------------------------------------------------------------------
        // ✅ GET BOOKING COUNT (ASYNC)
        // --------------------------------------------------------------------
        /// <summary>
        /// Asynchronously retrieves the total count of bookings in the system.
        /// </summary>
        /// <returns>
        /// A task representing the asynchronous operation. The task result contains 
        /// the total number of bookings as an integer value.
        /// </returns>
        Task<int> GetCountBookingsAsync();

        // --------------------------------------------------------------------
        // ✅ GET RECENT BOOKINGS (ASYNC)
        // --------------------------------------------------------------------
        /// <summary>
        /// Asynchronously retrieves bookings made within the current day.
        /// </summary>
        /// <returns>
        /// A task representing the asynchronous operation. The task result contains 
        /// a list of <see cref="ResponseGetrecentbookings"/> representing recent bookings.
        /// </returns>
        Task<List<ResponseGetrecentbookings>> GetRecentBookingsAsync();
    }
}
