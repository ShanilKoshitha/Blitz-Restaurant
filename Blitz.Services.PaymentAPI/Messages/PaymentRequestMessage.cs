using Blitz.MessageBus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blitz.Services.PaymentAPI.Messages
{
    public class PaymentRequestMessage :BaseMessage
    {
        public int OrderId { get; set; }
        public string Name { get; set; }
        public long CardNumber { get; set; }
        public int CVV { get; set; }
        public string ExpiryMonthYear { get; set; }
        public double OrderTotal { get; set; }
        public string Email { get; set; }
    }
}
