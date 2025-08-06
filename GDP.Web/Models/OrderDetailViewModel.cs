using GDP.Domain.Entities;

namespace GDP.Web.Models;

public class OrderDetailViewModel
{
    public Order Order { get; set; }
    public string NewStatus { get; set; }
}