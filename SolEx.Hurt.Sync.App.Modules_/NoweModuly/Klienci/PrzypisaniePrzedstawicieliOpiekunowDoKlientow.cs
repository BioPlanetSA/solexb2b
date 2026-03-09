using SolEx.Hurt.Core.Helper;
using SolEx.Hurt.Core.ModelBLL;
using SolEx.Hurt.Model;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ServiceStack.Common.Extensions;
using ServiceStack.Text;
using SolEx.Hurt.Core.BLL.Interfejsy;
using SolEx.Hurt.Core.Sync;
using SolEx.Hurt.Model.AtrybutyKlas;
using SolEx.Hurt.Model.Enums;
using SolEx.Hurt.Model.Interfaces;
using SolEx.Hurt.Model.Interfaces.Sync;

using Adres = SolEx.Hurt.Model.Adres;
using Klient = SolEx.Hurt.Model.Klient;

namespace SolEx.Hurt.Sync.App.Modules_.NoweModuly.Klienci
{
    public enum TypyPracownikow
    {
        Przedstawiciel = 0,
        Opiekun = 1,
        KontoNadrzedne= 2,
        DrugiOpiekun = 3
    }

    public enum SposobyPolaczenia
    {
        Email = 2,
        Symbol = 3,
        Nazwa = 7,
        [FriendlyName("Tylko Dwa Pierwsze Słowa np.: Imie Nazwisko")]
        TylkoDwaPierwszeSlowa_npImieNazwisko = 8,
        [FriendlyName("Id klienta z Erp-a")]
        IDklientaZERP=9
    }
    [FriendlyName("Przypisanie przedstawicieli i opiekunów do Klientów", FriendlyOpis = "Moduł, który automatycznie przydziela klientów do określonych przedstawicieli/opiekunów na podstawie wybranego sposobu łączenia")]
    public class PrzypisaniePrzedstawicieliOpiekunowDoKlientow : SyncModul, Model.Interfaces.SyncModuly.IModulKlienci, ITestowalna
    {       
        public override string uwagi
        {
            get { return ""; }
        }

        [FriendlyName("Typ pracownika, który będzie przypisany do klienta")]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        public TypyPracownikow TypPracownika { get; set; }

        [FriendlyName("Sposób dopasowania cechy klienta do przedstawiciela ")]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        public SposobyPolaczenia SposobPolaczenia { get; set; }

        [Niewymagane]
        [PobieranieSlownika(typeof(SlownikGrupyKategoriiKlienta))]
        [FriendlyName("Grupa kategorii klientów")]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        public string Grupa { get; set; }

        public PrzypisaniePrzedstawicieliOpiekunowDoKlientow() 
        {
            TypPracownika = TypyPracownikow.Przedstawiciel;
            SposobPolaczenia = SposobyPolaczenia.Email;
        }

        public void Przetworz(ref Dictionary<long, Klient> listaWejsciowa, Dictionary<long, Produkt> produktyB2B, ref Dictionary<Adres, KlientAdres> adresyWErp, List<KategoriaKlienta> kategorie, List<KlientKategoriaKlienta> laczniki, ref List<Sklep> sklepy, ref List<SklepKategoriaSklepu> sklpeylaczniki, ref List<KategoriaSklepu> sklepyKategorie, ref List<Kraje> kraje, ref List<Region> regiony, ref List<Magazyn> magazyny, ISyncProvider provider)
        {
            if (string.IsNullOrEmpty(Grupa))
            {
               throw new Exception("Brak grupy kategorii klientów.");
            }

            List<Klient> przedstawiciele = ApiWywolanie.PobierzKlientow().Values.ToList();
            if (TypPracownika != TypyPracownikow.KontoNadrzedne)
            {
                przedstawiciele = przedstawiciele.Where(x => x.Role.Contains(RoleType.Pracownik)).ToList();
            }
            PrzetworzMain(ref listaWejsciowa, przedstawiciele, kategorie,laczniki);
        }

        private string WyciagnijNazweKlienta(string nazwa)
        {
            string nowaNazwa = "";
            string[] czlony = nazwa.ToLower().Split(new [] { " " }, StringSplitOptions.RemoveEmptyEntries);
            if (czlony.Length >= 2)
            {
                nowaNazwa = string.Format("{0} {1}", czlony[0], czlony[1]);
            }
            return nowaNazwa;
        }

        public void PrzetworzMain(ref Dictionary<long, Klient> listaWejsciowa, List<Klient> przedstawiciele, List<KategoriaKlienta> kategorie, List<KlientKategoriaKlienta> laczniki)
        {
            if (string.IsNullOrEmpty(Grupa))
            {
                throw new Exception("Brak grupy kategorii w konfiguarcji modułu!");
            }

            Dictionary<int, KategoriaKlienta> kategorieKlientowWyfiltrowane = KategorieKlientowWyszukiwanie.PobierzInstancje.FiltrujKategorieWgGrupyLubCechy(kategorie, Grupa, true);
            Dictionary<long, List<KlientKategoriaKlienta>> filtrowaneLacznikiWgKlientow = laczniki.Where(x => kategorieKlientowWyfiltrowane.ContainsKey(x.KategoriaKlientaId)).GroupBy(x => x.KlientId).ToDictionary(x => x.Key, x => x.ToList());

            LogiFormatki.PobierzInstancje.LogujInfo($"Dopasowano {kategorieKlientowWyfiltrowane.Count} kategorii klientów wg. grupy: {Grupa}. Klientów z tymi kategoriami: {filtrowaneLacznikiWgKlientow.Count}");

            if (filtrowaneLacznikiWgKlientow.IsEmpty() || kategorieKlientowWyfiltrowane.IsEmpty())
            {
               LogiFormatki.PobierzInstancje.LogujInfo("Brak klientów do zmiany - żaden nie ma wymaganej kategorii. Pomijam moduł.");
            }

            int ileZmian = 0;
            List<Klient> klienciDoIteracji = listaWejsciowa.WhereKeyIsIn(filtrowaneLacznikiWgKlientow.Keys).ToList();

            Parallel.ForEach(klienciDoIteracji, k =>
            {
                List<KlientKategoriaKlienta> kategorieLaczniki = filtrowaneLacznikiWgKlientow[k.Id];

                if (kategorieLaczniki.Count > 1)
                {
                    string nazwaKategorii = kategorie.Where(x => kategorieLaczniki.Any(z => z.KategoriaKlientaId == x.Id)).Select(x => x.Nazwa).Join(",");
                    LogiFormatki.PobierzInstancje.LogujInfo($"Nie można ustawić pracownika klientow id: {k.Id}, email: {k.Email}. Klient musi mieć tylko jedną kategorie dla grupy: {Grupa}. Kategorie klienta id: {kategorieLaczniki.Select(x=>x.KategoriaKlientaId).ToCsv()} [{nazwaKategorii}]. Klient pomijany.");
                    return;
                }

                KategoriaKlienta kategoriaDoDopasowaniaPracownika = kategorieKlientowWyfiltrowane[kategorieLaczniki[0].KategoriaKlientaId];

                Klient pracownik = null;
                switch (SposobPolaczenia)
                {
                    case SposobyPolaczenia.Nazwa:
                    {
                        pracownik = przedstawiciele.FirstOrDefault(a => a.Nazwa.Equals(kategoriaDoDopasowaniaPracownika.Nazwa, StringComparison.InvariantCultureIgnoreCase));
                    }
                        break;

                    case SposobyPolaczenia.Email:
                    {
                        pracownik = przedstawiciele.FirstOrDefault(a => !string.IsNullOrEmpty(a.Email) && a.Email.Trim().Equals(kategoriaDoDopasowaniaPracownika.Nazwa, StringComparison.InvariantCultureIgnoreCase));
                    }
                        break;

                    case SposobyPolaczenia.Symbol:
                    {
                        pracownik = przedstawiciele.FirstOrDefault(a => a.Symbol.Trim().Equals(kategoriaDoDopasowaniaPracownika.Nazwa, StringComparison.InvariantCultureIgnoreCase));
                    }
                        break;

                    case SposobyPolaczenia.TylkoDwaPierwszeSlowa_npImieNazwisko:
                    {
                        string szukanaNazwa = WyciagnijNazweKlienta(kategoriaDoDopasowaniaPracownika.Nazwa);
                        if (!string.IsNullOrEmpty(szukanaNazwa))
                        {
                            pracownik = przedstawiciele.FirstOrDefault(a => WyciagnijNazweKlienta(a.Nazwa) == szukanaNazwa);
                        }
                    }
                        break;

                    case SposobyPolaczenia.IDklientaZERP:
                    {
                        int id;
                        if (int.TryParse(kategoriaDoDopasowaniaPracownika.Nazwa, out id))
                        {
                            pracownik = przedstawiciele.FirstOrDefault(a => a.Id == id);
                        }
                    }
                        break;
                }

                if (pracownik == null)
                {
                    LogiFormatki.PobierzInstancje.LogujInfo(
                        $"Dla kategorii o nazwie: {kategoriaDoDopasowaniaPracownika.Nazwa} nie można dopasować pracownika wg. sposobu dopasowania: {SposobPolaczenia.ToString()}. Czyżby taki pracownik nie istnieje? Klient id: {k.Id}, email: {k.Email} jest pomijany.");
                    return;
                }

                    switch (TypPracownika)
                    {
                        case TypyPracownikow.Opiekun:
                        {
                            k.OpiekunId = pracownik.Id;
                        }
                            break;

                        case TypyPracownikow.Przedstawiciel:
                        {
                            k.PrzedstawicielId = pracownik.Id;
                        }
                            break;
                        case TypyPracownikow.KontoNadrzedne:
                        {
                            k.KlientNadrzednyId = pracownik.Id;
                        }
                            break;

                        case TypyPracownikow.DrugiOpiekun:
                        {
                            k.DrugiOpiekunId = pracownik.Id;
                        }
                            break;
                    }
                    ++ileZmian;
            });

            if (ileZmian>0)
            {
                Log.Debug($"Wykonane {ileZmian} zmiany.");
            }
        }

        public List<string> TestPoprawnosci()
        {
            List<string> listaBledow = new List<string>();
            if (string.IsNullOrEmpty(Grupa))
            {
                listaBledow.Add("Pola Grupa nie została wypełniona !");
            }
            return listaBledow;
        }
    }
}
