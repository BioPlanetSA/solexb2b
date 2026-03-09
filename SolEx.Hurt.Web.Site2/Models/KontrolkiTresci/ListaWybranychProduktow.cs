using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ServiceStack.Common;
using SolEx.Hurt.Core;
using SolEx.Hurt.Core.BLL;
using SolEx.Hurt.Core.ModelBLL;
using SolEx.Hurt.Model.AtrybutyKlas;
using SolEx.Hurt.Model.Enums;
using SolEx.Hurt.Web.Site2.Models.KontrolkiTresci.Bazowe;

namespace SolEx.Hurt.Web.Site2.Models.KontrolkiTresci
{
    public class ListaWybranychProduktow: LosowaListaProduktowWybraneIdProduktow
    {
        public ListaWybranychProduktow()
        {
            Produkt = false;
        }

        [FriendlyName("Jakie produkty chcesz wyświetlić")]
        [WidoczneListaAdmin(true, true, true, true)]
        [Niewymagane]
        public TypProduktuDoWiswietlania TypProduktow { get; set; }

        private bool Produkt { get; set; }

        public long IdElementu
        {
            get
            {
                var id = PobierzIdentyfikator("produktId", false);
                if (id != null)
                {
                    Produkt = true;
                }
                else
                {
                    id = PobierzIdentyfikator("BlogWpisId", false);
                }
                return id!=null? long.Parse(id.ToString()):0;
            }
        }

        public override HashSet<long> ListaProduktowId
        {
            get
            {
                HashSet<long> wszystkie;
                if (ListaProduktow != null && ListaProduktow.Any())
                {
                    wszystkie = new HashSet<long>(SolexBllCalosc.PobierzInstancje.ProduktyKlienta.PobierzIdProduktowRzeczywiscieDostepnychDlaKlienta(this.AktualnyKlient));
                    return new HashSet<long>( wszystkie.Intersect(ListaProduktow) ) ;
                }

                ProduktBazowy produkt = null;
                BlogWpisBll blog = null;
                PropertyInfo property = null;
                MethodInfo metoda = null;
                HashSet<long> porduktyPobrane = null;
                long id = IdElementu;

                //z produktu pobiermay czy z blogow
                if (Produkt)
                {
                    produkt = SolexBllCalosc.PobierzInstancje.DostepDane.PobierzPojedynczy<ProduktKlienta>(id, AktualnyKlient);
                    switch (TypProduktow)
                    {
                        case TypProduktuDoWiswietlania.Zamienniki:
                            property = typeof(ProduktKlienta).GetProperty(TypProduktow.ToString());
                            if (property != null)
                            {
                                Dictionary<long, bool> zamienniki = property.GetValue(produkt) as Dictionary<long, bool>;
                                if (zamienniki != null) porduktyPobrane = new HashSet<long>( zamienniki.Keys );
                            }
                            break;
                        case TypProduktuDoWiswietlania.ZamiennikiJednostrone:
                            property = typeof(ProduktKlienta).GetProperty(TypProduktuDoWiswietlania.Zamienniki.ToString());
                            if (property != null)
                            {
                                Dictionary<long, bool> zamienniki = property.GetValue(produkt) as Dictionary<long, bool>;
                                if (zamienniki != null) porduktyPobrane = new HashSet<long>( zamienniki.Where(x=>!x.Value).Select(x=>x.Key) );
                            }
                            break;
                        case TypProduktuDoWiswietlania.ZamiennikiDwustronne:
                            property = typeof(ProduktKlienta).GetProperty(TypProduktuDoWiswietlania.Zamienniki.ToString());
                            if (property != null)
                            {
                                Dictionary<long, bool> zamienniki = property.GetValue(produkt) as Dictionary<long, bool>;
                                if (zamienniki != null) porduktyPobrane = new HashSet<long>( zamienniki.Where(x => x.Value).Select(x => x.Key) );
                            }
                            break;
                        default:
                            property = typeof(ProduktKlienta).GetProperty(TypProduktow.ToString());
                            if (property != null)
                            {
                                porduktyPobrane = property.GetValue(produkt) as HashSet<long>;
                            }
                            else
                            {
                                metoda = typeof(ProduktKlienta).GetMethod(TypProduktow.ToString());
                                porduktyPobrane = metoda.Invoke(produkt, null) as HashSet<long>;
                            }
                            break;
                    }
                }
                else
                {
                    blog = SolexBllCalosc.PobierzInstancje.DostepDane.PobierzPojedynczy<BlogWpisBll>(id);
                    if (blog == null)
                    {                        
                        return null;
                    }
                    property = typeof(BlogWpisBll).GetProperty(TypProduktow.ToString());
                    if (property != null)
                    {
                        porduktyPobrane = property.GetValue(blog) as HashSet<long>;
                    }
                    else
                    {
                        metoda = typeof(ProduktKlienta).GetMethod(TypProduktow.ToString());
                        porduktyPobrane = metoda.Invoke(blog, null) as HashSet<long>;
                    }
                }

                if (porduktyPobrane == null)
                {
                    return null;
                }

                wszystkie = new HashSet<long>(SolexBllCalosc.PobierzInstancje.ProduktyKlienta.PobierzIdProduktowRzeczywiscieDostepnychDlaKlienta(this.AktualnyKlient));
                return new HashSet<long>( wszystkie.Intersect(porduktyPobrane) );
            }
            set { ListaProduktow = value; }
        }

        public override string Grupa
        {
            get { return "Produkty"; }
        }

        public override string Nazwa
        {
            get { return "Lista wybranych produktów"; }
        }
        public override string Opis { get { return "Kontrolka dostępna we wpisach bloga oraz na szczegółach produktów. Można ją również wykorzystać w innym miejscu jawnie podając jakei produkty chcemy wyświetlić"; } }
    }
}