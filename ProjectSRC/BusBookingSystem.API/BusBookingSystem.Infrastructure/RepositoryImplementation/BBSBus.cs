// ============================================================================
// Project Name : BusBookingSystem
// File Name    : BBSBus.cs
// Created By   : Kaviraj M
// Created On   : 20/09/2025
// Modified By  : kaviraj M
// Modified On  : 28/10/2025
// Description  : Repository class implementing all Bus-related operations for
//                the Bus Booking System including CRUD, validation, and 
//                audit tracking (CreatedBy, ModifiedBy, etc.).
// ============================================================================


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusBookingSystem.Application;
using BusBookingSystem.Application.BusDtos;
using BusBookingSystem.Domain.Models;
using BusBookingSystem.Infrastructure.RepositoryInterface;
using Microsoft.EntityFrameworkCore;

namespace BusBookingSystem.Infrastructure.RepositoryImplementation
{
    /// <summary>
    /// Implementation of <see cref="IBBSBus"/> responsible for managing bus data
    /// in the database including CRUD operations and count retrieval.
    /// </summary>
    public class BBSBus: IBBSBus
    {
     
        private readonly DbBbsContext _dbBbsContext;
        private readonly ErrorHandler _errorHandler;

        /// <summary>
        /// Initializes a new instance of the <see cref="BBSBus"/> class.
        /// </summary>
        /// <param name="dbBbsContext">The database context.</param>
        /// <param name="errorHandler">Global error handler service.</param>
        public BBSBus (DbBbsContext dbBbsContext, ErrorHandler errorHandler)
        {
            _dbBbsContext = dbBbsContext;
            _errorHandler = errorHandler;
        }

        // --------------------------------------------------------------------
        // ✅ ADD BUS
        // --------------------------------------------------------------------
        /// <summary>
        /// Adds a new bus to the system after validating duplicates.
        /// </summary>
        /// <param name="addBuses">The DTO containing bus details to add.</param>
        /// <returns>Status message indicating success or failure.</returns>
        public string AddBuses(RequestAddBuses addBuses)
        {
            try
            {
                var now = DateTime.Now;
                if (addBuses == null)
                    return "Invalid Input";
                var existingBuses = _dbBbsContext.Tblbuses.Include(x=> x.Tblbookings).ThenInclude(x=> x.User)
                    .Any(y => y.Busnumber == addBuses.Busnumber && y.OperatorNumber == addBuses.OperatorNumber);
                if (existingBuses)
                    throw new Exception("Busnumber or Operatornumber Already Exist");
                var NewBuses = new Tblbuse
                {
                    BusName = addBuses.BusName,
                    Busnumber = addBuses.Busnumber,
                    BusType = addBuses.BusType,
                    TotalSeats = addBuses.TotalSeats,
                    Fare = addBuses.Fare,
                    OperatorName = addBuses.OperatorName,
                    OperatorNumber = addBuses.OperatorNumber,
                    CreatedAt = now,
                    ModifiedAt=now
                };

                _dbBbsContext.Tblbuses.Add(NewBuses);
                _dbBbsContext.SaveChanges();

                var addcreatedby = _dbBbsContext.Tblbuses.FirstOrDefault();

                if (addcreatedby != null)
                {
                    addcreatedby.CreatedAt = now;
                    addcreatedby.CreatedBy = addBuses.UserId;
                }

               
                return "Added Successfully";
            }
            catch (Exception ex)
            {
                _errorHandler.Capture(ex, "An Error while Add Buses");
                throw new Exception(ex.Message);
            }
        }

        // --------------------------------------------------------------------
        // ✅ DELETE BUS
        // --------------------------------------------------------------------
        /// <summary>
        /// Performs a soft delete of the specified bus record.
        /// </summary>
        /// <param name="busId">The unique ID of the bus to delete.</param>
        /// <returns>Status message indicating success or failure.</returns>
        public string DeleteBuses(int busId)
        {
            try
            {
                var deletebus = _dbBbsContext.Tblbuses.FirstOrDefault(d => d.BusId == busId);
                if (deletebus != null)
                {
                    deletebus.IsActive = false;
                    deletebus.IsDelete = true;
                    _dbBbsContext.Tblbuses.Update(deletebus);

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
        // ✅ GET ALL BUSES
        // --------------------------------------------------------------------
        /// <summary>
        /// Retrieves all buses with their details.
        /// </summary>
        /// <returns>List of all buses in the system.</returns>
        public List<ResponseGetbuses> GetAllbuses()
        {
            try
            {
                return _dbBbsContext.Tblbuses
                    .Select(x => new ResponseGetbuses
                    {
                        BusId=x.BusId,
                        BusName = x.BusName,
                        Busnumber = x.Busnumber,
                        BusType = x.BusType,
                        TotalSeats = x.TotalSeats,
                        Fare = x.Fare,
                        OperatorName = x.OperatorName,
                        OperatorNumber = x.OperatorNumber,
                        IsActive = x.IsActive

                    }).ToList();
            }
            catch (Exception ex)
            {
                _errorHandler.Capture(ex, "An Error while Get Buses");
                throw new Exception(ex.Message);
            }
        }

        // --------------------------------------------------------------------
        // ✅ GET BUS COUNT
        // --------------------------------------------------------------------
        /// <summary>
        /// Retrieves the total number of buses in the system.
        /// </summary>
        /// <returns>Total bus count.</returns>
        public int GetBusesCount()
        {
           return _dbBbsContext.Tblbuses.Count();
        }

        // --------------------------------------------------------------------
        // ✅ UPDATE BUS DETAILS
        // --------------------------------------------------------------------
        /// <summary>
        /// Updates the details of an existing bus record in the system.
        /// </summary>
        /// <param name="updateBusesinfo">
        /// The DTO containing updated bus information such as name, type, and fare.
        /// </param>
        /// <returns>Status message indicating whether the update was successful.</returns>
        public string UpdateBuses(RequestUpdateBuses updateBusesinfo)
        {
            try
            {
                if (updateBusesinfo != null && updateBusesinfo.BusId > 0)
                {
                    var Buses = _dbBbsContext.Tblbuses.Include(x=> x.Tblbookings).ThenInclude(x=> x.User)
                        .FirstOrDefault(x => x.BusId == updateBusesinfo.BusId);
                    if (Buses != null)
                    {
                        Buses.BusId = updateBusesinfo.BusId;
                        Buses.BusName = updateBusesinfo.BusName;
                        Buses.Busnumber = updateBusesinfo.Busnumber;
                        Buses.BusType = updateBusesinfo.BusType;
                        Buses.TotalSeats = updateBusesinfo.TotalSeats;
                        Buses.Fare = updateBusesinfo.Fare;
                        Buses.OperatorName = updateBusesinfo.OperatorName;
                        Buses.OperatorNumber = updateBusesinfo.OperatorNumber;

                        _dbBbsContext.SaveChanges();

                        var modifiedby = _dbBbsContext.Tblbuses.FirstOrDefault();
                        if (modifiedby != null)
                        {
                            modifiedby.ModifiedAt = DateTime.Now;
                            modifiedby.ModifiedBy = updateBusesinfo.UserId;
                            _dbBbsContext.SaveChanges();
                        }

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
