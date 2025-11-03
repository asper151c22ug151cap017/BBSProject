using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using BusBookingSystem.Application.BookingDtos;
using BusBookingSystem.Application.User;
using BusBookingSystem.Application.UserDto;
using BusBookingSystem.Domain.Models;

namespace BusBookingSystem.Domain.AutoMapper
{
    public class MappingProfile: Profile
    {
       public MappingProfile() 
        {
            ///DownloadBooking Tickets 
            CreateMap<Tblbooking, ResponseDownloadtickets>().ReverseMap();
            CreateMap<Tbluser, downloadUserinfo>().ReverseMap();    
            CreateMap<Tblbuse, Downloadbusdto>().ReverseMap();
            CreateMap<Tblroute, DownloadRoutesDto>().ReverseMap();
            CreateMap<Tblpassenger, Passangerinfo>().ReverseMap();

            ////GetallBookings 
            CreateMap<Tblbooking, Responsegetbooking>().ReverseMap();
            CreateMap<Tblbuse, BusesDto>().ReverseMap();
            CreateMap<Tbluser, UserinfoDto>().ReverseMap();
            CreateMap<Tblroute, RoutesDto>().ReverseMap();
            CreateMap<Tblpassenger, Responsepassangeinfo>().ReverseMap();


            ///GetRecent Bookings 

            CreateMap<Tblbooking, ResponseGetrecentbookings>()
             .ForMember(ks => ks.BusName, s => s.MapFrom(x => x.Bus.BusName))
                .ForMember(ks => ks.Busnumber, s => s.MapFrom(x => x.Bus.Busnumber))

                .ForMember(ks => ks.Source, s => s.MapFrom(x => x.Route.Source))
                .ForMember(ks => ks.Destination, s => s.MapFrom(x => x.Route.Destination))
                .ForMember(ks => ks.SeatNumbers, s => s.MapFrom(x => x.Tblbookingseats.Select(sk => sk.Seat.SeatNumber).ToList()))
                .ForMember(ks=> ks.SeatIds, s=> s.MapFrom(x=> x.Tblbookingseats.Select(sk => sk.SeatId).ToList()))
                .ForMember(ks => ks.Name, s => s.MapFrom(x => x.User.Name))
                .ForMember(ks => ks.Phone, s => s.MapFrom(x => x.User.Phone))
                .ForMember(ks => ks.passangers, s => s.MapFrom(x => x.Tblpassengers));
             
            CreateMap<Tblpassenger, Passangerinfo>().ReverseMap();

            ///User Table 
       
  
            CreateMap<Tbluser, ResponseuserbyidDto>().ReverseMap();
            CreateMap<Tbluser, RequestUpdateUser>().ReverseMap();
            CreateMap<Tbluser, RequestUserUpdateDto>().ReverseMap();


        } 
    }
}
