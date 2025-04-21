using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MVCProjectEraaSoft.Models
{
    public class Appointment
    {
        [Key]
        public int Id { get; set; }
        public string? PatientName { get; set; }

        [Required]
        public int DoctorId { get; set; }
        
        [ForeignKey(nameof(DoctorId))]
        public Doctor Doctor { get; set; }
        public string DoctorName { get; set; }
        [Required]
       // [Display(Name = "Disease Name")]
        public string DiseaseName { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Appointment Date")]
        public DateTime Date { get; set; }

        [Required]
        [DataType(DataType.Time)]
        [Display(Name = "Appointment Time")]
        public TimeSpan Time { get; set; }
    }
}