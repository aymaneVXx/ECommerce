using AutoMapper;
using ECommerce.Application.DTOs.Payment;
using ECommerce.Domain.Interfaces;

namespace ECommerce.Application.Services.Implementations;

public class PaymentService : IPaymentService
{
    private readonly IPaymentRepository _repo;
    private readonly IMapper _mapper;

    public PaymentService(IPaymentRepository repo, IMapper mapper)
    {
        _repo = repo;
        _mapper = mapper;
    }

    public async Task<List<GetPaymentMethodDto>> GetAllAsync()
    {
        var methods = await _repo.GetAllAsync();
        return _mapper.Map<List<GetPaymentMethodDto>>(methods);
    }
}
