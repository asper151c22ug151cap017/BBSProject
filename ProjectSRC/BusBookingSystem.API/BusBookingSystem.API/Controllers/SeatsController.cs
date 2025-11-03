// ============================================================================
// Project Name : BusBookingSystem
// File Name    : SeatsController.cs
// Created By   : [Your Name]
// Created On   : [Date]
// Modified By  : [Your Name]
// Modified On  : [Date]
// Description  : Handles all seat-related operations including CRUD actions,
//                retrieving seat availability, and fetching bus-specific seats.
// ============================================================================

using BusBookingSystem.Application.SeatsDtos;
using BusBookingSystem.Infrastructure.RepositoryInterface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BusBookingSystem.API.Controllers
{
    /// <summary>
    /// Provides endpoints for managing bus seat operations including adding,
    /// updating, deleting, and retrieving seat details.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class SeatsController : ControllerBase
    {
        // --------------------------------------------------------------------
        // 🔹 Private Field
        // --------------------------------------------------------------------
        private readonly IBBSSeats _bBsSeats;

        // --------------------------------------------------------------------
        // 🔹 Constructor
        // --------------------------------------------------------------------
        /// <summary>
        /// Initializes a new instance of the <see cref="SeatsController"/> class.
        /// </summary>
        /// <param name="bBsSeats">Repository interface for seat management.</param>
        public SeatsController(IBBSSeats bBsSeats)
        {
            _bBsSeats = bBsSeats;
        }

        // --------------------------------------------------------------------
        // ✅ GET ALL SEATS
        // --------------------------------------------------------------------
        /// <summary>
        /// Retrieves all seats available across all buses.
        /// </summary>
        /// <returns>List of all seat records.</returns>
        [HttpGet]
        [Route("GetAllSeats")]
        [AllowAnonymous]
        public IActionResult GetAllSeats()
        {
            return Ok(_bBsSeats.GetAllSeats());
        }

        // --------------------------------------------------------------------
        // ✅ GET SEATS BY BUS ID
        // --------------------------------------------------------------------
        /// <summary>
        /// Retrieves all seats for a specific bus by its unique ID.
        /// </summary>
        /// <param name="busId">Unique identifier of the bus.</param>
        /// <returns>List of seats belonging to the specified bus.</returns>
        [HttpGet]
        [Route("GetParticularBusSeats")]
        [AllowAnonymous]
        public IActionResult GetParticularBusSeats([FromQuery] int busId, DateTime travelDate)
        {
            return Ok(_bBsSeats.GetparthicularbusSeats(busId, travelDate));
        }

        // --------------------------------------------------------------------
        // ✅ ADD SEAT
        // --------------------------------------------------------------------
        /// <summary>
        /// Adds a new seat record to the system.
        /// </summary>
        /// <param name="addSeats">Seat data to be added.</param>
        /// <returns>Result message indicating success or failure.</returns>
        [HttpPost]
        [Route("AddSeat")]
        [Authorize]
        public IActionResult AddSeat([FromBody] RequestAddSeats addSeats)
        {
            if (addSeats.SeatNumber.Length > 10)
            {
                return BadRequest(new { Message = "Seat number cannot exceed 10 characters." });
            }

            return Ok(_bBsSeats.AddSeats(addSeats));
        }

        // --------------------------------------------------------------------
        // ✅ UPDATE SEAT
        // --------------------------------------------------------------------
        /// <summary>
        /// Updates an existing seat record.
        /// </summary>
        /// <param name="updateSeatsInfo">Seat details to update.</param>
        /// <returns>Result message indicating success or failure.</returns>
        [HttpPut]
        [Route("UpdateSeat")]
        [Authorize]
        public IActionResult UpdateSeat([FromBody] RequestUpdateSeats updateSeatsInfo)
        {
            return Ok(_bBsSeats.UpdateSeats(updateSeatsInfo));
        }

        // --------------------------------------------------------------------
        // ✅ DELETE SEAT
        // --------------------------------------------------------------------
        /// <summary>
        /// Deletes a seat from the system using its unique ID.
        /// </summary>
        /// <param name="seatId">Unique identifier of the seat.</param>
        /// <returns>Result message indicating success or failure.</returns>
        [HttpDelete]
        [Route("DeleteSeat")]
        [Authorize]
        public IActionResult DeleteSeat([FromQuery] int seatId)
        {
            return Ok(_bBsSeats.DeleteSeats(seatId));
        }

        // --------------------------------------------------------------------
        // ✅ GET AVAILABLE SEATS
        // --------------------------------------------------------------------
        /// <summary>
        /// Retrieves all available seats for a specific bus on a given date.
        /// </summary>
        /// <param name="busId">Unique identifier of the bus.</param>
        /// <param name="date">Journey date to check availability.</param>
        /// <returns>List of available seats for the specified date and bus.</returns>
        [HttpGet]
        [Route("GetAvailableSeats")]
        [AllowAnonymous]
        public IActionResult GetAvailableSeats([FromQuery] int busId, [FromQuery] DateTime? date)
        {
            if (date == null)
                return BadRequest(new { Message = "Date is required to check seat availability." });

            return Ok(_bBsSeats.GetAvailableseats(busId, date.Value));
        }
    }
}
