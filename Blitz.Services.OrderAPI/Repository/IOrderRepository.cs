﻿using Blitz.Services.OrderAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blitz.Services.OrderAPI.Repository
{
    public interface IOrderRepository
    {
        Task<bool> AddOrder(OrderHeader orderHeader);
        Task UpdateOrderPaymentStatus(int orderHeaderId, bool paid);
    }
}
