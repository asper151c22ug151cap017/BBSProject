using System;
using System.Collections.Generic;

namespace BusBookingSystem.Domain.Models;

public partial class Tblbusrating
{
    public int Ratingid { get; set; }

    public int BusId { get; set; }

    public int UserId { get; set; }

    public decimal RatingValue { get; set; }

    public string? ReviewComment { get; set; }

    public DateTime? CreatedAt { get; set; }

    public int? CreatedBy { get; set; }

    public DateTime? ModifiedAt { get; set; }

    public int? ModifiedBy { get; set; }

    public virtual Tblbuse Bus { get; set; } = null!;

    public virtual Tbluser? CreatedByNavigation { get; set; }

    public virtual Tbluser? ModifiedByNavigation { get; set; }

    public virtual ICollection<Tblbuse> Tblbuses { get; set; } = new List<Tblbuse>();

    public virtual Tbluser User { get; set; } = null!;
}
