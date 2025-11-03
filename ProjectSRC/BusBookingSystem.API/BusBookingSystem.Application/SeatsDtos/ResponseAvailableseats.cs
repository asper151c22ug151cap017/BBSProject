using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusBookingSystem.Application.SeatsDtos
{
    public class ResponseAvailableseats
    {
        public int BusId { get; set; }
       
        public string Date { get; set; } = string.Empty;
       
       
        public int TotalSeats { get; set; }
        public int BookedSeats { get; set; }
        public List<seatsinfo> AvailableSeats { get; set; } = new List<seatsinfo>();
    }

    public class seatsinfo
    {
        public int SeatId { get; set; }
        public string SeatNumber { get; set; } = null!;

    }
}
