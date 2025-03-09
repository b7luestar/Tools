using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using Tools.Controllers;
using Tools.Models;

namespace Tools.Tests.Controllers;

[TestFixture]
public class CalendarControllerTests : IDisposable
{
    private CalendarController _controller;
    private IChineseLunarCalendarService _calendarService;
    private ILogger<CalendarController> _logger;
    private bool _disposed;

    [SetUp]
    public void Setup()
    {
        _calendarService = Substitute.For<IChineseLunarCalendarService>();
        _logger = Substitute.For<ILogger<CalendarController>>();
        _controller = new CalendarController(_calendarService, _logger);
    }
    
    [TearDown]
    public void TearDown()
    {
        Dispose(true);
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (_disposed)
            return;

        if (disposing)
        {
            // Dispose managed resources if any
            // In this case, we don't have any resources that need explicit disposal
        }

        _disposed = true;
    }

    [Test]
    public void Index_ShouldReturnViewResult()
    {
        // Act
        var result = _controller.Index();

        // Assert
        result.Should().BeOfType<ViewResult>();
    }

    [Test]
    public void Lunar_WithNoDate_ShouldUseToday()
    {
        // Arrange
        var today = DateTime.Today;
        var model = new ChineseLunarCalendarModel { GregorianDate = today };
        _calendarService.GetLunarCalendar(Arg.Is<DateTime>(d => d.Date == today.Date))
            .Returns(model);

        // Act
        var result = _controller.Lunar();

        // Assert
        result.Should().BeOfType<ViewResult>();
        var viewResult = (ViewResult)result;
        viewResult.Model.Should().BeEquivalentTo(model);
    }

    [Test]
    public void Lunar_WithSpecificDate_ShouldUseProvidedDate()
    {
        // Arrange
        var specificDate = new DateTime(2023, 1, 22);
        var model = new ChineseLunarCalendarModel { GregorianDate = specificDate };
        _calendarService.GetLunarCalendar(Arg.Is<DateTime>(d => d.Date == specificDate.Date))
            .Returns(model);

        // Act
        var result = _controller.Lunar(specificDate);

        // Assert
        result.Should().BeOfType<ViewResult>();
        var viewResult = (ViewResult)result;
        viewResult.Model.Should().BeEquivalentTo(model);
    }

    [Test]
    public void Lunar_WhenExceptionOccurs_ShouldReturnErrorView()
    {
        // Arrange
        var exception = new ArgumentException("Test exception");
        _calendarService.GetLunarCalendar(Arg.Any<DateTime>())
            .Throws(exception);

        // Set up HttpContext.TraceIdentifier
        var httpContext = new Microsoft.AspNetCore.Http.DefaultHttpContext();
        httpContext.TraceIdentifier = "test-trace-id";
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = httpContext
        };

        // Act
        var result = _controller.Lunar();

        // Assert
        result.Should().BeOfType<ViewResult>();
        var viewResult = (ViewResult)result;
        viewResult.ViewName.Should().Be("Error");
        viewResult.Model.Should().BeOfType<ErrorViewModel>();
        var errorModel = (ErrorViewModel)viewResult.Model;
        errorModel.RequestId.Should().Be("test-trace-id");
    }

    [Test]
    public void LunarApi_WithNoDate_ShouldReturnJsonResult()
    {
        // Arrange
        var today = DateTime.Today;
        var model = new ChineseLunarCalendarModel { GregorianDate = today };
        _calendarService.GetLunarCalendar(Arg.Is<DateTime>(d => d.Date == today.Date))
            .Returns(model);

        // Act
        var result = _controller.LunarApi();

        // Assert
        result.Should().BeOfType<JsonResult>();
        var jsonResult = (JsonResult)result;
        jsonResult.Value.Should().BeEquivalentTo(model);
    }

    [Test]
    public void LunarApi_WithSpecificDate_ShouldUseProvidedDate()
    {
        // Arrange
        var specificDate = new DateTime(2023, 1, 22);
        var model = new ChineseLunarCalendarModel { GregorianDate = specificDate };
        _calendarService.GetLunarCalendar(Arg.Is<DateTime>(d => d.Date == specificDate.Date))
            .Returns(model);

        // Act
        var result = _controller.LunarApi(specificDate);

        // Assert
        result.Should().BeOfType<JsonResult>();
        var jsonResult = (JsonResult)result;
        jsonResult.Value.Should().BeEquivalentTo(model);
    }

    [Test]
    public void LunarApi_WhenExceptionOccurs_ShouldReturn500StatusCode()
    {
        // Arrange
        var exception = new ArgumentException("Test exception");
        _calendarService.GetLunarCalendar(Arg.Any<DateTime>())
            .Throws(exception);

        // Act
        var result = _controller.LunarApi();

        // Assert
        result.Should().BeOfType<ObjectResult>();
        var objectResult = (ObjectResult)result;
        objectResult.StatusCode.Should().Be(500);
    }
} 