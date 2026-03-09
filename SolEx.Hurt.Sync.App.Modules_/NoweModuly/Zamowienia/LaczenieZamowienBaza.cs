using System;
using System.Collections.Generic;
using System.Linq;
using ServiceStack.Text;
using SolEx.Hurt.Core.Helper;
using SolEx.Hurt.Core.ModelBLL;
using SolEx.Hurt.Core.Sync;
using SolEx.Hurt.Model;
using SolEx.Hurt.Model.AtrybutyKlas;
using SolEx.Hurt.Model.Interfaces.Sync;
using SolEx.Hurt.Model.Interfaces.SyncModuly;

namespace SolEx.Hurt.Sync.App.Modules_.NoweModuly.Zamowienia
{
    public abstract class LaczenieZamowienBaza : SyncModul, IModulZamowienia
    {
        [FriendlyName("Łączyć zamówienia nie starsze niż X dni ")]
        [WidoczneListaAdmin(false, false, true, false)]
        public int LiczbaDni { get; set; }

        [FriendlyName("Wykluczone kategorie klienta", FriendlyOpis = "Kategoria klientów B2B dla których NIGDY NIE łączyć zamówień, ma wyższy priorytet")]
        [Niewymagane]
        [PobieranieSlownika(typeof(SlownikKategoriiKlienta))]
        [WidoczneListaAdmin(false,false,true,false)]
        public List<int> WykluczoneKategorieKlienta { get; set; }

        [FriendlyName("Wymagane kategorie klienta", FriendlyOpis = "Kategoria klientów B2B wymagana dla klienta żeby łączyć mu zamówienia")]
        [Niewymagane]
        [PobieranieSlownika(typeof(SlownikKategoriiKlienta))]
        [WidoczneListaAdmin(false, false, true, false)]
        public List<int> WymaganeKategorieKlienta { get; set; }

        [FriendlyName("Nie łącz zamówień pochodzących z rozbicia")]
        [WidoczneListaAdmin(false, false, true, false)]
        public bool NieLaczZamowieniaRozbite { get; set; }

        [FriendlyName("Tekst dodawany do uwag zamówienia połączonego",FriendlyOpis = "Po tym tekscie bedzie podany ewentualnie nr łączonego zamówienia. Opcjonalnie można wpisać, {NOWE_UWAGI} - by po nr zamówienia dopisać uwagi z nowego zamówienia. Dopisek bedzie wtedy wyglądał np tak: ŁĄCZONE B2B 11/1/2 {NOWE_UWAGI} reszta uwag.")]
        [WidoczneListaAdmin(false, false, true, false)]
        public string TextDoUwag { get; set; }

        [FriendlyName("Dodawaj numer łączonego zamówienia B2B do uwag zamówienia.")]
        [WidoczneListaAdmin(false, false, true, false)]
        public bool NumerZamDoUwag { get; set; }

        protected LaczenieZamowienBaza()
        {
            LiczbaDni = 7;
            NieLaczZamowieniaRozbite = false;
            TextDoUwag = "ŁĄCZONE";
            NumerZamDoUwag = true;
        }

        public abstract void PrzetworzZamowienie(ref ZamowienieSynchronizacja zamowienieWejsciowe, ISyncProvider provider);

        public abstract object PrzetworzZamowieniePoPolaczeniu(object dokument, string uwagi, ZamowienieSynchronizacja zamowienie);

        public void Przetworz(ZamowienieSynchronizacja zamowienieWejsciowe, ref List<ZamowienieSynchronizacja> wszystkie, ISyncProvider provider, Dictionary<long, Jednostka> jednostki,Dictionary<long, ProduktJednostka> laczniki, 
            Dictionary<long, Produkt> produktyB2B, List<Cecha> cechyB2B, List<ProduktCecha> lacznikiCech)
        {
            Log.Debug("Uruchamiam moduł");
            if (zamowienieWejsciowe.PochodziZRozbicia && NieLaczZamowieniaRozbite)
            {
                Log.InfoFormat($"Zamówienie:{zamowienieWejsciowe.NumerZRozbicia} pochodzi z rozbicia, a wybrana jest opcja żeby nie łaczyć zamówień rozbitych.");
                return;
            }

            if (!SprawdzKlientaCzyMoznaLaczyc(zamowienieWejsciowe.KlientId)) return;
            for (int index = 0; index < wszystkie.Count; index++)
            {
                ZamowienieSynchronizacja zamowienieSynchronizacja = wszystkie[index];
                PrzetworzZamowienie(ref zamowienieSynchronizacja, provider);
            }
            Log.Debug("Koniecmoduł");
        }


        /// <summary>
        /// Sprawdza czy dla klienta można łączyć zamówienia
        /// </summary>
        /// <param name="klient"></param>
        /// <returns></returns>
        public bool SprawdzKlientaCzyMoznaLaczyc(long klientId)
        {
            var laczniki = SyncManager.PobierzInstancje.PobierzLacznikiKategoriiKlientow();
            var kategoriaKlienta = laczniki.Where(x => x.KlientId == klientId).Select(x=>x.KategoriaKlientaId).ToList();
            //sprawdzamy czy klient posiada kategorie wykluczające
            if (WykluczoneKategorieKlienta != null && WykluczoneKategorieKlienta.Any() && laczniki.Any())
            {
                var czescWspolna = WykluczoneKategorieKlienta.Intersect(kategoriaKlienta).ToList();
                if (czescWspolna.Any())
                {
                    Log.InfoFormat($"Klient:{klientId} posiada kategorie:{czescWspolna.ToJson()} które wykluczają łaczenie. Przerywam Łączenie.");
                    return false;
                }
            }
            //sprawdzamy czy klient ma kategorie wymagane
            if (WymaganeKategorieKlienta == null || !WymaganeKategorieKlienta.Any()) return true;

            var czescWspolnaWymaganych = WymaganeKategorieKlienta.Intersect(kategoriaKlienta).ToList();

            if (czescWspolnaWymaganych.Any()) return true;

            Log.InfoFormat($"Klient:{klientId} nie posiada wymaganej kategorie:{WymaganeKategorieKlienta.ToJson()}. Przerywam Łączenie.");
            return false;
        }
        /// <summary>
        /// Dopisuje nowe uwagi do starych przy zachowabiu maxymalnej liczby znaków.
        /// </summary>
        /// <param name="uwagiZam"></param>
        /// <param name="noweUwagi"></param>
        /// <returns></returns>
        public string DopiszNoweUwagi(string uwagiZam, string noweUwagi)
        {
            string wynik = uwagiZam;
            string frazaDoZastapienia = "{NOWE_UWAGI}";
            if (uwagiZam.IndexOf(frazaDoZastapienia, StringComparison.InvariantCultureIgnoreCase) > -1)
            {
                int max = 370;
                int liczbaZnakowObecnie = uwagiZam.Length - frazaDoZastapienia.Length;
                int doDopisania = max - liczbaZnakowObecnie -3;
                if (noweUwagi.Length > doDopisania)
                {
                    noweUwagi = noweUwagi.Substring(0, doDopisania)+"...";
                }
                wynik = uwagiZam.Replace(frazaDoZastapienia, noweUwagi,StringComparison.InvariantCultureIgnoreCase);
            }
            return wynik;
        }
        
        /// <summary>
        /// Dopisuje do uwag info o polączeniu
        /// </summary>
        /// <param name="uwagiStareZam"></param>
        /// <param name="nrZamowienia"></param>
        /// <param name="rozbite"></param>
        /// <param name="uwagiNoweZam"></param>
        /// <returns>Nowe uwagi do zamówienia</returns>
        public string DopiszWyrazenieDoUwag(string uwagiStareZam, string nrZamowienia, bool rozbite, string uwagiNoweZam)
        {
            string dopisek = TextDoUwag;
            if (NumerZamDoUwag) { dopisek += $" {nrZamowienia};";}
            string wynik= uwagiStareZam; 

            //jesli było juz łaczenie (w uwagach jest TextDoUwag) to dopisujemy tylko kolejny nr Zamówienia zawsze
            if (uwagiStareZam.IndexOf(TextDoUwag, StringComparison.InvariantCultureIgnoreCase) > -1)
            {
                //Jesli dopisujemy nr zamówoenia do uwag do dajemy replace 
                if (NumerZamDoUwag){wynik = uwagiStareZam.Replace(TextDoUwag, dopisek);}
                return DopiszNoweUwagi(wynik,uwagiNoweZam);
            }
            //jesli zamówienie nie było rozbijane
            if (!rozbite)
            {
                wynik = $"{dopisek} {uwagiStareZam}";
                return DopiszNoweUwagi(wynik, uwagiNoweZam);
            }
            //jesli zamówienie było rozbitę 
            string poczatek = PobierzWspolnyPoczatek(uwagiStareZam, uwagiNoweZam);
            //jesli nie ma wsólnego początku to znaczy że powód rozbicia jest na końcu czyli łaczymy normalnie 
            if (string.IsNullOrEmpty(poczatek))
            {
                wynik = $"{dopisek} {uwagiStareZam}";
                return DopiszNoweUwagi(wynik, uwagiNoweZam);
            }
            dopisek = $"{poczatek} {dopisek}";
            wynik = uwagiStareZam.Replace(poczatek, dopisek);

            return DopiszNoweUwagi(wynik, uwagiNoweZam);
        }
        /// <summary>
        /// Sprawdzamy czy oba texty rozpoczynają się tym samym wyrazem
        /// </summary>
        /// <param name="text1"></param>
        /// <param name="text2"></param>
        /// <returns>Wspólny początek jesli jest, albo null jesli nie ma</returns>
        public string PobierzWspolnyPoczatek(string text1, string text2)
        {
            //trimujemy texty żeby nie było niepotrzebnych spacji
            text1 = text1?.Trim();
            text2 = text2?.Trim();
            if (string.IsNullOrEmpty(text1))
            {
                return null;
            }
            if (string.IsNullOrEmpty(text2))
            {
                return null;
            }

            //pobieramy pierwszy wyraz z text1
            string pocztek = text1.Split(' ')[0];
            //sprawdzamy czy drugi text zaczyna się 
            if (text2.StartsWith(pocztek,StringComparison.InvariantCultureIgnoreCase))
            {
                return pocztek;
            }
            return null;
        }
    }
}
