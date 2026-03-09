using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using SolEx.Hurt.Core.ModelBLL;
using SolEx.Hurt.Model;
using SolEx.Hurt.Model.AtrybutyKlas;
using SolEx.Hurt.Model.Enums;
using SolEx.Hurt.Model.Helpers;
using SolEx.Hurt.Model.Interfaces.Sync;
using SolEx.Hurt.Model.Interfaces.SyncModuly;
using SolEx.Hurt.Sync.App.Modules_.Helpers;

namespace SolEx.Hurt.Sync.App.Modules_.NoweModuly.KategorieProduktow
{
    public class OpisyDoKategorii : SyncModul, IModulKategorieProduktow
    {
        public override string Opis
        {
            get { return "Opisy do kategorii"; }
        }

        [FriendlyName("Pole wg ktorego mapujemy opisy do kategorii")]
        [SyncSlownikNaPodstawieInnegoTypu(typeof(KategoriaProduktu))]
        public string Pole { get; set; }
        
        [FriendlyName("Ścieżka do katalogu z plikami - np. c:\\pliki\\")]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        public string Sciezka { get; set; }

        private bool WypelnijKategorie(object c,Plik p)
        {
            KategoriaProduktu kategoria = (KategoriaProduktu)c;
            kategoria.Opis = Hurt.Helpers.Tools.PobierzInstancje.PobierzZawartoscPlikuTekstowegoZFormatowaniem(p.Sciezka + p.nazwaLokalna, Opisy.KodowanieOpisow.Dopasuj, true);
            return true;
        }

        public override string uwagi
        {
            get { return string.Empty; }
        }

        public void Przetworz(ref Dictionary<long, KategoriaProduktu> listaWejsciowa, Dictionary<long, KategoriaProduktu> listaKategoriiB2B, ISyncProvider provider, List<Grupa> grupyPRoduktow)
        {
            List<Plik> plikiLokalne = new List<Plik>();
            List<KategoriaProduktu> listaKategorii = listaWejsciowa.Values.ToList();
            PlikiHelper.PrzetworzPlikiDlaTypu<KategoriaProduktu>(Sciezka, plikiLokalne, Pole, WypelnijKategorie, listaKategorii, false);
            listaWejsciowa = listaKategorii.ToDictionary(a => a.Id, a => a);
        }
    }
}
