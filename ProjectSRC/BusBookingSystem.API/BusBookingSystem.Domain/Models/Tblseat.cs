using System;
using System.Collections.Generic;

namespace BusBookingSystem.Domain.Models;

public partial class Tblseat
{
    public int SeatId { get; set; }

    public int? BusId { get; set; }

    public string SeatNumber { get; set; } = null!;

    public DateTime? CreatedAt { get; set; }

    public int? CreatedBy { get; set; }

    public DateTime? ModifiedAt { get; set; }

    public int? ModifiedBy { get; set; }

    public bool? IsActive { get; set; }

    public bool? IsDelete { get; set; }

    public virtual Tblbuse? Bus { get; set; }

    public virtual Tbluser? CreatedByNavigation { get; set; }

    public virtual Tbluser? ModifiedByNavigation { get; set; }

    public virtual ICollection<Tblbookingseat> Tblbookingseats { get; set; } = new List<Tblbookingseat>();
}
