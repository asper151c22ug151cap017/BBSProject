// ============================================================================
// Project Name : BusBookingSystem
// File Name    : IConfirmBooking.cs
// Created By   : Kaviraj M
// Created On   : 20/09/2025
// Modified By  : Kaviraj M
// Modified On  : 28/10/2025
// Description  : Defines the repository interface for managing booking and 
//                passenger operations in the Bus Booking System, including 
//                adding bookings, retrieving details, and downloading tickets.
// ============================================================================

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusBookingSystem.Application.BookingDtos;


namespace BusBookingSystem.Infrastructure.RepositoryInterface
{

    /// <summary>
    /// Provides a contract for all booking confirmation and passenger-related 
    /// operations in the Bus Booking System.
    /// </summary>
    public interface Iconfirmbooking
    {


        // --------------------------------------------------------------------
        // ✅ GET BUS BY ID
        // --------------------------------------------------------------------
        /// <summary>
        /// Retrieves bus details based on a unique bus ID.
        /// </summary>
        /// <param name="busId">The unique identifier of the bus.</param>
        /// <returns>
        /// A list of <see cref="BusDto"/> objects containing bus information.
        /// </returns>
       Task< List<BusDto>> GetBus(int busId);

        // --------------------------------------------------------------------
        // ✅ ADD PASSENGERS
        // --------------------------------------------------------------------
        /// <summary>
        /// Adds passenger information associated with a particular booking.
        /// </summary>
        /// <param name="addpassengers">
        /// The <see cref="Addpassengers"/> object containing passenger details.
        /// </param>
        /// <returns>
        /// A message indicating whether passenger data was successfully added.
        /// </returns>
        string Addpassaengers(Addpassengers addpassengers);


        // --------------------------------------------------------------------
        // ✅ GET BOOKINGS BY USER ID
        // --------------------------------------------------------------------
        /// <summary>
        /// Retrieves all booking details associated with a specific user.
        /// </summary>
        /// <param name="userId">The unique identifier of the user.</param>
        /// <returns>
        /// A list of <see cref="BookingResponse"/> containing booking information.
        /// </returns>
        List<BookingResponse> GetBookingByid(int userId);

        // --------------------------------------------------------------------
        // ✅ ADD BOOKING
        // --------------------------------------------------------------------
        /// <summary>
        /// Adds a new booking for a user, including seat and payment details.
        /// </summary>
        /// <param name="addbookings">
        /// The <see cref="Requestbookingdto"/> object containing booking information.
        /// </param>
        /// <returns>
        /// A <see cref="ResponsebookingDto"/> object containing booking confirmation details.
        /// </returns>
        ResponsebookingDto Addbooking(Requestbookingdto addbookings);


        // --------------------------------------------------------------------
        // ✅ CANCEL BOOKING
        // --------------------------------------------------------------------
        /// <summary>
        /// Cancels an existing booking by its unique booking ID.
        /// </summary>
        /// <param name="Bookingid">The unique identifier of the booking to cancel.</param>
        /// <returns>
        /// A message indicating the result of the cancellation process.
        /// </returns>
        string Cancellbooking(int Bookingid);

        // --------------------------------------------------------------------
        // ✅ DOWNLOAD TICKETS
        // --------------------------------------------------------------------
        /// <summary>
        /// Generates and retrieves ticket information for a specific booking.
        /// </summary>
        /// <param name="Bookingid">The booking ID for which tickets are downloaded.</param>
        /// <returns>
        /// A list of <see cref="ResponseDownloadtickets"/> objects representing the ticket details.
        /// </returns>
        List<ResponseDownloadtickets> DownloadTickets(int Bookingid);

    }
}
