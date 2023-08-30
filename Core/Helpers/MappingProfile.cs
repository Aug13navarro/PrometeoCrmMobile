using AutoMapper;
using Core.Data.Tables;
using Core.Model;
using Core.Model.Extern;
using Newtonsoft.Json;
using System.Collections.Generic;
using static Core.Model.OrderNote;

namespace Core.Helpers
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<CompanyExtern, Company>();

            CreateMap<CustomerTable, Customer>()
                .ForMember(x => x.Id, o => o.MapFrom(z => z.Id))
                .ForMember(x => x.CompanyName, o => o.MapFrom(z => z.CompanyName))
                .ForMember(x => x.BusinessName, o => o.MapFrom(z => z.BusinessName))
                .ForMember(x => x.Abbreviature, o => o.MapFrom(z => z.Abbreviature))
                .ForMember(x => x.TypeId, o => o.MapFrom(z => z.TypeId))
                .ForMember(x => x.IdNumber, o => o.MapFrom(z => z.IdNumber))
                .ForMember(x => x.IdParentCustomer, o => o.MapFrom(z => z.IdParentCustomer))
                .ForMember(x => x.TaxCondition, o => o.MapFrom(z => z.TaxCondition))
                .ForMember(x => x.AccountOwnerId, o => o.MapFrom(z => z.AccountOwnerId))
                .ForMember(x => x.AccountOwnerName, o => o.MapFrom(z => z.AccountOwnerName))
                .ForMember(x => x.PesosBalance, o => o.MapFrom(z => z.PesosBalance))
                .ForMember(x => x.DollarBalance, o => o.MapFrom(z => z.DollarBalance))
                .ForMember(x => x.UnitBalance, o => o.MapFrom(z => z.UnitBalance))
                .ForMember(x => x.Descriptions, o => o.MapFrom(z => z.Descriptions))
                .ForMember(x => x.externalCustomerId, o => o.MapFrom(z => z.externalCustomerId))
                .ForMember(x => x.IsContactsVisible, o => o.MapFrom(z => z.IsContactsVisible))
                .ForMember(x => x.CompanyId, o => o.MapFrom(z => z.CompanyId));
            CreateMap<Customer, CustomerTable>();

            CreateMap<OpportunityStatusExtern, OpportunityStatus>();

            CreateMap<UserExtern, User>();

            CreateMap<PaymentMethodExtern, PaymentMethod>();

            CreateMap<IncotermExtern, Incoterm>();

            CreateMap<FreightInChargeExtern, FreightInCharge>();

            CreateMap<TransportExtern, Transport>();

            CreateMap<PaymentConditionTable, PaymentCondition>();

            CreateMap<AssistantComercialTable, User>();
            CreateMap<TransportCompanyTable, TransportCompany>();

            CreateMap<ProductExtern, Product>();

            CreateMap<ProductTable, Product>()
                .ForMember(x => x.Id, o => o.MapFrom(z => z.Id))
                .ForMember(x => x.name, o => o.MapFrom(z => z.Name))
                .ForMember(x => x.price, o => o.MapFrom(z => z.Price))
                .ForMember(x => x.stock, o => o.MapFrom(z => z.Stock))
                .ForMember(x => x.Discount, o => o.MapFrom(z => z.Discount))
                .ForMember(x => x.quantity, o => o.MapFrom(z => z.Quantity))
                .ForMember(x => x.CompanyId, o => o.MapFrom(z => z.CompanyId));

            CreateMap<OrderNote, OrderNoteTable>()
                .ForMember(x => x.productsJson, o => o.MapFrom(z => JsonConvert.SerializeObject(z.products)))
                .ForMember(x => x.companyJson, o => o.MapFrom(z => JsonConvert.SerializeObject(z.company)))
                .ForMember(x => x.customerJson, o => o.MapFrom(z => JsonConvert.SerializeObject(z.customer)));

            CreateMap<OrderNoteTable, OrderNote>()
                .ForMember(x => x.idOffline, o => o.MapFrom(z => z.idOffline))
                //.ForMember(x => x.products, o => o.Ignore())
                .ForMember(x => x.products, o => o.MapFrom(z => JsonConvert.DeserializeObject<List<ProductOrder>>(z.productsJson)))
                .ForMember(x => x.company, o => o.MapFrom(z => JsonConvert.DeserializeObject<Company>(z.companyJson)))
                .ForMember(x => x.customer, o => o.MapFrom(z => JsonConvert.DeserializeObject<Customer>(z.customerJson)))
                .ForMember(x => x.currency, o => o.Ignore())
                .ForMember(x => x.Details, o => o.Ignore())
                .ForMember(x => x.TransportCompany, o => o.Ignore())
                .ForMember(x => x.paymentMethod, o => o.Ignore())
                .ForMember(x => x.FreightInCharge, o => o.Ignore());
        }
    }
}
