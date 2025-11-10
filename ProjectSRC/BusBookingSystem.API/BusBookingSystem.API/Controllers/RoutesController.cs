// ============================================================================
// Project Name : BusBookingSystem
// File Name    : RoutesController.cs
// Created By   : Kaviraj M
// Created On   : 20/09/2025
// Modified By  : Kaviraj M
// Modified On  : 28/10/2025
// Description  : Handles all route management operations including
//                adding, updating, deleting, retrieving, and filtering routes.
// ============================================================================

using BusBookingSystem.Application;
using BusBookingSystem.Application.RoutesDtos;
using BusBookingSystem.Infrastructure.RepositoryInterface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BusBookingSystem.API.Controllers
{
    /// <summary>
    /// Provides endpoints for managing bus routes including CRUD operations
    /// and route filtering by source and destination.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class RoutesController : ControllerBase
    {
        private readonly IBBSRoutes _bBSRoutes;
        private readonly ErrorHandler _errorHandler;


        /// <summary>
        /// Initializes a new instance of the <see cref="RoutesController"/> class.
        /// </summary>
        /// <param name="bBSRoutes">Repository interface for route management.</param>
        public RoutesController(IBBSRoutes bBSRoutes, ErrorHandler errorHandler)
        {
            _bBSRoutes = bBSRoutes;
            _errorHandler = errorHandler;
        }

        // --------------------------------------------------------------------
        // ✅ GET ALL ROUTES
        // --------------------------------------------------------------------
        /// <summary>
        /// Retrieves all available bus routes.
        /// </summary>
        /// <returns>List of all routes in the system.</returns>
        [HttpGet]
        [Route("GetAllRoutes")]
        [AllowAnonymous]
        public async Task <IActionResult> GetAllRoutes()
        {
            try
            {
                return Ok(await _bBSRoutes.GetAllRoutes());
            }
            catch (Exception ex)
            {
                // You can log the error
                _errorHandler.Capture(ex, Messages.RoutesMessage.ExceptiongetRoutes);

                // Return proper response
                return StatusCode(500, new { message = Messages.User.Controller, error = ex.Message });
            }
        }

        // --------------------------------------------------------------------
        // ✅ GET ROUTE COUNT
        // --------------------------------------------------------------------
        /// <summary>
        /// Retrieves the total number of available routes.
        /// </summary>
        /// <returns>Count of all routes.</returns>
        [HttpGet]
        [Route("GetRoutesCount")]
        [Authorize]
        public async Task<IActionResult> GetRoutesCount()
        {
            try
            {
                return Ok(await _bBSRoutes.GetRoutesCount());
            }
            catch (Exception ex)
            {
                // You can log the error
                _errorHandler.Capture(ex, Messages.RoutesMessage.ExceptiongetRoutecount);

                // Return proper response
                return StatusCode(500, new { message = Messages.User.Controller, error = ex.Message });
            }
        }

        // --------------------------------------------------------------------
        // ✅ ADD ROUTE
        // --------------------------------------------------------------------
        /// <summary>
        /// Adds a new route to the system.
        /// </summary>
        /// <param name="addRoutes">DTO containing route details to add.</param>
        /// <returns>Result message of the add operation.</returns>
        [HttpPost]
        [Route("AddRoute")]
        [Authorize]
        public async Task<IActionResult> AddRoute([FromBody] RequestAddRoutes addRoutes)
        {
            try
            {
                return Ok(await _bBSRoutes.AddRoutes(addRoutes));
            }
            catch (Exception ex)
            {
                // You can log the error
                _errorHandler.Capture(ex,Messages.RoutesMessage.ExceptionaddRoutes);

                // Return proper response
                return StatusCode(500, new { message = Messages.User.Controller, error = ex.Message });
            }
        }

        // --------------------------------------------------------------------
        // ✅ UPDATE ROUTE
        // --------------------------------------------------------------------
        /// <summary>
        /// Updates an existing route's details.
        /// </summary>
        /// <param name="updateRoutesInfo">DTO containing updated route information.</param>
        /// <returns>Result message of the update operation.</returns>
        [HttpPut]
        [Route("UpdateRoute")]
        [Authorize]
        public async Task<IActionResult> UpdateRoute([FromBody] RequestUpdateRoutes updateRoutesInfo)
        {
            try
            {
                return Ok(await _bBSRoutes.UpdateRoutes(updateRoutesInfo));
            }
            catch (Exception ex)
            {
                // You can log the error
                _errorHandler.Capture(ex, Messages.RoutesMessage.ExceptionUpdateroutes);

                // Return proper response
                return StatusCode(500, new { message = Messages.User.Controller, error = ex.Message });
            }
        }

        // --------------------------------------------------------------------
        // ✅ DELETE ROUTE
        // --------------------------------------------------------------------
        /// <summary>
        /// Deletes an existing route by its unique ID.
        /// </summary>
        /// <param name="routeId">Unique identifier of the route.</param>
        /// <returns>Result message of the delete operation.</returns>
        [HttpDelete]
        [Route("DeleteRoute")]
        [Authorize]
        public async Task<IActionResult> DeleteRoute([FromQuery] int routeId)
        {
            try
            {
                return Ok(await _bBSRoutes.DeleteRoutes(routeId));
            }
            catch (Exception ex)
            {
                // You can log the error
                _errorHandler.Capture(ex, Messages.RoutesMessage.Exceptiondeleteroutes);

                // Return proper response
                return StatusCode(500, new { message = Messages.User.Controller, error = ex.Message });
            }   
        }

        // --------------------------------------------------------------------
        // ✅ FILTER ROUTES
        // --------------------------------------------------------------------
        /// <summary>
        /// Filters routes based on source and destination inputs.
        /// </summary>
        /// <param name="source">Starting point of the journey.</param>
        /// <param name="destination">Ending point of the journey.</param>
        /// <returns>List of routes matching the given criteria.</returns>
        [HttpGet]
        [Route("FilterRoutes")]
        [AllowAnonymous]
        public async Task<IActionResult> FilterRoutes([FromQuery] string source, [FromQuery] string destination, [FromQuery] DateTime travelDate)
        {
            try
            {
                if (string.IsNullOrEmpty(source) || string.IsNullOrEmpty(destination) || travelDate == default(DateTime))
                    return BadRequest(new { Message = Messages.RoutesMessage.Required });

                var routes = await _bBSRoutes.FilterRoutes(source, destination, travelDate);
                return Ok(routes);
            }
            catch (Exception ex)
            {
                // You can log the error
                _errorHandler.Capture(ex, Messages.RoutesMessage.ExceptionFilterroutes);

                // Return proper response
                return StatusCode(500, new { message = Messages.User.Controller, error = ex.Message });
            }
        }
    }
}