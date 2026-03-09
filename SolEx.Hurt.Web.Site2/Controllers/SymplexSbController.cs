using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Text;
using ServiceStack.ServiceHost;
using SolEx.Hurt.Core;
using SolEx.Hurt.Core.BLL;
using SolEx.Hurt.Core.ModelBLL.Interfejsy;
using SolEx.Hurt.Model;
using SolEx.Hurt.Model.Enums;
using SolEx.Hurt.Model.Interfaces;
using SolEx.Hurt.Web.Site2.PageBases;

namespace SolEx.Hurt.Web.Site2.Controllers
{
    public class SymplexSbController : SolexControler
    {
        [Route("SymplexSB/edi_order.php")]
        public string PobierzZamowienia(string AuthKey)
        {
            string key = ConfigurationManager.AppSettings["api_token"];
            if (AuthKey != key)
            {
                Log.Error(string.Format("Błędny klucz"));
                return "Błędny klucz";
            }
            var statusImportu = SolexBllCalosc.PobierzInstancje.Konfiguracja.StatusyZamowien.Values.Where(p => p.Importowac).Select(p => (StatusImportuZamowieniaDoErp)p.Id).ToList();
            Dictionary<int, ZamowieniaBLL> oczekujaceZamowienia = SolexBllCalosc.PobierzInstancje.DostepDane.Pobierz<ZamowieniaBLL>(null).
                Where(x =>
                {
                    var statusId = x.StatusId;
                    return statusImportu.Contains(statusId);
                }).ToDictionary(x => x.Id, x => x);
            if (!oczekujaceZamowienia.Any())
            {
                Log.Info("Brak zamówień. ");
                return "Brak Zamówień" ;
            }
            StringBuilder zamowieniaFile = new StringBuilder();

            foreach (ZamowieniaBLL zo in oczekujaceZamowienia.Values.Where(x=> statusImportu.Contains(x.StatusId)))
            {
                //var ceny_poziomu = SolexBllCalosc.PobierzInstancje.DostepDane.Pobierz<ceny_poziomy>(null);
                ZamowienieSynchronizacja z = new ZamowienieSynchronizacja(zo);
                z.Rozbijaj = true;
                long idKlienta = zo.Klient.Id;
                //int? klientID = zo.Klient.klient_id;
                IKlient k= zo.Klient;
                while (idKlienta < 0)
                {
                    
                    if (!k.KlientNadrzednyId.HasValue)
                    {
                        break;
                    }
                    idKlienta = k.KlientNadrzednyId.Value;
                }
                if (idKlienta < 0)
                {
                    continue;
                }
                zamowieniaFile.AppendLine("[Dokument]");
                zamowieniaFile.AppendLine("Rejestr=ZamowieniaSpr");
                zamowieniaFile.AppendLine("DataSprzed=" + zo.DataUtworzenia.ToString("yy.MM.dd"));
                zamowieniaFile.AppendLine("DataWyst=" + zo.DataUtworzenia.ToString("yy.MM.dd"));
                zamowieniaFile.AppendLine("NazwiskoOdbiorcy=" + k.Nazwa);
                zamowieniaFile.AppendLine("Godzina=" + zo.DataUtworzenia.ToString("HH:mm:ss"));
                zamowieniaFile.AppendLine("NrDok=" + z.NumerZPlatformy.Replace("  ", "/").Replace(" ", "/"));
                zamowieniaFile.AppendLine("NrKontrahenta=" + idKlienta);
                zamowieniaFile.AppendLine("Nip=" + k.Nip);
                zamowieniaFile.AppendLine("SymbolKontrahenta=" + k.Symbol);
                zamowieniaFile.AppendLine("SposobPlatnosci=" + zo.NazwaPlatnosci);
                zamowieniaFile.AppendLine("Uwagi=" + zo.GetUwagiText().Replace("\r\n", " | "));
                zamowieniaFile.AppendLine("[ZawartoscDokumentu]");

               int i = 1;
                z.pozycje = SolexBllCalosc.PobierzInstancje.DostepDane.Pobierz<ZamowieniaProduktyBLL>(null,x=>x.DokumentId==z.Id).Select(p => new ZamowienieProdukt(p)).ToList();
               
                foreach (var zp in z.pozycje)
                {
                    zamowieniaFile.AppendLine("[Poz" + (i++) + "]");
                    zamowieniaFile.AppendLine("Mag=1");
                    zamowieniaFile.AppendLine("Jm=" + zp.Jednostka);

                    zamowieniaFile.AppendLine("Ilosc=" + zp.Ilosc.ToString(CultureInfo.CreateSpecificCulture("en-US")));
                    zamowieniaFile.AppendLine("CenaNetto=" +
                                              zp.CenaNetto.ToString(CultureInfo.CreateSpecificCulture("en-US")));
                    zamowieniaFile.AppendLine("CenaBrutto=" +
                                              Math.Round(zp.CenaBrutto, 2)
                                                  .ToString(CultureInfo.CreateSpecificCulture("en-US")));
                    decimal wartoscNetto = Math.Round(zp.CenaNetto*zp.Ilosc, 2);
                    decimal wartoscBrutto = Math.Round(zp.CenaBrutto*zp.Ilosc, 2);
                    decimal wartoscVat = wartoscBrutto - wartoscNetto;
                    int vat = 0;
                    if (zp.CenaBrutto > 0 && zp.CenaNetto > 0)
                    {
                        vat = (int) Math.Round(((zp.CenaBrutto/zp.CenaNetto) - 1)*100);
                    }
                    zamowieniaFile.AppendLine("WartoscNetto=" +
                                              wartoscNetto.ToString(CultureInfo.CreateSpecificCulture("en-US")));
                    zamowieniaFile.AppendLine("WartoscBrutto=" +
                                              wartoscBrutto.ToString(CultureInfo.CreateSpecificCulture("en-US")));
                    zamowieniaFile.AppendLine("Vat=" + vat);
                    zamowieniaFile.AppendLine("WartoscVat=" +
                                              wartoscVat.ToString(CultureInfo.CreateSpecificCulture("en-US")));

                    
                    decimal cenaPrzedRabatemNetto = Math.Round(zp.CenaNetto, 2);
                    decimal rabat = 0;
                    var ceny_poziomu = SolexBllCalosc.PobierzInstancje.CenyPoziomy.PobierzCenyProduktu(zp.ProduktId);
                    if (ceny_poziomu!=null)
                    {
                        if (z.PoziomCenyId.HasValue)
                        {
                            var cena = ceny_poziomu.FirstOrDefault(x => x.Value.ProduktId == zp.ProduktId && x.Key == z.PoziomCenyId).Value;
                            if (cena !=null)
                            {
                                cenaPrzedRabatemNetto = Math.Round(cena.Netto, 2);
                            }
                            if (cenaPrzedRabatemNetto != 0)
                            {
                                rabat = 100 - (zp.CenaNetto/cenaPrzedRabatemNetto)*100M;
                                rabat -= k.Rabat;
                            }
                        }
                    }
                    rabat = Math.Round(rabat, 2);
                    decimal cenaPrzedRabatemBrutto = Math.Round(cenaPrzedRabatemNetto*(1.0M + (vat/100.0M)), 2);
                    zamowieniaFile.AppendLine("Rabat=" + rabat.ToString(CultureInfo.CreateSpecificCulture("en-US")));
                    zamowieniaFile.AppendLine("CenaPrzedRabatemNetto=" +
                                              cenaPrzedRabatemNetto.ToString(CultureInfo.CreateSpecificCulture("en-US")));
                    zamowieniaFile.AppendLine("CenaPrzedRabatemBrutto=" +
                                              cenaPrzedRabatemBrutto.ToString(CultureInfo.CreateSpecificCulture("en-US")));
                    zamowieniaFile.AppendLine("Nrtowaru=" + zp.ProduktId);
                    var produkt = SolexBllCalosc.PobierzInstancje.DostepDane.PobierzPojedynczy<ProduktKlienta>(zp.ProduktId,k.JezykId, k);
                    //SolexBllCalosc.PobierzInstancje.ProduktyKlienta.Pobierz(zp.produkt_id,k.JezykId,k);
                    if (produkt != null)
                    {
                        zamowieniaFile.AppendLine("Symbol=" + produkt.KodKreskowy);
                    }
                    else
                    {
                        throw new Exception("Nie można dopasować produktu - nie można zapisać zamówienia bez kodu kreskowego produktu");
                    }
                    

                }
               
                // z.status_id = _config.StatusyZamowien.Values.First(p => p.Symbol == "Zapisane").id;
            }
            
            return zamowieniaFile.ToString();
        }

        [Route("SymplexSB/change_status.php")]
        public void ZmienStatus(string id="",string s="", string idZam="", string nrZam="" ,string AuthKey="")
        {
           string key = ConfigurationManager.AppSettings["api_token"];
           if (string.IsNullOrEmpty(AuthKey) || AuthKey != key)
            {
                 Log.Info("błędny klucz logowania");
                return;
            }
            var statusImportu = SolexBllCalosc.PobierzInstancje.Konfiguracja.StatusyZamowien.Values.Where(p => p.Importowac).Select(p => p.Id).ToList();
            
            int idZamowienia = 0;
            id = id.Replace("/", " ").Replace("B2B", "");
            int.TryParse(id, out idZamowienia);
            if (idZamowienia == 0)
            {
                Log.Info("Brak id zamówienia");
                return;
            } 
            if (idZamowienia > 0) idZamowienia = -1 * idZamowienia;

            Dictionary<int, ZamowieniaBLL> zamowienia = SolexBllCalosc.PobierzInstancje.DostepDane.Pobierz<ZamowieniaBLL>(null).ToDictionary(x=>x.Id,x=>x);
            if (zamowienia.ContainsKey(idZamowienia))
            {
               var zamowienie = zamowienia[idZamowienia];
                if (statusImportu.Contains((int)zamowienie.StatusId))
                {
                    zamowienie.StatusId = StatusImportuZamowieniaDoErp.Zaimportowane;  //SolexBllCalosc.PobierzInstancje.Konfiguracja.StatusyZamowien.Values.First(p => p.Symbol == "Zapisane").Id;
                    int idDokumentu;
                    if (!string.IsNullOrEmpty(idZam) && int.TryParse(idZam, out idDokumentu))
                    {
                        var zamowienieDokument = new ZamowienieDokumenty(idZamowienia, idDokumentu, nrZam);
                        SolexBllCalosc.PobierzInstancje.DostepDane.AktualizujPojedynczy<ZamowienieDokumenty>(zamowienieDokument);
                    }
                    SolexBllCalosc.PobierzInstancje.ZamowieniaDostep.AktualizujZamowienia(zamowienie);
                    Log.Info(string.Format("Zmieniono status dla zamówienia: {0} ", zamowienie.Id));
                    return;
                }
            }
            Log.Info(string.Format("Brak zamówienia o id: {0} ", idZamowienia));
            return;
        }
    }
}