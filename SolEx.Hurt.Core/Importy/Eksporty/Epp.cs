using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using SolEx.Hurt.Core.BLL;
using SolEx.Hurt.Core.BLL.Interfejsy;
using SolEx.Hurt.Core.Helper;
using SolEx.Hurt.Core.ModelBLL;
using SolEx.Hurt.Core.ModelBLL.Interfejsy;
using SolEx.Hurt.Model;
using SolEx.Hurt.Model.AtrybutyKlas;
using SolEx.Hurt.Model.Enums;
using SolEx.Hurt.Model.Interfaces;

namespace SolEx.Hurt.Core.Importy.Eksporty
{
    [LinkDokumentacji("http://bok.solexb2b.com/index.php?/Knowledgebase/Article/View/89/0/import-pliku-epp")]
    [FriendlyName("Format plików EPP jest przygotowany tak aby umożliwić szybki i łatwy import faktur i zamówień do SubiektaGT")]
    public class Epp : GenerowanieDokumentu
    {
        private const string DocumentNaglowek = @"""{document_type}"",{document_status},0,{document_number},""{name}"",,""{name}"",,,,,""{customer_code}"",""{customer_short_name}"",""{customer_full_name}"",""{customer_city}"",""{customer_zipcode}"",""{customer_street}"",""{customer_nip}"",""{document_category}"","""",""{seller_city}"",{date},{date},{date},{document_items},1,""{price_level}"",{sum_netto},{sum_vat},{sum_brutto},{sum_cost},,0.00,{payment_form_name},{payment_date},0.0000,{to_pay},0,0,1,0,""{seller_person}"","""",,0.0000,0.0000,""{currency_symbol}"",1.0000,""{document_desc}"",""{docuemnt_comment}"",,,0,0,0,""{payment_card_name}"",{payment_card_value},""{payment_credit_name}"",{payment_credit_value},""Polska"",""PL"",0";
        private const string DocumentItem = @"{item_number},{product_type},""{product_code}"",{is_discount},1,0,1,{discount_value},{discount_percent},""{jm}"",{quantity},{quantity},0.0000,{price_netto},{price_brutto},{vat},{sum_netto},{sum_vat},{sum_brutto},0.0000,""{usluga_jednorazowa_desc}"",""{usluga_jednorazowa_name}""";
    
        protected byte[] Generuj(DokumentyBll o, IKlient c,bool zakladajKartoteki)
        {
            IKlient customer = c.KlientNadrzedny ?? c;
            string uwagiDokumentu = (o.Uwagi??"").UsunFormatowanie() + "  Import EPP z SOLEX B2B";
             string document = EppHelper.PobierzInstancje.WygenerujNaglowek(seller,o.DataUtworzenia.Date);
             string cd = string.Format("{0:yyyyMMddHHmmss}", o.DataUtworzenia.Date);
          
            Waluta waluta = SolexBllCalosc.PobierzInstancje.DostepDane.PobierzPojedynczy<Waluta>(customer.WalutaId);
            document += "\r\n[NAGLOWEK]\r\n";
            document += new
            {
                document_type = "FZ",
                document_status = "6",
                document_number = o.Id,
                seller_document_number = "",
                name = o.NazwaDokumentu,
                customer_code = seller.Symbol,
                customer_short_name = seller.Name,
                customer_full_name = seller.Name,
                customer_city = seller.Address.Miasto,
                customer_zipcode = seller.Address.KodPocztowy,
                customer_street = seller.Address.UlicaNr,
                customer_nip = seller.NIP,
                document_category = "SOLEXB2B",
                seller_city = "",
                date = cd,
                document_items = o.PobierzPozycjeDokumentu().Count(),
                price_level = "",
                sum_netto = o.DokumentWartoscNetto.Wartosc.ToString(CultureInfo.InvariantCulture).Replace(",", "."),
                sum_vat = o.DokumentWartoscVat.Wartosc.ToString(CultureInfo.InvariantCulture).Replace(",", "."),
                sum_brutto = o.DokumentWartoscBrutto.Wartosc.ToString(CultureInfo.InvariantCulture).Replace(",", "."),
                sum_cost = "0.0000",
                payment_form_name = "",
                payment_date = cd,
                to_pay = o.DokumentWartoscBrutto.Wartosc.ToString(CultureInfo.InvariantCulture).Replace(",", "."),
                seller_person = seller.User,
                currency_symbol = waluta.WalutaB2b ?? waluta.WalutaB2b ?? "PLN",
                document_desc = uwagiDokumentu,
                docuemnt_comment = uwagiDokumentu,
                payment_card_name = "",
                payment_card_value = "0.000",
                payment_credit_name = "",
                payment_credit_value = "0.000"
            }.ToString(DocumentNaglowek) + "\r\n";

            document += "\r\n[ZAWARTOSC]\r\n";
            int id = 0;
            List<DokumentyPozycje> pozycje = o.PobierzPozycjeDokumentu().ToList();
            foreach (DokumentyPozycje i in pozycje)
            {
                ProduktBazowy produkt = i.ProduktBazowy;
                string kod = this.GetProductCode(i);

                id++;
                bool rabat = i.CenaNetto < i.PozycjaDokumentuCenaNetto.Wartosc;
                document += new
                {
                    item_number = id,
                    product_type = "1",
                    product_code = kod,
                    price_netto = rabat ? i.PozycjaDokumentuCenaNetto.Wartosc.ToString(CultureInfo.InvariantCulture).Replace(",", ".") : i.CenaNetto.ToString(CultureInfo.InvariantCulture).Replace(",", "."),
                    price_brutto = rabat ? i.PozycjaDokumentuCenaBrutto.Wartosc.ToString(CultureInfo.InvariantCulture).Replace(",", ".") : i.CenaBrutto.ToString(CultureInfo.InvariantCulture).Replace(",", "."),
                    sum_netto = i.PozycjaDokumentuWartoscNetto.Wartosc.ToString(CultureInfo.InvariantCulture).Replace(",", "."),
                    sum_vat = i.PozycjaDokumentuWartoscVat.Wartosc.ToString(CultureInfo.InvariantCulture).Replace(",", "."),
                    sum_brutto = i.PozycjaDokumentuWartoscBrutto.Wartosc.ToString(CultureInfo.InvariantCulture).Replace(",", "."),
                    seller_code = kod,
                    ean = produkt != null?produkt.KodKreskowy:i.KodProduktu,
                    name = produkt != null?produkt.Nazwa:i.NazwaProduktu,
                    desc = "",
                    fiscal_name = "",
                    SWW = "",
                    PKWIU = produkt != null?produkt.PKWiU:"",
                    quantity = i.PozycjaDokumentuIlosc.Wartosc.ToString(CultureInfo.InvariantCulture).Replace(",", "."),
                    jm = i.Jednostka,
                    vat = i.Vat.ToString(CultureInfo.InvariantCulture).Replace(",", "."),
                    usluga_jednorazowa_desc = "",
                    usluga_jednorazowa_name = "",
                    is_discount = "1",
                    discount_value = "0.0000",
                    discount_percent = i.PozycjaDokumentuRabat.Wartosc.ToString().Replace(",","."),
                   
                }.ToString(DocumentItem) + "\r\n";
            }
        
            var towary = new List<EppHelper.TowarDoGeneracjiEpp>();
            foreach (DokumentyPozycje i in pozycje)
            {
                ProduktBazowy produkt = i.ProduktBazowy;
                if(produkt == null) produkt = new ProduktBazowy() {Kod = i.KodProduktu,Nazwa = i.NazwaProduktu, PKWiU = ""};
                string kod = this.GetProductCode(i);
                towary.Add(new EppHelper.TowarDoGeneracjiEpp(kod, zakladajKartoteki?produkt.Nazwa:"", produkt.KodKreskowy, i.Jednostka, i.Vat, produkt.PKWiU,"","","","",",","","","","",""));
            }
            document += EppHelper.PobierzInstancje.WygenrujSekcjeTowary(towary);

            document += this.WygenerujSekcjeKodCN(pozycje);

            return Kodowanie.GetBytes(document);
        }

        private string GetProductCode(DokumentyPozycje pozycja)
        {
            bool kodkreskowyjakokod = SolexBllCalosc.PobierzInstancje.Konfiguracja.EppKodKresowyKod;

            ProduktBazowy produkt = pozycja.ProduktBazowy;
            
            if (produkt != null)
            {
                return kodkreskowyjakokod && !string.IsNullOrEmpty(produkt.KodKreskowy)
                    ? produkt.KodKreskowy
                    : produkt.Kod;
            }
            return pozycja.KodProduktu;
        }

        protected override byte[] PobierzDane(DokumentyBll dokument, IKlient klient)
        {
            return Generuj(dokument, klient, false);
        }
        public override Encoding Kodowanie
        {
            get { return Encoding.GetEncoding(1250); }
        }

        private string WygenerujSekcjeKodCN(List<DokumentyPozycje> pozycje)
        {
            StringBuilder sb = new StringBuilder("[NAGLOWEK]\r\n");
            sb.AppendLine("[TOWARYKODYCN]");
            sb.AppendLine();
            sb.AppendLine("[ZAWARTOSC]");

            foreach (var p in pozycje)
            {
                string kod = this.GetProductCode(p);
                string kodCN = p.ProduktBazowy?.OpisKrotki5;    //w bio kod cn jest w opis 5
                sb.AppendLine($"\"{kod}\",\"{kodCN}\" "); 
            }
            sb.AppendLine();

            return sb.ToString();
        }

        public override string Nazwa
        {
            get { return "Subiekt EPP"; }
        }

        public override string PobierzNazwePliku(DokumentyBll dokument)
        {
            return NazwaPliku(dokument) + "-subiekt.epp";
        }
        public override bool MoznaGenerowac(DokumentyBll dokument)
        {
            return true;
        }

        public override Licencje? WymaganaLicencja
        {
            get {return Licencje.DokumentyEPP; }
        }
    

private static int FriendlyName(string p)
{
 	throw new NotImplementedException();
}}
}
