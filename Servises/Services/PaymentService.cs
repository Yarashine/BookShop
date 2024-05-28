using Microsoft.Extensions.Options;
using Models.Exceptions;
using Models.Entities;
using Servises.Interfaces;
using Models.Dtos;
using Stripe;
using Mapster;
using static System.Runtime.InteropServices.JavaScript.JSType;
using Stripe.Checkout;
using System.Security.Policy;
using System.Net;
using Models.Configs;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Http;

namespace Servises.Services;

public class PaymentService : IPaymentService
{
    private readonly StripeSessionConfig _sessionConfig;
    private readonly IConfiguration _config;
    public PaymentService(IConfiguration config, IOptions<StripeSessionConfig> sessionConfig)
    {
        StripeConfiguration.ApiKey = config["StripeOptions:PrivateKey"];
        _config = config;
        _sessionConfig = sessionConfig.Value;
    }
        
    /*public async Task<string> CreateCustomer(string email)
    {
        var options = new CustomerCreateOptions
        {
            Email = email
        };
        var service = new CustomerService();
        Customer customer = await service.CreateAsync(options);
        return customer.Id;
    }*/
    public async Task<string> Charge(Guid userId, string userEmail, BuyBookDto book)
    {

        var options = new SessionCreateOptions
        {
            PaymentMethodTypes = [_sessionConfig.PaymentMethod,],
            LineItems =
            [
                new() {
                    PriceData = new SessionLineItemPriceDataOptions
                    {
                        UnitAmount = (int)(book.Price * 100), // цена в копейках
                        Currency = "usd",
                        ProductData = new SessionLineItemPriceDataProductDataOptions
                        {
                            Name = book.Title,
                        },
                    },
                    Quantity = 1,
                },
            ],
            Metadata = new Dictionary<string, string>
            {
                { "bookId", book.Id.ToString() },
                { "userId", userId.ToString() }
            },
            Mode = "payment",
            CustomerEmail = userEmail,
            SuccessUrl = _sessionConfig.SuccessUrl,
            CancelUrl = _sessionConfig.CancelUrl,
        };
        var service = new SessionService();
        var session = await service.CreateAsync(options);
        return session.Url;
    }

    public Event EventFromJson(string json, HttpRequest request)
    {
        return EventUtility.ConstructEvent(json,
            request.Headers["Stripe-Signature"], 
            _config["StripeOptions:Whkey"]) ?? throw new BadRequestException("Desiarylization error");
    }
}