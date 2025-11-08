// ==============================================================================
// Project Name : BusBookingSystem
// File Name    : BusController.cs
// Created By   : Kaviraj M
// Created On   : 19/09/2025
// Modified By  : Kaviraj M
// Modified On  : 05/11/2025
// Description  : Handles all bus-related operations in the Bus Booking System,
//                including adding, updating, deleting, and retrieving buses.
// ==============================================================================


using BusBookingSystem.Application;
using BusBookingSystem.Application.BusDtos;
using BusBookingSystem.Infrastructure.RepositoryImplementation;
using BusBookingSystem.Infrastructure.RepositoryInterface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BusBookingSystem.API.Controllers
{
    /// <summary>
    /// Provides endpoints for managing buses in the Bus Booking System.
    /// Supports operations to create, update, delete, and retrieve bus information.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class BusController : ControllerBase
    {
        private readonly IBBSBus _bBSBus;
        private readonly ErrorHandler _errorHandler;

        /// <summary>
        /// Initializes a new instance of the <see cref="BusController"/> class.
        /// </summary>
        /// <param name="bbsBus">The bus repository interface.</param>
        public BusController (IBBSBus bBSBus, ErrorHandler errorHandler)
        {
            _bBSBus = bBSBus;
            _errorHandler = errorHandler;
        }


        // --------------------------------------------------------------------
        // ✅ GET ALL BUSES
        // --------------------------------------------------------------------
        /// <summary>
        /// Retrieves all available buses from the system.
        /// </summary>
        /// <returns>Returns a list of all buses.</returns>
        [HttpGet]
        [Route("GetAllBuses")]
        [AllowAnonymous]

        public async Task<IActionResult> GetAllBuses()
        {
            try
            {
                return Ok(await _bBSBus.GetAllbuses());
            }
            catch (Exception ex)
            {
                // You can log the error
                _errorHandler.Capture(ex, "Error fetching all buses");

                // Return proper response
                return StatusCode(500, new { message = "Something went wrong", error = ex.Message });
            }

        }

        // --------------------------------------------------------------------
        // ✅ ADD BUS
        // --------------------------------------------------------------------
        /// <summary>
        /// Adds a new bus record to the system.
        /// </summary>
        /// <param name="addBuses">The DTO containing bus details.</param>
        /// <returns>Status message indicating success or failure.</returns>
        [HttpPost]
        [Route("AddBus")]
        [Authorize]
        public async Task<IActionResult> AddBus(RequestAddBuses addBuses)
        {
            try
            {
                var result = User.FindFirst("userid")?.Value;
                if (string.IsNullOrEmpty(result))
                    return Unauthorized(new { message = "Invalid or missing user identity" });
                addBuses.CreatedBy = Convert.ToInt16(result);
                return Ok(await _bBSBus.AddBuses(addBuses));

            }
            catch (Exception ex)
            {
                // You can log the error
                _errorHandler.Capture(ex, "Error while adding Bus ");

                // Return proper response
                return StatusCode(500, new { message = "Something went wrong", error = ex.Message });
            }

        }

        // --------------------------------------------------------------------
        // ✅ UPDATE BUS
        // --------------------------------------------------------------------
        /// <summary>
        /// Updates existing bus information such as name, type, or route assignment.
        /// </summary>
        /// <param name="updateBuses">The DTO containing updated bus information.</param>
        /// <returns>Status message indicating update result.</returns>
        [HttpPut]
        [Route("UpdateBus")]
        [Authorize]

        public async Task<IActionResult> UpdateBus(RequestUpdateBuses updateBuses)
        {
            try
            {
                var result = User.FindFirst("userid")?.Value;
                if (string.IsNullOrEmpty(result))
                    return Unauthorized(new { message = "Invalid or missing user identity" });
                updateBuses.ModifiedBy = Convert.ToInt16(result);
                return Ok (await _bBSBus.UpdateBuses(updateBuses));
            }
            catch (Exception ex)
            {
                // You can log the error
                _errorHandler.Capture(ex, "Error While update bus informations");

                // Return proper response
                return StatusCode(500, new { message = "Something went wrong", error = ex.Message });
            }

        }


        // --------------------------------------------------------------------
        // ✅ DELETE BUS
        // --------------------------------------------------------------------
        /// <summary>
        /// Marks a bus as deleted (soft delete) using its unique identifier.
        /// </summary>
        /// <param name="busId">The unique identifier of the bus.</param>
        /// <returns>Status message indicating deletion result.</returns>
        [HttpDelete]
        [Route("DeleteBus")]
        [Authorize]
        public async Task<IActionResult> DeleteBus( [FromQuery]int busId)
        {
            try
            { 
                return Ok(await _bBSBus.DeleteBuses(busId));

            }
            catch (Exception ex)
            {
                // You can log the error
                _errorHandler.Capture(ex, "Error While Delete bus ");

                // Return proper response
                return StatusCode(500, new { message = "Something went wrong", error = ex.Message });
            }
        }

        // --------------------------------------------------------------------
        // ✅ GET BUS COUNT
        // --------------------------------------------------------------------
        /// <summary>
        /// Retrieves the total number of buses in the system.
        /// </summary>
        /// <returns>Total count of buses.</returns>
        [HttpGet]
        [Route("GetBusescount")]
        [Authorize]
        public async Task<IActionResult> GetBusescount()
        {
            try
            {
                return Ok(await _bBSBus.GetBusesCount());

            }
            catch (Exception ex)
            {
                // You can log the error
                _errorHandler.Capture(ex, "Error fetching buses counts ");

                // Return proper response
                return StatusCode(500, new { message = "Something went wrong", error = ex.Message });
            }

        }



    }
}
