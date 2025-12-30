using AutoMapper;
using ECommerce.Application.DTOs.Cart;
using ECommerce.Application.DTOs.Payment;
using ECommerce.Application.Services;
using ECommerce.Domain.Interfaces;

namespace ECommerce.Application.Services.Implementations;

public class PaymentMethodService : IPaymentMethodService
{
    private readonly IPaymentMethodRepository _paymentMethodRepository;
    private readonly IMapper _mapper;

    public PaymentMethodService(IPaymentMethodRepository paymentMethodRepository, IMapper mapper)
    {
        _paymentMethodRepository = paymentMethodRepository;
        _mapper = mapper;
    }

    public async Task<IEnumerable<GetPaymentMethodDto>> GetPaymentMethodsAsync()
    {
        var methods = await _paymentMethodRepository.GetPaymentMethodsAsync();

        if (!methods.Any())
            return Enumerable.Empty<GetPaymentMethodDto>();

        return _mapper.Map<IEnumerable<GetPaymentMethodDto>>(methods);
    }
}
