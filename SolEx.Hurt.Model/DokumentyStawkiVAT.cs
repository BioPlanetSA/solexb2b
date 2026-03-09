using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SolEx.Hurt.Model.AtrybutyKlas;
using SolEx.Hurt.Model.Core;

namespace SolEx.Hurt.Model
{

    [FriendlyClassName("Grupa vat na dokumencie")]
    public class DokumentyStawkiVAT
    {
          [FriendlyName("stawka %")]
        public WartoscLiczbowaZaokraglana Stawka { get; set; }
         [FriendlyName("wartość vat z tę stawką")]
          public WartoscLiczbowa WartoscVAT { get; set; }
          [FriendlyName("wartość netto z tę stawką")]
        public WartoscLiczbowa WartoscNetto {get;set;}
                  [FriendlyName("wartość brutto z tę stawką")]
          public WartoscLiczbowa WartoscBrutto { get; set; }

        public  DokumentyStawkiVAT(string waluta)
        {
            Stawka=new WartoscLiczbowaZaokraglana(0);
            WartoscNetto=new WartoscLiczbowa(0,waluta);
            WartoscVAT=new WartoscLiczbowa(0,waluta);
            WartoscBrutto=new WartoscLiczbowa(0,waluta);
        }
    }
}
