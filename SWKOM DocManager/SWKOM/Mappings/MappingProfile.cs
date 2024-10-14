using AutoMapper;
using DocumentDAL.Entities;
using SWKOM.Models;

namespace SWKOM.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<DocumentItem, DocumentInformation>()
                .ForMember(dest => dest.Id, opt
                    => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Title, opt
                    => opt.MapFrom(src => $"*{src.Title ?? string.Empty}*"))
                .ForMember(dest => dest.Author, opt
                    => opt.MapFrom(src => $"*{src.Author ?? string.Empty}*"))
                .ReverseMap()
                .ForMember(dest => dest.Id, opt
                    => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Title, opt
                    => opt.MapFrom(src => $"*{src.Title ?? string.Empty}*"))
                .ForMember(dest => dest.Author, opt
                    => opt.MapFrom(src => $"*{src.Author ?? string.Empty}*"));


        }
    }
}
