using Azure.Messaging.ServiceBus;
using Blitz.Services.OrderAPI.Messages;
using Blitz.Services.OrderAPI.Models;
using Blitz.Services.OrderAPI.Repository;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blitz.Services.OrderAPI.Messaging
{
    public class AzureServiceBusConsumer
    {
        private readonly string serviceBusConnectionString;
        private readonly string checkoutMessageTopic;
        private readonly string subscriptionCheckOut;

        private ServiceBusProcessor checkoutProcessor;

        private readonly IConfiguration _configuration;
        private readonly OrderRepository _orderRepository;

        public AzureServiceBusConsumer(OrderRepository orderRepository, IConfiguration configuration)
        {
            _orderRepository = orderRepository;
            _configuration = configuration;

            serviceBusConnectionString = _configuration.GetValue<string>("ServiceBusConnectionString");
            checkoutMessageTopic = _configuration.GetValue<string>("CheckoutMessageTopic");
            subscriptionCheckOut = _configuration.GetValue<string>("SubscriptionCheckOut");

            var client = new ServiceBusClient(serviceBusConnectionString);
            //we need to instantiate the checkout Processor
            checkoutProcessor = client.CreateProcessor(checkoutMessageTopic, subscriptionCheckOut);
        }
        private async Task OnCheckOutMessageReceived(ProcessMessageEventArgs args)
        {
            var message = args.Message;
            var body = Encoding.UTF8.GetString(message.Body);

            CheckoutHeaderDto checkoutHeaderDto = JsonConvert.DeserializeObject<CheckoutHeaderDto>(body);

            OrderHeader orderHeader = new()
            {
                UserId          = checkoutHeaderDto.UserId,
                FirstName       = checkoutHeaderDto.FirstName,
                LastName        = checkoutHeaderDto.LastName,
                OrderDetails    = new List<OrderDetails>(),
                CardNumber      = checkoutHeaderDto.CardNumber,
                CouponCode      = checkoutHeaderDto.CouponCode,
                CVV             = checkoutHeaderDto.CVV,
                DiscountTotal   = checkoutHeaderDto.DiscountTotal,
                Email           = checkoutHeaderDto.Email,
                ExpiryMonthYear = checkoutHeaderDto.ExpiryMonthYear,
                OrderTime       = DateTime.Now,
                OrderTotal      = checkoutHeaderDto.OrderTotal,
                PaymentStatus   = false,
                PhoneNumber     = checkoutHeaderDto.PhoneNumber,
                PickupDateTime  = checkoutHeaderDto.PickupDateTime
            };

            foreach(var detailList in checkoutHeaderDto.CartDetails)
            {
                OrderDetails orderDetails = new()
                {
                    ProductId   = detailList.ProductId,
                    ProductName = detailList.Product.Name,
                    Price       = detailList.Product.Price,
                    Count       = detailList.Count
                };
                orderHeader.CartTotalItems += detailList.Count;
                orderHeader.OrderDetails.Add(orderDetails);
            }
            await _orderRepository.AddOrder(orderHeader);
        }
    }
}
