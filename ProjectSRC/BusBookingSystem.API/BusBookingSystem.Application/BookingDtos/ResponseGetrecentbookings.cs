using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusBookingSystem.Application.BookingDtos
{
    public class ResponseGetrecentbookings
    {
        public int BookingId { get; set; }

        public int UserId { get; set; }
        public string Name { get; set; } = null!;

        public string Phone { get; set; } = null!;
        public string Busnumber { get; set; } = null!;

        public string BusName { get; set; } = null!;
        public DateTime? BookingDate { get; set; }
        public List<int> SeatIds { get; set; } = new List<int>();
        public List<string> SeatNumbers { get; set; } = new List<string>();

        public ICollection<Passangerinfo> passangers { get; set; }
        public decimal TotalFare { get; set; }
        public string? Status { get; set; }
        public bool? IsActive { get; set; }
        public int RouteId { get; set; }
        public string Source { get; set; }
        public string Destination { get; set; }
    }
}
