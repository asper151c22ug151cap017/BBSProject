using System;
using System.Collections.Generic;

namespace BusBookingSystem.Domain.Models;

public partial class Tblbuse
{
    public int BusId { get; set; }

    public string Busnumber { get; set; } = null!;

    public string OperatorName { get; set; } = null!;

    public string OperatorNumber { get; set; } = null!;

    public string BusName { get; set; } = null!;

    public string BusType { get; set; } = null!;

    public int? TotalSeats { get; set; }

    public decimal? Fare { get; set; }

    public DateTime? CreatedAt { get; set; }

    public int? CreatedBy { get; set; }

    public DateTime? ModifiedAt { get; set; }

    public int? ModifiedBy { get; set; }

    public bool? IsActive { get; set; }

    public bool? IsDelete { get; set; }

    public int? Ratingid { get; set; }

    public virtual Tbluser? CreatedByNavigation { get; set; }

    public virtual Tbluser? ModifiedByNavigation { get; set; }

    public virtual Tblbusrating? Rating { get; set; }

    public virtual ICollection<Tblbooking> Tblbookings { get; set; } = new List<Tblbooking>();

    public virtual ICollection<Tblbookingseat> Tblbookingseats { get; set; } = new List<Tblbookingseat>();

    public virtual ICollection<Tblbusrating> Tblbusratings { get; set; } = new List<Tblbusrating>();

    public virtual ICollection<Tblroute> Tblroutes { get; set; } = new List<Tblroute>();

    public virtual ICollection<Tblseat> Tblseats { get; set; } = new List<Tblseat>();
}
