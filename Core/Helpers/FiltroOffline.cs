using Core.Model;
using Core.ViewModels.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Core.Helpers
{
    public static class FiltroOffline
    {
        public static List<OrderNote> SearchOrders(List<OrderNote> ordsCache, FilterOrderModel filtro)
        {
            var OrdFiltro = new List<OrderNote>();

            if (filtro.companyId != null && filtro.orderStatusId == null && filtro.priceFrom == null && filtro.priceTo == null && filtro.userId == null) //Company
            {
                OrdFiltro.AddRange(ordsCache.Where(x => x.companyId == filtro.companyId
                                                && x.fecha >= filtro.dateFrom && x.fecha <= filtro.dateTo));
            }

            if (filtro.companyId == null && filtro.orderStatusId != null && filtro.priceFrom == null && filtro.priceTo == null && filtro.userId == null) //Status
            {
                OrdFiltro.AddRange(ordsCache.Where(x => x.orderStatus == filtro.orderStatusId
                                                && x.fecha >= filtro.dateFrom && x.fecha <= filtro.dateTo));
            }

            if (filtro.companyId == null && filtro.orderStatusId == null && filtro.priceFrom != null && filtro.priceTo == null && filtro.userId == null) //Price From
            {
                OrdFiltro.AddRange(ordsCache.Where(x => x.total >= Convert.ToDecimal(filtro.priceFrom)
                                                && x.fecha >= filtro.dateFrom && x.fecha <= filtro.dateTo));
            }

            if (filtro.companyId == null && filtro.orderStatusId == null && filtro.priceFrom == null && filtro.priceTo != null && filtro.userId == null) //Price To
            {
                OrdFiltro.AddRange(ordsCache.Where(x => x.total == Convert.ToDecimal(filtro.priceTo)
                                                && x.fecha >= filtro.dateFrom && x.fecha <= filtro.dateTo));
            }

            if (filtro.companyId == null && filtro.orderStatusId == null && filtro.priceFrom == null && filtro.priceTo == null && filtro.userId != null) //Seller
            {
                //OrdFiltro.AddRange(ordsCache.Where(x => x.createdBy == filtro.sellerId
                //                                && x.fecha >= filtro.dateFrom && x.fecha <= filtro.dateTo)); // esperando que se agregue CREATEDBY en los pedidos
            }

            if (filtro.companyId != null && filtro.orderStatusId != null && filtro.priceFrom == null && filtro.priceTo == null && filtro.userId == null) //Company and Status
            {
                OrdFiltro.AddRange(ordsCache.Where(x => x.companyId == filtro.companyId && x.orderStatus == filtro.orderStatusId
                                                && x.fecha >= filtro.dateFrom && x.fecha <= filtro.dateTo));
            }

            if (filtro.companyId != null && filtro.orderStatusId == null && filtro.priceFrom != null && filtro.priceTo == null && filtro.userId == null) //Company and Price From
            {
                OrdFiltro.AddRange(ordsCache.Where(x => x.companyId == filtro.companyId && x.total >= Convert.ToDecimal(filtro.priceFrom)
                                                && x.fecha >= filtro.dateFrom && x.fecha <= filtro.dateTo));
            }

            if (filtro.companyId != null && filtro.orderStatusId == null && filtro.priceFrom == null && filtro.priceTo != null && filtro.userId == null) //Company and Price To
            {
                OrdFiltro.AddRange(ordsCache.Where(x => x.companyId == filtro.companyId && x.total <= Convert.ToDecimal(filtro.priceTo)
                                                && x.fecha >= filtro.dateFrom && x.fecha <= filtro.dateTo));
            }

            if (filtro.companyId != null && filtro.orderStatusId == null && filtro.priceFrom == null && filtro.priceTo == null && filtro.userId != null) //Company and Seller
            {
                //OrdFiltro.AddRange(ordsCache.Where(x => x.companyId == filtro.companyId && x.createdBy == filtro.sellerId
                //                                && x.fecha >= filtro.dateFrom && x.fecha <= filtro.dateTo));
            }

            if (filtro.companyId == null && filtro.orderStatusId != null && filtro.priceFrom == null && filtro.priceTo != null && filtro.userId == null) //Status and Price From
            {
                OrdFiltro.AddRange(ordsCache.Where(x => x.orderStatus == filtro.orderStatusId && x.total <= Convert.ToDecimal(filtro.priceFrom)
                                                && x.fecha >= filtro.dateFrom && x.fecha <= filtro.dateTo));
            }

            if (filtro.companyId == null && filtro.orderStatusId != null && filtro.priceFrom == null && filtro.priceTo != null && filtro.userId == null) //Status and Price To
            {
                OrdFiltro.AddRange(ordsCache.Where(x => x.orderStatus == filtro.orderStatusId && x.total <= Convert.ToDecimal(filtro.priceTo)
                                                && x.fecha >= filtro.dateFrom && x.fecha <= filtro.dateTo));
            }

            if (filtro.companyId == null && filtro.orderStatusId != null && filtro.priceFrom == null && filtro.priceTo == null && filtro.userId != null) //Status and Seller
            {
                //OrdFiltro.AddRange(ordsCache.Where(x => x.orderStatus == filtro.orderStatusId && x.createdBy == filtro.sellerId
                //                                && x.fecha >= filtro.dateFrom && x.fecha <= filtro.dateTo));
            }

            if (filtro.companyId == null && filtro.orderStatusId == null && filtro.priceFrom != null && filtro.priceTo != null && filtro.userId == null) //Price From and Price To
            {
                OrdFiltro.AddRange(ordsCache.Where(x => x.total >= Convert.ToDecimal(filtro.priceFrom) && x.total <= Convert.ToDecimal(filtro.priceTo)
                                                && x.fecha >= filtro.dateFrom && x.fecha <= filtro.dateTo));
            }

            return OrdFiltro;
        }

        public static List<Opportunity> SearchOppotunities(List<Opportunity> opportunitiesCache, FilterOportunityModel filtro)
        {
            var OppCache = new List<Opportunity>();

            return OppCache;
        }
    }

}
