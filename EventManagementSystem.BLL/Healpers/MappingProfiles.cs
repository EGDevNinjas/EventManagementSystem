using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using EventManagementSystem.Core.DTOs;
using EventManagementSystem.Core.Entities;


namespace EventManagementSystem.BLL.Healpers
{
    public class MappingProfiles:Profile 
    {
        public MappingProfiles() 
        {
            CreateMap<Booking, BookingDTO>().ReverseMap();
        }
    }
}
