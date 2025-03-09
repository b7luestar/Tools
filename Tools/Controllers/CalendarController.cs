using Microsoft.AspNetCore.Mvc;
using Tools.Models;

namespace Tools.Controllers;

public class CalendarController : Controller
{
    private readonly IChineseLunarCalendarService _calendarService;
    private readonly ILogger<CalendarController> _logger;

    public CalendarController(IChineseLunarCalendarService calendarService, ILogger<CalendarController> logger)
    {
        _calendarService = calendarService;
        _logger = logger;
    }

    // GET: /Calendar
    public IActionResult Index()
    {
        return View();
    }

    // GET: /Calendar/Lunar
    public IActionResult Lunar(DateTime? date = null)
    {
        try
        {
            DateTime targetDate = date ?? DateTime.Today;
            var model = _calendarService.GetLunarCalendar(targetDate);
            return View(model);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating lunar calendar");
            return View("Error", new ErrorViewModel { RequestId = HttpContext.TraceIdentifier });
        }
    }

    // GET: /Calendar/LunarApi
    [HttpGet]
    [Route("api/calendar/lunar")]
    public IActionResult LunarApi([FromQuery] DateTime? date = null)
    {
        try
        {
            DateTime targetDate = date ?? DateTime.Today;
            var model = _calendarService.GetLunarCalendar(targetDate);
            return Json(model);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating lunar calendar API response");
            return StatusCode(500, new { error = "Failed to generate lunar calendar" });
        }
    }
} 