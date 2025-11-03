using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusBookingSystem.Application.BookingDtos
{
    public class Responsegetbooking
    {
        public int BookingId { get; set; }

        public DateTime? BookingDate { get; set; }

        public decimal TotalFare { get; set; }

        public List<Responsepassangeinfo> Passengers { get; set; }
        public UserinfoDto Users { get; set; }
        public RoutesDto Routes { get; set; }
        public BusesDto buses { get; set; }
        public List<Seatsdto> seatsdto { get; set; }
        public string? Status { get; set; }
        public bool? IsActive { get; set; }
    }

    public class Responsepassangeinfo
    {
        public int PassengerId { get; set; }
        public string PassengerName { get; set; } = null!;

        public int PassengerAge { get; set; }

        public string PassengerGender { get; set; } = null!;
    }

    public class BusesDto
    {
        public int BusId { get; set; }
        public string Busnumber { get; set; } = null!;

        public string OperatorName { get; set; } = null!;

        public string OperatorNumber { get; set; } = null!;

        public string BusName { get; set; } = null!;

        public string BusType { get; set; } = null!;


    }

    public class UserinfoDto
    {
        public int UserId { get; set; }
        public string Name { get; set; } = null!;

        public string Phone { get; set; } = null!;

    }

    public class RoutesDto
    {
        public int RouteId { get; set; }
        public string Source { get; set; } = null!;

        public string Destination { get; set; } = null!;
    }

    public class Seatsdto
    {
        public List<int> SeatIds { get; set; } = new List<int>();
        public List<string> SeatNumbers { get; set; } = new List<string>();
    }
}
