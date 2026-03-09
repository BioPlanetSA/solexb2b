using System.Linq;
using ServiceStack.DataAnnotations;
using ServiceStack.DesignPatterns.Model;
using SolEx.Hurt.Core.BLL;
using SolEx.Hurt.Core.BLL.ZadaniaKoszyka;
using SolEx.Hurt.Core.Helper;
using SolEx.Hurt.Model.AtrybutyKlas;
using System.Collections.Generic;
using SolEx.Hurt.Model.Enums;
using SolEx.Hurt.Model.Interfaces;

namespace SolEx.Hurt.Model
{
    public class SposobPokazywaniaStanow : IObiektWidocznyDlaOkreslonychGrupKlientow, IPolaIDentyfikujaceRecznieDodanyObiekt, IHasIntId
    {
        public SposobPokazywaniaStanow()
        {
            PozycjaLista = PozycjaLista.Kolumna;
            PozycjaKarta = PozycjaKarta.ObokKoszyka;
        }

        [PrimaryKey]
        [UpdateColumnKey]
        [AutoIncrement]
        [WidoczneListaAdmin(true, true, false, false)]
        public int Id { get; set; }
        [WidoczneListaAdmin(true, true, true, true)]
        public string Nazwa { get; set; }
        [WidoczneListaAdmin(false, false, true, false)]
        [PobieranieSlownika(typeof(SlownikMagazynow))]
        public int? DomyslnyMagazynId { get; set; }

        //[Ignore]
        //public List<SposobPokazywaniaStanowRegula> Reguly { get; set; }


        [WidoczneListaAdmin(true, true, true, true)]
        public AccesLevel Dostep { get; set; }

        [WidoczneListaAdmin(false, false, true, false)]
        public PozycjaLista PozycjaLista { get; set; }
        [WidoczneListaAdmin(false, false, true, false)]
        public PozycjaKarta PozycjaKarta { get; set; }
        [WidoczneListaAdmin(false, false, true, false)]
        [Niewymagane]
        [PobieranieSlownika(typeof(SlownikGrupyKategoriiKlienta))]
        public string KategoriaKlientaMagazyn { get; set; }
        [WidoczneListaAdmin(false, false, true, false)]
        [Niewymagane]
        public List<RoleType> DozwolonaRolaKlienta { get; set; }

        [FriendlyName("Widoczność")]
        [WymuszonyTypEdytora(TypEdytora.WidocznoscDlaKlientow)]
        [Ignore]
        [WidoczneListaAdmin(false, false, true, false)]
        public WidocznosciTypow Widocznosc { get; set; }

        public bool RecznieDodany()
        {
            return true;
        }

           
        
            
            
        //[Ignore]
        //[WidoczneListaAdmin(false,false,true,false)]
        //[WymuszonyTypEdytora(TypEdytora.ListaWarunkow, typeof(SposobPokazywaniaStanowRegula))]
        //[FriendlyName("Reguły sposobu pokazywania stanów")]
        //public List<SposobPokazywaniaStanowRegula> Reguly
        //{
        //    get
        //    {
                
        //        // ReSharper disable once SuspiciousTypeConversion.Global
        //        //var obj =(IObiektPrzechowujacyKontrolke) SolexBllCalosc.PobierzInstancje.DostepDane.Pobierz<SposobPokazywaniaStanowRegula>(null, x => x.SposobId == Id).ToList();
        //        //var pars = OpisObiektu.PobierzParametry(typeof(SposobPokazywaniaStanowRegula));
        //        //var istniejace = obj.PobierzParametry();


        //    }
        //}
    }
}
