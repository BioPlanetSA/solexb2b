using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using SolEx.Hurt.Core.ModelBLL;
using SolEx.Hurt.Model;
using SolEx.Hurt.Model.AtrybutyKlas;
using SolEx.Hurt.Model.Enums;
using SolEx.Hurt.Model.Web;

namespace SolEx.Hurt.Sync.App.Modules_.NoweModuly.Dokumenty
{
    public class AktywacjaKlientowNaPodstawieWartosciFaktur : SyncModul, Model.Interfaces.SyncModuly.IModulDokumenty
    {
        public override string Opis
        {
            get { return "Deaktywacja klientów którzy w ostatnim czasie nie mieli faktur na określoną kwotę."; }
        }
        [FriendlyName("Co ile dni moduł ma być uruchamiany")]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        public int CoIleDni { get; set; }
        
        [FriendlyName("W ciągu ilu ostatnich dni musiały być stworzone dokumenty")]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        public int IleDniPrzed { get; set; }
        
        [FriendlyName("Minimalna suma wartości netto faktur")]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        public decimal MinimalnaWartosc { get; set; }
        public void Przetworz(ref ConcurrentDictionary<HistoriaDokumentu, List<HistoriaDokumentuProdukt>> listaWejsciowa, ref List<StatusZamowienia> statusy, Dictionary<int, long> hashe, ref List<Klient> klienci)
        {
            //if (!UruchamiacModul)
            //{
            //    return;
            //}
            //var data = DateTime.Now.Date.AddDays(-IleDniPrzed);
            ////var hashewgklienta = hashe.Values.Where(x => x.Rodzaj == RodzajDokumentu.Faktura && x.DataWystawienia >= data).GroupBy(x => x.PlatnikId).ToDictionary(x => x.Key, x => x.ToList());
            //foreach (var k in klienci)
            //{
            //    if (k.Role.Contains(RoleType.Administrator))
            //    {
            //        continue;
            //    }
            //    if (k.Id < 0)
            //    {
            //        continue;
            //    }
            //    bool blokowac = true;
            //    if (hashewgklienta.ContainsKey(k.Id))
            //    {
            //        decimal suma = hashewgklienta[k.Id].Sum(x => x.WartoscNetto);
            //        if (suma >= MinimalnaWartosc)
            //        {
            //            blokowac = false;
            //        }
            //    }
            //    if (blokowac)
            //    {
            //        k.PowodBlokady=BlokadaPowod.BrakFaktur;

            //    }
            //}
            //File.WriteAllText(SciezkaDoPliku,DateTime.Now.ToString(CultureInfo.InvariantCulture));

        }

        public string SciezkaDoPliku
        {
            get { return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, nazwaPliku); }
        }
        public bool ZnacznikIstnieje
        {
            get { return File.Exists(SciezkaDoPliku); }
        }
        public DateTime? DataOstatniegoUruchomienia
        {
            get
            {
                if (ZnacznikIstnieje)
                {
                    return File.GetLastWriteTime(SciezkaDoPliku);
                }
                return null;
            }
        }
        private string nazwaPliku = "AktywacjaKlientowNaPodstawieWartosciFaktur-znacznik.txt";
        public override bool UruchamiacModul
        {
            get
            {
                DateTime datauruchomienia = DateTime.Now.AddDays(-CoIleDni);
                return !ZnacznikIstnieje || !DataOstatniegoUruchomienia.HasValue || DataOstatniegoUruchomienia.Value < datauruchomienia;

            }
        }


    }

}
