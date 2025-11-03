using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace BusBookingSystem.Application
{
    public class ErrorHandler
    {
        private readonly string _logDirectory;

        public ErrorHandler(string logDirectory)
        {
            _logDirectory = logDirectory;
        }

        public void Capture(Exception ex, string context = "")
        {
            try
            {
                Directory.CreateDirectory(_logDirectory);

                string logFilePath = Path.Combine(_logDirectory, $"error_{DateTime.Now:yyyy-MM-dd}.txt");

                string errorMessage = FormatErrorMessage(ex, context);

                File.AppendAllText(logFilePath, errorMessage);
            }
            catch (Exception loggingEx)
            {
                Console.WriteLine($"Error logging failed: {loggingEx.Message}");
            }
        }

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
