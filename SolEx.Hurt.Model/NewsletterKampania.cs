using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using ServiceStack.DataAnnotations;
using ServiceStack.DesignPatterns.Model;
using SolEx.Hurt.Model.AtrybutyKlas;
using SolEx.Hurt.Model.Enums;
using SolEx.Hurt.Model.Helpers;
using SolEx.Hurt.Model.Interfaces;

namespace SolEx.Hurt.Model
{
    /// <summary>
    /// Model newslettera
    /// </summary>
    [Alias("Newsletter")]
    [NieSprawdzajCzyIsnieje]
    [TworzDynamicznieTabele]
    public class NewsletterKampania : IPolaIDentyfikujaceRecznieDodanyObiekt, IObiektWidocznyDlaOkreslonychGrupKlientow
    {        
        [FriendlyName("Nazwa wysy³ki")]
        [WidoczneListaAdmin(true, true, true, false)]
        [GrupaAtttribute("Dane o wysy³ce", -99)]
        public string Nazwa { get; set; }


        //[FriendlyName("Aktywny")]
        //[WidoczneListaAdmin(true, true, true, false)]
        //[GrupaAtttribute("Dane o wysy³ce", -99)]
        //public bool Aktywna { get; set; }



        [FriendlyName("Temat newslettera")]
        [GrupaAtttribute("Dane o wysy³ce", -99)]
        [WidoczneListaAdmin(false, false, true, false)]
        public string Temat { get; set; }


        [FriendlyName("Kategorie klientów którym wys³any zostanie newsletter")]
        [WymuszonyTypEdytora(TypEdytora.WidocznoscDlaKlientow)]
        [Ignore]
        [Niewymagane]
        [WidoczneListaAdmin(false, false, true, false)]
        [GrupaAtttribute("Dane o wysy³ce", -99)]
        public WidocznosciTypow Widocznosc { get; set; }

        //todo:  odkryæ jak juz poprawimy wybieranie klientów z kategorii - kontrolka
        [FriendlyName("Dodatkowe adresy email klientów oddzielone œrednikiem")]
        [GrupaAtttribute("Dane o wysy³ce", -99)]
        [WidoczneListaAdmin(true, false, true, false)]
        [Niewymagane]
        public string DodatkoweAdresyKlientow { get; set; }



        [Niewymagane]
        [FriendlyName("Adres email na jaki maj¹ byæ kierowane odpowiedzi", FriendlyOpis = "Odpowiedz klienta jest kierowana zawsze do opiekuna klienta. Jeœli klient nie ma posiada opiekuna, mo¿esz w poni¿szym polu wskazaæ zastêpczy adres odpowiedzi.")]
        [GrupaAtttribute("Dane o wysy³ce", -99)]
        [WidoczneListaAdmin(false, false, true, false)]
        public string OdpowiedzNaAdres { get; set; }




        [Niewymagane]
        [FriendlyName("Kiedy wys³aæ automatycznie", FriendlyOpis = "Jeœli data zostanie wybrana, system automatycznie o zadanym czasie zacznie wysy³aæ maile. Jeœli chcesz rêcznie rozpocz¹æ wysy³anie maili, nie wybieraj daty w tym polu. <br/> Date nale¿y podaæ w formacie: 2016-07-18 20:20")]
        [WidoczneListaAdmin(true, false, true, false)]
        [WymuszonyTypEdytora(TypEdytora.PoleDatoweZCzasem)]
        [GrupaAtttribute("Dane o wysy³ce", -99)]
        public DateTime? DataWysylki { get; set; }
        

        [WidoczneListaAdmin(true, true, true, false)]
        [GrupaAtttribute("Dane o wysy³ce", -99)]
        [FriendlyName("Autor")]
        public string Autor { get; set; }
        

        [FriendlyName("Treœæ newslettera", FriendlyOpis = "Tagi jakich mo¿na u¿ywaæ:  <br/> @Html.RenderujTuProduktyNewslettera()  - lista produktów <br/>  ")]
        [WidoczneListaAdmin(true, false, true, false)]
        [GrupaAtttribute("Treœæ", 1)]
        [WymuszonyTypEdytora(TypEdytora.EdytorTekstowy)]
        public string Tresc { get; set; }

        [FriendlyName("Produkty")]
        [Niewymagane]
        [WidoczneListaAdmin(true, false, true, false)]
        [PobieranieSlownika("SolEx.Hurt.Core.Helper.SlownikProduktow,SolEx.Hurt.Core")]
        [GrupaAtttribute("Treœæ", 1)]
        public List<long> WybraneProdukty { get; set; }

        [Niewymagane]
        [FriendlyName("SzablonListyProduktow listy produktów")]
        [WidoczneListaAdmin(true, true, true, false)]
        [GrupaAtttribute("Treœæ", 1)]
        [PobieranieSlownika("SolEx.Hurt.Core.Helper.SlownikSzablonowProduktowWNewsletterach,SolEx.Hurt.Core")]
        public string SzablonListyProduktow { get; set; }
        

        //------------------------------------------------------------------------------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------STATYSTYKI---------------------------------------------------------------------------------------------------------------------        

        [FriendlyName("Status")]
        [WidoczneListaAdmin(true, true, false, false)]
        [GrupaAtttribute("Informacje", 99)]
        public StatusNewsletter Status { get; set; }
        

  
        public NewsletterKampania()
        {
            //Aktywna = true;
            DataWysylki = null;
            Status = StatusNewsletter.Przygotowywany;
        }
        

        public bool RecznieDodany()
        {
            return true;
        }

     

        [PrimaryKey]
        [AutoIncrement]
        [WidoczneListaAdmin(true, true, false, false)]
        [GrupaAtttribute("Informacje", 99)]
        public long Id { get; set; }


        [Ignore]
        public HashSet<string> ListaAdresowKlientowDodatkowe
        {
            get
            {
                var r = Serializacje.PobierzInstancje.DeSerializeList<string>(DodatkoweAdresyKlientow, ";");
                return r;
            }
            set
            {
                DodatkoweAdresyKlientow = Serializacje.PobierzInstancje.SerializeList(value, ';');
            }
        }

    }
}
