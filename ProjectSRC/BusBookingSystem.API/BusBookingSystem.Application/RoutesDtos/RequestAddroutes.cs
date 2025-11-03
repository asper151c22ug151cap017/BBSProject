using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusBookingSystem.Application.RoutesDtos
{
    public class RequestAddRoutes
    {
        public int? createdBy;

        public string BusType { get; set; } = null!;
        public decimal? Fare { get; set; }
        public string BusName { get; set; } = null!;

        public string Busnumber { get; set; } = null!;
        public string Source { get; set; } = null!;

        public string Destination { get; set; } = null!;

        public DateOnly Journeydate { get; set; }

        public TimeOnly Departuretime { get; set; }

        public TimeOnly Arrivaltime { get; set; }
        public int? Busid { get; set; }
    }
}
