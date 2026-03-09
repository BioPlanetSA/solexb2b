using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using SolEx.Hurt.Core.BLL;
using SolEx.Hurt.Core.BLL.Interfejsy;
using SolEx.Hurt.Core.ModelBLL;
using SolEx.Hurt.Core.ModelBLL.Interfejsy;
using SolEx.Hurt.Model;
using SolEx.Hurt.Model.Enums;
using SolEx.Hurt.Model.Interfaces;

namespace SolEx.Hurt.Core.Importy.Eksporty
{
    public class PcMarket : GenerowanieDokumentu
    {
        public override bool MoznaGenerowac(DokumentyBll dokument)
        {
            return true;
        }

        public override Licencje? WymaganaLicencja
        {
            get { return Licencje.DokumentyPCMarket5;}
        }

        public override Encoding Kodowanie
        {
            get { return Encoding.GetEncoding(1250); }
        }

        public override string Nazwa
        {
            get { return "PcMarket5"; }
        }

        public override string PobierzNazwePliku(DokumentyBll dokument)
        {
            return NazwaPliku(dokument) + "-pcmarket.txt";
        }

        protected override byte[] PobierzDane(DokumentyBll dokument, IKlient klient)
        {
            StringBuilder sb = new StringBuilder();
            string fraza = "Linia:Nazwa{{{0}}}Kod{{{1}}}Vat{{{2}}}Jm{{{3}}}Asortyment{{{4}}}Sww{{}}PKWiU{{{5}}}Ilosc{{{6}}}Cena{{{7}}}Cena{{b{8}}}Wartosc{{b{9}}}IleWOpak{{{10}}}CenaSp{{}}TowId{{{11}}}\r\n";
            Dictionary<long, int> slownikZaokraglen = SolexBllCalosc.PobierzInstancje.DostepDane.Pobierz<Jednostka>(null).ToDictionary(x => x.Id, x => x.Zaokraglenie);
            foreach (var poz in dokument.PobierzPozycjeDokumentu())
            {
                try
                {
                    var produkt = poz.ProduktBazowy;
                    if (!slownikZaokraglen.TryGetValue(poz.JednostkaMiary, out int zaokraglenie))
                    {
                        zaokraglenie = 2;
                    }
                    if (produkt == null)
                    {
                        produkt = new ProduktBazowy();
                        produkt.Nazwa = poz.NazwaProduktu;
                        produkt.Kod = poz.KodProduktu;
                        produkt.IloscWOpakowaniu = 1;
                    }
                        sb.AppendFormat(fraza, produkt.Nazwa, produkt.KodKreskowy, poz.Vat != 0 ? poz.Vat.ToString("#") : "0", poz.Jednostka, "", produkt.PKWiU, Math.Round(poz.PozycjaDokumentuIlosc.Wartosc, zaokraglenie).ToString(CultureInfo.InvariantCulture).Replace(",","."),
                        poz.PozycjaDokumentuCenaNetto.Wartosc.ToString("0.##").Replace(",", "."), poz.PozycjaDokumentuCenaBrutto.Wartosc.ToString("0.##").Replace(",", "."), poz.PozycjaDokumentuWartoscBrutto.Wartosc.ToString("0.##").Replace(",", "."), produkt.IloscWOpakowaniu.ToString("0.##").Replace(",", "."),poz.ProduktId);
                } catch (Exception e)
                {
                    SolexBllCalosc.PobierzInstancje.Log.Error($"Błąd zapisu PCMARKET pliku. Błąd dla pozycji: {poz.Id}. Zawartość pozycji: {poz.ToCsv()}. Błąd: {e.Message}");
                    throw;
                }
            }

            return Kodowanie.GetBytes(sb.ToString());
        }
    }
}
