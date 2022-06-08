using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using WebApplication1.Data;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    public class HomeController : Controller
    {
        #region Fields

        private readonly IHostEnvironment _hostEnvironment;
        private readonly ApplicationDbContext _dbContext;

        #endregion
        #region Methods
        public async Task<IActionResult> StartProcess()
        {
            var json = await ReadJsonFile();
            var list = JsonConvert.DeserializeObject<List<TimeIrJsonModel>>(json);
            var events = FindEvents(list);
            await SaveEvents(events);
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> GetEventsByYear(int yearNumber)
        {
            var allEvents = await GetEvents();
            var result = await GetEventsByYear(allEvents, yearNumber);
            return Json(result);
        }




        public IActionResult Index()
        {
            return View();
        }


        #endregion
        #region Utilities


        private async Task<List<TimeIrJsonModel>> GetEventsByYear(List<Event> events, int yearNumber)
        {
            var result = new List<TimeIrJsonModel>();
            foreach (var e in events)
            {
                TimeIrJsonModel temp = null;
                switch (e.CalendarType)
                {
                    case CalendarType.Shamsi:
                        temp = ShamsiEvent(e, yearNumber);
                        break;
                    case CalendarType.Gregorian:
                        temp = GregorianEvent(e, yearNumber);
                        break;
                    case CalendarType.Ghamari:
                        temp = await GhamariEvent(e, yearNumber);
                        break;
                }

                result.Add(temp);
            }

            return result;
        }

        private async Task<TimeIrJsonModel> GhamariEvent(Event e, int yearNumber)
        {

            return await Task.Run(() =>
            {
                try
                {
                    if (e.MonthNumber == 2 && e.DayNumber > 29)
                    {

                    }

                    var now = DateTime.Now;
                    var pc = new PersianCalendar();
                    var hc = new UmAlQuraCalendar();
                    //var date = new DateTime(hc.GetYear(now), e.MonthNumber, e.DayNumber, hc);
                    var date = DateTime.ParseExact($"{hc.GetYear(now)}/{e.MonthNumber:00}/{e.DayNumber:00}","yyyy/MM/dd",CultureInfo.CreateSpecificCulture("ar-SA"));
                    if (pc.GetYear(date) < yearNumber)
                        date.AddYears(1);
                    return new TimeIrJsonModel()
                    {
                        DateTime = new TimeIrDateTimeModel(pc.GetYear(date), pc.GetMonth(date), pc.GetDayOfMonth(date)),
                        IsHoliday = e.IsHoliday,
                        Descriotion = e.Title,
                        GhamariDay = e.DayNumber,
                        GhamariMonth = e.MonthNumber
                    };
                }
                catch (Exception exception) 
                {
                    return null;

                }
            });
        }

        private TimeIrJsonModel GregorianEvent(Event e, int yearNumber)
        {
            var now = DateTime.Now;
            var pc = new PersianCalendar();
            var gregorianYear = pc.GetYear(now) < yearNumber ? now.Year + 1 : now.Year;
            var date = new DateTime(gregorianYear, e.MonthNumber, e.DayNumber);
            return new TimeIrJsonModel()
            {
                DateTime = new TimeIrDateTimeModel(pc.GetYear(date), pc.GetMonth(date), pc.GetDayOfMonth(date)),
                IsHoliday = e.IsHoliday,
                Descriotion = e.Title,
                MiladiDay = e.DayNumber,
                MiladiMonth = e.MonthNumber
            };
        }

        private TimeIrJsonModel ShamsiEvent(Event e, int yearNumber)
        {
            return new TimeIrJsonModel()
            {
                DateTime = new TimeIrDateTimeModel(yearNumber, e.MonthNumber, e.DayNumber),
                IsHoliday = e.IsHoliday,
                Descriotion = e.Title
            };
        }

        private async Task<List<Event>> GetEvents()
        {
            return await _dbContext.Events.ToListAsync();
        }

        private async Task SaveEvents(List<Event> events)
        {
            await _dbContext.Events.AddRangeAsync(events);
            await _dbContext.SaveChangesAsync();
        }
        private List<Event> FindEvents(List<TimeIrJsonModel> list)
        {
            var result = new List<Event>();
            var byDays = list.GroupBy(x => new { x.DateTime.Day, x.DateTime.Month }).ToList();
            foreach (var byDay in byDays)
            {
                var temp = FindEvents(byDay.Key.Month, byDay.Key.Day, byDay.ToList());
                result.AddRange(temp);
            }

            return result;
        }

        private IEnumerable<Event> FindEvents(int month, int day, List<TimeIrJsonModel> list)
        {
            var result = new List<Event>();
            var isHoliday = list.Any(x => x.IsHoliday);
            foreach (var timeIrJsonModel in list)
            {
                Event temp = new Event()
                {
                    IsHoliday = isHoliday,
                    Title = timeIrJsonModel.Descriotion
                };
                if (timeIrJsonModel.MiladiDay != 0 && timeIrJsonModel.MiladiMonth != 0)
                {
                    temp.CalendarType = CalendarType.Gregorian;
                    temp.DayNumber = timeIrJsonModel.MiladiDay;
                    temp.MonthNumber = timeIrJsonModel.MiladiMonth;
                }
                else if (timeIrJsonModel.GhamariDay != 0 && timeIrJsonModel.GhamariMonth != 0)

                {
                    temp.CalendarType = CalendarType.Ghamari;
                    temp.DayNumber = timeIrJsonModel.GhamariDay;
                    temp.MonthNumber = timeIrJsonModel.GhamariMonth;
                }
                else
                {
                    temp.CalendarType = CalendarType.Shamsi;
                    temp.DayNumber = timeIrJsonModel.DateTime.Day;
                    temp.MonthNumber = timeIrJsonModel.DateTime.Month;
                }

                result.Add(temp);
            }

            return result;
        }


        private async Task<string> ReadJsonFile()
        {
            var path = $"{_hostEnvironment.ContentRootPath}/wwwroot/Time.ir.json";
            var content = await System.IO.File.ReadAllTextAsync(path);
            return content;
        }

        #endregion
        #region Ctor
        public HomeController(IHostEnvironment hostEnvironment, ApplicationDbContext dbContext)
        {
            _hostEnvironment = hostEnvironment;
            _dbContext = dbContext;
        }
        #endregion




    }
}
