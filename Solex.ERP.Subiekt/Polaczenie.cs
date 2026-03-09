using System;
using InsERT;

namespace SolEx.ERP.SubiektGT
{
    public class Polaczenie
    {
        private static Subiekt _subiektInstancja = null;
        private static Navireo _navireoInstancja = null;
        private static string _agentName = null;
        private static string _agentPassword = null;
        private static string _cs = null;
        private static string _podmiot = null;
        private static bool _szyfruj = false;
        public static void KillSubiekt()
        {
            try
            {
                if (_subiektInstancja != null)
                {
                    _subiektInstancja.Zakoncz();
                    _subiektInstancja = null;
                }
            }
            catch
            {
            }
            try
            {
                if (_navireoInstancja != null)
                {
                    _navireoInstancja.Zakoncz();
                    _navireoInstancja = null;
                }
            }
            catch
            {
            }
       
        }

        public static void UstawParametryPolaczenia(string agentName, string agentPassword, string cs,bool szyfruj)
        {
            _agentName = agentName;
            _agentPassword = agentPassword;
            _cs = cs;
            _szyfruj = szyfruj;
        }
        public static void UstawParametryPolaczenia(string agentName, string agentPassword, string cs,string podmiot,bool szyfruj)
        {

            _podmiot = podmiot;
            UstawParametryPolaczenia(agentName, agentPassword, cs,szyfruj);
        }
        public static string ConnectionString
        {
            get
            {
                if (string.IsNullOrEmpty(_cs))
                {
                    throw new Exception("Brak ustawionych parametrów połączenia");
                }
                return _cs;
            }
        }

        public static Subiekt GetSubiekt
        {
            get
            {
                if (_agentName == null  || _cs == null)
                {
                    throw new Exception("Brak ustawionych parametrów połączenia Subiekta !");
                }

                if (_subiektInstancja == null)
                {
                    _subiektInstancja = GetSubiektInstancja();
                }
                return _subiektInstancja;
            }
        }
        public static Navireo GetNavireo
        {
            get
            {
                if (_agentName == null || _agentPassword == null || _cs == null)
                {
                    throw new Exception("Brak ustawionych parametrów połączenia Navireo !");
                }

                if (_navireoInstancja == null)
                {
                    _navireoInstancja = GetNavireoInstancja();
                }
                return _navireoInstancja;
            }
        }
        private static Subiekt GetSubiektInstancja()
        {
            GT gt = new GTClass();
            Dodatki dodatki = new DodatkiClass();
            string serwer = string.Empty;
            string baza = string.Empty;
            string uzytkownik = string.Empty;
            string haslo = string.Empty;
            string connStr = _cs;
         
            foreach (string str in connStr.Split(';'))
            {

                string[] arr = str.Split('=');

                switch (arr[0].Trim().ToLower())
                {

                    case "server":

                    case "data source":
                        serwer = arr[1].Trim();
                        break;

                    case "initial catalog":
                        baza = arr[1].Trim();
                        break;
                    case "database":
                        baza = arr[1].Trim();
                        break;
                    case "user id":
                        uzytkownik = arr[1].Trim();
                        break;

                    case "password":
                        haslo = arr[1].Trim();
                        break;
                }
            }
            string pass = _agentPassword ?? "";
            string passsql = haslo;
            if (_szyfruj)
            {
                if (!string.IsNullOrEmpty(pass))
                {
                    pass = dodatki.Szyfruj(pass);
                }
                if (!string.IsNullOrEmpty(haslo))
                {
                    passsql = dodatki.Szyfruj(haslo);
                }
            }
      
            gt.Serwer = serwer;
            gt.Baza = baza;
            if (!string.IsNullOrEmpty(_podmiot))
            {
                gt.Baza = _podmiot;
            }
            gt.Uzytkownik = uzytkownik;
            gt.UzytkownikHaslo = passsql;
            gt.Produkt = ProduktEnum.gtaProduktSubiekt;
            gt.Autentykacja = (string.IsNullOrEmpty(uzytkownik) ? AutentykacjaEnum.gtaAutentykacjaWindows : AutentykacjaEnum.gtaAutentykacjaMieszana);
            gt.Operator = _agentName;
            gt.OperatorHaslo = pass;
                return (Subiekt)gt.Uruchom((int)UruchomDopasujEnum.gtaUruchomDopasuj, (int)UruchomEnum.gtaUruchomWTle);
        }
        private static Navireo GetNavireoInstancja()
        {
            GT gt = new GTClass();
            Dodatki dodatki = new DodatkiClass();
            string serwer = string.Empty;
            string baza = string.Empty;
            string uzytkownik = string.Empty;
            string haslo = string.Empty;
            string connStr = _cs;
            foreach (string str in connStr.Split(';'))
            {

                string[] arr = str.Split('=');

                switch (arr[0].Trim().ToLower())
                {

                    case "server":

                    case "data source":
                        serwer = arr[1].Trim();
                        break;

                    case "initial catalog":
                        baza = arr[1].Trim();
                        break;

                    case "user id":
                        uzytkownik = arr[1].Trim();
                        break;

                    case "password":
                        haslo = arr[1].Trim();
                        break;
                }
            }

                try
                {
                   //string[] name= agentName.Split(new string[]{" "},StringSplitOptions.RemoveEmptyEntries);
                   
                   Navireo sub = new Navireo();

                    sub.Zaloguj(serwer, (string.IsNullOrEmpty(uzytkownik) ? AutentykacjaEnum.gtaAutentykacjaWindows : AutentykacjaEnum.gtaAutentykacjaMieszana),uzytkownik,haslo,baza,"","",AutentykacjaEnum.gtaAutentykacjaMieszana,
                       Pracownicy.PobierzIdPracownika(_agentName), _agentPassword);
                    return sub;
                }

            catch
            {
                throw;
            }
        }
    }
}
