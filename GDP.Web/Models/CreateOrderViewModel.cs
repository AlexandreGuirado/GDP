using GDP.Domain.Entities;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace GDP.Web.Models;

public class CreateOrderViewModel
{
    [Display(Name = "Customer")]
    [Required(ErrorMessage = "Por favor selecione um cliente.")]
    public int CustomerId { get; set; }

    public List<OrderItemViewModel> Items { get; set; } = new();

    public SelectList? Customers { get; set; }
    public IEnumerable<Product>? AvailableProducts { get; set; }
}