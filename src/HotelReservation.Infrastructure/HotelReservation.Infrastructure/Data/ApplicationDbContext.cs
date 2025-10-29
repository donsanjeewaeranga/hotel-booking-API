using HotelReservation.Domain.Entities;
using HotelReservation.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace HotelReservation.Infrastructure.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    public DbSet<AppUser> AppUsers { get; set; }
    public DbSet<Guest> Guests { get; set; }
    public DbSet<RoomType> RoomTypes { get; set; }
    public DbSet<Room> Rooms { get; set; }
    public DbSet<RoomFeature> RoomFeatures { get; set; }
    public DbSet<RoomFeatureAssignment> RoomFeatureAssignments { get; set; }
    public DbSet<Reservation> Reservations { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure AppUser
        modelBuilder.Entity<AppUser>(entity =>
        {
            entity.HasKey(e => e.UserId);
            entity.Property(e => e.Email).IsRequired().HasMaxLength(255);
            entity.Property(e => e.PasswordHash).IsRequired();
            entity.Property(e => e.UserType)
                .HasConversion<string>()
                .HasMaxLength(50);
            entity.HasIndex(e => e.Email).IsUnique();
            
            // Soft delete filter
            entity.HasQueryFilter(e => e.DeletedAt == null);
        });

        // Configure Guest
        modelBuilder.Entity<Guest>(entity =>
        {
            entity.HasKey(e => e.GuestId);
            entity.Property(e => e.FirstName).IsRequired().HasMaxLength(100);
            entity.Property(e => e.LastName).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Title).HasMaxLength(10);
            entity.Property(e => e.PhoneNumber).HasMaxLength(20);
            
            entity.HasOne(e => e.User)
                .WithOne(e => e.Guest)
                .HasForeignKey<Guest>(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);
            
            entity.HasIndex(e => e.UserId).IsUnique();
            
            // Soft delete filter
            entity.HasQueryFilter(e => e.DeletedAt == null);
        });

        // Configure RoomType
        modelBuilder.Entity<RoomType>(entity =>
        {
            entity.HasKey(e => e.RoomTypeId);
            entity.Property(e => e.TypeName).IsRequired().HasMaxLength(50);
            entity.Property(e => e.Capacity).IsRequired();
            entity.Property(e => e.BasePrice).IsRequired().HasColumnType("decimal(10,2)");
            
            entity.HasIndex(e => e.TypeName).IsUnique();
            
            // Soft delete filter
            entity.HasQueryFilter(e => e.DeletedAt == null);
        });

        // Configure Room
        modelBuilder.Entity<Room>(entity =>
        {
            entity.HasKey(e => e.RoomId);
            entity.Property(e => e.RoomNumber).IsRequired().HasMaxLength(10);
            entity.Property(e => e.Status)
                .HasConversion<string>()
                .HasMaxLength(50)
                .HasDefaultValue(RoomStatus.Available);
            
            entity.HasOne(e => e.RoomType)
                .WithMany(e => e.Rooms)
                .HasForeignKey(e => e.RoomTypeId)
                .OnDelete(DeleteBehavior.SetNull);
            
            entity.HasIndex(e => e.RoomNumber).IsUnique();
            
            // Soft delete filter
            entity.HasQueryFilter(e => e.DeletedAt == null);
        });

        // Configure RoomFeature
        modelBuilder.Entity<RoomFeature>(entity =>
        {
            entity.HasKey(e => e.RoomFeatureId);
            entity.Property(e => e.FeatureName).IsRequired().HasMaxLength(100);
            
            entity.HasIndex(e => e.FeatureName).IsUnique();
        });

        // Configure RoomFeatureAssignment (many-to-many)
        modelBuilder.Entity<RoomFeatureAssignment>(entity =>
        {
            entity.HasKey(e => new { e.RoomId, e.RoomFeatureId });
            
            entity.HasOne(e => e.Room)
                .WithMany(e => e.RoomFeatureAssignments)
                .HasForeignKey(e => e.RoomId)
                .OnDelete(DeleteBehavior.Cascade);
            
            entity.HasOne(e => e.RoomFeature)
                .WithMany(e => e.RoomFeatureAssignments)
                .HasForeignKey(e => e.RoomFeatureId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // Configure Reservation
        modelBuilder.Entity<Reservation>(entity =>
        {
            entity.HasKey(e => e.ReservationId);
            entity.Property(e => e.CheckInDate).IsRequired();
            entity.Property(e => e.CheckOutDate).IsRequired();
            entity.Property(e => e.TotalAmount).IsRequired().HasColumnType("decimal(10,2)");
            entity.Property(e => e.TaxServiceCharge).IsRequired().HasColumnType("decimal(10,2)");
            entity.Property(e => e.GrandTotal).IsRequired().HasColumnType("decimal(10,2)");
            entity.Property(e => e.ReservationStatus)
                .HasConversion<string>()
                .HasMaxLength(50)
                .HasDefaultValue(ReservationStatus.Confirmed);
            
            entity.HasOne(e => e.Guest)
                .WithMany(e => e.Reservations)
                .HasForeignKey(e => e.GuestId)
                .OnDelete(DeleteBehavior.Cascade);
            
            entity.HasOne(e => e.Room)
                .WithMany(e => e.Reservations)
                .HasForeignKey(e => e.RoomId)
                .OnDelete(DeleteBehavior.Cascade);
            
            // Add check constraint for dates
            entity.HasCheckConstraint("chk_dates", "\"CheckOutDate\" > \"CheckInDate\"");
            
            // Soft delete filter
            entity.HasQueryFilter(e => e.DeletedAt == null);
        });

        // Seed data
        SeedData(modelBuilder);
    }

    private void SeedData(ModelBuilder modelBuilder)
    {
        // Seed RoomTypes
        modelBuilder.Entity<RoomType>().HasData(
            new RoomType { RoomTypeId = 1, TypeName = "Standard", Description = "Standard room with basic amenities", Capacity = 2, BasePrice = 100.00m },
            new RoomType { RoomTypeId = 2, TypeName = "Deluxe", Description = "Deluxe room with enhanced amenities", Capacity = 2, BasePrice = 150.00m },
            new RoomType { RoomTypeId = 3, TypeName = "Suite", Description = "Luxury suite with premium amenities", Capacity = 4, BasePrice = 300.00m }
        );

        // Seed RoomFeatures
        modelBuilder.Entity<RoomFeature>().HasData(
            new RoomFeature { RoomFeatureId = 1, FeatureName = "WiFi" },
            new RoomFeature { RoomFeatureId = 2, FeatureName = "Air Conditioning" },
            new RoomFeature { RoomFeatureId = 3, FeatureName = "Mini Bar" },
            new RoomFeature { RoomFeatureId = 4, FeatureName = "Balcony" },
            new RoomFeature { RoomFeatureId = 5, FeatureName = "Ocean View" },
            new RoomFeature { RoomFeatureId = 6, FeatureName = "Jacuzzi" }
        );

        // Seed Rooms
        modelBuilder.Entity<Room>().HasData(
            new Room { RoomId = 1, RoomNumber = "101", RoomTypeId = 1, Status = RoomStatus.Available },
            new Room { RoomId = 2, RoomNumber = "102", RoomTypeId = 1, Status = RoomStatus.Available },
            new Room { RoomId = 3, RoomNumber = "201", RoomTypeId = 2, Status = RoomStatus.Available },
            new Room { RoomId = 4, RoomNumber = "202", RoomTypeId = 2, Status = RoomStatus.Available },
            new Room { RoomId = 5, RoomNumber = "301", RoomTypeId = 3, Status = RoomStatus.Available },
            new Room { RoomId = 6, RoomNumber = "302", RoomTypeId = 3, Status = RoomStatus.Available }
        );

        // Seed RoomFeatureAssignments
        modelBuilder.Entity<RoomFeatureAssignment>().HasData(
            // Standard rooms (101, 102)
            new RoomFeatureAssignment { RoomId = 1, RoomFeatureId = 1 }, // WiFi
            new RoomFeatureAssignment { RoomId = 1, RoomFeatureId = 2 }, // Air Conditioning
            new RoomFeatureAssignment { RoomId = 2, RoomFeatureId = 1 }, // WiFi
            new RoomFeatureAssignment { RoomId = 2, RoomFeatureId = 2 }, // Air Conditioning
            
            // Deluxe rooms (201, 202)
            new RoomFeatureAssignment { RoomId = 3, RoomFeatureId = 1 }, // WiFi
            new RoomFeatureAssignment { RoomId = 3, RoomFeatureId = 2 }, // Air Conditioning
            new RoomFeatureAssignment { RoomId = 3, RoomFeatureId = 3 }, // Mini Bar
            new RoomFeatureAssignment { RoomId = 3, RoomFeatureId = 4 }, // Balcony
            new RoomFeatureAssignment { RoomId = 4, RoomFeatureId = 1 }, // WiFi
            new RoomFeatureAssignment { RoomId = 4, RoomFeatureId = 2 }, // Air Conditioning
            new RoomFeatureAssignment { RoomId = 4, RoomFeatureId = 3 }, // Mini Bar
            new RoomFeatureAssignment { RoomId = 4, RoomFeatureId = 4 }, // Balcony
            
            // Suites (301, 302)
            new RoomFeatureAssignment { RoomId = 5, RoomFeatureId = 1 }, // WiFi
            new RoomFeatureAssignment { RoomId = 5, RoomFeatureId = 2 }, // Air Conditioning
            new RoomFeatureAssignment { RoomId = 5, RoomFeatureId = 3 }, // Mini Bar
            new RoomFeatureAssignment { RoomId = 5, RoomFeatureId = 4 }, // Balcony
            new RoomFeatureAssignment { RoomId = 5, RoomFeatureId = 5 }, // Ocean View
            new RoomFeatureAssignment { RoomId = 5, RoomFeatureId = 6 }, // Jacuzzi
            new RoomFeatureAssignment { RoomId = 6, RoomFeatureId = 1 }, // WiFi
            new RoomFeatureAssignment { RoomId = 6, RoomFeatureId = 2 }, // Air Conditioning
            new RoomFeatureAssignment { RoomId = 6, RoomFeatureId = 3 }, // Mini Bar
            new RoomFeatureAssignment { RoomId = 6, RoomFeatureId = 4 }, // Balcony
            new RoomFeatureAssignment { RoomId = 6, RoomFeatureId = 5 }, // Ocean View
            new RoomFeatureAssignment { RoomId = 6, RoomFeatureId = 6 }  // Jacuzzi
        );
    }
}
