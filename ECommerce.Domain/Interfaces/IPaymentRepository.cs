using System.Collections.Generic;
using System.Threading.Tasks;
using ECommerce.Domain.Entities;

namespace ECommerce.Domain.Interfaces;

public interface IPaymentRepository
{
    Task<List<PaymentMethod>> GetAllAsync();
}
