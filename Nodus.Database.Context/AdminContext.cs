using Microsoft.EntityFrameworkCore;
using Nodus.Database.Context.Constants;
using Nodus.Database.Models.Admin;
using System.Security.Cryptography;
using System.Text;

namespace Nodus.Database.Context
{
    public class AdminContext : DbContext
    {
        private readonly string _connectionString;

        public AdminContext(DbContextOptions<AdminContext> options) : base(options)
        {

        }

        public AdminContext()
        {

        }

        public AdminContext(string connectionString)
        {
            _connectionString = connectionString;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                // if we got here, then this wasn't instantiated in Startup & the caller better use the constructor that passes
                // the connection string
                if (string.IsNullOrEmpty(this._connectionString))
                {
                    throw new System.Exception("No database connection for Admin database specified");
                }
                else
                {
                    // set options
                    optionsBuilder.UseSqlServer(this._connectionString);
                }
            }
        }

        public DbSet<Company> Companies { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Feature> Features { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<RoleFeature> RoleFeatures { get; set; }
        public DbSet<Link> Links { get; set; }
        public DbSet<TgChat> TgChats { get; set; }
        public DbSet<SentMessageWithInlineItems> MessagesWithInlineItems { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<RoleFeature>(entity =>
            {
                entity.HasKey(e => new { e.FeatureId, e.RoleId });
            });

            modelBuilder.Entity<Feature>().HasData(
                new Feature
                {
                    Id = (int)FeatureNames.ManageCompanies,
                    Name = FeatureNames.ManageCompanies.ToString(),
                    Description = "Feature for super admin only"
                },
                new Feature
                {
                    Id = (int)FeatureNames.ReadUsers,
                    Name = FeatureNames.ReadUsers.ToString(),
                    Description = "Allows to read users personal information"
                },
                new Feature
                {
                    Id = (int)FeatureNames.WriteUsers,
                    Name = FeatureNames.WriteUsers.ToString(),
                    Description = "Allows to update users personal information"
                },
                new Feature
                {
                    Id = (int)FeatureNames.ReadTrips,
                    Name = FeatureNames.ReadTrips.ToString(),
                    Description = "Allows to access trips"
                },
                new Feature
                {
                    Id = (int)FeatureNames.WriteTrips,
                    Name = FeatureNames.WriteTrips.ToString(),
                    Description = "Allows to manage trips"
                },
                new Feature
                {
                    Id = (int)FeatureNames.ReadBills,
                    Name = FeatureNames.ReadBills.ToString(),
                    Description = "Allows to access bills"
                },
                new Feature
                {
                    Id = (int)FeatureNames.WriteBills,
                    Name = FeatureNames.WriteBills.ToString(),
                    Description = "Allows to create bills"
                },
                new Feature
                {
                    Id = (int)FeatureNames.AccessStatistics,
                    Name = FeatureNames.AccessStatistics.ToString(),
                    Description = "Additional feature"
                });

            modelBuilder.Entity<Role>(entity =>
            {
                entity.HasData(new Role
                {
                    Id = -1,
                    Name = "SuperAdmin",
                    Description = "Super admin role for admin-side workers",
                    CompanyId = null,
                });
            });

            modelBuilder.Entity<RoleFeature>(entity =>
            {
                entity.HasData(new RoleFeature
                {
                    RoleId = -1,
                    FeatureId = 1
                });
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.HasIndex(u => u.Email).IsUnique();
                entity.HasIndex(u => u.PhoneNumber).IsUnique();

                var salt = RNGCryptoServiceProvider.GetBytes(32);

                entity.HasData(new User
                {
                    Id = new Guid("0311ddca-0795-4606-b8d0-8d3c46a734fd"),
                    RoleId = -1,
                    PasswordSalt = salt,
                    PasswordHash = HashPasswordWithSalt("Qwerty1!", salt),
                    Email = "nodus.admin@email.com",
                    PhoneNumber = "1234567890",
                    IsActive = true,
                    Created = DateTime.Now,
                });
            });
        }

        private byte[] HashPasswordWithSalt(string password, byte[] salt)
        {
            // Hash the password with the salt
            byte[] passwordBytes = Encoding.UTF8.GetBytes(password);
            byte[] saltedPasswordBytes = new byte[passwordBytes.Length + salt.Length];
            Buffer.BlockCopy(passwordBytes, 0, saltedPasswordBytes, 0, passwordBytes.Length);
            Buffer.BlockCopy(salt, 0, saltedPasswordBytes, passwordBytes.Length, salt.Length);
            byte[] hashedPasswordBytes = new SHA256Managed().ComputeHash(saltedPasswordBytes);

            return hashedPasswordBytes;
        }
    }
}
