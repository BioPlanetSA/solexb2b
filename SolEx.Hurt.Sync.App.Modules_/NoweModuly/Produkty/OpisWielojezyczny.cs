using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using SolEx.Hurt.Core.ModelBLL;
using SolEx.Hurt.Helpers;
using SolEx.Hurt.Model;
using SolEx.Hurt.Model.AtrybutyKlas;
using System;
using System.Collections.Generic;
using System.Linq;
using SolEx.Hurt.Core.Sync;
using SolEx.Hurt.Model.Helpers;
using SolEx.Hurt.Model.Interfaces.Sync;



namespace SolEx.Hurt.Sync.App.Modules_.NoweModuly.Produkty
{
    public class OpisWielojezyczny : SyncModul, Model.Interfaces.SyncModuly.IModulProdukty
    {
        
        public OpisWielojezyczny()
        {
            Regex = @"[-]{4}(?<symbolJezyka>[A-Za-z]{2})-*(?<tresc>.*)";
            WymaganyCiag = "----";
            UsunacHTML = true;
        }

        public override string uwagi
        {
            get { return ""; }
        }

        [FriendlyName("Wyrażenie regularne do wyciągania opisów.")]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        public string Regex { get; set; }

        
        [FriendlyName("Ciąg, który jest wymagany w opisie. (opcjonalnie)")]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        public string WymaganyCiag { get; set; }

        [FriendlyName("Czy oczyścić opis z kodu HTML przy użyciu Tidy HTML")]
        [WidoczneListaAdminAttribute(false, false, true, false)]
        public bool UsunacHTML { get; set; }

        public override string Opis
        {
            get { return "Wyciąga wielojęzyczne opisy z pola opis z ERP"; }
        }

        public void Przetworz(ref List<Produkt> listaWejsciowa, ref List<Tlumaczenie> produktyTlumaczenia, Dictionary<long, Produkt> produktyNaB2B, ref List<JednostkaProduktu> jednostki, ref Dictionary<long, ProduktCecha> lacznikiCech, ref List<ProduktKategoria> lacznikiKategorii, ISyncProvider provider, ref List<ProduktUkryty> produktuUkryteErp, ref List<ProduktyZamienniki> zamienniki, Dictionary<long, KategoriaProduktu> kategorie, ref List<Cecha> cechy, ref List<Atrybut> atrybuty)
        {
            //1. dajsz ustawienie czy html tidy zrobic (wywalic caly HTML - tak / nie)
            //2. replace na calosci zrobi do jezyka zrodlowgo to co zostanie

            Dictionary<int, Jezyk> jezyki = SyncManager.PobierzInstancje.Konfiguracja.JezykiWSystemie;
            int i = 0;
            Dictionary<long, Tlumaczenie> slownikiTlumaczen = new Dictionary<long, Tlumaczenie>();
            LogiFormatki.PobierzInstancje.LogujInfo("Uruchamianie modułu OpisWielojezyczny");

            string typ = (typeof(ProduktBazowy)).PobierzOpisTypu();

            Regex re = new Regex(Regex, RegexOptions.Singleline);
            foreach (var produkty in listaWejsciowa)
            {
                if (UsunacHTML)
                {
                    produkty.Opis = (produkty.Opis ?? "").UsunFormatowanieHTML();
                }

                if(i++ % 100 == 0)
                    LogiFormatki.PobierzInstancje.LogujInfo(string.Format("Produkt (symbol: {2}) {0} z {1}", i, listaWejsciowa.Count, produkty.Kod));
                    
                if (string.IsNullOrEmpty(produkty.Opis))
                    continue;

                if (!string.IsNullOrEmpty(WymaganyCiag) && !produkty.Opis.Contains(WymaganyCiag))
                    continue;

                string ostatniSymbolJezyka = "";
                string opis = produkty.Opis;
              PropertyInfo[] propertisy = typeof(Produkt).GetProperties();

              var polezrodlowe = propertisy.FirstOrDefault(a => a.Name == "opis");

              foreach (string gn in re.GetGroupNames())
              {


                  if (gn.StartsWith("symbolJezyka"))
                  {
                      ostatniSymbolJezyka = re.Match(opis).Groups[gn].Value.ToLower();
                     // polezrodlowe.SetValue(produkty, opis.Replace(WymaganyCiag+ostatniSymbolJezyka,"").TrimEnd('-'), null);
                  }

                  

                  //jeśli nie ma jezyka to ustawiamy pole o takiej nazwie
                  //if (string.IsNullOrEmpty(ostatniSymbolJezyka))
                  //{
                  //    if (polezrodlowe != null)
                  //    {
                  //        try
                  //        {
                  //            if (string.IsNullOrEmpty(polePierwszegoOpisu))
                  //            {
                  //                polePierwszegoOpisu = gn;
                  //            }

                  //            polezrodlowe.SetValue(produkty, re.Match(opis).Groups[polePierwszegoOpisu].Value, null);
                            
                  //        }
                  //        catch (Exception ex)
                  //        {
                  //            Log.Error("błąd przy przetwarzaniu towaru " + ex.Message, ex);
                  //        }
                  //    }
                  //}
                  //else 
                      if (!string.IsNullOrEmpty(ostatniSymbolJezyka) && ostatniSymbolJezyka != re.Match(opis).Groups[gn].Value.ToLower())
                  {
                     
                          KeyValuePair<int, Jezyk> j = jezyki.FirstOrDefault(a => a.Value.Symbol.ToLower() == ostatniSymbolJezyka);
                          if (j.Value != null)
                          {
                              string nowyOpis = Tools.PobierzInstancje.PoprawHTML(re.Match(opis).Groups[gn].Value, Encoding.Default);
                              string brudnyOpis = re.Match(opis).Groups[gn].Value;
                              Tlumaczenie s = produktyTlumaczenia.FirstOrDefault(p => p.ObiektId == produkty.Id && p.JezykId == j.Key && p.Pole == "opis");
                              if (s == null)
                              {
                                  s = new Tlumaczenie();
                                  s.JezykId = j.Key;
                                  s.ObiektId = produkty.Id;
                                  s.Pole = "opis";
                                  s.Typ = typ;
                                  s.Wpis = nowyOpis;
                                
                                  if (!slownikiTlumaczen.ContainsKey(s.Id))
                                      slownikiTlumaczen.Add(s.Id, s);
                              }
                              else s.Wpis = nowyOpis;
                              polezrodlowe.SetValue(produkty, Tools.PobierzInstancje.PoprawHTML(opis.Replace(WymaganyCiag + ostatniSymbolJezyka, "").Replace(brudnyOpis, "").TrimEnd('-'), Encoding.Default), null);
                      }
                         
                      ostatniSymbolJezyka = "";
                  }
              }
            }
            produktyTlumaczenia.AddRange(slownikiTlumaczen.Values);
        

            LogiFormatki.PobierzInstancje.LogujInfo("Koniec pracy modułu OpisWielojezyczny");
        }
    }
}
