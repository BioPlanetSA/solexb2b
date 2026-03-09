using System;
using System.Globalization;
using System.Text;
using SolEx.Hurt.Core.BLL;
using SolEx.Hurt.Core.BLL.Interfejsy;
using SolEx.Hurt.Core.ModelBLL;
using SolEx.Hurt.Core.ModelBLL.Interfejsy;
using SolEx.Hurt.Model.Enums;
using SolEx.Hurt.Model.Interfaces;

namespace SolEx.Hurt.Core.Importy.Eksporty
{
    public class Kucharscy : GenerowanieDokumentu
    {
        public override bool MoznaGenerowac(DokumentyBll dokument)
        {
            return true;
        }

        public override Licencje? WymaganaLicencja
        {
            get { return Licencje.DokumentyKucharscy;}
        }

        public override Encoding Kodowanie
        {
            get { return Encoding.UTF8; }
        }

        public override string Nazwa
        {
            get { return "Kucharscy"; }
        }

        public override string PobierzNazwePliku(DokumentyBll dokument)
        {
            return NazwaPliku(dokument) + "-kucharscy.txt";
        }

        protected override byte[] PobierzDane(DokumentyBll dokument, IKlient klient)
        {
            StringBuilder sb = new StringBuilder();
            foreach (var poz in dokument.PobierzPozycjeDokumentu())
            {
                string nazwaProduktu = poz.NazwaProduktu;

                if (nazwaProduktu.Length > 100)
                {
                    nazwaProduktu = nazwaProduktu.Substring(0, 100);
                }
             
                //Kod kreskowy;Ilość;Cena zakupu;Nazwa towaru;Stawka VAT;PKWiU;Jednostka; Nr dowodu dostawy;Cena sprzedaży
                sb.AppendFormat("{0};{1};{2};{3};{4};{5};{6};{7};\r\n", poz.ProduktBazowy?.KodKreskowy,
                    Math.Round(poz.Ilosc, base.Zaokraglenia[poz.JednostkaMiary]).ToString(CultureInfo.InvariantCulture).Replace(".", ","),
                    poz.PozycjaDokumentuCenaNetto.Wartosc.ToString("0.##").Replace(".", ","), nazwaProduktu,
                (int)poz.Vat, poz.ProduktBazowy?.PKWiU, poz.Jednostka, dokument.NazwaDokumentu);
            }
          
            return Kodowanie.GetBytes(sb.ToString());
        }
    }
}
