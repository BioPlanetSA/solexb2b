using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using SolEx.Hurt.Core.BLL;
using SolEx.Hurt.Core.Importy.Model;
using SolEx.Hurt.Core.ModelBLL.Interfejsy;
using SolEx.Hurt.Helpers;
using SolEx.Hurt.Model;
using SolEx.Hurt.Model.Web;
using log4net;
using SolEx.Hurt.Core.Importy.Koszyk;
using SolEx.Hurt.Core.ModelBLL;
using SolEx.Hurt.Model.Helpers;
using SolEx.Hurt.Model.Interfaces;

namespace SolEx.Hurt.Core.Importy
{
    public class ManagerImportow
    {
        private static ILog Log
        {
            get { return LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType); }
        }

        public static List<ImportBaza> PobierzDostepneModuly()
        {
            List<Type> lista = Refleksja.PobierzListeKlasDziedziczacychPoKlasieBazowej(typeof(ImportBaza));
            List<ImportBaza> listaModulow = new List<ImportBaza>(lista.Count);
            listaModulow.AddRange(from t in lista where !t.IsAbstract select (ImportBaza) Activator.CreateInstance(t));
            return listaModulow;
        }

        public static List<Komunikat> WczytajZamowienie(Type modul, string dane, IKlient klient, KoszykBll koszyk, out OdpowiedzKoszyk odpKoszyka, Stream stumien = null )
        {
            odpKoszyka = null;
            List<Komunikat> bledy;
            ImportBaza pasujacy = PobierzDostepneModuly().First(x => x.GetType() == modul);
            List<PozycjaKoszykaImportowana> znalezione;
            try
            {
                znalezione = pasujacy.Przetworz(dane, out bledy, stumien);
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                Komunikat tmp = new Komunikat("Nie udało się odczytać pliku", KomunikatRodzaj.danger, "MenagerImportow");
                return new List<Komunikat> {tmp};

            }

            //wolimy cale pobranie koszyka zrobic zeby byc pewnym do czego bedziemy dodawać
            // IKoszykiBLL koszyk = SolexBllCalosc.PobierzInstancje.Koszyk.PobierzKoszykWgTypu(klient, TypKoszyka.Koszyk);

            List<KoszykPozycje> kp = new List<KoszykPozycje>();
            foreach (PozycjaKoszykaImportowana pozycjaKoszykaImportowana in znalezione)
            {
                KoszykPozycje tmp = new KoszykPozycje();
                tmp.ProduktId = pozycjaKoszykaImportowana.Produkt;
                tmp.Ilosc = pozycjaKoszykaImportowana.Ilosc;
                if (pozycjaKoszykaImportowana.Jednostki == null || pozycjaKoszykaImportowana.Jednostki.Count == 0)
                {
                    continue;
                }
                JednostkaProduktu j =
                    pozycjaKoszykaImportowana.Jednostki.FirstOrDefault(
                        x => x.Nazwa == pozycjaKoszykaImportowana.Jednostka) ??
                    pozycjaKoszykaImportowana.Jednostki.First(x => x.Podstawowa);
                tmp.JednostkaId = j.Id;
                tmp.KoszykId = koszyk.Id;
                tmp.ProduktBazowyId = tmp.ProduktId;
                kp.Add(tmp);
            }

            List<IKoszykPozycja> przekroczone, limity, zmienione, dodanepozycje;
            List<IProduktKlienta> nowe;
            List<Komunikat> komunikat = new List<Komunikat>();
            IKoszykiBLL zmieniony =
                SolexBllCalosc.PobierzInstancje.Koszyk.ZmienPozycjeKoszyka(kp, klient, out przekroczone, out limity,
                    out nowe, out zmienione, out dodanepozycje, koszyk) ??
                SolexBllCalosc.PobierzInstancje.DostepDane.PobierzPojedynczy<KoszykBll>(koszyk.Id, klient);
            odpKoszyka = SolexBllCalosc.PobierzInstancje.Koszyk.WygenerujKomunikaty(klient.JezykId, koszyk, przekroczone, limity, nowe, zmienione, dodanepozycje, klient);

            foreach (PozycjaKoszykaImportowana p in znalezione)
            {
                if (zmienione.Any(x => x.ProduktId == p.Id) || limity.Any(x => x.ProduktId == p.Id))
                {
                    var pr = zmienione.FirstOrDefault(x => x.ProduktId == p.Id) ??
                             limity.FirstOrDefault(x => x.ProduktId == p.Id);


                    if (pr != null)
                    {
                        Komunikat tmp =
                            new Komunikat(
                                string.Format(
                                    SolexBllCalosc.PobierzInstancje.Konfiguracja.PobierzTlumaczenie(klient.JezykId,
                                        "Zmieniono ilośc dla produktu: {0}"), pr.Produkt.Nazwa),
                                KomunikatRodzaj.warning, "MenagerImportow");
                        komunikat.Add(tmp);
                    }
                }
                else
                {
                    var pr = zmieniony.PobierzPozycje.FirstOrDefault(x => x.ProduktId == p.Id);

                    if (pr == null)
                    {
                        Komunikat tmp =
                            new Komunikat(
                                string.Format(
                                    SolexBllCalosc.PobierzInstancje.Konfiguracja.PobierzTlumaczenie(klient.JezykId,
                                        "Pominięto produkt") + " {0}, " +
                                    SolexBllCalosc.PobierzInstancje.Konfiguracja.PobierzTlumaczenie(klient.JezykId,
                                        "jest on obecnie niedostępny"), p.Nazwa),
                                KomunikatRodzaj.danger, "MenagerImportowPominieto");
                        komunikat.Add(tmp);
                    }
                    else
                    {
                        Komunikat tmp =
                            new Komunikat(
                                string.Format(
                                    SolexBllCalosc.PobierzInstancje.Konfiguracja.PobierzTlumaczenie(klient.JezykId,
                                        "Dodano produkt") + " {0}",
                                    pr.Produkt.Nazwa), KomunikatRodzaj.success, "MenagerImportowDodano");
                        komunikat.Add(tmp);
                    }
                }
            }
            komunikat.AddRange(bledy);
            return komunikat;
        }
    }

}
    