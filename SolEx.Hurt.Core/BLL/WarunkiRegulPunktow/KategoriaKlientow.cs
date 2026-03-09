using SolEx.Hurt.Core.BLL.RegulyKoszyka;
using SolEx.Hurt.Core.Helper;
using SolEx.Hurt.Core.ModelBLL.Interfejsy;
using SolEx.Hurt.Model.AtrybutyKlas;
using SolEx.Hurt.Model.Interfaces;
using System.Collections.Generic;
using System.Linq;
using SolEx.Hurt.Core.ModelBLL;
using SolEx.Hurt.Model;
using SolEx.Hurt.Model.Enums;

namespace SolEx.Hurt.Core.BLL.WarunkiRegulPunktow
{
    public class KategoriaKlientow : WarunekRegulyPunktowej, IWarunekRegulyPozycjiDokumentu, IWarunekRegulyCalegoDokumentu, ITestowalna
    {
        public KategoriaKlientow()
        {
            Warunek = RelacjaJestNieJest.Jest;
        }

        public override string Opis
        {
            get { return "Sprawdza kategorie klientów płatnika dokumentu."; }
        }

        [PobieranieSlownika(typeof(SlownikKategoriiKlienta))]
        [FriendlyName("Sprawdzana kategoria")]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        public List<string> KategoriaID { get; set; }

        [FriendlyName("Warunek rodzaj")]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        public RelacjaJestNieJest Warunek { get; set; }

        public bool SpelniaWarunek(DokumentuPozycjaBazowa pozycja, DokumentyBll dokument)
        {
            return SpelniaWarunek(dokument);
        }

        private List<int> IdKategori
        {
            get { return KategoriaID.Select(int.Parse).ToList(); }
        }

        public List<string> TestPoprawnosci()
        {
            List<string> listaBledow = new List<string>();
            IDictionary<int, Model.KategoriaKlienta> kat = SolexBllCalosc.PobierzInstancje.DostepDane.Pobierz<Model.KategoriaKlienta>(null).ToDictionary(x => x.Id, x => x);
            if (KategoriaID == null)
            {
                listaBledow.Add("Nie można poprać id kategorii");
            }
            else
            {
                foreach (var k in IdKategori)
                {
                    if (!kat.ContainsKey(k))
                    {
                        listaBledow.Add(string.Format("Nie ma kategorii o id{0}", KategoriaID));
                    }
                }
            }

            return listaBledow;
        }

        public bool SpelniaWarunek(DokumentyBll dokument)
        {
            var kats = new HashSet<int>(dokument.DokumentPlatnik.Kategorie);
            bool wynik = IdKategori.Any(kats.Contains);
            return Warunek == RelacjaJestNieJest.NieJest ? !wynik : wynik;
        }
    }
}