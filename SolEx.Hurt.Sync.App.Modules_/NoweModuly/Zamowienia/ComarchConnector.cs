using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml;
using SolEx.Hurt.Core.Sync;
using SolEx.Hurt.Model;
using SolEx.Hurt.Model.AtrybutyKlas;
using SolEx.Hurt.Model.Enums;
using SolEx.Hurt.Model.Helpers;
using SolEx.Hurt.Model.Interfaces;
using SolEx.Hurt.Sync.App.Modules_.Rozne;

namespace SolEx.Hurt.Sync.App.Modules_.NoweModuly.Zamowienia
{
    [FriendlyName("Import zamówień z plików Edi - comarch")]
    public class ComarchConnector : ImportZPliku
    {
        public IConfigSynchro ConfigBll = SyncManager.PobierzInstancje.Konfiguracja;

        [FriendlyName("Pole w którym zapisany jest numer ILN klienta")]
        [SyncSlownikNaPodstawieInnegoTypu(typeof(Klient))]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        public string PoleIln { get; set; }

        [FriendlyName("Domyślny kod klienta", FriendlyOpis = "Kod który będzie wybierany w przypadku kiedy Iln klienta z zamówienia będzie niepoprawny lub takiego klienta nie będzie w systemie")]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        public string DomyslnyKodIln { get; set; }

        public override IEnumerable<string> WyszukajPlikDoWczytania()
        {
            var pliki = Directory.GetFiles(KatalogZamowien, "*.xml", SearchOption.AllDirectories);
            return PrzefiltrujListePlikow(pliki);
        }

        /// <summary>
        /// Usuwa wszystkie pliki z niepotrzebnych katalogów
        /// </summary>
        /// <param name="listaPlikow"></param>
        /// <returns></returns>
        public IEnumerable<string> PrzefiltrujListePlikow(IEnumerable<string> listaPlikow)
        {
            string katalogStarych = $"{KatalogZamowien.TrimEnd("\\")}\\{katalogStarychZamowien}\\";
            return listaPlikow.Where(a => !a.StartsWith(katalogStarych) && !a.Contains($"\\{katalogArchiwum}\\") && !a.Contains($"\\{katalogbuffer}\\"));
        }

        private string katalogStarychZamowien = "STARE";
        private string katalogArchiwum = "archiwum";
        private string katalogbuffer = "buffer";

        private Klient ZnajdzKlienta(string numer)
        {
            PropertyInfo[] propertisy = typeof(Klient).GetProperties();
            PropertyInfo info = propertisy.First(x => x.Name == PoleIln);
            foreach (Klient k in Klienci.Values)
            {
                object val = info.GetValue(k);
                if (val != null && val.ToString() == numer)
                {
                    return k;
                }
            }

            LogiFormatki.PobierzInstancje.LogujInfo($"Nie można wyszukać klienta o numerze ILN: '{numer}' - klient musi być na B2B i w polu: {PoleIln} mieć wpisany podany numer ILN.");
            return null;
        }

        public virtual XmlDocument PobierzDokument(string sciezka)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(sciezka);
            return doc;
        }

        public override ZamowieniaImport WczytajZPliku(string sciezka, Dictionary<long, Jednostka> jednostki, Dictionary<long, ProduktJednostka> produktyjednostki)
        {
            return WczytajZPlikuMain(sciezka, jednostki, produktyjednostki);
        }

        public ZamowieniaImport WczytajZPlikuMain(string sciezka, Dictionary<long, Jednostka> jednostki, Dictionary<long, ProduktJednostka> produktyjednostki)
        {
            ZamowieniaImport wynik = new ZamowieniaImport { Rozbijaj = true };
            XmlDocument doc = PobierzDokument(sciezka);
            XmlNode numer = PobierzWezel(doc, "/Document-Order/Order-Header/OrderNumber");
            string nr = "";
            if (numer != null)
            {
                nr = numer.InnerText ?? "";
            }

            wynik.NumerTymczasowyZamowienia = nr;
            wynik.Podtytul = nr;
            wynik.DataUtworzenia = DateTime.Now;
            wynik.TerminDostawy = wynik.DataUtworzenia;
            wynik.ZamowienieImportowanePoStronieKlienta = true;
            XmlNode terminDostawy = PobierzWezel(doc, "/Document-Order/Order-Header/ExpectedDeliveryDate");
            if (terminDostawy != null)
            {
                DateTime data;
                if (DateTime.TryParse(terminDostawy.InnerText, out data))
                    wynik.TerminDostawy = data;
            }
            string dataZamowieniaUwagi = string.Empty;
            XmlNode dataZamowienia = PobierzWezel(doc, "/Document-Order/Order-Header/OrderDate");
            if (dataZamowienia != null)
            {
                DateTime data;
                if (DateTime.TryParse(dataZamowienia.InnerText, out data))
                    dataZamowieniaUwagi = data.ToShortDateString();
            }

            string godzinaZamowieniaUwagi = string.Empty;
            XmlNode godzinaDostawy = PobierzWezel(doc, "/Document-Order/Order-Header/ExpectedDeliveryTime");
            if (godzinaDostawy != null)
            {
                DateTime data;
                if (DateTime.TryParse(godzinaDostawy.InnerText, out data))
                    godzinaZamowieniaUwagi = data.ToShortTimeString();
            }

            wynik.StatusId = (StatusImportuZamowieniaDoErp)ConfigBll.StatusyZamowien.Values.First(x => x.Importowac).Id;

            //wynik.uwagi = "Zamówienie z modułu ComarchConnector";
            wynik.Uwagi = $"Numer pierwotny: {wynik.NumerTymczasowyZamowienia}, data zamówienia: {dataZamowieniaUwagi}, data realizacji: {wynik.TerminDostawy.Value.ToShortDateString()}, data dostawy {godzinaZamowieniaUwagi}";

            XmlNode deliveryPointNode = PobierzWezel(doc, "/Document-Order/Order-Parties/DeliveryPoint/ILN");
            XmlNode buyerNode = PobierzWezel(doc, "/Document-Order/Order-Parties/Buyer/ILN");
            if (deliveryPointNode == null && buyerNode == null)
            {
                throw new Exception("Brak węzła z numerem iln klienta");
            }

            string numerKlienta = !string.IsNullOrEmpty(deliveryPointNode?.InnerText) ? deliveryPointNode.InnerText : buyerNode.InnerText;

            Klient klient = ZnajdzKlienta(numerKlienta);
            if (klient == null)
            {
                if (!string.IsNullOrEmpty(DomyslnyKodIln))
                    klient = ZnajdzKlienta(DomyslnyKodIln);

                if (klient == null)
                    throw new Exception("Brak klienta o numerze " + numerKlienta);
            }
            wynik.KlientId = klient.Id;
            wynik.PoziomCenyId = klient.PoziomCenowyId;
            wynik.WalutaId = klient.WalutaId;

            var pozycje = doc.SelectNodes("/Document-Order/Order-Lines/Line/Line-Item");
            if (pozycje == null)
            {
                throw new Exception("Brak węzłów pozycji");
            }
            if (pozycje.Count == 0)
            {
                throw new Exception("Brak pozycji");
            }

            //pobieranie brakujących cen
            List<FlatCeny> cenyKlienta = null;
            List<string> kodyEaNdoPobrania;
            bool czyPobieracCeny = CzyPobieracCenyKlientaZB2B(pozycje, out kodyEaNdoPobrania);
            if (czyPobieracCeny)
            {
                HashSet<long> idKlienta = new HashSet<long>() { klient.Id };
                try
                {
                    cenyKlienta = ApiWywolanie.PobierzCenyKlientow(idKlienta);
                }
                catch (Exception e)
                {
                    throw new Exception($"Bład pobierania ceny dla klienta:{idKlienta} z platformy. \r\n{e.Message}");
                }
            }

            int i = 0;
            foreach (XmlNode pozycja in pozycje)
            {
                i++;
                XmlNode kod = PobierzWezelZBledem(pozycja, "EAN", i);

                ZamowienieProdukt zp;
                XmlNode cenyWPliku = PobierzWezel(pozycja, "OrderedUnitNetPrice");

                if (ProduktyWgKoduKreskowego.TryGetValue(kod.InnerText, out Produkt produkt))
                {
                    //jest normalny produkt - a nie wydmuszka
                    zp = new ZamowienieProdukt { ProduktId = produkt.Id, ProduktIdBazowy = produkt.Id };
                   
                    if (cenyWPliku == null)
                    {
                        LogiFormatki.PobierzInstancje.LogujError(new Exception($"W pliku xml dla produktu o kodzie EAN {kod.InnerText} brakuje węzła OrderedUnitNetPrice zawierającego cenę produktu. Nastąpi pobranie ceny klienta z platformy."));
                        var cenaB2B = cenyKlienta.FirstOrDefault(a => a.ProduktId == zp.ProduktId);
                        if (cenaB2B != null)
                        {
                            zp.CenaNetto = cenaB2B.CenaNetto;
                            zp.CenaBrutto = Kwoty.WyliczBrutto(zp.CenaNetto, produkt.Vat, klient);
                        }
                    }

                    //JEDNOSTKA
                    XmlNode jednostka = PobierzWezelZBledem(pozycja, "UnitOfMeasure", i);

                    if (string.IsNullOrEmpty(jednostka.InnerText))
                    {
                        throw new Exception("Brak jednostki. Pozycja " + i);
                    }

                    var jednostkaPodstawowa = produktyjednostki.Values.FirstOrDefault(a => a.ProduktId == produkt.Id && a.Podstawowa);

                    var jednostkaB2B = jednostki.FirstOrDefault(a => a.Value.Nazwa == jednostka.InnerText);
                    if (jednostkaB2B.Value != null)
                    {
                        var jednostkaProduktu =
                            produktyjednostki.FirstOrDefault(a => a.Value.JednostkaId == jednostkaB2B.Key);
                        if (jednostkaProduktu.Value != null)
                        {
                            var jednsotka = jednostki.Values.FirstOrDefault(x => x.Nazwa == jednostka.InnerText);

                            if (jednostka == null)
                            {
                                throw new Exception("Bład importu zamówienia COMARCH. Brak jednostki w B2B o nazwie :" + jednostka.InnerText);
                            }

                            zp.UstawJednostke(jednsotka.Nazwa);
                            zp.JednostkaMiary = jednsotka.Id;
                        }
                    }

                    if (string.IsNullOrEmpty(zp.Jednostka) && jednostkaPodstawowa != null)
                    {
                        if (jednostki.ContainsKey(jednostkaPodstawowa.JednostkaId))
                        {
                            var j = jednostki[jednostkaPodstawowa.JednostkaId];
                            zp.UstawJednostke(j.Nazwa);
                            zp.JednostkaMiary = j.Id;
                        }
                    }
                }
                else
                {
                    //sztuczny produkt Bio Planet
                    string tekstZmiana = $"zmiana: {kod.InnerText}";
                    zp = new ZamowienieProdukt() { ProduktId = 2923, ProduktIdBazowy = 2923, Opis = tekstZmiana, Opis2 = tekstZmiana };
                }

                //CENY z pliku o i ile jest podana
                if (cenyWPliku != null)
                {
                    if (!TextHelper.PobierzInstancje.SprobojSparsowac(cenyWPliku.InnerText, out decimal cena))
                    {
                        throw new Exception($"Pozycja {i} Nie udało się odczytać ceny: {cenyWPliku.InnerText}");
                    }
                    zp.CenaNetto = cena;
                    zp.CenaBrutto = Kwoty.WyliczBrutto(zp.CenaNetto, produkt.Vat, klient);
                }

                //ILOŚĆ
                XmlNode ilosc = PobierzWezelZBledem(pozycja, "OrderedQuantity", i);
                decimal ile;
                if (!TextHelper.PobierzInstancje.SprobojSparsowac(ilosc.InnerText, out ile))
                {
                    throw new Exception($"Pozycja {i} Nie udało się odczytać ilości {ilosc.InnerText}");
                }
                zp.Ilosc = ile;

                zp.Id = WygenerujIdDlaZamowieniaPozycji(zp);
                wynik.pozycje.Add(zp);
            }

            return wynik;
        }

        /// <summary>
        /// pobieranie węzła z elementu xml
        /// </summary>
        /// <param name="pozycja"></param>
        /// <param name="nazwa"></param>
        /// <param name="i"></param>
        /// <returns></returns>
        private static XmlNode PobierzWezel(XmlNode pozycja, string nazwa)
        {
            XmlNode wynik = pozycja.SelectSingleNode(nazwa);
            return wynik;
        }

        /// <summary>
        /// Pobiera węzeł ze sprawdzeniem czy jest wartość i jesli nie ma to wyrzuca błąd.
        /// </summary>
        /// <param name="pozycja"></param>
        /// <param name="nazwa"></param>
        /// <param name="i"></param>
        /// <returns></returns>
        private static XmlNode PobierzWezelZBledem(XmlNode pozycja, string nazwa, int i)
        {
            XmlNode wynik = PobierzWezel(pozycja, nazwa);
            if (wynik == null)
            {
                throw new Exception($"Brak węzła {nazwa} dla pozycja {i}");
            }
            return wynik;
        }

        /// <summary>
        /// zgodnie z dokumentacją comarchowego edi ceny netto nie są wymagane w xml dlatego ta metoda sprawdza czy jakikolwiek produkt nie ma ceny i dopiero wtedy pobiera ceny
        /// </summary>
        /// <param name="pozycje"></param>
        /// <returns></returns>
        private bool CzyPobieracCenyKlientaZB2B(XmlNodeList pozycje, out List<string> kodyEAN)
        {
            kodyEAN = new List<string>(pozycje.Count);
            bool czypobierac = false;
            int i = 0;
            foreach (XmlNode pozycja in pozycje)
            {
                XmlNode ceny = PobierzWezel(pozycja, "OrderedUnitNetPrice");
                if (ceny == null)
                {
                    czypobierac = true;
                    XmlNode kod = PobierzWezel(pozycja, "EAN");
                    if (kod != null)
                    {
                        kodyEAN.Add(kod.InnerText);
                    }
                }
            }
            return czypobierac;
        }

        protected override bool WalidujPoprawnoscPliku(string sciezka, Dictionary<long, Jednostka> jednostki, Dictionary<long, ProduktJednostka> produktyjednostki, out string powod)
        {
            powod = "";
            try
            {
                WczytajZPliku(sciezka, jednostki, produktyjednostki);
            }
            catch (Exception ex)
            {
                powod = ex.Message;
                Log.ErrorFormat($"Zamówienie {sciezka} jest niepoprawne - błąd poniżej");
                Log.Error(ex);
                return false;
            }
            return true;
        }

        protected override void ZamowienieNiepoprawne(string sciezka, string powod)
        {
            string error = $"Zamówienie {sciezka} jest niepoprawne, powód {powod}";
            LogiFormatki.PobierzInstancje.LogujError(new Exception(error));
        }

        protected override void OznaczJakoZaimportowane(ZamowieniaImport zamowienie)
        {
            PrzeniesPlik(zamowienie.Link, katalogArchiwum);
        }

        public int WygenerujIdDlaZamowieniaPozycji(ZamowienieProdukt zamowienie)
        {
            return zamowienie.WygenerujIDObiektuSHA(1);
        }
    }
}