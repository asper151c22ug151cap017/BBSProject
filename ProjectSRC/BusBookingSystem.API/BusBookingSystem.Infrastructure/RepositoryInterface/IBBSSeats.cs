// =============================================================================================
// Project Name : BusBookingSystem
// File Name    : IBBSSeats.cs
// Created By   : Kaviraj M
// Created On   : 19/09/2025
// Modified By  : Kaviraj M
// Modified On  : 28/10/2025
// Description  :  Interface definition for seat-related operations in the Bus Booking System.
//                 Provides method contracts for adding, updating, deleting, and retrieving seats,
//                 including availability and bus-specific seat details.
// ===================================================================================================

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusBookingSystem.Application;
using BusBookingSystem.Application.RoutesDtos;
using BusBookingSystem.Application.SeatsDtos;

namespace BusBookingSystem.Infrastructure.RepositoryInterface
{
    /// <summary>
    /// Defines the contract for seat management operations.
    /// Handles CRUD actions and availability retrieval for bus seats.
    /// </summary>
    public interface IBBSSeats
    {
        /// <summary>
        /// Retrieves all seats with associated bus information.
        /// </summary>
        /// <returns>List of seats mapped to response DTOs.</returns>
       Task <List<ResponseGetSeats>> GetAllSeats();

        /// <summary>
        /// Adds a new seat record.
        /// </summary>
        /// <param name="addSeats">Seat details to add.</param>
        /// <returns>Status message.</returns>
        Task<string>  AddSeats(RequestAddSeats addSeats);

        /// <summary>
        /// Updates an existing seat record.
        /// </summary>
        /// <param name="updateSeats">Updated seat information.</param>
        /// <returns>Status message.</returns>
        Task<string> UpdateSeats(RequestUpdateSeats UpdateSeats);


        /// <summary>
        /// Performs a soft delete on a seat record.
        /// </summary>
        /// <param name="seatId">Unique ID of the seat to delete.</param>
        /// <returns>Status message.</returns>
        Task<string> DeleteSeats(int SeatsId);

        /// <summary>
        /// Retrieves seats for a specific bus along with user booking information.
        /// </summary>
        /// <param name="busId">Bus ID.</param>
        /// <returns>List of seats for the given bus.</returns>
        Task <List<ResponseGetseatsbybusid>> GetparthicularbusSeats(int Busid, DateTime travelDate);


        /// <summary>
        /// Retrieves available seats for a specific bus on a particular date.
        /// </summary>
        /// <param name="busId">Bus ID.</param>
        /// <param name="date">Journey date.</param>
        /// <returns>List of available seats with booking details.</returns>
        Task<List<ResponseAvailableseats>> GetAvailableseats(int Busid, DateTime? Date);
    }
}
