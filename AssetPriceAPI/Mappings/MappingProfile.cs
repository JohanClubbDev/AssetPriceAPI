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
        CreateMap<Price, PriceReadDto>()
            .ForMember(dest => dest.AssetName, opt => opt.MapFrom(src => src.Asset.Name))
            .ForMember(dest => dest.SourceName, opt => opt.MapFrom(src => src.Source.Name));

        CreateMap<PriceCreateDto, Price>();
        CreateMap<SourceCreateDto, Source>();
        CreateMap<Source, SourceReadDto>();
    }
}