// ============================================================================
// Project Name : BusBookingSystem
// File Name    : BBSAuth.cs
// Created By   : Kaviraj M
// Created On   : 19/099/2025
// Modified By  : Kaviraj M
// Modified On  : 28/10/2025
// Description  : Handles authentication logic for user login and password 
//                validation within the Bus Booking System.
// ============================================================================

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusBookingSystem.Application;
using BusBookingSystem.Domain.Models;
using BusBookingSystem.Infrastructure.RepositoryInterface;
using Microsoft.EntityFrameworkCore;

namespace BusBookingSystem.Infrastructure.RepositoryImplementation
{

    /// <summary>
    /// Repository implementation for authentication-related operations.
    /// </summary>
    public class BBSAuth : IBBSAuth
    {
        private readonly DbBbsContext _dbBbsContext;

        /// <summary>
        /// Initializes a new instance of the <see cref="BBSAuth"/> class.
        /// </summary>
        /// <param name="dbBbsContext">Database context for accessing user data.</param>

        public BBSAuth(DbBbsContext dbBbsContext)
        {
            _dbBbsContext = dbBbsContext;
        }

        // --------------------------------------------------------------------
        // ✅ USER LOGIN
        // --------------------------------------------------------------------
        /// <summary>
        /// Authenticates a user based on email and password.
        /// </summary>
        /// <param name="email">The user's email address.</param>
        /// <param name="password">The user's password (plain text input).</param>
        /// <returns>
        /// Returns the <see cref="Tbluser"/> object if authentication is successful;
        /// otherwise, returns null.
        /// </returns>
        public async Task <Tbluser> Login(string email, string password)
        {
            var user = await _dbBbsContext.Tblusers.FirstOrDefaultAsync(e => e.Email == email);
            if (user == null)
                return null;


            bool isValid = BBSHashCode.VerifyPassword(password, user.Password);
            if (!isValid)
                return null;

            return user;
        }
    }
}
