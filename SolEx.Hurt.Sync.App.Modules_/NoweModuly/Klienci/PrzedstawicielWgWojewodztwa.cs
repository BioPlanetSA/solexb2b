using System;
using SolEx.Hurt.Core.ModelBLL;
using SolEx.Hurt.Model;
using System.Collections.Generic;
using System.Linq;
using SolEx.Hurt.Model.AtrybutyKlas;
using SolEx.Hurt.Model.Interfaces.Sync;
using Adres = SolEx.Hurt.Model.Adres;

namespace SolEx.Hurt.Sync.App.Modules_.NoweModuly.Klienci
{
    public class PrzedstawicielWgWojewodztwa : SyncModul, Model.Interfaces.SyncModuly.IModulKlienci
    {

        [FriendlyName("WiadomoscEmail dla woj. dolnośląskiego")]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        public string dolnośląskie { get; set; }
        
        [FriendlyName("WiadomoscEmail dla woj. kujawsko-pomorskiego")]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        public string kujawskopomorskie { get; set; }
        
        [FriendlyName("WiadomoscEmail dla woj. lubelskiego")]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        public string lubelskie { get; set; }
        
        [FriendlyName("WiadomoscEmail dla woj. lubuskiego")]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        public string lubuskie { get; set; }
        
        [FriendlyName("WiadomoscEmail dla woj. łódzkiego")]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        public string łódzkie { get; set; }
        
        [FriendlyName("WiadomoscEmail dla woj. małopolskiego")]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        public string małopolskie { get; set; }
        
        [FriendlyName("WiadomoscEmail dla woj. mazowieckiego")]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        public string mazowieckie { get; set; }
        
        [FriendlyName("WiadomoscEmail dla woj. opolskiego")]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        public string opolskie { get; set; }
        
        [FriendlyName("WiadomoscEmail dla woj. podkarpackiego")]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        public string podkarpackie { get; set; }
        
        [FriendlyName("WiadomoscEmail dla woj. podlaskiego")]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        public string podlaskie { get; set; }
        
        [FriendlyName("WiadomoscEmail dla woj. pomorskiego")]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        public string pomorskie { get; set; }
        
        [FriendlyName("WiadomoscEmail dla woj. śląskiego")]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        public string śląskie { get; set; }
        
        [FriendlyName("WiadomoscEmail dla woj. świętokrzyskiego")]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        public string świętokrzyskie { get; set; }
        
        [FriendlyName("WiadomoscEmail dla woj. warmińsko-mazurskiego")]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        public string warmińskomazurskie { get; set; }
        
        [FriendlyName("WiadomoscEmail dla woj. wielkopolskiego")]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        public string wielkopolskie { get; set; }
        
        [FriendlyName("WiadomoscEmail dla woj. zachodniopomorskiego")]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        public string zachodniopomorskie { get; set; }

        public PrzedstawicielWgWojewodztwa()
        {
            dolnośląskie = string.Empty;
            kujawskopomorskie = string.Empty;
            lubelskie = string.Empty;
            lubuskie = string.Empty;
            łódzkie = string.Empty;
            małopolskie = string.Empty;
            mazowieckie = string.Empty;
            opolskie = string.Empty;
            podkarpackie = string.Empty;
            podlaskie = string.Empty;
            pomorskie = string.Empty;
            śląskie = string.Empty;
            świętokrzyskie = string.Empty;
            warmińskomazurskie = string.Empty;
            wielkopolskie = string.Empty;
            zachodniopomorskie = string.Empty;
        }

        public override string uwagi
        {
            get { return ""; }
        }


        public override string Opis
        {
            get
            {
                return "Moduł, który dopasowuje przedstawiciela dla klienta na podstawie województwa";
            }
        }

        public void Przetworz(ref Dictionary<long, Klient> listaWejsciowa, Dictionary<long, Produkt> produktyB2B, ref Dictionary<Adres, KlientAdres> adresyWErp, List<KategoriaKlienta> kategorie, List<KlientKategoriaKlienta> laczniki, ref List<Sklep> sklepy, ref List<SklepKategoriaSklepu> sklpeylaczniki, ref List<KategoriaSklepu> sklepyKategorie, ref List<Kraje> kraje, ref List<Region> regiony, ref List<Magazyn> magazyny, ISyncProvider provider)
        {
            Dictionary<Region, string> wojewodztwaPrzedstawiciele = new Dictionary<Region, string>(16);
            //wojewodztwaPrzedstawiciele.Add(Wojewodztwa.dolnośląskie, dolnośląskie);
            //wojewodztwaPrzedstawiciele.Add(Wojewodztwa.kujawskopomorskie, kujawskopomorskie);
            //wojewodztwaPrzedstawiciele.Add(Wojewodztwa.lubelskie, lubelskie);
            //wojewodztwaPrzedstawiciele.Add(Wojewodztwa.lubuskie, lubuskie);
            //wojewodztwaPrzedstawiciele.Add(Wojewodztwa.łódzkie, łódzkie);
            //wojewodztwaPrzedstawiciele.Add(Wojewodztwa.małopolskie, małopolskie);
            //wojewodztwaPrzedstawiciele.Add(Wojewodztwa.mazowieckie, mazowieckie);
            //wojewodztwaPrzedstawiciele.Add(Wojewodztwa.opolskie, opolskie);
            //wojewodztwaPrzedstawiciele.Add(Wojewodztwa.podkarpackie, podkarpackie);
            //wojewodztwaPrzedstawiciele.Add(Wojewodztwa.podlaskie, podlaskie);
            //wojewodztwaPrzedstawiciele.Add(Wojewodztwa.pomorskie, pomorskie);
            //wojewodztwaPrzedstawiciele.Add(Wojewodztwa.śląskie, śląskie);
            //wojewodztwaPrzedstawiciele.Add(Wojewodztwa.świętokrzyskie, świętokrzyskie);
            //wojewodztwaPrzedstawiciele.Add(Wojewodztwa.warmińskomazurskie, warmińskomazurskie);
            //wojewodztwaPrzedstawiciele.Add(Wojewodztwa.wielkopolskie, wielkopolskie);
            //wojewodztwaPrzedstawiciele.Add(Wojewodztwa.zachodniopomorskie, zachodniopomorskie);


            if (wojewodztwaPrzedstawiciele.Values.Where(a => string.IsNullOrEmpty(a)).Count() == 16)
            {
                Log.Debug("Nie wpisano żadnego adresu email. Moduł nie będzie uruchomiony.");
                return;
            }
            foreach (Klient k in listaWejsciowa.Values)
            {
                Adres adres = adresyWErp.Where(a => a.Value.KlientId == k.Id).Select(x=>x.Key).FirstOrDefault();
                if (adres != null)
                {
                    throw new NotImplementedException("adresy");
                    //if (adres.Wojewodztwo != null && wojewodztwaPrzedstawiciele.ContainsKey(adres.Wojewodztwo.Value) &&
                    //    !string.IsNullOrEmpty(wojewodztwaPrzedstawiciele[adres.Wojewodztwo.Value]))
                    //{
                    //    string emailPrzedstawiciela = wojewodztwaPrzedstawiciele[adres.Wojewodztwo.Value];
                    //    if (emailPrzedstawiciela != k.email)
                    //    {
                    //        klienci przedstawiciel =
                    //            listaWejsciowa.Values.FirstOrDefault(a => a.email == emailPrzedstawiciela);
                    //        if (przedstawiciel != null)
                    //            k.przedstawiciel_id = przedstawiciel.klient_id;
                    //    }
                    //}
                }
            }
        }
    }
}

