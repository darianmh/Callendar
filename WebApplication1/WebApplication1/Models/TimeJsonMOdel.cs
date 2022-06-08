using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication1.Models
{
    public class TimeIrJsonModel
    {
        public TimeIrDateTimeModel DateTime { get; set; }
        public string Descriotion { get; set; }
        public bool IsHoliday { get; set; } = false;
        public int MiladiMonth { get; set; } = 0;
        public int MiladiDay { get; set; } = 0;
        public int GhamariMonth { get; set; } = 0;
        public int GhamariDay { get; set; } = 0;
    }

    public class TimeIrDateTimeModel
    {
        public int Year { get; set; }
        public int Month { get; set; }
        public int Day { get; set; }

        public TimeIrDateTimeModel(int year, int month, int day)
        {
            Year = year;
            Month = month;
            Day = day;
        }
    }
}
