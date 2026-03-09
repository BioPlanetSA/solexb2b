using System;
using ServiceStack.Common;
using SolEx.Hurt.Core.BLL.Interfejsy;
using SolEx.Hurt.Model;
using System.Collections.Generic;
using System.Linq;

namespace SolEx.Hurt.Core.BLL
{
    public class ProduktyStanBll : BllBazaCalosc, IProduktyStanBll
    {
        public ProduktyStanBll(SolexBllCalosc calosc) : base(calosc)
        {
        }

     
        /// <summary>
        /// 
        /// </summary>
        /// <param name="nazwyMagazynow"></param>
        /// <param name="idProduktu">musi byc podane id bazowe - nie produktu klienta z powdu produktow wirtualnych</param>
        /// <returns></returns>
     
        private Dictionary<int, Dictionary<long, decimal>> _stany = null;


        public Dictionary<int, Dictionary<long, decimal>> WszystkieStanyDlaMagazynow()
        {
            if (_stany == null)
            {
                Dictionary<int, Dictionary<long, decimal>>wynik = new Dictionary<int, Dictionary<long, decimal>>();
                var wszystkieStany =Calosc.DostepDane.Pobierz<ProduktStan>(null);
                foreach (var s in wszystkieStany)
                {                    
                    if (!wynik.ContainsKey(s.MagazynId))
                    {
                        wynik.Add(s.MagazynId, new Dictionary<long, decimal>());
                    }
                    wynik[s.MagazynId].Add(s.ProduktId,s.Stan);
                }
                //jesli magazyn jest na platformie a nie ma stanów to dodajemy bo on ciągle jest ma platformie. 
                var magazyny = Calosc.Konfiguracja.SlownikMagazynowPoId.Select(x => x.Key);
                foreach (int i in magazyny)
                {
                    if (!wynik.ContainsKey(i))
                    {
                        wynik.Add(i,new Dictionary<long, decimal>());
                    }
                }
                _stany = wynik;
            }
            return _stany;
           
        }

        public List<ProduktStan> PobierzStanyProduktowNieZerowe(Magazyn mag)
        {
            return SolexBllCalosc.PobierzInstancje.DostepDane.Pobierz<ProduktStan>(null, x => x.MagazynId == mag.Id).ToList();
        }

        private Magazyn _domyslnyMagazyn = null;

        public Magazyn MagazynDomyslny()
        {
            if (_domyslnyMagazyn == null)
            {
                var magPdst = Calosc.Konfiguracja.SlownikMagazynowPoId.Values.FirstOrDefault(x => x.MagazynRealizujacy);
                if (magPdst == null)
                {
                    magPdst = Calosc.Konfiguracja.SlownikMagazynowPoId.Values.FirstOrDefault();
                }
                if (magPdst == null)
                {
                    throw new Exception("Brak magazynów - nie można pokazywać ilości produktów");
                }

                _domyslnyMagazyn = magPdst;
            }
            return _domyslnyMagazyn;
        }
        //Trzeba się Bartka zpaytać jak to ma wyglądać w sytuacji gdy zamawiamy 15sztuk na magazynie realizujacym jest 13 czy te dwie sztuki odejmujemy z jakiegos innego magazynu czy popraostu odejmujemy te 13 zeby na tym magazynie bylo 0
        /// <summary>
        /// 
        /// </summary>
        /// <param name="produktID">musi być podane id bazowe - nie produktu klienta z powodu produktów wirtualnych</param>
        /// <param name="ilosc"></param>
        /// <param name="magazynPostawowy"></param>
        public void ZmniejszStany(long produktID, decimal ilosc, string magazynPostawowy)
        {
            List<ProduktStan> doZmiany = new List<ProduktStan>();
            var idMagazynu = Calosc.DostepDane.PobierzPojedynczy<Magazyn>(x => x.Symbol == magazynPostawowy, null).Id;
            var stan = SolexBllCalosc.PobierzInstancje.DostepDane.PobierzPojedynczy<ProduktStan>(x => x.MagazynId == idMagazynu && x.ProduktId == produktID, null);
            if (stan == null){return;}
            decimal odejmowane = stan.Stan >= ilosc ? ilosc : stan.Stan;
            stan.Stan -= odejmowane;
            doZmiany.Add(stan);
            SolexBllCalosc.PobierzInstancje.DostepDane.AktualizujListe<ProduktStan>(doZmiany);
            
        }

        /// <summary>
        /// Metoda która sprawdza czy aktualizowany magazyn został zmieniony na realizujący w sytuacji gdy tak było stary magazyn realizujący przestaje być realizującym
        /// </summary>
        /// <param name="obj"></param>
        public void UstawMagazynRealizujacy(IList<Magazyn> obj)
        {

            var magazynRealizujący = obj.FirstOrDefault(x => x.MagazynRealizujacy);
            if (magazynRealizujący != null)
            {
                var staryMagazynRealizujacy = Calosc.Konfiguracja.SlownikMagazynowPoId.Values.FirstOrDefault(x => x.MagazynRealizujacy);
                //Sprawdzamy czy już był magazyn realizujący jak był oraz symbol pokrywa się z nowym to pomijamy bo jest to ten sam magazyn
                if (staryMagazynRealizujacy != null)
                {
                    if (staryMagazynRealizujacy.Symbol.Equals(magazynRealizujący.Symbol, StringComparison.InvariantCultureIgnoreCase))
                    {
                        return;
                    }
                    //Sprawdzamy czy w aktualizowanych jest stary magazyn realizujący, jeżeli jest to wyłączamy mu funkcje magazynu realizującego
                    var staryRealizujacy = obj.FirstOrDefault(x => x.Id == staryMagazynRealizujacy.Id);
                    if (staryRealizujacy != null)
                    {
                        staryRealizujacy.MagazynRealizujacy = false;
                    }
                    //Wyłączamy funkcje magazynu realizującego i dodajemy dokolekcji aktualizowanych
                    else
                    {
                        staryMagazynRealizujacy.MagazynRealizujacy = false;
                        obj.Add(staryMagazynRealizujacy);
                    }
                }
                
            }


            ////Sprawdzenie to jest ze względu na fakt iż Magazyn podstawowy mozna ustawic tylko i wyłacznie z poziomu administratora przez co do aktualizacji bedzie tylko jeden magazyn.
            //if (obj.Count == 1 && obj.First().MagazynRealizujacy)
            //{
            //    var obj1 = obj;
            //    var magazyny = Calosc.Konfiguracja.SlownikMagazynowPoId.Values.FirstOrDefault(x => x.MagazynRealizujacy && x.Symbol != obj1.First().Symbol);//SolexBllCalosc.PobierzInstancje.DostepDane.PobierzPojedynczy<Magazyn>(x => x.MagazynRealizujacy && x.Symbol != obj1.First().Symbol, null);
            //    foreach (Magazyn t in magazyny)
            //    {
            //        t.MagazynPodstawowy = false;
            //        obj.Add(t);
            //    }
            //}
        }


        public void UsunCache(IList<object> obj)
        {
            _stany = null;
        }
    }
}