using System.Linq;
using SolEx.Hurt.Core.ModelBLL;
using SolEx.Hurt.Model.Enums;
using SolEx.Hurt.Model.Interfaces;
using SolEx.Hurt.Model.Interfaces.Sync;

namespace SolEx.Hurt.Sync.ProviderOptima
{
    using Model;
    using System;
    using System.Collections.Generic;

    public class Provider : ISyncProvider, IDokumentyRoznicowe, IDrukowanieDokumentowPdf, IEksportZdjecNadysk, ISciaganieNowychRejestracji, IPobieraniePolaDokumentu, IPobieranieZamiennikow, IPobieranieKodwKreskowych
    {
        private MainDAO _dao;

        private Optima _optima = Optima.PobierzInstancje;
        public Optima OptimaInstancja
        {
            get { return _optima; }
            set { _optima = value; }
        }

        public string przesunMazazyn(List<ZamowienieProdukt> suBraki, string mag, string docelowy, string nr, string uwagi)
        {
            throw new NotImplementedException();
        }

        public bool CleanUp()
        {
            try
            {
                OptimaInstancja.Wylogowanie();
            }
            catch (Exception)
            {
                
            }
            return true;
        }

        public MainDAO DAO
        {
            get
            {
                if (_dao == null)
                {
                    _dao = new MainDAO(_config);
                }
                return _dao;
            }
        }

        public string SourceCS { get; set; }

        public List<Produkt> PobierzProdukty(out List<Tlumaczenie> slownikiTlumaczen, out List<JednostkaProduktu> jednostki, HashSet<string> magazyny)
        {
            return DAO.PobierzProdukty(out slownikiTlumaczen, out jednostki);
        }


        public List<KategoriaKlienta> PobierzKategorieKlientow()
        {
            return DAO.GetCustomerCategories();
        }

        public Dictionary<long, Klient> PobierzKlientow(List<Klient> klienciNaPlatformie, out Dictionary<Adres, KlientAdres> adresy)
        {
            return DAO.PobierzKlientow(klienciNaPlatformie, out adresy);
        }
        public List<Cecha> PobierzCechyIAtrybuty(out List<Atrybut> atrybuty, int[] atrybutydlaktorychniepobieramycechy)
        {
            return DAO.GetAttributes(out atrybuty);
        }

        public List<ProduktCecha> PobierzCechyProduktow_Polaczenia(int[] atrybutydlaktorychpobieramycechy)
        {
            return DAO.GetAttributesLacznik();
        }

        public List<ZamowienieSynchronizacja> ImportZamowien(List<ZamowienieSynchronizacja> zamowienia, Dictionary<long, Klient> wszyscy)
        {
            return DAO.SetOrders(zamowienia,wszyscy);
        }

        public List<StatusZamowienia> PobierzStatusyDokumentow()
        {
            return DAO.PobierzStatusyDokumnetow();
            //return new List<zamowienia_statusy>();
        }

        public Dictionary<long, decimal> PobierzStanyDlaMagazynu(string mag)
        {
            return DAO.GetLiteProducts(mag);
        }

        public List<CenaPoziomu> PobierzPoziomyCenoweProduktow()
        {
            return DAO.GetProductPrices();
        }

        public List<PoziomCenowy> PobierzDostepnePoziomyCen()
        {
            return DAO.GetPriceLevels();
        }


        public List<KlientKategoriaKlienta> PobierzKategorieKlientowPolaczenia()
        {
            return DAO.KategorieKlientowPolaczenia();
        }

        public Dictionary<HistoriaDokumentu, List<HistoriaDokumentuProdukt>> DokumentyDoWyslania(Dictionary<int, long> dokumentyNaPlatformie, DateTime @from, List<Klient> klienciNaPlatformie)
        {
            return DAO.GetDocuments(dokumentyNaPlatformie, from, klienciNaPlatformie);
        }

        public List<int> DokumentyDoUsuniecia(Dictionary<int, long> dokumentyNaPlatformie, HashSet<long> idKlientowB2b)
        {
            return DAO.GetDocumentsToDelete(dokumentyNaPlatformie, idKlientowB2b);
           
        }

        public List<Rabat> PobierzRabaty(Dictionary<long, Klient> dlaKogoLiczyc, Dictionary<long, Produkt> produkty, List<PoziomCenowy> ceny, List<Cecha> cechy, List<ProduktCecha> cechyProdukty, Dictionary<long, KategoriaProduktu> kategorie, List<ProduktKategoria> produktyKategorie, IDictionary<int, KategoriaKlienta> kategorieKlientow, IDictionary<long, KlientKategoriaKlienta> klienciKategorie)
        {
            return DAO.GetDiscounts(dlaKogoLiczyc.Keys.ToList(), produkty.Values.ToList(), ceny, cechy, cechyProdukty, kategorie, produktyKategorie, kategorieKlientow, klienciKategorie);
        }

        public bool DrukujPdfDokument(StatusDokumentuPDF docId, ref string sciezka)
        {
            return OptimaInstancja.Drukuj(docId.IdDokumentu, sciezka);
        }

      

        public void ZapiszZdjeciaNaDysk(string path, string separator, TypyPolDoDopasowaniaZdjecia polaZapisuZdjec)
        {
            DAO.ZapiszZdjecia(path, separator, polaZapisuZdjec);
        }



        private IConfigSynchro _config;
        public void UstawParametryLaczenia(IConfigSynchro config)
        {
            _config = config;
        }


        public Rejestracja ImportKlientowDoERP(Rejestracja klienci)
        {
            return DAO.CreateCustomers(klienci);
        }


        public List<Kraje> PobierzKraje()
        {
            return DAO.PobierzKraje();
        }


        public List<Region> PobierzRegiony()
        {
            return DAO.PobierzWojewodztwa();
        }

        public string PobierzPole(int dokumentId, string pole)
        {
            throw new NotImplementedException("pola dokumentów z optimy");
        }

        public Dictionary<int, Dictionary<string, string>> PobierzPole(HashSet<int> dokumentyId)
        {
            //TODO pobieranie wszystkich pól z dokumentów, teraz pobiera tylko tutaj nr listu przewozowego ale nie dotyka pól w dokumencie
            return DAO.PobierzNrListowPrzewozowych(dokumentyId);
        }

        public List<ProduktyZamienniki> PobierzZamiennikiProduktu(long produkt)
        {
            return DAO.PobierzZamienniki(produkt);
        }

        public List<ProduktyKodyDodatkowe> PobierzAlternatywneKodyKreskowe()
        {
            return DAO.PobierzAlternatywneKodyKreskowe();
        }


        public List<Magazyn> PobierzMagazynyErp()
        {
            throw new NotImplementedException();
        }

        public Dictionary<long, Waluta> PobierzDostepneWaluty()
        {
            return DAO.PobierzDostepneWaluty();
        }
    }
}