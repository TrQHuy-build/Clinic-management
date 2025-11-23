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
        }
    }
}
