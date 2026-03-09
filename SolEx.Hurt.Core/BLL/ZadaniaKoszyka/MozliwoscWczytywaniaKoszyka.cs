using SolEx.Hurt.Core.ModelBLL.Interfejsy;
using SolEx.Hurt.Model.AtrybutyKlas;
using SolEx.Hurt.Model.Web;

namespace SolEx.Hurt.Core.BLL.ZadaniaKoszyka
{
    internal class MozliwoscWczytywaniaKoszyka : ZadanieCalegoKoszyka, IModulStartowy, IFinalizacjaKoszyka, IModulKoszykPusty
    {
        public MozliwoscWczytywaniaKoszyka()
        {
            Format = "{0} <a class=\"alert-link\" href=\"{3}\">{1}</a>{2}";
            TekstPrzed = "Aby wczytać lub wkleić produkty do koszyka z plików CSV, Excel lub z Twojego programu do zamówień";
            TekstLink = "kliknij tutaj";
            TekstZaLinkiem = "";
            Link = "/import-koszyka";
        }

        public override string Opis
        {
            get { return "Pokazuje belkę z linkiem do wczytania zamówienia z pliku, formatuje zgodnie z polem Format, za {0} podstawiane jest TekstPrzedLinkiem, za {1} podstawiane jest TekstLink, za {2} podstawiane jest TekstZaLinkiem, za {3} podstawiany jest Link"; }
        }

        [Lokalizowane]
        [FriendlyName("Tekst przed linkiem")]
        [Niewymagane]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        public string TekstPrzed { get; set; }

        [FriendlyName("Link do strony")]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        public string Link { get; set; }

        [Lokalizowane]
        [FriendlyName("Tekst linku")]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        public string TekstLink { get; set; }

        [Lokalizowane]
        [Niewymagane]
        [FriendlyName("Tekst za linkiem")]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        public string TekstZaLinkiem { get; set; }

        [FriendlyName("Formaty linku")]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        public string Format { get; set; }

        public override bool Wykonaj(IKoszykiBLL koszyk)
        {
            string fraza = string.Format(Format, TekstPrzed, TekstLink, TekstZaLinkiem, Link);
            WyslijWiadomosc(string.Format(fraza, koszyk.WagaCalokowita()), KomunikatRodzaj.info);
            return true;
        }
    }
}