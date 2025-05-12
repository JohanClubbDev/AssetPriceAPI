using AssetPriceAPI.Entities;
using AssetPriceAPI.Models;
using AutoMapper;

namespace AssetPriceAPI.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // Asset mappings
        CreateMap<Asset, AssetReadDto>();
        CreateMap<AssetCreateDto, Asset>();
    }
}