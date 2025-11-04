// =====================================================================================
// Project Name : BusBookingSystem
// File Name    : ConfirmBooking.cs
// Created By   : Kaviraj M
// Created On   : 21/09/2025
// Modified By  : Kaviraj M
// Modified On  : 28/10/2025
// Description  : Handles booking confirmation, passenger addition, seat updates,
//                cancellations, and ticket generation logic for the Bus Booking System.
// =======================================================================================

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text.Json;
using System.IO;
using BusBookingSystem.Application;
using BusBookingSystem.Application.BookingDtos;
using BusBookingSystem.Domain.Models;
using BusBookingSystem.Infrastructure.RepositoryInterface;
using Microsoft.EntityFrameworkCore;
using Npgsql.Internal;
using static System.Net.Mime.MediaTypeNames;
using AutoMapper;

namespace BusBookingSystem.Infrastructure.RepositoryImplementation
{
    /// <summary>
    /// Repository implementation for confirming bookings, managing passengers, and
    /// generating ticket-related data in the Bus Booking System.
    /// </summary>
    public class ConfirmBooking : Iconfirmbooking
    {
        private readonly DbBbsContext _dbBbsContext;
        private readonly ErrorHandler _errorHandler;
        private readonly IMapper _mapper;
        /// <summary>
        /// Initializes a new instance of the <see cref="ConfirmBooking"/> class.
        /// </summary>
        /// <param name="dbBbsContext">Database context for accessing booking data.</param>
        /// <param name="errorHandler">Error handler for capturing and logging exceptions.</param>
        public ConfirmBooking(DbBbsContext dbBbsContext, ErrorHandler errorHandler , IMapper mapper)
        {
            _dbBbsContext = dbBbsContext;
            _errorHandler = errorHandler;
            _mapper = mapper;

        }

        // --------------------------------------------------------------------
        // ✅ ADD PASSENGER
        // --------------------------------------------------------------------
        /// <summary>
        /// Adds a single passenger to an existing booking.
        /// </summary>
        /// <param name="addpassengers">Passenger details DTO.</param>
        /// <returns>Status message.</returns>
        public string Addpassaengers(Addpassengers addpassengers)
        {
            try
            {
                if (addpassengers == null)
                    return "Invalid Input";

                var passenger = new Tblpassenger
                {
                    BookingId = addpassengers.BookingId,
                    PassengerName = addpassengers.PassengerName,
                    PassengerAge = addpassengers.PassengerAge,
                    PassengerGender = addpassengers.PassengerGender,
                };

                _dbBbsContext.Tblpassengers.Add(passenger);
                _dbBbsContext.SaveChanges();

                return "Added Successfully";
            }
            catch (Exception ex)
            {
                _errorHandler.Capture(ex, "Error while adding passenger");
                throw;
            }
        }

        // --------------------------------------------------------------------
        // ✅ ADD BOOKING
        // --------------------------------------------------------------------
        /// <summary>
        /// Creates a new booking with passenger and seat information.
        /// </summary>
        /// <param name="addbookings">Booking request DTO.</param>
        /// <returns>Response DTO containing booking details and status.</returns>
        public ResponsebookingDto Addbooking(Requestbookingdto addbookings)
        {
            using var transaction = _dbBbsContext.Database.BeginTransaction();
            try
            {
                if (addbookings == null || addbookings.UserId <= 0 || addbookings.BusId <= 0 || addbookings.RouteId <= 0)
                {
                    return new ResponsebookingDto
                    {
                        BookingId = 0,
                        Message = "Invalid booking request"
                    };
                }

                var bookingDate = DateTime.SpecifyKind(
                         (addbookings.BookingDate ?? DateTime.Now).Date,
                         DateTimeKind.Unspecified
                                );


                // ✅ 1. Create new booking
                var booking = new Tblbooking
                {
                    UserId = addbookings.UserId,
                    BusId = addbookings.BusId,
                    RouteId = addbookings.RouteId,
                    BookingDate = bookingDate,
                    TotalFare = addbookings.TotalFare,
                    Status = "Confirmed",
                    IsActive = true,
                    IsDelete = false
                };

                _dbBbsContext.Tblbookings.Add(booking);
                _dbBbsContext.SaveChanges();

                // ✅ 2. Add passengers
                if (addbookings.Passangers != null && addbookings.Passangers.Any())
                {
                    var passengers = addbookings.Passangers.Select(p => new Tblpassenger
                    {
                        BookingId = booking.BookingId,
                        PassengerName = p.PassengerName,
                        PassengerAge = p.PassengerAge,
                        PassengerGender = p.PassengerGender
                    }).ToList();

                    _dbBbsContext.Tblpassengers.AddRange(passengers);
                    _dbBbsContext.SaveChanges();
                }

                // ✅ 3. Handle seat booking (date-wise availability)
                var seatIds = addbookings.SeatIds?.Where(s => s > 0).ToList() ?? new List<int>();

                // All seats selected by user
                var seatsToBook = _dbBbsContext.Tblseats
                    .Where(s => seatIds.Contains(s.SeatId))
                    .ToList();

                // ✅ 4. Check if any of these seats are already booked for the same bus on same date
                var bookedSeatIds = _dbBbsContext.Tblbookingseats
                    .Where(bs =>
                      bs.BusId == addbookings.BusId &&
                            bs.Booking.BookingDate.HasValue &&
                          bs.Booking.BookingDate.Value.Date == bookingDate &&
                        (bs.Booking.IsDelete != true) &&
                         (bs.Booking.Status != "Cancelled"))
                        .Select(bs => bs.SeatId)
                             .ToList();


                var unavailableSeats = seatsToBook.Where(s => bookedSeatIds.Contains(s.SeatId)).ToList();

                if (unavailableSeats.Any())
                {
                    transaction.Rollback();
                    return new ResponsebookingDto
                    {
                        BookingId = 0,
                        Message = $"Seats already booked for {bookingDate:dd-MMM-yyyy}: {string.Join(", ", unavailableSeats.Select(s => s.SeatNumber))}",
                        SeatNumbers = unavailableSeats.Select(s => s.SeatNumber).ToList()
                    };
                }

                // ✅ 5. Add seats to booking junction table (no IsAvailable flag needed)
                var bookingSeats = seatsToBook.Select(seat => new Tblbookingseat
                {
                    BookingId = booking.BookingId,
                    SeatId = seat.SeatId,
                    BusId = booking.BusId
                }).ToList();

                _dbBbsContext.Tblbookingseats.AddRange(bookingSeats);
                _dbBbsContext.SaveChanges();

                transaction.Commit();

                // ✅ 6. Return response
                return new ResponsebookingDto
                {
                    BookingId = booking.BookingId,
                    Message = "Booking Added Successfully",
                    BookingDate = booking.BookingDate,
                    SeatIds = seatsToBook.Select(s => s.SeatId).ToList(),
                    SeatNumbers = seatsToBook.Select(s => s.SeatNumber).ToList()
                };
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                _errorHandler.Capture(ex, "Error in Addbooking");
                return new ResponsebookingDto
                {
                    BookingId = 0,
                    Message = $"Error: {ex.InnerException?.Message ?? ex.Message}"
                };
            }
        }

        // --------------------------------------------------------------------
        // ✅ GET BUS BY ID
        // --------------------------------------------------------------------
        /// <summary>
        /// Retrieves bus and route information for the specified Bus ID.
        /// </summary>
        /// <param name="busId">Unique Bus ID.</param>
        /// <returns>List of bus DTOs with route details.</returns>
        public async Task<List<BusDto>> GetBus(int busId)
        {
            try
            {
                return await _dbBbsContext.Tblbuses
                    .Include(b => b.Tblroutes)
                    .Where(b => b.BusId == busId)
                    .Select(b => new BusDto
                    {
                        BusId = b.BusId,
                        BusName = b.BusName,
                        BusNumber = b.Busnumber,
                        BusType = b.BusType,
                        Routes = b.Tblroutes.Select(r => new RouteDto
                        {
                            RouteId = r.RouteId,
                            Source = r.Source,
                            Destination = r.Destination
                        }).ToList()
                    }).ToListAsync ();
            }
            catch (Exception ex)
            {
                _errorHandler.Capture(ex, $"Error fetching buses for busid: {busId}");
                throw;
            }
        }


        // --------------------------------------------------------------------
        // ✅ GET BOOKINGS BY USER ID
        // --------------------------------------------------------------------
        /// <summary>
        /// Retrieves all bookings for a specific user, including passengers and seat details.
        /// </summary>
        /// <param name="userId">User ID.</param>
        /// <returns>List of bookings with associated details.</returns>
        public List<BookingResponse> GetBookingByid(int userId)
        {
            try
            {
                var booking = (from b in _dbBbsContext.Tblbookings
                               join s in _dbBbsContext.Tblusers on b.UserId equals s.UserId
                               join a in _dbBbsContext.Tblbuses on b.BusId equals a.BusId
                               join c in _dbBbsContext.Tblroutes on b.RouteId equals c.RouteId
                               where b.UserId == userId
                               orderby b.BookingDate descending
                               select new BookingResponse
                               {
                                   BookingId = b.BookingId,
                                   BookingDate = b.BookingDate,
                                   UserId = b.UserId,
                                   BusId = b.BusId,
                                   RouteId = b.RouteId,
                                   TotalFare = b.TotalFare,
                                   Status = b.Status,
                                   IsActive = b.IsActive,
                                   passangers = (from x in _dbBbsContext.Tblpassengers
                                                 where x.BookingId == b.BookingId
                                                 select new Passangerinfo
                                                 {
                                                     PassengerId = x.PassengerId,
                                                     PassengerName = x.PassengerName,

                                                     PassengerAge = x.PassengerAge,
                                                     PassengerGender = x.PassengerGender
                                                 }).ToList(),
                                   SeatIds = (from y in _dbBbsContext.Tblbookingseats
                                              where y.BookingId == b.BookingId
                                              select y.Seat.SeatId).ToList(),

                                   SeatNumbers = (from ks in _dbBbsContext.Tblbookingseats
                                                  where ks.BookingId == b.BookingId
                                                  select ks.Seat.SeatNumber).ToList(),
                                   tblbuses = new BusInfoDto
                                   {
                                       BusId = b.BusId,
                                       BusName = b.Bus.BusName,
                                       Busnumber = b.Bus.Busnumber,

                                       BusType = b.Bus.BusType,


                                       routesinfo = (from sk in _dbBbsContext.Tblroutes
                                                     where sk.BusId == b.BusId
                                                     select new RoutesinfoDto
                                                     {
                                                         RouteId = b.Route.RouteId,
                                                         Source = b.Route.Source,
                                                         Destination = b.Route.Destination
                                                     }).ToList()
                                   }
                               }).ToList();
                return booking;
            }
            catch (Exception ex)
            {
                _errorHandler.Capture(ex, $"Error fetching booking info: {userId}");
                throw;
            }
        }


        // --------------------------------------------------------------------
        // ✅ CANCEL BOOKING
        // --------------------------------------------------------------------
        /// <summary>
        /// Cancels a booking and releases all booked seats.
        /// </summary>
        /// <param name="Bookingid">Booking ID.</param>
        /// <returns>Status message.</returns>
        public string Cancellbooking(int bookingId)
        {
            try
            {
                var booking = _dbBbsContext.Tblbookings
                    .Include(b => b.Tblbookingseats)
                    .FirstOrDefault(b => b.BookingId == bookingId);

                if (booking == null)
                    return "Booking ID not found";

                if (booking.Status == "Cancelled" && booking.IsActive == false)
                    return "Booking is already cancelled";

                // ✅ Update booking status
                booking.Status = "Cancelled";
                booking.IsActive = false;
                booking.IsDelete = true;

                // ✅ Remove the link to booked seats — seats become available automatically
                if (booking.Tblbookingseats != null && booking.Tblbookingseats.Any())
                {
                    _dbBbsContext.Tblbookingseats.RemoveRange(booking.Tblbookingseats);
                }

                _dbBbsContext.Tblbookings.Update(booking);
                _dbBbsContext.SaveChanges();

                return "Booking cancelled successfully";
            }
            catch (Exception ex)
            {
                _errorHandler.Capture(ex, $"Error cancelling booking: {bookingId}");
                throw;
            }
        }


        // --------------------------------------------------------------------
        // ✅ DOWNLOAD TICKETS
        // --------------------------------------------------------------------
        /// <summary>
        /// Retrieves ticket information for a given booking ID.
        /// </summary>
        /// <param name="Bookingid">Booking ID.</param>
        /// <returns>List of downloadable ticket DTOs.</returns>
        public List<ResponseDownloadtickets> DownloadTickets(int Bookingid)
        {
            try
            {
                var booking = _dbBbsContext.Tblbookings
                    .Include(x => x.User)
                    .Include(x => x.Bus)
                    .Include(x => x.Route)
                    .Include(x => x.Tblbookingseats).ThenInclude(x => x.Seat)
                    .Where(x => x.BookingId == Bookingid)
                    .ToList();

                List<ResponseDownloadtickets> responseDownloadtickets = [];
                foreach (var book in booking)
                {
                    var downloadbookings = _mapper.Map<ResponseDownloadtickets>(book);
                    downloadbookings.DownloadUserinfo = _mapper.Map<downloadUserinfo>(book.User);
                    downloadbookings.Busesdto = _mapper.Map<Downloadbusdto>(book.Bus);
                    downloadbookings.RoutesDto = _mapper.Map<DownloadRoutesDto>(book.Route);
                    downloadbookings.SeatNumber = book.Tblbookingseats.Select(bs => bs.Seat.SeatNumber).ToList();
                    downloadbookings.passangers = _mapper.Map<List<Passangerinfo>>(book.Tblpassengers);


                    responseDownloadtickets.Add(downloadbookings);

                }

                if (booking == null)
                    return null;

                return responseDownloadtickets;
            }

            catch (Exception ex)
            {
                _errorHandler.Capture(ex, $"Error downloading ticket for booking: {Bookingid}");
                throw;
            }
        
        }
    }
}



