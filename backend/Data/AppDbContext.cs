using Microsoft.EntityFrameworkCore;
using backend.Models;

namespace backend.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    // PM
    public DbSet<Passenger> Passengers => Set<Passenger>();
    public DbSet<SavedRoute> SavedRoutes => Set<SavedRoute>();
    public DbSet<TravelHistory> TravelHistories => Set<TravelHistory>();
    public DbSet<Feedback> Feedbacks => Set<Feedback>();
    public DbSet<SosAlert> SosAlerts => Set<SosAlert>();

    // RM / SM
    public DbSet<Models.Route> Routes => Set<Models.Route>();
    public DbSet<Stop> Stops => Set<Stop>();
    public DbSet<RouteStopMapping> RouteStopMappings => Set<RouteStopMapping>();

    // FM
    public DbSet<Bus> Buses => Set<Bus>();
    public DbSet<BusStatus> BusStatuses => Set<BusStatus>();
    public DbSet<MaintenanceRecord> MaintenanceRecords => Set<MaintenanceRecord>();
    public DbSet<FuelRecord> FuelRecords => Set<FuelRecord>();

    // DM
    public DbSet<Depot> Depots => Set<Depot>();
    public DbSet<BusAssignment> BusAssignments => Set<BusAssignment>();

    // DC
    public DbSet<Driver> Drivers => Set<Driver>();
    public DbSet<Conductor> Conductors => Set<Conductor>();
    public DbSet<DutyAssignment> DutyAssignments => Set<DutyAssignment>();
    public DbSet<Attendance> Attendances => Set<Attendance>();

    // GT
    public DbSet<BusLocation> BusLocations => Set<BusLocation>();
    public DbSet<EtaRecord> EtaRecords => Set<EtaRecord>();

    // FP
    public DbSet<FareRule> FareRules => Set<FareRule>();
    public DbSet<Ticket> Tickets => Set<Ticket>();
    public DbSet<Pass> Passes => Set<Pass>();
    public DbSet<Transaction> Transactions => Set<Transaction>();

    // NA
    public DbSet<Notification> Notifications => Set<Notification>();

    // EM
    public DbSet<Incident> Incidents => Set<Incident>();
    public DbSet<IncidentResponse> IncidentResponses => Set<IncidentResponse>();

    // AS
    public DbSet<AuditLog> AuditLogs => Set<AuditLog>();

    // RBAC
    public DbSet<Role> Roles => Set<Role>();
    public DbSet<UserRole> UserRoles => Set<UserRole>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Passenger unique indexes
        modelBuilder.Entity<Passenger>().HasIndex(p => p.Email).IsUnique();
        modelBuilder.Entity<Passenger>().HasIndex(p => p.MobileNumber).IsUnique();

        // RouteStopMapping
        modelBuilder.Entity<RouteStopMapping>()
            .HasOne(r => r.Route).WithMany(r => r.RouteStopMappings)
            .HasForeignKey(r => r.RouteId).OnDelete(DeleteBehavior.Cascade);
        modelBuilder.Entity<RouteStopMapping>()
            .HasOne(r => r.Stop).WithMany(s => s.RouteStopMappings)
            .HasForeignKey(r => r.StopId).OnDelete(DeleteBehavior.Cascade);

        // SavedRoute
        modelBuilder.Entity<SavedRoute>()
            .HasOne(s => s.Passenger).WithMany().HasForeignKey(s => s.PassengerId).OnDelete(DeleteBehavior.Cascade);
        modelBuilder.Entity<SavedRoute>()
            .HasOne(s => s.Route).WithMany().HasForeignKey(s => s.RouteId).OnDelete(DeleteBehavior.Cascade);
        modelBuilder.Entity<SavedRoute>()
            .HasIndex(s => new { s.PassengerId, s.RouteId }).IsUnique();

        // TravelHistory
        modelBuilder.Entity<TravelHistory>()
            .HasOne(t => t.Passenger).WithMany().HasForeignKey(t => t.PassengerId).OnDelete(DeleteBehavior.Cascade);
        modelBuilder.Entity<TravelHistory>()
            .HasOne(t => t.Route).WithMany().HasForeignKey(t => t.RouteId).OnDelete(DeleteBehavior.NoAction);

        // Feedback
        modelBuilder.Entity<Feedback>()
            .HasOne(f => f.Passenger).WithMany().HasForeignKey(f => f.PassengerId).OnDelete(DeleteBehavior.Cascade);

        // SosAlert
        modelBuilder.Entity<SosAlert>()
            .HasOne(s => s.Passenger).WithMany().HasForeignKey(s => s.PassengerId).OnDelete(DeleteBehavior.Cascade);

        // Bus
        modelBuilder.Entity<Bus>().HasIndex(b => b.RegistrationNumber).IsUnique();
        modelBuilder.Entity<Bus>()
            .HasOne(b => b.Depot).WithMany(d => d.Buses).HasForeignKey(b => b.DepotId).OnDelete(DeleteBehavior.SetNull);

        // BusStatus
        modelBuilder.Entity<BusStatus>()
            .HasOne(b => b.Bus).WithMany(b => b.BusStatuses).HasForeignKey(b => b.BusId).OnDelete(DeleteBehavior.Cascade);

        // MaintenanceRecord
        modelBuilder.Entity<MaintenanceRecord>()
            .HasOne(m => m.Bus).WithMany(b => b.MaintenanceRecords).HasForeignKey(m => m.BusId).OnDelete(DeleteBehavior.Cascade);

        // FuelRecord
        modelBuilder.Entity<FuelRecord>()
            .HasOne(f => f.Bus).WithMany(b => b.FuelRecords).HasForeignKey(f => f.BusId).OnDelete(DeleteBehavior.Cascade);

        // BusAssignment
        modelBuilder.Entity<BusAssignment>()
            .HasOne(a => a.Depot).WithMany(d => d.BusAssignments).HasForeignKey(a => a.DepotId).OnDelete(DeleteBehavior.Cascade);
        modelBuilder.Entity<BusAssignment>()
            .HasOne(a => a.Bus).WithMany().HasForeignKey(a => a.BusId).OnDelete(DeleteBehavior.NoAction);
        modelBuilder.Entity<BusAssignment>()
            .HasOne(a => a.Route).WithMany().HasForeignKey(a => a.RouteId).OnDelete(DeleteBehavior.NoAction);

        // DutyAssignment
        modelBuilder.Entity<DutyAssignment>()
            .HasOne(d => d.Driver).WithMany(d => d.DutyAssignments).HasForeignKey(d => d.DriverId).OnDelete(DeleteBehavior.NoAction);
        modelBuilder.Entity<DutyAssignment>()
            .HasOne(d => d.Conductor).WithMany(c => c.DutyAssignments).HasForeignKey(d => d.ConductorId).OnDelete(DeleteBehavior.NoAction);
        modelBuilder.Entity<DutyAssignment>()
            .HasOne(d => d.Bus).WithMany().HasForeignKey(d => d.BusId).OnDelete(DeleteBehavior.NoAction);
        modelBuilder.Entity<DutyAssignment>()
            .HasOne(d => d.Route).WithMany().HasForeignKey(d => d.RouteId).OnDelete(DeleteBehavior.NoAction);

        // Attendance
        modelBuilder.Entity<Attendance>()
            .HasOne(a => a.DutyAssignment).WithMany().HasForeignKey(a => a.DutyAssignmentId).OnDelete(DeleteBehavior.Cascade);

        // BusLocation index for performance
        modelBuilder.Entity<BusLocation>()
            .HasOne(b => b.Bus).WithMany().HasForeignKey(b => b.BusId).OnDelete(DeleteBehavior.Cascade);
        modelBuilder.Entity<BusLocation>().HasIndex(b => new { b.BusId, b.RecordedAt });

        // EtaRecord
        modelBuilder.Entity<EtaRecord>()
            .HasOne(e => e.Bus).WithMany().HasForeignKey(e => e.BusId).OnDelete(DeleteBehavior.Cascade);
        modelBuilder.Entity<EtaRecord>()
            .HasOne(e => e.Stop).WithMany().HasForeignKey(e => e.StopId).OnDelete(DeleteBehavior.NoAction);

        // Ticket
        modelBuilder.Entity<Ticket>()
            .HasOne(t => t.Bus).WithMany().HasForeignKey(t => t.BusId).OnDelete(DeleteBehavior.NoAction);
        modelBuilder.Entity<Ticket>()
            .HasOne(t => t.BoardingStop).WithMany().HasForeignKey(t => t.BoardingStopId).OnDelete(DeleteBehavior.NoAction);
        modelBuilder.Entity<Ticket>()
            .HasOne(t => t.AlightingStop).WithMany().HasForeignKey(t => t.AlightingStopId).OnDelete(DeleteBehavior.NoAction);

        // Pass
        modelBuilder.Entity<Pass>()
            .HasOne(p => p.Passenger).WithMany().HasForeignKey(p => p.PassengerId).OnDelete(DeleteBehavior.Cascade);

        // Transaction
        modelBuilder.Entity<Transaction>()
            .HasOne(t => t.Passenger).WithMany().HasForeignKey(t => t.PassengerId).OnDelete(DeleteBehavior.Cascade);

        // Incident
        modelBuilder.Entity<Incident>()
            .HasOne(i => i.Bus).WithMany().HasForeignKey(i => i.BusId).OnDelete(DeleteBehavior.NoAction);
        modelBuilder.Entity<Incident>()
            .HasOne(i => i.Driver).WithMany().HasForeignKey(i => i.DriverId).OnDelete(DeleteBehavior.NoAction);
        modelBuilder.Entity<IncidentResponse>()
            .HasOne(r => r.Incident).WithMany(i => i.Responses).HasForeignKey(r => r.IncidentId).OnDelete(DeleteBehavior.Cascade);

        // AuditLog index
        modelBuilder.Entity<AuditLog>().HasIndex(a => new { a.UserId, a.Timestamp });

        // RBAC
        modelBuilder.Entity<Role>().HasIndex(r => r.RoleName).IsUnique();
        modelBuilder.Entity<UserRole>()
            .HasOne(ur => ur.Passenger).WithMany(p => p.UserRoles).HasForeignKey(ur => ur.PassengerId).OnDelete(DeleteBehavior.Cascade);
        modelBuilder.Entity<UserRole>()
            .HasOne(ur => ur.Role).WithMany(r => r.UserRoles).HasForeignKey(ur => ur.RoleId).OnDelete(DeleteBehavior.Cascade);
        modelBuilder.Entity<UserRole>()
            .HasIndex(ur => new { ur.PassengerId, ur.RoleId }).IsUnique();

        // Seed default roles
        modelBuilder.Entity<Role>().HasData(
            new Role { Id = 1, RoleName = "Admin" },
            new Role { Id = 2, RoleName = "Passenger" },
            new Role { Id = 3, RoleName = "Driver" },
            new Role { Id = 4, RoleName = "Conductor" },
            new Role { Id = 5, RoleName = "DepotManager" }
        );
    }
}
