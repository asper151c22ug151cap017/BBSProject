// ============================================================================
// Project Name : BusBookingSystem
// File Name    : UserController.cs
// Created By   : Kaviraj M
// Created On   : 19/09/2025
// Modified By  : Kaviraj M
// Modified On  : 05/11/2025
// Description  : Handles all user-related operations such as registration,
//                retrieval, updating, deletion, and user count management
//                within the Bus Booking System.
// ============================================================================

using BusBookingSystem.Application;
using BusBookingSystem.Application.UserDto;
using BusBookingSystem.Infrastructure.RepositoryInterface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BusBookingSystem.API.Controllers
{
    /// <summary>
    /// Provides endpoints for managing user data including registration,
    /// updates, deletion, and retrieval operations.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        // --------------------------------------------------------------------
        // 🔹 Private Field
        // --------------------------------------------------------------------
        private readonly IBBSUser _bbsUser;
        private readonly ErrorHandler _errorHandler;

        // --------------------------------------------------------------------
        // 🔹 Constructor
        // --------------------------------------------------------------------
        /// <summary>
        /// Initializes a new instance of the <see cref="UserController"/> class.
        /// </summary>
        /// <param name="bbsUser">Repository interface for user management.</param>
        public UserController(IBBSUser bbsUser, ErrorHandler errorHandler)
        {
            _bbsUser = bbsUser;
            _errorHandler = errorHandler;
        }

        // --------------------------------------------------------------------
        // ✅ GET ALL USERS
        // --------------------------------------------------------------------
        /// <summary>
        /// Retrieves all registered users.
        /// </summary>
        /// <returns>List of all users.</returns>
        [HttpGet]
        [Route("GetAllUsers")]
        [Authorize]
        public async Task<IActionResult> GetAllUsers()
        {
            try
            {
                return Ok(await _bbsUser.Getusers()); 
            }
            catch (Exception ex)
            {
                // You can log the error
                _errorHandler.Capture(ex, "Error fetching all users");

                // Return proper response
                return StatusCode(500, new { message = "Something went wrong", error = ex.Message });
            }
        }

        // --------------------------------------------------------------------
        // ✅ ADD USER (REGISTER)
        // --------------------------------------------------------------------
        /// <summary>
        /// Registers a new user with hashed password for security.
        /// </summary>
        /// <param name="addUser">User registration details.</param>
        /// <returns>Status message indicating registration result.</returns>
        [HttpPost]
        [Route("AddUser")]
        [AllowAnonymous]
        public async Task<IActionResult> AddUser([FromBody] RequestAdduser addUser)
        {
            try

            {
                addUser.Password = BBSHashCode.HashcodePassword(addUser.Password);
                return Ok(await _bbsUser.AddUsers(addUser));
            }
            catch (Exception ex)
            {
                // You can log the error
                _errorHandler.Capture(ex, "Error Adding Userinformation");

                // Return proper response
                return StatusCode(500, new { message = "Something went wrong", error = ex.Message });
            }
        }

        // --------------------------------------------------------------------
        // ✅ UPDATE USER
        // --------------------------------------------------------------------
        /// <summary>
        /// Updates existing user details.
        /// </summary>
        /// <param name="updateUserInfo">Updated user information DTO.</param>
        /// <returns>Status message indicating update result.</returns>
        [HttpPut]
        [Route("UpdateUser")]
        [Authorize]
        public async Task<IActionResult> UpdateUser([FromBody] RequestUpdateUser updateUserInfo)
        {
            try
            {
                return Ok(await _bbsUser.UpdateUser(updateUserInfo));
            }
            catch (Exception ex)
            {
                // You can log the error
                _errorHandler.Capture(ex, "Error Update Userinformation ");

                // Return proper response
                return StatusCode(500, new { message = "Something went wrong", error = ex.Message });
            }
        }

        // --------------------------------------------------------------------
        // ✅ DELETE USER
        // --------------------------------------------------------------------
        /// <summary>
        /// Deletes a user account based on the provided user ID.
        /// </summary>
        /// <param name="userId">Unique identifier of the user to delete.</param>
        /// <returns>Status message indicating deletion result.</returns>
        [HttpDelete]
        [Route("DeleteUser")]
        [Authorize]
        public async Task<IActionResult> DeleteUser([FromQuery] int userId)
        {
            try
            {
                return Ok(await _bbsUser.DeleteUser(userId));
            }
            catch (Exception ex)
            {
                // You can log the error
                _errorHandler.Capture(ex, "Error Delete user information ");

                // Return proper response
                return StatusCode(500, new { message = "Something went wrong", error = ex.Message });
            }
        }

        // --------------------------------------------------------------------
        // ✅ GET USER COUNT
        // --------------------------------------------------------------------
        /// <summary>
        /// Retrieves the total count of registered users.
        /// </summary>
        /// <returns>Total number of users.</returns>
        [HttpGet]
        [Route("GetUserCount")]
        [Authorize]
        public async Task<IActionResult> GetUserCount()
        {
            try
            {
                return Ok(await _bbsUser.GetUserCount());

            }
            catch (Exception ex)
            {
                // You can log the error
                _errorHandler.Capture(ex, "Error fetching User counts ");

                // Return proper response
                return StatusCode(500, new { message = "Something went wrong", error = ex.Message });
            }

        }

        // --------------------------------------------------------------------
        // ✅ GET USER BY ID
        // --------------------------------------------------------------------
        /// <summary>
        /// Retrieves detailed information of a specific user by their unique ID.
        /// </summary>
        /// <param name="userId">Unique identifier of the user.</param>
        /// <returns>User details for the specified ID.</returns>
        [HttpGet]
        [Route("GetUserById")]
        [Authorize]
        public async Task<IActionResult> GetUserById([FromQuery] int userId)
        {
            try
            {
                return Ok(await _bbsUser.GetUserbyId(userId));
            }
            catch (Exception ex)
            {
                // You can log the error
                _errorHandler.Capture(ex, $"Error fetching User information by :{userId}");

                // Return proper response
                return StatusCode(500, new { message = "Something went wrong", error = ex.Message });
            }

        }

        [HttpPut]
        [Route("UpdateUserProfile")]
        [Authorize]
        public async Task<IActionResult> UpdateUserProfile([FromBody] RequestUserUpdateDto updateUserProfile)
        {
            try
            {
                if (updateUserProfile == null)
                    return BadRequest("Invalid user data.");

                var result = await _bbsUser.UpdateUserprofileAsync(updateUserProfile);

                return Ok(result);
            }
            catch (Exception ex)
            {
                // You can log the error
                _errorHandler.Capture(ex, "Error update user profile information by user");

                // Return proper response
                return StatusCode(500, new { message = "Something went wrong", error = ex.Message });
            }

        }

    }
}
