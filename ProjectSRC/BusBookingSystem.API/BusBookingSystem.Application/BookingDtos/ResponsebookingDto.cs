using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusBookingSystem.Application.BookingDtos
{
    public class ResponsebookingDto
    {

        public int BookingId { get; set; }
        public string Message { get; set; }
        public DateTime? BookingDate { get; set; }    
        public List<int> SeatIds { get; set; } = new List<int>();
        public List<string> SeatNumbers { get; set; } = new List<string>();
    }

}

