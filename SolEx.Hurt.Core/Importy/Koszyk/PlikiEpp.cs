using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using SolEx.Hurt.Core.BLL;
using SolEx.Hurt.Core.Importy.Model;
using SolEx.Hurt.Model;
using SolEx.Hurt.Model.Web;

namespace SolEx.Hurt.Core.Importy.Koszyk
{
    public class PlikiEpp : ImportBaza
    {
        const string KoniecSekcji = "\r\n[";
        private const string Zawartosc = "[ZAWARTOSC]";
        public override string LadnaNazwa
        {
            get { return "Import w formacie epp (Subiekt)"; }
        }

        public override List<string> Rozszerzenia
        {
            get { return new List<string> { "epp" }; }
        }
        
        public override List<PozycjaKoszykaImportowana> Przetworz(string dane, out List<Komunikat> bledy, Stream stumien)
        {
            List<PozycjaKoszykaImportowana> wynik = new List<PozycjaKoszykaImportowana>();
            bledy = new List<Komunikat>();
            Dictionary<string, Tuple<string,string>> resulttmp = new Dictionary<string, Tuple<string,string>>();
            if (dane.StartsWith("[INFO]"))//to plik edi
            {
                if (dane.Contains(Zawartosc))
                {
                    int idxs = dane.IndexOf(Zawartosc, StringComparison.Ordinal);
                    int idxe = dane.IndexOf(KoniecSekcji, idxs + Zawartosc.Length, StringComparison.Ordinal);

                    string produktyString = dane.Substring(idxs + Zawartosc.Length, idxe - idxs - Zawartosc.Length).Trim();
                    string[] produkty = produktyString.Split(new [] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
                    foreach (string t in produkty)
                    {
                        string[] wiersz = t.Split(new [] {","}, StringSplitOptions.None);
                        string symbol = wiersz[2].Trim('\"');
                        string ilosc = wiersz[10].Replace(".", ",");
                        if (!resulttmp.ContainsKey(symbol))
                        {
                            resulttmp.Add(symbol,new Tuple<string,string>( ilosc,t)); //ddanie pozycji, narazie wg symbolu, kod kreskowy będzie za chwilę
                        }
                    }
                    if (resulttmp.Count > 0)//czy jest sens szukać kodów kreskowych
                    {
                        const string zawartoscPRodukty = "[NAGLOWEK]\r\n\"TOWARY\"\r\n\r\n[ZAWARTOSC]";
                        idxs = dane.IndexOf(zawartoscPRodukty, StringComparison.Ordinal);
                        idxe = dane.IndexOf(KoniecSekcji, idxs + zawartoscPRodukty.Length, StringComparison.Ordinal);
                        if (idxe > 0)
                        {
                            produktyString = dane.Substring(idxs + zawartoscPRodukty.Length,  idxe - idxs - zawartoscPRodukty.Length);
                        }
                        else
                        {
                            produktyString = dane.Substring(idxs + zawartoscPRodukty.Length);
                        }
                        produktyString = produktyString.Trim();
                        produkty = produktyString.Split(new [] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
                        foreach (string t in produkty)
                        {
                            try
                            {
                                string[] wiersz = t.Split(new[] {","}, StringSplitOptions.None);
                                string symbol = wiersz[1].Trim('\"');
                                string symbolUDostawcy = wiersz[2].Trim('\"');
                                string kodKReskowy = wiersz[3].Trim('\"');
                                Jednostka jednostki = SolexBllCalosc.PobierzInstancje.JednostkaProduktuBll.PobierzJednostki(SolexBllCalosc.PobierzInstancje.Konfiguracja.JezykIDPolski).Values.FirstOrDefault(x => x.Nazwa == wiersz[9].Trim('\"'));

                                if (resulttmp.ContainsKey(symbol))
                                {
                                    ZnajdzProdukt(string.IsNullOrEmpty(kodKReskowy) ? symbol : kodKReskowy, resulttmp[symbol].Item1, resulttmp[symbol].Item2, wynik, bledy, jednostki);

                                    if (ZaDuzoElementow)
                                    {
                                        break;
                                    }
                                }
                                else
                                {
                                    bledy.Add(new Komunikat("Plik epp był nieudolnie modyfikowany ręcznie.",KomunikatRodzaj.danger));
                                }
                            }
                            catch (Exception)
                            {
                            }
                        }
                    }
                }
            }
            return wynik;
        }
    }
}
