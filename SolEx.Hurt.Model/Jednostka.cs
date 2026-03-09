using System;
using ServiceStack.DataAnnotations;
using SolEx.Hurt.Model.AtrybutyKlas;
using SolEx.Hurt.Model.Enums;
using SolEx.Hurt.Model.Interfaces;

namespace SolEx.Hurt.Model
{
    public class Jednostka : IObiektWidocznyDlaOkreslonychGrupKlientow, IPoleJezyk
    {
        public Jednostka()
        {
            Nazwa = "";
            Aktywna = true;
            Calkowitoliczowa = true;
            Zaokraglenie = 2;
        }

        public decimal PrzeliczIlosc(decimal tmp)
        {
            return Calkowitoliczowa ? decimal.Ceiling(tmp) : tmp;
        }
        public Jednostka(Jednostka baza)
        {
            Id = baza.Id;
            Nazwa = baza.Nazwa;
            Calkowitoliczowa = baza.Calkowitoliczowa;
            Aktywna = baza.Aktywna;
            Komunikat = baza.Komunikat;
            Zaokraglenie = baza.Zaokraglenie;
        }

        [PrimaryKey]
        [UpdateColumnKey]
        [WidoczneListaAdmin(true, true, false, false)]
        public virtual long Id { get; set; }

        [WidoczneListaAdmin(true, true, true, true)]   
        [Lokalizowane]
        public string Nazwa { get; set; }

        [WidoczneListaAdmin(true, true, true, true)]     
        public bool Calkowitoliczowa { get; set; }

        [WidoczneListaAdmin(true, true, true, true)]
        public bool Aktywna { get; set; }

        [Niewymagane]
        [FriendlyName("Komunikat pokazywany przy kupowaniu produktów w tej jednostce")]
        [WidoczneListaAdmin(true, true, true, true)]
        [Lokalizowane]
        public string Komunikat { get; set; }
        
        [FriendlyName("Do ilu miejsc zaokrąglić", FriendlyOpis = "Maxymalne zaorąglenie to 6")]
        [WidoczneListaAdmin(true, true, true, true)]
        public int Zaokraglenie { get; set; }

        [FriendlyName("Widoczność")]
        [WymuszonyTypEdytora(TypEdytora.WidocznoscDlaKlientow)]
        [Ignore]
        [WidoczneListaAdmin(false, false, true, false)]
        public WidocznosciTypow Widocznosc { get; set; }

        [Ignore]
        public int JezykId { get; set; }

        [Ignore]
        public decimal Krok { get; set; }

        public virtual decimal PoprawanaIloscZakupu(decimal? maksimum=null)
        {
            if (maksimum.HasValue && maksimum.Value <= 0)
            {
                throw new InvalidOperationException("Maksimum musi być większe o 0");
            }
            if (Calkowitoliczowa)
            {
                return maksimum.HasValue ?maksimum.Value- 1 :1;
            }
            return maksimum.HasValue ? maksimum.Value - 0.1M : 0.1M;
        }
    }
}
