using System.Collections.Generic;
using System.Threading.Tasks;
using ECommerce.Domain.Entities;

namespace ECommerce.Domain.Interfaces;

public interface ICartRepository
{
    Task<bool> SaveCheckoutHistoryAsync(IEnumerable<CheckoutArchive> archives);
    Task<List<CheckoutArchive>> GetCheckoutHistoryAsync();
}
