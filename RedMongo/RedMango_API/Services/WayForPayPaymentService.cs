using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace RedMango_API.Services
{
    public class WayForPayPaymentService
    {
        private readonly string _merchantAccount;
        private readonly string _merchantSecretKey;
        private readonly string _apiUrl = "https://api.wayforpay.com/api";

        public WayForPayPaymentService(IConfiguration configuration)
        {
            _merchantAccount = configuration["WayForPay:MerchantAccount"];
            _merchantSecretKey = configuration["WayForPay:SecretKey"];
        }

        public async Task<string> CreatePayment(string amount, string currency, string orderId, string description)
        {
            var requestData = new WayForPayRequest
            {
                transactionType = "CREATE_INVOICE",
                merchantAccount = _merchantAccount,
                orderReference = orderId,
                orderDate = DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
                amount = amount,
                currency = currency,
                productName = new[] { description },
                productCount = new[] { 1 },
                productPrice = new[] { amount },
                apiVersion = "1",
                merchantDomainName = "https://31ad-91-196-194-224.ngrok-free.app",
                callbackUrl =  "https://31ad-91-196-194-224.ngrok-free.app/api/payments/callback"
            };

            AddSignature(requestData);

            var client = new HttpClient();
            var options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };
            var requestContent = new StringContent(JsonSerializer.Serialize(requestData, options), Encoding.UTF8, "application/json");

            var response = await client.PostAsync(_apiUrl, requestContent);
            response.EnsureSuccessStatusCode();

            var responseContent = await response.Content.ReadAsStringAsync();
            var responseObject = JsonSerializer.Deserialize<Dictionary<string, object>>(responseContent);

            if (responseObject.TryGetValue("invoiceUrl", out var checkoutUrl))
            {
                return checkoutUrl.ToString();
            }

            throw new Exception($"Error creating payment: {responseContent}");
        }

        private void AddSignature(WayForPayRequest requestData)
        {
            var valuesToSign = new List<string>
            {
                requestData.merchantAccount,
                requestData.merchantDomainName.Trim(),
                requestData.orderReference,
                requestData.orderDate.ToString(),
                requestData.amount.ToString(), 
                requestData.currency,
                requestData.productName[0],
                requestData.productCount[0].ToString(),
                requestData.productPrice[0].ToString()
            };

            var stringToSign = string.Join(";", valuesToSign);

            using (var hmacMd5 = new HMACMD5(Encoding.UTF8.GetBytes(_merchantSecretKey)))
            {
                var hashBytes = hmacMd5.ComputeHash(Encoding.UTF8.GetBytes(stringToSign));
                requestData.merchantSignature = string.Concat(hashBytes.Select(b => b.ToString("x2")));

                Console.WriteLine($"String to Sign: {stringToSign}");
                Console.WriteLine($"Generated Signature: {requestData.merchantSignature}");
            }
        }
    }

    public class WayForPayRequest
    {
        public string transactionType { get; set; }
        public string merchantAccount { get; set; }
        public string orderReference { get; set; }
        public long orderDate { get; set; }
        public string amount { get; set; }
        public string currency { get; set; }
        public string[] productName { get; set; }
        public int[] productCount { get; set; }
        public string[] productPrice { get; set; }
        public string apiVersion { get; set; }
        public string merchantSignature { get; set; }
        public string merchantDomainName { get; set; }
        public string callbackUrl { get; set; }
    }
}
