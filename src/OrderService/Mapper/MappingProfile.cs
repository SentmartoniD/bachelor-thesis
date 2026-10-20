using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClassLibrary.WcfModels;
using ClassLibrary.Models;

namespace OrderService.Mapper
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Order, OrderDto>();
            CreateMap<Order, WcfOrder>();

            CreateMap<OrderDto, Order>();
            CreateMap<WcfOrder, Order>();
        }
    }
}
