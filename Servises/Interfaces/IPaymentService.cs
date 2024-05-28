
using Microsoft.AspNetCore.Http;
using Models.Dtos;
using Models.Entities;
using Stripe;

namespace Servises.Interfaces;

public interface IPaymentService
{
    Event EventFromJson(string json, HttpRequest request);
    Task<string> Charge(Guid userId, string userEmail, BuyBookDto book);
}
