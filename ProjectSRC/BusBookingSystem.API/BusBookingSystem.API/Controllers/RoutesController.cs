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

        /// <summary>
        /// Initializes a new instance of the <see cref="RoutesController"/> class.
        /// </summary>
        /// <param name="bBSRoutes">Repository interface for route management.</param>
        public RoutesController(IBBSRoutes bBSRoutes)
        {
            _bBSRoutes = bBSRoutes;
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
        public IActionResult GetAllRoutes()
        {
            return Ok(_bBSRoutes.GetAllRoutes());
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
        public IActionResult GetRoutesCount()
        {
            return Ok(_bBSRoutes.GetRoutesCount());
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
        public IActionResult AddRoute([FromBody] RequestAddRoutes addRoutes)
        {
            return Ok(_bBSRoutes.AddRoutes(addRoutes));
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
        public IActionResult UpdateRoute([FromBody] RequestUpdateRoutes updateRoutesInfo)
        {
            return Ok(_bBSRoutes.UpdateRoutes(updateRoutesInfo));
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
        public IActionResult DeleteRoute([FromQuery] int routeId)
        {
            return Ok(_bBSRoutes.DeleteRoutes(routeId));
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
        public IActionResult FilterRoutes([FromQuery] string source, [FromQuery] string destination, [FromQuery] DateTime travelDate)
        {
            if (string.IsNullOrEmpty(source) || string.IsNullOrEmpty(destination) || travelDate == default(DateTime))
                return BadRequest(new { Message = "Source, Destination, and valid TravelDate are required." });

            var routes = _bBSRoutes.FilterRoutes(source, destination, travelDate);
            return Ok(routes);
        }
    }
}