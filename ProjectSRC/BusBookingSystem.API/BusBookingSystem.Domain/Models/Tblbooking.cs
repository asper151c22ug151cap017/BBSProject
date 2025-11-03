using System;
using System.Collections.Generic;

namespace BusBookingSystem.Domain.Models;

public partial class Tblbooking
{
    public int BookingId { get; set; }

    public int UserId { get; set; }

    public int BusId { get; set; }

    public int RouteId { get; set; }

    public DateTime? BookingDate { get; set; }

    public decimal TotalFare { get; set; }

    public string? Status { get; set; }

    public DateTime? CreatedAt { get; set; }

    public int? CreatedBy { get; set; }

    public DateTime? ModifiedAt { get; set; }

    public int? ModifiedBy { get; set; }

    public bool? IsActive { get; set; }

    public bool? IsDelete { get; set; }

    public virtual Tblbuse Bus { get; set; } = null!;

    public virtual Tbluser? CreatedByNavigation { get; set; }

    public virtual Tbluser? ModifiedByNavigation { get; set; }

    public virtual Tblroute Route { get; set; } = null!;

    public virtual ICollection<Tblbookingseat> Tblbookingseats { get; set; } = new List<Tblbookingseat>();

    public virtual ICollection<Tblpassenger> Tblpassengers { get; set; } = new List<Tblpassenger>();

    public virtual Tbluser User { get; set; } = null!;
}
