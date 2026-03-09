
using System;
using SolEx.Hurt.Core.Helper;
using SolEx.Hurt.Core.ModelBLL;
using SolEx.Hurt.Model.AtrybutyKlas;
using SolEx.Hurt.Model.Enums;
using SolEx.Hurt.Model.Interfaces;

namespace SolEx.Hurt.Web.Site2.Models.KontrolkiTresci.Bazowe
{
    public abstract class PokazWpisBaza : KontrolkaTresciBaza, IPoleJezyk
    {

        [FriendlyName("Nagłówek sekcji")]
        [WidoczneListaAdmin(true, false, true, false)]
        [WymuszonyTypEdytora(TypEdytora.EdytorTekstowy)]
        [Niewymagane]
        [Lokalizowane]
        public string Naglowek { get; set; }


        [WidoczneListaAdmin(true, false, true, false)]
        [WymuszonyTypEdytora(TypEdytora.EdytorTekstowy)]
        [Niewymagane]
        [FriendlyName("Stopka",FriendlyOpis = "Stopka dla pola")]
        [Lokalizowane]
        public string Stopka { get; set; }

        [FriendlyName("Rozmiar zdjęcia")]
        [Niewymagane]
        [GrupaAtttribute("Zdjęcia", 11)]
        [WidoczneListaAdmin(true, false, true, false)]
        [PobieranieSlownika(typeof(SlownikRozmiarZdjec))]
        public string Preset { get; set; }

        [FriendlyName("Opakowanie Html dla pól tekstowych - jako zawartość pola do podstawienia użyj {0}.")]
        [Niewymagane]
        [GrupaAtttribute("Pola tekstowe", 11)]
        [WidoczneListaAdmin(true, false, true, false)]
        public string Opakowanie { get; set; }

        public abstract string SymbolIdentyfikatora { get; }

        /// <summary>
        /// określa z jakiego modelu korzysta kontrolka (Kategorie, produkty, blog itp)
        /// </summary>
        public abstract string ModelObiektu { get; }

        public object IdentyfikatorObiektu
        {
            get
            {
                if (string.IsNullOrEmpty(SymbolIdentyfikatora)) return null;
                object wynik=null;
                wynik = PobierzIdentyfikator(SymbolIdentyfikatora, true, ModelObiektu);
                return wynik;  
            } 
        }

        public int JezykId { get; set; }
    }
  
   
}