using System.ComponentModel.DataAnnotations;

namespace GDP.Web.Models;

public class OrderItemViewModel
{
    [Required]
    public int ProductId { get; set; }

    [Required]
    [Range(1, int.MaxValue, ErrorMessage = "Quantidade deve ser pelo menos 1.")]
    public int Quantity { get; set; }
}