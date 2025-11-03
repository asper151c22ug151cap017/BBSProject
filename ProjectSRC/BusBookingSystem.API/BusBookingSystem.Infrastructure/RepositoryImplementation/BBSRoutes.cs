
// =====================================================================================================
// Project Name : BusBookingSystem
// File Name    : BBSRoutes.cs
// Created By   : Kaviraj M
// Created On   : 20/09/2025
// Modified By  : Kaviraj M
// Modified On  : 28/10/2025
// Description  :  Repository implementation for managing bus routes in the Bus Booking System.
//                 Handles CRUD operations, route filtering, and route count retrieval.
//                 Uses Entity Framework Core for database interactions with proper error handling.
// =====================================================================================================

using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusBookingSystem.Application;
using BusBookingSystem.Application.BusDtos;
using BusBookingSystem.Application.RoutesDtos;
using BusBookingSystem.Domain.Models;
using BusBookingSystem.Infrastructure.RepositoryInterface;
using Microsoft.EntityFrameworkCore;

namespace BusBookingSystem.Infrastructure.RepositoryImplementation
{
    /// <summary>
    /// Repository implementation for managing route-related operations in the Bus Booking System.
    /// </summary>
    public class BBSRoutes : IBBSRoutes
    {
        private readonly DbBbsContext _dbBbsContext;
        private readonly ErrorHandler _errorHandler;

        /// <summary>
        /// Initializes a new instance of the <see cref="BBSRoutes"/> class.
        /// </summary>
        /// <param name="dbContext">Database context for EF operations.</param>
        /// <param name="errorHandler">Custom error handler for managing exceptions.</param>
        public BBSRoutes(DbBbsContext dbBbsContext, ErrorHandler errorHandler)
        {
            _dbBbsContext = dbBbsContext;
            _errorHandler = errorHandler;
        }


        // --------------------------------------------------------------------
        // ✅ ADD ROUTE
        // --------------------------------------------------------------------
        /// <summary>
        /// Adds a new route to the system.
        /// </summary>
        /// <param name="addRoutes">DTO containing new route details.</param>
        /// <returns>Status message.</returns>
        public string AddRoutes(RequestAddRoutes addRoutes)
        {
            try
            {
                var now = DateTime.Now;

                if (addRoutes == null)
                    return "Invalid Input";

                // ✅ Using LINQ Query Syntax instead of FirstOrDefault()
                var busQuery =
                    from b in _dbBbsContext.Tblbuses
                    where b.BusId == addRoutes.Busid
                    select b;

                var bus = busQuery.FirstOrDefault();

                if (bus == null)
                    return "Bus not found";

                // ✅ Create new route record
                var newRoute = new Tblroute
                {
                    BusId = bus.BusId,
                    Source = addRoutes.Source,
                    Destination = addRoutes.Destination,
                    Departuretime = addRoutes.Departuretime,
                    Arrivaltime = addRoutes.Arrivaltime,
                    CreatedAt = now,
                    CreatedBy = addRoutes.createdBy,
                    ModifiedAt = now,
                    IsActive = true
                };

                // ✅ Using LINQ method to add new entity
                _dbBbsContext.Tblroutes.Add(newRoute);
                _dbBbsContext.SaveChanges();

                return "Added Successfully";
            }
            catch (Exception ex)
            {
                _errorHandler.Capture(ex, "An Error while Add Routes");
                throw new Exception(ex.Message);
            }
        }


        // --------------------------------------------------------------------
        // ✅ DELETE ROUTE (Soft Delete)
        // --------------------------------------------------------------------
        /// <summary>
        /// Performs a soft delete on a route by marking it inactive and deleted.
        /// </summary>
        /// <param name="routeId">Route ID to be deleted.</param>
        /// <returns>Status message.</returns>
        public string DeleteRoutes(int RouteId)
        {
            try
            {
                var deleteroutes = _dbBbsContext.Tblroutes.FirstOrDefault(d => d.RouteId == RouteId);
                if (deleteroutes != null)
                {
                    deleteroutes.IsActive = false;
                    deleteroutes.IsDelete = true;
                    _dbBbsContext.Tblroutes.Update(deleteroutes);

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
        // ✅ FILTER ROUTES
        // --------------------------------------------------------------------
        /// <summary>
        /// Retrieves routes based on source and destination filters.
        /// Includes bus details, ratings, and available seats.
        /// </summary>
        /// <param name="source">Starting point of the route.</param>
        /// <param name="destination">Ending point of the route.</param>
        /// <returns>List of routes matching the criteria.</returns>
        public List<ResponseRoutes> FilterRoutes(string Source, string Destination, DateTime TravelDate)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(Source) || string.IsNullOrWhiteSpace(Destination))
                    return new List<ResponseRoutes>();

                var sourceNormalized = Source.Trim().ToLower();
                var destinationNormalized = Destination.Trim().ToLower();

                // ✅ FIX: Do NOT convert to local time — it causes one-day shift
                var travelDateOnly = TravelDate.Date;

                var routes = _dbBbsContext.Tblroutes
                    .Include(r => r.Bus)
                        .ThenInclude(b => b.Tblbusratings)
                    .Include(r => r.Bus)
                        .ThenInclude(b => b.Tblseats)
                    .Include(r => r.Bus)
                        .ThenInclude(b => b.Tblbookings)
                            .ThenInclude(bk => bk.Tblbookingseats)
                    .Where(r => r.Source.Trim().ToLower() == sourceNormalized
                             && r.Destination.Trim().ToLower() == destinationNormalized)
                    .ToList();

                var result = new List<ResponseRoutes>();

                foreach (var route in routes)
                {
                    var bus = route.Bus;
                    if (bus == null)
                        continue;

                    // ✅ Use the travelDateOnly directly
                    var bookedSeatNumbers = bus.Tblbookings?
                        .Where(bk => bk.BookingDate.HasValue
                                  && bk.BookingDate.Value.Date == travelDateOnly
                                  && bk.Status.ToLower() != "cancelled")
                        .SelectMany(bk => bk.Tblbookingseats)
                        .Select(bs => bs.Seat.SeatNumber)
                        .Distinct()
                        .ToList() ?? new List<string>();

                    var response = new ResponseRoutes
                    {
                        RouteId = route.RouteId,
                        Busid = bus.BusId,
                        Source = route.Source,
                        Destination = route.Destination,
                        Departuretime = route.Departuretime,
                        Arrivaltime = route.Arrivaltime,
                        BusName = bus.BusName ?? "N/A",
                        Fare = bus.Fare ?? 0,
                        BusType = bus.BusType ?? "Unknown",
                        RatingValue = bus.Tblbusratings?.Any() == true
                            ? bus.Tblbusratings.Average(br => br.RatingValue)
                            : 0,
                        BookedSeatNumbers = bookedSeatNumbers,
                        TravelDate = TravelDate.Date
                    };

                    result.Add(response);
                }

                return result;
            }
            catch (Exception ex)
            {
                _errorHandler.Capture(ex, "Error while filtering routes");
                throw;
            }
        }



        // --------------------------------------------------------------------
        // ✅ GET ALL ROUTES
        // --------------------------------------------------------------------
        /// <summary>
        /// Retrieves all active routes with associated bus details.
        /// </summary>
        /// <returns>List of all active routes.</returns>
        public List<ResponseRoutes> GetAllRoutes()
        {
            try
            {
                var result = (from r in _dbBbsContext.Tblroutes
                              join b in _dbBbsContext.Tblbuses
                              on r.BusId equals b.BusId
                              where r.IsActive == true
                              select new ResponseRoutes
                              {
                                  RouteId = r.RouteId,
                                  Busid = b.BusId,
                                  Source = r.Source,
                                  Destination = r.Destination,
                                  IsActive = r.IsActive,
                                  Busnumber = b.Busnumber,
                                  Departuretime = r.Departuretime,
                                  Arrivaltime = r.Arrivaltime,
                                  BusName = b.BusName,
                                  Fare = b.Fare,
                                  BusType = b.BusType
                              }).ToList();

                return result;
            }
            catch (Exception ex)
            {
                _errorHandler.Capture(ex, "An Error while Get Buses");
                throw new Exception(ex.Message);
            }
        }



        // --------------------------------------------------------------------
        // ✅ GET ROUTES COUNT
        // --------------------------------------------------------------------
        /// <summary>
        /// Retrieves the total number of routes in the system.
        /// </summary>
        /// <returns>Total count of routes.</returns>
        public int GetRoutesCount()
        {
            return _dbBbsContext.Tblroutes.Count();
        }

        // --------------------------------------------------------------------
        // ✅ UPDATE ROUTE
        // --------------------------------------------------------------------
        /// <summary>
        /// Updates route and bus details.
        /// </summary>
        /// <param name="updateRoutesInfo">Updated route details DTO.</param>
        /// <returns>Status message.</returns>
        public string UpdateRoutes(RequestUpdateRoutes updateRoutesinfo )
        {
            try
            {
                if (updateRoutesinfo != null && updateRoutesinfo.RouteId > 0)
                {
                    // ✅ LINQ query to get route with related bus details
                    var query = (from r in _dbBbsContext.Tblroutes
                                 join b in _dbBbsContext.Tblbuses
                                 on r.BusId equals b.BusId
                                 where r.RouteId == updateRoutesinfo.RouteId
                                 select new { Route = r, Bus = b }).FirstOrDefault();

                    if (query != null)
                    {
                        var route = query.Route;
                        var bus = query.Bus;

                        // ✅ Update values
                        route.BusId = updateRoutesinfo.BusId;
                        bus.BusName = updateRoutesinfo.BusName;
                        bus.Busnumber = updateRoutesinfo.Busnumber;
                        bus.BusType = updateRoutesinfo.BusType;
                        route.Source = updateRoutesinfo.Source;
                        route.Destination = updateRoutesinfo.Destination;
                        route.Departuretime = updateRoutesinfo.Departuretime;
                        route.Arrivaltime = updateRoutesinfo.Arrivaltime;
                        bus.Fare = updateRoutesinfo.Fare;

                        // ✅ Set modified info
                        route.ModifiedAt = DateTime.Now;
                        route.ModifiedBy = updateRoutesinfo.UserId;

                        // ✅ Save all changes
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
