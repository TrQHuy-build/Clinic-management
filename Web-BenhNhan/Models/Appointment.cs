using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Web_BenhNhan.Models
{
    [Table("Appointment")]
    public class Appointment
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("appointment_id")]
        public int AppointmentId { get; set; }

        [Required]
        [MaxLength(100)]
        [Column("patient_name")]
        public string PatientName { get; set; } = string.Empty;

        [MaxLength(15)]
        [Column("phone")]
        public string? Phone { get; set; }

        [MaxLength(100)]
        [Column("email")]
        public string? Email { get; set; }

        [Column("service_id")]
        public int? ServiceId { get; set; }

        [Required]
        [Column("appointment_date")]
        public DateTime AppointmentDate { get; set; }

        [MaxLength(20)]
        [Column("status")]
        public string? Status { get; set; }

        [MaxLength(255)]
        [Column("notes")]
        public string? Notes { get; set; }
    }
}
