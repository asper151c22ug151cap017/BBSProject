using System;
using System.Collections.Generic;

namespace BusBookingSystem.Domain.Models;

public partial class Tblpassenger
{
    public int PassengerId { get; set; }

    public int BookingId { get; set; }

    public string PassengerName { get; set; } = null!;

    public int PassengerAge { get; set; }

    public string PassengerGender { get; set; } = null!;

    public virtual Tblbooking Booking { get; set; } = null!;
}
