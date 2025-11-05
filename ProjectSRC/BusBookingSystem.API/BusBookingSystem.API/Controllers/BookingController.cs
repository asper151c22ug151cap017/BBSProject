// ================================================================================
// Project Name : BusBookingSystem
// File Name    : BookingController.cs
// Created By   : Kaviraj M
// Created On   : 20/09/2025
// Modified By  : Kaviraj M
// Modified On  : 28/10/2025
// Description  : Handles all booking-related operations in the Bus Booking System,
//                including creating, updating, deleting, and retrieving bookings.
// =================================================================================

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

        /// <summary>
        /// Initializes a new instance of the <see cref="BookingController"/> class
        /// with dependency injection for the booking repository.
        /// </summary>
        public BookingController(IBBSbooking bBSbooking)
        {
            _bBSbooking = bBSbooking;
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

        public async Task<IActionResult> Getallbookings()
        {
           var getbookings = await _bBSbooking.GetAllBookingsAsync();
            return Ok(getbookings);
        }

        // --------------------------------------------------------------------
        // ✅ GET BOOKING COUNT
        // --------------------------------------------------------------------
        /// <summary>
        /// Retrieves the total number of bookings in the system.
        /// </summary>
        [HttpGet]
        [Route ("Getbokingcounts")]
        [AllowAnonymous]

        public async Task< IActionResult> Getbookingcount()
        {
          var bookingcount = await _bBSbooking.GetCountBookingsAsync();
            return Ok(bookingcount);

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
            var addbooking = await _bBSbooking.AddBookingAsync(addBooking);
            return Ok(addbooking);
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
            var updatebookings = await _bBSbooking.UpdateBookingAsync(updateBooking);
            return Ok(updatebookings);
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
            var deletbooking = await _bBSbooking.DeleteBookingAsync(Bookingid);
            return Ok(deletbooking);
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

        public async Task<IActionResult> Getrecentbookings()
        {
            var bookingcount = await _bBSbooking.GetCountBookingsAsync();
            return Ok(bookingcount);
        }
    }
}
