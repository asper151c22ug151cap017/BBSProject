using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace BusBookingSystem.Domain.Models;

public partial class DbBbsContext : DbContext
{
    public DbBbsContext()
    {
    }

    public DbBbsContext(DbContextOptions<DbBbsContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Tblbooking> Tblbookings { get; set; }

    public virtual DbSet<Tblbookingseat> Tblbookingseats { get; set; }

    public virtual DbSet<Tblbuse> Tblbuses { get; set; }

    public virtual DbSet<Tblbusrating> Tblbusratings { get; set; }

    public virtual DbSet<Tblgender> Tblgenders { get; set; }

    public virtual DbSet<Tblpassenger> Tblpassengers { get; set; }

    public virtual DbSet<Tblrole> Tblroles { get; set; }

    public virtual DbSet<Tblroute> Tblroutes { get; set; }

    public virtual DbSet<Tblseat> Tblseats { get; set; }

    public virtual DbSet<Tbluser> Tblusers { get; set; }



    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .HasPostgresEnum("gender", new[] { "Male", "Female", "Other" })
            .HasPostgresEnum("gender_enum", new[] { "Male", "Female", "Other" })
            .HasPostgresEnum("role_enum", new[] { "Admin", "User" });

        modelBuilder.Entity<Tblbooking>(entity =>
        {
            entity.HasKey(e => e.BookingId).HasName("tblbookings_pkey");

            entity.ToTable("tblbookings");

            entity.Property(e => e.BookingId).HasColumnName("booking_id");
            entity.Property(e => e.BookingDate)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("booking_date");
            entity.Property(e => e.BusId).HasColumnName("bus_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.CreatedBy).HasColumnName("created_by");
            entity.Property(e => e.IsActive)
                .HasDefaultValue(true)
                .HasColumnName("is_active");
            entity.Property(e => e.IsDelete)
                .HasDefaultValue(false)
                .HasColumnName("is_delete");
            entity.Property(e => e.ModifiedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("modified_at");
            entity.Property(e => e.ModifiedBy).HasColumnName("modified_by");
            entity.Property(e => e.RouteId).HasColumnName("route_id");
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .HasDefaultValueSql("'Confirmed'::character varying")
                .HasColumnName("status");
            entity.Property(e => e.TotalFare)
                .HasPrecision(10, 2)
                .HasColumnName("total_fare");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.Bus).WithMany(p => p.Tblbookings)
                .HasForeignKey(d => d.BusId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("tblbookings_bus_id_fkey");

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.TblbookingCreatedByNavigations)
                .HasForeignKey(d => d.CreatedBy)
                .HasConstraintName("tblbookings_created_by_fkey");

            entity.HasOne(d => d.ModifiedByNavigation).WithMany(p => p.TblbookingModifiedByNavigations)
                .HasForeignKey(d => d.ModifiedBy)
                .HasConstraintName("tblbookings_modified_by_fkey");

            entity.HasOne(d => d.Route).WithMany(p => p.Tblbookings)
                .HasForeignKey(d => d.RouteId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("tblbookings_route_id_fkey");

            entity.HasOne(d => d.User).WithMany(p => p.TblbookingUsers)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("tblbookings_user_id_fkey");
        });

        modelBuilder.Entity<Tblbookingseat>(entity =>
        {
            entity.HasKey(e => e.BookingSeatId).HasName("tblbookingseats_pkey");

            entity.ToTable("tblbookingseats");

            entity.HasIndex(e => new { e.BookingId, e.SeatId }, "tblbookingseats_booking_id_seat_id_key").IsUnique();

            entity.Property(e => e.BookingSeatId).HasColumnName("booking_seat_id");
            entity.Property(e => e.BookingId).HasColumnName("booking_id");
            entity.Property(e => e.BusId).HasColumnName("bus_id");
            entity.Property(e => e.SeatId).HasColumnName("seat_id");

            entity.HasOne(d => d.Booking).WithMany(p => p.Tblbookingseats)
                .HasForeignKey(d => d.BookingId)
                .HasConstraintName("fk_booking");

            entity.HasOne(d => d.Bus).WithMany(p => p.Tblbookingseats)
                .HasForeignKey(d => d.BusId)
                .HasConstraintName("tblbookingseats_bus_id_fkey");

            entity.HasOne(d => d.Seat).WithMany(p => p.Tblbookingseats)
                .HasForeignKey(d => d.SeatId)
                .HasConstraintName("fk_seat");
        });

        modelBuilder.Entity<Tblbuse>(entity =>
        {
            entity.HasKey(e => e.BusId).HasName("tblbuses_pkey");

            entity.ToTable("tblbuses");

            entity.HasIndex(e => e.Busnumber, "tblbuses_busnumber_key").IsUnique();

            entity.Property(e => e.BusId).HasColumnName("bus_id");
            entity.Property(e => e.BusName)
                .HasMaxLength(200)
                .HasColumnName("bus_name");
            entity.Property(e => e.BusType)
                .HasMaxLength(50)
                .HasColumnName("bus_type");
            entity.Property(e => e.Busnumber)
                .HasMaxLength(150)
                .HasColumnName("busnumber");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.CreatedBy).HasColumnName("created_by");
            entity.Property(e => e.Fare)
                .HasPrecision(10, 2)
                .HasColumnName("fare");
            entity.Property(e => e.IsActive)
                .HasDefaultValue(true)
                .HasColumnName("is_active");
            entity.Property(e => e.IsDelete)
                .HasDefaultValue(false)
                .HasColumnName("is_delete");
            entity.Property(e => e.ModifiedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("modified_at");
            entity.Property(e => e.ModifiedBy).HasColumnName("modified_by");
            entity.Property(e => e.OperatorName)
                .HasMaxLength(200)
                .HasColumnName("operator_name");
            entity.Property(e => e.OperatorNumber)
                .HasMaxLength(15)
                .HasColumnName("operator_number");
            entity.Property(e => e.Ratingid).HasColumnName("ratingid");
            entity.Property(e => e.TotalSeats).HasColumnName("total_seats");

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.TblbuseCreatedByNavigations)
                .HasForeignKey(d => d.CreatedBy)
                .HasConstraintName("tblbuses_created_by_fkey");

            entity.HasOne(d => d.ModifiedByNavigation).WithMany(p => p.TblbuseModifiedByNavigations)
                .HasForeignKey(d => d.ModifiedBy)
                .HasConstraintName("tblbuses_modified_by_fkey");

            entity.HasOne(d => d.Rating).WithMany(p => p.Tblbuses)
                .HasForeignKey(d => d.Ratingid)
                .HasConstraintName("tblbuses_ratingid_fkey");
        });

        modelBuilder.Entity<Tblbusrating>(entity =>
        {
            entity.HasKey(e => e.Ratingid).HasName("tblbusratings_pkey");

            entity.ToTable("tblbusratings");

            entity.Property(e => e.Ratingid).HasColumnName("ratingid");
            entity.Property(e => e.BusId).HasColumnName("bus_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.CreatedBy).HasColumnName("created_by");
            entity.Property(e => e.ModifiedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("modified_at");
            entity.Property(e => e.ModifiedBy).HasColumnName("modified_by");
            entity.Property(e => e.RatingValue)
                .HasPrecision(2, 1)
                .HasColumnName("rating_value");
            entity.Property(e => e.ReviewComment).HasColumnName("review_comment");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.Bus).WithMany(p => p.Tblbusratings)
                .HasForeignKey(d => d.BusId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("tblbusratings_bus_id_fkey");

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.TblbusratingCreatedByNavigations)
                .HasForeignKey(d => d.CreatedBy)
                .HasConstraintName("tblbusratings_created_by_fkey");

            entity.HasOne(d => d.ModifiedByNavigation).WithMany(p => p.TblbusratingModifiedByNavigations)
                .HasForeignKey(d => d.ModifiedBy)
                .HasConstraintName("tblbusratings_modified_by_fkey");

            entity.HasOne(d => d.User).WithMany(p => p.TblbusratingUsers)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("tblbusratings_user_id_fkey");
        });

        modelBuilder.Entity<Tblgender>(entity =>
        {
            entity.HasKey(e => e.Genderid).HasName("tblgender_pkey");

            entity.ToTable("tblgender");

            entity.Property(e => e.Genderid).HasColumnName("genderid");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.CreatedBy).HasColumnName("created_by");
            entity.Property(e => e.Gendername)
                .HasMaxLength(50)
                .HasColumnName("gendername");
            entity.Property(e => e.IsActive)
                .HasDefaultValue(true)
                .HasColumnName("is_active");
            entity.Property(e => e.IsDelete)
                .HasDefaultValue(false)
                .HasColumnName("is_delete");
            entity.Property(e => e.ModifiedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("modified_at");
            entity.Property(e => e.ModifiedBy).HasColumnName("modified_by");

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.TblgenderCreatedByNavigations)
                .HasForeignKey(d => d.CreatedBy)
                .HasConstraintName("tblgender_created_by_fkey");

            entity.HasOne(d => d.ModifiedByNavigation).WithMany(p => p.TblgenderModifiedByNavigations)
                .HasForeignKey(d => d.ModifiedBy)
                .HasConstraintName("tblgender_modified_by_fkey");
        });

        modelBuilder.Entity<Tblpassenger>(entity =>
        {
            entity.HasKey(e => e.PassengerId).HasName("tblpassengers_pkey");

            entity.ToTable("tblpassengers");

            entity.Property(e => e.PassengerId).HasColumnName("passenger_id");
            entity.Property(e => e.BookingId).HasColumnName("booking_id");
            entity.Property(e => e.PassengerAge).HasColumnName("passenger_age");
            entity.Property(e => e.PassengerGender)
                .HasMaxLength(20)
                .HasColumnName("passenger_gender");
            entity.Property(e => e.PassengerName)
                .HasMaxLength(100)
                .HasColumnName("passenger_name");

            entity.HasOne(d => d.Booking).WithMany(p => p.Tblpassengers)
                .HasForeignKey(d => d.BookingId)
                .HasConstraintName("fk_tblpassengers_booking");
        });

        modelBuilder.Entity<Tblrole>(entity =>
        {
            entity.HasKey(e => e.Roleid).HasName("tblrole_pkey");

            entity.ToTable("tblrole");

            entity.Property(e => e.Roleid).HasColumnName("roleid");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.CreatedBy).HasColumnName("created_by");
            entity.Property(e => e.IsActive)
                .HasDefaultValue(true)
                .HasColumnName("is_active");
            entity.Property(e => e.IsDelete)
                .HasDefaultValue(false)
                .HasColumnName("is_delete");
            entity.Property(e => e.ModifiedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("modified_at");
            entity.Property(e => e.ModifiedBy).HasColumnName("modified_by");
            entity.Property(e => e.Rolename)
                .HasMaxLength(50)
                .HasColumnName("rolename");

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.TblroleCreatedByNavigations)
                .HasForeignKey(d => d.CreatedBy)
                .HasConstraintName("tblrole_created_by_fkey");

            entity.HasOne(d => d.ModifiedByNavigation).WithMany(p => p.TblroleModifiedByNavigations)
                .HasForeignKey(d => d.ModifiedBy)
                .HasConstraintName("tblrole_modified_by_fkey");
        });

        modelBuilder.Entity<Tblroute>(entity =>
        {
            entity.HasKey(e => e.RouteId).HasName("tblroutes_pkey");

            entity.ToTable("tblroutes");

            entity.Property(e => e.RouteId).HasColumnName("route_id");
            entity.Property(e => e.Arrivaltime).HasColumnName("arrivaltime");
            entity.Property(e => e.BusId).HasColumnName("bus_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.CreatedBy).HasColumnName("created_by");
            entity.Property(e => e.Departuretime).HasColumnName("departuretime");
            entity.Property(e => e.Destination)
                .HasMaxLength(300)
                .HasColumnName("destination");
            entity.Property(e => e.IsActive)
                .HasDefaultValue(true)
                .HasColumnName("is_active");
            entity.Property(e => e.IsDelete)
                .HasDefaultValue(false)
                .HasColumnName("is_delete");
            entity.Property(e => e.ModifiedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("modified_at");
            entity.Property(e => e.ModifiedBy).HasColumnName("modified_by");
            entity.Property(e => e.Source)
                .HasMaxLength(300)
                .HasColumnName("source");

            entity.HasOne(d => d.Bus).WithMany(p => p.Tblroutes)
                .HasForeignKey(d => d.BusId)
                .HasConstraintName("tblroutes_bus_id_fkey");

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.TblrouteCreatedByNavigations)
                .HasForeignKey(d => d.CreatedBy)
                .HasConstraintName("tblroutes_created_by_fkey");

            entity.HasOne(d => d.ModifiedByNavigation).WithMany(p => p.TblrouteModifiedByNavigations)
                .HasForeignKey(d => d.ModifiedBy)
                .HasConstraintName("tblroutes_modified_by_fkey");
        });

        modelBuilder.Entity<Tblseat>(entity =>
        {
            entity.HasKey(e => e.SeatId).HasName("tblseats_pkey");

            entity.ToTable("tblseats");

            entity.Property(e => e.SeatId).HasColumnName("seat_id");
            entity.Property(e => e.BusId).HasColumnName("bus_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.CreatedBy).HasColumnName("created_by");
            entity.Property(e => e.IsActive)
                .HasDefaultValue(true)
                .HasColumnName("is_active");
            entity.Property(e => e.IsDelete)
                .HasDefaultValue(false)
                .HasColumnName("is_delete");
            entity.Property(e => e.ModifiedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("modified_at");
            entity.Property(e => e.ModifiedBy).HasColumnName("modified_by");
            entity.Property(e => e.SeatNumber)
                .HasMaxLength(15)
                .HasColumnName("seat_number");

            entity.HasOne(d => d.Bus).WithMany(p => p.Tblseats)
                .HasForeignKey(d => d.BusId)
                .HasConstraintName("tblseats_bus_id_fkey");

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.TblseatCreatedByNavigations)
                .HasForeignKey(d => d.CreatedBy)
                .HasConstraintName("tblseats_created_by_fkey");

            entity.HasOne(d => d.ModifiedByNavigation).WithMany(p => p.TblseatModifiedByNavigations)
                .HasForeignKey(d => d.ModifiedBy)
                .HasConstraintName("tblseats_modified_by_fkey");
        });

        modelBuilder.Entity<Tbluser>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("tblusers_pkey");

            entity.ToTable("tblusers");

            entity.HasIndex(e => e.Email, "tblusers_email_key").IsUnique();

            entity.Property(e => e.UserId).HasColumnName("user_id");
            entity.Property(e => e.Age).HasColumnName("age");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.CreatedBy).HasColumnName("created_by");
            entity.Property(e => e.Email)
                .HasMaxLength(100)
                .HasColumnName("email");
            entity.Property(e => e.GenderId).HasColumnName("gender_id");
            entity.Property(e => e.IsActive)
                .HasDefaultValue(true)
                .HasColumnName("is_active");
            entity.Property(e => e.IsDelete)
                .HasDefaultValue(false)
                .HasColumnName("is_delete");
            entity.Property(e => e.ModifiedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("modified_at");
            entity.Property(e => e.ModifiedBy).HasColumnName("modified_by");
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .HasColumnName("name");
            entity.Property(e => e.Password)
                .HasMaxLength(255)
                .HasColumnName("password");
            entity.Property(e => e.Phone)
                .HasMaxLength(15)
                .HasColumnName("phone");
            entity.Property(e => e.RoleId).HasColumnName("role_id");

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.InverseCreatedByNavigation)
                .HasForeignKey(d => d.CreatedBy)
                .HasConstraintName("tblusers_created_by_fkey");

            entity.HasOne(d => d.Gender).WithMany(p => p.Tblusers)
                .HasForeignKey(d => d.GenderId)
                .HasConstraintName("tblusers_gender_id_fkey");

            entity.HasOne(d => d.ModifiedByNavigation).WithMany(p => p.InverseModifiedByNavigation)
                .HasForeignKey(d => d.ModifiedBy)
                .HasConstraintName("tblusers_modified_by_fkey");

            entity.HasOne(d => d.Role).WithMany(p => p.Tblusers)
                .HasForeignKey(d => d.RoleId)
                .HasConstraintName("tblusers_role_id_fkey");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
