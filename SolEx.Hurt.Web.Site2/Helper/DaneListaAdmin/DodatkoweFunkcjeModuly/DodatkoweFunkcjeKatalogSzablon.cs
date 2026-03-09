using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using SolEx.Hurt.Core.BLL;
using SolEx.Hurt.Core.ModelBLL;
using SolEx.Hurt.Helpers;
using SolEx.Hurt.Model.AtrybutyKlas;
using SolEx.Hurt.Model.Enums;
using SolEx.Hurt.Model.Helpers;
using SolEx.Hurt.Model.Web;
using SolEx.Hurt.Web.Site2.Controllers;

namespace SolEx.Hurt.Web.Site2.Helper.DaneListaAdmin.DodatkoweFunkcjeModuly
{
    public class DodatkoweFunkcjeKatalogSzablon: DodatkoweFunkcjeBaza
    {
        public override Type OblugiwanyTyp()
        {
            return typeof(KatalogSzablonModelBLL);
        }

        public override IList<DodatkowaFunkcja> PobierzDodatkoweFunkcjeDlaObiektu(object o)
        {
            return null;
        }

        public override List<Komunikat> KomunitatyNaEdycjiObiektu(object o)
        {
            return null;
        }

        public override List<Komunikat> KomunitatyNaLiscieObiektu(Type o)
        {
            List<Komunikat> wynik = new List<Komunikat>();

            List<PlikIntegracjiSzablon> zrdodlaDanych = SolexBllCalosc.PobierzInstancje.Konfiguracja.PobierzListePlikowIntegracji[TypDanychIntegracja.ProduktyKatalogDrukowanie];
                if (zrdodlaDanych.Any())
                {
                    StringBuilder komunikat = new StringBuilder("Testowe pliki danych wymagany do tworzenia szablonów: ");
                    foreach (PlikIntegracjiSzablon zrodlo in zrdodlaDanych)
                    {
                        string url = Url.ZbudujLinkDoTestowychDanychKataloguProduktow(zrodlo);
                        string typPrzyjazny;
                        try
                        {
                            typPrzyjazny = zrodlo.typDanych.PobierzAtrybutDlaEnuma<FriendlyNameAttribute>().FriendlyName;
                        }
                        catch (Exception e)
                        {
                            throw new Exception("Nie udało się pobrać friendly name z typu danych: " + zrodlo.typDanych, e);
                        }
                        komunikat.AppendFormat("<a href='{0}'>{1} [{2}]</a>, ", url, zrodlo.SzablonLadnaNazwa, typPrzyjazny);

                    }
                    wynik.Add(new Komunikat(komunikat.ToString().TrimEnd(", "), KomunikatRodzaj.info));
                }
          
            return wynik;
        }

        public override bool? MoznaUsuwacObiekt(object o)
        {
            return null;
        }

        public override bool? MoznaEdytowacObiekt(object o)
        {
            return null;
        }
    }
}