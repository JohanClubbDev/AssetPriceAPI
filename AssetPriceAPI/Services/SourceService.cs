using AssetPriceAPI.Entities;
using AssetPriceAPI.Models;
using AssetPriceAPI.Repositories;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AssetPriceAPI.Services;

public class SourceService : ISourceService
{
    private readonly ISourceRepository _sourceRepository;
    private readonly IMapper _mapper;

    public SourceService(ISourceRepository sourceRepository, IMapper mapper)
    {
        _sourceRepository = sourceRepository;
        _mapper = mapper;
    }

    public async Task<SourceReadDto> GetSourceByIdAsync(Guid id)
    {
        var source = await _sourceRepository.GetSourceByIdAsync(id);
        if (source == null)
            return null;

        return _mapper.Map<SourceReadDto>(source);
    }

    public async Task<SourceReadDto> GetSourceByNameAsync(string name)
    {
        var source = await _sourceRepository.GetSourceByNameAsync(name);
        if (source == null)
            return null;

        return _mapper.Map<SourceReadDto>(source);
    }

    public async Task<IEnumerable<SourceReadDto>> GetAllSourcesAsync()
    {
        var sources = await _sourceRepository.GetAllSourcesAsync();
        return _mapper.Map<IEnumerable<SourceReadDto>>(sources);
    }

    public async Task<SourceReadDto> CreateSourceAsync(SourceCreateDto sourceDto)
    {
        var source = _mapper.Map<Source>(sourceDto);
        var createdSource = await _sourceRepository.AddSourceAsync(source);
        return _mapper.Map<SourceReadDto>(createdSource);
    }

    public async Task<SourceReadDto> UpdateSourceAsync(Guid id, SourceCreateDto sourceDto)
    {
        var existingSource = await _sourceRepository.GetSourceByIdAsync(id);
        if (existingSource == null)
            return null;

        existingSource.Name = sourceDto.Name;
        var updatedSource = await _sourceRepository.UpdateSourceAsync(existingSource);
        return _mapper.Map<SourceReadDto>(updatedSource);
    }

    public async Task<bool> DeleteSourceAsync(Guid id)
    {
        return await _sourceRepository.DeleteSourceAsync(id);
    }
}