using Microsoft.AspNetCore.Mvc;
using MVCProjectEraaSoft.Data;
using MVCProjectEraaSoft.Models;
using System.Linq;

public class DoctorController : Controller
{
    private readonly ApplicationDbContext _context;

    public DoctorController(ApplicationDbContext context)
    {
        _context = context;
    }

    // صفحة البداية
    public IActionResult Index()
    {
        return View();
    }

    // عرض نموذج الحجز والبحث
    [HttpGet]
    public IActionResult Appointment(string name = "", string specialization = "", int page = 1)
    {
        int pageSize = 8;
        var query = _context.Doctors.AsQueryable();
        ViewData["Doctors"] = query.ToList();

        if (!string.IsNullOrEmpty(name))
        {
            query = query.Where(d => d.Name.Contains(name));
            ViewBag.name = name;

        }

        if (!string.IsNullOrEmpty(specialization))
        {
            query = query.Where(d => d.Specialization == specialization);
            ViewBag.specialization = specialization;

        }

        
        query = query.Skip((page - 1) * 8).Take(8);
        ViewBag.TotalCountOfDoctor = Math.Ceiling(_context.Doctors.Count() / 8.0);
        

        ViewBag.Specializations = _context.Doctors
            .Select(d => d.Specialization)
            .Distinct()
            .ToList();

        return View(query);
    }
    [HttpGet]
    public IActionResult BookAppointment(int doctorId)
    {
        var doctor = _context.Doctors.FirstOrDefault(d => d.Id == doctorId);
        if (doctor == null)
        {
            return NotFound();
        }

        ViewBag.Doctor = doctor;
        return View(); // هيروح لـ Views/Doctor/BookAppointment.cshtml
    }

    // تنفيذ الحجز
    [HttpPost]
    public IActionResult BookAppointment(int doctorId, string PatientName, DateTime? date, TimeSpan? time)
    {
        if (!date.HasValue || !time.HasValue || string.IsNullOrWhiteSpace(PatientName))
        {
            TempData["Error"] = "Please fill in all required fields.";
            return RedirectToAction("BookAppointment", new { doctorId = doctorId });
        }

        var doctor = _context.Doctors.FirstOrDefault(d => d.Id == doctorId);
        if (doctor == null)
        {
            return NotFound();
        }

        // 1. التاريخ ميكونش في الماضي
        if (date.Value.Date < DateTime.Today)
        {
            TempData["Error"] = "You cannot book an appointment in the past.";
            return RedirectToAction("BookAppointment", new { doctorId = doctorId });
        }

        // 2. الوقت لازم يكون بين 8 صباحًا و8 مساءً
        var start = new TimeSpan(8, 0, 0);
        var end = new TimeSpan(20, 0, 0);
        if (time < start || time > end)
        {
            TempData["Error"] = "Appointments must be booked between 8:00 AM and 8:00 PM.";
            return RedirectToAction("BookAppointment", new { doctorId = doctorId });
        }

        // 3. لا يمكن الحجز يوم الجمعة أو السبت
        var dayOfWeek = date.Value.DayOfWeek;
        if (dayOfWeek == DayOfWeek.Friday || dayOfWeek == DayOfWeek.Saturday)
        {
            TempData["Error"] = "Appointments cannot be booked on Fridays or Saturdays.";
            return RedirectToAction("BookAppointment", new { doctorId = doctorId });
        }

        // 4. لا يمكن الحجز في نفس الوقت لنفس الدكتور
        bool timeSlotTaken = _context.Appointments.Any(a =>
            a.DoctorId == doctorId &&
            a.Date.Date == date.Value.Date &&
            a.Time == time.Value
        );

        if (timeSlotTaken)
        {
            TempData["Error"] = "This time slot is already booked for the selected doctor.";
            return RedirectToAction("BookAppointment", new { doctorId = doctorId });
        }

        // لو كل حاجة تمام، سجل الحجز
        var appointment = new Appointment
        {
            DoctorId = doctorId,
            DoctorName = doctor.Name,
            PatientName = PatientName,
            Date = date.Value,
            Time = time.Value
        };

        _context.Appointments.Add(appointment);
        _context.SaveChanges();

        TempData["Success"] = "Appointment booked successfully!";
        return RedirectToAction("AllAppointments");
    }



    // عرض كل المواعيد
    public IActionResult AllAppointments()
    {
        var appointments = _context.Appointments.ToList();
        return View(appointments);
    }
}
