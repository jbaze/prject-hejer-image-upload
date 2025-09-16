using AutoMapper;
using CustomerImageApi.Application.Interfaces;
using CustomerImageApi.Domain.Entities;
using CustomerImageApi.Domain.Interfaces;
using CustomerImageApi.DTOs.Requests;
using CustomerImageApi.DTOs.Responses;

namespace CustomerImageApi.Application.Services;

public class CustomerService : ICustomerService
{
    private readonly ICustomerRepository _customerRepository;
    private readonly IMapper _mapper;

    public CustomerService(ICustomerRepository customerRepository, IMapper mapper)
    {
        _customerRepository = customerRepository;
        _mapper = mapper;
    }

    public async Task<CustomerResponse?> GetByIdAsync(int id)
    {
        var customer = await _customerRepository.GetByIdWithImagesAsync(id);
        return customer == null ? null : _mapper.Map<CustomerResponse>(customer);
    }

    public async Task<IEnumerable<CustomerResponse>> GetAllAsync()
    {
        var customers = await _customerRepository.GetAllAsync();
        return _mapper.Map<IEnumerable<CustomerResponse>>(customers);
    }

    public async Task<CustomerResponse> CreateAsync(CreateCustomerRequest request)
    {
        var customer = _mapper.Map<Customer>(request);
        await _customerRepository.CreateAsync(customer);
        return _mapper.Map<CustomerResponse>(customer);
    }

    public async Task<CustomerResponse?> UpdateAsync(int id, CreateCustomerRequest request)
    {
        var customer = await _customerRepository.GetByIdAsync(id);
        if (customer == null)
            return null;

        _mapper.Map(request, customer);

        await _customerRepository.UpdateAsync(customer);
        return _mapper.Map<CustomerResponse>(customer);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        if (!await _customerRepository.ExistsAsync(id))
            return false;

        await _customerRepository.DeleteAsync(id);
        return true;
    }
}