using GDP.Domain.Interfaces;
using GDP.Web.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace GDP.Web.Controllers;

public class HomeController : Controller
{
    private readonly ICustomerRepository _customerRepo;
    private readonly IProductRepository _productRepo;
    private readonly IOrderRepository _orderRepo;

    public HomeController(ICustomerRepository customerRepo, IProductRepository productRepo, IOrderRepository orderRepo)
    {
        _customerRepo = customerRepo;
        _productRepo = productRepo;
        _orderRepo = orderRepo;
    }

    public async Task<IActionResult> Index()
    {
        var viewModel = new DashboardViewModel
        {
            TotalCustomers = await _customerRepo.GetTotalCountAsync(),
            TotalProducts = await _productRepo.GetTotalCountAsync(),
            TotalOrders = await _orderRepo.GetTotalCountAsync()
        };
        return View(viewModel);
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