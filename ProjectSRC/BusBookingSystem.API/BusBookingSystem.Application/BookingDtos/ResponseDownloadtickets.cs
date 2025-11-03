using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusBookingSystem.Application.BookingDtos
{
    public class ResponseDownloadtickets
    {

        public int BookingId { get; set; }       
      
        public DateTime? BookingDate { get; set; }
        public List<string> SeatNumber { get; set; }
        public decimal TotalFare { get; set; }
        public string? Status { get; set; }
        public List< Passangerinfo> passangers { get; set; }
        public Downloadbusdto Busesdto { get; set; }
        public DownloadRoutesDto RoutesDto { get; set; }
        public downloadUserinfo DownloadUserinfo { get; set; }


    }
    public class Downloadbusdto
    {
        public string Busnumber { get; set; } = null!;

        public string BusName { get; set; } = null!;
        public string BusType { get; set; } = null!;
        public string OperatorName { get; set; } = null!;

        public string OperatorNumber { get; set; } = null!;
    }
    public class DownloadRoutesDto
    {
        public string Source { get; set; } = null!;

        public string Destination { get; set; } = null!;
    }
  
    public class downloadUserinfo
    {
        public int UserId { get; set; }
        public string Name { get; set; } = null!;

        public string Phone { get; set; } = null!;

    }
}
