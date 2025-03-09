using System.Globalization;
using NodaTime;

namespace Tools.Models;

public class ChineseLunarCalendarService : IChineseLunarCalendarService
{
    private readonly ChineseLunisolarCalendar _chineseCalendar = new();
    private static readonly string[] AnimalSigns = { "Rat", "Ox", "Tiger", "Rabbit", "Dragon", "Snake", "Horse", "Goat", "Monkey", "Rooster", "Dog", "Pig" };
    private static readonly string[] StemNames = { "Jia", "Yi", "Bing", "Ding", "Wu", "Ji", "Geng", "Xin", "Ren", "Gui" };
    private static readonly string[] BranchNames = { "Zi", "Chou", "Yin", "Mao", "Chen", "Si", "Wu", "Wei", "Shen", "You", "Xu", "Hai" };
    
    public ChineseLunarCalendarModel GetLunarCalendar(DateTime date)
    {
        try
        {
            // Validate date is within supported range
            if (date < new DateTime(1901, 2, 19) || date > new DateTime(2101, 1, 28))
            {
                throw new ArgumentException($"The date {date:yyyy-MM-dd} is outside the supported range for the Chinese lunar calendar. Supported range is 1901-02-19 to 2101-01-28.");
            }
            
            var model = new ChineseLunarCalendarModel
            {
                GregorianDate = date,
                LunarYear = _chineseCalendar.GetYear(date),
                LunarMonth = _chineseCalendar.GetMonth(date),
                LunarDay = _chineseCalendar.GetDayOfMonth(date)
            };
            
            // Use the correct era (1 is the only valid era for ChineseLunisolarCalendar)
            model.IsLeapMonth = _chineseCalendar.IsLeapMonth(model.LunarYear, model.LunarMonth, 1);
            
            // Calculate animal sign (year % 12)
            model.AnimalSign = AnimalSigns[(model.LunarYear - 4) % 12];
            
            // Calculate stem-branch cycle (Heavenly Stems and Earthly Branches)
            int stemIndex = (model.LunarYear - 4) % 10;
            int branchIndex = (model.LunarYear - 4) % 12;
            model.MonthName = $"{StemNames[stemIndex]} {BranchNames[branchIndex]}";
            
            // Generate calendar view
            GenerateCalendarView(model);
            
            return model;
        }
        catch (ArgumentOutOfRangeException ex)
        {
            // Handle dates outside the supported range (1901-2101)
            throw new ArgumentException($"The date {date:yyyy-MM-dd} is outside the supported range for the Chinese lunar calendar. Supported range is 1901-02-19 to 2101-01-28.", ex);
        }
    }
    
    private void GenerateCalendarView(ChineseLunarCalendarModel model)
    {
        DateTime firstDayOfMonth = new DateTime(model.GregorianDate.Year, model.GregorianDate.Month, 1);
        int daysInMonth = DateTime.DaysInMonth(model.GregorianDate.Year, model.GregorianDate.Month);
        
        // Get the day of week for the first day (0 = Sunday, 1 = Monday, etc.)
        int firstDayOfWeek = (int)firstDayOfMonth.DayOfWeek;
        
        // Calculate previous month days to show
        DateTime prevMonth = firstDayOfMonth.AddMonths(-1);
        int daysInPrevMonth = DateTime.DaysInMonth(prevMonth.Year, prevMonth.Month);
        
        // Set month title
        model.MonthTitle = $"{model.GregorianDate:MMMM yyyy} - Lunar {model.LunarMonth}/{model.LunarYear}";
        
        // Generate weeks
        List<CalendarDay> currentWeek = new();
        
        // Add days from previous month
        for (int i = 0; i < firstDayOfWeek; i++)
        {
            int prevMonthDay = daysInPrevMonth - firstDayOfWeek + i + 1;
            DateTime prevDate = new DateTime(prevMonth.Year, prevMonth.Month, prevMonthDay);
            
            int lunarDay;
            try
            {
                lunarDay = _chineseCalendar.GetDayOfMonth(prevDate);
            }
            catch
            {
                // If the date is outside the supported range, just use a placeholder
                lunarDay = 1;
            }
            
            currentWeek.Add(new CalendarDay
            {
                Day = prevMonthDay,
                LunarDay = lunarDay,
                IsCurrentMonth = false,
                IsToday = prevDate.Date == DateTime.Today.Date,
                IsWeekend = i == 0 || i == 6 // Sunday or Saturday
            });
        }
        
        // Add days from current month
        for (int day = 1; day <= daysInMonth; day++)
        {
            DateTime currentDate = new DateTime(model.GregorianDate.Year, model.GregorianDate.Month, day);
            int dayOfWeek = (int)currentDate.DayOfWeek;
            
            int lunarDay;
            try
            {
                lunarDay = _chineseCalendar.GetDayOfMonth(currentDate);
            }
            catch
            {
                // If the date is outside the supported range, just use a placeholder
                lunarDay = day;
            }
            
            currentWeek.Add(new CalendarDay
            {
                Day = day,
                LunarDay = lunarDay,
                IsCurrentMonth = true,
                IsToday = currentDate.Date == DateTime.Today.Date,
                IsWeekend = dayOfWeek == 0 || dayOfWeek == 6 // Sunday or Saturday
            });
            
            // Start a new week if we've reached the end of a week
            if (dayOfWeek == 6 || day == daysInMonth)
            {
                model.CalendarWeeks.Add(new List<CalendarDay>(currentWeek));
                currentWeek.Clear();
            }
        }
        
        // If the last week is not complete, add days from next month
        if (currentWeek.Count > 0)
        {
            DateTime nextMonth = firstDayOfMonth.AddMonths(1);
            int nextMonthDay = 1;
            
            while (currentWeek.Count < 7)
            {
                DateTime nextDate = new DateTime(nextMonth.Year, nextMonth.Month, nextMonthDay);
                int dayOfWeek = (int)nextDate.DayOfWeek;
                
                int lunarDay;
                try
                {
                    lunarDay = _chineseCalendar.GetDayOfMonth(nextDate);
                }
                catch
                {
                    // If the date is outside the supported range, just use a placeholder
                    lunarDay = nextMonthDay;
                }
                
                currentWeek.Add(new CalendarDay
                {
                    Day = nextMonthDay,
                    LunarDay = lunarDay,
                    IsCurrentMonth = false,
                    IsToday = nextDate.Date == DateTime.Today.Date,
                    IsWeekend = dayOfWeek == 0 || dayOfWeek == 6 // Sunday or Saturday
                });
                
                nextMonthDay++;
            }
            
            model.CalendarWeeks.Add(new List<CalendarDay>(currentWeek));
        }
    }
} 