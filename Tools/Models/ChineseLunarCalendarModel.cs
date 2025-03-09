using System.Globalization;

namespace Tools.Models;

public class ChineseLunarCalendarModel
{
    public DateTime GregorianDate { get; set; }
    public int LunarYear { get; set; }
    public int LunarMonth { get; set; }
    public int LunarDay { get; set; }
    public string AnimalSign { get; set; } = string.Empty;
    public bool IsLeapMonth { get; set; }
    public string MonthName { get; set; } = string.Empty;
    public string DayName { get; set; } = string.Empty;
    
    // Calendar display properties
    public List<List<CalendarDay>> CalendarWeeks { get; set; } = new();
    public string MonthTitle { get; set; } = string.Empty;
}

public class CalendarDay
{
    public int Day { get; set; }
    public int LunarDay { get; set; }
    public bool IsCurrentMonth { get; set; }
    public bool IsToday { get; set; }
    public bool IsWeekend { get; set; }
    public bool IsHoliday { get; set; }
    public string HolidayName { get; set; } = string.Empty;
} 