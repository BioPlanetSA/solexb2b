using SolEx.Hurt.Core.Helper;
using SolEx.Hurt.Core.ModelBLL;
using SolEx.Hurt.Model;
using SolEx.Hurt.Model.AtrybutyKlas;
using SolEx.Hurt.Model.Enums;

namespace SolEx.Hurt.Web.Site2.Models.KontrolkiTresci
{
    public class Blog : KontrolkaTresciBaza
    {
        public override string Nazwa
        {
            get { return "Pokaż wpisy bloga"; }
        }

        public override string Kontroler
        {
            get { return "Blog"; }
        }

        public override string Akcja
        {
            get { return "WpisyBloga"; }
        }


        public override string Grupa
        {
            get { return "Wygląd"; }
        }

        public Blog()
        {
            BlogPokazujZdjecia = false;
            BlogIlePokazywacMaxWpisow = 10;
            BlogIloscPierwszychZnakow = 200;
            BlogPokazujUklad = "ListaProsta";
            BlogIleDynamicznieZaladowacAktualnosci = 0;
            BlogPokazujTytul = true;
            BlogKafleUklad = "col-xs-12 col-sm-6 col-md-4 col-lg-3";
            Naglowek = "<h2 style='color:#cf242a; margin-bottom: 40px' class='display-4'><b>AKTUALNOŚCI</b></h2>";
            CzytajDalejTresc = "CZYTAJ DALEJ";
            BlogPokazujDate = BlogiSposobPokazaniaDaty.Brak;
            UkladPuzli = "lg3 md2 sm1 xs1";
        }

      


        [FriendlyName("Kategorie do pokazania")]
        [WidoczneListaAdmin(true,true,true,true)]
        [PobieranieSlownika(typeof(SlownikKategorieBlogow))]
        [Wymagane]
        public long[] BlogKategorieDoPokazania { get; set; }

        [FriendlyName("Pokazuj zdjęcia")]
        [WidoczneListaAdmin(true, true, true, true)]
        [Wymagane]
        public bool BlogPokazujZdjecia { get; set; }

        [FriendlyName("Maksymalna ilość wpisów")]
        [WidoczneListaAdmin(true, true, true, true)]
        [Wymagane]
        public int BlogIlePokazywacMaxWpisow { get; set; }

        [FriendlyName("Ilość pierwszych znaków pokazywać jako skrót treści")]
        [WidoczneListaAdmin(true, true, true, true)]
        [Wymagane]
        public int BlogIloscPierwszychZnakow { get; set; }

        [FriendlyName("Czy pokazywać tytuł")]
        [WidoczneListaAdmin(true, true, true, true)]
        public bool BlogPokazujTytul { get; set; }

        [FriendlyName("Sposób pokazania daty")]
        [WidoczneListaAdmin(true, true, true, true)]
        public BlogiSposobPokazaniaDaty BlogPokazujDate { get; set; }

        [FriendlyName("Układ")]
        [WidoczneListaAdmin(true, true, true, true)]
        [PobieranieSlownika(typeof(SlownikUkladowDlaBloga))]
        [WymuszonyTypEdytora(TypEdytora.PoleDropDown)]
        public string BlogPokazujUklad { get; set; }

        [FriendlyName("Kolejność")] 
        [WidoczneListaAdmin(true, true, true, true)]
        public BlogiKolejnosc BlogKolejnosc { get; set; }

        [WidoczneListaAdmin(true, true, true, true)]
        [FriendlyName("Ile nowych wpisów ładować dynamicznie", FriendlyOpis = "Jeśli 0 to przycisk ładowania więcej nie pokazuje się")]
        public int BlogIleDynamicznieZaladowacAktualnosci { get; set; }

        [WidoczneListaAdmin(true, true, true, true)]
        [FriendlyName("Preset dla zdjęć",FriendlyOpis = "Presety do zdjęć pobierane są z pliku obrazki_ustawienie.config")]
        [PobieranieSlownika(typeof(SlownikPresetowDoZdjec))]
        [Niewymagane]
        public string BlogPresetDlaZdjec { get; set; }

        [WymuszonyTypEdytora(TypEdytora.EdytorTekstowy)]
        [WidoczneListaAdmin(true, true, true, true)]
        [FriendlyName("Nagłówek wpisów")]
        [Niewymagane]
        public string Naglowek { get; set; }


        [WymuszonyTypEdytora(TypEdytora.EdytorTekstowy)]
        [WidoczneListaAdmin(true, true, true, true)]
        [FriendlyName("Czytaj dalej treść przycisku", FriendlyOpis = "Pokazuje się tylko jeśli można przejść do czytaj dalej. Jeśli puste pole przycisk nie pokazuje się")]
        [Niewymagane]
        public string CzytajDalejTresc { get; set; }

        [Niewymagane]
        [FriendlyName("Symbol strony otwieranej dla bloga - jeśli nie wybrana to nie będą pokazywane linki do szczegółów wpisów")]
        [WidoczneListaAdmin(true, true, true, true)]
        [PobieranieSlownika(typeof(SlownikTresciSystemowych))]
        public string SymbolStrony { get; set; }

        [Niewymagane]
        [FriendlyName("Klasy styli CSS dla pojedyńczego wpisu bloga", FriendlyOpis = "np. cień, tekst to lewej, img-circle itp")]
        [WidoczneListaAdmin(true, true, true, true)]
        [PobieranieSlownika(typeof(SlownikKlassCss))]
        public string[] CssDlaPojedynczegoWpisu { get; set; }

      //  private BlogModel _model = null;

        [GrupaAtttribute("Puzle", 6)]
        [FriendlyName("Puzle ilość w rzędzie", FriendlyOpis = "Domyślnie: lg3 md2 sm1 xs1 - Uwaga! nie stosuje się tu składni bootstrapa col-x-y!! ")]
        [WidoczneListaAdmin(true, false, true, true)]
        public string UkladPuzli { get; set; }


        // [GrupaAtttribute("Puzle", 6)]
        //[FriendlyName("Puzle ilość w rzędzie", FriendlyOpis = "Domyślnie: lg3 md2 sm1 xs1 - Uwaga! nie stosuje się tu składni bootstrapa col-x-y!! ")]
        //[WidoczneListaAdmin(true, false, true, true)]
        //public string UkladPuzli { get; set; }

        [GrupaAtttribute("Kafle", 6)]
        [FriendlyName("Kafle ilość w rzędzie", FriendlyOpis = "Domyślnie: col-xs-12 col-sm-6 col-md-4 col-lg-3")]
        [WidoczneListaAdmin(true, false, true, true)]
        public string BlogKafleUklad { get; set; }

        public BlogModel Model
        {
            get
            {

                BlogModel _model = new BlogModel();
                    _model.BlogKolejnosc = this.BlogKolejnosc;
                    _model.BlogIloscPierwszychZnakow = this.BlogIloscPierwszychZnakow;
                    _model.BlogPokazujZdjecia = this.BlogPokazujZdjecia;
                    _model.BlogIleDynamicznieZaladowacAktualnosci = this.BlogIleDynamicznieZaladowacAktualnosci;
                    _model.BlogKategorieDoPokazania = this.BlogKategorieDoPokazania;
                    _model.BlogIlePokazywacMaxWpisow = this.BlogIlePokazywacMaxWpisow;
                    _model.BlogPokazujTytul = this.BlogPokazujTytul;
                    _model.BlogPresetDoZdjec = this.BlogPresetDlaZdjec;
                    _model.SymbolStronyWpisPojedynczy = this.SymbolStrony;
                    _model.BlogUklad = this.BlogPokazujUklad;
                    _model.PokazujDate = this.BlogPokazujDate;
                    _model.BlogKafleUklad = this.BlogKafleUklad;
                    _model.NaglowekWpisow = this.Naglowek;
                    _model.CzytajDalejTresc = this.CzytajDalejTresc;
                    _model.CssDlaPojedynczegoWpisu = this.CssDlaPojedynczegoWpisu;
                    _model.UkladPuzli = this.UkladPuzli;
                
                return _model;
            }
        }

    }
}