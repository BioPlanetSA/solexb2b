using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using InsERT;
using log4net;
using SolEx.ERP.SubiektGT;
using SolEx.Hurt.Model;
using SolEx.Hurt.Model.Enums;
using SolEx.Hurt.Helpers;
using SolEx.Hurt.Sync.ProviderSubiekt;

namespace SolEx.Hurt.Sync.ProviderNavireo
{
    public class Provider : ProviderSubiekt.Provider
    {
        private MainDataContext _db = null;
        private MainDataContext DB
        {
            get
            {
                if (_db == null)
                {
                    _db = new MainDataContext(_config.ERPcs);

                }
                return _db;
            }
        }
        
        private Navireo nav;
        private static ILog log => LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public Navireo GetNavireo(string agentName, string agentPassword, string cs)
        {
            if (nav == null)
            {
                try
                {
                    Polaczenie.UstawParametryPolaczenia(agentName, agentPassword, cs, _config.SubiektPodmiot, _config.SubiektSzyfrujHaslo);
                    nav = Polaczenie.GetNavireo;
                }
                catch (COMException ex)
                {
                    uint kod = (uint)ex.ErrorCode;
                    switch (kod)
                    {
                        case 2147750699:
                            throw new SaveOrderException($"Nie poprawne dane użyte do logowania, aktualne login {agentName} hasło {agentPassword} ");
                        case 2147750498:
                            throw new SaveOrderException("Licencja sfery wygasła");
                        case 2147750386:
                            throw new SaveOrderException("Brak ważnej licencji Sfery i/lub stowarzyszonego produktu w podmiocie.");
                        case 2147750590:
                            throw new SaveOrderException("Zostało osiągnięte ograniczenie licencji Sfery na liczbę stanowisk.");
                        case 2147750646:
                            throw new SaveOrderException("Osiągnięty limit aktywnych pracowników. Rozszerz licencję.");
                        case 2147750869:
                            throw new SaveOrderException("Nie można zalogować wybranego użytkownika, ponieważ został przekroczony limit wykupionych licencji.");
                        default:
                            throw;
                    }
                }
                catch (Exception ex)
                {
                    throw new SaveOrderException("Inicjalizacja sfery: " + ex.Message + $" Dane użyte do łączenia: {agentName}, {agentPassword}", null, ex);
                }
            }
            return nav;
        }


        public override bool DrukujPdfDokument(StatusDokumentuPDF id, ref string sciezka)
        {
            if (string.IsNullOrEmpty(_config.ERPLogin) || string.IsNullOrEmpty(_config.ERPcs))
                throw new Exception("Niepoprawne parametry konfiguracyjne Subiekta");
            Navireo navireo;
            try
            {
                navireo = GetNavireo(_config.ERPLogin, _config.ERPHaslo, _config.ERPcs);
            }
            catch (Exception ex)
            {
                throw new SaveOrderException("Inicjalizacja sfery: " + ex.Message, null, ex);
            }
            //var a = navireo.SuDokumentyManager.WczytajDokument(id);


            SuDokument toPrintDoc = navireo.SuDokumentyManager.WczytajDokument(id.IdDokumentu);
            if (toPrintDoc != null)
            {
                try
                {
                    toPrintDoc.DrukujDoPliku(sciezka, TypPlikuEnum.gtaTypPlikuPDF);
                }
                catch (COMException ex)
                {
                    string komunikat = string.Format("Dokument {0} Nie poprawny szablon wydruku. Spróbuj wyeksportować pdf bezpośrednio z programu. ", toPrintDoc.NumerPelny);
                    if (ex.HResult == 0x800416FB)
                    {
                        komunikat += " Prawdopodobnie niepoprawna czcionka w szablonie wydruku";
                    }
                    throw new Exception(komunikat);
                }

            }
            else
            {
                throw new InvalidOperationException($"Próba drukowania nieistniejącego dokumentu: {id}");
            }
            if (File.Exists(sciezka))
            {
                return true;
            }
            string nazwa = Path.GetFileNameWithoutExtension(sciezka);
            if (string.IsNullOrEmpty(nazwa))
            {
                return false;
            }
            string katalog = Path.GetDirectoryName(sciezka);
            if (string.IsNullOrEmpty(katalog))
            {
                return false;
            }
            sciezka = Directory.GetFiles(katalog).FirstOrDefault(x => Path.GetFileNameWithoutExtension(x).StartsWith(nazwa, StringComparison.InvariantCultureIgnoreCase));
            if (sciezka == null)
            {
                return false;
            }
            return true;
        }

        public override List<ZamowienieSynchronizacja> ImportZamowien(List<ZamowienieSynchronizacja> list, Dictionary<long, Klient> wszyscy)
        {
            if (string.IsNullOrEmpty(_config.ERPLogin))
                throw new Exception("Pusty ERPLogin");
            if (string.IsNullOrEmpty(_config.ERPcs))
                throw new Exception("Pusty ERPcs");


            var waluty = PobierzDostepneWaluty();
            List<ZamowienieSynchronizacja> docs = new List<Model.ZamowienieSynchronizacja>();
            bool liczonyOdNetto = _config.LiczonyOdCenyNetto;
            Navireo navireo = null;
            try
            {
                navireo = GetNavireo(_config.ERPLogin, _config.ERPHaslo, _config.ERPcs);
            }
            catch (Exception ex)
            {
                foreach (ZamowienieSynchronizacja t in list)
                {
                    t.StatusId = StatusImportuZamowieniaDoErp.Błąd;
                    t.BladKomunikat = ex.Message;
                    docs.Add(t);
                }
                throw new SaveOrderException("Inicjalizacja sfery " + ex.Message, null, ex);
            }
            foreach (ZamowienieSynchronizacja o in list)
            {
                //var o1 = o;
                //var wal = PobierzDostepneWaluty().FirstOrDefault(x => x.Key == o1.WalutaId);
                SuDokument dokument2 = null;
                try
                {
                    log.Debug("Rozpoczynanie importu zamówienia o ID " + o.NumerZPlatformy);
                    Kontrahent knt = navireo.KontrahenciManager.Wczytaj(o.KlientId);
                    if (knt == null || !knt.Aktywny)
                    {
                        o.StatusId = StatusImportuZamowieniaDoErp.Błąd;//_config.StatusyZamowien.Values.First(p => p.Symbol == "Błąd").Id;
                        o.BladKomunikat = "Klient nie znaleziony lub nieaktywny. Pomijanie zamówienia B2B nr " + o.NumerZPlatformy;
                        continue;
                    }
                    int magazyn = 0;
                    if (!string.IsNullOrEmpty(o.MagazynRealizujacy))
                    {
                        magazyn = DB.sl_Magazyns.First(p => o.MagazynRealizujacy == p.mag_Symbol).mag_Id;
                    }
                    else
                    {
                        magazyn = DB.sl_Magazyns.First(p => p.mag_Glowny == true).mag_Id;
                    }
                    navireo.MagazynId = magazyn;
                    dokument2 = navireo.SuDokumentyManager.DodajZK();
                    dokument2.KontrahentId = o.KlientId;
                    dokument2.OdbiorcaId = o.KlientId;
                    Klient k = wszyscy[o.KlientId];
                    if (o.TerminDostawy != null)
                    {
                        dokument2.TerminRealizacji = o.TerminDostawy;
                    }
                    dokument2.LiczonyOdCenNetto = liczonyOdNetto;
                    dokument2.DoDokumentuNumerPelny = o.NumerZPlatformy;
                    dokument2.NumerOryginalny = o.NumerZPlatformy;

                    int? idKategorii = GetCategoryId(o.KategoriaZamowienia);
                    if (idKategorii.HasValue)
                    {
                        dokument2.KategoriaId = idKategorii;
                    }
                    string desc = (o.Uwagi ?? "").UsunFormatowanieHTML();
                    try
                    {
                        dokument2.Uwagi = (desc.Length > 490) ? desc.Substring(0, 490) : desc;
                    }
                    catch
                    {
                        log.ErrorFormat($"Błąd zapisania uwag. Próbowano wpisać uwagi: {desc}");
                        throw;
                    }
                    dokument2.Tytul = "Zamówienie z platformy B2B";
                    dokument2.Podtytul = o.Podtytul;
                    if (k.KlientEu)
                    {
                        dokument2.TransakcjaRodzajOperacjiVat = 2;
                        if (_config.SubiektKodTransakcjiDlaKlientowEU > -1)
                        {
                            dokument2.TransakcjaKod = _config.SubiektKodTransakcjiDlaKlientowEU;
                        }
                    }
                    else if (!k.KlientEu && k.Eksport)
                    {
                        dokument2.TransakcjaRodzajOperacjiVat = 1;
                    }
                    if (o.PoziomCenyId != null)
                        dokument2.PoziomCenyId = o.PoziomCenyId.Value;

                    dokument2.WalutaSymbol = waluty[(long) o.WalutaId].WalutaErp;
                    //BARTEK zmienil ze domyslnie jest PLN - bedzie prblem jak ktos nie ma domyslnie PLN, ale takich to chyba nie ma?
                    if (o.WalutaId != null && waluty[(long) o.WalutaId].WalutaErp.ToUpper() != "PLN")
                    {
                        decimal kurs = 1;
                        DateTime? dataKursu= null;
                        var x = DB.sl_WalutaKurs.OrderByDescending(p => p.wk_Data).FirstOrDefault(p => p.wk_Symbol == waluty[(long) o.WalutaId].WalutaErp);
                        if (x != null)
                        {
                            kurs = x.wk_Sredni;
                            dataKursu = x.wk_Data;
                        }
                        //  dokument2.WalutaSymbol = waluty[(long)o.WalutaId].WalutaErp;
                        dokument2.KursCeny = kurs; //kurs > kurs0 ? 1 / kurs : kurs0;
                        dokument2.WalutaKurs = kurs; // dokument2.KursCeny;

                        //do tego by poprawnie można było zrealizować fakturę w walucie nalezy uzupełnić poniższe rzeczy 

                        int idBankuKursu = DB.ExecuteQuery<int>("SELECT TOP 1 wbn_ID FROM sl_WalutaBank WHERE [wbn_Podstawowy] = 1").FirstOrDefault();
                        dokument2.KursCenyDataKursu = dataKursu;
                        dokument2.KursCenyTabelaBanku = idBankuKursu;
                        dokument2.WalutaDataKursu = dataKursu;
                        dokument2.WalutaTabelaBanku = idBankuKursu;
                    }

                    foreach (ZamowienieProdukt i in o.pozycje)
                    {
                        SuPozycja powiazany = null;
                        log.Debug($"Wczytywanie towaru o id: {i.ProduktIdBazowy}");
                        var produkt = navireo.TowaryManager.WczytajTowar(i.ProduktIdBazowy);
                        if (!produkt.Aktywny)
                            continue;
                        SuPozycja pozycja2 = (SuPozycja) dokument2.Pozycje.Dodaj(i.ProduktIdBazowy);
                        if (pozycja2.TowarId != i.ProduktIdBazowy) //produkt miał towar poziwązany, rzeczywisty produkt jest na przedostatniej pozycji
                        {
                            powiazany = pozycja2;
                            pozycja2 = dokument2.Pozycje[dokument2.Pozycje.Liczba - 1];
                        }

                        pozycja2.Jm = i.Jednostka;
                        pozycja2.IloscJm = i.Ilosc;
                        pozycja2.Opis = i.Opis;
                        if (i.CenaNetto != -1)
                        {
                            if (liczonyOdNetto)
                            {
                                if (pozycja2.CenaNettoPrzedRabatem == 0)
                                {
                                    pozycja2.CenaNettoPrzedRabatem = i.CenaNetto;
                                }
                                pozycja2.CenaNettoPoRabacie = i.CenaNetto;
                            }
                            else
                            {
                                if (pozycja2.CenaBruttoPrzedRabatem == 0)
                                {
                                    pozycja2.CenaBruttoPrzedRabatem = i.CenaNetto;
                                }
                                pozycja2.CenaBruttoPoRabacie = i.CenaBrutto;
                            }
                        }
                        if (i.CenaNetto != -1)
                        {
                            if (liczonyOdNetto)
                            {
                                pozycja2.WartoscNettoPoRabacie = i.PozycjaDokumentuWartoscNetto.Wartosc;
                            }
                            else
                            {
                                pozycja2.WartoscBruttoPoRabacie = i.PozycjaDokumentuWartoscBrutto.Wartosc;
                            }
                        }
                        if (powiazany != null)
                        {
                            powiazany.RabatProcent = pozycja2.RabatProcent;
                        }
                        try
                        {
                            //log.InfoFormat("Próba dodania pola własnego REZ do pozycji:{0}.", pozycja2.TowarNazwa);
                            pozycja2.PoleWlasne["REZ"] = -1;
                            //log.InfoFormat("Pole własnego REZ dla pozycji:{0}. Wartość :{1}", pozycja2.TowarNazwa, pozycja2.PoleWlasne["REZ"]);
                        }
                        catch (Exception ex)
                        {
                            log.Error($"Bład przy dodawaniu pola własnego do pozycji: {ex.Message}:{ex.StackTrace}");
                        }

                        //Marshal.ReleaseComObject(pozycja2);
                    }
                    dokument2.StatusDokumentu = (StatusZdarzeniaEnum) _config.SubiektStatusDokumentu;
                    dokument2.Rezerwacja = _config.ZamowieniaTworzRezerwacje;
                    string flaga="";
                    //dodawanie wartości do pola własnego zamówienia
                    if (!string.IsNullOrEmpty(o.DodatkowePola))
                    {
                        var pola = o.DodatkowePola.Split(';');
                        foreach (var a in pola)
                        {
                            if (string.IsNullOrEmpty(a))
                            {
                                continue;
                            }
                            var pole = a.Split(':');
                            var nazwa = pole.First();
                            var wartosc = pole.Last();
                            try
                            {
                                if (nazwa.Equals("FLAGA", StringComparison.InvariantCultureIgnoreCase))
                                {
                                   flaga = wartosc;
                                }
                                else
                                {
                                    dokument2.PoleWlasne[nazwa] = wartosc;
                                }
                            }
                            catch (Exception ex)
                            {
                                throw new Exception($"Błąd ustawienia parametru pola własnego o nazwie: {nazwa} lub flagi:{wartosc}, error: {ex.Message}");
                            }
                        }
                    }
                  
                    try
                    {
                        //dokument2.Wyswietl();
                        dokument2.Zapisz();
                        //string json = new JavaScriptSerializer().Serialize(dokument2);
                       // log.Debug($"{json}");
                    }
                    catch (Exception ex)
                    {
                        throw new Exception($"Błąd przy zapisie zamówienia: {ex.Message}");
                    }

                    o.StatusId = StatusImportuZamowieniaDoErp.Zaimportowane;
                    try
                    {
                        o.ListaDokumentowZamowienia = new List<ZamowienieDokumenty>() {new ZamowienieDokumenty(o.Id, dokument2.Identyfikator, dokument2.NumerPelny)};
                    }
                    catch(Exception ex)
                    {
                        string nrPelny;
                        int idZam = PobierzDaneOZamowieniu(o.NumerZPlatformy, out nrPelny);
                        if (idZam <= 0 || string.IsNullOrEmpty(nrPelny)) throw ex;

                        o.ListaDokumentowZamowienia = new List<ZamowienieDokumenty>() { new ZamowienieDokumenty(o.Id, idZam, nrPelny) };
                        if (!string.IsNullOrEmpty(flaga))
                        {
                            log.Debug($"Ustawianie flagi:{flaga} na dokumencie.");
                            dokument2 = navireo.SuDokumentyManager.WczytajDokument(idZam);
                            dokument2.FlagaNazwa = flaga;
                            dokument2.Zapisz();
                        }
                    }
                }
                catch (Exception ex)
                {
                    log.Error(ex);
                    o.StatusId = StatusImportuZamowieniaDoErp.Błąd;
                    o.BladKomunikat = ex.Message;
                }
                finally
                {
                    if (dokument2 != null)
                    {
                        //log.Debug("Zakończono import zamówienia o ID " + o.NumerZPlatformy + " Zamowienie ma numer " + dokument2.NumerPelny);
                        log.Debug("Zakończono import zamówienia o ID " + o.NumerZPlatformy );
                        try
                        {
                        dokument2.Zamknij();
                        }
                        catch { }
                        Marshal.ReleaseComObject(dokument2);
                    }
                }

                docs.Add(o);
            }
            return docs;
        }

        private int PobierzDaneOZamowieniu(string nrZamZB2b, out string nrOrginalny)
        {
            int? idZam = null;
            nrOrginalny = "";
            string sql = $"SELECT [dok_Id],[dok_NrPelny] FROM [dok__Dokument] WHERE [dok_NrPelnyOryg] like '%{nrZamZB2b}%'";
            SqlConnection conn = null;
            SqlCommand cmd = null;
            SqlDataReader r = null;

            try
            {
                conn = new SqlConnection(_config.ERPcs);
                conn.Open();
                cmd = new SqlCommand(sql, conn);
                r = cmd.ExecuteReader();
                while (r.Read())
                {
                    idZam = DataHelper.dbi("dok_Id", r);
                    nrOrginalny = DataHelper.dbs("dok_NrPelny", r).Trim();
                }
            }
            finally
            {
                r?.Dispose();
                cmd?.Dispose();
                if (conn != null)
                {
                    conn.Close();
                    conn.Dispose();
                }
            }
            if (!idZam.HasValue)
            {
                throw new Exception($"Brak zamówienia: {nrZamZB2b} w progranie księgowym. ");
            }
            return idZam.Value;
        }

        private int? GetCategoryId(string kategoria)
        {

            var x = DB.sl_Kategorias.FirstOrDefault(p => p.kat_Nazwa == kategoria);
            return x?.kat_Id;
        }
    }
}
