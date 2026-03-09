using System;
using System.Collections.Generic;
using SolEx.Hurt.Core.Helper;
using SolEx.Hurt.Core.ModelBLL;
using SolEx.Hurt.Model;
using SolEx.Hurt.Model.AtrybutyKlas;
using SolEx.Hurt.Model.Helpers;
using SolEx.Hurt.Model.Interfaces.Sync;
using SolEx.Hurt.Model.Interfaces.SyncModuly;

namespace SolEx.Hurt.Sync.App.Modules_.NoweModuly.WyliczenieGotowychCen
{
    class PrzelicznieWyliczonychCenNaWybranaWalute : SyncModul, IModulWyliczanieGotowychCen
    {
        public override string uwagi
        {
            get { return "Przelicza wyliczone ceny na inną walutę. Moduł księgowy musi dziedziczyć po IPobieranieKursuWaluty"; }
        }
        [FriendlyName("Waluta podstawowa programu księgowego")]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        [PobieranieSlownika(typeof(SlownikWalut))]
        public long WalutaPodstawowa { get; set; }
        
        [FriendlyName("Waluta zródłowa z której przeliczamy")]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        [PobieranieSlownika(typeof(SlownikWalut))]
        public long WalutaZrodlowa { get; set; }
        
        [FriendlyName("Waluta docelowa")]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        [PobieranieSlownika(typeof(SlownikWalut))]
        public long WalutaDocelowa { get; set; }
        
        public void Przetworz(ref List<FlatCeny> wynik, Dictionary<long, Klient> dlaKogoLiczyc, ISyncProvider aktualnyProvider)
        {
            IPobieranieKursuWaluty pro = aktualnyProvider as IPobieranieKursuWaluty;
            if (pro == null)
            {
                throw new Exception("Moduł księgowy nie dziedziczy pod IPobieranieKursuWaluty");
            }
            var walutyWSystemie = ApiWywolanie.PobierzWaluty();

            var zrodlowa = walutyWSystemie[WalutaZrodlowa].WalutaErp;
            var docelowa = walutyWSystemie[WalutaDocelowa].WalutaErp;
            var podstawowa = walutyWSystemie[WalutaPodstawowa].WalutaErp;

            decimal kurs = pro.PobierzKursWaluty(zrodlowa);
            if (!zrodlowa.Equals(podstawowa, StringComparison.InvariantCultureIgnoreCase))
            {
                decimal kursd = pro.PobierzKursWaluty(docelowa);
                kurs = kurs*(1M/kursd);
            }
            foreach (var cena in wynik)
            {
                var walutaKlienta = walutyWSystemie[dlaKogoLiczyc[cena.KlientId].WalutaId].WalutaErp;
                if (walutaKlienta.Equals(zrodlowa, StringComparison.InvariantCultureIgnoreCase))
                {
                    continue;
                }                    

                if (cena.WalutaId==WalutaZrodlowa)
                {
                    cena.WalutaId = WalutaDocelowa;
                    cena.CenaHurtowaNetto *= kurs;
                    cena.CenaNetto*= kurs;
                }
            }
        }
    }
}
