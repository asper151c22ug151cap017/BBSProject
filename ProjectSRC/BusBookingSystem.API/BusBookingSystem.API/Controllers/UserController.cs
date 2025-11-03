// ============================================================================
// Project Name : BusBookingSystem
// File Name    : UserController.cs
// Created By   : [Your Name]
// Created On   : [Date]
// Modified By  : [Your Name]
// Modified On  : [Date]
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

        // --------------------------------------------------------------------
        // 🔹 Constructor
        // --------------------------------------------------------------------
        /// <summary>
        /// Initializes a new instance of the <see cref="UserController"/> class.
        /// </summary>
        /// <param name="bbsUser">Repository interface for user management.</param>
        public UserController(IBBSUser bbsUser)
        {
            _bbsUser = bbsUser;
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
        public IActionResult GetAllUsers()
        {
            return Ok(_bbsUser.Getusers());
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
        public IActionResult AddUser([FromBody] RequestAdduser addUser)
        {
            addUser.Password = BBSHashCode.HashcodePassword(addUser.Password);
            return Ok(_bbsUser.AddUsers(addUser));
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
        [AllowAnonymous]
        public IActionResult UpdateUser([FromBody] RequestUpdateUser updateUserInfo)
        {
            return Ok(_bbsUser.UpdateUser(updateUserInfo));
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
        public IActionResult DeleteUser([FromQuery] int userId)
        {
            return Ok(_bbsUser.DeleteUser(userId));
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
        public IActionResult GetUserCount()
        {
            return Ok(_bbsUser.GetUserCount());
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
        public IActionResult GetUserById([FromQuery] int userId)
        {
            return Ok(_bbsUser.GetUserbyId(userId));
        }

        [HttpPut]
        [Route("UpdateUserProfile")]
        public async Task<IActionResult> UpdateUserProfile([FromBody] RequestUserUpdateDto updateUserProfile)
        {
            if (updateUserProfile == null)
                return BadRequest("Invalid user data.");

            var result = await _bbsUser.UpdateUserprofileAsync(updateUserProfile);

            return Ok(result);
        }

    }
}
