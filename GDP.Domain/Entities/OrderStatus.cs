using System.ComponentModel.DataAnnotations;

namespace GDP.Domain.Enums;

public enum OrderStatus
{
    [Display(Name = "Pendente")]
    Pending,

    [Display(Name = "Processando")]
    Processing,

    [Display(Name = "Concluído")]
    Completed,

    [Display(Name = "Cancelado")]
    Cancelled
}