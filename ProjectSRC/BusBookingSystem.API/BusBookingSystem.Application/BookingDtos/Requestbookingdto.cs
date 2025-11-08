using System;
using System.Collections.Generic;

namespace BusBookingSystem.Application.BookingDtos
{
    public class Requestbookingdto
    {
        public int UserId { get; set; }
        public int BusId { get; set; }
        public int RouteId { get; set; }
        public int SeatId { get; set; }
        public int BookingId { get; set; }
        public DateTime? BookingDate { get; set; }
        public int? CreatedBy { get; set; }

        public decimal TotalFare { get; set; }

        // Optional, backend can set default
        public string? Status { get; set; }

        // Store multiple passenger IDs as comma-separated string
        public string PassengerIds { get; set; } = string.Empty;

        public List<string> SeatNumbers { get; set; }

        public List<int> SeatIds { get; set; } = new List<int>();
        public List<Addpassengers>? Passangers { get; set; }
    }

}
