using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ServiceStack.DataAnnotations;
using SolEx.Hurt.Model.AtrybutyKlas;
using SolEx.Hurt.Model.Enums;
using SolEx.Hurt.Model.Interfaces;

namespace SolEx.Hurt.Model
{
    public class Komunikaty : IObiektWidocznyDlaOkreslonychGrupKlientow, IPolaIDentyfikujaceRecznieDodanyObiekt
    {
        public Komunikaty()
        {
            OdKiedy = null;
        }
        
        [PrimaryKey]
        [AutoIncrement]
        [WidoczneListaAdmin(true, true, false, false)]
        public long Id { get; set; }

        [FriendlyName("Kategorie klientów którym będzie pokazany lub ukryty komunikat")]
        [WymuszonyTypEdytora(TypEdytora.WidocznoscDlaKlientow)]
        [Ignore]
        [Niewymagane]
        [WidoczneListaAdmin(false, false, true, false)]
        public WidocznosciTypow Widocznosc { get; set; }

        [WidoczneListaAdmin(true, true, true, true)]
        [FriendlyName("Nazwa")]
        public string Nazwa { get; set; }

        [WidoczneListaAdmin(true, true, true, true)]
        [FriendlyName("Treść")]
        [Lokalizowane]
        [GrupaAtttribute("Ogólne", 1)]
        [WymuszonyTypEdytora(TypEdytora.EdytorTekstowy)]
        public string Tresc { get; set; }

        [WidoczneListaAdmin(true, true, true, true)]
        [FriendlyName("Od kiedy komunikat ma być aktywny")]
        [WymuszonyTypEdytora(TypEdytora.PoleDatoweZCzasem)]
        [Niewymagane]
        public DateTime? OdKiedy { get; set; }

        [WidoczneListaAdmin(true, true, true, true)]
        [FriendlyName("Przyciski dla klienta")]
        [Lokalizowane]
        public PrzyciskiDlaKlienta Przycisk { get; set; }

        [WidoczneListaAdmin(true, true, true, true)]
        [FriendlyName("Cykl pokazywanie komunikatu")]
        [Lokalizowane]
        [GrupaAtttribute("Ogólne", 1)]
        public CyklKomunikatu CyklPokazywania { get; set; }

        [Ignore]
        public string Klucz { get; set; }

        public bool RecznieDodany()
        {
            return true;
        }
    }
}
