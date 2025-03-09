namespace Tools.Models;

public interface IChineseLunarCalendarService
{
    ChineseLunarCalendarModel GetLunarCalendar(DateTime date);
} 