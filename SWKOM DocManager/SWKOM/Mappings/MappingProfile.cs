using AutoMapper;
using DocumentDAL.Entities;
using SWKOM.DTO;

namespace SWKOM.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Mapping for DocumentItem and DocumentItemDTO
            CreateMap<DocumentItem, DocumentItemDTO>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Title))
                .ForMember(dest => dest.Author, opt => opt.MapFrom(src => src.Author))
                .ForMember(dest => dest.DocumentContentDto, opt => opt.MapFrom(src => src.DocumentContent))
                .ForMember(dest => dest.DocumentMetadataDto, opt => opt.MapFrom(src => src.DocumentMetadata))
                .ReverseMap()
                .ForMember(dest => dest.DocumentContent, opt => opt.MapFrom(src => src.DocumentContentDto))
                .ForMember(dest => dest.DocumentMetadata, opt => opt.MapFrom(src => src.DocumentMetadataDto))
                .ForMember(dest => dest.Id, opt => opt.Ignore()) // Ignore Id for reverse mapping if needed
                .ForMember(dest => dest.DocumentContent, opt => opt.Ignore()) // Ignore circular reference
                .ForMember(dest => dest.DocumentMetadata, opt => opt.Ignore());

            // Mapping for DocumentContent and DocumentContentDTO
            CreateMap<DocumentContent, DocumentContentDTO>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.DocumentId, opt => opt.MapFrom(src => src.DocumentId))
                .ForMember(dest => dest.FileName, opt => opt.MapFrom(src => src.FileName))
                .ForMember(dest => dest.ContentType, opt => opt.MapFrom(src => src.ContentType))
                .ForMember(dest => dest.Content, opt => opt.MapFrom(src => src.Content))
                .ReverseMap()
                .ForMember(dest => dest.DocumentItem, opt => opt.Ignore()); // Ignore circular reference

            // Mapping for DocumentMetadata and DocumentMetadataDTO
            CreateMap<DocumentMetadata, DocumentMetadataDTO>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.DocumentId, opt => opt.MapFrom(src => src.DocumentId))
                .ForMember(dest => dest.UploadDate, opt => opt.MapFrom(src => src.UploadDate))
                .ForMember(dest => dest.FileSize, opt => opt.MapFrom(src => src.FileSize))
                .ReverseMap()
                .ForMember(dest => dest.DocumentItem, opt => opt.Ignore()); // Ignore circular reference

            //public MappingProfile()
            //{
            //    CreateMap<DocumentItem, DocumentItemDTO>()
            //        .ForMember(dest => dest.Id, opt
            //            => opt.MapFrom(src => src.Id))
            //        .ForMember(dest => dest.Title, opt
            //            => opt.MapFrom(src => $"{src.Title ?? string.Empty}"))
            //        .ForMember(dest => dest.Author, opt
            //            => opt.MapFrom(src => $"{src.Author ?? string.Empty}"))

            //        //.ForMember(dest => dest.Contentpath, opt
            //        //    => opt.MapFrom(src => $"{src.contentpath ?? string.Empty}"))
            //        //.ForMember(dest => dest.FileName, opt
            //        //    => opt.MapFrom(src => $"{src.fileName ?? string.Empty}"))
            //        .ReverseMap()
            //        .ForMember(dest => dest.Id, opt
            //            => opt.MapFrom(src => src.Id))
            //        .ForMember(dest => dest.Title, opt
            //            => opt.MapFrom(src => $"{src.Title ?? string.Empty}"))
            //        .ForMember(dest => dest.Author, opt
            //            => opt.MapFrom(src => $"{src.Author ?? string.Empty}"));
            //    //.ForMember(dest => dest.contentpath, opt
            //    //    => opt.MapFrom(src => $"{src.Contentpath ?? string.Empty}"))
            //    //.ForMember(dest => dest.fileName, opt
            //    //=> opt.MapFrom(src => $"{src.FileName ?? string.Empty}"));
            //}
        }
    }
}
