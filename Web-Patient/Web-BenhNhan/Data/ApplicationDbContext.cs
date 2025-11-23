using Microsoft.EntityFrameworkCore;
using Web_BenhNhan.Models;

namespace Web_BenhNhan.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Appointment> Appointments { get; set; }
        public DbSet<UserAccount> UserAccounts { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Additional configuration if needed
            modelBuilder.Entity<Appointment>(entity =>
            {
                entity.ToTable("Appointment");
                entity.HasKey(e => e.AppointmentId);
                entity.Property(e => e.AppointmentId).HasColumnName("appointment_id");
                entity.Property(e => e.PatientName).HasColumnName("patient_name").IsRequired().HasMaxLength(100);
                entity.Property(e => e.Phone).HasColumnName("phone").HasMaxLength(15);
                entity.Property(e => e.Email).HasColumnName("email").HasMaxLength(100);
                entity.Property(e => e.ServiceId).HasColumnName("service_id");
                entity.Property(e => e.AppointmentDate).HasColumnName("appointment_date").IsRequired();
                entity.Property(e => e.Status).HasColumnName("status").HasMaxLength(20);
                entity.Property(e => e.Notes).HasColumnName("notes").HasMaxLength(255);
            });

            modelBuilder.Entity<UserAccount>(entity =>
            {
                entity.ToTable("UserAccount");
                entity.HasKey(e => e.UserId);
                entity.Property(e => e.UserId).HasColumnName("user_id");
                entity.Property(e => e.FullName).HasColumnName("fullname").IsRequired().HasMaxLength(100);
                entity.Property(e => e.Phone).HasColumnName("phone").IsRequired().HasMaxLength(15);
                entity.Property(e => e.Email).HasColumnName("email").HasMaxLength(255);
                entity.Property(e => e.PasswordHash).HasColumnName("password_hash").IsRequired().HasMaxLength(255);
                entity.Property(e => e.Role).HasColumnName("role").IsRequired().HasMaxLength(20).HasDefaultValue("patient");
                entity.Property(e => e.Status).HasColumnName("status").HasMaxLength(20).HasDefaultValue("active");
                entity.Property(e => e.CreatedAt).HasColumnName("created_at").HasDefaultValueSql("GETDATE()");
            });
        }
    }
}
