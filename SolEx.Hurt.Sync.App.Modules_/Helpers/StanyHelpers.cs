using System;
using System.Collections.Generic;
using System.Linq;
using SolEx.Hurt.Core.Sync;
using SolEx.Hurt.Model;
using SolEx.Hurt.Model.Interfaces.Sync;


namespace SolEx.Hurt.Sync.App.Modules_.Helpers
{
    public static class StanyHelpers
    {
        private static Dictionary<string, Dictionary<long, decimal>> _slownikStanow;
        /// <summary>
        /// Pobiera stan dla magazynu z aktualnego providera. Obsługuje łączenie stanów z kilku magazynów za pomocą + w symbolu magazynu.
        /// </summary>
        /// <param name="aktualnyProvider"></param>
        /// <param name="m">Magazyn z kórrego chcemy pobrać stan</param>
        /// <returns></returns>
        public static Dictionary<long, decimal> PobierzStanyDlaMagazynow(ISyncProvider aktualnyProvider, Magazyn m)
        {
            if(_slownikStanow==null) _slownikStanow = new Dictionary<string, Dictionary<long, decimal>>();

            Dictionary<long, decimal> stanyDoWyslania = new Dictionary<long, decimal>();
            
            string[] listaDoWyslania = m.Symbol.Split('+').Select(x => x.Trim()).ToArray();
            foreach (string symbol in listaDoWyslania)
            {
                Dictionary<long, decimal> paczka = null;
                if (!_slownikStanow.TryGetValue(symbol, out paczka))
                {
                    try
                    {
                        paczka = aktualnyProvider.PobierzStanyDlaMagazynu(symbol);
                    }
                    catch (Exception ex)
                    {
                        LogiFormatki.PobierzInstancje.LogujError(new Exception("Błąd przetwarzania stanów dla magazynu " + symbol + ". Magazyn jest pomijany", ex));
                    }
                    _slownikStanow.Add(symbol,paczka);
                }
                if (paczka == null) continue;
                foreach (var s in paczka)
                {
                    if (stanyDoWyslania.ContainsKey(s.Key))
                    {
                        stanyDoWyslania[s.Key] += s.Value;
                    }
                    else
                    {
                        stanyDoWyslania.Add(s.Key, s.Value);
                    }
                }
            }
            return stanyDoWyslania;
        }
    }
}
