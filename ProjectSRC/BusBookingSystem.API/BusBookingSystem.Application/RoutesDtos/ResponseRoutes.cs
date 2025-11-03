using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusBookingSystem.Application.RoutesDtos
{
    public class ResponseRoutes
    {
        public int RouteId { get; set; }
        public string BusType { get; set; } = null!;
        public string BusName { get; set; } = null!;
        public string Source { get; set; } = null!;

        public string Destination { get; set; } = null!;

        public string Busnumber { get; set; } = null!;

        public TimeOnly Departuretime { get; set; }

        public TimeOnly Arrivaltime { get; set; }

        public decimal? Fare { get; set; }
        public decimal RatingValue { get; set; }
        public int? Busid { get; set; }
        public bool? IsActive { get; set; }
        public int TotalSeats { get; set; }
        public int BookedSeats { get; set; }
        public int AvailableSeats { get; set; }


        // ✅ Optional: List of booked seat numbers (for UI display)
        public List<string>? BookedSeatNumbers { get; set; }

        // ✅ Optional: List of available seat numbers (for UI display)
        public List<string>? AvailableSeatNumbers { get; set; }
        public DateTime TravelDate { get; set; }
    }
}
