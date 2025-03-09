using FluentAssertions;
using Tools.Models;

namespace Tools.Tests.Models;

[TestFixture]
public class ChineseLunarCalendarModelTests
{
    [Test]
    public void ChineseLunarCalendarModel_DefaultProperties_ShouldBeInitialized()
    {
        // Act
        var model = new ChineseLunarCalendarModel();

        // Assert
        model.AnimalSign.Should().BeEmpty();
        model.MonthName.Should().BeEmpty();
        model.DayName.Should().BeEmpty();
        model.CalendarWeeks.Should().NotBeNull();
        model.CalendarWeeks.Should().BeEmpty();
        model.MonthTitle.Should().BeEmpty();
    }

    [Test]
    public void ChineseLunarCalendarModel_SetProperties_ShouldRetainValues()
    {
        // Arrange
        var date = new DateTime(2023, 1, 22);
        var weeks = new List<List<CalendarDay>>
        {
            new() { new CalendarDay { Day = 1 }, new CalendarDay { Day = 2 } }
        };

        // Act
        var model = new ChineseLunarCalendarModel
        {
            GregorianDate = date,
            LunarYear = 2023,
            LunarMonth = 1,
            LunarDay = 1,
            AnimalSign = "Rabbit",
            IsLeapMonth = false,
            MonthName = "Test Month",
            DayName = "Test Day",
            CalendarWeeks = weeks,
            MonthTitle = "January 2023"
        };

        // Assert
        model.GregorianDate.Should().Be(date);
        model.LunarYear.Should().Be(2023);
        model.LunarMonth.Should().Be(1);
        model.LunarDay.Should().Be(1);
        model.AnimalSign.Should().Be("Rabbit");
        model.IsLeapMonth.Should().BeFalse();
        model.MonthName.Should().Be("Test Month");
        model.DayName.Should().Be("Test Day");
        model.CalendarWeeks.Should().BeSameAs(weeks);
        model.MonthTitle.Should().Be("January 2023");
    }
}

[TestFixture]
public class CalendarDayTests
{
    [Test]
    public void CalendarDay_DefaultProperties_ShouldBeInitialized()
    {
        // Act
        var day = new CalendarDay();

        // Assert
        day.Day.Should().Be(0);
        day.LunarDay.Should().Be(0);
        day.IsCurrentMonth.Should().BeFalse();
        day.IsToday.Should().BeFalse();
        day.IsWeekend.Should().BeFalse();
        day.IsHoliday.Should().BeFalse();
        day.HolidayName.Should().BeEmpty();
    }

    [Test]
    public void CalendarDay_SetProperties_ShouldRetainValues()
    {
        // Act
        var day = new CalendarDay
        {
            Day = 15,
            LunarDay = 20,
            IsCurrentMonth = true,
            IsToday = true,
            IsWeekend = true,
            IsHoliday = true,
            HolidayName = "Test Holiday"
        };

        // Assert
        day.Day.Should().Be(15);
        day.LunarDay.Should().Be(20);
        day.IsCurrentMonth.Should().BeTrue();
        day.IsToday.Should().BeTrue();
        day.IsWeekend.Should().BeTrue();
        day.IsHoliday.Should().BeTrue();
        day.HolidayName.Should().Be("Test Holiday");
    }
} 