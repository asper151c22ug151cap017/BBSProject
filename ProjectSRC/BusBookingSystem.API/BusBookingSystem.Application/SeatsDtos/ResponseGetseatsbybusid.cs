using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusBookingSystem.Application.SeatsDtos
{
    public class ResponseGetseatsbybusid
    {
        public int UserId { get; set; }
        public int RouteId { get; set; }
        public int BusId { get; set; }
        public bool IsBooked { get; set; }
        public DateTime? TravelDate { get; set; }
        public int SeatId { get; set; }
        public string SeatNumber { get; set; } = null!;

  
    }
}
