// ================================================================================
// Project Name : BusBookingSystem
// File Name    : BookingController.cs
// Created By   : Kaviraj M
// Created On   : 20/09/2025
// Modified By  : Kaviraj M
// Modified On  : 05/11/2025
// Description  : Handles all booking-related operations in the Bus Booking System,
//                including creating, updating, deleting, and retrieving bookings.
// =================================================================================

using BusBookingSystem.Application;
using BusBookingSystem.Application.BookingDtos;
using BusBookingSystem.Application.BusDtos;
using BusBookingSystem.Infrastructure.RepositoryImplementation;
using BusBookingSystem.Infrastructure.RepositoryInterface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BusBookingSystem.API.Controllers
{

    /// <summary>
    /// Provides endpoints for managing bookings in the Bus Booking System.
    /// Supports CRUD operations, booking counts, and recent booking retrieval.
    /// </summary>

    [Route("api/[controller]")]
    [ApiController]

    public class BookingController : ControllerBase
    {
        private readonly IBBSbooking _bBSbooking;
        private readonly ErrorHandler _errorHandler;

        /// <summary>
        /// Initializes a new instance of the <see cref="BookingController"/> class
        /// with dependency injection for the booking repository.
        /// </summary>
        public BookingController(IBBSbooking bBSbooking, ErrorHandler errorHandler)
        {
            _bBSbooking = bBSbooking;
            _errorHandler = errorHandler;
        }


        // --------------------------------------------------------------------
        // ✅ GET ALL BOOKINGS
        // --------------------------------------------------------------------
        /// <summary>
        /// Retrieves all bookings along with their associated route, bus, and passenger details.
        /// </summary>
        /// <returns>Returns a list of all bookings.</returns>
        [HttpGet]
        [Route("Getallbookings")]
        [Authorize]
        public async Task<IActionResult> Getallbookings()
        {
            try
            {
                var getbookings = await _bBSbooking.GetAllBookingsAsync();
                return Ok(getbookings);
            }
            catch (Exception ex)
            {
                // You can log the error
                _errorHandler.Capture(ex, "Error fetching all bookings");

                // Return proper response
                return StatusCode(500, new { message = "Something went wrong", error = ex.Message });
            }
        }

        // --------------------------------------------------------------------
        // ✅ GET BOOKING COUNT
        // --------------------------------------------------------------------
        /// <summary>
        /// Retrieves the total number of bookings in the system.
        /// </summary>
        [HttpGet]
        [Route ("Getbokingcounts")]
        [Authorize]

        public async Task< IActionResult> Getbookingcount()
        {
            try
            {
                var bookingcount = await _bBSbooking.GetCountBookingsAsync();
                return Ok(bookingcount);
            }
            catch (Exception ex)
            {
                // You can log the error
                _errorHandler.Capture(ex, "Error fetching bookings count");

                // Return proper response
                return StatusCode(500, new { message = "Something went wrong", error = ex.Message });
            }

        }

        // --------------------------------------------------------------------
        // ✅ ADD BOOKING
        // --------------------------------------------------------------------
        /// <summary>
        /// Adds a new booking with passenger and seat details.
        /// </summary>
        /// <param name="addBooking">The booking data transfer object containing booking details.</param>
        /// <returns>Status message indicating whether the booking was successfully added.</returns>
        [HttpPost]
        [Route("AddBooking")]
        [Authorize]
        public async Task<IActionResult> AddBooking(RequestAddbookings addBooking)
        {
            try
           { 
                var addbooking = await _bBSbooking.AddBookingAsync(addBooking);
                return Ok(addbooking);
            }
            catch (Exception ex)
            {
                // You can log the error
                _errorHandler.Capture(ex, "Error while  Add  bookings");

                // Return proper response
                return StatusCode(500, new { message = "Something went wrong", error = ex.Message });
            }
        }

        // --------------------------------------------------------------------
        // ✅ UPDATE BOOKING
        // --------------------------------------------------------------------
        /// <summary>
        /// Updates existing booking information such as passenger details or seat assignments.
        /// </summary>
        /// <param name="updateBooking">The booking update DTO containing new details.</param>
        /// <returns>Status message indicating update success or failure.</returns>

        [HttpPut]
        [Route("UpdateBus")]
        [Authorize]

        public async Task<IActionResult> UpdateBus(Requestupdatebooking updateBooking)
        {
            try
            {         
                var updatebookings = await _bBSbooking.UpdateBookingAsync(updateBooking);
                return Ok(updatebookings);
            }
            catch (Exception ex)
            {
                // You can log the error
                _errorHandler.Capture(ex, "Error while update bookings");

                // Return proper response
                return StatusCode(500, new { message = "Something went wrong", error = ex.Message });
            }
        }


        // --------------------------------------------------------------------
        // ✅ DELETE BOOKING
        // --------------------------------------------------------------------
        /// <summary>
        /// Deletes (or marks as cancelled) a booking using its unique ID.
        /// </summary>
        /// <param name="bookingId">The unique booking identifier.</param>
        /// <returns>Status message indicating delete result.</returns>
        [HttpDelete]
        [Route("DeleteBus")]
        [Authorize]
        public async Task<IActionResult> DeleteBus(int Bookingid)
        {
            try
           {
                var deletbooking = await _bBSbooking.DeleteBookingAsync(Bookingid);
                return Ok(deletbooking); 
            }
            catch (Exception ex)
            {
                // You can log the error
                _errorHandler.Capture(ex, "Error While Delete bookings");

                // Return proper response
                return StatusCode(500, new { message = "Something went wrong", error = ex.Message });
            }
        }

        // --------------------------------------------------------------------
        // ✅ GET RECENT BOOKINGS
        // --------------------------------------------------------------------
        /// <summary>
        /// Retrieves bookings made on the current day or most recent transactions.
        /// </summary>
        /// <returns>Returns a list of recently made bookings.</returns>
        [HttpGet]
        [Route("Getrecentbookings")]
        [Authorize]
        public async Task<IActionResult> Getrecentbookings()
        {
            try
            {
                var recent = await _bBSbooking.GetRecentBookingsAsync();
                return Ok(recent); 
            }
            catch (Exception ex)
            {
                // You can log the error
                _errorHandler.Capture(ex, "Error fetching Recent bookings");

                // Return proper response
                return StatusCode(500, new { message = "Something went wrong", error = ex.Message });
            }
        }
    }
}
