using System;
using System.Collections.Generic;
using System.IO;
using ServiceStack.Net30.Collections.Concurrent;
using SolEx.Hurt.Core.BLL;

namespace SolEx.Hurt.Web
{
    public  class ObslugaSzablonow
    {
        private static  ObslugaSzablonow Instancja=new ObslugaSzablonow();
            
   
        public static ObslugaSzablonow PobierzInstancje
        {
            get { return Instancja; }
        }
        private  HashSet<string> listaNadpisanychSzablonowASPXASCX = null;

        private  HashSet<string> ListaNadpisanychSzablonowASPXASCX
        {
            get
            {
                if (listaNadpisanychSzablonowASPXASCX == null && !string.IsNullOrEmpty(SolEx.Hurt.Core.BLL.SolexBllCalosc.PobierzInstancje.Konfiguracja.SzablonNiestandardowyNazwa))
                {
                    //jesli nie ma sciezki to ja tworzymy
                    if (!System.IO.Directory.Exists(SolEx.Hurt.Core.BLL.SolexBllCalosc.PobierzInstancje.Konfiguracja.SzablonNiestandardowySciezkaBezwzgledna))
                    {
                        Directory.CreateDirectory(SolEx.Hurt.Core.BLL.SolexBllCalosc.PobierzInstancje.Konfiguracja.SzablonNiestandardowySciezkaBezwzgledna);
                    }
                    string[] pliki = System.IO.Directory.GetFiles(SolEx.Hurt.Core.BLL.SolexBllCalosc.PobierzInstancje.Konfiguracja.SzablonNiestandardowySciezkaBezwzgledna, "*.as?x", SearchOption.AllDirectories);
                    listaNadpisanychSzablonowASPXASCX = new HashSet<string>();
                    foreach (string p in pliki)
                    {
                        listaNadpisanychSzablonowASPXASCX.Add(p.Replace(SolEx.Hurt.Core.BLL.SolexBllCalosc.PobierzInstancje.Konfiguracja.SzablonNiestandardowySciezkaBezwzgledna, "").Replace("\\","/"));
                    }
                    szablonyPodmienione = new Dictionary<string, string>();
                }
                return listaNadpisanychSzablonowASPXASCX;
            }
        }

        private ConcurrentDictionary<string, bool> CzyKontrolkaJestMVC = new ConcurrentDictionary<string, bool>();

        private  bool czyKontrolkaJestMVC(string sciezkaPodstawowa)
        {
            if (!CzyKontrolkaJestMVC.ContainsKey(sciezkaPodstawowa))
            {
                bool jest = !File.Exists(sciezkaPodstawowa);
                CzyKontrolkaJestMVC.TryAdd(sciezkaPodstawowa, jest);
            }
            return CzyKontrolkaJestMVC[sciezkaPodstawowa];
            
        }

        private  Dictionary<string, string> szablonyPodmienione = new Dictionary<string, string>();
        private  bool _jestZdefiniowanaSciezka = true;
        public  string PobierzSciezkeSzablonuPodmienionego(string sciezkaPodstawowa, bool podanaSciezkaJestWzgledna)
        {
            if (czyKontrolkaJestMVC(AppDomain.CurrentDomain.BaseDirectory + sciezkaPodstawowa))
            {
                return null;
            }

            if (!_jestZdefiniowanaSciezka || string.IsNullOrEmpty(SolEx.Hurt.Core.BLL.SolexBllCalosc.PobierzInstancje.Konfiguracja.SzablonNiestandardowyNazwa))
            {
                _jestZdefiniowanaSciezka = false;
                return sciezkaPodstawowa;
            }
            string zwroconaNowaSciezka = null;
            if (!szablonyPodmienione.TryGetValue(sciezkaPodstawowa, out zwroconaNowaSciezka))
            {
                string sciezkaWzgledna = sciezkaPodstawowa;
                if (!podanaSciezkaJestWzgledna)
                {
                    //bezwzgledne trzeba wzglednic
                    sciezkaWzgledna = sciezkaPodstawowa.Replace(AppDomain.CurrentDomain.BaseDirectory, "");
                }

                if (ListaNadpisanychSzablonowASPXASCX.Contains(sciezkaPodstawowa))
                {
                    if (!podanaSciezkaJestWzgledna)
                    {
                        szablonyPodmienione.Add(sciezkaPodstawowa, SolEx.Hurt.Core.BLL.SolexBllCalosc.PobierzInstancje.Konfiguracja.SzablonNiestandardowySciezkaWzgledna + sciezkaWzgledna);
                        zwroconaNowaSciezka = SolEx.Hurt.Core.BLL.SolexBllCalosc.PobierzInstancje.Konfiguracja.SzablonNiestandardowySciezkaWzgledna + sciezkaWzgledna;
                    }
                    else
                    {
                        szablonyPodmienione.Add(sciezkaPodstawowa, SolEx.Hurt.Core.BLL.SolexBllCalosc.PobierzInstancje.Konfiguracja.SzablonNiestandardowySciezkaWzgledna + sciezkaWzgledna);
                        zwroconaNowaSciezka = SolEx.Hurt.Core.BLL.SolexBllCalosc.PobierzInstancje.Konfiguracja.SzablonNiestandardowySciezkaWzgledna + sciezkaWzgledna;
                    }
                }
                else
                {
                    szablonyPodmienione.Add(sciezkaPodstawowa, null);
                }
            }
            if (string.IsNullOrEmpty(zwroconaNowaSciezka))
            {
                return sciezkaPodstawowa;
            }
            return zwroconaNowaSciezka;
        }
    }

}
