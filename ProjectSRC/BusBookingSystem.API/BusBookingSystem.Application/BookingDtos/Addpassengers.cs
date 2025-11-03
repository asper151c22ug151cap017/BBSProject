using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusBookingSystem.Application.BookingDtos
{
    public class Addpassengers
    {
        public int BookingId { get; set; }
        public string PassengerName { get; set; } = null!;

        public int PassengerAge { get; set; }

        public string PassengerGender { get; set; } = null!;
    }
  
}
