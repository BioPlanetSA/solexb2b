using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using InsERT;
using log4net;
using ServiceStack.OrmLite;
using SolEx.ERP.Model;


namespace SolEx.ERP.SubiektGT
{
    public enum SubiektGTDokumentEnum
    {
        gtaSubiektDokumentZWZ = -47,
        gtaSubiektDokumentZWn = -46,
        gtaSubiektDokumentWZv = -45,
        gtaSubiektDokumentWZa = -44,
        gtaSubiektDokumentPZv = -43,
        gtaSubiektDokumentPZa = -42,
        gtaSubiektDokumentPAk = -41,
        gtaSubiektDokumentPAf = -40,
        gtaSubiektDokumentKFZn = -39,
        gtaSubiektDokumentKFZ = -38,
        gtaSubiektDokumentKFSn = -37,
        gtaSubiektDokumentFZz = -36,
        gtaSubiektDokumentFZr = -35,
        gtaSubiektDokumentRR = -35,
        gtaSubiektDokumentFSzal = -34,
        gtaSubiektDokumentFSz = -33,
        gtaSubiektDokumentFSd = -32,
        gtaSubiektDokumentZW = -31,
        gtaSubiektDokumentZPZ = -30,
        gtaSubiektDokumentRZ = -29,
        gtaSubiektDokumentRS = -28,
        gtaSubiektDokumentMM = -27,
        gtaSubiektDokumentKFS = -26,
        gtaSubiektDokumentIW = -25,
        gtaSubiektDokumentPAi = -16,
        gtaSubiektDokumentPA = -9,
        gtaSubiektDokumentZK = -8,
        gtaSubiektDokumentZD = -7,
        gtaSubiektDokumentRW = -6,
        gtaSubiektDokumentPW = -5,
        gtaSubiektDokumentWZ = -4,
        gtaSubiektDokumentPZ = -3,
        gtaSubiektDokumentFS = -2,
        gtaSubiektDokumentFZ = -1,
        gtaSubiektDokumentDowolny = 0,
    }

    public static class Dokumenty
    {
        private static  ILog Log
        {
            get
            {
                return LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
            }
        }
        public static int? PobierzIdWgPodtytulu(string podtytul)
        {
            const string SQL_SELECT = "SELECT top 1 dok_Id FROM dok__Dokument where dok_Podtytul='{0}'";
            int? idGrupy = SolEx.DBHelper.Baza.db.Scalar<int?>(string.Format(SQL_SELECT, podtytul));
            return idGrupy;
        }
        public static string PobierzNazweWgId(int id)
        {
            const string SQL_SELECT = "SELECT top 1 dok_nrpelny FROM dok__Dokument where dok_id={0}";
            string idGrupy = SolEx.DBHelper.Baza.db.Scalar<string>(string.Format(SQL_SELECT, id));
            return idGrupy;
        }
        public static string DodajDokument(Subiekt subiekt, SubiektGTDokumentEnum typDokumentu, int idMagazynu, List<Produkt> produkty, string nr, int kontrahentId, string uwagiOpcjonalne, bool dodawajProduktyTylkoJesliWCalosciNaMagazynie, int? numerCeny)
        {
            string braki;
            bool duble = false;
            return DodajDokument(subiekt, typDokumentu, idMagazynu, produkty, nr, kontrahentId, uwagiOpcjonalne, dodawajProduktyTylkoJesliWCalosciNaMagazynie, numerCeny, out braki, duble);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="subiekt"></param>
        /// <param name="typDokumentu"></param>
        /// <param name="produkty"></param>
        /// <param name="nr">numer dokumentu oryginalu</param>
        /// <param name="kontrahentId"></param>
        /// <param name="idMagazynu">id magazynu na jaki zostanie zapisany dokument</param>
        /// <param name="uwagiOpcjonalne">opcjonalne uwagi do dokumentu</param>
        /// <param name="dodawajProduktyTylkoJesliWCalosciNaMagazynie">Jesli TRUE to nie dodaje produktów ktorych czesciowo nie ma</param>
        /// <param name="numerCeny">Numer ceny domyślnej produktów - wartość może być null.</param>
        /// <param name="braki"></param>
        public static string DodajDokument(Subiekt subiekt,SubiektGTDokumentEnum typDokumentu, int idMagazynu, List<Produkt> produkty, string nr, int kontrahentId, string uwagiOpcjonalne, bool dodawajProduktyTylkoJesliWCalosciNaMagazynie, int? numerCeny,out string braki,bool bezdubli)
        {
            string numerDokumentu = "";
            braki = "";
            SuDokument dok = null;
            try
            {
                if (bezdubli)
                {
                    Log.InfoFormat("Szukanie dublu dokumentu na podstawie podtytulu {0}", nr);
                    int? id = PobierzIdWgPodtytulu(nr);
                    if (id.HasValue)
                    {
                     
                        string nazwa = PobierzNazweWgId(id.Value);
                        Log.InfoFormat("Ponowba próba zapisu dokumentu, nazwa {0}",nazwa);
                        return string.Format("Wcześnie wystawiono już odpowiedni nr {0} - szukanie na podstawie podtytulu {1}", nazwa,nr);
                    }
                }

                subiekt.MagazynId = idMagazynu;
                dok = (SuDokument)subiekt.Dokumenty.Dodaj(typDokumentu);
                if (numerCeny.HasValue)
                {
                    dok.PoziomCenyId = numerCeny;
                }
                int magazynZrodlowy = 0;
                
                if (typDokumentu == SubiektGTDokumentEnum.gtaSubiektDokumentMM)
                {
                    magazynZrodlowy = kontrahentId;
                    if (idMagazynu == magazynZrodlowy)
                        return "";
                    dok.MagazynNadawczyId = magazynZrodlowy;
                    dok.MagazynOdbiorczyId = idMagazynu;
                }
                else
                {
                    var klient = subiekt.KontrahenciManager.WczytajKontrahenta(kontrahentId);
                    dok.KontrahentId = (int)klient.Identyfikator;
                    dok.OdbiorcaId = (int)klient.Identyfikator;
                }

                if (!string.IsNullOrEmpty(nr))
                {
                    try
                    {
                        dok.Podtytul = nr;
                    }
                    catch (Exception e)
                    {
                        Log.Info("Błąd " + e.Message);
                    }
                    try
                    {
                        dok.DoDokumentuNumerPelny = nr;
                    }
                    catch (Exception e)
                    {
                        Log.Info("Błąd " + e.Message);
                    }
                    try
                    {
                        dok.NumerOryginalny = nr;
                    }
                    catch (Exception e)
                    {
                        Log.Info("Błąd " + e.Message);
                    }
                }

                //dok.PozycjeBrakujace.PomijanieRezerwacji = true;
                foreach (Produkt product in produkty)
                {
                    var magQuanity = Magazyny.PobierzStanProduktu(product.Id,
                        typDokumentu == SubiektGTDokumentEnum.gtaSubiektDokumentMM ? magazynZrodlowy : idMagazynu,
                        true);

                    if (magQuanity < product.Ilosc)
                    {
                        if (dodawajProduktyTylkoJesliWCalosciNaMagazynie)
                        {
                            braki += string.Format("  {0} /{1}/", product.Symbol, product.Ilosc);
                            continue;
                        }
                        else
                        {
                            braki += string.Format("  {0} /{1}/", product.Symbol, product.Ilosc - magQuanity);
                            product.Ilosc = magQuanity;
                        }
                    }


                    if (product.Ilosc <= 0)
                    {
                        continue;
                    }

                    SuPozycja poz = (SuPozycja)dok.Pozycje.Dodaj(product.Id);

                    poz.IloscJm = product.Ilosc;
                    poz.Jm = product.PodstJmiary;
                }

                if (!string.IsNullOrEmpty(braki))
                {
                    braki = "  BRAKI: " + braki;
                }
                if (dok.Pozycje.Liczba > 0)
                {
                    dok.Uwagi = uwagiOpcjonalne + " " + braki;
                    
                    //dok.Wyswietl();
                    
                    dok.Zapisz();
                    numerDokumentu = dok.NumerPelny.ToString();

                    dok.Zamknij();
                }
            }
            catch (Exception e)
            {
                throw new Exception("Nie udało się zapisać dokumentu w Subiekcie", e);
            }
            Log.InfoFormat("Wystawiono nowy dokument, nazwa {0}", numerDokumentu);
            return (numerDokumentu + braki).Trim();
        }
    }

}