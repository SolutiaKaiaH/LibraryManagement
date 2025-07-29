using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LibraryManagement.Shared.Models
{
    [Table("Calendar")]
    public class CalendarObj
    {
        [Key]
        public int Id { get; set; }

        public string Subject { get; set; } = "";

        public DateTime StartTime { get; set; }

        public DateTime EndTime { get; set; }

        public bool IsAllDay { get; set; }

        public string Location { get; set; } = "";

        public string Description { get; set; } = "";

        public int CalendarId { get; set; }

        public int OwnerId { get; set; }

        public bool IsPlanned { get; set; }

        public string? RecurrenceRule { get; set; }

        public int? RecurrenceID { get; set; }

        public string? RecurrenceException { get; set; }
        public string? StartTimezone { get; set; }
        public string? EndTimezone { get; set; }
    }
}
