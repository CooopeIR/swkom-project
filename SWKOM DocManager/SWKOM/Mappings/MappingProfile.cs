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
                    => opt.MapFrom(src => src.id))
                .ForMember(dest => dest.Title, opt
                    => opt.MapFrom(src => $"*{src.title ?? string.Empty}*"))
                .ForMember(dest => dest.Author, opt
                    => opt.MapFrom(src => $"*{src.author ?? string.Empty}*"))
                .ReverseMap()
                .ForMember(dest => dest.id, opt
                    => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.title, opt
                    => opt.MapFrom(src => $"*{src.Title ?? string.Empty}*"))
                .ForMember(dest => dest.author, opt
                    => opt.MapFrom(src => $"*{src.Author ?? string.Empty}*"));
        }
    }
}
