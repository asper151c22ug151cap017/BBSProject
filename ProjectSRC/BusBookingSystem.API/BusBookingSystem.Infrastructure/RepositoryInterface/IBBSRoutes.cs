// ============================================================================
// Project Name : BusBookingSystem
// File Name    : IBBSRoutes.cs
// Created By   : Kaviraj M
// Created On   : 20/09/2025
// Modified By  : Kaviraj M
// Modified On  : 28/10/2025
// Description  : Defines the repository interface for managing bus route 
//                operations, including adding, updating, deleting, and 
//                filtering routes in the Bus Booking System.
// ============================================================================

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusBookingSystem.Application.BusDtos;
using BusBookingSystem.Application.RoutesDtos;

namespace BusBookingSystem.Infrastructure.RepositoryInterface
{
    /// <summary>
    /// Provides a contract for CRUD operations and route management logic 
    /// in the Bus Booking System.
    /// </summary>
    public interface IBBSRoutes
    {

        // --------------------------------------------------------------------
        // ✅ GET ALL ROUTES
        // --------------------------------------------------------------------
        /// <summary>
        /// Retrieves all routes available in the system with their source 
        /// and destination details.
        /// </summary>
        /// <returns>
        /// A list of <see cref="ResponseRoutes"/> representing all routes.
        /// </returns>
       Task< List<ResponseRoutes>> GetAllRoutes();

        // --------------------------------------------------------------------
        // ✅ ADD ROUTE
        // --------------------------------------------------------------------
        /// <summary>
        /// Adds a new route record to the system.
        /// </summary>
        /// <param name="addRoutes">
        /// The <see cref="RequestAddRoutes"/> object containing details of the new route.
        /// </param>
        /// <returns>
        /// A status message indicating the result of the add operation.
        /// </returns>
       Task <string> AddRoutes(RequestAddRoutes addRoutes);


        // --------------------------------------------------------------------
        // ✅ UPDATE ROUTE
        // --------------------------------------------------------------------
        /// <summary>
        /// Updates an existing route with new source, destination, or timing details.
        /// </summary>
        /// <param name="updateRoutesinfo">
        /// The <see cref="RequestUpdateRoutes"/> object containing updated route information.
        /// </param>
        /// <returns>
        /// A message indicating whether the update was successful.
        /// </returns>
        Task<string> UpdateRoutes( RequestUpdateRoutes updateRoutesinfo);

        // --------------------------------------------------------------------
        // ✅ DELETE ROUTE
        // --------------------------------------------------------------------
        /// <summary>
        /// Deletes a route based on its unique Route ID (soft delete or permanent).
        /// </summary>
        /// <param name="RouteId">The unique identifier of the route.</param>
        /// <returns>
        /// A message indicating the result of the delete operation.
        /// </returns>
        Task<string> DeleteRoutes(int RouteId);

        // --------------------------------------------------------------------
        // ✅ FILTER ROUTES
        // --------------------------------------------------------------------
        /// <summary>
        /// Filters and retrieves routes based on the provided source , destination and Traveldate.
        /// </summary>
        /// <param name="Source">Starting location of the route.</param>
        /// <param name="Destination">Ending location of the route.</param>
        /// <param name="Traveldate"> Travel Date of the route.</param>
        /// <returns>
        /// A filtered list of <see cref="ResponseRoutes"/> matching the criteria.
        /// </returns>
        Task<List<ResponseRoutes>> FilterRoutes(string Source, string Destination, DateTime TravelDate);

        // --------------------------------------------------------------------
        // ✅ GET ROUTE COUNT
        // --------------------------------------------------------------------
        /// <summary>
        /// Retrieves the total number of routes available in the system.
        /// </summary>
        /// <returns>
        /// The total count of routes as an integer.
        /// </returns>
        Task< int> GetRoutesCount();
    }
}
