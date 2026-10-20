using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClassLibrary.WcfModels;
using ClassLibrary.Models;

namespace UserService.Mapper
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<WcfUser, UserDtoRegistration>();
            CreateMap<WcfUser, UserDtoLogin>();
            CreateMap<WcfUser, UserDtoApprovedVerified>();
            CreateMap<WcfUser, User>();

            CreateMap<UserDtoRegistration, WcfUser>();
            CreateMap<UserDtoLogin, WcfUser>();
            CreateMap<UserDtoApprovedVerified, WcfUser>();
            CreateMap<User, WcfUser>();
        }
    }
}
