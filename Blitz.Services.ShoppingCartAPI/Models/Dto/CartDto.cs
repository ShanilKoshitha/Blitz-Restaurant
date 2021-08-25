using Blitz.Services.ShoppingCartAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blitz.Services.ShoppingCartAPI.Models.Dto
{
    public class CartDto
    {
        public CartHeaderDto CartHeader { get; set; }
        public IEnumerable<CartDetailsDto> CartDetails { get; set; }
    }
}
