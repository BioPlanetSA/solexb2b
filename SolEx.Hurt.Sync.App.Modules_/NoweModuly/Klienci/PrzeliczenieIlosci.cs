using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using SolEx.Hurt.Core.ModelBLL;
using SolEx.Hurt.Core.Sync;
using SolEx.Hurt.Model;
using SolEx.Hurt.Model.AtrybutyKlas;
using SolEx.Hurt.Model.Enums;
using SolEx.Hurt.Model.Helpers;
using SolEx.Hurt.Model.Interfaces.Sync;
using SolEx.Hurt.Model.Interfaces.SyncModuly;

using Adres = SolEx.Hurt.Model.Adres;

namespace SolEx.Hurt.Sync.App.Modules_.NoweModuly.Klienci
{

    [Obsolete]
    public class PrzeliczenieIlosci : SyncModul, IModulKlienci
    {
        public override string uwagi
        {
            get { return "Smmash. Przelicza kupowane ilości  przez klientów, jeśli wybrano konkretną datę należy wprowadzić w formacie dd.MM.yyyy"; }
        }
        [FriendlyName("Typ dokumentu")]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        public RodzajDokumentu RodzajDokumentu { get; set; }
        
        [FriendlyName("Od kiedy uwzględniać dokumenty")]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        public DataOdKiedyLiczyc OdKiedyLiczyc { get; set; }
        
        [FriendlyName("Parametry od kiedy")]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        [Niewymagane]
        public string DataOdKiedy { get; set; }

        [FriendlyName("Do kiedy uwzględniać dokumenty")]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        public DataOdKiedyLiczyc DoKiedyLiczyc { get; set; }
        
        [FriendlyName("Parametry do kiedy")]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        [Niewymagane]
        public string DataDoKiedy { get; set; }

        [FriendlyName("Ścieżka do pliku CSV z dodatkowymi ilościami do sumowania")]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        public string SciezkaDoCSVZDodatkowymiIlosciami { get; set; }



        private DateTime? PoprawDate(DateTime? datetime)
        {
            if (datetime == DateTime.MaxValue || datetime == DateTime.MinValue)
                return null;

            return datetime;
        }

        protected void ZrobDaty(out DateTime? odkiedy, out DateTime? dokiedy)
        {
            odkiedy = DateTimeHelper.PobierzInstancje.WyliczDate(OdKiedyLiczyc, DataOdKiedy);
            dokiedy = DateTimeHelper.PobierzInstancje.WyliczDate(DoKiedyLiczyc, DataDoKiedy);

            odkiedy = PoprawDate(odkiedy);
            dokiedy = PoprawDate(dokiedy);  
        }
        public void Przetworz(ref Dictionary<long, Klient> listaWejsciowa, Dictionary<long, Produkt> produktyB2B, ref Dictionary<Adres, KlientAdres> adresyWErp, List<KategoriaKlienta> kategorie, List<KlientKategoriaKlienta> laczniki, ref List<Sklep> sklepy, ref List<SklepKategoriaSklepu> sklpeylaczniki, ref List<KategoriaSklepu> sklepyKategorie, ref List<Kraje> kraje, ref List<Region> regiony, ref List<Magazyn> magazyny, ISyncProvider provider)
        {
            //wylaczone - jesli jest potrzebny to trzeba roibic sztuczen zamowienia
            throw new NotImplementedException();
            //DateTime? odkiedy;
            //DateTime? dokiedy;
            //ZrobDaty(out odkiedy,out dokiedy);
            //Dictionary<long, KupowaneIlosci> ilosciKlientow = ilosci.ToDictionary(x => x.Id, x => x);
            //DodajDane(odkiedy, dokiedy, ilosciKlientow, listaWejsciowa, produktyB2B);
            //ilosci.Clear();
            //foreach (var ilosc in ilosciKlientow)
            //{
            //    ilosci.Add(ilosc.Value);
            //}
        }

        protected virtual void DodajDane(DateTime? odkiedy, DateTime? dokiedy, Dictionary<long, KupowaneIlosci> ilosciKlientow, Dictionary<long, Klient> listaWejsciowa, Dictionary<long, Produkt> produktyB2B)
        {
            
            if (File.Exists(SciezkaDoCSVZDodatkowymiIlosciami))
            {
                string[,] plikcsv = new Hurt.Helpers.CSVHelperExt().OdczytajCSV(SciezkaDoCSVZDodatkowymiIlosciami);
                for (int i = 1; i < plikcsv.GetLength(0); i++)
                {
                    string symbolKlienta = plikcsv[i, 0];
                    Klient klient = listaWejsciowa.Values.FirstOrDefault(a => a.Symbol.Equals(symbolKlienta, StringComparison.InvariantCultureIgnoreCase));
                    if (klient == null)
                    {
                        LogiFormatki.PobierzInstancje.LogujError(new Exception(string.Format("W pliku CSV {0} nie znaleziono klienta o symbolu {1}", SciezkaDoCSVZDodatkowymiIlosciami, symbolKlienta)));
                        continue;
                    }

                    string symbol = plikcsv[i, 1];
                    string wersja = plikcsv[i, 2];
                    string iloscProduktu = plikcsv[i, 3];
                    string rodzina = string.Format("{0}-{1}", symbol, wersja);
                    Produkt produkt = produktyB2B.Values.FirstOrDefault(a => a.PoleLiczba2 == klient.Id && a.Rodzina.Equals(rodzina, StringComparison.InvariantCultureIgnoreCase));
                    if (produkt == null || produkt.Id == 0)
                    {
                        LogiFormatki.PobierzInstancje.LogujError(new Exception(string.Format("Dla klienta o symbolu {0} nie znaleziono produktu o symbolu {1}", symbolKlienta, rodzina)));
                        continue;
                    }

                    decimal ilosc;
                    if (TextHelper.PobierzInstancje.SprobojSparsowac(iloscProduktu, out ilosc))
                    {
                        DodajWpis(odkiedy, dokiedy, ilosc, produkt.Id, klient.Id, ilosciKlientow);
                    }
                }

            }
            else throw new Exception("Nie znaleziono pliku " + SciezkaDoCSVZDodatkowymiIlosciami);
        }
        protected void DodajWpis(DateTime? dokiedy, DateTime? odkiedy, decimal ilosc, long produkt, long klient, Dictionary<long, KupowaneIlosci> ilosciKlientow)
        {
            KupowaneIlosci kupowaneIlosci = new KupowaneIlosci
            {
                DataZakupu = dokiedy,
                KlientId = klient,
                RodzajDokumentu = RodzajDokumentu,
                Ilosc = ilosc,
                ProduktId = produkt
            };

            long klucz = kupowaneIlosci.Id;

            if (!ilosciKlientow.ContainsKey(klucz))
            {
                ilosciKlientow.Add(klucz, kupowaneIlosci);
            }
            else
            {
                LogiFormatki.PobierzInstancje.LogujError(new Exception(string.Format("Zdublowany produkt {0} u klienta {1}. Wybrano większą wartość.", produkt, klient)));

                if (ilosciKlientow[klucz].Ilosc < kupowaneIlosci.Ilosc)
                    ilosciKlientow[klucz] = kupowaneIlosci;
            }
        }

    }
}
