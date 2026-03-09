using System;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Xml;
using SolEx.Hurt.Core.BLL;
using SolEx.Hurt.Core.BLL.Interfejsy;
using SolEx.Hurt.Core.ModelBLL;
using SolEx.Hurt.Core.ModelBLL.Interfejsy;
using SolEx.Hurt.Model.Enums;
using SolEx.Hurt.Model.Interfaces;

namespace SolEx.Hurt.Core.Importy.Eksporty
{
    public class OptimaXml :  GenerowanieDokumentu

    {

        //korekta - 302
        //302101

        //faktura - 302
        //302000


        protected override byte[] PobierzDane(DokumentyBll dokument, IKlient klient)
        {
            string typDokumentu = "302";
            string rodzajDokumentu = "302000";
            bool czyKorekta = false;
            if (dokument.NazwaDokumentu.StartsWith("KFS"))
            {
                czyKorekta = true;
                rodzajDokumentu = "302101";
            }

            IKlient platnikKlient = SolexBllCalosc.PobierzInstancje.DostepDane.PobierzPojedynczy<Klient>(dokument.DokumentPlatnikId);
            XmlDocument doc = new XmlDocument();
            XmlDeclaration dec = doc.CreateXmlDeclaration("1.0", "UTF-8", "");
            doc.AppendChild(dec);
            XmlElement szablon = doc.CreateElement("ROOT");
            doc.AppendChild(szablon);
            XmlElement dokumentO = doc.CreateElement("DOKUMENT");
            szablon.AppendChild(dokumentO);
            XmlElement naglowek = doc.CreateElement("NAGLOWEK");
            dokumentO.AppendChild(naglowek);
            XmlElement generator = doc.CreateElement("GENERATOR");
            generator.InnerText = "Solex";
            naglowek.AppendChild(generator);
            XmlElement typ = doc.CreateElement("TYP_DOKUMENTU");
            typ.InnerText = typDokumentu;
            naglowek.AppendChild(typ);
            XmlElement rodzaj = doc.CreateElement("RODZAJ_DOKUMENTU");
            rodzaj.InnerText = rodzajDokumentu;
            naglowek.AppendChild(rodzaj);
            XmlElement numerpeny = doc.CreateElement("NUMER_PELNY");
            numerpeny.InnerText = dokument.NazwaDokumentu;
            naglowek.AppendChild(numerpeny);
            XmlElement data_Dokumentu = doc.CreateElement("DATA_DOKUMENTU");
            data_Dokumentu.InnerText = dokument.DataUtworzenia.Date.ToString("yyyy-MM-dd");
            naglowek.AppendChild(data_Dokumentu);
            XmlElement data_Wystawienia = doc.CreateElement("DATA_WYSTAWIENIA");
            data_Wystawienia.InnerText = dokument.DataUtworzenia.Date.ToString("yyyy-MM-dd");
            naglowek.AppendChild(data_Wystawienia);
            XmlElement data_Operacji = doc.CreateElement("DATA_OPERACJI");
            data_Operacji.InnerText = dokument.DataUtworzenia.Date.ToString("yyyy-MM-dd");
            naglowek.AppendChild(data_Operacji);
            XmlElement termin_Zwrotu_Kaucji = doc.CreateElement("TERMIN_ZWROTU_KAUCJI");
            naglowek.AppendChild(termin_Zwrotu_Kaucji);
            XmlElement korekta = doc.CreateElement("KOREKTA");
            korekta.InnerText = czyKorekta? "1": "0";
            naglowek.AppendChild(korekta);
            XmlElement detal = doc.CreateElement("DETAL");
            detal.InnerText = "0";
            naglowek.AppendChild(detal);
            XmlElement typ_Netto_Brutto = doc.CreateElement("TYP_NETTO_BRUTTO");
            typ_Netto_Brutto.InnerText = "1";
            naglowek.AppendChild(typ_Netto_Brutto);
            XmlElement rabat = doc.CreateElement("RABAT");
            naglowek.AppendChild(rabat);
            XmlElement opis = doc.CreateElement("OPIS");
            opis.InnerText = "Faktura pobrana z Solex B2B numer " + dokument.NazwaDokumentu;
            naglowek.AppendChild(opis);
          
            XmlElement platnik = doc.CreateElement("PLATNIK");
            naglowek.AppendChild(platnik);
        
            XmlElement platnikkod = doc.CreateElement("KOD");
            platnikkod.InnerText = platnikKlient.Symbol;
            platnik.AppendChild(platnikkod);

            XmlElement platnikNipKraj = doc.CreateElement("NIP_KRAJ");
            platnikNipKraj.InnerText = "";
            platnik.AppendChild(platnikNipKraj);


            XmlElement platniknip = doc.CreateElement("NIP");
            platniknip.InnerText =platnikKlient.Nip;
            platnik.AppendChild(platniknip);
            XmlElement platnikgln = doc.CreateElement("GLN");
            platnik.AppendChild(platnikgln);
            XmlElement platniknazwa = doc.CreateElement("NAZWA");
            platniknazwa.InnerText = platnikKlient.Nazwa;
            platnik.AppendChild(platniknazwa);
            XmlElement platnikadres = doc.CreateElement("ADRES");
            platnik.AppendChild(platnikadres);
            XmlElement platnikadreSkod = doc.CreateElement("KOD_POCZTOWY");
            platnikadreSkod.InnerText = platnikKlient.DomyslnyAdres.KodPocztowy;
            platnikadres.AppendChild(platnikadreSkod);
            XmlElement platnikadreskMiasto = doc.CreateElement("MIASTO");
            platnikadreskMiasto.InnerText =platnikKlient.DomyslnyAdres.Miasto;
            platnikadres.AppendChild(platnikadreskMiasto);
            XmlElement platnikadresulica = doc.CreateElement("ULICA");
            platnikadresulica.InnerText =platnikKlient.DomyslnyAdres.UlicaNr;
            platnikadres.AppendChild(platnikadresulica);
            XmlElement platnikadreskraj = doc.CreateElement("KRAJ");
            platnikadreskraj.InnerText =platnikKlient.DomyslnyAdres.Kraj;
            platnikadres.AppendChild(platnikadreskraj);
            IKlient odbiorcaKlient = SolexBllCalosc.PobierzInstancje.DostepDane.PobierzPojedynczy<Klient>(dokument.DokumentOdbiorcaId);
            XmlElement odbiorca = doc.CreateElement("ODBIORCA");
            naglowek.AppendChild(odbiorca);

            XmlElement odbiorcakod = doc.CreateElement("KOD");
            odbiorcakod.InnerText = odbiorcaKlient.Symbol;
            odbiorca.AppendChild(odbiorcakod);

            XmlElement odbiorcaNipKraj = doc.CreateElement("NIP_KRAJ");
            odbiorcaNipKraj.InnerText = "";
            odbiorca.AppendChild(odbiorcaNipKraj);


            XmlElement odbiorcaknip = doc.CreateElement("NIP");
            odbiorcaknip.InnerText = odbiorcaKlient.Nip;
            odbiorca.AppendChild(odbiorcaknip);
            XmlElement odbiorcagln = doc.CreateElement("GLN");
            odbiorca.AppendChild(odbiorcagln);
            XmlElement odbiorcanazwa = doc.CreateElement("NAZWA");
            odbiorcanazwa.InnerText = odbiorcaKlient.Nazwa;
            odbiorca.AppendChild(odbiorcanazwa);
            XmlElement odbiorcakadres = doc.CreateElement("ADRES");
            odbiorca.AppendChild(odbiorcakadres);
            XmlElement odbiorcaadreSkod = doc.CreateElement("KOD_POCZTOWY");
            odbiorcaadreSkod.InnerText = odbiorcaKlient.DomyslnyAdres.KodPocztowy;
            odbiorcakadres.AppendChild(odbiorcaadreSkod);
            XmlElement odbiorcaadreskMiasto = doc.CreateElement("MIASTO");
            odbiorcaadreskMiasto.InnerText = odbiorcaKlient.DomyslnyAdres.Miasto;
            odbiorcakadres.AppendChild(odbiorcaadreskMiasto);
            XmlElement odbiorcaAdresulica = doc.CreateElement("ULICA");
            odbiorcaAdresulica.InnerText = odbiorcaKlient.DomyslnyAdres.UlicaNr;
            odbiorcakadres.AppendChild(odbiorcaAdresulica);
            XmlElement odbiorcaAdreskraj = doc.CreateElement("KRAJ");
            odbiorcaAdreskraj.InnerText = odbiorcaKlient.DomyslnyAdres.Kraj;
            odbiorcakadres.AppendChild(odbiorcaAdreskraj);

            XmlElement sprzedawca = doc.CreateElement("SPRZEDAWCA");
            naglowek.AppendChild(sprzedawca);

            XmlElement sprzedawcaNipKraj = doc.CreateElement("NIP_KRAJ");
            sprzedawcaNipKraj.InnerText = "";
            sprzedawca.AppendChild(sprzedawcaNipKraj);

            XmlElement nip = doc.CreateElement("NIP");
            nip.InnerText =seller.NIP;
            sprzedawca.AppendChild(nip);
            XmlElement gln = doc.CreateElement("GLN");
            sprzedawca.AppendChild(gln);
            XmlElement sprzNAZWA = doc.CreateElement("NAZWA");
            sprzNAZWA.InnerText =seller.Name;
            sprzedawca.AppendChild(sprzNAZWA);
            XmlElement selADRES = doc.CreateElement("ADRES");
            sprzedawca.AppendChild(selADRES);
            XmlElement spKADRESkod = doc.CreateElement("KOD_POCZTOWY");
            spKADRESkod.InnerText = seller.Address.KodPocztowy;
            selADRES.AppendChild(spKADRESkod);
            XmlElement selmiasto = doc.CreateElement("MIASTO");
            selmiasto.InnerText = seller.Address.Miasto;
            selADRES.AppendChild(selmiasto);
            XmlElement selUlica = doc.CreateElement("ULICA");
            selUlica.InnerText = seller.Address.UlicaNr;
            selADRES.AppendChild(selUlica);
            XmlElement sprzkraj = doc.CreateElement("KRAJ");
            sprzkraj.InnerText = seller.Address.Kraj;
            selADRES.AppendChild(sprzkraj);
            XmlElement numerKontaBankowego = doc.CreateElement("NUMER_KONTA_BANKOWEGO");
            sprzedawca.AppendChild(numerKontaBankowego);
            XmlElement nazwaBanku = doc.CreateElement("NAZWA_BANKU");
            sprzedawca.AppendChild(nazwaBanku);



            XmlElement KATEGORIA = doc.CreateElement("KATEGORIA");
            naglowek.AppendChild(KATEGORIA);
            XmlElement KATEGORIAKod = doc.CreateElement("KOD");
            KATEGORIA.AppendChild(KATEGORIAKod);
            XmlElement KATEGORIAopis = doc.CreateElement("OPIS");
            KATEGORIA.AppendChild(KATEGORIAopis);

            XmlElement PLATNOSC = doc.CreateElement("PLATNOSC");
            naglowek.AppendChild(PLATNOSC);
            XmlElement FORMA = doc.CreateElement("FORMA");
            FORMA.InnerText = string.IsNullOrEmpty(dokument.NazwaPlatnosci) ? "" : "przelew";
            PLATNOSC.AppendChild(FORMA);
            XmlElement TERMIN = doc.CreateElement("TERMIN");
            TERMIN.InnerText = dokument.TerminPlatnosci?.ToString("yyyy-MM-dd") ?? "";
            PLATNOSC.AppendChild(TERMIN);

            XmlElement WALUTA = doc.CreateElement("WALUTA");
            naglowek.AppendChild(WALUTA);
            XmlElement WALUTAs = doc.CreateElement("SYMBOL");
            WALUTAs.InnerText = dokument.walutaB2b;
            WALUTA.AppendChild(WALUTAs);
            XmlElement KURS_L = doc.CreateElement("KURS_L");
            WALUTA.AppendChild(KURS_L);
            XmlElement KURS_M = doc.CreateElement("KURS_M");
            WALUTA.AppendChild(KURS_M);
            XmlElement PLAT_WAL_OD_PLN = doc.CreateElement("PLAT_WAL_OD_PLN");
            WALUTA.AppendChild(PLAT_WAL_OD_PLN);
            XmlElement KURS_NUMER = doc.CreateElement("KURS_NUMER");
            WALUTA.AppendChild(KURS_NUMER);
            XmlElement KURS_DATA = doc.CreateElement("KURS_DATA");
            WALUTA.AppendChild(KURS_DATA);

            XmlElement KWOTY = doc.CreateElement("KWOTY");
            naglowek.AppendChild(KWOTY);
            XmlElement razem_Netto_Wal = doc.CreateElement("RAZEM_NETTO_WAL");
            razem_Netto_Wal.InnerText = dokument.DokumentWartoscNetto.Wartosc.ToString(CultureInfo.InvariantCulture).Replace(",", ".");
            KWOTY.AppendChild(razem_Netto_Wal);
            XmlElement razem_Netto = doc.CreateElement("RAZEM_NETTO");
            razem_Netto.InnerText = dokument.DokumentWartoscNetto.Wartosc.ToString(CultureInfo.InvariantCulture).Replace(",", ".");
            KWOTY.AppendChild(razem_Netto);
            XmlElement razem_Brutto = doc.CreateElement("RAZEM_BRUTTO");
            razem_Brutto.InnerText = dokument.DokumentWartoscBrutto.Wartosc.ToString(CultureInfo.InvariantCulture).Replace(",", ".");
            KWOTY.AppendChild(razem_Brutto);
            XmlElement RAZEM_VAT = doc.CreateElement("RAZEM_VAT");
            RAZEM_VAT.InnerText = dokument.DokumentWartoscVat.Wartosc.ToString(CultureInfo.InvariantCulture).Replace(",", ".");
            KWOTY.AppendChild(RAZEM_VAT);

            XmlElement MAGAZYN_ZRODLOWY = doc.CreateElement("MAGAZYN_ZRODLOWY");
            naglowek.AppendChild(MAGAZYN_ZRODLOWY);
            XmlElement MAGAZYN_DOCELOWY = doc.CreateElement("MAGAZYN_DOCELOWY");
            naglowek.AppendChild(MAGAZYN_DOCELOWY);
            XmlElement KAUCJE_PLATNOSCI = doc.CreateElement("KAUCJE_PLATNOSCI");
            KAUCJE_PLATNOSCI.InnerText ="0";
            naglowek.AppendChild(KAUCJE_PLATNOSCI);
            XmlElement BLOKADA_PLATNOSCI = doc.CreateElement("BLOKADA_PLATNOSCI");
            BLOKADA_PLATNOSCI.InnerText = "0";
            naglowek.AppendChild(BLOKADA_PLATNOSCI);
            XmlElement VAT_DLA_DOK_WAL = doc.CreateElement("VAT_DLA_DOK_WAL");
            VAT_DLA_DOK_WAL.InnerText = "0";
            naglowek.AppendChild(VAT_DLA_DOK_WAL);
            XmlElement trybNetootVat = doc.CreateElement("TRYB_NETTO_VAT");
            VAT_DLA_DOK_WAL.InnerText = "0";
            naglowek.AppendChild(trybNetootVat);


            XmlElement POZYCJE = doc.CreateElement("POZYCJE");
            dokumentO.AppendChild(POZYCJE);
            var pozycjekol = dokument.PobierzPozycjeDokumentu().ToList();
            for (int i = 0; i < pozycjekol.Count; i++)
            {
                var produkt = pozycjekol[i].ProduktBazowy;
                XmlElement pozycja = doc.CreateElement("POZYCJA");
                POZYCJE.AppendChild(pozycja);
                XmlElement lp = doc.CreateElement("LP");
                lp.InnerText = (i + 1).ToString();
                pozycja.AppendChild(lp);
                XmlElement towar = doc.CreateElement("TOWAR");
                pozycja.AppendChild(towar);
                XmlElement kod = doc.CreateElement("KOD");
                kod.InnerText = produkt!=null?produkt.Kod: pozycjekol[i].KodProduktu;
                towar.AppendChild(kod);
                XmlElement NAZWA = doc.CreateElement("NAZWA");
                NAZWA.InnerText = produkt != null ? produkt.Nazwa : pozycjekol[i].NazwaProduktu;
                towar.AppendChild(NAZWA);
                XmlElement OPIS = doc.CreateElement("OPIS");
                towar.AppendChild(OPIS);
                XmlElement EAN = doc.CreateElement("EAN");
                EAN.InnerText = produkt != null ? produkt.KodKreskowy : pozycjekol[i].KodProduktu;
                towar.AppendChild(EAN);
                XmlElement SWW = doc.CreateElement("SWW");
                towar.AppendChild(SWW);
                XmlElement NUMER_KATALOGOWY = doc.CreateElement("NUMER_KATALOGOWY");
                towar.AppendChild(NUMER_KATALOGOWY);

                XmlElement STAWKA_VAT = doc.CreateElement("STAWKA_VAT");
                pozycja.AppendChild(STAWKA_VAT);

                XmlElement STAWKA = doc.CreateElement("STAWKA");
                STAWKA.InnerText =pozycjekol[i].Vat.ToString().Replace(",", ".");
                STAWKA_VAT.AppendChild(STAWKA);
                XmlElement FLAGA = doc.CreateElement("FLAGA");
                FLAGA.InnerText = "2";
                STAWKA_VAT.AppendChild(FLAGA);
                XmlElement ZRODLOWA = doc.CreateElement("ZRODLOWA");
                ZRODLOWA.InnerText = "0.00";
                STAWKA_VAT.AppendChild(ZRODLOWA);
                XmlElement CENY = doc.CreateElement("CENY");
                pozycja.AppendChild(CENY);

                XmlElement POCZATKOWA_WAL_CENNIKA = doc.CreateElement("POCZATKOWA_WAL_CENNIKA");
                POCZATKOWA_WAL_CENNIKA.InnerText = pozycjekol[i].CenaNetto.ToString().Replace(",", ".");
                CENY.AppendChild(POCZATKOWA_WAL_CENNIKA);
                XmlElement POCZATKOWA_WAL_DOKUMENTU = doc.CreateElement("POCZATKOWA_WAL_DOKUMENTU");
                POCZATKOWA_WAL_DOKUMENTU.InnerText = pozycjekol[i].CenaBrutto.ToString().Replace(",", ".");
                CENY.AppendChild(POCZATKOWA_WAL_DOKUMENTU);
                XmlElement PO_RABACIE_WAL_CENNIKA = doc.CreateElement("PO_RABACIE_WAL_CENNIKA");
                PO_RABACIE_WAL_CENNIKA.InnerText = pozycjekol[i].CenaNettoPoRabacie.ToString(CultureInfo.InvariantCulture).Replace(",", ".");
                CENY.AppendChild(PO_RABACIE_WAL_CENNIKA);
                XmlElement PO_RABACIE_PLN = doc.CreateElement("PO_RABACIE_PLN");
                PO_RABACIE_PLN.InnerText = pozycjekol[i].CenaNettoPoRabacie.ToString(CultureInfo.InvariantCulture).Replace(",", ".");
                CENY.AppendChild(PO_RABACIE_PLN);
                XmlElement PO_RABACIE_WAL_DOKUMENTU = doc.CreateElement("PO_RABACIE_WAL_DOKUMENTU");
                PO_RABACIE_WAL_DOKUMENTU.InnerText = pozycjekol[i].CenaNettoPoRabacie.ToString().Replace(",", ".");
                CENY.AppendChild(PO_RABACIE_WAL_DOKUMENTU);


                XmlElement pWALUTA = doc.CreateElement("WALUTA");
                pozycja.AppendChild(pWALUTA);
                XmlElement SYMBOL = doc.CreateElement("SYMBOL");
                SYMBOL.InnerText =pozycjekol[i].walutaB2b;
                pWALUTA.AppendChild(SYMBOL);
                XmlElement pKURS_L = doc.CreateElement("KURS_L");
                pKURS_L.InnerText = "1.00";
                pWALUTA.AppendChild(pKURS_L);
                XmlElement pKURS_M = doc.CreateElement("KURS_M");
                pKURS_M.InnerText = "1.00";
                pWALUTA.AppendChild(pKURS_M);

                XmlElement RABAT = doc.CreateElement("RABAT");
                RABAT.InnerText = pozycjekol[i].PozycjaDokumentuRabat.Wartosc.ToString("0.00").Replace(",", ".");
                pozycja.AppendChild(RABAT);

                XmlElement WARTOSC_NETTO = doc.CreateElement("WARTOSC_NETTO");
                WARTOSC_NETTO.InnerText = pozycjekol[i].PozycjaDokumentuWartoscNetto.Wartosc.ToString("0.00").Replace(",", ".");
                pozycja.AppendChild(WARTOSC_NETTO);

                XmlElement WARTOSC_BRUTTO = doc.CreateElement("WARTOSC_BRUTTO");
                WARTOSC_BRUTTO.InnerText =pozycjekol[i].PozycjaDokumentuWartoscBrutto.Wartosc.ToString("0.00").Replace(",", ".");
                pozycja.AppendChild(WARTOSC_BRUTTO);

                XmlElement WARTOSC_NETTO_WAL = doc.CreateElement("WARTOSC_NETTO_WAL");
                WARTOSC_NETTO_WAL.InnerText = pozycjekol[i].PozycjaDokumentuWartoscNetto.Wartosc.ToString("0.00").Replace(",", ".");
                pozycja.AppendChild(WARTOSC_NETTO_WAL);

                XmlElement WARTOSC_BRUTTO_WAL = doc.CreateElement("WARTOSC_BRUTTO_WAL");
                WARTOSC_BRUTTO_WAL.InnerText =pozycjekol[i].PozycjaDokumentuWartoscBrutto.Wartosc.ToString("0.00").Replace(",", ".");
                pozycja.AppendChild(WARTOSC_BRUTTO_WAL);

                XmlElement ILOSC = doc.CreateElement("ILOSC");
                ILOSC.InnerText = Math.Round(pozycjekol[i].PozycjaDokumentuIlosc.Wartosc, base.Zaokraglenia[pozycjekol[i].JednostkaMiary]).ToString(CultureInfo.InvariantCulture).Replace(",", ".");
                pozycja.AppendChild(ILOSC);

                XmlElement JM = doc.CreateElement("JM");
                JM.InnerText = pozycjekol[i].Jednostka;
                pozycja.AppendChild(JM);

                XmlElement JM_CALKOWITE = doc.CreateElement("JM_CALKOWITE");
                JM_CALKOWITE.InnerText = "0.00";
                pozycja.AppendChild(JM_CALKOWITE);

                XmlElement JM_ZLOZONA = doc.CreateElement("JM_ZLOZONA");
                pozycja.AppendChild(JM_ZLOZONA);

                XmlElement JMZ = doc.CreateElement("JMZ");
                JMZ.InnerText = pozycjekol[i].Jednostka;
                JM_ZLOZONA.AppendChild(JMZ);

                XmlElement JM_PRZELICZNIK_L = doc.CreateElement("JM_PRZELICZNIK_L");
                JM_PRZELICZNIK_L.InnerText = "1.00";
                JM_ZLOZONA.AppendChild(JM_PRZELICZNIK_L);

                XmlElement JM_PRZELICZNIK_M = doc.CreateElement("JM_PRZELICZNIK_M");
                JM_PRZELICZNIK_M.InnerText = "1";
                JM_ZLOZONA.AppendChild(JM_PRZELICZNIK_M);

            }
            XmlElement KAUCJE = doc.CreateElement("KAUCJE");
            dokumentO.AppendChild(KAUCJE);
            XmlElement PLATNOSCI = doc.CreateElement("PLATNOSCI");
            dokumentO.AppendChild(PLATNOSCI);

            XmlElement PLATNOSCd = doc.CreateElement("PLATNOSC");
            PLATNOSCI.AppendChild(PLATNOSCd);

            XmlElement FORMAp = doc.CreateElement("FORMA");
            FORMAp.InnerText = string.IsNullOrEmpty(dokument.NazwaPlatnosci)? "": "przelew";
            PLATNOSCd.AppendChild(FORMAp);

            XmlElement TERMINp = doc.CreateElement("TERMIN");
            TERMINp.InnerText = dokument.TerminPlatnosci.HasValue? dokument.TerminPlatnosci.Value.ToString("yyyy-MM-dd"):"";
            PLATNOSCd.AppendChild(TERMINp);

            XmlElement KWOTAp = doc.CreateElement("KWOTA");
            KWOTAp.InnerText = dokument.DokumentWartoscBrutto.Wartosc.ToString().Replace(",", ".");
            PLATNOSCd.AppendChild(KWOTAp);
            XmlElement KWOTA_W_WAL_SYSTEMOWEJ = doc.CreateElement("KWOTA_W_WAL_SYSTEMOWEJ");
            KWOTA_W_WAL_SYSTEMOWEJ.InnerText = dokument.DokumentWartoscBrutto.Wartosc.ToString().Replace(",", ".");
            PLATNOSCd.AppendChild(KWOTA_W_WAL_SYSTEMOWEJ);

            XmlElement plWALUTA = doc.CreateElement("WALUTA");
            PLATNOSCd.AppendChild(plWALUTA);
            XmlElement plSYMBOL = doc.CreateElement("SYMBOL");
            plSYMBOL.InnerText = dokument.walutaB2b;
            plWALUTA.AppendChild(plSYMBOL);
            XmlElement plpKURS_L = doc.CreateElement("KURS_L");
            plpKURS_L.InnerText = "1.00";
            plWALUTA.AppendChild(plpKURS_L);
            XmlElement plpKURS_M = doc.CreateElement("KURS_M");
            plpKURS_M.InnerText = "1.00";
            plWALUTA.AppendChild(plpKURS_M);


            XmlElement PLATNOSCI_KAUCJE = doc.CreateElement("PLATNOSCI_KAUCJE");
            dokumentO.AppendChild(PLATNOSCI_KAUCJE);

            XmlElement TABELKA_VAT = doc.CreateElement("TABELKA_VAT");
            dokumentO.AppendChild(TABELKA_VAT);

            foreach (var stawka in SolexBllCalosc.PobierzInstancje.DokumentyDostep.DokumentyStawkiVat(dokument) )
            {
                XmlElement LINIA_VAT = doc.CreateElement("LINIA_VAT");
                TABELKA_VAT.AppendChild(LINIA_VAT);

                XmlElement STAWKA_VAT = doc.CreateElement("STAWKA_VAT");
                LINIA_VAT.AppendChild(STAWKA_VAT);

                XmlElement STAWKA = doc.CreateElement("STAWKA");
                STAWKA.InnerText = stawka.Stawka.Wartosc.ToString().Replace(",", ".");
                STAWKA_VAT.AppendChild(STAWKA);
                XmlElement FLAGA = doc.CreateElement("FLAGA");
                FLAGA.InnerText = "2";
                STAWKA_VAT.AppendChild(FLAGA);
                XmlElement ZRODLOWA = doc.CreateElement("ZRODLOWA");
                ZRODLOWA.InnerText = "0.00";
                STAWKA_VAT.AppendChild(ZRODLOWA);


                XmlElement NETTO = doc.CreateElement("NETTO");
                NETTO.InnerText = stawka.WartoscNetto.Wartosc.ToString().Replace(",", ".");
                LINIA_VAT.AppendChild(NETTO);
                XmlElement VAT = doc.CreateElement("VAT");
                VAT.InnerText = stawka.WartoscVAT.Wartosc.ToString().Replace(",", ".");
                LINIA_VAT.AppendChild(VAT);
                XmlElement BRUTTO = doc.CreateElement("BRUTTO");
                BRUTTO.InnerText = stawka.WartoscBrutto.Wartosc.ToString().Replace(",", ".");
                LINIA_VAT.AppendChild(BRUTTO);

                XmlElement NETTO_WAL = doc.CreateElement("NETTO_WAL");
                NETTO_WAL.InnerText = stawka.WartoscNetto.Wartosc.ToString().Replace(",", ".");
                LINIA_VAT.AppendChild(NETTO_WAL);
                XmlElement VAT_WAL = doc.CreateElement("VAT_WAL");
                VAT_WAL.InnerText = stawka.WartoscVAT.Wartosc.ToString().Replace(",", ".");
                LINIA_VAT.AppendChild(VAT_WAL);
                XmlElement BRUTTO_WAL = doc.CreateElement("BRUTTO_WAL");
                BRUTTO_WAL.InnerText = stawka.WartoscBrutto.Wartosc.ToString().Replace(",", ".");
                LINIA_VAT.AppendChild(BRUTTO_WAL);
            }
            XmlElement ATRYBUTY = doc.CreateElement("ATRYBUTY");
            dokumentO.AppendChild(ATRYBUTY);
            return Kodowanie.GetBytes(doc.OuterXml);
        }

        public override Encoding Kodowanie
        {
            get { return Encoding.UTF8; }
        }

        public override string Nazwa
        {
            get { return "Optima XML"; }
        }

        public override string PobierzNazwePliku(DokumentyBll dokument)
        {
            return NazwaPliku(dokument) + "-optima.xml";
        }

        public override bool MoznaGenerowac(DokumentyBll dokument)
        {
            return true;
        }

        public override Hurt.Model.Enums.Licencje? WymaganaLicencja
        {
            get {return Licencje.OptimaEPP; }
        }


    }
}
