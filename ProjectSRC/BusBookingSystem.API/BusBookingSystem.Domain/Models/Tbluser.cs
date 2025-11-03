using System;
using System.Collections.Generic;

namespace BusBookingSystem.Domain.Models;

public partial class Tbluser
{
    public int UserId { get; set; }

    public string Name { get; set; } = null!;

    public string Password { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string Phone { get; set; } = null!;

    public int Age { get; set; }

    public DateTime? CreatedAt { get; set; }

    public int? CreatedBy { get; set; }

    public DateTime? ModifiedAt { get; set; }

    public int? ModifiedBy { get; set; }

    public bool? IsActive { get; set; }

    public bool? IsDelete { get; set; }

    public int? GenderId { get; set; }

    public int? RoleId { get; set; }

    public virtual Tbluser? CreatedByNavigation { get; set; }

    public virtual Tblgender? Gender { get; set; }

    public virtual ICollection<Tbluser> InverseCreatedByNavigation { get; set; } = new List<Tbluser>();

    public virtual ICollection<Tbluser> InverseModifiedByNavigation { get; set; } = new List<Tbluser>();

    public virtual Tbluser? ModifiedByNavigation { get; set; }

    public virtual Tblrole? Role { get; set; }

    public virtual ICollection<Tblbooking> TblbookingCreatedByNavigations { get; set; } = new List<Tblbooking>();

    public virtual ICollection<Tblbooking> TblbookingModifiedByNavigations { get; set; } = new List<Tblbooking>();

    public virtual ICollection<Tblbooking> TblbookingUsers { get; set; } = new List<Tblbooking>();

    public virtual ICollection<Tblbuse> TblbuseCreatedByNavigations { get; set; } = new List<Tblbuse>();

    public virtual ICollection<Tblbuse> TblbuseModifiedByNavigations { get; set; } = new List<Tblbuse>();

    public virtual ICollection<Tblbusrating> TblbusratingCreatedByNavigations { get; set; } = new List<Tblbusrating>();

    public virtual ICollection<Tblbusrating> TblbusratingModifiedByNavigations { get; set; } = new List<Tblbusrating>();

    public virtual ICollection<Tblbusrating> TblbusratingUsers { get; set; } = new List<Tblbusrating>();

    public virtual ICollection<Tblgender> TblgenderCreatedByNavigations { get; set; } = new List<Tblgender>();

    public virtual ICollection<Tblgender> TblgenderModifiedByNavigations { get; set; } = new List<Tblgender>();

    public virtual ICollection<Tblrole> TblroleCreatedByNavigations { get; set; } = new List<Tblrole>();

    public virtual ICollection<Tblrole> TblroleModifiedByNavigations { get; set; } = new List<Tblrole>();

    public virtual ICollection<Tblroute> TblrouteCreatedByNavigations { get; set; } = new List<Tblroute>();

    public virtual ICollection<Tblroute> TblrouteModifiedByNavigations { get; set; } = new List<Tblroute>();

    public virtual ICollection<Tblseat> TblseatCreatedByNavigations { get; set; } = new List<Tblseat>();

    public virtual ICollection<Tblseat> TblseatModifiedByNavigations { get; set; } = new List<Tblseat>();
}
