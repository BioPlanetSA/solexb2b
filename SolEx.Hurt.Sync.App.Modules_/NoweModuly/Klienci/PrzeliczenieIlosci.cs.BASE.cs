using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using SolEx.Hurt.Model;
using SolEx.Hurt.Model.AtrybutyKlas;
using SolEx.Hurt.Model.Enums;
using SolEx.Hurt.Model.Helpers;
using SolEx.Hurt.Model.Interfaces.SyncModuly;
using SolEx.Hurt.Model.Web;
using SolEx.Hurt.Sync.Core;

namespace SolEx.Hurt.Sync.App.Modules_.NoweModuly.Klienci
{
    public class PrzeliczenieIlosci : SyncModul, IModulKlienci
    {
        public override string uwagi
        {
            get { return "Przelicza kupowane ilości przez klientów, jeśli wybrano konkretną datę należy wprowadzić w formacie dd.MM.yyyy"; }
        }
        [FriendlyName("Typ dokumentu")]
        public RodzajDokumentu RodzajDokumentu { get; set; }
        [FriendlyName("Od kiedy uwzględniać dokumenty")]
        public DataOdKiedyLiczyc OdKiedyLiczyc { get; set; }
        [FriendlyName("Parametry od kiedy")]
        public string DataOdKiedy { get; set; }

        [FriendlyName("Do kiedy uwzględniać dokumenty")]
        public DataOdKiedyLiczyc DoKiedyLiczyc { get; set; }
        [FriendlyName("Parametry do kiedy")]
        public string DataDoKiedy { get; set; }

        [FriendlyName("Ścieżka do pliku CSV z dodatkowymi ilościami do sumowania")]
        public string SciezkaDoCSVZDodatkowymiIlosciami { get; set; }


        public void Przetworz(ref Dictionary<int, klienci> listaWejsciowa, Dictionary<int, produkty> produktyB2B, ref List<KupowaneIlosci> ilosci, ref List<Adresy> adresyWErp, List<kategorie_klientow> kategorie, List<klienci_kategorie> laczniki)
        {
            DateTime odkiedy = DateTimeHelper.PobierzInstancje.WyliczDate(OdKiedyLiczyc, DataOdKiedy);
            DateTime dokiedy = DateTimeHelper.PobierzInstancje.WyliczDate(DoKiedyLiczyc, DataDoKiedy);
            Dictionary<string, KupowaneIlosci> ilosciKlientow = ilosci.ZbudojSlownikZKluczemPropertisowym();
            if (File.Exists(SciezkaDoCSVZDodatkowymiIlosciami))
            {
             
                string[,] plikcsv = new SolEx.Hurt.Helpers.CSVHelperExt().OdczytajCSV(SciezkaDoCSVZDodatkowymiIlosciami);
                
                for(int i = 1; i < plikcsv.GetLength(0); i++)
                {
                    string symbolKlienta = plikcsv[i, 0];
                    klienci  klient = listaWejsciowa.Values.FirstOrDefault(a => a.symbol == symbolKlienta);

                    if (klient == null)
                    {
                        LogiFormatki.PobierzInstancje.LogujError(new Exception(string.Format("W pliku CSV {0} nie znaleziono klienta o symbolu {1}", SciezkaDoCSVZDodatkowymiIlosciami, symbolKlienta)));
                        continue;
                    }
                    
                    string symbol = plikcsv[i, 1];
                    string wersja = plikcsv[i, 2];
                    string iloscProduktu = plikcsv[i, 3];
                    string rodzina = string.Format("{0}-{1}", symbol, wersja);
               //     string symbolProduktu = string.Format("{0}-{1}-{2}-", symbol, symbolKlienta, wersja).ToLower();
                   // LogiFormatki.PobierzInstancje.LogujDebug("smmashsymbol: " + rodzina);
                    produkty produkt = produktyB2B.Values.FirstOrDefault(a =>a.pole_liczba2==klient.klient_id && a.rodzina.ToLower()==rodzina.ToLower() );
                    //produkt_id	nazwa	kod	widoczny	stan_maksymalny	stan_min	ilosc_w_opakowaniu	kod_kreskowy	vat	PKWiU	opis	opis_krotki	opis2	opis_krotki2	opis3	opis_krotki3	opis4	opis_krotki4	opis5	opis_krotki5	obrazek_id	pole_tekst1	pole_tekst2	pole_tekst3	pole_liczba1	pole_liczba2	pole_liczba3	rodzina	ojciec	www	waga	pole_tekst4	pole_tekst5	pole_liczba4	pole_liczba5	kolumna_liczba1	kolumna_liczba2	kolumna_liczba3	kolumna_liczba4	kolumna_liczba5	kolumna_tekst1	kolumna_tekst2	kolumna_tekst3	kolumna_tekst4	kolumna_tekst5	ilosc_minimalna	przedstawiciel_id	dostepny_dla_wszystkich	ilosc_w_opakowaniu_tryb	popup_tekst	popup_komunikat	dostawa	wymagane_OZ	Typ	DataDodania	StatusUkryty	ParametryDostosowywania	VatOdwrotneObciazenie	Objetosc
//1907230021	VISUAL4	SHC2-NO-FEAR-GYM-GELNHAUS-VISUAL4-XXL	1	0.00	0.00	1.00	NULL	23.00	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	no-fear-gym-gelnhaus-spodenki-mma-projekty-graficzne-visual4	VISUAL4	NULL	4185.00	NULL	NULL	spodenki-mma-VISUAL4	0	NULL	0.360	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	0.00	NULL	0	0	NULL	NULL	NULL	0	Produkt	2014-05-14 15:19:56.470	1	Input:Nadruk imienia lub innej nazwy:+15 PLN:+4 EUR:;	0	0.0000
                    if (produkt == null)
                    {
                        LogiFormatki.PobierzInstancje.LogujError(new Exception(string.Format("Dla klienta o symbolu {0} nie znaleziono produktu o symbolu {1}",symbolKlienta, rodzina)));
                        continue;
                    }

                    decimal ilosc = 0;
                    if (TextHelper.SprobojSparsowac(iloscProduktu, out ilosc))
                    {
                        KupowaneIlosci kupowaneIlosci = new KupowaneIlosci
                        {
                            Do = dokiedy,
                            KlientId = klient.klient_id,
                            Od = odkiedy,
                            RodzajDokumentu = RodzajDokumentu,
                            DodatkowaIlosc = ilosc
                        };

                        string klucz = kupowaneIlosci.ZbudujKlucz();

                        if (!ilosciKlientow.ContainsKey(klucz))
                        {
                            ilosciKlientow.Add(klucz, kupowaneIlosci);
                        }
                        else
                        {
                            LogiFormatki.PobierzInstancje.LogujError(new Exception(string.Format("Zdublowany produkt o symbolu {0} u klienta {1} na pozycji {2}. Wybrano większą wartość.", rodzina, klient.nazwa, i)));

                            if (ilosciKlientow[klucz].DodatkowaIlosc < kupowaneIlosci.DodatkowaIlosc)
                                ilosciKlientow[klucz] = kupowaneIlosci;
                        }

                    }
                }

            }
            else throw new Exception("Nie znaleziono pliku " + SciezkaDoCSVZDodatkowymiIlosciami);

            ilosci.Clear();

            foreach (var ilosc in ilosciKlientow)
            {
                ilosci.Add(ilosc.Value);
            }
        }

    }
}
