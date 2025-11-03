// ===========================================================================================================
// Project Name : BusBookingSystem
// File Name    : BBSbooking.cs
// Created By   : Kaviraj M
// Created On   : 20/09/2025
// Modified By  : Kaviraj M
// Modified On  : 28/10/2025
// Description  : Repository implementation for managing booking-related operations in the Bus Booking System.
//                Handles booking creation, updates, soft deletion, retrieval, and recent booking details.
//                Includes passenger and seat mapping, Entity Framework usage, and proper error handling.
// ===========================================================================================================

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using BusBookingSystem.Application;
using BusBookingSystem.Application.BookingDtos;
using BusBookingSystem.Domain.Models;
using BusBookingSystem.Infrastructure.RepositoryInterface;
using Microsoft.EntityFrameworkCore;

namespace BusBookingSystem.Infrastructure.RepositoryImplementation
{
    /// <summary>
    /// Repository implementation for managing booking-related operations.
    /// Handles booking creation, updates, deletion, and retrieval.
    /// </summary>
    public class BBSbooking:IBBSbooking
    {
        private readonly DbBbsContext _dbBbsContext;
        private readonly ErrorHandler _errorHandler;
        public readonly IMapper _mapper;


        /// <summary>
        /// Initializes a new instance of the <see cref="BBSbooking"/> class.
        /// </summary>
        public BBSbooking(DbBbsContext dbBbsContext, ErrorHandler errorHandler , IMapper mapper)
        {
            _dbBbsContext = dbBbsContext;
            _errorHandler = errorHandler;
            _mapper = mapper;
        }


        // --------------------------------------------------------------------
        // ✅ ADD BOOKING
        // --------------------------------------------------------------------
        /// <summary>
        /// Adds a new booking to the system.
        /// </summary>
        /// <param name="addbookings">Booking details provided by the client.</param>
        /// <returns>Status message.</returns>
        public string AddBooking(RequestAddbookings addbookings)
        {
            try
            {
                var now = DateTime.Now;
                if (addbookings == null)
                    return "Invalid Input";
                var NewBooking = new Tblbooking
                {
                    TotalFare = addbookings.TotalFare,
                    Status = addbookings.Status,
                    CreatedAt = now,
                    ModifiedAt = now
                };

                _dbBbsContext.Tblbookings.Add(NewBooking);
                _dbBbsContext.SaveChanges();


                return "Added Successfully";
            }
            catch (Exception ex)
            {
                _errorHandler.Capture(ex, "An Error while Add Buses");
                throw new Exception(ex.Message);
            }
        
        }


        // --------------------------------------------------------------------
        // ✅ DELETE BOOKING (Soft Delete)
        // --------------------------------------------------------------------
        /// <summary>
        /// Soft deletes a booking by updating IsActive and IsDelete flags.
        /// </summary>
        /// <param name="Bookingid">Booking ID to be deleted.</param>
        /// <returns>Status message.</returns>
        public string DeleteBooking ( int Bookingid )
        {
            try
            {
                var deletebooking = _dbBbsContext.Tblbookings.FirstOrDefault(d => d.BookingId == Bookingid);
                if (deletebooking != null)
                {
                    deletebooking.IsActive = false;
                    deletebooking.IsDelete = true;
                    _dbBbsContext.Tblbookings.Update(deletebooking);

                    _dbBbsContext.SaveChanges();
                    return "Deleted Successfully";
                }
                else
                {
                    return "User Not Found";
                }
            }
            catch (Exception ex)
            {
                _errorHandler.Capture(ex, "An Error while Add Buses");
                throw new Exception(ex.Message);
            }
        }

        // --------------------------------------------------------------------
        // ✅ GET ALL BOOKINGS
        // --------------------------------------------------------------------
        /// <summary>
        /// Retrieves all bookings with related route, bus, user, and seat details.
        /// </summary>
        /// <returns>List of all bookings mapped to response DTOs.</returns>
        public List<Responsegetbooking> Getallbookings()
        {
            try
            {
                    var testResult = _dbBbsContext.Tblbookings
                       .Include(b => b.Route)
                       .Include(b => b.Bus)
                       .Include(b => b.User)
                       .Include(b => b.Tblpassengers)
                       .Include(b => b.Tblbookingseats)
                           .ThenInclude(bs => bs.Seat)
                      
                       .OrderByDescending(b => b.BookingDate)
                       .ToList();

                    List<Responsegetbooking> responsegetbookings = [];
                    foreach (var booking in testResult)
                    {
                        var bookingDto = _mapper.Map<Responsegetbooking>(booking);
                        bookingDto.Passengers = _mapper.Map<List<Responsepassangeinfo>>(booking.Tblpassengers);
                        bookingDto.buses = _mapper.Map<BusesDto>(booking.Bus);
                        bookingDto.Routes = _mapper.Map<RoutesDto>(booking.Route);
                        bookingDto.Users = _mapper.Map<UserinfoDto>(booking.User);
                        bookingDto.seatsdto = booking.Tblbookingseats
                            .Select(bs => new Seatsdto
                            {
                                SeatIds = new List<int> { bs.Seat.SeatId },
                                SeatNumbers = new List<string> { bs.Seat.SeatNumber }
                            }).ToList();


                        responsegetbookings.Add(bookingDto);
                    }
                    return responsegetbookings;
                }
            catch (Exception ex)
            {
                _errorHandler.Capture(ex, "Error fetching all bookings");
                throw;
            }
        }


        // --------------------------------------------------------------------
        // ✅ GET BOOKING COUNT
        // --------------------------------------------------------------------
        /// <summary>
        /// Returns the total count of bookings.
        /// </summary>
        /// <returns>Integer representing total bookings.</returns>
        public int Getcountbookings()
        {
            return _dbBbsContext.Tblbookings.Count();
        }

        // --------------------------------------------------------------------
        // ✅ GET RECENT BOOKINGS
        // --------------------------------------------------------------------
        /// <summary>
        /// Retrieves bookings made within the last 24 hours.
        /// </summary>
        /// <returns>List of recent bookings mapped to response DTOs.</returns>
        public List<ResponseGetrecentbookings> Getrecentbookings()
        {
            try
            {
                var today = DateTime.Today;
                var tomorrow = today.AddDays(1);

                var result = _dbBbsContext.Tblbookings
                    .Where(b => b.BookingDate >= today && b.BookingDate < tomorrow)
                    .OrderByDescending(b => b.BookingDate)
                    .ProjectTo<ResponseGetrecentbookings>(_mapper.ConfigurationProvider).ToList();


               
                return result;
            }
            catch (Exception ex)
            {
                _errorHandler.Capture(ex, "Error fetching recent bookings");
                throw new Exception(ex.Message);
            }
        }

        // --------------------------------------------------------------------
        // ✅ UPDATE BOOKING
        // --------------------------------------------------------------------
        /// <summary>
        /// Updates booking details such as total fare and modification time.
        /// </summary>
        /// <param name="updatebooking">Booking data to be updated.</param>
        /// <returns>Status message.</returns>
        public string UpdateBooking(Requestupdatebooking updatebooking)
        {
            try
            {
                if (updatebooking != null && updatebooking.BookingId > 0)
                {
                    var Bookings = _dbBbsContext.Tblbookings.Include(x=> x.Tblbookingseats).ThenInclude(x=> x.Seat)
                        .FirstOrDefault(x => x.BookingId == updatebooking.BookingId);
                    if (Bookings != null)
                    {
                        Bookings.BookingId = updatebooking.BookingId;
                       Bookings.TotalFare = updatebooking.TotalFare;
                        Bookings.Status = updatebooking.Status;
                        Bookings.BookingDate = updatebooking.BookingDate;
                        Bookings.ModifiedAt= DateTime.Now;


                        var seatnum = Bookings.Tblbookingseats.Select(b => b.Seat.SeatNumber).ToList();

                        // ✅ Optional: if frontend sends updated seat numbers
                        if (updatebooking.SeatNumbers != null && updatebooking.SeatNumbers.Any())
                        {
                            // Step 1: Remove old seat links
                            _dbBbsContext.Tblbookingseats
                                .RemoveRange(Bookings.Tblbookingseats);

                            // Step 2: Fetch the new seats from Tblseats
                            var newSeats = _dbBbsContext.Tblseats
                                .Where(s => updatebooking.SeatNumbers.Contains(s.SeatNumber))
                                .ToList();

                            // Step 3: Re-link the new seats
                            Bookings.Tblbookingseats = newSeats
                                .Select(seat => new Tblbookingseat
                                {
                                    BookingId = Bookings.BookingId,
                                    SeatId = seat.SeatId
                                })
                                .ToList();

                            // Step 4: Update seatNumbers variable for response/log
                            seatnum = updatebooking.SeatNumbers;
                        }

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
