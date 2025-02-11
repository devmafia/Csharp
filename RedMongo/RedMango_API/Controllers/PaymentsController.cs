using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RedMango_API.Models;
using RedMango_API.Services;
using System.Text;
using Microsoft.Extensions.Logging;
using System.Security.Cryptography;
using RedMango_API.Data;
using Microsoft.EntityFrameworkCore;
using RedMango_API.Models.Dto;
using RedMango_API.Utility;
using RedMango_API.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace RedMango_API.Controllers
{
    [Route("api/payments")]
    [ApiController]
    public class PaymentsController : ControllerBase
    {
        private readonly WayForPayPaymentService _paymentService;
        private readonly ILogger<PaymentsController> _logger;
        private readonly string _merchantSecretKey;
        private readonly ApplicationDbContext _db;
        private readonly IHubContext<OrderStatusHub> _hubContext;
        protected ApiResponse _response;
        public PaymentsController(IConfiguration configuration, ILogger<PaymentsController> logger, ApplicationDbContext db, IHubContext<OrderStatusHub> hubContext)
        {
            _paymentService = new WayForPayPaymentService(configuration);
            _merchantSecretKey = configuration["WayForPay:SecretKey"];
            _logger = logger;
            _db = db;
            _response = new();
            _hubContext = hubContext;
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreatePayment([FromBody] UserDataDTO model)
        {
            try
            {
                _logger.LogWarning("Try");

                var shoppingCart = _db.ShoppingCarts
                    .Include(c => c.CartItems)
                    .ThenInclude(ci => ci.MenuItem)
                    .FirstOrDefault(c => c.UserId == model.UserId);

                _logger.LogWarning("Shopping cart");
                if (shoppingCart == null || !shoppingCart.CartItems.Any())
                {
                    return BadRequest(new { Error = "Shopping cart is empty." });
                }

                shoppingCart.CartTotal = shoppingCart.CartItems.Sum(ci => ci.MenuItem.Price * ci.Quantity);
                var totalItems = shoppingCart.CartItems.Sum(ci => ci.Quantity);

                _logger.LogWarning("Sum");
                var orderHeader = new OrderHeader
                {
                    PickupName = model.Name,
                    PickupEmail = model.Email,
                    PickupPhoneNumber = model.Phone,
                    OrderTotal = shoppingCart.CartTotal,
                    TotalItems = totalItems,
                    UserId = model.UserId,
                    OrderDate = DateTime.UtcNow,
                    Status = SD.status_pending,
                    OrderID = Guid.NewGuid().ToString()
                };

                _db.OrderHeaders.Add(orderHeader);
                await _db.SaveChangesAsync();

                _logger.LogWarning("Order is saved");
                foreach (var cartItem in shoppingCart.CartItems)
                {
                    var orderDetails = new OrderDetails
                    {
                        OrderHeaderId = orderHeader.OrderHeaderId,
                        MenuItemId = cartItem.MenuItemId,
                        ItemName = cartItem.MenuItem.Name,
                        Quantity = cartItem.Quantity,
                        Price = cartItem.MenuItem.Price
                    };
                    _db.OrderDetails.Add(orderDetails);
                }

                await _db.SaveChangesAsync();

                var amountInCents = (decimal)(Math.Round(orderHeader.OrderTotal, 2));
                string amountWithDot = amountInCents.ToString("F2", System.Globalization.CultureInfo.InvariantCulture);
                var checkoutUrl = await _paymentService.CreatePayment(
                    amountWithDot,
                    "UAH",
                    orderHeader.OrderID,
                    "Order Payment"
                );

                return Ok(new
                {
                    ClientSecret = checkoutUrl,
                    OrderId = orderHeader.OrderID,
                    Message = "Order created successfully, proceed with payment."
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating payment");
                return BadRequest(new { Error = "An error occurred while creating the payment." });
            }
        }


       [HttpPost("callback")]
        public async Task<IActionResult> PaymentCallback([FromBody] Dictionary<string, object> callbackData)
        {
            try
            {
                if (!IsValidSignature(callbackData))
                {
                    _logger.LogWarning("Invalid callback signature received");
                    return BadRequest(new { Error = "Invalid signature" });
                }

                var orderReference = callbackData["orderReference"]?.ToString();
                var transactionStatus = callbackData["transactionStatus"]?.ToString();

                var order = _db.OrderHeaders.FirstOrDefault(o => o.OrderID == orderReference);
                if (order == null)
                {
                    _logger.LogError($"Order with ID {orderReference} not found");
                    return NotFound(new { Error = "Order not found" });
                }

                if (transactionStatus == "Approved")
                {
                    order.Status = SD.status_confirmed;
                }
                else
                {
                    order.Status = SD.status_Cancelled;
                }

                await _db.SaveChangesAsync();

                await _hubContext.Clients.Group(order.UserId.ToString())
                    .SendAsync("OrderStatusUpdated", new { OrderId = order.OrderHeaderId, Status = order.Status });

                return Ok(new { Message = "Payment callback processed successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing payment callback");
                return StatusCode(500, new { Error = "Internal server error" });
            }
        }


        private bool IsValidSignature(Dictionary<string, object> callbackData)
        {
            if (!callbackData.TryGetValue("merchantSignature", out var receivedSignatureObj) || receivedSignatureObj == null)
            {
                return false;
            }

            string receivedSignature = receivedSignatureObj.ToString();
            callbackData.Remove("merchantSignature");

            var sortedData = callbackData.OrderBy(kvp => kvp.Key);
            string dataString = string.Join(";", sortedData.Select(kvp => kvp.Value?.ToString() ?? string.Empty));

            string stringToSign = $"{_merchantSecretKey};{dataString}";

            using (var hmacMd5 = new HMACMD5(Encoding.UTF8.GetBytes(_merchantSecretKey)))
            {
                var hashBytes = hmacMd5.ComputeHash(Encoding.UTF8.GetBytes(stringToSign));
                string computedSignature = string.Concat(hashBytes.Select(b => b.ToString("x2")));

                _logger.LogInformation($"String to Sign: {stringToSign}");
                _logger.LogInformation($"Computed Signature: {computedSignature}");
                _logger.LogInformation($"Received Signature: {receivedSignature}");

                return receivedSignature.Equals(computedSignature, StringComparison.OrdinalIgnoreCase);
            }
        }

        private void ProcessCallbackData(Dictionary<string, object> callbackData)
        {
            var orderId = callbackData.ContainsKey("orderReference") ? callbackData["orderReference"].ToString() : "Unknown";
            var status = callbackData.ContainsKey("transactionStatus") ? callbackData["transactionStatus"].ToString() : "Unknown";

            _logger.LogInformation($"Order ID: {orderId}, Status: {status}");
            _logger.LogInformation("Callback Data: " + JsonConvert.SerializeObject(callbackData, Formatting.Indented));
        }
    }
}
