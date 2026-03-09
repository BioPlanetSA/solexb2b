using SolEx.Hurt.Core.BLL;
using SolEx.Hurt.Core.Importy.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using SolEx.Hurt.Core.ModelBLL;
using SolEx.Hurt.Core.ModelBLL.Interfejsy;
using SolEx.Hurt.Model.CustomSearchCriteria;
using SolEx.Hurt.Model;
using SolEx.Hurt.Model.Helpers;

namespace SolEx.Hurt.Core.Importy
{
    [Serializable]
    public class CenyPoziomyImport : PrzetwarzanieCsv<PoziomyCen>
    {
        public override bool ImportDanych(IKlient zadajacy, object dane, List<object> parametry, List<WierszMapowania> mapowania, ref List<object> zmienioneDane)
        {
            PoziomyCen data = (PoziomyCen)dane;
            IList<poziomy_cen> poziomyCen = SolexBllCalosc.PobierzInstancje.PoziomyCenDostep.Wszystykie();
            string waluta = "";
            var criteria = new ProduktySearchCriteria();
            if (data.produkt_id == 0)
            {
                criteria.kod.Add(data.SymbolProduktu);
            }
            else
            {
                criteria.produkt_id.Add(data.produkt_id);
            }
            ProduktBazowy pk = SolexBllCalosc.PobierzInstancje.ProduktyBazowe.Pobierz(criteria, ConfigBLL.PobierzInstancje.JezykIDPolski, zadajacy).FirstOrDefault();
            if (pk == null)
            {
                throw new Exception("Brak produktu");
            }
            if (data.produkt_id == 0)
            {
               data.produkt_id = pk.produkt_id;
            }
            if (mapowania.Any(p => p.Pole == "CenaNettoZWaluta") && !string.IsNullOrEmpty(data.CenaNettoZWaluta))
            {
                decimal wartosc;
                if (!TextHelper.PobierzInstancje.SprobojSparsowac(data.CenaNettoZWaluta, out wartosc))
                {


                    int idx = data.CenaNettoZWaluta.LastIndexOf(' ');

                    string wart = data.CenaNettoZWaluta;
                    if (idx > -1)
                    {
                        wart = data.CenaNettoZWaluta.Substring(0, idx + 1);
                    }

                    TextHelper.PobierzInstancje.SprobojSparsowac(wart, out wartosc);
                  
                    if (idx > 1)
                    {
                        waluta = data.CenaNettoZWaluta.Substring(idx).Trim();
                        data.Waluta = waluta;
                    }
                }
                data.netto = wartosc;
                
            }
            if (mapowania.Any(p => p.Pole == "PoziomCenowy"))
            {
                var pc = poziomyCen.FirstOrDefault(p => p.nazwa == data.PoziomCenowy);
                if (pc != null)
                {
                    data.poziom_id = pc.id;
                }
                else
                {
                    int poziom_Id = poziomyCen.Max(p => p.id) + 1;
                    SolexBllCalosc.PobierzInstancje.PoziomyCenDostep.Aktualizuj(new poziomy_cen { nazwa = data.PoziomCenowy, id = poziom_Id, waluta = waluta  });
                  

                    data.poziom_id = poziom_Id;
                }
            }
            if (data.poziom_id == 0)
            {
              if (poziomyCen.Count == 0)
              {
                  SolexBllCalosc.PobierzInstancje.PoziomyCenDostep.Aktualizuj( new poziomy_cen { nazwa = "Domyślny", id = 1, waluta = waluta } );
                  data.poziom_id = 1;
              }
              else
              {
                  data.poziom_id = poziomyCen.First().id;
              }
            }
            if (data.produkt_id != 0 && data.poziom_id != 0)
            {
                ceny_poziomy poziomIstniejacy = SolexBllCalosc.PobierzInstancje.CenyPoziomy.Wszystykie().FirstOrDefault(x => x.produkt_id == pk.produkt_id && x.poziom_id == data.poziom_id);
                if (poziomIstniejacy != null)
                {
                    data.Id = poziomIstniejacy.Id;
                    zmienioneDane.Add(new ceny_poziomy(data));
                }
                else
                {
                    zmienioneDane.Add(new ceny_poziomy(data)); 
                }
            }
            
            return true;
        }

        public override List<object> EksportDanych(IKlient zadajacy, List<object> parametry, out string nazwa)
        {
            List<object> dane=new List<object>();
            HashSet<int> prod = ProduktyUkryteBll.PobierzInstancje.PobierzProduktyDostepneDlaKlienta(zadajacy);
            nazwa = "PoziomyCen";
            var ceny = SolexBllCalosc.PobierzInstancje.CenyPoziomy.Wszystykie();
            foreach (ceny_poziomy poziomyCen in ceny)
            {
                if (prod.Contains(poziomyCen.produkt_id) && poziomyCen.poziom_id == ConfigBLL.PobierzInstancje.GetPriceLevelDetal)
                {
                 PoziomyCen cp=new PoziomyCen(poziomyCen);
                 poziomy_cen poziom =      SolexBllCalosc.PobierzInstancje.PoziomyCenDostep.Pobierz( cp.poziom_id);
                    cp.Waluta = poziom.waluta;
                    cp.PoziomCenowy = poziom.nazwa;
                    cp.SymbolProduktu = SolexBllCalosc.PobierzInstancje.ProduktyBazowe.Pobierz(cp.produkt_id, ConfigBLL.PobierzInstancje.JezykIDPolski).kod;
                    cp.CenaNettoZWaluta = string.Format("{0} {1}", cp.netto, cp.Waluta);
                    cp.kod_kreskowy = SolexBllCalosc.PobierzInstancje.ProduktyBazowe.Pobierz(cp.produkt_id, ConfigBLL.PobierzInstancje.JezykIDPolski).kod_kreskowy;
                    dane.Add(cp);

                }
            }
            return dane;
        }
        public override void PoImporcie(IKlient zadajacy, List<object> parametry, List<WierszMapowania> mapowania, ref List<object> zmienioneDane)
        {


            if (zmienioneDane.Any())
            {
                int max = 10;
                var ceny = zmienioneDane.Select(x => (ceny_poziomy) x).ToList();
                while (ceny.Count > 0)
                {
                    int dopobrania = ceny.Count > max ? max : ceny.Count;
                    var listaidklientow = ceny.Take(dopobrania).ToList();

                    SolexBllCalosc.PobierzInstancje.CenyPoziomy.Aktualizuj(listaidklientow);
                    ceny.RemoveRange(0, dopobrania);
                }

            }
        }
    }
}
