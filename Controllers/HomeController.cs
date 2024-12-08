using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using SpreadsheetWebApp.Models;
using OfficeOpenXml;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Http;

namespace SpreadsheetWebApp.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

    public IActionResult Index()
    {
        var addresses = GetAddressesFromCsv("wwwroot/addresses.csv");
        var cities = GetCitiesFromCsv("wwwroot/municipalies.csv");
        ViewBag.Addresses = addresses;
        ViewBag.Cities = cities;
        return View();
    }

    [HttpPost]
    public IActionResult Export(string dataEntries)
    {
        var entries = JsonSerializer.Deserialize<List<DataEntry>>(dataEntries);
        ExcelPackage.LicenseContext = LicenseContext.NonCommercial; // Set the license context

        using (var package = new ExcelPackage())
        {
            var worksheet = package.Workbook.Worksheets.Add("Data");
            worksheet.Cells[1, 1].Value = "City";
            worksheet.Cells[1, 2].Value = "Address";
            worksheet.Cells[1, 3].Value = "Client";
            worksheet.Cells[1, 4].Value = "Start Date";
            worksheet.Cells[1, 5].Value = "Start Time";
            worksheet.Cells[1, 6].Value = "Load(mins)";

            for (int i = 0; i < entries.Count; i++)
            {
                worksheet.Cells[i + 2, 1].Value = entries[i].City;
                worksheet.Cells[i + 2, 2].Value = entries[i].Address;
                worksheet.Cells[i + 2, 3].Value = entries[i].Client;
                worksheet.Cells[i + 2, 4].Value = entries[i].Date;
                worksheet.Cells[i + 2, 5].Value = entries[i].Time;
                worksheet.Cells[i + 2, 6].Value = entries[i].Load;
            }

            var stream = new MemoryStream();
            package.SaveAs(stream);
            stream.Position = 0;
            var content = stream.ToArray();

            return File(content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Data.xlsx");
        }
    }

/*
   public IActionResult CsvExport(List<DataEntry> dataEntries)
    {
        {
            var csvContent = new StringBuilder();
            csvContent.AppendLine("City,Address,Date,Time"); // Add your column headers here

            foreach (var entry in dataEntries)
            {
                Console.WriteLine($"{entry.City},{entry.Address},{entry.Date},{entry.Time}");
                csvContent.AppendLine($"{entry.City},{entry.Address},{entry.Date},{entry.Time}"); // Adjust according to your DataEntry properties
            }

            var csvBytes = Encoding.UTF8.GetBytes(csvContent.ToString());
            var csvStream = new MemoryStream(csvBytes);

            return File(csvStream, "text/csv", $"Tasks_{DateTime.Now.ToString("yyyy-MM-dd_HHmmss")}.csv");
        }
    }
*/
    [HttpPost]
    public IActionResult Preview(IFormFile file)
    {
        if (file == null || file.Length == 0)
        {
            ModelState.AddModelError("File", "Please upload a valid Excel file.");
            return View("Index");
        }

        var previewData = new List<List<string>>();

        ExcelPackage.LicenseContext = LicenseContext.NonCommercial; // Set the license context

        using (var package = new ExcelPackage(file.OpenReadStream()))
        {
            var worksheet = package.Workbook.Worksheets.First();
            var rowCount = worksheet.Dimension.Rows;
            var colCount = worksheet.Dimension.Columns;

            for (int row = 1; row <= rowCount && row <= 5; row++) // Preview first 5 rows
            {
                var rowData = new List<string>();
                for (int col = 1; col <= colCount; col++)
                {
                    rowData.Add(worksheet.Cells[row, col].Text);
                }
                previewData.Add(rowData);
            }
        }

        ViewBag.PreviewData = previewData;
        return View("Index");
    }

    [HttpPost]
    public IActionResult Import(string file, string sheetName, string cityColumn, string addressColumn, string clientColumn, string dateColumn, string timeColumn, string loadColumn)
    {
        if (string.IsNullOrEmpty(file))
        {
            ModelState.AddModelError("File", "Please upload a valid Excel file.");
            return View("Index");
        }

        var entries = new List<DataEntry>();
        ExcelPackage.LicenseContext = LicenseContext.NonCommercial; // Set the license context

        using (var package = new ExcelPackage(new MemoryStream(Convert.FromBase64String(file))))
        {
            var worksheet = package.Workbook.Worksheets[sheetName];
            var rowCount = worksheet.Dimension.Rows;

            for (int row = 2; row <= rowCount; row++)
            {
                var entry = new DataEntry
                {
                    City = !string.IsNullOrEmpty(cityColumn) ? worksheet.Cells[row, GetColumnIndex(cityColumn)].Text : null,
                    Address = !string.IsNullOrEmpty(addressColumn) ? worksheet.Cells[row, GetColumnIndex(addressColumn)].Text : null,
                    Client = !string.IsNullOrEmpty(clientColumn) ? worksheet.Cells[row, GetColumnIndex(clientColumn)].Text : null,
                    Date = !string.IsNullOrEmpty(dateColumn) ? worksheet.Cells[row, GetColumnIndex(dateColumn)].Text : null,
                    Time = !string.IsNullOrEmpty(timeColumn) ? worksheet.Cells[row, GetColumnIndex(timeColumn)].Text : null,
                    Load = !string.IsNullOrEmpty(loadColumn) ? worksheet.Cells[row, GetColumnIndex(loadColumn)].Text : null
                };

                if (!string.IsNullOrEmpty(entry.City) || !string.IsNullOrEmpty(entry.Address) || !string.IsNullOrEmpty(entry.Client) || !string.IsNullOrEmpty(entry.Date) || !string.IsNullOrEmpty(entry.Time) || !string.IsNullOrEmpty(entry.Load))
                {
                    entries.Add(entry);
                }
            }
        }

        // Process the entries as needed, e.g., save to database or display in view
        return View("Index", entries);
    }

    private int GetColumnIndex(string columnName)
    {
        return columnName.ToUpper()[0] - 'A' + 1;
    }

    private List<string> GetAddressesFromCsv(string filePath)
    {
        var addresses = new List<string>();
        using (var reader = new StreamReader(filePath))
        {
            while (!reader.EndOfStream)
            {
                var line = reader.ReadLine();
                addresses.Add(line);
            }
        }
        return addresses;
    }

    private List<string> GetCitiesFromCsv(string filePath)
    {
        var cities = new List<string>();
        using (var reader = new StreamReader(filePath))
        {
            while (!reader.EndOfStream)
            {
                var line = reader.ReadLine();
                cities.Add(line);
            }
        }
        return cities;
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}