using AutoMapper;
using HotelReservation.Application.DTOs;
using HotelReservation.Domain.Entities;

namespace HotelReservation.Application.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // User mappings
        CreateMap<AppUser, UserDto>();
        CreateMap<CreateUserDto, AppUser>()
            .ForMember(dest => dest.PasswordHash, opt => opt.Ignore())
            .ForMember(dest => dest.DeletedAt, opt => opt.Ignore());

        // Guest mappings
        CreateMap<Guest, GuestDto>();
        CreateMap<CreateGuestDto, Guest>()
            .ForMember(dest => dest.DeletedAt, opt => opt.Ignore());
        CreateMap<UpdateGuestDto, Guest>()
            .ForMember(dest => dest.GuestId, opt => opt.Ignore())
            .ForMember(dest => dest.UserId, opt => opt.Ignore())
            .ForMember(dest => dest.DeletedAt, opt => opt.Ignore());

        // Room mappings
        CreateMap<Room, RoomDto>()
            .ForMember(dest => dest.Features, opt => opt.MapFrom(src => src.RoomFeatureAssignments.Select(rfa => rfa.RoomFeature)));
        CreateMap<RoomType, RoomTypeDto>();
        CreateMap<RoomFeature, RoomFeatureDto>();

        // Reservation mappings
        CreateMap<Reservation, ReservationDto>();
        CreateMap<Reservation, ReservationSummaryDto>()
            .ForMember(dest => dest.RoomNumber, opt => opt.MapFrom(src => src.Room.RoomNumber))
            .ForMember(dest => dest.RoomType, opt => opt.MapFrom(src => src.Room.RoomType.TypeName))
            .ForMember(dest => dest.NumberOfDays, opt => opt.MapFrom(src => src.GetNumberOfDays()));
        CreateMap<CreateReservationDto, Reservation>()
            .ForMember(dest => dest.ReservationId, opt => opt.Ignore())
            .ForMember(dest => dest.TotalAmount, opt => opt.Ignore())
            .ForMember(dest => dest.TaxServiceCharge, opt => opt.Ignore())
            .ForMember(dest => dest.GrandTotal, opt => opt.Ignore())
            .ForMember(dest => dest.ReservationStatus, opt => opt.Ignore())
            .ForMember(dest => dest.DeletedAt, opt => opt.Ignore())
            .ForMember(dest => dest.Guest, opt => opt.Ignore())
            .ForMember(dest => dest.Room, opt => opt.Ignore());
    }
}
