// ============================================================================
// Project Name : BusBookingSystem
// File Name    : IBBSBus.cs
// Created By   : Kaviraj M
// Created On   : 19/09/2025
// Modified By  : Kaviraj M
// Modified On  : 28/10/2025
// Description  : Defines the repository interface for managing bus-related 
//                operations in the Bus Booking System, including adding, 
//                updating, deleting, and retrieving bus details.
// ============================================================================

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusBookingSystem.Application.BusDtos;

namespace BusBookingSystem.Infrastructure.RepositoryInterface
{

    /// <summary>
    /// Provides a contract for performing CRUD operations on bus entities.
    /// Defines methods to add, update, delete, and retrieve bus details 
    /// within the Bus Booking System.
    /// </summary>
    public interface IBBSBus
    {

        // --------------------------------------------------------------------
        // ✅ GET ALL BUSES
        // --------------------------------------------------------------------
        /// <summary>
        /// Retrieves all bus records with their details such as type, 
        /// capacity, route, and operator information.
        /// </summary>
        /// <returns>
        /// A list of <see cref="ResponseGetbuses"/> representing all buses.
        /// </returns>

        List<ResponseGetbuses> GetAllbuses();

        // --------------------------------------------------------------------
        // ✅ ADD BUS
        // --------------------------------------------------------------------
        /// <summary>
        /// Adds a new bus record to the system.
        /// </summary>
        /// <param name="addBuses">
        /// The <see cref="RequestAddBuses"/> object containing the details 
        /// of the bus to be added.
        /// </param>
        /// <returns>
        /// A status message indicating whether the bus was added successfully.
        /// </returns>
        string AddBuses(RequestAddBuses addBuses);

        // --------------------------------------------------------------------
        // ✅ UPDATE BUS
        // --------------------------------------------------------------------
        /// <summary>
        /// Updates details of an existing bus record.
        /// </summary>
        /// <param name="updateBusesinfo">
        /// The <see cref="RequestUpdateBuses"/> object containing updated bus information.
        /// </param>
        /// <returns>
        /// A message indicating the result of the update operation.
        /// </returns>
        string UpdateBuses (RequestUpdateBuses updateBusesinfo);

        // --------------------------------------------------------------------
        // ✅ DELETE BUS
        // --------------------------------------------------------------------
        /// <summary>
        /// Deletes a bus record based on its unique bus ID.
        /// </summary>
        /// <param name="busesId">Unique identifier of the bus.</param>
        /// <returns>
        /// A status message indicating the result of the delete operation.
        /// </returns>
        string DeleteBuses (int  busesId);


        // --------------------------------------------------------------------
        // ✅ GET BUS COUNT
        // --------------------------------------------------------------------
        /// <summary>
        /// Retrieves the total number of buses available in the system.
        /// </summary>
        /// <returns>
        /// The total count of buses as an integer.
        /// </returns>
        int GetBusesCount();
    }
}
