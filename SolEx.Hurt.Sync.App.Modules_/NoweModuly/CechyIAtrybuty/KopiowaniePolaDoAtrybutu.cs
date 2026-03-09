using System.Reflection;
using SolEx.Hurt.Core.ModelBLL;
using SolEx.Hurt.Helpers;
using SolEx.Hurt.Model;
using SolEx.Hurt.Model.AtrybutyKlas;
using SolEx.Hurt.Model.CustomSearchCriteria;
using System;
using System.Collections.Generic;
using System.Linq;
using SolEx.Hurt.Core.Sync;
using SolEx.Hurt.Model.Helpers;
using SolEx.Hurt.Model.Interfaces.Sync;


namespace SolEx.Hurt.Sync.App.Modules_.NoweModuly.CechyIAtrybuty
{
    public class KopiowaniePolaDoAtrybutu : SyncModul, SolEx.Hurt.Model.Interfaces.SyncModuly.IModulCechyIAtrybuty, Model.Interfaces.SyncModuly.IModulProdukty
    {

        [FriendlyName("Nazwa atrybutu który zostanie stworzony na podstawie pola")]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        public string Atrybut { get; set; }

        [FriendlyName("Pole,z którego zostanie stworzona cecha dla wybranego atrybutu")]
        [SyncSlownikNaPodstawieInnegoTypu(typeof(Produkt))]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        public string Pole { get; set; }

        public KopiowaniePolaDoAtrybutu()
        {
            Atrybut = "";
            Pole = "";
        }

        public override string uwagi
        {
            get { return ""; }
        }

        public override string Opis
        {
            get { return "Tworzy atrybuty na podstawie wybranego pola produktu"; }
        }

        public void Przetworz(ref List<Produkt> listaWejsciowa, ref List<Tlumaczenie> produktyTlumaczenia, Dictionary<long, Produkt> produktyNaB2B, ref List<JednostkaProduktu> jednostki, ref Dictionary<long, ProduktCecha> lacznikiCech, ref List<ProduktKategoria> lacznikiKategorii, ISyncProvider provider, ref List<ProduktUkryty> produktuUkryteErp, ref List<ProduktyZamienniki> zamienniki, Dictionary<long, KategoriaProduktu> kategorie, ref List<Cecha> cechy, ref List<Atrybut> atrybuty)
        {
            //var atrybuty = ApiWywolanie.PobierzAtrybuty().Values.ToList();
            //var cechy = ApiWywolanie.PobierzCechy().Values.ToList();

            Przetworz(ref atrybuty, ref cechy, ref lacznikiCech, listaWejsciowa);

        }

        public void Przetworz(ref List<Atrybut> atrybuty, ref List<Cecha> cechy, Dictionary<long, Produkt> produktyNaB2B)
        {
            Dictionary<long, ProduktCecha> cechyprodukty = new Dictionary<long, ProduktCecha>();
           // var produkty = ApiWywolanie.PobierzProdukty().Values.ToList();
            Przetworz(ref atrybuty, ref cechy, ref cechyprodukty, produktyNaB2B.Values.ToList());
        }

        private void Przetworz(ref List<Atrybut> atrybuty, ref List<Cecha> cechy, ref Dictionary<long, ProduktCecha> cechyprodukty, List<Produkt> listaWejsciowa)
        {
            if (string.IsNullOrEmpty(Atrybut) || string.IsNullOrEmpty(Pole))
            {
                LogiFormatki.PobierzInstancje.LogujInfo("Moduł kończy działanie pole Atrybut lub pole Pole nie są ustawione. Wartości Pole {0}, Atrybut {1}",Pole,Atrybut);
                return;
            }
            PropertyInfo[] propertisy = typeof(Produkt).GetProperties();

            Atrybut atrybut = atrybuty.FirstOrDefault(a => a.Nazwa.ToLower() == Atrybut.ToLower());
            if (atrybut == null)
            {
                atrybut = new Atrybut();
                atrybut.Nazwa = Atrybut;
                atrybut.Widoczny = true;
                atrybut.Id = atrybut.WygenerujIDObiektuSHA(1);
                atrybuty.Add(atrybut);
                LogiFormatki.PobierzInstancje.LogujDebug("Dodanie atrybutu "+ atrybut.Nazwa +" id "  +atrybut.Id);
            }

            foreach (Produkt produkt in listaWejsciowa)
            {

                foreach (var p in propertisy)
                {
                    string nazwa = p.Name;
                    if (nazwa == Pole)
                    {
                        object wartosc = p.GetValue(produkt);

                        Type t = p.GetType().PobierzPodstawowyTyp();
                        if (t == typeof(bool))
                        {
                            wartosc = (bool) p.GetValue(produkt) ? "Tak" : "Nie";
                        }
                        else wartosc = p.GetValue(produkt);

                        if (wartosc == null)
                             continue;

                            Cecha nowaCecha = UtworzCeche(atrybut, wartosc.ToString());
                            DodajCeche(nowaCecha, cechy);
                            DodajLacznikCechy(produkt, nowaCecha, ref cechyprodukty);
                    }
                }
            }
        }

        private Cecha UtworzCeche(Atrybut atrybut, string wartosc)
        {
            Cecha cecha = new Cecha();
            cecha.AtrybutId = atrybut.Id;
            cecha.Nazwa = wartosc;
            cecha.Symbol = string.Format("{0}:{1}", atrybut.Nazwa, wartosc).ToLower();
            cecha.Widoczna = true;
            cecha.Id = cecha.WygenerujIDObiektuSHA(1);

            return cecha;
        }

        private void DodajCeche(Cecha cecha, List<Cecha> listacech)
        {
            if(!listacech.AsParallel().Any(a=> a.Id == cecha.Id && a.Symbol == cecha.Symbol))
                listacech.Add(cecha);
        }

        private void DodajLacznikCechy(Produkt produkt, Cecha cecha, ref Dictionary<long, ProduktCecha> cechyprodukty)
        {
            ProduktCecha cp = new ProduktCecha();
            cp.ProduktId = produkt.Id;
            cp.CechaId = cecha.Id;

            if(!cechyprodukty.ContainsKey(cp.Id) )
                cechyprodukty.Add(cp.Id, cp);
        }
    }
}
