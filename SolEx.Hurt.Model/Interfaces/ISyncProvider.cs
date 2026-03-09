using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SolEx.Hurt.Model
{
    public interface ISyncProvider
    {
        string SourceCS { get;set; }

        List<produkty> GetProducts(  out List<Model.slowniki> slowniki);
        List<cechy_produkty> GetProductTraits();
        List<produkty_kategorie_zrodlowe> GetProductCategories();
        List<platnosci> GetPayments();

        List<Category> GetCustomerCategories();

        List<klienci> GetCustomers(List<klienci> klienciNaPlatformie);

        List<historia_dokumenty> GetDocuments(List<int?> ids, DateTime startDate, DateTime startDate2, out List<historia_dokumenty_produkty> subitems);

        List<DiscountItem> GetDiscounts();

        List<kategorie_zrodlowe> GetCategories();

        List<cechy> GetAttributes(out List<atrybuty> atrybuty);

        List<historia_dokumenty> SetOrders(List<Order> list);

        List<flat_stany> GetLiteProducts();

        Dictionary<int, decimal> PobierzStanyDlaMagazynu(magazyny mag);

        List<produkty> GetLiteProductsPrices();

        List<Complaint> GetComplaints();

        List<string> CreateDocs(SolEx.Hurt.Model.Core.SendingDocs docs);//zwraca listę plików do wysłania na serwer
        
        bool CleanUp();

        List<PriceLevel> GetProductsPriceLevels();
    }

}
