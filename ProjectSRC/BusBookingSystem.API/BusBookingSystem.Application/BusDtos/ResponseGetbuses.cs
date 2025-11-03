using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusBookingSystem.Application.BusDtos
{
    public class ResponseGetbuses
    {
        public int BusId { get; set; }
        public string Busnumber { get; set; } = null!;

        public string OperatorName { get; set; } = null!;

        public string OperatorNumber { get; set; } = null!;

        public string BusName { get; set; } = null!;

        public string BusType { get; set; } = null!;

        public int? TotalSeats { get; set; }

        public decimal? Fare { get; set; }
        public bool? IsActive { get; set; }

    }
}
