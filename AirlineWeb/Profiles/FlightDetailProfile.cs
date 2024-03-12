using AutoMapper;

namespace AirlineWeb.Profiles;

public class FlightDetailProfile : Profile
{
    public FlightDetailProfile()
    {
        CreateMap<Models.FlightDetail, Dtos.FlightDetailReadDto>();
        CreateMap<Dtos.FlightDetailCreateDto, Models.FlightDetail>();
        CreateMap<Dtos.FlightDetailUpdateDto, Models.FlightDetail>();
    }
}
