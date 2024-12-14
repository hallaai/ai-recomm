using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using ai_recomm.Models;

namespace ai_recomm.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly CollaborativeFilteringService _cfService;
    private readonly LLMService _llmService;

    public HomeController(ILogger<HomeController> logger, CollaborativeFilteringService cfService, LLMService llmService)
    {
        _logger = logger;
        _cfService = cfService;
        _llmService = llmService;
    }

    public IActionResult Index()
    {
        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    private User GetUser()
    {
        // Implement logic to get the current user
        return new User(); // Replace with actual user retrieval logic
    }

    [HttpPost]
    public IActionResult Recommend(List<int> selectedProducts, string userComment)
    {
        var user = GetUser(); // Get the current user
        var insights = _llmService.GetInsights(userComment);
        var recommendations = _cfService.GetRecommendations(user);

        // Combine insights with recommendations
        // Return the final list of recommended products

        return View("Recommendations", recommendations);
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
