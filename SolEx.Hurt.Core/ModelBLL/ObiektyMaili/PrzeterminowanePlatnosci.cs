using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using SolEx.Hurt.Core.BLL;
using SolEx.Hurt.Core.ModelBLL.Interfejsy;
using SolEx.Hurt.Helpers;
using SolEx.Hurt.Model.Core;
using SolEx.Hurt.Model.Enums;
using SolEx.Hurt.Model.Interfaces;

namespace SolEx.Hurt.Core.ModelBLL.ObiektyMaili
{
    public class PrzeterminowanePlatnosci : SzablonMailaBaza
    {

        public PrzeterminowanePlatnosci(IEnumerable<DokumentyBll> dokumenty, IKlient klient) : base(klient)
        {
            Dokumenty = dokumenty;
        }

        public PrzeterminowanePlatnosci() : base(null)
        {
        }

        public IEnumerable<DokumentyBll> Dokumenty { get; set; }

        public override string NazwaFormatu()
        {
            return "Zbliżające się i przeterminowane płatności";
        }

        public override string OpisFormatu()
        {
            return "Mail przypominający o przeterminowanych płatnościach i tych których termin zbliża się do zapłaty. Mail będzie wysłany tylko do klientów którzy posiadają wypełniony alternatywny adres email" +
                   "Maile wysyłane są przez moduł synchronizacji 'Wyślij maile o przeterminowanych fakturach' lub " +
                   "przez wywołanie metody API - api2/maile/Niezaplacone/ile dni przed terminem/ile dni po terminie/co ile dni ponowne wyslanie?kategoriaKlientaNieWysylaj=1,2&kategoriaKlientaWysylaj=3,4.<br />" +
                   "Przykładowy link wygląda następująco: api2/maile/Niezaplacone/5/2/1?kategoriaKlientaNieWysylaj=1,2&kategoriaKlientaWysylaj=3,4 <br/> " +
                   "Parametry kategoriaKlientaNieWysylaj oraz kategoriaKlientaWysylaj są opcjonalne. Przykładowy link bez kategorii wygląda następująco: api2/maile/Niezaplacone/5/2/1" +
                   "Konfiguracja jakie dokumenty mają być wysyłane ustawiana jest w module synchronizacji.";
        }
        public override string OpisDlaKlienta()
        {
            return "Mail przypominający o przeterminowanych płatnościach i tych których termin zbliża się do zapłaty.";
        }
        public WartoscLiczbowa SumaDoZaplatyZKorektami
        {
            get
            {
                decimal wartoscNalezna =0;
                foreach (var dok in Dokumenty)
                {
                    wartoscNalezna += dok.DokumentWartoscNalezna??0;
                }
                return new WartoscLiczbowa(wartoscNalezna, Dokumenty.First().walutaB2b);
            }
        }

        public List<DokumentyBll> Przeterminowane
        {
            get { return Dokumenty.Where(x => x.TerminPlatnosci.HasValue && x.TerminPlatnosci.Value < DateTime.Now.Date && (x.DokumentWartoscNalezna==null ||x.DokumentWartoscNalezna>0)).OrderByDescending(x => x.TerminPlatnosci).ToList(); }
        }

        public List<DokumentyBll> Nadchodzace
        {
            get { return Dokumenty.Where(x => (!x.TerminPlatnosci.HasValue || x.TerminPlatnosci.Value >= DateTime.Now.Date) && (x.DokumentWartoscNalezna == null || x.DokumentWartoscNalezna > 0)).OrderBy(x => x.TerminPlatnosci).ToList(); }
        }

        public List<DokumentyBll> Faktoringowe
        {
            get { return Dokumenty.Where(x => x.StatusId != null && Calosc.Konfiguracja.StatusyZamowien[x.StatusId.Value].TraktujJakoFaktoring).ToList(); }
        }

        public List<DokumentyBll> Wlasne
        {
            get { return Dokumenty.Where(x => x.StatusId != null && !Calosc.Konfiguracja.StatusyZamowien[x.StatusId.Value].TraktujJakoFaktoring || x.StatusId == null && x.DokumentWartoscNalezna > 0).ToList(); }
        }

        public WartoscLiczbowa WlasneNadchodzaceDoZaplaty
        {
            get
            {
                WartoscLiczbowa wartosc = new WartoscLiczbowa(0);
                if (Wlasne != null && Wlasne.Any())
                {
                    foreach (var wlasnyDokument in Wlasne)
                    {
                        if (!wlasnyDokument.CzyPrzeterminowany())
                        {
                            wartosc += wlasnyDokument.DokumentWartoscNalezna;
                        }
                    }
                    return new WartoscLiczbowa(wartosc, Wlasne.First().walutaB2b);
                }
                return new WartoscLiczbowa(0, Dokumenty.First().walutaB2b);
            }
        }

        public WartoscLiczbowa WlasnePrzeterminowaneDoZaplaty
        {
            get
            {
                WartoscLiczbowa wartosc = new WartoscLiczbowa(0);
                if (Wlasne != null && Wlasne.Any())
                {
                    foreach (var wlasnyDokument in Wlasne)
                    {
                        if (wlasnyDokument.CzyPrzeterminowany())
                        {
                            wartosc += wlasnyDokument.DokumentWartoscNalezna;
                        }
                    }
                    return new WartoscLiczbowa(wartosc, Wlasne.First().walutaB2b);
                }
                return new WartoscLiczbowa(0, Dokumenty.First().walutaB2b);

            }
        }

        public WartoscLiczbowa WlasneWszystkieDoZaplatyBrutto
        {
            get
            {
                WartoscLiczbowa wartosc = new WartoscLiczbowa(0);
                if (Wlasne != null && Wlasne.Any())
                {
                    foreach (var wlasnyDokument in Wlasne)
                    {
                        wartosc += wlasnyDokument.DokumentWartoscNalezna;
                    }
                    return new WartoscLiczbowa(wartosc, Wlasne.First().walutaB2b);
                }
                return new WartoscLiczbowa(0, Dokumenty.First().walutaB2b);

            }
        }

        public WartoscLiczbowa WszystkoDoZaplatyBrutto {
            get { return new WartoscLiczbowa(WlasneWszystkieDoZaplatyBrutto, WlasneWszystkieDoZaplatyBrutto.Waluta); } 
    }

    public WartoscLiczbowa WlasneNadchodzaceWartoscBrutto
        {
            get
            {
                WartoscLiczbowa wartosc = new WartoscLiczbowa(0);
                if (Wlasne != null && Wlasne.Any())
                {
                    foreach (var wlasnyDokument in Wlasne)
                    {
                        if (!wlasnyDokument.CzyPrzeterminowany())
                        {
                            wartosc += wlasnyDokument.DokumentWartoscBrutto;
                        }
                    }
                    return new WartoscLiczbowa(wartosc, Wlasne.First().walutaB2b);
                }
                return new WartoscLiczbowa(0, Dokumenty.First().walutaB2b);
            }
        }
        public WartoscLiczbowa WlasnePrzeterminowaneWartoscBrutto
        {
            get
            {

                WartoscLiczbowa wartosc = new WartoscLiczbowa(0);
                if (Wlasne != null && Wlasne.Any())
                {
                    foreach (var wlasnyDokument in Wlasne)
                    {
                        if (wlasnyDokument.CzyPrzeterminowany())
                        {
                            wartosc += wlasnyDokument.DokumentWartoscBrutto;
                        }
                    }
                    return new WartoscLiczbowa(wartosc, Wlasne.First().walutaB2b);
                }
                return new WartoscLiczbowa(0, Dokumenty.First().walutaB2b);
            }
        }


        public string SciezkaDoPlikuPobierz(DokumentyBll dokument)
        {
            var pdf = Calosc.DokumentyDostep.PobierzDostepneFormatyDoPobrania(dokument).FirstOrDefault(x => x.Nazwa == "Pdf");
            return pdf != null ? string.Format("{0}/{1}",Konfiguracja.wlasciciel_AdresPlatformy,pdf.WygenerujLink()) : "";
        }

        public bool CzyIstniejePlikPdf(DokumentyBll dok)
        {
            var test = Sciezka(dok);
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(test);
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                return true;
            }
            catch
            {
                return false;
            }
            
        }

        public bool FaktoringowePrzeterminowaneCzySa
        {
            get
            {
               // WartoscLiczbowa wartosc = new WartoscLiczbowa(0); //zmienna nieużywana 
                if (Faktoringowe != null && Faktoringowe.Any())
                {
                    foreach (var dokumentyFaktoringowe in Faktoringowe)
                    {
                        if (dokumentyFaktoringowe.CzyPrzeterminowany())
                        {
                            return true;
                        }
                    }
                }
                return false;
            }
        }
        public bool FaktoringoweNadchodzaceCzySa
        {
            get
            {
                //WartoscLiczbowa wartosc = new WartoscLiczbowa(0); //zmienna nieużywana 
                if (Faktoringowe!=null && Faktoringowe.Any())
                {
                    foreach (var dokumentyFaktoringowe in Faktoringowe)
                    {
                        if (!dokumentyFaktoringowe.CzyPrzeterminowany())
                        {
                            return true;
                        }
                    }
                }
                return false;
            }
        }

        public string Sciezka(DokumentyBll dok)
        {
            string nazwa = Tools.PobierzInstancje.GetMd5Hash(dok.NazwaDokumentu);
            return string.Format("{1}/sfera/Dokumenty/{0}.pdf", nazwa, Konfiguracja.wlasciciel_AdresPlatformy);
        }

        public WartoscLiczbowa WlasnePrzeterminowaneWartoscZaplacona
        {
            get
            {
                WartoscLiczbowa wartosc = new WartoscLiczbowa(0);
                if (Wlasne!=null && Wlasne.Any())
                {
                    foreach (var wlasnyDokument in Wlasne)
                    {
                        if (wlasnyDokument.CzyPrzeterminowany())
                        {
                            wartosc += (wlasnyDokument.DokumentWartoscBrutto - wlasnyDokument.DokumentWartoscNalezna);
                        }
                    }
                    return new WartoscLiczbowa(wartosc, Wlasne.First().walutaB2b);
                }
                return new WartoscLiczbowa(0, Dokumenty.First().walutaB2b);
            
            }
        }
        public WartoscLiczbowa WlasneNadchodzaceWartoscZaplacona
        {
            get
            {
                WartoscLiczbowa wartosc = new WartoscLiczbowa(0);
                if (Wlasne != null && Wlasne.Any())
                {
                    foreach (var wlasnyDokument in Wlasne)
                    {
                        if (!wlasnyDokument.CzyPrzeterminowany())
                        {
                            wartosc += (wlasnyDokument.DokumentWartoscBrutto - wlasnyDokument.DokumentWartoscNalezna);
                        }
                    }
                    return new WartoscLiczbowa(wartosc, Wlasne.First().walutaB2b);
                }
                return new WartoscLiczbowa(0, Dokumenty.First().walutaB2b);
               
            }
        }

        public override TypyPowiadomienia[] PowiadomieniaDomyslnieAktywne
        {
            get { return new[] {TypyPowiadomienia.Klient}; }
        }
    }
}
