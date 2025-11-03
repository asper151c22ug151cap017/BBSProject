using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusBookingSystem.Application.BookingDtos
{
    public class Requestupdatebooking
    {
        public int BookingId { get; set; }
        public decimal TotalFare { get; set; }
        public DateTime? BookingDate { get; set; }
        public List<string>? SeatNumbers { get; set; }
        public string? Status { get; set; }

    }

  
}
