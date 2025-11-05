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

using BusBookingSystem.Application;
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
        private readonly ErrorHandler _errorHandler;


        // --------------------------------------------------------------------
        // 🔹 Constructor
        // --------------------------------------------------------------------
        /// <summary>
        /// Initializes a new instance of the <see cref="SeatsController"/> class.
        /// </summary>
        /// <param name="bBsSeats">Repository interface for seat management.</param>
        public SeatsController(IBBSSeats bBsSeats, ErrorHandler errorHandler)
        {
            _bBsSeats = bBsSeats;
            _errorHandler = errorHandler;
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
        [Authorize]
        public async Task<IActionResult> GetAllSeats()
        {
            try
            {
                return Ok(await _bBsSeats.GetAllSeats());
            }
            catch (Exception ex)
            {
                // You can log the error
                _errorHandler.Capture(ex, "Error fetching all seats");

                // Return proper response
                return StatusCode(500, new { message = "Something went wrong", error = ex.Message });
            }
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
        public async Task<IActionResult> GetParticularBusSeats([FromQuery] int busId, DateTime travelDate)
        {
            try
            {
                return Ok(await _bBsSeats.GetparthicularbusSeats(busId, travelDate));
            }
            catch (Exception ex)
            {
                // You can log the error
                _errorHandler.Capture(ex, $"Error fetching particular bus seats by:{busId}&& {travelDate} ");

                // Return proper response
                return StatusCode(500, new { message = "Something went wrong", error = ex.Message });
            }
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
        public async Task<IActionResult> AddSeat([FromBody] RequestAddSeats addSeats)
        {
            try
            {
                if (addSeats.SeatNumber.Length > 10)
                {
                    return BadRequest(new { Message = "Seat number cannot exceed 10 characters." });
                }

                return Ok(await _bBsSeats.AddSeats(addSeats));
            }
            catch (Exception ex)
            {
                // You can log the error
                _errorHandler.Capture(ex, "Error While add seats ");

                // Return proper response
                return StatusCode(500, new { message = "Something went wrong", error = ex.Message });
            }
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
        public async Task<IActionResult> UpdateSeat([FromBody] RequestUpdateSeats updateSeatsInfo)
        {
            try
            {
                return Ok(await _bBsSeats.UpdateSeats(updateSeatsInfo));
            }
            catch (Exception ex)
            {
                // You can log the error
                _errorHandler.Capture(ex, "Error While update seats");

                // Return proper response
                return StatusCode(500, new { message = "Something went wrong", error = ex.Message });
            }
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
        public async Task<IActionResult> DeleteSeat([FromQuery] int seatId)
        {
            try
            {
                return Ok(await _bBsSeats.DeleteSeats(seatId));
            }
            catch (Exception ex)
            {
                // You can log the error
                _errorHandler.Capture(ex, "Error While Delete seats");

                // Return proper response
                return StatusCode(500, new { message = "Something went wrong", error = ex.Message });
            }
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
        public async Task<IActionResult> GetAvailableSeats([FromQuery] int busId, [FromQuery] DateTime? date)
        {
            try
            {
                if (date == null)
                    return BadRequest(new { Message = "Date is required to check seat availability." });

                return Ok(await _bBsSeats.GetAvailableseats(busId, date.Value));
            }
            catch (Exception ex)
            {
                // You can log the error
                _errorHandler.Capture(ex, "Error fetching available seats ");

                // Return proper response
                return StatusCode(500, new { message = "Something went wrong", error = ex.Message });
            }
        }
    }
}
