
using Models.Dtos;
using Models.Entities;

namespace Servises.Interfaces;

public interface IPaymentService
{
    string Charge(User user, int totalCost);
    Task<string> CreateCustomer(string email);
}
