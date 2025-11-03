using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusBookingSystem.Application.SeatsDtos
{
    public class ResponseGetSeats
    {
  
        public int BusId { get; set; }
        public int SeatId { get; set; }
        public string SeatNumber { get; set; } = null!;

      
    }
}
