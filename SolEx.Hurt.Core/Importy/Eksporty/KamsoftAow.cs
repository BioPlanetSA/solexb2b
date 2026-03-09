using System;
using System.Globalization;
using System.Text;
using SolEx.Hurt.Core.BLL.Interfejsy;
using SolEx.Hurt.Core.ModelBLL;
using SolEx.Hurt.Model.Enums;
using SolEx.Hurt.Model.Interfaces;

namespace SolEx.Hurt.Core.Importy.Eksporty
{
    public class KamsoftAow : GenerowanieDokumentu
    {
        public override bool MoznaGenerowac(DokumentyBll dokument)
        {
            return true;
        }

        public override Licencje? WymaganaLicencja => Licencje.DokumentyKamsoftAow;

        public override Encoding Kodowanie => Encoding.UTF8;

        public override string Nazwa => "Kamsoft AOW";

        public override string PobierzNazwePliku(DokumentyBll dokument)
        {
            return NazwaPliku(dokument) + "-kamsoft.kt0";
        }

        protected override byte[] PobierzDane(DokumentyBll dokument, IKlient klient)
        {
            string naglowekDok = @"-IdentDostawcy------------IdentOdbiorcy--SymbolDokumentu-----DataWystawienia-DataSprzedazy-TerminPlatnosci-StandardPL-";

            StringBuilder plik = new StringBuilder();
            plik.AppendLine(naglowekDok);
            string termin = dokument.DokumentTerminRealizacji?.ToString("yy.MM.dd") ?? "";
            string platnosc = dokument.TerminPlatnosci?.ToString("yy.MM.dd") ?? "";
            plik.AppendLine($" {"",24} {dokument.DokumentOdbiorcaId,-14} {dokument.NazwaDokumentu,-19} {dokument.DataUtworzenia.ToString("yy.MM.dd"),-15} {termin,-13} {platnosc,-15} {2,-10} ");

            string naglowekPoz = @"-KSBLOZ--------NazwaTowaru------------------------------Ilosc-----CenaTransBU-CenaTrans---BCenaTransBU-BCenaTrans---RCeny-VAT-CenaDetal----DataWaznosci-Seria---------SymbolSWW------";
            plik.AppendLine(naglowekPoz);
            
            int liczbaPoz = 0;

            foreach (DokumentyPozycje poz in dokument.PobierzPozycjeDokumentu())
            {
                string nazwaProd = poz.NazwaProduktu.Trim().PobierzMaksZnakow(40);

                plik.AppendLine($" {"",-13} {nazwaProd,-40} {Math.Round(poz.Ilosc, base.Zaokraglenia[poz.JednostkaMiary]).ToString(CultureInfo.InvariantCulture).Replace(",","."),9} {poz.CenaNetto.ToString("0.##").Replace(",", "."),11} {poz.CenaNettoPoRabacie.ToString("0.##").Replace(",", "."),11} {poz.CenaBrutto.ToString("0.##").Replace(",", "."),12} {poz.CenaBruttoPoRabacie.ToString("0.##").Replace(",", "."),12} {"B",5} {poz.Vat.ToString("0.##").Replace(",", "."),3} {poz.CenaBrutto.ToString("0.##").Replace(",", "."),11}  {"",12} {"",13} {poz.ProduktBazowy?.PKWiU,-14} ");
                liczbaPoz++;
            }
            plik.AppendLine("......................................................................");
            plik.AppendLine($".-----KONIEC-DOKUMENTU-----------LICZBA-WYDRUKOWANYCH-POZYCJI-{liczbaPoz}");
            return Kodowanie.GetBytes(plik.ToString());
        }
        
    }
}
