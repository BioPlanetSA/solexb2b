using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using SolEx.Hurt.Core.BLL;
using SolEx.Hurt.Core.BLL.Interfejsy;
using SolEx.Hurt.Core.ModelBLL;
using SolEx.Hurt.Model;
using SolEx.Hurt.Model.Enums;
using SolEx.Hurt.Model.Interfaces;

namespace SolEx.Hurt.Core.Importy.Eksporty
{
    public class Kamsoft : GenerowanieDokumentu
    {
        public override bool MoznaGenerowac(DokumentyBll dokument)
        {
            return true;
        }

        public override Licencje? WymaganaLicencja => Licencje.DokumentyKamsoft;

        public override Encoding Kodowanie => Encoding.UTF8;

        public override string Nazwa => "Kamsoft SOW";

        public override string PobierzNazwePliku(DokumentyBll dokument)
        {
            return NazwaPliku(dokument) + "-kamsoft.xml";
        }

        private Dictionary<decimal, string>opisStawekVat = new Dictionary<decimal, string>()
        {
            {23, "A" },
            {8, "B" },
            {0, "C" },
            {5, "D5" }
        };

        //Kontrahent do dodania:
        //kraj, zezwolenie, 
        protected override byte[] PobierzDane(DokumentyBll dokument, IKlient klient)
        {

           

            XmlDocument doc = new XmlDocument();
            XmlDeclaration dec = doc.CreateXmlDeclaration("1.0", "UTF-8", "");
            doc.AppendChild(dec);
            XmlElement dokumenty = doc.CreateElement("dokumenty");
            dokumenty.SetAttribute("xmlns", "http://www.ks-ewd.pl/osoz-edi");
            doc.AppendChild(dokumenty);


            XmlElement info = doc.CreateElement("info");
            dokumenty.AppendChild(info);
            XmlElement wersja = doc.CreateElement("wersja");
            wersja.InnerText = "2012.02.0.0";
            info.AppendChild(wersja);
            XmlElement copyright = doc.CreateElement("copyright");
            copyright.InnerText = "Wszelkie prawa dotyczące formatu OSOZ-EDI są zastrzeżone przez KAMSOFT S.A. Korzystanie z formatu OSOZ-EDI wymaga rejestracji na Portalu KS-EWD oraz spełnienia warunków określonych na stronie www.ks-ewd.pl/osoz-edi.";
            info.AppendChild(copyright);
            XmlElement dataUtworzenia = doc.CreateElement("data-utworzenia");
            dataUtworzenia.InnerText = DateTime.Now.ToString("yyyy-MM-dd h:mm tt");
            info.AppendChild(dataUtworzenia);

            XmlElement kontrahenci = doc.CreateElement("kontrahenci");
            dokumenty.AppendChild(kontrahenci);

            //Kontrahent odbiorca
            XmlElement kontrahentOdbiorca = doc.CreateElement("kontrahent");
            kontrahenci.AppendChild(kontrahentOdbiorca);
            XmlElement kontaOdbiorca = doc.CreateElement("konta");
            kontaOdbiorca.InnerText = " ";
            kontrahentOdbiorca.AppendChild(kontaOdbiorca);
            XmlElement idOdbierajacy = doc.CreateElement("id-knt");
            idOdbierajacy.InnerText = dokument.DokumentOdbiorca.Id.ToString();
            kontrahentOdbiorca.AppendChild(idOdbierajacy);
            XmlElement idOdbierajacyKs = doc.CreateElement("id-knt-ks");
            idOdbierajacyKs.InnerText = dokument.DokumentOdbiorca.Id.ToString();
            kontrahentOdbiorca.AppendChild(idOdbierajacyKs);
            XmlElement nazwaOdbiorcaa = doc.CreateElement("nazwa");
            nazwaOdbiorcaa.InnerText = dokument.DokumentOdbiorca.Nazwa;
            kontrahentOdbiorca.AppendChild(nazwaOdbiorcaa);
            XmlElement nipOdbiorcaa = doc.CreateElement("nip");
            nipOdbiorcaa.InnerText = dokument.DokumentOdbiorca.Nip;
            kontrahentOdbiorca.AppendChild(nipOdbiorcaa);
            XmlElement regonOdbiorcaa = doc.CreateElement("regon");
            regonOdbiorcaa.InnerText = " ";
            kontrahentOdbiorca.AppendChild(regonOdbiorcaa);
            XmlElement ulicaOdbiorca = doc.CreateElement("ulica");
            ulicaOdbiorca.InnerText = dokument.DokumentOdbiorca.DomyslnyAdres.UlicaNr;
            kontrahentOdbiorca.AppendChild(ulicaOdbiorca);
            XmlElement nrDomuOdbiorca = doc.CreateElement("nr-domu");
            nrDomuOdbiorca.InnerText = " ";
            kontrahentOdbiorca.AppendChild(nrDomuOdbiorca);
            XmlElement nrLokaluOdbiorcaa = doc.CreateElement("nr-lokalu");
            nrLokaluOdbiorcaa.InnerText = " ";
            kontrahentOdbiorca.AppendChild(nrLokaluOdbiorcaa);
            XmlElement kodPocztowyOdbiorcaa = doc.CreateElement("kod-pocztowy");
            kodPocztowyOdbiorcaa.InnerText = dokument.DokumentOdbiorca.DomyslnyAdres.KodPocztowy;
            kontrahentOdbiorca.AppendChild(kodPocztowyOdbiorcaa);
            XmlElement miejscowoscOdbiorcaa = doc.CreateElement("miejscowosc");
            miejscowoscOdbiorcaa.InnerText = dokument.DokumentOdbiorca.DomyslnyAdres.Miasto;
            kontrahentOdbiorca.AppendChild(miejscowoscOdbiorcaa);
            XmlElement krajOdbiorcaa = doc.CreateElement("kraj");
            krajOdbiorcaa.InnerText = dokument.DokumentOdbiorca.DomyslnyAdres.KrajSymbol;
            kontrahentOdbiorca.AppendChild(krajOdbiorcaa);
            XmlElement telefonOdbiorcaa = doc.CreateElement("telefon");
            telefonOdbiorcaa.InnerText = dokument.DokumentOdbiorca.Telefon;
            kontrahentOdbiorca.AppendChild(telefonOdbiorcaa);
            XmlElement faxOdbiorcaa = doc.CreateElement("fax");
            faxOdbiorcaa.InnerText = " ";
            kontrahentOdbiorca.AppendChild(faxOdbiorcaa);
            XmlElement emailOdbiorcaa = doc.CreateElement("email");
            emailOdbiorcaa.InnerText = dokument.DokumentOdbiorca.Email;
            kontrahentOdbiorca.AppendChild(emailOdbiorcaa);
            XmlElement zezwolenieOdbiorc = doc.CreateElement("zezwolenie");
            zezwolenieOdbiorc.InnerText = " ";
            kontrahentOdbiorca.AppendChild(zezwolenieOdbiorc);
            XmlElement bankNazwaOdbiorcaa = doc.CreateElement("bank-nazwa");
            bankNazwaOdbiorcaa.InnerText = " ";
            kontrahentOdbiorca.AppendChild(bankNazwaOdbiorcaa);
            XmlElement bankNrKontaOdbiorcaa = doc.CreateElement("bank-nr-konta");
            bankNrKontaOdbiorcaa.InnerText = " ";
            kontrahentOdbiorca.AppendChild(bankNrKontaOdbiorcaa);

            //Kontrahent sprzedawca
            XmlElement kontrahentWystawiajacy = doc.CreateElement("kontrahent");
            kontrahenci.AppendChild(kontrahentWystawiajacy);
            XmlElement konta = doc.CreateElement("konta");
            konta.InnerText = " ";
            kontrahentWystawiajacy.AppendChild(konta);
            XmlElement idKontahenta = doc.CreateElement("id-knt");
            idKontahenta.InnerText = " ";
            kontrahentWystawiajacy.AppendChild(idKontahenta);
            XmlElement idKontahentaKs = doc.CreateElement("id-knt-ks");
            idKontahentaKs.InnerText = " ";
            kontrahentWystawiajacy.AppendChild(idKontahentaKs);
            XmlElement nazwa = doc.CreateElement("nazwa");
            nazwa.InnerText = SolexBllCalosc.PobierzInstancje.Konfiguracja.wlasciciel_nazwa;
            kontrahentWystawiajacy.AppendChild(nazwa);
            XmlElement nip = doc.CreateElement("nip");
            nip.InnerText = " ";
            kontrahentWystawiajacy.AppendChild(nip);
            XmlElement regon = doc.CreateElement("regon");
            regon.InnerText = " ";
            kontrahentWystawiajacy.AppendChild(regon);
            XmlElement ulica = doc.CreateElement("ulica");
            ulica.InnerText = SolexBllCalosc.PobierzInstancje.Konfiguracja.wlasciciel_adres_ulica;
            kontrahentWystawiajacy.AppendChild(ulica);
            XmlElement nrDomu = doc.CreateElement("nr-domu");
            nrDomu.InnerText = " ";
            kontrahentWystawiajacy.AppendChild(nrDomu);
            XmlElement nrLokalu = doc.CreateElement("nr-lokalu");
            nrLokalu.InnerText = " ";
            kontrahentWystawiajacy.AppendChild(nrLokalu);
            XmlElement kodPocztowy = doc.CreateElement("kod-pocztowy");
            kodPocztowy.InnerText = SolexBllCalosc.PobierzInstancje.Konfiguracja.wlasciciel_adres_kod;
            kontrahentWystawiajacy.AppendChild(kodPocztowy);
            XmlElement miejscowosc = doc.CreateElement("miejscowosc");
            miejscowosc.InnerText = SolexBllCalosc.PobierzInstancje.Konfiguracja.wlasciciel_adres_miasto;
            kontrahentWystawiajacy.AppendChild(miejscowosc);
            XmlElement kraj = doc.CreateElement("kraj");
            kraj.InnerText = "PL";
            kontrahentWystawiajacy.AppendChild(kraj);
            XmlElement telefon = doc.CreateElement("telefon");
            telefon.InnerText = SolexBllCalosc.PobierzInstancje.Konfiguracja.WlascicielAdresTelefon;
            kontrahentWystawiajacy.AppendChild(telefon);
            XmlElement fax = doc.CreateElement("fax");
            fax.InnerText = " ";
            kontrahentWystawiajacy.AppendChild(fax);
            XmlElement email = doc.CreateElement("email");
            email.InnerText = " ";
            kontrahentWystawiajacy.AppendChild(email);
            XmlElement zezwolenie = doc.CreateElement("zezwolenie");
            zezwolenie.InnerText = " ";
            kontrahentWystawiajacy.AppendChild(zezwolenie);
            XmlElement bankNazwa = doc.CreateElement("bank-nazwa");
            bankNazwa.InnerText = " ";
            kontrahentWystawiajacy.AppendChild(bankNazwa);
            XmlElement bankNrKonta = doc.CreateElement("bank-nr-konta");
            bankNrKonta.InnerText = " ";
            kontrahentWystawiajacy.AppendChild(bankNrKonta);


            //kontrahent platnik
            XmlElement kontrahentPlatnik = doc.CreateElement("kontrahent");
            kontrahenci.AppendChild(kontrahentPlatnik);
            XmlElement kontaPlatnika = doc.CreateElement("konta");
            kontaPlatnika.InnerText = " ";
            kontrahentPlatnik.AppendChild(kontaPlatnika);
            XmlElement idPlatnik = doc.CreateElement("id-knt");
            idPlatnik.InnerText = dokument.Klient.Id.ToString();
            kontrahentPlatnik.AppendChild(idPlatnik);
            XmlElement idOPlatnikKs = doc.CreateElement("id-knt-ks");
            idOPlatnikKs.InnerText = dokument.Klient.Id.ToString();
            kontrahentPlatnik.AppendChild(idOPlatnikKs);
            XmlElement nazwaPlatnika = doc.CreateElement("nazwa");
            nazwaPlatnika.InnerText = dokument.Klient.Nazwa;
            kontrahentPlatnik.AppendChild(nazwaPlatnika);
            XmlElement nipPlatnika = doc.CreateElement("nip");
            nipPlatnika.InnerText = dokument.Klient.Nip;
            kontrahentPlatnik.AppendChild(nipPlatnika);
            XmlElement regonPlatnika = doc.CreateElement("regon");
            regonPlatnika.InnerText = " ";
            kontrahentPlatnik.AppendChild(regonPlatnika);
            XmlElement ulicaPlatnik = doc.CreateElement("ulica");
            ulicaPlatnik.InnerText = dokument.Klient.DomyslnyAdres.UlicaNr;
            kontrahentPlatnik.AppendChild(ulicaPlatnik);
            XmlElement nrDomuPlatnik = doc.CreateElement("nr-domu");
            nrDomuPlatnik.InnerText = " ";
            kontrahentPlatnik.AppendChild(nrDomuPlatnik);
            XmlElement nrLokaluPlatnika = doc.CreateElement("nr-lokalu");
            nrLokaluPlatnika.InnerText = " ";
            kontrahentPlatnik.AppendChild(nrLokaluPlatnika);
            XmlElement kodPocztowyPlatnika = doc.CreateElement("kod-pocztowy");
            kodPocztowyPlatnika.InnerText = dokument.Klient.DomyslnyAdres.KodPocztowy;
            kontrahentPlatnik.AppendChild(kodPocztowyPlatnika);
            XmlElement miejscowoscPlatnika = doc.CreateElement("miejscowosc");
            miejscowoscPlatnika.InnerText = dokument.Klient.DomyslnyAdres.Miasto;
            kontrahentPlatnik.AppendChild(miejscowoscPlatnika);
            XmlElement krajPlatnika = doc.CreateElement("kraj");
            krajPlatnika.InnerText = dokument.Klient.DomyslnyAdres.KrajSymbol;
            kontrahentPlatnik.AppendChild(krajPlatnika);
            XmlElement telefonPlatnika = doc.CreateElement("telefon");
            telefonPlatnika.InnerText = dokument.Klient.Telefon;
            kontrahentPlatnik.AppendChild(telefonPlatnika);
            XmlElement faxPlatnika = doc.CreateElement("fax");
            faxPlatnika.InnerText = " ";
            kontrahentPlatnik.AppendChild(faxPlatnika);
            XmlElement emailPlatnika = doc.CreateElement("email");
            emailPlatnika.InnerText = dokument.Klient.Email;
            kontrahentPlatnik.AppendChild(emailPlatnika);
            zezwolenie.InnerText = " ";
            XmlElement zezwoleniePlatnik = doc.CreateElement("zezwolenie");
            kontrahentPlatnik.AppendChild(zezwoleniePlatnik);
            XmlElement bankNazwaPlatnika = doc.CreateElement("bank-nazwa");
            bankNazwaPlatnika.InnerText = " ";
            kontrahentPlatnik.AppendChild(bankNazwaPlatnika);
            XmlElement bankNrKontaPlatnika = doc.CreateElement("bank-nr-konta");
            bankNrKontaPlatnika.InnerText = " ";
            kontrahentPlatnik.AppendChild(bankNrKontaPlatnika);

            //Do sekcji dokumenty dodajemy wezel towary
            XmlElement towary = doc.CreateElement("towary");
            dokumenty.AppendChild(towary);

            var produkty = dokument.PobierzPozycjeDokumentu().ToList();
            foreach (var dokumentyPozycje in produkty)
            {
                XmlElement towar = doc.CreateElement("towar");
                towary.AppendChild(towar);

                XmlElement idTowaru = doc.CreateElement("id-towaru");
                idTowaru.InnerText = dokumentyPozycje.ProduktId.ToString();
                towar.AppendChild(idTowaru);
                XmlElement idTowaruKs = doc.CreateElement("id-towaru-ks");
                idTowaruKs.InnerText = " ";
                towar.AppendChild(idTowaruKs);
                XmlElement nazwaTowaru = doc.CreateElement("nazwa");
                nazwaTowaru.InnerText = dokumentyPozycje.ProduktBazowy?.Nazwa ?? dokumentyPozycje.NazwaProduktu;
                towar.AppendChild(nazwaTowaru);
                XmlElement postac = doc.CreateElement("postac");
                postac.InnerText = " ";
                towar.AppendChild(postac);
                XmlElement dawka = doc.CreateElement("dawka");
                dawka.InnerText = " ";
                towar.AppendChild(dawka);
                XmlElement opakowanie = doc.CreateElement("opakowanie");
                opakowanie.InnerText = " ";
                towar.AppendChild(opakowanie);
                XmlElement stawkaVatProduktu = doc.CreateElement("stawka-vat");
                stawkaVatProduktu.InnerText = dokumentyPozycje.ProduktBazowy==null? ((int)dokumentyPozycje.Vat).ToString() : ((int)dokumentyPozycje.ProduktBazowy.Vat).ToString();
                towar.AppendChild(stawkaVatProduktu);
                XmlElement pkwiu = doc.CreateElement("pkwiu");
                pkwiu.InnerText = dokumentyPozycje.ProduktBazowy?.PKWiU?? " ";
                towar.AppendChild(pkwiu);
                XmlElement podmiotOdpowiedzialnyNazwa = doc.CreateElement("podmiot-odpowiedzialny-nazwa");
                podmiotOdpowiedzialnyNazwa.InnerText = " ";
                towar.AppendChild(podmiotOdpowiedzialnyNazwa);
                XmlElement podmiotOdpowiedzialnyKraj = doc.CreateElement("podmiot-odpowiedzialny-kraj");
                podmiotOdpowiedzialnyKraj.InnerText = " ";
                towar.AppendChild(podmiotOdpowiedzialnyKraj);
            }
            XmlElement faktury = doc.CreateElement("faktury");
            dokumenty.AppendChild(faktury);
            XmlElement faktura = doc.CreateElement("faktura");
            faktury.AppendChild(faktura);
            XmlElement naglowek = doc.CreateElement("naglowek");
            faktura.AppendChild(naglowek);


            //zawartosc naglowka
            XmlElement czyDuplikat = doc.CreateElement("czy-duplikat");
            czyDuplikat.InnerText = "Nie";
            naglowek.AppendChild(czyDuplikat);
            XmlElement dataWystawienia = doc.CreateElement("data-wystawienia");
            dataWystawienia.InnerText = dokument.DataUtworzenia.Date.ToString("yyyy-MM-dd");
            naglowek.AppendChild(dataWystawienia);
            XmlElement dataSprzedazy = doc.CreateElement("data-sprzedazy");
            dataSprzedazy.InnerText = dokument.DataUtworzenia.Date.ToString("yyyy-MM-dd");
            naglowek.AppendChild(dataSprzedazy);
            XmlElement idSprzedawcy = doc.CreateElement("id-knt-sprzedawcy");
            idSprzedawcy.InnerText = " ";
            naglowek.AppendChild(idSprzedawcy);
            XmlElement idNabywcy = doc.CreateElement("id-knt-nabywcy");
            idNabywcy.InnerText = dokument.Klient.Id.ToString();
            naglowek.AppendChild(idNabywcy);
            XmlElement idOdbiorcy = doc.CreateElement("id-knt-odbiorcy");
            idOdbiorcy.InnerText = dokument.DokumentOdbiorca.Id.ToString();
            naglowek.AppendChild(idOdbiorcy);
            XmlElement terminPlatnosci = doc.CreateElement("termin-platnosci");
            terminPlatnosci.InnerText = dokument.TerminPlatnosci?.ToString("yyyy-MM-dd") ?? "";
            naglowek.AppendChild(terminPlatnosci);
            XmlElement nrFaktury = doc.CreateElement("nr-faktury");
            nrFaktury.InnerText = dokument.NazwaDokumentu;
            naglowek.AppendChild(nrFaktury);
            XmlElement typFaktury = doc.CreateElement("typ-faktury");
            typFaktury.InnerText = dokument.NazwaDokumentu.Split(' ').First();
            naglowek.AppendChild(typFaktury);
            XmlElement idFaktury = doc.CreateElement("id-faktury");
            idFaktury.InnerText = dokument.Id.ToString();
            naglowek.AppendChild(idFaktury);
            XmlElement uwagi = doc.CreateElement("uwagi");
            uwagi.InnerText = dokument.Uwagi;
            naglowek.AppendChild(uwagi);
            XmlElement gid = doc.CreateElement("gid");
            gid.InnerText = " ";
            naglowek.AppendChild(gid);
            XmlElement nrRezerwacji = doc.CreateElement("nr-rezerwacji");
            nrRezerwacji.InnerText = " ";
            naglowek.AppendChild(nrRezerwacji);
            XmlElement wartoscNetto = doc.CreateElement("wartosc-netto");
            wartoscNetto.InnerText = dokument.WartoscNetto.ToString("F2");
            naglowek.AppendChild(wartoscNetto);
            XmlElement wartoscBruttoSlownie = doc.CreateElement("wartosc-brutto-slownie");
            wartoscBruttoSlownie.InnerText = " ";
            naglowek.AppendChild(wartoscBruttoSlownie);
            XmlElement wartoscBrutto = doc.CreateElement("wartosc-brutto");
            wartoscBrutto.InnerText = dokument.WartoscBrutto.ToString("F2");
            naglowek.AppendChild(wartoscBrutto);
            XmlElement wartoscVat = doc.CreateElement("wartosc-vat");
            wartoscVat.InnerText = dokument.WartoscVat.ToString("F2");
            naglowek.AppendChild(wartoscVat);
            XmlElement formaPlatnosci = doc.CreateElement("forma-platnosci");
            formaPlatnosci.InnerText = dokument.NazwaPlatnosci;
            naglowek.AppendChild(formaPlatnosci);
            XmlElement czyKorekta = doc.CreateElement("czy-korekta");
            czyKorekta.InnerText = dokument.DokumentWartoscNetto < 0 ? "Tak" : "Nie";
            naglowek.AppendChild(czyKorekta);
            XmlElement liczbaPozycji = doc.CreateElement("liczba-poz");
            liczbaPozycji.InnerText = produkty.Count.ToString();
            naglowek.AppendChild(liczbaPozycji);
            
            XmlElement pozycje = doc.CreateElement("pozycje");
            faktura.AppendChild(pozycje);

            for (int i = 0; i < produkty.Count; i++)
            {
                XmlElement pozycja = doc.CreateElement("pozycja");
                pozycje.AppendChild(pozycja);
                XmlElement idPozycji = doc.CreateElement("id-poz-faktury");
                idPozycji.InnerText = produkty[i].Id.ToString();
                pozycja.AppendChild(idPozycji);
                XmlElement nrPozycji = doc.CreateElement("nr-poz-faktury");
                nrPozycji.InnerText = (i + 1).ToString();
                pozycja.AppendChild(nrPozycji);
                XmlElement idTowaru = doc.CreateElement("id-towaru");
                idTowaru.InnerText = produkty[i].ProduktId.ToString();
                pozycja.AppendChild(idTowaru);
                
                XmlElement rodzajCeny = doc.CreateElement("rodzaj-ceny");
                rodzajCeny.InnerText = "N";
                pozycja.AppendChild(rodzajCeny);
                XmlElement cenaNettoPrzedRabatem = doc.CreateElement("cena-netto-bu");
                cenaNettoPrzedRabatem.InnerText = produkty[i].CenaNetto.ToString("F2");
                pozycja.AppendChild(cenaNettoPrzedRabatem);
                XmlElement cenaNetto = doc.CreateElement("cena-netto");
                cenaNetto.InnerText = produkty[i].CenaNettoPoRabacie.ToString("F2");
                pozycja.AppendChild(cenaNetto);
                XmlElement cenaBruttoPrzedRabatem = doc.CreateElement("cena-brutto-bu");
                cenaBruttoPrzedRabatem.InnerText = produkty[i].CenaBrutto.ToString("F2");
                pozycja.AppendChild(cenaBruttoPrzedRabatem);
                XmlElement cenaBrutto = doc.CreateElement("cena-brutto");
                cenaBrutto.InnerText = produkty[i].CenaBruttoPoRabacie.ToString("F2");
                pozycja.AppendChild(cenaBrutto);
                XmlElement ilosc = doc.CreateElement("ilosc");
                ilosc.InnerText = produkty[i].Ilosc.ToString(CultureInfo.InvariantCulture);
                pozycja.AppendChild(ilosc);
                XmlElement jdm = doc.CreateElement("jednostka-miary");
                jdm.InnerText = produkty[i].Jednostka;
                pozycja.AppendChild(jdm);
                XmlElement stawkaVatPozycji = doc.CreateElement("stawka-vat");
                stawkaVatPozycji.InnerText = ((int)produkty[i].Vat).ToString(CultureInfo.InvariantCulture);
                pozycja.AppendChild(stawkaVatPozycji);
                XmlElement wartoscPozycjiVat = doc.CreateElement("wartosc-vat");
                wartoscPozycjiVat.InnerText = produkty[i].WartoscVat.ToString("F2");
                pozycja.AppendChild(wartoscPozycjiVat);

                decimal wartoscWatBu = produkty[i].CenaBrutto * produkty[i].Ilosc - (produkty[i].CenaNetto * produkty[i].Ilosc);
                XmlElement wartoscVatBu = doc.CreateElement("wartosc-vat");
                wartoscVatBu.InnerText = wartoscWatBu.ToString("F2");
                pozycja.AppendChild(wartoscVatBu);
                XmlElement wartoscPozycjiNetto = doc.CreateElement("wartosc-netto");
                wartoscPozycjiNetto.InnerText = produkty[i].WartoscNetto.ToString("F2");
                pozycja.AppendChild(wartoscPozycjiNetto);
                XmlElement wartoscPozycjiBrutto = doc.CreateElement("wartosc-brutto");
                wartoscPozycjiBrutto.InnerText = produkty[i].WartoscBrutto.ToString("F2");
                pozycja.AppendChild(wartoscPozycjiBrutto);
                XmlElement cenaDetalicznaBrutto = doc.CreateElement("cena-detal-brutto");

                int cena = SolexBllCalosc.PobierzInstancje.Konfiguracja.GetPriceLevelDetal ?? 0;
                decimal cenaDetaliczna = produkty[i].CenaBrutto;
                if (produkty[i].ProduktBazowy!=null && produkty[i].ProduktBazowy.CenyPoziomy != null && produkty[i].ProduktBazowy.CenyPoziomy.TryGetValue(cena, out CenaPoziomu cenaPoziom))
                {
                    cenaDetaliczna = cenaPoziom.Netto * (1+ produkty[i].ProduktBazowy.Vat/100);
                }
                cenaDetalicznaBrutto.InnerText = cenaDetaliczna.ToString("F2");
                pozycja.AppendChild(cenaDetalicznaBrutto);

                XmlElement dataWaznosci = doc.CreateElement("data-waznosci");
                dataWaznosci.InnerText = " ";
                pozycja.AppendChild(dataWaznosci);
                XmlElement seria = doc.CreateElement("seria");
                seria.InnerText = " ";
                pozycja.AppendChild(seria);
                XmlElement kodKreskowy = doc.CreateElement("kod-kreskowy");
                kodKreskowy.InnerText = produkty[i].ProduktBazowy?.KodKreskowy?? " ";
                pozycja.AppendChild(kodKreskowy);
                XmlElement upust = doc.CreateElement("upust");
                upust.InnerText = produkty[i].Rabat.ToString(CultureInfo.InvariantCulture);
                pozycja.AppendChild(upust);

                XmlElement upustKwota = doc.CreateElement("upust-kwota");
                upustKwota.InnerText = ((produkty[i].CenaNetto * produkty[i].Ilosc) - produkty[i].WartoscNetto).ToString("F2");
                pozycja.AppendChild(upustKwota);
                XmlElement wytworniaNazwa = doc.CreateElement("wytworca-nazwa");
                wytworniaNazwa.InnerText = " ";
                pozycja.AppendChild(wytworniaNazwa);
                XmlElement wytworniaKraj = doc.CreateElement("wytworca-kraj");
                wytworniaKraj.InnerText = " ";
                pozycja.AppendChild(wytworniaKraj);
                XmlElement pkwiu = doc.CreateElement("pkwiu");
                pkwiu.InnerText = produkty[i].ProduktBazowy?.PKWiU?? " ";
                pozycja.AppendChild(pkwiu);

            }

            XmlElement podsumowanie = doc.CreateElement("podsumowanie");
            faktura.AppendChild(podsumowanie);
            XmlElement stawki = doc.CreateElement("stawki");
            podsumowanie.AppendChild(stawki);

            Dictionary<decimal, List<DokumentyPozycje>> slowikStawekVAt = produkty.GroupBy(x => x.Vat).ToDictionary(x => x.Key, x => x.ToList());
            foreach (var stawkaVat in slowikStawekVAt)
            {
                XmlElement stawka = doc.CreateElement("stawka");
                stawki.AppendChild(stawka);
                XmlElement stawkaVatPodsumowanie = doc.CreateElement("stawka-vat");
                stawkaVatPodsumowanie.InnerText = ((int)stawkaVat.Key).ToString();
                stawka.AppendChild(stawkaVatPodsumowanie);
                XmlElement wartoscVatPodsumowanie = doc.CreateElement("wartosc-vat");
                wartoscVatPodsumowanie.InnerText = stawkaVat.Value.Sum(x => x.WartoscVat).ToString("F2");//dokument.DokumentWartoscVat.Wartosc.ToString("F2");
                stawka.AppendChild(wartoscVatPodsumowanie);

                XmlElement wartoscNettoStawka = doc.CreateElement("wartosc-netto");
                wartoscNettoStawka.InnerText = stawkaVat.Value.Sum(x => x.WartoscNetto).ToString("F2");//dokument.DokumentWartoscVat.Wartosc.ToString("F2");
                stawka.AppendChild(wartoscNettoStawka);
                XmlElement wartoscBruttoStawka = doc.CreateElement("wartosc-brutto");
                wartoscBruttoStawka.InnerText = stawkaVat.Value.Sum(x => x.WartoscBrutto).ToString("F2");//dokument.DokumentWartoscVat.Wartosc.ToString("F2");
                stawka.AppendChild(wartoscBruttoStawka);

                XmlElement opisStawki = doc.CreateElement("opis");
                opisStawki.InnerText = opisStawekVat[stawkaVat.Key];
                stawka.AppendChild(opisStawki);
            }

            return Kodowanie.GetBytes(doc.OuterXml.Replace("> <", "><"));


        }
        
    }
}
