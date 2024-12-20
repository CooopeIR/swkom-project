using AutoMapper;
using DocumentDAL.Entities;
using SWKOM.DTO;

namespace SWKOM.Mappings
{
    /// <summary>
    /// Mapping Profiles for datastructures between SWKOM and DocumentDAL
    /// </summary>
    public class MappingProfile : Profile
    {
        /// <summary>
        /// Mapping Profile for variables from and to DocumentItem and DocumentItemDTO, DocumentContent and DocumentContentDTO, DocumentMetadata and DocumentMetadataDTO
        /// </summary>
        public MappingProfile()
        {
            // Mapping for DocumentItem and DocumentItemDTO
            CreateMap<DocumentItem, DocumentItemDTO>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Title))
                .ForMember(dest => dest.Author, opt => opt.MapFrom(src => src.Author))
                .ForMember(dest => dest.DocumentContentDto, opt => opt.MapFrom(src => src.DocumentContent))
                .ForMember(dest => dest.DocumentMetadataDto, opt => opt.MapFrom(src => src.DocumentMetadata))
                .ForMember(dest => dest.OcrText, opt => opt.MapFrom(src => src.OcrText))
                .ReverseMap()
                .ForMember(dest => dest.Id, opt => opt.Ignore()); // Ignore Id for reverse mapping if needed

            // Mapping for DocumentContent and DocumentContentDTO
            CreateMap<DocumentContent, DocumentContentDTO>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.DocumentId, opt => opt.MapFrom(src => src.DocumentId))
                .ForMember(dest => dest.FileName, opt => opt.MapFrom(src => src.FileName))
                .ForMember(dest => dest.ContentType, opt => opt.MapFrom(src => src.ContentType))
                .ForMember(dest => dest.Content, opt => opt.MapFrom(src => src.Content))
                .ReverseMap();

            // Mapping for DocumentMetadata and DocumentMetadataDTO
            CreateMap<DocumentMetadata, DocumentMetadataDTO>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.DocumentId, opt => opt.MapFrom(src => src.DocumentId))
                .ForMember(dest => dest.UploadDate, opt => opt.MapFrom(src => src.UploadDate))
                .ForMember(dest => dest.FileSize, opt => opt.MapFrom(src => src.FileSize))
                .ReverseMap();

            // Specific Mapping for Search, making sure the Content and Metadata objects are not needed on Mapping while Searching
            CreateMap<Document, DocumentItemDTO>()
                .ForMember(dest => dest.UploadedFile, opt => opt.Ignore())
                .ForMember(dest => dest.DocumentContentDto, opt => opt.Ignore())
                .ForMember(dest => dest.DocumentMetadataDto, opt => opt.Ignore());
        }
    }
}
