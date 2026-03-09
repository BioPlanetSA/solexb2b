using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using ServiceStack.DataAnnotations;
using SolEx.Hurt.Core.BLL;
using SolEx.Hurt.Model;
using SolEx.Hurt.Model.AtrybutyKlas;
using SolEx.Hurt.Model.Enums;
using SolEx.Hurt.Model.Interfaces;

namespace SolEx.Hurt.Core.ModelBLL
{
    [Alias("Atrybut")]
    [FriendlyName("Atrybut", FriendlyOpis = "Atrybuty")]
    public class AtrybutBll : Atrybut
    {
        public AtrybutBll() { }

        public AtrybutBll(AtrybutBll a):base(a)
        {
            if (a == null)
            {
                throw new Exception("Atrybut nie może być pusty w konstruktorze");
            }
        }

        public AtrybutBll(Atrybut a, List<CechyBll> sztucznaListaCech) : base(a)
        {
            this._sztucznaListaCech = sztucznaListaCech;
        }

        private List<CechyBll> _sztucznaListaCech = null;

        [Ignore]
        [FriendlyName("Cechy atrybutu")]
        [WidoczneListaAdmin(false, false, true, false,true, new[] { typeof(AtrybutBll) })]
        [GrupaAtttribute("Lista cech", 1)]
        [WymuszonyTypEdytora(TypEdytora.ListaPodrzedna, typeof(CechyBll))]
        public virtual List<CechyBll> ListaCech
        {
            get
            {
                if (_sztucznaListaCech == null)
                {
                    _sztucznaListaCech = SolexBllCalosc.PobierzInstancje.CechyAtrybuty.PobierzCecheDlaAtrybutu(this.Id, SolexBllCalosc.PobierzInstancje.Konfiguracja.JezykIDDomyslny);
                }
                return _sztucznaListaCech;
            }
        }

        [Ignore]
        public string PoleDoBudowyLinkow { get; set; }
    }
}
