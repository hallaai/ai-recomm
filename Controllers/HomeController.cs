using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Text.Json;
using SpreadsheetWebApp.Models;
using System.Diagnostics;

//using OfficeOpenXml;
//using System.Collections.Generic;
//using System.IO;
//using System.Linq;
//using System.Text;
//using System.Text.Json;
//using Microsoft.AspNetCore.Http;

namespace SpreadsheetWebApp.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

/*    public IActionResult Index(string selectedCity = null)
    {
        var addresses = GetAddressesFromCsv("wwwroot/addresses.csv");
        var cities = GetCitiesFromCsv("wwwroot/municipalies.csv");
        ViewBag.Addresses = addresses;
        ViewBag.Cities = cities;
        ViewBag.SelectedCity = selectedCity;
        return View();
    }*/

/*
    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }*/
        [HttpPost]
    public IActionResult ReceiveSelectedValues([FromBody] List<string> selectedValues)
    {
        _logger.LogInformation("Selected values: {SelectedValues}", JsonSerializer.Serialize(selectedValues));
        return Ok();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}

