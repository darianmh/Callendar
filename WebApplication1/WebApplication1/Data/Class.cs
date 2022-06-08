using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication1.Data
{
    public enum CalendarType
    {
        Gregorian = 0,
        Shamsi = 1,
        Ghamari = 2
    }
    public class Event
    {
        [Key]
        public int Id { get; set; }
        public int MonthNumber { get; set; }
        public int DayNumber { get; set; }
        public string Title { get; set; }
        public bool IsHoliday { get; set; }
        public CalendarType CalendarType { get; set; }
    }

    public class ShamsiCalendarEvent
    {
        [Key]
        public int Id { get; set; }
        public string Title { get; set; }
        public bool IsHoliday { get; set; }
        public DateTime UtcDateTime { get; set; }
    }
}
