using System;
using System.Collections.Generic;

namespace BusBookingSystem.Domain.Models;

public partial class Tblbookingseat
{
    public int BookingSeatId { get; set; }

    public int BookingId { get; set; }

    public int SeatId { get; set; }

    public int? BusId { get; set; }

    public virtual Tblbooking Booking { get; set; } = null!;

    public virtual Tblbuse? Bus { get; set; }

    public virtual Tblseat Seat { get; set; } = null!;
}
