using SolEx.Hurt.Core.ModelBLL;
using SolEx.Hurt.Model.AtrybutyKlas;

namespace SolEx.Hurt.Web.Site2.Models.KontrolkiTresci
{
    public abstract class SlajderKontrolka : KontrolkaTresciBaza
    {
        public SlajderKontrolka()
        {
            CzasAnimacji = 2;
            CzasPrzeskoku = 0;
        }
        [Lokalizowane]
        public override string Nazwa
        {
            get { return "Slajder"; }
        }

        public override string Grupa
        {
            get { return "Slajder"; }
        }

        public override string Kontroler
        {
            get { return "Slajder"; }
        }

        [WidoczneListaAdmin(false, false, true, false)]
        [PobieranieSlownika("SolEx.Hurt.Core.Helper.SlownikSlajdow,SolEx.Hurt.Core")]
        [FriendlyName("Lista slajdów",FriendlyOpis = "kolejność slajdów ustawiana jest na podstawie pola kolejność ze slajdów")]
        public long[]ListaSlajdow { get; set; }

        [WidoczneListaAdmin(true, true, true, false)]
        [FriendlyName("Czas automatycznego przeskoku w sek. - 0 oznacza wyłączenie automatycznego przeskoku")]
        public int CzasPrzeskoku { get; set; }

        [WidoczneListaAdmin(true, true, true, false)]
        [FriendlyName("Czas animacji w sek.")]
        public int CzasAnimacji { get; set; }
    }
}