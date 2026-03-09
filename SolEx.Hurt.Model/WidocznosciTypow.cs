using ServiceStack.DesignPatterns.Model;
using SolEx.Hurt.Model.Enums;
using SolEx.Hurt.Model.Helpers;

namespace SolEx.Hurt.Model
{
    /// <summary>
    ///Uzycie tego typu jest WYMAGANE razem z interefejsem IObiektWidocznyDlaOkreslonychGrupKlientow!!
    /// </summary>
    public class WidocznosciTypow:IHasLongId
    {
        public WidocznosciTypow()
        {
            KategoriaKlientaIdKtorakolwiek = null;
            KategoriaKlientaIdWszystkie = null;
        }

        public static long ObliczKlucz(long ObiektId, string Typ)
        {
            return string.Format("{0}_{1}", ObiektId, Typ).WygenerujIDObiektuSHAWersjaLong();
        }

        public long Id {
            get
            {
                //jesli obiektID == 0 to inaczej budujejemy klucz - czyli SZABLON ogólny
                if (ObiektId == 0)
                {
                 return string.Format("{0}_{1}", Nazwa, Typ).WygenerujIDObiektuSHAWersjaLong();
                }
                return WidocznosciTypow.ObliczKlucz(ObiektId, Typ);
            }
        }

        public long ObiektId { get; set; }
        
        public string Typ { get; set; }

        public int[] KategoriaKlientaIdWszystkie { get; set; }

        public int[] KategoriaKlientaIdKtorakolwiek { get; set; }
        public WidocznoscTypoKierunek Kierunek { get; set; }
        public string Nazwa { get; set; }

    }
}
