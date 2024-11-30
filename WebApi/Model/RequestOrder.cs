using ConsoleExchange.Model;
using System.ComponentModel.DataAnnotations;

namespace WebApi.Model
{
    public class RequestOrder
    {
        [Required]
        [Range(0.0000001, 1000)]
        public decimal Amount { get; set; }
        [Required]
        [Range(0, 1)]
        public OrderType Type { get; set; }
    }
}
