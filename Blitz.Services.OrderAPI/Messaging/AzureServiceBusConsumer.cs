using Azure.Messaging.ServiceBus;
using Blitz.MessageBus;
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
    public class AzureServiceBusConsumer : IAzureServiceBusConsumer
    {
        private readonly string serviceBusConnectionString;
        private readonly string checkOutMessageTopic;
        private readonly string subscriptionCheckOut;
        private readonly string orderPaymentProcessTopic;
        private readonly string orderUpdatePaymentResultTopic;


        private ServiceBusProcessor checkOutProcessor;
        private ServiceBusProcessor orderUpdatePaymentProcessor;
        private readonly IMessageBus _messageBus;

        private readonly IConfiguration _configuration;
        private readonly OrderRepository _orderRepository;

        public AzureServiceBusConsumer(OrderRepository orderRepository, IConfiguration configuration, IMessageBus messageBus)
        {
            _orderRepository = orderRepository;
            _configuration = configuration;
            _messageBus = messageBus;
            serviceBusConnectionString = _configuration.GetValue<string>("ServiceBusConnectionString");
            checkOutMessageTopic = _configuration.GetValue<string>("CheckoutMessageTopic");
            subscriptionCheckOut = _configuration.GetValue<string>("SubscriptionCheckOut");
            orderPaymentProcessTopic = _configuration.GetValue<string>("OrderPaymentProcessTopic");
            orderUpdatePaymentResultTopic = _configuration.GetValue<string>("OrderUpdatePaymentResultTopic");


            var client = new ServiceBusClient(serviceBusConnectionString);
            //we need to instantiate the checkout Processor
            checkOutProcessor = client.CreateProcessor(checkOutMessageTopic);
            orderUpdatePaymentProcessor = client.CreateProcessor(orderUpdatePaymentResultTopic, subscriptionCheckOut);
        }

        public async Task Start()
        {
            
            checkOutProcessor.ProcessMessageAsync += OnCheckOutMessageReceived;
            checkOutProcessor.ProcessErrorAsync += ErrorHandler;
            await checkOutProcessor.StartProcessingAsync();

            orderUpdatePaymentProcessor.ProcessMessageAsync += OnOrderPaymentUpdateReceived;
            orderUpdatePaymentProcessor.ProcessErrorAsync += ErrorHandler;
            await orderUpdatePaymentProcessor.StartProcessingAsync();
        }
        public async Task Stop()
        {
            await checkOutProcessor.StopProcessingAsync();
            await checkOutProcessor.DisposeAsync();

            await orderUpdatePaymentProcessor.StopProcessingAsync();
            await orderUpdatePaymentProcessor.DisposeAsync();
        }

        Task ErrorHandler(ProcessErrorEventArgs args)
        {
            Console.WriteLine(args.Exception.ToString());
            return Task.CompletedTask;
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

            PaymentRequestMessage paymentRequestMessage = new()
            {
                Name            = orderHeader.FirstName + " " + orderHeader.LastName,
                CardNumber      = orderHeader.CardNumber,
                CVV             = orderHeader.CVV,
                ExpiryMonthYear = orderHeader.ExpiryMonthYear,
                OrderId         = orderHeader.OrderHeaderId,
                OrderTotal      = orderHeader.OrderTotal,
                Email           = orderHeader.Email
            };
            try
            {
                //Create a topic called orderpaymentprocesstopic make the max delivery count to be 3
                await _messageBus.PublishMessage(paymentRequestMessage, orderPaymentProcessTopic);
                await args.CompleteMessageAsync(args.Message);
            }catch(Exception e)
            {
                var errorMessages = new List<string>() { e.ToString() };
                Console.WriteLine(errorMessages);
            }
        }

        private async Task OnOrderPaymentUpdateReceived(ProcessMessageEventArgs args)
        {
            var message = args.Message;
            var body = Encoding.UTF8.GetString(message.Body);

            UpdatePaymentResultMessage paymentResultMessage = JsonConvert.DeserializeObject<UpdatePaymentResultMessage>(body);

            await _orderRepository.UpdateOrderPaymentStatus(paymentResultMessage.OrderId, paymentResultMessage.status);
            await args.CompleteMessageAsync(args.Message);
        }
    }
}
