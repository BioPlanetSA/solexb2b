using System.Reflection;
using SolEx.Hurt.Core.ModelBLL;
using SolEx.Hurt.Helpers;
using SolEx.Hurt.Model;
using SolEx.Hurt.Model.AtrybutyKlas;
using SolEx.Hurt.Model.CustomSearchCriteria;
using System;
using System.Collections.Generic;
using System.Linq;
using SolEx.Hurt.Core.Sync;
using SolEx.Hurt.Model.Helpers;
using SolEx.Hurt.Model.Interfaces.Sync;


namespace SolEx.Hurt.Sync.App.Modules_.NoweModuly.Produkty
{
    public class KopiowanieCechyDoPola : SyncModul, Model.Interfaces.SyncModuly.IModulProdukty
    {
        [FriendlyName("Nazwa atrybutu, którego wartość będzie skopiowana do wybranego pola")]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        public string Atrybut { get; set; }

        [FriendlyName("Pole, do którego będzie skopiowana wartość atrybutu")]
        [SyncSlownikNaPodstawieInnegoTypu(typeof(Produkt))]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        public string Pole { get; set; }

        public KopiowanieCechyDoPola()
        {
            Atrybut = "";
            Pole = "";
        }

        public override string uwagi
        {
            get { return "Moduł nie będzie już wspierany. Należy wykorzystać moduł o nazwie PrzepisanieCechDoPola, w którym można podać dowolną ilość atrybutów."; }
        }

        public override string Opis
        {
            get { return "WYCOFANY !!!  Automatyczne kopiowanie wartości atrybutu do wybranego pola produktu. Należy korzystać z PrzepisanieCechDoPola"; }
        }

        public void Przetworz(ref List<Produkt> listaWejsciowa, ref List<Tlumaczenie> produktyTlumaczenia, Dictionary<long, Produkt> produktyNaB2B, ref List<JednostkaProduktu> jednostki, ref Dictionary<long, ProduktCecha> lacznikiCech, ref List<ProduktKategoria> lacznikiKategorii, ISyncProvider provider, ref List<ProduktUkryty> produktuUkryteErp, ref List<ProduktyZamienniki> zamienniki, Dictionary<long, KategoriaProduktu> kategorie, ref List<Cecha> cechy, ref List<Atrybut> atrybuty)
        {
            if (string.IsNullOrEmpty(Atrybut) || string.IsNullOrEmpty(Pole))
                return;

            PropertyInfo[] propertisy = typeof(Produkt).GetProperties(); 

            //var atrybuty = ApiWywolanie.PobierzAtrybuty().Values;
            //var cechy = ApiWywolanie.PobierzCechy().Values;
            var cechyprodukty = ApiWywolanie.PobierzCechyProdukty().Values;

            var atrybut = atrybuty.FirstOrDefault(a => a.Nazwa.ToLower() == Atrybut.ToLower());
            if (atrybut == null)
            {
                Log.Error("Nie znaleziono atrybutu " + Atrybut);
                return;
            }

            var wybranaCecha = cechy.FirstOrDefault(a => a.AtrybutId == atrybut.Id);
            if (wybranaCecha == null)
            {
                Log.Error("Nie znaleziono cech dla atrybutu " + Atrybut);
                return;
            }

            foreach (Produkt produkt in listaWejsciowa)
            {

                var cechaproduktu = cechyprodukty.FirstOrDefault(b => wybranaCecha.Id == b.CechaId);

                if (cechaproduktu != null)
                {
                    foreach (var p in propertisy)
                    {
                        string nazwa = p.Name;
                        var atribut = p.GetCustomAttributes(true).FirstOrDefault(a => a.GetType() == typeof(FriendlyNameAttribute)) as FriendlyNameAttribute;
                        if (atribut!=null)
                        {
                            nazwa = atribut.FriendlyName;
                        }
                        if (nazwa == Pole)
                        {
                            try
                            {
                                if (p.PropertyType == typeof (decimal))
                                {
                                    decimal liczba = 0;
                                    if (TextHelper.PobierzInstancje.SprobojSparsowac(wybranaCecha.Nazwa.Trim(), out liczba))
                                    {
                                        p.SetValue(produkt, liczba, null);
                                    }
                                }
                                else p.SetValue(produkt, wybranaCecha.Nazwa.Trim(), null);

                                break;
                            }
                            catch (Exception ex)
                            {
                                LogiFormatki.PobierzInstancje.LogujDebug("błąd przy przetwarzaniu towaru " + ex.Message);
                            }
                        }
                    }
                }
            }
        }
    }
}
