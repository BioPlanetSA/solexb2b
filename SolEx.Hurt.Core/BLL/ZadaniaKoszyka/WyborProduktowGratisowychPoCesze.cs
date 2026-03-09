using System;
using SolEx.Hurt.Core.Helper;
using SolEx.Hurt.Model.AtrybutyKlas;
using System.Collections.Generic;
using System.Linq;
using ServiceStack.Common;

namespace SolEx.Hurt.Core.BLL.ZadaniaKoszyka
{
    public class WyborProduktowGratisowychPoCesze : WyborProdoktowBaza
    {
        public ICechyProduktyDostep CechyProduktyDostep = SolexBllCalosc.PobierzInstancje.CechyProduktyDostep;

        public override string Opis
        {
            get { return "Wybór produtków gratisowych. Produkty muszą mieć określoną cechę"; }
        }

        [FriendlyName("Cecha oznaczająca produkt gratisowy")]
        [PobieranieSlownika(typeof(SlownikCech))]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        public List<string> Cecha { get; set; }

        public HashSet<long> CechyId
        {
            get { return new HashSet<long>( Cecha.Select(long.Parse) ); }
        }

        protected override HashSet<long> PobierzProduktyID()
        {
            return CechyProduktyDostep.PobierzProduktyZCechami(CechyId);
        }
    }
}