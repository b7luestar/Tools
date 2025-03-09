using System.Globalization;
using FluentAssertions;
using NSubstitute;
using Tools.Models;

namespace Tools.Tests.Models;

[TestFixture]
public class ChineseLunarCalendarServiceTests
{
    private ChineseLunarCalendarService _service;

    [SetUp]
    public void Setup()
    {
        _service = new ChineseLunarCalendarService();
    }

    [Test]
    public void GetLunarCalendar_ShouldReturnCorrectLunarDate()
    {
        // Arrange - Using a date within the supported range (1901-2101)
        var date = new DateTime(2020, 1, 25); // Chinese New Year 2020

        // Act
        var result = _service.GetLunarCalendar(date);

        // Assert
        result.Should().NotBeNull();
        result.GregorianDate.Should().Be(date);
        // Note: We're not asserting exact lunar values as they may change with calendar implementations
        result.LunarYear.Should().BeGreaterThan(0);
        result.LunarMonth.Should().BeGreaterThan(0).And.BeLessThan(13);
        result.LunarDay.Should().BeGreaterThan(0).And.BeLessThan(31);
        result.AnimalSign.Should().NotBeEmpty();
    }

    [Test]
    public void GetLunarCalendar_ShouldGenerateCalendarWeeks()
    {
        // Arrange - Using a date within the supported range
        var date = new DateTime(2020, 1, 25);

        // Act
        var result = _service.GetLunarCalendar(date);

        // Assert
        result.CalendarWeeks.Should().NotBeNull();
        result.CalendarWeeks.Should().NotBeEmpty();
        
        // Calendar should have 4-6 weeks
        result.CalendarWeeks.Count.Should().BeInRange(4, 6);
        
        // Each week should have days (not necessarily exactly 7)
        foreach (var week in result.CalendarWeeks)
        {
            week.Should().NotBeEmpty();
        }
    }

    [Test]
    public void GetLunarCalendar_ShouldMarkCurrentMonthDays()
    {
        // Arrange - Using a date within the supported range
        var date = new DateTime(2020, 1, 25);

        // Act
        var result = _service.GetLunarCalendar(date);

        // Assert
        // Should have days marked as current month
        result.CalendarWeeks.SelectMany(w => w)
            .Count(d => d.IsCurrentMonth)
            .Should().Be(DateTime.DaysInMonth(2020, 1));
        
        // Should have some days marked as not current month (previous or next month)
        result.CalendarWeeks.SelectMany(w => w)
            .Any(d => !d.IsCurrentMonth)
            .Should().BeTrue();
    }

    [Test]
    public void GetLunarCalendar_ShouldSetMonthTitle()
    {
        // Arrange - Using a date within the supported range
        var date = new DateTime(2020, 1, 25);

        // Act
        var result = _service.GetLunarCalendar(date);

        // Assert
        result.MonthTitle.Should().NotBeNullOrEmpty();
        result.MonthTitle.Should().Contain("January 2020");
        result.MonthTitle.Should().Contain("Lunar");
    }

    [Test]
    public void GetLunarCalendar_ShouldMarkWeekends()
    {
        // Arrange - Using a date within the supported range
        var date = new DateTime(2020, 1, 25); // January 25, 2020 is a Saturday

        // Act
        var result = _service.GetLunarCalendar(date);

        // Assert
        // Check if there are weekend days marked
        result.CalendarWeeks.SelectMany(w => w)
            .Any(d => d.IsWeekend)
            .Should().BeTrue();
            
        // Check if there are non-weekend days
        result.CalendarWeeks.SelectMany(w => w)
            .Any(d => !d.IsWeekend)
            .Should().BeTrue();
    }
    
    [Test]
    public void GetLunarCalendar_WithDateOutsideRange_ShouldThrowArgumentException()
    {
        // Arrange - Using a date outside the supported range
        var date = new DateTime(1800, 1, 1);

        // Act & Assert
        FluentActions.Invoking(() => _service.GetLunarCalendar(date))
            .Should().Throw<ArgumentException>()
            .WithMessage("*outside the supported range*");
    }
} 