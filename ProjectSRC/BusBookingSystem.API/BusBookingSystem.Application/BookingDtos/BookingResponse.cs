using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusBookingSystem.Application.BookingDtos
{
    public class BookingResponse
    {
        public List<int> SeatIds { get; set; } = new List<int>();
        public List<string> SeatNumbers { get; set; } = new List<string>();

        public int? BusId { get; set; }
        public int? UserId { get; set; }
        public int? RouteId { get; set; }

        public DateTime? BookingDate { get; set; }

        public decimal TotalFare { get; set; }

        public string? Status { get; set; }

        // Success or error message
        public int? BookingId { get; set; }       // ID of the newly created booking
         public BusInfoDto? tblbuses { get; set; }

        public ICollection<Passangerinfo> passangers { get; set; }
// ✅ this works  
        public bool? IsActive { get; set; }
    }
    public class BusInfoDto
    {
        public int BusId { get; set; }
        public string BusName { get; set; } = null!;

        public string Busnumber { get; set; } = null!;
        public string BusType { get; set; } = null!;

        public ICollection<RoutesinfoDto> routesinfo { get; set; }

    }
    public class RoutesinfoDto
    {
        public int RouteId { get; set; }

        public string Source { get; set; } = null!;

        public string Destination { get; set; } = null!;

       
    }

    public class Passangerinfo
{
       
        public int PassengerId { get; set; }
        public string PassengerName { get; set; } = null!;

        public int PassengerAge { get; set; }

        public string PassengerGender { get; set; } = null!;
    }

}
