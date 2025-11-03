// ========================================================================================================
// Project Name : BusBookingSystem
// File Name    : IBBSUser.cs
// Created By   : Kaviraj M
// Created On   : 19/09/2025
// Modified By  : Kaviraj M
// Modified On  : 28/10/2025
// Description  : Interface definition for managing user-related operations in the Bus Booking System.
//                Provides method contracts for CRUD operations, user retrieval, and user count tracking.
// =========================================================================================================

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusBookingSystem.Application.User;
using BusBookingSystem.Application.UserDto;

namespace BusBookingSystem.Infrastructure.RepositoryInterface
{
    /// <summary>
    /// Defines the contract for user management operations.
    /// Handles CRUD operations, user retrieval, and user count tracking.
    /// </summary>
    public interface IBBSUser
    {
        /// <summary>
        /// Retrieves all users from the database.
        /// </summary>
        /// <returns>List of users mapped to response DTOs.</returns>
        List<ResponseGetusersDto> Getusers();


        /// <summary>
        /// Adds a new user record to the system.
        /// </summary>
        /// <param name="addUser">User details to add.</param>
        /// <returns>Status message.</returns>
        string AddUsers(RequestAdduser adduser);

        /// <summary>
        /// Updates an existing user's details.
        /// </summary>
        /// <param name="updateUserInfo">Updated user data.</param>
        /// <returns>Status message.</returns>
        string UpdateUser(RequestUpdateUser UpdateUserinfo);


        /// <summary>
        /// Soft deletes a user by updating status flags.
        /// </summary>
        /// <param name="userId">Unique ID of the user to delete.</param>
        /// <returns>Status message.</returns>
        string DeleteUser(int UserId);


        /// <summary>
        /// Retrieves the total number of users in the database.
        /// </summary>
        /// <returns>Total user count.</returns>
        int GetUserCount();

        /// <summary>
        /// Retrieves user details by user ID.
        /// </summary>
        /// <param name="userId">Unique user ID.</param>
        /// <returns>User details mapped to response DTOs.</returns>
        List<ResponseuserbyidDto> GetUserbyId(int Userid);

        Task<string> UpdateUserprofileAsync(RequestUserUpdateDto userUpdateDto);

    }
}
