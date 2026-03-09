using SolEx.Hurt.Core.Helper;
using SolEx.Hurt.Core.ModelBLL.Interfejsy;
using SolEx.Hurt.Model.AtrybutyKlas;
using SolEx.Hurt.Model.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SolEx.Hurt.Core.BLL.RegulyKoszyka
{
    public class ProduktKategoria : RegulaKoszyka, IRegulaPozycji //, ITestowalna
    {
        public override string Opis
        {
            get { return "Czy produkt nalezy do którejś z kategorii"; }
        }

        [FriendlyName("Kategorie")]
        [PobieranieSlownika(typeof(SlownikKategoriiProduktow))]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        public List<string> Kategorie { get; set; }

        private List<long> KategorieId
        {
            get
            {
                List<long> wynik = new List<long>();

                foreach (var kat in Kategorie)
                {
                    int idKategorii;
                    if (Int32.TryParse(kat, out idKategorii))
                    {
                        wynik.Add(idKategorii);
                    }
                    //nik.Add(
                }
                return wynik;               
            }
        }

        public bool PozycjaSpelniaRegule(IKoszykPozycja pozycja, IKoszykiBLL koszyk)
        {
            return pozycja.Produkt.KategorieId.Overlaps(KategorieId);
        }
    }
}