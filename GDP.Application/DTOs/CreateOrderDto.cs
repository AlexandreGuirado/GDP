namespace GDP.Application.DTOs;
public class CreateOrderDto
{
    public int CustomerId { get; set; }
    public List<OrderItemDto> Items { get; set; } = new();
}