using ServiceStack.DataAnnotations;
using ServiceStack.DesignPatterns.Model;
using ServiceStack.Text;
using SolEx.Hurt.Core.BLL;
using SolEx.Hurt.Core.DostepDane.DaneObiektu;
using SolEx.Hurt.Core.Helper;
using SolEx.Hurt.Model;
using SolEx.Hurt.Model.AtrybutyKlas;
using SolEx.Hurt.Model.Enums;
using SolEx.Hurt.Model.Helpers;
using SolEx.Hurt.Model.Interfaces;
using SolEx.Hurt.Model.Interfaces.Modele;

namespace SolEx.Hurt.Core.ModelBLL
{
    [EdytowalnyAdmin]
    [Alias("ProfilKlienta")]
    public class ProfilKlienta : IProfilKlienta, IPolaIDentyfikujaceRecznieDodanyObiekt, IStringIntern
    {
        [Ignore]
        [WidoczneListaAdmin(true, true, true, true)]
        [GrupaAtttribute("Dane podstawowe", 0)]
        [FriendlyName("Wartość")]
        public object WartoscLadna
        {
            get { return JsonSerializer.DeserializeFromString<object>(Wartosc); }
            set
            {
                Wartosc = JsonSerializer.SerializeToString(value);
            }
        }

        public long Id
        {
            get
            {
                return (KlientId + "_" + TypUstawienia + "_" + Dodatkowe + "_" + Dopisek).WygenerujIDObiektuSHAWersjaLong();
            }
        }

        [WidoczneListaAdmin(true, true, false, false)]
        [PobieranieSlownika(typeof(SlownikKlientow))]
        public long? KlientId { get; set; }

        [StringInternuj]
        [Niewymagane]
        [WidoczneListaAdmin(true, true, false, false)]
        [GrupaAtttribute("Dane podstawowe", 0)]
        [FriendlyName("Dane dodatkowe")]
        public string Dodatkowe { get; set; }

        /// <summary>
        /// typ ustawienia czyli do czego odnosi się ustawienie (Układ komumn, filtry)
        /// </summary>
        [WidoczneListaAdmin(true, true, false, false)]
        [GrupaAtttribute("Dane podstawowe", 0)]
        public TypUstawieniaKlienta TypUstawienia { get; set; }

        /// <summary>
        /// wartość ustawienia srerializowana i przechowywana jako tekst 
        /// </summary>
        [StringInternuj]
        public string Wartosc { get; set; }

        /// <summary>
        /// dodatkowy dopisek do Id np jesli wartość jest dla niezalogowanych
        /// </summary>
        [StringInternuj]
        [WidoczneListaAdmin(true, true, false, false)]
        [GrupaAtttribute("Dane podstawowe", 0)]
        [FriendlyName("Domyslnie dla")]
        public string Dopisek { get; set; }

        public ProfilKlienta() { }

        public ProfilKlienta(ProfilKlienta pk)
        {
            if (pk == null)
            {
                return;
            }
            TypUstawienia = pk.TypUstawienia;
            Wartosc = pk.Wartosc;
            Dodatkowe = pk.Dodatkowe;
            Dopisek = pk.Dopisek;
            KlientId = pk.KlientId;
        }

        public ProfilKlienta(TypUstawieniaKlienta typ, object war, string dodakowe, AccesLevel dopisek)
        {
            TypUstawienia = typ;
            Wartosc = JsonSerializer.SerializeToString(war);
            Dodatkowe = dodakowe;
            Dopisek = dopisek.ToString();
        }

        public bool RecznieDodany()
        {
            return true;
        }
    }
}
