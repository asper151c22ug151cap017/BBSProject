
// ===========================================================================================================
// Project Name : BusBookingSystem
// File Name    : BBSseats.cs
// Created By   : Kaviraj M
// Created On   : 19/9/2025
// Modified By  : Kaviraj M
// Modified On  : 28/10/2025
// Description  : Repository implementation for managing seat-related operations in the Bus Booking System.
//                Handles seat creation, updates, soft deletion, retrieval, and seat availability checking.
//                Includes proper error handling, Entity Framework usage, and DTO-based responses.
// ===========================================================================================================
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusBookingSystem.Application;
using BusBookingSystem.Application.RoutesDtos;
using BusBookingSystem.Application.SeatsDtos;
using BusBookingSystem.Domain.Models;
using BusBookingSystem.Infrastructure.RepositoryInterface;
using Microsoft.EntityFrameworkCore;

namespace BusBookingSystem.Infrastructure.RepositoryImplementation
{

    /// <summary>
    /// Repository implementation for managing seat-related operations.
    /// Handles seat creation, updates, deletion, and availability checks.
    /// </summary>
    public class BBSseats : IBBSSeats
    {
        private readonly DbBbsContext _dbBbsContext;
        private readonly ErrorHandler _errorHandler;

        /// <summary>
        /// Initializes a new instance of the <see cref="BBSseats"/> class.
        /// </summary>
        public BBSseats(DbBbsContext dbBbsContext, ErrorHandler errorHandler)
        {
            _dbBbsContext = dbBbsContext;
            _errorHandler = errorHandler;
        }

        // --------------------------------------------------------------------
        // ✅ ADD SEAT
        // --------------------------------------------------------------------
        /// <summary>
        /// Adds a new seat to the system.
        /// </summary>
        /// <param name="addSeats">Seat details to be added.</param>
        /// <returns>Status message.</returns>
        public string AddSeats(RequestAddSeats addSeats)
        {
            try
            {
                var now = DateTime.Now;
                if (addSeats == null)
                    return "Invalid Input";

                var Newseats = new Tblseat
                {
                    SeatNumber = addSeats.SeatNumber,
                
                    CreatedAt = now,
                    ModifiedAt = now
                };

                _dbBbsContext.Tblseats.Add(Newseats);
                _dbBbsContext.SaveChanges();

                //NewRoutes.CreatedBy = addRoutes.Busid;


                return "Added Successfully";
            }
            catch (Exception ex)
            {
                _errorHandler.Capture(ex, "An Error while Add Buses");
                throw new Exception(ex.Message);
            }
        }

        // --------------------------------------------------------------------
        // ✅ DELETE SEAT (Soft Delete)
        // --------------------------------------------------------------------
        /// <summary>
        /// Soft deletes a seat by updating IsActive and IsDelete flags.
        /// </summary>
        /// <param name="seatId">Seat ID to be deleted.</param>
        /// <returns>Status message.</returns>
        public string DeleteSeats(int SeatsId)
        {
            try
            {
                var deleteseats = _dbBbsContext.Tblseats.FirstOrDefault(d => d.SeatId == SeatsId);
                if (deleteseats != null)
                {
                    deleteseats.IsActive = false;
                    deleteseats.IsDelete = true;
                    _dbBbsContext.Tblseats.Update(deleteseats);

                    _dbBbsContext.SaveChanges();
                    return "Deleted Successfully";
                }
                else
                {
                    return "Seats Not Found";
                }
            }
            catch (Exception ex)
            {
                _errorHandler.Capture(ex, "An Error while Add Buses");
                throw new Exception(ex.Message);
            }
        }

        // --------------------------------------------------------------------
        // ✅ GET ALL SEATS
        // --------------------------------------------------------------------
        /// <summary>
        /// Retrieves all seats including their associated bus details.
        /// </summary>
        /// <returns>List of seats mapped to response DTOs.</returns>
        public List<ResponseGetSeats> GetAllSeats()
        {
            try
            {
                return _dbBbsContext.Tblseats.Include(x=>x.Bus)
                    .Select(x => new ResponseGetSeats
                    {

                       SeatNumber = x.SeatNumber,
                     
                       BusId=x.Bus.BusId
                    }).ToList();
            }
            catch (Exception ex)
            {
                _errorHandler.Capture(ex, "An Error while Get Buses");
                throw new Exception(ex.Message);
            }
        }


        // --------------------------------------------------------------------
        // ✅ GET AVAILABLE SEATS
        // --------------------------------------------------------------------
        /// <summary>
        /// Retrieves available seats for a given bus and journey date.
        /// </summary>
        /// <param name="busId">Bus ID.</param>
        /// <param name="date">Journey date.</param>
        /// <returns>List of available seats with booking details.</returns>
        public List<ResponseAvailableseats> GetAvailableseats(int Busid, DateTime? date)
        {
            try
            {
                if (Busid <= 0)
                    throw new ArgumentException("Invalid BusId.");

                if (date == null)
                    throw new ArgumentException("Date is required.");

                var targetDate = date.Value.Date;

                // ✅ Fetch all seats for the given bus
                var allSeats = _dbBbsContext.Tblseats
                    .Where(s => s.BusId == Busid)
                    .ToList();

                // ✅ Fetch all booked seat IDs for this bus on the selected date (excluding cancelled)
                var bookedSeatIds = _dbBbsContext.Tblbookingseats
                    .Include(bs => bs.Booking)
                    .Where(bs => bs.BusId == Busid
                              && bs.Booking != null
                              && bs.Booking.BookingDate.HasValue
                              && bs.Booking.BookingDate.Value.Date == targetDate
                              && bs.Booking.Status.ToLower() != "cancelled")
                    .Select(bs => bs.SeatId)
                    .ToList();

                // ✅ Identify available seats
                var availableSeats = allSeats
                    .Where(s => !bookedSeatIds.Contains(s.SeatId))
                    .Select(s => new seatsinfo
                    {
                        SeatId = s.SeatId,
                        SeatNumber = s.SeatNumber
                    })
                    .ToList();

                // ✅ Prepare response object
                var response = new ResponseAvailableseats
                {
                    BusId = Busid,
                    Date = targetDate.ToString("yyyy-MM-dd"),
                    TotalSeats = allSeats.Count,
                    BookedSeats = bookedSeatIds.Count,
                    AvailableSeats = availableSeats
                };

                // ✅ Return as list (to match your return type)
                return new List<ResponseAvailableseats> { response };
            }
            catch (Exception ex)
            {
                _errorHandler.Capture(ex, $"Error occurred while fetching available seats for BusId: {Busid}");
                throw new ApplicationException("An error occurred while fetching available seats.", ex);
            }

        }

        // --------------------------------------------------------------------
        // ✅ GET SEATS BY BUS ID
        // --------------------------------------------------------------------
        /// <summary>
        /// Retrieves seats for a specific bus along with booking info.
        /// </summary>
        /// <param name="busId">Bus ID.</param>
        /// <returns>List of seats with user booking information.</returns>
        public List<ResponseGetseatsbybusid> GetparthicularbusSeats(int busId, DateTime travelDate)
        {
            try
            {
                if (busId <= 0)
                    throw new ArgumentException("Invalid BusId.");

                // Normalize the travel date to ignore time
                var normalizedDate = travelDate.Date;

                // ✅ Fetch all seats for the given bus
                var seats = _dbBbsContext.Tblseats
                             .Include(s => s.Bus)
                             .Where(s => s.BusId == busId)
                             .ToList();

                // ✅ Fetch only booked seat IDs for the same bus & selected travel date
                var bookedSeatIds = _dbBbsContext.Tblbookingseats
                    .Include(bs => bs.Booking)
                    .Where(bs =>
                        bs.Booking.BusId == busId &&
                        bs.Booking.BookingDate.Value.Date == normalizedDate && // compare user date
                        bs.Booking.Status.ToLower() != "cancelled"
                    )
                    .Select(bs => bs.SeatId)
                    .ToList();

                // ✅ Build response
                var response = seats.Select(s => new ResponseGetseatsbybusid
                {
                    BusId = s.Bus.BusId,
                    SeatId = s.SeatId,
                    SeatNumber = s.SeatNumber,
                    IsBooked = bookedSeatIds.Contains(s.SeatId)
                }).ToList();

                return response;
            }
            catch (Exception ex)
            {
                _errorHandler.Capture(ex, $"Error fetching seats for BusId: {busId} on {travelDate:yyyy-MM-dd}");
                throw new ApplicationException($"An error occurred while fetching seats for BusId: {busId}.", ex);
            }
        }



        // --------------------------------------------------------------------
        // ✅ UPDATE SEAT
        // --------------------------------------------------------------------
        /// <summary>
        /// Updates seat information such as seat number and availability.
        /// </summary>
        /// <param name="updateSeats">Updated seat data.</param>
        /// <returns>Status message.</returns>

        public string UpdateSeats(RequestUpdateSeats UpdateSeats)
        {
            try
            {
                if (UpdateSeats != null && UpdateSeats.SeatId > 0)
                {
                    var seats = _dbBbsContext.Tblseats.FirstOrDefault(x => x.SeatId == UpdateSeats.SeatId);
                    if (seats != null)
                    {
                      seats.SeatId = UpdateSeats.SeatId;
                        seats.SeatNumber = UpdateSeats.SeatNumber;
                      
                        seats.ModifiedAt=DateTime.Now;
                        _dbBbsContext.SaveChanges();


                    }
                }
                return "Updated Successfully ";

            }
            catch (Exception ex)
            {
                _errorHandler.Capture(ex, "An Error while Get Buses");
                throw new Exception(ex.Message);

            }
        }

    }
}
