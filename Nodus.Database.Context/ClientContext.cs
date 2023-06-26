using Microsoft.EntityFrameworkCore;
using Nodus.Database.Models.Customer;

namespace Nodus.Database.Context
{
    public class ClientContext : DbContext
    {
        private readonly string _connectionString;

        public ClientContext(string conntectionString)
        {
            _connectionString = conntectionString;
        }

        public ClientContext(DbContextOptions<ClientContext> options) : base(options)
        {

        }

        public ClientContext()
        {

        }

        public DbSet<Bill> Bills { get; set; }
        public DbSet<BillCategory> BillCategories { get; set; }
        public DbSet<BillStatus> BillStatuses { get; set; }
        public DbSet<Document> Documents { get; set; }
        public DbSet<HistoricalBill> HistoricalBills { get; set; }
        public DbSet<Trip> Trips { get; set; }
        public DbSet<UserDetails> UserDetails { get; set; }
        public DbSet<UserTrip> UserTrips { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                // if we got here, then this wasn't instantiated in Startup & the caller better use the constructor that passes
                // the connection string
                if (string.IsNullOrEmpty(this._connectionString))
                {
                    throw new System.Exception("No database connection for StaffRight specified");
                }
                else
                {
                    // set options
                    optionsBuilder.UseSqlServer(this._connectionString);
                }
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<UserTrip>(entity =>
            {
                entity.HasKey(e => new { e.TripId, e.UserId });

                entity.HasOne(s => s.User)
                 .WithMany(s => s.UserTrips)
                 .HasForeignKey(s => s.UserId)
                 .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<Bill>(entity =>
            {
                entity.HasOne(s => s.Creator)
                    .WithMany(s => s.CreatedBills)
                    .HasForeignKey(s => s.CreatorId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(s => s.Editor)
                    .WithMany(s => s.EditedBills)
                    .HasForeignKey(s => s.EditorId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(s => s.Status)
                    .WithMany(s => s.Bills)
                    .HasForeignKey(s => s.StatusId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(s => s.BillCategory)
                    .WithMany(s => s.Bills)
                    .HasForeignKey(s => s.BillCategoryId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<Document>(entity =>
            {
                entity.HasOne(s => s.Creator)
                    .WithMany(s => s.CreatedDocuments)
                    .HasForeignKey(s => s.CreatorId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.Property(s => s.IsActive)
                    .HasDefaultValue(true);
            });

            modelBuilder.Entity<HistoricalBill>(entity =>
            {
                entity.HasOne(s => s.Editor)
                    .WithMany(s => s.HistoricalBillsEdited)
                    .HasForeignKey(s => s.EditorId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(s => s.Status)
                    .WithMany(s => s.HistoricalBills)
                    .HasForeignKey(s => s.StatusId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasMany(s => s.Documents)
                    .WithMany(s => s.HistoricalBills)
                    .UsingEntity<Dictionary<string, object>>(
                        "DocumentsInHistoricalBills",
                        x => x.HasOne<Document>().WithMany().OnDelete(DeleteBehavior.Restrict),
                        x => x.HasOne<HistoricalBill>().WithMany().OnDelete(DeleteBehavior.Restrict)
                    );
            });

            modelBuilder.Entity<Trip>(entity =>
            {
                entity.HasOne(s => s.Creator)
                    .WithMany(s => s.CreatedTrips)
                    .HasForeignKey(s => s.CreatorId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.Property(p => p.Status)
                .HasColumnType("nvarchar(16)")
                .HasDefaultValueSql("'New'")
                .HasConversion<string>();


            });

            //Seed data

            modelBuilder.Entity<BillStatus>().HasData(
                new BillStatus
                {
                    Id = (int)TripStatusEnum.New,
                    Name = TripStatusEnum.New.ToString()
                },
                new BillStatus
                {
                    Id = (int)TripStatusEnum.Active,
                    Name = TripStatusEnum.Active.ToString()
                },
                new BillStatus
                {
                    Id = (int)TripStatusEnum.Inactive,
                    Name = TripStatusEnum.Inactive.ToString()
                });
        }
    }
}
