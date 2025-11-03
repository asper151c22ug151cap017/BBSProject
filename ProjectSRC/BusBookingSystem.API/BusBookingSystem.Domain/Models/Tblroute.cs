using System;
using System.Collections.Generic;

namespace BusBookingSystem.Domain.Models;

public partial class Tblroute
{
    public int RouteId { get; set; }

    public string Source { get; set; } = null!;

    public string Destination { get; set; } = null!;

    public TimeOnly Departuretime { get; set; }

    public TimeOnly Arrivaltime { get; set; }

    public DateTime? CreatedAt { get; set; }

    public int? CreatedBy { get; set; }

    public DateTime? ModifiedAt { get; set; }

    public int? ModifiedBy { get; set; }

    public bool? IsActive { get; set; }

    public bool? IsDelete { get; set; }

    public int? BusId { get; set; }

    public virtual Tblbuse? Bus { get; set; }

    public virtual Tbluser? CreatedByNavigation { get; set; }

    public virtual Tbluser? ModifiedByNavigation { get; set; }

    public virtual ICollection<Tblbooking> Tblbookings { get; set; } = new List<Tblbooking>();
}
