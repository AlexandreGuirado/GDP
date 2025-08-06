using GDP.Application.DTOs;
using GDP.Domain.Interfaces;
using GDP.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using GDP.Web.Extensions;
using Microsoft.Data.SqlClient;

namespace GDP.Web.Controllers;

public class OrdersController : Controller
{
    private readonly IOrderService _orderService;
    private readonly IOrderRepository _orderRepository;
    private readonly ICustomerRepository _customerRepository;
    private readonly IProductRepository _productRepository;
    private readonly ILogger<OrdersController> _logger;

    public OrdersController(IOrderService orderService, IOrderRepository orderRepository, ICustomerRepository customerRepository, IProductRepository productRepository, ILogger<OrdersController> logger)
    {
        _orderService = orderService;
        _orderRepository = orderRepository;
        _customerRepository = customerRepository;
        _productRepository = productRepository;
        _logger = logger;
    }

    public async Task<IActionResult> Create()
    {
        var viewModel = new CreateOrderViewModel
        {
            Customers = new SelectList(await _customerRepository.GetAllAsync(), "Id", "Name"),
            AvailableProducts = await _productRepository.GetAllAsync()
        };
        return View(viewModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateOrderViewModel model)
    {
        if (model.Items == null || !model.Items.Any())
        {
            ModelState.AddModelError(string.Empty, "Um pedido deve ter pelo menos um produto.");
        }

        if (ModelState.IsValid)
        {
            var dto = new CreateOrderDto
            {
                CustomerId = model.CustomerId,
                Items = model.Items.Select(i => new OrderItemDto { ProductId = i.ProductId, Quantity = i.Quantity }).ToList()
            };

            var (success, message, orderId) = await _orderService.CreateOrderAsync(dto);

            if (success)
            {
                TempData["SuccessMessage"] = "Pedido criado com sucesso!";
                return RedirectToAction("Details", new { id = orderId });
            }

            TempData["ErrorMessage"] = message;
        }

        model.Customers = new SelectList(await _customerRepository.GetAllAsync(), "Id", "Name", model.CustomerId);
        model.AvailableProducts = await _productRepository.GetAllAsync();
        return View(model);
    }

    public async Task<IActionResult> Details(int id)
    {
        var order = await _orderRepository.GetByIdWithDetailsAsync(id);
        if (order == null) return NotFound();

        if (order.Customer == null)
        {
            order.Customer = await _customerRepository.GetByIdAsync(order.CustomerId);
        }

        var viewModel = new OrderDetailViewModel
        {
            Order = order,
            NewStatus = order.Status
        };

        var statusList = Enum.GetValues(typeof(GDP.Domain.Enums.OrderStatus))
                             .Cast<GDP.Domain.Enums.OrderStatus>()
                             .Select(e => new { Value = e.ToString(), Text = e.GetDisplayName() });

        ViewBag.StatusOptions = new SelectList(statusList, "Value", "Text", order.Status);

        return View(viewModel);
    }

    public async Task<IActionResult> Index(int? customerIdFilter, string statusFilter)
    {
        var statusList = Enum.GetValues(typeof(GDP.Domain.Enums.OrderStatus))
                             .Cast<GDP.Domain.Enums.OrderStatus>()
                             .Select(e => new { Value = e.ToString(), Text = e.GetDisplayName() });

        ViewData["Customers"] = new SelectList(await _customerRepository.GetAllAsync(), "Id", "Name", customerIdFilter);
        ViewData["Statuses"] = new SelectList(statusList, "Value", "Text", statusFilter);

        var orders = await _orderRepository.SearchWithDetailsAsync(customerIdFilter, statusFilter);
        return View(orders);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateStatus(int orderId, string newStatus)
    {
        var success = await _orderRepository.UpdateStatusAsync(orderId, newStatus);
        if (success)
        {
            _logger.LogInformation("Notificação: Status do Pedido {OrderId} alterado para {Status}", orderId, newStatus);
            TempData["SuccessMessage"] = "Status do pedido atualizado com sucesso!";
        }
        else
        {
            TempData["ErrorMessage"] = "Não foi possível atualizar o status do pedido.";
        }
        return RedirectToAction("Details", new { id = orderId });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Cancel(int orderId)
    {
        var success = await _orderRepository.UpdateStatusAsync(orderId, GDP.Domain.Enums.OrderStatus.Cancelled.ToString());
        if (success)
        {
            _logger.LogInformation("Notificação: O pedido {OrderId} foi Cancelado.", orderId);
            TempData["SuccessMessage"] = "Pedido cancelado com sucesso!";
        }
        else
        {
            TempData["ErrorMessage"] = "Não foi possível cancelar o pedido.";
        }
        return RedirectToAction("Details", new { id = orderId });
    }
}