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

            CreateMap<PaymentConditionsExtern, PaymentCondition>();

            CreateMap<ProductExtern, Product>();

            CreateMap<OpportunityExtern, Opportunity>()
                .ForMember(x => x.customer, o => o.MapFrom(z => z.customer))
                .ForMember(x => x.Company, o=> o.MapFrom(z => z.company))
                .ForMember(x => x.opportunityStatus, o=> o.MapFrom(z => z.opportunityStatus))
                .ForMember(x => x.oppProducts, o => o.MapFrom(z => z.opportunityProducts));

            CreateMap<OrderNoteExtern, OrderNote>()
                .ForMember(x => x.company, o => o.MapFrom(z => z.company))
                .ForMember(x => x.customer, o => o.MapFrom(z => z.customer))
                .ForMember(x => x.products, o => o.MapFrom(z => z.products))
                .ForMember(x => x.FreightInCharge, o => o.MapFrom(z => z.FreightInCharge))
                .ForMember(x => x.paymentMethod, o => o.MapFrom(z => z.paymentMethod));
        }
    }
}
