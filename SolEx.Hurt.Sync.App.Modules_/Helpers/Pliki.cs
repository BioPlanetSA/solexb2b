using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using FastMember;
using SolEx.Hurt.Model;
using SolEx.Hurt.Model.Enums;
using SolEx.Hurt.Model.Helpers;

using Produkt = SolEx.Hurt.Model.Produkt;
using ServiceStack.Net30.Collections.Concurrent;
using SolEx.Hurt.Core.Sync;
using System.Runtime.CompilerServices;

namespace SolEx.Hurt.Sync.App.Modules_.Helpers
{
    public static class PlikiHelper
    {
        public static int PrzetworzPlikiDlaTypu<T>(string sciezka, List<Plik> plikiLokalne, string Pole, Func<object, Plik, bool> funkcja, List<T> listaDoSprawdzenia, bool tylkoZdjecia = true,
                                                   KomuPrzypisywac komu = KomuPrzypisywac.PierwzemuZnalezionemu, SposobDopasowania dopasowanie = SposobDopasowania.JedenDoJednego, int maksymalnyRozmiarZdjeciaMB = 4)
        {
            int iloscZmian = 0;

            if (string.IsNullOrEmpty(sciezka) || !Directory.Exists(sciezka))
            {
                throw new Exception(string.Format("Brak katalogu: '{0}' skonfigurowanego w module lub katalog nie istnieje. Sprawdź konfiguracje modułu.", sciezka));
            }

            PropertyInfo pole = typeof(T).GetProperty(Pole);
            if (pole == null)
            {
                throw new InvalidOperationException("Coś poszło nie tak nie znaleziono pola: " + Pole);
            }

            string[] plikiWKatalogu = Directory.GetFiles(sciezka, "*", SearchOption.AllDirectories);
            int licznikMinimalny = -100000;
            if (plikiLokalne.Count > 0)
            {
                licznikMinimalny = plikiLokalne.Min(b => b.Id) - 1;
                if (licznikMinimalny >= 0)
                {
                    licznikMinimalny = -100000;
                }
            }

            ConcurrentDictionary<string, List<T>> slownik = new ConcurrentDictionary<string, List<T>>();
            TypeAccessor akcesor = typeof(T).PobierzRefleksja();

            Parallel.ForEach(listaDoSprawdzenia, temp =>
            {
                object wartosc = akcesor[temp, pole.Name];
                if (wartosc == null)
                {
                    return;
                }

                slownik.AddOrUpdate(TextHelper.PobierzInstancje.OczyscNazwePliku(wartosc.ToString()), new List<T> {temp}, (klucz, lista) =>
                {
                    lista.Add(temp);
                    return lista;
                });
            });

            LogiFormatki.PobierzInstancje.LogujInfo($"Zdjęcia dla klientów - wybrano: {plikiWKatalogu.Length} plików. Szukanie odbędzie się po polu klienta: {pole.Name}");

            //to nie moze byc parallel bo w srodku jest sprawdzanie czy zdjecie juz bylo - przy wielu watkach nie ma jak tego sprawdzac dobrze
           foreach(var plik in plikiWKatalogu)
            {
                if (tylkoZdjecia)
                {
                    var rodzajPliku = Core.BLL.SolexBllCalosc.PobierzInstancje.Pliki.CzyPlikToZdjecie(plik) ? RodzajPliku.Zdjecie : RodzajPliku.Zalacznik;
                    if (rodzajPliku != RodzajPliku.Zdjecie)
                    {
                        continue;
                    }
                }

                string nazwapliku = Path.GetFileNameWithoutExtension(plik);
                if (nazwapliku == null)
                {
                    throw new InvalidOperationException("Błąd nie udało się pobrać nazwy pliku");
                }
                string s = nazwapliku.ToLower();
                s = TextHelper.PobierzInstancje.OczyscNazwePliku(s);
                List<T> tmp = new List<T>();

                if (dopasowanie == SposobDopasowania.JedenDoJednego)
                {
                    string klucz = slownik.Keys.FirstOrDefault(x => x.Equals(s, StringComparison.InvariantCultureIgnoreCase));
                    if (klucz != null)
                    {
                        if (komu == KomuPrzypisywac.PierwzemuZnalezionemu)
                        {
                            tmp.Add(slownik[klucz].First());
                        }
                        else
                        {
                            tmp.AddRange(slownik[klucz]);
                        }
                    }
                }

                if (dopasowanie == SposobDopasowania.ZawieraWSobie)
                {
                    var klucz = slownik.Keys.FirstOrDefault(x => x.IndexOf(s, StringComparison.InvariantCultureIgnoreCase) >= 0);
                    if (klucz != null)
                    {
                        if (komu == KomuPrzypisywac.PierwzemuZnalezionemu)
                        {
                            tmp.Add(slownik[klucz].First());
                        }
                        else
                        {
                            tmp.AddRange(slownik[klucz]);
                        }
                    }
                }
              
                //przygotowanie pliku
                FileInfo info = new FileInfo(plik);

                //sprawdzanie rozmiary czy nie przekracza maksymalnego
                try
                {
                    if (info.Length > maksymalnyRozmiarZdjeciaMB * 1024 * 1024) //krytycznie istotne spradzenie rozmiaru dla kazdego pliku, nie tylko zdjeca
                    {
                        LogiFormatki.PobierzInstancje.LogujInfo($"Zdjęcia ma za duży rozmiar ({maksymalnyRozmiarZdjeciaMB}MiB): {plik}. Pomijam plik.");
                        continue;
                    }
                }
                catch (Exception e)
                {
                    LogiFormatki.PobierzInstancje.LogujInfo($"Nie udało się sprawdzić rozmiaru pliku: {plik}. Pomijam plik.");
                    continue;
                }



                DateTime data = info.LastWriteTime.ToUniversalTime().AddMilliseconds(-info.LastWriteTime.ToUniversalTime().Millisecond);
                Plik ptemp = new Plik(data, info.Name, (int)info.Length, info.DirectoryName, --licznikMinimalny);
                ptemp.PoprawNazwaPlikuDlaURL();
                Plik ptempIsteniejacy = plikiLokalne.FirstOrDefault(a => a.CzyTeSamePliki(ptemp));

                if (ptempIsteniejacy == null)
                {
                    plikiLokalne.Add(ptemp);
                    ptempIsteniejacy = ptemp;
                }

                foreach (var c in tmp)
                {                  
                    funkcja(c, ptempIsteniejacy);
                    ++iloscZmian;
                }
            }

            LogiFormatki.PobierzInstancje.LogujInfo($"Wykonane zmian: {iloscZmian}");

            return iloscZmian;
        }

        public static string WygenerujKluczeDopasowania(TypyPolDoDopasowaniaZdjecia poCzymSzukacPlikow, Produkt p)
        {
            string klucz = null;

            switch (poCzymSzukacPlikow)
            {
                case TypyPolDoDopasowaniaZdjecia.KodKreskowy:
                    klucz = p.KodKreskowy == null ? "" : p.KodKreskowy.ToLower();
                    break;
                case TypyPolDoDopasowaniaZdjecia.Kod:
                    klucz = p.Kod==null?null:TextHelper.PobierzInstancje.OczyscNazwePliku(p.Kod.Trim().ToLower()).ToLower();
                    break;
                case TypyPolDoDopasowaniaZdjecia.Rodzina:
                    klucz = p.Rodzina.ToLower(); break;
                case TypyPolDoDopasowaniaZdjecia.PoleTekst1:
                    klucz = !string.IsNullOrEmpty(p.PoleTekst1) ? TextHelper.PobierzInstancje.OczyscNazwePliku(p.PoleTekst1.ToLower()) : null;
                    break;
                case TypyPolDoDopasowaniaZdjecia.Idproduktu:
                    klucz = p.Id.ToString(CultureInfo.InvariantCulture);
                    break;
                case TypyPolDoDopasowaniaZdjecia.PoleTekst2:
                    klucz = !string.IsNullOrEmpty(p.PoleTekst2) ? p.PoleTekst2.ToLower() : null;
                    break;
            }
            return klucz;
        }
    }
}
