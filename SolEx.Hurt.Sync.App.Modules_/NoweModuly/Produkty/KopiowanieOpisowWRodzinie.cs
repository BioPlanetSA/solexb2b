using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using ServiceStack.Text;
using SolEx.Hurt.Core.BLL;
using SolEx.Hurt.Core.Helper;
using SolEx.Hurt.Core.ModelBLL;
using SolEx.Hurt.Core.Sync;
using SolEx.Hurt.Helpers;
using SolEx.Hurt.Model;
using SolEx.Hurt.Model.AtrybutyKlas;
using SolEx.Hurt.Model.Helpers;
using SolEx.Hurt.Model.Interfaces;
using SolEx.Hurt.Model.Interfaces.Sync;


namespace SolEx.Hurt.Sync.App.Modules_.NoweModuly.Produkty
{
    public class KopiowanieOpisowWRodzinie : SyncModul, Model.Interfaces.SyncModuly.IModulProdukty
    {
        public KopiowanieOpisowWRodzinie()
        {
            WymuszenieKopiowania = false;
        }

        public override string uwagi
        {
            get { return ""; }
        }


        [FriendlyName("Pola które będą kopiowane w obrębie rodziny")]
        [PobieranieSlownika(typeof(SlownikPolStringowychProduktu))]
        [Niewymagane]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        public List<string> Pola { get; set; }

        [FriendlyName("Czy kopiować również niepuste pola? ")]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        public bool WymuszenieKopiowania { get; set; }

        public void Przetworz(ref List<Produkt> listaWejsciowa, ref List<Tlumaczenie> produktyTlumaczenia, Dictionary<long, Produkt> produktyNaB2B, ref List<JednostkaProduktu> jednostki, ref Dictionary<long, ProduktCecha> lacznikiCech, ref List<ProduktKategoria> lacznikiKategorii, ISyncProvider provider, ref List<ProduktUkryty> produktuUkryteErp, ref List<ProduktyZamienniki> zamienniki, Dictionary<long, KategoriaProduktu> kategorie, ref List<Cecha> cechy, ref List<Atrybut> atrybuty)
        {
            Dictionary<string, List<Produkt>> slownikRodzin = SlownikRodzin.PobierzRodzinySlownik(listaWejsciowa);
            if (!slownikRodzin.Any())
            {
                LogiFormatki.PobierzInstancje.LogujInfo("Brak rodzin - przerywam działanie. ");
                return;
            }
            List<string> iloscZmian = new List<string>();
            Log.DebugFormat("Przetwarzam {0} rodzin", slownikRodzin.Count);
            foreach (var rodzinaProdukt in slownikRodzin)
            {
                foreach (var pole in Pola)
                {
                    string pole1 = pole;
                    var propertisy = typeof(Produkt).GetProperties().FirstOrDefault(x=>x.Name==pole1);
                    if (propertisy == null)
                    {
                        LogiFormatki.PobierzInstancje.LogujDebug(string.Format("Brak pola o nazwie: {0}", pole1));
                        continue;
                    }
                    //Znajdujemy najdluzsza wartosc pola
                    string najdluzszaWartosc = string.Empty;
                    Produkt produktZrodlowy = new Produkt();
                    foreach (var produkt in rodzinaProdukt.Value)
                    {
                        var wartosciPolaWRodzinie = propertisy.GetValue(produkt);
                        if (wartosciPolaWRodzinie == null)
                        {
                            continue;
                        }
                        if (wartosciPolaWRodzinie.ToString().Trim().Length > najdluzszaWartosc.Length)
                        {
                            najdluzszaWartosc = wartosciPolaWRodzinie.ToString();
                            produktZrodlowy = produkt;
                        }
                    }
                    //Przechodzimy po produktach i sprawdzamy czy ma pusta wartosc bądz jest wymuszenie uzupelnienia pola
                    foreach (var produkt in rodzinaProdukt.Value)
                    {
                        var wartosc = propertisy.GetValue(produkt);
                        if (WymuszenieKopiowania || wartosc == null)
                        {
                            WypelnijPoleProduktu(pole, listaWejsciowa.First(x=>x.Id==produkt.Id), produktZrodlowy, produktyTlumaczenia);
                            if (!iloscZmian.Contains(rodzinaProdukt.Key))
                            {
                                iloscZmian.Add(rodzinaProdukt.Key);
                            }
                        }
                    }
                }
            }
            LogiFormatki.PobierzInstancje.LogujInfo("Poprawiono {0} rodzin. Rodziny te to: {1}", iloscZmian.Count, iloscZmian.ToCsv());
        }
        public IConfigSynchro config = SyncManager.PobierzInstancje.Konfiguracja;
        private void WypelnijPoleProduktu(string pole, Produkt produktDocelowy, Produkt produktZrodlowy,List<Tlumaczenie> produktyTlumaczenia)
        {
            Dictionary<int, Jezyk> jezyki = config.JezykiWSystemie;
            var prop = typeof(Produkt).GetProperties().FirstOrDefault(x => x.Name == pole);
            string wartosc = prop.GetValue(produktZrodlowy).ToString();//ToString - ze wzgledu na fakt ze działamy tylko na polach stringowych

            prop.SetValue(produktDocelowy,wartosc);


            var typ = typeof(ProduktBazowy).PobierzOpisTypu();
            if (typ == null)
            {
                Log.ErrorFormat("Brak typu produkty w systemie");

            }
            else
            {
                foreach (var jezyk in jezyki)
                {
                    var wartoscTlumaczenia =
                        produktyTlumaczenia.FirstOrDefault(
                            x => x.JezykId == jezyk.Key && x.ObiektId == produktZrodlowy.Id && x.Pole == pole);
                    var czyJestJuzTlumaczenie =
                        produktyTlumaczenia.FirstOrDefault(
                            x => x.ObiektId == produktDocelowy.Id && x.JezykId == jezyk.Key && x.Pole == pole);
                    if (czyJestJuzTlumaczenie != null)
                    {
                        czyJestJuzTlumaczenie.Wpis = wartoscTlumaczenia == null ? wartosc : wartoscTlumaczenia.Wpis;
                    }
                    else
                    {
                        Tlumaczenie s = new Tlumaczenie()
                        {
                            JezykId = jezyk.Key,
                            ObiektId = produktDocelowy.Id,
                            Pole = pole,
                            Typ = typ,
                            Wpis = wartoscTlumaczenia == null ? wartosc : wartoscTlumaczenia.Wpis,

                        };

                        if (produktyTlumaczenia.FirstOrDefault(x => x.Id == s.Id) == null)
                            produktyTlumaczenia.Add(s);
                    }
                    

                }
            }
            
        }

    }
}
