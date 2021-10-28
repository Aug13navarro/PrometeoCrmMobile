using AutoMapper;
using Core.Model;
using Core.Model.Extern;
using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Helpers
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<CustomerExtern, Customer>();
            //FormMember(d => d.name, o=> .MapFrom( s => s.nombre ));
        }
    }
}
