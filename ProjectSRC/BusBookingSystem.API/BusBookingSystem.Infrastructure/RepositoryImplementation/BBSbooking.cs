// ===========================================================================================================
// Project Name : BusBookingSystem
// File Name    : BBSbooking.cs
// Created By   : Kaviraj M
// Created On   : 20/09/2025
// Modified By  : Kaviraj M
// Modified On  : 05/11/2025
// Description  : Implements the booking repository for the Bus Booking System.
//                Provides asynchronous operations for creating, updating, deleting, 
//                retrieving, and counting bookings. Includes Entity Framework Core 
//                data access, AutoMapper integration, and structured error handling.
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
using BusBookingSystem.Application.SeatsDtos;
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
        /// <param name="dbBbsContext">The database context used for data operations.</param>
        /// <param name="errorHandler">The centralized error handler for logging exceptions.</param>
        /// <param name="mapper">The AutoMapper instance for entity-to-DTO mapping.</param>
        public BBSbooking(DbBbsContext dbBbsContext, ErrorHandler errorHandler , IMapper mapper)
        {
            _dbBbsContext = dbBbsContext;
            _errorHandler = errorHandler;
            _mapper = mapper;
        }

        // --------------------------------------------------------------------
        // ✅ GET ALL BOOKINGS
        // --------------------------------------------------------------------
        /// <summary>
        /// Asynchronously retrieves all active bookings from the database, 
        /// including related route, bus, user, passenger, and seat details.
        /// </summary>
        /// <returns>
        /// A task that represents the asynchronous operation. The task result 
        /// contains a list of <see cref="Responsegetbooking"/> DTOs.
        /// </returns>
        public async Task<List<Responsegetbooking>> GetAllBookingsAsync()
        {
            try
            {
                var testResult = await _dbBbsContext.Tblbookings
                   .Include(b => b.Route)
                   .Include(b => b.Bus)
                   .Include(b => b.User)
                   .Include(b => b.Tblpassengers)
                   .Include(b => b.Tblbookingseats)
                       .ThenInclude(bs => bs.Seat)

                   .OrderByDescending(b => b.BookingDate)
                   .AsNoTracking()// for read-only  then improve the performance
                   .ToListAsync();

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
                        }).ToList()?? new List<Seatsdto>();//// for handel null safety 


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
        // ✅ ADD BOOKING
        // --------------------------------------------------------------------
        /// <summary>
        /// Asynchronously creates a new booking in the system.
        /// </summary>
        /// <param name="addbookings">The booking details provided by the client.</param>
        /// <returns>
        /// A task that represents the asynchronous operation. The task result 
        /// contains a status message indicating success or failure.
        /// </returns>
        public async Task<string> AddBookingAsync(RequestAddbookings addbookings)
        {

            try
            {
                var now = DateTime.Now;

                if (addbookings == null)
                    return "Invalid Input";

                var newBooking = new Tblbooking
                {
                    TotalFare = addbookings.TotalFare,
                    Status = addbookings.Status,
                    CreatedAt = now,
                    ModifiedAt = now
                };

                await _dbBbsContext.Tblbookings.AddAsync(newBooking); // ✅ Async Add
                await _dbBbsContext.SaveChangesAsync();               // ✅ Async Save

                return "Added Successfully";
            }
            catch (Exception ex)
            {
                _errorHandler.Capture(ex, "An error occurred while adding booking");
                throw; // Re-throw to preserve stack trace (better than new Exception)
            }
        }

        // --------------------------------------------------------------------
        // ✅ UPDATE BOOKING
        // --------------------------------------------------------------------
        /// <summary>
        /// Asynchronously updates booking details such as total fare, status, 
        /// and seat assignments.
        /// </summary>
        /// <param name="updatebooking">The booking details to update.</param>
        /// <returns>
        /// A task that represents the asynchronous operation. The task result 
        /// contains a status message indicating success or failure.
        /// </returns>
        public async Task<string> UpdateBookingAsync(Requestupdatebooking updatebooking)
        {
            try
            {
                if (updatebooking != null && updatebooking.BookingId > 0)
                {
                    var Bookings = await _dbBbsContext.Tblbookings.Include(x => x.Tblbookingseats).ThenInclude(x => x.Seat)
                        .FirstOrDefaultAsync(x => x.BookingId == updatebooking.BookingId);
                    if (Bookings != null)
                    {
                        Bookings.BookingId = updatebooking.BookingId;
                        Bookings.TotalFare = updatebooking.TotalFare;
                        Bookings.Status = updatebooking.Status;
                        Bookings.BookingDate = updatebooking.BookingDate;
                        Bookings.ModifiedAt = DateTime.Now;


                        var seatnum = Bookings.Tblbookingseats.Select(b => b.Seat.SeatNumber).ToList();

                        // ✅ Optional: if frontend sends updated seat numbers
                        if (updatebooking.SeatNumbers != null && updatebooking.SeatNumbers.Any())
                        {
                            // Step 1: Remove old seat links
                            _dbBbsContext.Tblbookingseats
                                .RemoveRange(Bookings.Tblbookingseats);

                            // Step 2: Fetch the new seats from Tblseats
                            var newSeats = await _dbBbsContext.Tblseats
                                .Where(s => updatebooking.SeatNumbers.Contains(s.SeatNumber))
                                .ToListAsync();

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

                       await _dbBbsContext.SaveChangesAsync();


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

        // --------------------------------------------------------------------
        // ✅ DELETE BOOKING (Soft Delete)
        // --------------------------------------------------------------------
        /// <summary>
        /// Asynchronously performs a soft delete by marking a booking as inactive.
        /// </summary>
        /// <param name="Bookingid">The unique booking ID to delete.</param>
        /// <returns>
        /// A task that represents the asynchronous operation. The task result 
        /// contains a status message indicating success or failure.
        /// </returns>
        public async Task<string> DeleteBookingAsync(int Bookingid)
        {
            try
            {
                var deletebooking = await _dbBbsContext.Tblbookings.FirstOrDefaultAsync(d => d.BookingId == Bookingid);
                if (deletebooking != null)
                {
                    deletebooking.IsActive = false;
                    deletebooking.IsDelete = true;
                     _dbBbsContext.Tblbookings.Update(deletebooking);

                    await _dbBbsContext.SaveChangesAsync();
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
        // ✅ GET BOOKING COUNT
        // --------------------------------------------------------------------
        /// <summary>
        /// Asynchronously retrieves the total number of bookings in the system.
        /// </summary>
        /// <returns>
        /// A task that represents the asynchronous operation. The task result 
        /// contains the total booking count.
        /// </returns>
        public async Task<int> GetCountBookingsAsync()
        {
            return await _dbBbsContext.Tblbookings.CountAsync();
        }

        // --------------------------------------------------------------------
        // ✅ GET RECENT BOOKINGS
        // --------------------------------------------------------------------
        /// <summary>
        /// Retrieves bookings made within the last 24 hours.
        /// </summary>
        /// <returns>List of recent bookings mapped to response DTOs.</returns>
        public async Task<List<ResponseGetrecentbookings>> GetRecentBookingsAsync()
        {
            try
            {
                var today = DateTime.Today;
                var tomorrow = today.AddDays(1);

                var result = await _dbBbsContext.Tblbookings
                    .Where(b => b.BookingDate >= today && b.BookingDate < tomorrow)
                    .OrderByDescending(b => b.BookingDate)
                    .ProjectTo<ResponseGetrecentbookings>(_mapper.ConfigurationProvider).ToListAsync();



                return result?? new List<ResponseGetrecentbookings>();
            }
            catch (Exception ex)
            {
                _errorHandler.Capture(ex, "Error fetching recent bookings");
                throw new Exception(ex.Message);
            }
        }
    }
}
