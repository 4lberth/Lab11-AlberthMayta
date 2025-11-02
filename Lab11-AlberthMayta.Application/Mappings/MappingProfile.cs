using AutoMapper;
using Lab10_AlberthMayta.Application.DTOs;
using Lab10_AlberthMayta.Infrastructure;

namespace Lab10_AlberthMayta.Application.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Mapeo de Entidad (Domain) -> DTO (Application)
            CreateMap<User, UserDto>();
            CreateMap<Ticket, TicketSummaryDto>()
                .ForMember(dest => dest.CreatorUsername, opt => opt.MapFrom(src => src.User.Username));
            
            CreateMap<Response, ResponseDto>()
                .ForMember(dest => dest.ResponderUsername, opt => opt.MapFrom(src => "Usuario")); // Simplificado

            CreateMap<Ticket, TicketDetailDto>()
                .ForMember(dest => dest.CreatorUsername, opt => opt.MapFrom(src => src.User.Username));
        }
    }
}