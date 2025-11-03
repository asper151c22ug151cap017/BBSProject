// ================================================================================================================
// Project Name : BusBookingSystem
// File Name    : BBSUser.cs
// Created By   : Kaviraj M
// Created On   : 19/09/2025
// Modified By  : Kaviraj M
// Modified On  : 28/10/2025
// Description  :  This class (BBSUser) handles all user-related operations in the Bus Booking System.
//                 It performs CRUD actions, user count retrieval, and manages user data using Entity Framework.
//                 Includes proper error handling, soft delete, and audit fields (CreatedBy, ModifiedBy, etc.).
// ===============================================================================================================
using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using BusBookingSystem.Application;
using BusBookingSystem.Application.User;
using BusBookingSystem.Application.UserDto;
using BusBookingSystem.Domain.Models;
using BusBookingSystem.Infrastructure.RepositoryInterface;
using Microsoft.EntityFrameworkCore;

namespace BusBookingSystem.Infrastructure.RepositoryImplementation
{
    /// <summary>
    /// Repository implementation for managing user-related operations.
    /// Handles CRUD operations, user count, and data retrieval.
    /// </summary>
    public class BBSUser : IBBSUser
    {
        private readonly DbBbsContext _dbContext;
        private readonly ErrorHandler _errorHandler;
        private readonly IMapper _mapper;
        /// <summary>
        /// Initializes a new instance of the <see cref="BBSUser"/> class.
        /// </summary>
        /// <param name="dbBbsContext">Database context for Entity Framework operations.</param>
        /// <param name="errorHandler">Custom error handler for exception management.</param>
        public BBSUser(DbBbsContext dbBbsContext, ErrorHandler errorHandler, IMapper mapper)
        {
            _dbContext = dbBbsContext;
            _errorHandler = errorHandler;
            _mapper = mapper;
        }


        // --------------------------------------------------------------------
        // ✅ GET ALL USERS
        // --------------------------------------------------------------------
        /// <summary>
        /// Retrieves all active users from the database.
        /// </summary>
        /// <returns>List of users mapped to Response DTOs.</returns>
        public List<ResponseGetusersDto> Getusers()
        {
            try
            {
                return _dbContext.Tblusers
                    .Select(x => new ResponseGetusersDto
                    {
                        UserId = x.UserId,
                        Name = x.Name,
                        Email = x.Email,
                        Phone = x.Phone,
                        Age = x.Age,
                        GenderId = x.GenderId,
                        IsActive = x.IsActive,
                        RoleId = x.RoleId
                    }).ToList();
            }
            catch (Exception ex)
            {
                _errorHandler.Capture(ex, "Error while fetching user list.");
                throw new Exception("An error occurred while retrieving users.", ex);
            }
        }

        // --------------------------------------------------------------------
        // ✅ ADD USER
        // --------------------------------------------------------------------
        /// <summary>
        /// Adds a new user to the system after validation.
        /// </summary>
        /// <param name="addUser">The user details to add.</param>
        /// <returns>Result message.</returns>
        public string AddUsers(RequestAdduser addUser)
        {
            try
            {
                if (addUser == null)
                    return "Invalid input.";

                bool existingUser = _dbContext.Tblusers.Any(e => e.Email == addUser.Email);
                if (existingUser)
                    throw new Exception("Email already exists.");

                var now = DateTime.Now;

                var newUser = new Tbluser
                {
                    Name = addUser.Name,
                    Email = addUser.Email,
                    Password = addUser.Password,
                    Phone = addUser.Phone,
                    Age = addUser.Age,
                    GenderId = addUser.GenderId,
                    RoleId = 2, // Default role: User
                    CreatedAt = now,
                    ModifiedAt = now,
                    IsActive = true,
                    IsDelete = false
                };

                _dbContext.Tblusers.Add(newUser);
                _dbContext.SaveChanges();

                // Update CreatedBy and ModifiedBy with the new user's ID
                newUser.CreatedBy = newUser.UserId;
                newUser.ModifiedBy = newUser.UserId;
                _dbContext.SaveChanges();

                return "User added successfully.";
            }
            catch (Exception ex)
            {
                _errorHandler.Capture(ex, "Error while adding a new user.");
                throw new Exception("An error occurred while adding a user.", ex);
            }
        }

        // --------------------------------------------------------------------
        // ✅ UPDATE USER
        // --------------------------------------------------------------------
        /// <summary>
        /// Updates an existing user's details.
        /// </summary>
        /// <param name="updateUserInfo">The updated user data.</param>
        /// <returns>Result message.</returns>
        public string UpdateUser(RequestUpdateUser updateUserInfo)
        {
            try
            {
                if (updateUserInfo == null || updateUserInfo.UserId <= 0)
                    return "Invalid UserId.";

                var user = _dbContext.Tblusers.FirstOrDefault(x => x.UserId == updateUserInfo.UserId);
                if (user == null)
                    return "User not found.";

                _mapper.Map(updateUserInfo, user);

                _dbContext.Tblusers.Update(user);
                 
                _dbContext.SaveChanges();
                return "User updated successfully.";
            }
            catch (Exception ex)
            {
                _errorHandler.Capture(ex, "Error while updating user details.");
                throw new Exception("An error occurred while updating the user.", ex);
            }
        }


        // --------------------------------------------------------------------
        // ✅ DELETE USER (Soft Delete)
        // --------------------------------------------------------------------
        /// <summary>
        /// Soft deletes a user by setting IsActive and IsDelete flags.
        /// </summary>
        /// <param name="userId">The ID of the user to delete.</param>
        /// <returns>Result message.</returns>
        public string DeleteUser(int userId)
        {
            try
            {
                var user = _dbContext.Tblusers.FirstOrDefault(u => u.UserId == userId);
                if (user == null)
                    return "User not found.";

                user.IsActive = false;
                user.IsDelete = true;
                user.ModifiedAt = DateTime.Now;
                user.ModifiedBy = userId;

                _dbContext.Tblusers.Update(user);
                _dbContext.SaveChanges();

                return "User deleted successfully.";
            }
            catch (Exception ex)
            {
                _errorHandler.Capture(ex, "Error while deleting user.");
                throw new Exception("An error occurred while deleting the user.", ex);
            }
        }


        // --------------------------------------------------------------------
        // ✅ USER COUNT
        // --------------------------------------------------------------------
        /// <summary>
        /// Retrieves the total number of users in the database.
        /// </summary>
        /// <returns>User count.</returns>
        public int GetUserCount()
        {
            try
            {
                return _dbContext.Tblusers.Count();
            }
            catch (Exception ex)
            {
                _errorHandler.Capture(ex, "Error while counting users.");
                throw new Exception("An error occurred while counting users.", ex);
            }
        }

        // --------------------------------------------------------------------
        // ✅ GET USER BY ID
        // --------------------------------------------------------------------
        /// <summary>
        /// Retrieves a user by their unique ID.
        /// </summary>
        /// <param name="userId">The user ID.</param>
        /// <returns>User details mapped to a response DTO.</returns>
        public List<ResponseuserbyidDto> GetUserbyId(int userId)
        {
            try
            {
                return _dbContext.Tblusers
                    .Where(x => x.UserId == userId)
                    .ProjectTo<ResponseuserbyidDto>(_mapper.ConfigurationProvider).ToList();
            }
            catch (Exception ex)
            {
                _errorHandler.Capture(ex, "Error while fetching user by ID.");
                throw new Exception("An error occurred while retrieving user details.", ex);
            }
        }

        public async Task<string> UpdateUserprofileAsync(RequestUserUpdateDto userUpdateDto)
        {
            try
            {
                if (userUpdateDto == null || userUpdateDto.UserId <= 0)
                    return "Invalid user data.";

                var user = await _dbContext.Tblusers
                    .FirstOrDefaultAsync(x => x.UserId == userUpdateDto.UserId);

                if (user == null)
                    return "User not found.";

                // ✅ Update fields
                user.Name = userUpdateDto.Name;
                user.Email = userUpdateDto.Email;
                user.Phone = userUpdateDto.Phone;
                user.ModifiedAt = DateTime.UtcNow;
                user.ModifiedBy = userUpdateDto.UserId;

                await _dbContext.SaveChangesAsync();

                return "User profile updated successfully.";
            }
            catch (Exception ex)
            {
                _errorHandler.Capture(ex, "Error while updating user profile.");
                throw new Exception("An unexpected error occurred while updating the user profile.", ex);
            }
        }
    }
}
