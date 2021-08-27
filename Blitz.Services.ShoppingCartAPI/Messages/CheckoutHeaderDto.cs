using Blitz.Services.ShoppingCartAPI.Models.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blitz.Services.ShoppingCartAPI.Messages
{
    public class CheckoutHeaderDto
    {
        public int CartHeaderId { get; set; }
        public string UserId { get; set; }
        public string CouponCode { get; set; }
        public double OrderTotal { get; set; }
        public double DiscountTotal { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime PickupDateTime { get; set; }
        public long PhoneNumber { get; set; }
        public string Email { get; set; }
        public long CardNumber { get; set; }
        public int CVV { get; set; }
        public string ExpiryMonthYear { get; set; }
        public int CartTotalItems { get; set; }
        public IEnumerable<CartDetailsDto> CartDetails { get; set; }
    }
}
