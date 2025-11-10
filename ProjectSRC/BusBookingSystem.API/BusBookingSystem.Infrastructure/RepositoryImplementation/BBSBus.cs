// ============================================================================
// Project Name : BusBookingSystem
// File Name    : BBSBus.cs
// Created By   : Kaviraj M
// Created On   : 20/09/2025
// Modified By  : kaviraj M
// Modified On  : 05/11/2025
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
        public async Task<string> AddBuses(RequestAddBuses addBuses)
        {
            try
            {
                var now = DateTime.Now;
                if (addBuses == null)
                    return Messages.User.vaild;
                var existingBuses = await _dbBbsContext.Tblbuses.Include(x=> x.Tblbookings).ThenInclude(x=> x.User)
                    .AnyAsync(y => y.Busnumber == addBuses.Busnumber && y.OperatorNumber == addBuses.OperatorNumber);
                if (existingBuses)
                    return Messages.BusMessage.ValidateBusinfo;
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
                    CreatedBy = addBuses.CreatedBy
                };

                _dbBbsContext.Tblbuses.Add(NewBuses);
                await _dbBbsContext.SaveChangesAsync();

               
                return Messages.BusMessage.Added;
            }
            catch (Exception ex)
            {
                _errorHandler.Capture(ex,Messages.BusMessage.Exceptionaddbus);
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
        public async Task<string> DeleteBuses(int busId)
        {
            try
            {
                var deletebus = await _dbBbsContext.Tblbuses.FirstOrDefaultAsync(d => d.BusId == busId);
                if (deletebus != null)
                {
                    deletebus.IsActive = false;
                    deletebus.IsDelete = true;
                    _dbBbsContext.Tblbuses.Update(deletebus);

                    await _dbBbsContext.SaveChangesAsync();
                    return Messages.BusMessage.Delete;
                }
                else
                {
                    return Messages.User.NotFound;
                }
            }
            catch (Exception ex)
            {
                _errorHandler.Capture(ex, Messages.BusMessage.Exceptiondeletebus);
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
        public async Task<List<ResponseGetbuses>> GetAllbuses()
        {
            try
            {
                return await _dbBbsContext.Tblbuses
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

                    }).ToListAsync();
            }
            catch (Exception ex)
            {
                _errorHandler.Capture(ex, Messages.BusMessage.ExceptionGetbus);
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
        public async Task<int> GetBusesCount()
        {
           return await _dbBbsContext.Tblbuses.CountAsync();
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
        public async Task<string> UpdateBuses(RequestUpdateBuses updateBusesinfo)
        {
            try
            {
                if (updateBusesinfo != null && updateBusesinfo.BusId > 0)
                {
                    var Buses = await _dbBbsContext.Tblbuses.Include(x=> x.Tblbookings).ThenInclude(x=> x.User)
                        .FirstOrDefaultAsync(x => x.BusId == updateBusesinfo.BusId);
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
                        Buses.ModifiedAt = DateTime.Now;
                        Buses.ModifiedBy = updateBusesinfo.ModifiedBy;


                        await _dbBbsContext.SaveChangesAsync();

                    }
                }
                return Messages.BusMessage.Updated;

            }
            catch (Exception ex)
            {
                _errorHandler.Capture(ex,Messages.BusMessage.ExceptionUpdatebus);
                throw new Exception(ex.Message);

            }
        }
    }
}
