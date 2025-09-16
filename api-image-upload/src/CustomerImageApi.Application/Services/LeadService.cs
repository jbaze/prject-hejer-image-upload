using AutoMapper;
using CustomerImageApi.Application.Interfaces;
using CustomerImageApi.Domain.Entities;
using CustomerImageApi.Domain.Interfaces;
using CustomerImageApi.DTOs.Requests;
using CustomerImageApi.DTOs.Responses;

namespace CustomerImageApi.Application.Services;

public class LeadService : ILeadService
{
    private readonly ILeadRepository _leadRepository;
    private readonly IMapper _mapper;

    public LeadService(ILeadRepository leadRepository, IMapper mapper)
    {
        _leadRepository = leadRepository;
        _mapper = mapper;
    }

    public async Task<LeadResponse?> GetByIdAsync(int id)
    {
        var lead = await _leadRepository.GetByIdWithImagesAsync(id);
        return lead == null ? null : _mapper.Map<LeadResponse>(lead);
    }

    public async Task<IEnumerable<LeadResponse>> GetAllAsync()
    {
        var leads = await _leadRepository.GetAllAsync();
        return _mapper.Map<IEnumerable<LeadResponse>>(leads);
    }

    public async Task<LeadResponse> CreateAsync(CreateLeadRequest request)
    {
        var lead = _mapper.Map<Lead>(request);
        await _leadRepository.CreateAsync(lead);
        return _mapper.Map<LeadResponse>(lead);
    }

    public async Task<LeadResponse?> UpdateAsync(int id, CreateLeadRequest request)
    {
        var lead = await _leadRepository.GetByIdAsync(id);
        if (lead == null)
            return null;

        _mapper.Map(request, lead);

        await _leadRepository.UpdateAsync(lead);
        return _mapper.Map<LeadResponse>(lead);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        if (!await _leadRepository.ExistsAsync(id))
            return false;

        await _leadRepository.DeleteAsync(id);
        return true;
    }
}