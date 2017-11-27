using System.Data.Entity;
using GH.DAL.Model;

namespace GH.DAL.Context
{
    public class DataContext : DbContext
    {
        public DbSet<ApplicationSetting> ApplicationSettings { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }




        public DbSet<NextItemNo> NextItemNo { get; set; }
        public DbSet<ClaimNextItemNo> ClaimNextItemNo { get; set; }

        public DbSet<Customer> Customers { get; set; }
        public DbSet<Staff> Staffs { get; set; }
        //public DbSet<StaffUser> StaffUsers { get; set; }
        public DbSet<StaffPosition> StaffPositions { get; set; }
        public DbSet<Transport> Transports { get; set; }
        public DbSet<Insurance> Insurances { get; set; }
        public DbSet<Brand> Brands { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Color> Colors { get; set; }
        public DbSet<ProductType> ProductTypes { get; set; }
        public DbSet<ProductAccessorie> ProductAccessories { get; set; }
        public DbSet<Cause> Causes { get; set; }
        public DbSet<Repair> Repairs { get; set; }
        public DbSet<Claim> Claims { get; set; }
        public DbSet<WorkingStatus> WorkingStatus { get; set; }
        public DbSet<CloseStatus> CloseStatus { get; set; }
        public DbSet<ProcessStatus> ProcessStatus { get; set; }

        public DbSet<ClaimCause> ClaimCauses { get; set; }
        public DbSet<ClaimStatus> ClaimStatuies { get; set; }
        public DbSet<RepairCause> RepairCauses { get; set; }
        public DbSet<RepairCauseEstimate> RepairCauseEstimates { get; set; }
        
        public DbSet<RepairStatus> RepairStatuies { get; set; }
        //public DbSet<RepairBack> RepairBacks { get; set; }
        public DbSet<FileUpload> FileUploads { get; set; }
        public DbSet<Remind> Reminds { get; set; }
        public DbSet<RemindHistory> RemindHistories { get; set; }
        public DbSet<TrackingCounter> TrackingCounters { get; set; }

    }

    public class DataContext2 : DbContext
    {
        public DbSet<Repairs> Repairs { get; set; }
        public DbSet<Claims> Claims { get; set; }
    }
}
