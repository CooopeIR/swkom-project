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
                    => opt.MapFrom(src => $"{src.title ?? string.Empty}"))
                .ForMember(dest => dest.Author, opt
                    => opt.MapFrom(src => $"{src.author ?? string.Empty}"))
                .ForMember(dest => dest.Contentpath, opt
                    => opt.MapFrom(src => $"{src.contentpath ?? string.Empty}"))
                .ForMember(dest => dest.FileName, opt
                    => opt.MapFrom(src => $"{src.fileName ?? string.Empty}"))
                .ReverseMap()
                .ForMember(dest => dest.id, opt
                    => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.title, opt
                    => opt.MapFrom(src => $"{src.Title ?? string.Empty}"))
                .ForMember(dest => dest.author, opt
                    => opt.MapFrom(src => $"{src.Author ?? string.Empty}"))
                .ForMember(dest => dest.contentpath, opt
                    => opt.MapFrom(src => $"{src.Contentpath ?? string.Empty}"))
                .ForMember(dest => dest.fileName, opt
                => opt.MapFrom(src => $"{src.FileName ?? string.Empty}"));
        }
    }
}
