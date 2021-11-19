using AutoMapper;
using Core.Model;
using Core.Model.Extern;

namespace Core.Helpers
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<CompanyExtern, Company>();

            CreateMap<CustomerExtern, Customer>();
            //FormMember(d => d.name, o=> .MapFrom( s => s.nombre ));

            CreateMap<OpportunityStatusExtern, OpportunityStatus>();

            CreateMap<UserExtern, User>();

            CreateMap<PaymentMethodExtern, PaymentMethod>();

            CreateMap<IncotermExtern, Incoterm>();

            CreateMap<FreightInChargeExtern, FreightInCharge>();

            CreateMap<TransportExtern, Transport>();
        }
    }
}
