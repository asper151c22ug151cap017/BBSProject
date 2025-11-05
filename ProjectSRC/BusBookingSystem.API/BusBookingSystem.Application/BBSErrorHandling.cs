// ===================================================================================
// Project Name : BusBookingSystem
// File Name    : ErrorHandler.cs
// Created By   : Kaviraj M
// Created On   : 05/11/2025
// Modified By  : Kaviraj M
// Modified On  : 05/11/2025
// Description  : Provides a centralized error handling mechanism by capturing
//                exceptions and writing them to daily log files in the specified
//                directory. Ensures logging reliability and fallback on console.
// ===================================================================================

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace BusBookingSystem.Application
{
    /// <summary>
    /// Handles application exception logging for the Bus Booking System.
    /// </summary>
    public class ErrorHandler
    {
        // --------------------------------------------------------------------
        // 🔹 Log Directory Path
        // --------------------------------------------------------------------
        /// <summary>
        /// Directory path where the error log files will be stored.
        /// </summary>
        private readonly string _logDirectory;

        // --------------------------------------------------------------------
        // ✅ Constructor
        // --------------------------------------------------------------------
        /// <summary>
        /// Initializes a new instance of the <see cref="ErrorHandler"/> class
        /// with the specified log directory.
        /// </summary>
        /// <param name="logDirectory">
        /// The directory path where error log files will be created.
        /// </param>
        public ErrorHandler(string logDirectory)
        {
            _logDirectory = logDirectory;
        }

        // --------------------------------------------------------------------
        // ✅ Capture Exception
        // --------------------------------------------------------------------
        /// <summary>
        /// Captures an exception and writes its details to a log file.
        /// </summary>
        /// <param name="ex">
        /// The <see cref="Exception"/> object to capture and log.
        /// </param>
        /// <param name="context">
        /// Optional string describing the context or location where the exception occurred.
        /// </param>
        /// <remarks>
        /// Creates the log directory if it does not exist.
        /// Appends exception details to a daily log file.
        /// If logging fails, writes the error message to the console as a fallback.
        /// </remarks>
        public void Capture(Exception ex, string context = "")
        {
            try
            {
                // Ensure log directory exists
                Directory.CreateDirectory(_logDirectory);

                // Build daily log file path
                string logFilePath = Path.Combine(_logDirectory, $"error_{DateTime.Now:yyyy-MM-dd}.txt");

                // Format exception details
                string errorMessage = FormatErrorMessage(ex, context);

                // Append the formatted message to the log file
                File.AppendAllText(logFilePath, errorMessage);
            }
            catch (Exception loggingEx)
            {
                // Fallback if logging fails
                Console.WriteLine($"Error logging failed: {loggingEx.Message}");
            }
        }
        // --------------------------------------------------------------------
        // ✅ Format Error Message
        // --------------------------------------------------------------------
        /// <summary>
        /// Formats an exception into a detailed string containing timestamp, context,
        /// exception type, message, and stack trace.
        /// </summary>
        /// <param name="ex">
        /// The <see cref="Exception"/> to format.
        /// </param>
        /// <param name="context">
        /// Optional context information describing where the exception occurred.
        /// </param>
        /// <returns>
        /// A formatted string containing all exception details ready to write to a log file.
        /// </returns>
        private string FormatErrorMessage(Exception ex, string context)
        {
            return $@"
=============================
Timestamp: {DateTime.Now:yyyy-MM-dd HH:mm:ss}
Context: {context}
Exception: {ex.GetType().FullName}
Message: {ex.Message}
StackTrace: {ex.StackTrace}
=============================
";
        }
    }
}
