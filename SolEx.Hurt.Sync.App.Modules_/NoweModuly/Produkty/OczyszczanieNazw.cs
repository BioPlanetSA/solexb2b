using System.Reflection;
using System.Text.RegularExpressions;
using SolEx.Hurt.Core.ModelBLL;
using SolEx.Hurt.Model;
using SolEx.Hurt.Model.AtrybutyKlas;
using System;
using System.Collections.Generic;
using System.Linq;
using ServiceStack.Common;
using SolEx.Hurt.Core.Sync;
using SolEx.Hurt.Model.Interfaces.Sync;


namespace SolEx.Hurt.Sync.App.Modules_.NoweModuly.Produkty
{
    [FriendlyName("Usun lub podmie wybrane znaki w polu", FriendlyOpis = "Moduł, który usuwa znaki specjalne z nazw i kodów produktów i zamienia je na wybrany znak. ZNAK A NIE FRAZĘ")]
    public class OczyszczanieNazw : SyncModul, Model.Interfaces.SyncModuly.IModulProdukty
    {
        private static readonly log4net.ILog Log = log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public OczyszczanieNazw()
        {
            ZnakiDoPodmiany = @"[^0-9a-zA-Z-,ążśźćńłęóŻŹŃĄŚŁĘÓ\-&\+.]+";
            CzyRegex = true;
            ZnakDocelowy = " ";
            Pola = new List<string>();
        }

        [FriendlyName("Znaki do podmiany.", FriendlyOpis = "Lista znaków jeden po drugim, nie rozdzielone niczym, lub wyrażenie regex.")]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        public string ZnakiDoPodmiany { get; set; }

        [FriendlyName("Traktuj podane znaki do szukania jako REGEX.")]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        public bool CzyRegex { get; set; }

        [FriendlyName("Docelowy znak na którym będzie zamiana.", FriendlyOpis = "Domyślnie spacja.<br/>Jeśli wpiszesz więcej niż jeden znak użyty PIERWSZY znak.")]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        public string ZnakDocelowy { get; set; }

        [FriendlyName("Lista pól, które mają zostać oczyszczone")]
        [SyncSlownikNaPodstawieInnegoTypu(typeof(Produkt))]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        public List<string> Pola { get; set; }

        //public override string Opis
        //{
        //    get { return "Moduł, który usuwa znaki specjalne z nazw i kodów produktów i zamienia je na wybrany znak."; }
        //}

        public void Przetworz(ref List<Produkt> listaWejsciowa, ref List<Tlumaczenie> produktyTlumaczenia, Dictionary<long, Produkt> produktyNaB2B, ref List<JednostkaProduktu> jednostki, ref Dictionary<long, ProduktCecha> lacznikiCech, ref List<ProduktKategoria> lacznikiKategorii, ISyncProvider provider, ref List<ProduktUkryty> produktuUkryteErp, ref List<ProduktyZamienniki> zamienniki, Dictionary<long, KategoriaProduktu> kategorie, ref List<Cecha> cechy, ref List<Atrybut> atrybuty)
        {
            PropertyInfo[] propertisy = typeof(Produkt).GetProperties();

            foreach (var produkty in listaWejsciowa)
            {
                foreach (var p in propertisy)
                {
                    if (Pola.Contains(p.Name))
                    {
                        try
                        {
                            p.SetValue(produkty, Oczysc(p.GetValue(produkty, null).ToString()), null);

                            //oczyszczanie tłumaczenia dla danego pola
                            foreach (KeyValuePair<int, Jezyk> jezyk in SyncManager.PobierzInstancje.Konfiguracja.JezykiWSystemie)
                            {
                                if (jezyk.Key != SyncManager.PobierzInstancje.Konfiguracja.JezykIDPolski)
                                {
                                    Tlumaczenie tlumaczenienazwy =
                                        produktyTlumaczenia.FirstOrDefault(
                                            a =>
                                            a.ObiektId == produkty.Id && a.JezykId == jezyk.Key &&
                                            a.Pole == p.Name);

                                    if (tlumaczenienazwy != null)
                                    {
                                        tlumaczenienazwy.Wpis = Oczysc(p.GetValue(produkty, null).ToString());
                                    }
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            Log.Error(new Exception("Wystąpił błąd przy przetwarzaniu danych produktu: " + ex.Message, ex));
                            Log.Error(new Exception("pole " + p.Name));
                            Log.Error(new Exception("Towar: " + produkty.Kod));
                        }
                    }
                }
            }
        }

        private string Oczysc(string tekst)
        {
            if (CzyRegex)
            {
                Regex myRegex = new Regex(ZnakiDoPodmiany);
                return myRegex.Replace(tekst, ZnakDocelowy);
            }
            foreach (char c in ZnakiDoPodmiany)
            {
                tekst = tekst.Replace(c, ZnakDocelowy[0]);
            }
            
            return tekst;
        }
    }
}
