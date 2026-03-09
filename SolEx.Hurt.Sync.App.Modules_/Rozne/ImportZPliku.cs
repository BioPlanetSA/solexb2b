using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ServiceStack.Text;
using SolEx.Hurt.Core.ModelBLL;
using SolEx.Hurt.Core.Sync;
using SolEx.Hurt.Model;
using SolEx.Hurt.Model.AtrybutyKlas;
using SolEx.Hurt.Model.Enums;
using SolEx.Hurt.Model.Interfaces.Sync;
using SolEx.Hurt.Model.Interfaces.SyncModuly;

namespace SolEx.Hurt.Sync.App.Modules_.Rozne
{
    public abstract class ImportZPliku : SyncModul, IPrzedZapisemZamowien, IPoZapisieZamowien
    {
        public override string uwagi => "Pobiera zamówienia z pliku na dysku";

        [FriendlyName("Katalog z zamówieniami")]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        public string KatalogZamowien { get; set; }

        public abstract IEnumerable<string> WyszukajPlikDoWczytania();

        public abstract ZamowieniaImport WczytajZPliku(string sciezka, Dictionary<long, Jednostka> jednostki, Dictionary<long, ProduktJednostka> produktyjednostki);

        protected abstract bool WalidujPoprawnoscPliku(string sciezka, Dictionary<long, Jednostka> jednostki, Dictionary<long, ProduktJednostka> produktyjednostki,
            out string powod);

        protected abstract void ZamowienieNiepoprawne(string sciezka, string powod);

        protected abstract void OznaczJakoZaimportowane(ZamowieniaImport zamowienie);

        public virtual Dictionary<long, Klient> Klienci { get; set; }
        public virtual Dictionary<string, Produkt> ProduktyWgKoduKreskowego { get; set; }

        /// <summary>
        /// Przenosi wybrany plik do wzkazanego podkatalogu docelowego. W katalogu docelowym tworzony jest dodatkowy katalog z datą w której przechowywane sa pliki
        /// </summary>
        /// <param name="plikZrodlowy"></param>
        /// <param name="katalogDocelowy"></param>
        public void PrzeniesPlik(string plikZrodlowy, string katalogDocelowy)
        {
            LogiFormatki.PobierzInstancje.LogujInfo($"Przenoszenie pliku: {plikZrodlowy} do katalogu archiwum: {katalogDocelowy}");

            if (!File.Exists(plikZrodlowy))
            {
                Log.Warn($"Plik: {plikZrodlowy} nie istnieje - nie można go przenieść");
                return;
            }

            string katalogPliku = Path.GetDirectoryName(plikZrodlowy);
            if (string.IsNullOrEmpty(katalogPliku))
            {
                throw new Exception($"Nie można pobrać katalog pliku. Plik źródłowy: {plikZrodlowy}");
            }

            string nazwapliku = Path.GetFileName(plikZrodlowy);
            if (string.IsNullOrEmpty(nazwapliku))
            {
                throw new Exception("Nazwa pliku pusta, nie mogę przenieść");
            }

            //sciezka do katalogu doceloweego
            string sciezka = Path.Combine(katalogPliku, katalogDocelowy);
            //jesli nie ma katalogu docelowego to go tworzymy
            if (!Directory.Exists(sciezka))
            {
                Directory.CreateDirectory(sciezka);
            }

            string katalog = Path.Combine(sciezka, DateTime.Now.ToString("yyyy-MM"));
            if (!Directory.Exists(katalog))
            {
                Directory.CreateDirectory(katalog);
            }

            string plik = Path.Combine(katalog, nazwapliku);

            if (File.Exists(plik))
                File.Delete(plik);

            File.Move(plikZrodlowy, plik);
            Log.Debug($"Plik:{plikZrodlowy} został przeniesiony do: {plik}");
        }

        protected Dictionary<long, FlatCeny> PobierzCenyKlienta(long klient)
        {
            return ApiWywolanie.PobierzCenyKlientow(new HashSet<long>() { klient }).ToDictionary(x => x.ProduktId, x => x);
        }

        public void Przetworz(List<ZamowienieSynchronizacja> zamowieniaDoZapisania, Dictionary<long, Klient> wszyscy, Dictionary<long, Produkt> produktyB2B,
            Dictionary<long, Jednostka> jednostki, Dictionary<long, ProduktJednostka> produktyjednostki, ISyncProvider aktualnyProvider)
        {
            Klienci = wszyscy;

            try
            {
                ProduktyWgKoduKreskowego = produktyB2B.Values.Where(x => !string.IsNullOrEmpty(x.KodKreskowy) && x.Widoczny).ToDictionary(x => x.KodKreskowy, x => x);
            }catch(Exception ex)
            {
                LogiFormatki.PobierzInstancje.LogujInfo("Zdublowane kody kreskowe produktów?");
                throw;
            }

            List<ProduktyKodyDodatkowe> dodatkoweKodyKreskowe = ApiWywolanie.PobierzKodyAlternatywne();

            foreach (KeyValuePair<long, Produkt> produkt in produktyB2B)
            {
                List<ProduktyKodyDodatkowe> dodatkoweKodyProduktu =
                    dodatkoweKodyKreskowe.Where(a => a.ProduktId == produkt.Key).ToList();

                foreach (var produktyKodyDodatkowe in dodatkoweKodyProduktu)
                {
                    if (!ProduktyWgKoduKreskowego.ContainsKey(produktyKodyDodatkowe.Kod))
                    {
                        ProduktyWgKoduKreskowego.Add(produktyKodyDodatkowe.Kod, produkt.Value);
                    }
                }
            }

            if (!Directory.Exists(KatalogZamowien))
            {
                throw new Exception("Katalog nie może być pusty");
            }

            foreach (string sciezka in WyszukajPlikDoWczytania())
            {
                string powod;
                if (WalidujPoprawnoscPliku(sciezka, jednostki, produktyjednostki, out powod))
                {
                    ZamowieniaImport zamowienie = WczytajZPliku(sciezka, jednostki, produktyjednostki);
                    zamowienie.Link = sciezka;
                    Log.Debug($"Zamówienie:{zamowienie.ToJson()}");
                    zamowieniaDoZapisania.Insert(0, zamowienie);    //wsadzanie na poczatek zamowien z EDI - Bio Planet tak chce  #B2B-4847
                    LogiFormatki.PobierzInstancje.LogujInfo($"Odczytno zamówienie nr z b2b: {zamowienie.NumerZPlatformy}, numer: {zamowienie.NumerTymczasowyZamowienia}, id: {zamowienie.Id} z pliku: {zamowienie.Link}.");
                }
                else
                {
                    ZamowienieNiepoprawne(sciezka, powod);
                }
            }
        }

        public void Przetworz(List<ZamowienieSynchronizacja> zapisane, List<ZamowienieSynchronizacja> zamowieniaDoZapisania, Dictionary<long, Klient> wszyscy,
            Dictionary<long, Produkt> produktyB2B, Dictionary<long, Jednostka> jednostki, Dictionary<long, ProduktJednostka> produktyjednostki, ISyncProvider aktualnyProvider)
        {
            for (int i = 0; i < zapisane.Count; i++)
            {
                if (zapisane[i] is ZamowieniaImport zi)
                {
                    LogiFormatki.PobierzInstancje.LogujInfo($"Zamówienie: {zi.NumerTymczasowyZamowienia} / {zi.NumerZPlatformy} wydaje się być z importu z pliku - odznaczamy je jako pobrane.");

                    if (zi.StatusId == StatusImportuZamowieniaDoErp.Zaimportowane) //kometuje zeby jak pochodzi z rozbicia to tez sie przenosil plik do archium  && !zi.PochodziZRozbicia
                    {
                        if (!string.IsNullOrWhiteSpace(zi.Link))
                        {
                            LogiFormatki.PobierzInstancje.LogujInfo($"Zamówienie: {zi.Id} i nazwie: {zi.NumerTymczasowyZamowienia} / {zi.NumerZPlatformy} oznaczam jako zaimportowane");
                            OznaczJakoZaimportowane(zi);
                        }
                        else
                        {
                            LogiFormatki.PobierzInstancje.LogujInfo($"Zamówienie: {zi.Id} i nazwie: {zi.NumerTymczasowyZamowienia} / {zi.NumerZPlatformy} nie zaznaczam jako pobrane dlatego że pole Link jest puste");
                        }
                    }

                    //zamowienie jest z rozbicia - dlaczego nie jest oznaczone jak zaimportowane?

                    zapisane.RemoveAt(i);
                    i--;
                }
            }
        }
    }
}