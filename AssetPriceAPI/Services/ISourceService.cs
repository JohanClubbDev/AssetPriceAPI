using AssetPriceAPI.Entities;
using AssetPriceAPI.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AssetPriceAPI.Services
{
    public interface ISourceService
    {
        Task<SourceReadDto> GetSourceByIdAsync(Guid id);
        Task<SourceReadDto> GetSourceByNameAsync(string name);
        Task<IEnumerable<SourceReadDto>> GetAllSourcesAsync();
        Task<SourceReadDto> CreateSourceAsync(SourceCreateDto sourceDto);
        Task<SourceReadDto> UpdateSourceAsync(Guid id, SourceCreateDto sourceDto);
        Task<bool> DeleteSourceAsync(Guid id);
    }
}