using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;

public class IndexModel : PageModel
{
    [BindProperty]
    public TimetableRequestModel TimetableRequest { get; set; } = new();

    public List<List<string>> Timetable { get; private set; } = new();

    public class TimetableRequestModel
    {
        [Required(ErrorMessage = "Required")]
        [Range(1, 7, ErrorMessage = "1-7 days")]
        public int WorkingDays { get; set; }

        [Required(ErrorMessage = "Required")]
        [Range(1, 8, ErrorMessage = "1-8 subjects/day")]
        public int SubjectsPerDay { get; set; }

        [Required(ErrorMessage = "Required")]
        [Range(1, int.MaxValue, ErrorMessage = "At least 1 subject")]
        public int TotalSubjects { get; set; }

        public List<SubjectHours> Subjects { get; set; } = new();
    }

    public class SubjectHours
    {
        [Required(ErrorMessage = "Subject name required")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Hours required")]
        [Range(1, int.MaxValue, ErrorMessage = "Minimum 1 hour")]
        public int Hours { get; set; }
    }

    public IActionResult OnPost()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        var totalHours = TimetableRequest.WorkingDays * TimetableRequest.SubjectsPerDay;
        
        if (TimetableRequest.Subjects.Sum(s => s.Hours) != totalHours)
        {
            ModelState.AddModelError("", $"Total hours must equal {totalHours}");
            return Page();
        }

        Timetable = GenerateTimetable();
        return Page();
    }

    private List<List<string>> GenerateTimetable()
    {
        var rng = new Random();
        var subjects = TimetableRequest.Subjects
            .SelectMany(s => Enumerable.Repeat(s.Name, s.Hours))
            .OrderBy(_ => rng.Next())
            .ToList();

        var timetable = new List<List<string>>();
        int index = 0;

        for (int day = 0; day < TimetableRequest.WorkingDays; day++)
        {
            var dailySubjects = new List<string>();
            for (int period = 0; period < TimetableRequest.SubjectsPerDay; period++)
            {
                dailySubjects.Add(index < subjects.Count ? subjects[index++] : "Free");
            }
            timetable.Add(dailySubjects);
        }

        return timetable;
    }
}