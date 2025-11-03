
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusBookingSystem.Application.BookingDtos
{
    public class BusDto
    {
        
            public int BusId { get; set; }
            public string BusName { get; set; } = string.Empty;
            public string BusNumber { get; set; } = string.Empty;
        public string BusType { get; set; } = string.Empty;
        public List<RouteDto> Routes { get; set; } = new List<RouteDto>();

    }

    public class RouteDto
        {
             public int RouteId { get; set; }
             public string Source { get; set; } = string.Empty;
            public string Destination { get; set; } = string.Empty;
        }
    }

