using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusBookingSystem.Application.BookingDtos
{
    public class RequestAddbookings
    {


        public int? CreatedBy { get; set; }

        public decimal TotalFare { get; set; }

        public string? Status { get; set; }
    }
}
