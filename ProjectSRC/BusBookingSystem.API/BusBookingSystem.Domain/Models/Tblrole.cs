using System;
using System.Collections.Generic;

namespace BusBookingSystem.Domain.Models;

public partial class Tblrole
{
    public int Roleid { get; set; }

    public string Rolename { get; set; } = null!;

    public int? CreatedBy { get; set; }

    public DateTime? CreatedAt { get; set; }

    public int? ModifiedBy { get; set; }

    public DateTime? ModifiedAt { get; set; }

    public bool? IsActive { get; set; }

    public bool? IsDelete { get; set; }

    public virtual Tbluser? CreatedByNavigation { get; set; }

    public virtual Tbluser? ModifiedByNavigation { get; set; }

    public virtual ICollection<Tbluser> Tblusers { get; set; } = new List<Tbluser>();
}
