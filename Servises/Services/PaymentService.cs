using Microsoft.Extensions.Options;
using Models.Exceptions;
using Models.Entities;
using Servises.Interfaces;
using Models.Dtos;
using Stripe;
using Mapster;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Servises.Services;

public class PaymentService : IPaymentService
{
    private readonly IOptions<StripeSettings> _stripeSettings;
    public PaymentService(IOptions<StripeSettings> stripeSettings)
    {
        _stripeSettings = stripeSettings;
        StripeConfiguration.ApiKey = _stripeSettings.Value.SecretKey;
        
    }
    public async Task<string> CreateCustomer(string email)
    {
        var options = new CustomerCreateOptions
        {
            Email = email
        };
        var service = new CustomerService();
        Customer customer = await service.CreateAsync(options);
        return customer.Id;
    }
    public string Charge(User user, int totalCost)
    {
        var charges = new ChargeService();


        var charge = charges.Create(new ChargeCreateOptions
        {
            Amount = totalCost,
            Description = "Buying an e-book",
            Currency = "usd",
            Customer = user.BankAccount
        });

        if (charge.Status != "succeeded")
            throw new BadRequestException("Payment error.");
        string BalanceTransactionId = charge.BalanceTransactionId;
        return BalanceTransactionId;
    }
}