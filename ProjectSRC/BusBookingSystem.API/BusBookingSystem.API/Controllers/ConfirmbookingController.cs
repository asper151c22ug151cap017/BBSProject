// ============================================================================
// Project Name : BusBookingSystem
// File Name    : ConfirmBookingController.cs
// Created By   : Kaviraj M
// Created On   : 20/09/2025
// Modified By  : Kaviraj M
// Modified On  : 28/10/2025
// Description  : Handles all booking confirmation operations including
//                adding bookings, passengers, retrieving booked data,
//                downloading tickets, and canceling bookings.
// ============================================================================

using BusBookingSystem.Application.BookingDtos;
using BusBookingSystem.Infrastructure.RepositoryInterface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BusBookingSystem.API.Controllers
{
    /// <summary>
    /// Provides endpoints to manage confirmed bookings, including passenger details,
    /// booking creation, ticket downloads, and cancellations.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class ConfirmbookingController : ControllerBase
    {
        private readonly Iconfirmbooking _confirmBooking;

        /// <summary>
        /// Initializes a new instance of the <see cref="ConfirmBookingController"/> class.
        /// </summary>
        /// <param name="confirmBooking">The repository interface for confirm booking operations.</param>
        public ConfirmbookingController(Iconfirmbooking confirmBooking)
        {
            _confirmBooking = confirmBooking;
        }

        // --------------------------------------------------------------------
        // ✅ GET BUS BY ID
        // --------------------------------------------------------------------
        /// <summary>
        /// Retrieves bus details along with associated route information for a given bus ID.
        /// </summary>
        /// <param name="busId">Unique identifier of the bus.</param>
        /// <returns>Bus details with route data.</returns>
        [HttpGet]
        [Route("GetBusesById")]
        [AllowAnonymous]
        public async Task<IActionResult> GetBusesById([FromQuery] int busId)
        {
            var buses = await _confirmBooking.GetBus(busId); // ✅ await added
            return Ok(buses); // ✅ now it returns actual data, not Task
        }

        // --------------------------------------------------------------------
        // ✅ ADD PASSENGER
        // --------------------------------------------------------------------
        /// <summary>
        /// Adds a single passenger to an existing booking.
        /// </summary>
        /// <param name="addpassengers">Passenger information DTO.</param>
        /// <returns>Status message indicating operation result.</returns>
        [HttpPost]
        [Route("AddPassenger")]
        [Authorize]
        public IActionResult AddPassenger([FromBody] Addpassengers addpassengers)
        {
            return Ok(_confirmBooking.Addpassaengers(addpassengers));
        }

        // --------------------------------------------------------------------
        // ✅ GET BOOKINGS BY USER ID
        // --------------------------------------------------------------------
        /// <summary>
        /// Retrieves all bookings made by a specific user.
        /// </summary>
        /// <param name="userId">Unique identifier of the user.</param>
        /// <returns>List of bookings made by the user.</returns>
        [HttpGet]
        [Route("GetBookingsByUserId")]
        [Authorize]
        public IActionResult GetBookingsByUserId([FromQuery] int userId)
        {
            return Ok(_confirmBooking.GetBookingByid(userId));
        }

        // --------------------------------------------------------------------
        // ✅ ADD BOOKING
        // --------------------------------------------------------------------
        /// <summary>
        /// Adds a new booking along with passengers and selected seat details.
        /// </summary>
        /// <param name="addbookings">Booking request DTO containing all booking details.</param>
        /// <returns>Response DTO containing booking ID, message, and seat details.</returns>
        [HttpPost]
        [Route("AddBooking")]
        [Authorize]
        public IActionResult AddBooking([FromBody] Requestbookingdto addbookings)
        {
            return Ok(_confirmBooking.Addbooking(addbookings));
        }

        // --------------------------------------------------------------------
        // ✅ DOWNLOAD TICKET
        // --------------------------------------------------------------------
        /// <summary>
        /// Downloads ticket information for a specific booking ID.
        /// </summary>
        /// <param name="bookingId">Unique identifier of the booking.</param>
        /// <returns>Ticket information including seat numbers and passenger details.</returns>
        [HttpGet]
        [Route("DownloadTicket")]
        [Authorize]
        public IActionResult DownloadTicket([FromQuery] int bookingId)
        {
            return Ok(_confirmBooking.DownloadTickets(bookingId));
        }

        // --------------------------------------------------------------------
        // ✅ CANCEL BOOKING
        // --------------------------------------------------------------------
        /// <summary>
        /// Cancels an existing booking and releases all booked seats.
        /// </summary>
        /// <param name="bookingId">Unique identifier of the booking.</param>
        /// <returns>Status message indicating cancellation result.</returns>
        [HttpPost]
        [Route("CancelBooking")]
        [Authorize]
        public IActionResult CancelBooking([FromQuery] int bookingId)
        {
            return Ok(_confirmBooking.Cancellbooking(bookingId));
        }
    }
}