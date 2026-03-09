using System.Collections.Generic;
using ServiceStack.DataAnnotations;

namespace SolEx.Hurt.Model
{
    public class ZamowieniaImport : ZamowienieSynchronizacja
    {
        [Ignore]
        public string Link { get; set; }

        //[Ignore]
        //public string Numer { get; set; }

        [Ignore]
        public string Pochodzenie { get; set; }

        public override string NumerZPlatformy => "Import " + DoklejInfoNumer();

        public ZamowieniaImport()
        {
        }

        public ZamowieniaImport(ZamowieniaImport item) : base(item)
        {
        }

        public new ZamowieniaImport StworzZamowieniaRozbite(int kolejnyNumerRozbitegoZamowienia, string powodRozbicia, int dlugoscNumeruRozbicia = 20)
        {
            ZamowieniaImport nowe = new ZamowieniaImport(this)
            {
                LacznieRozbitych = this.LacznieRozbitych,
                ListaDokumentowZamowienia = this.ListaDokumentowZamowienia,
                Podtytul = this.Podtytul,
                Rozbijaj = this.Rozbijaj,
                ZamowienieImportowanePoStronieKlienta = this.ZamowienieImportowanePoStronieKlienta,
                NumerZRozbicia = $"{NumerTymczasowyZamowienia}/{kolejnyNumerRozbitegoZamowienia}/{powodRozbicia}",
                PochodziZRozbicia = true,
                pozycje = new List<ZamowienieProdukt>()
            };

            //numer rozbiciaj max 20 znakow
            if (nowe.NumerZRozbicia.Length > dlugoscNumeruRozbicia)
            {
                nowe.NumerZRozbicia = nowe.NumerZRozbicia.Substring(0, dlugoscNumeruRozbicia);
            }

            //uzupełniamy nowe zamóienie o pola których nie ma w ZamowienieSynchronizacja
            nowe.Link = this.Link;
            nowe.Pochodzenie = this.Pochodzenie;
            return nowe;
        }
    }
}