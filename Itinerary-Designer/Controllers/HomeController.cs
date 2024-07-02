using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Itinerary_Designer.Models;
using System.Linq.Expressions;

namespace Itinerary_Designer.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly ExchangeRatesApiService _exchangeRatesApi;

    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
        _exchangeRatesApi = new ExchangeRatesApiService(); //initalizing
    }

    public IActionResult Index()
    {
        return View();
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
    //Action for the Currency Conversion
    [HttpPost]
    public async Task<IActionResult> CovertCurrency(string fromCurrency, string toCurrency, double amount)
    {
        try
        {
            ConvertRequest request = new ConvertRequest(fromCurrency, toCurrency, amount);
            ConveryResponse response = await _exchangeRatesApi.ConvertAsync(request);

            //Preparing data to pass to the View
            ViewData["Amount"] = amount;
            ViewData["FromCurrency"] = fromCurrency;
            ViewData["ToCurrency"] = toCurrency;
            ViewData["ConvertedAmount"] = response.Result;

            return View("Index");
        {
            catch (Exception ex)
            {
                ViewData["Error"] = ex.Message;
                return View("Index")
            }
        }
        }
    }
}
