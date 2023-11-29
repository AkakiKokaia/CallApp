using AutoMapper;
using CallApp.Application.DTOs.Account;
using CallApp.Application.DTOs.User;
using CallApp.Application.Features.Account.Commands;
using CallApp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace CallApp.Application.Mappings
{
    public class GeneralProfile : Profile
    {
        public GeneralProfile() 
        {
            CreateMap<RegisterAsyncCommand, UserEntity>().ReverseMap();
            CreateMap<UpdateUserAsyncCommand, UserEntity>().ReverseMap();
            CreateMap<RegisterAsyncCommand, UserProfileEntity>().ReverseMap();
            CreateMap<UserEntity, GetUserResponse>().ReverseMap();
            CreateMap<UserEntity, GetUserResponse>()
                .ForMember(dest => dest.FirstName, opt => opt.MapFrom(src => src.UserProfiles.FirstOrDefault().FirstName))
                .ForMember(dest => dest.LastName, opt => opt.MapFrom(src => src.UserProfiles.FirstOrDefault().LastName))
                .ForMember(dest => dest.IDNumber, opt => opt.MapFrom(src => src.UserProfiles.FirstOrDefault().IDNumber))
                .ReverseMap();
            CreateMap<UserProfileEntity, GetUserResponse>()
                .ForMember(x => x.FirstName, y => y.MapFrom(z => z.FirstName))
                .ForMember(x => x.LastName, y => y.MapFrom(z => z.LastName))
                .ForMember(x => x.IDNumber, y => y.MapFrom(z => z.IDNumber))
                .ReverseMap();

            CreateMap<LoginResponse, UserResponse>().ReverseMap();
            CreateMap<LoginResponse, UserEntity>().ReverseMap();
            CreateMap<UserEntity, UserResponse>();

            CreateMap<UpdateUserAsyncCommand, UserProfileEntity>().ReverseMap();
        }
    }
}
