using SolEx.Hurt.Core.ModelBLL.Interfejsy;
using SolEx.Hurt.Model.Enums;
using SolEx.Hurt.Model.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using SolEx.Hurt.Model.Interfaces;

namespace SolEx.Hurt.Core.BLL.Web
{
    public class TrybyListyProduktow : BllBaza<TrybyListyProduktow>
    {
        private readonly List<TrybListyProduktow> _trybyZalogowanych = new List<TrybListyProduktow>();
        private readonly List<TrybListyProduktow> _trybyNieZalogowanych = new List<TrybListyProduktow>();

        public TrybyListyProduktow()
        {
            UstawTrybyWidokow();
        }

        public List<TrybListyProduktow> WszystkieTryby(IKlient klient)
        {
            if (klient.Dostep == AccesLevel.Zalogowani)
            {
                if (!_trybyZalogowanych.Any())
                {
                    Log.Info(string.Format("Brak elementów w na liscie zalogowanych zwracam liste niezalogowanych"));
                    return _trybyNieZalogowanych;
                }
                return _trybyZalogowanych;
            }
            return _trybyNieZalogowanych;
        }

        private List<TrybListyProduktow> pobierzTrybyListyZwStringa(HashSet<string> stringi)
        {
            List<TrybListyProduktow> lista = new List<TrybListyProduktow>(stringi.Count);

            foreach (string s in stringi)
            {
                if (s.StartsWith("Losowe", StringComparison.CurrentCultureIgnoreCase))
                {
                    continue;
                }
                TrybListyProduktow tmp = new TrybListyProduktow();
                tmp.Nazwa = s;
                tmp.Kolejnosc = 0;
                tmp.Symbol = s;

                switch (s)
                {
                    case "Lista":
                        tmp.Ikona = "th-list";
                        break;

                    case "Kafle":
                        tmp.Ikona = "th-large";
                        break;
                }
                lista.Add(tmp);
            }
            return lista;
        }

        private void UstawTrybyWidokow()
        {
            _trybyZalogowanych.AddRange( pobierzTrybyListyZwStringa( SolexBllCalosc.PobierzInstancje.Konfiguracja.AktywneWidokiListyProduktow(true) ) );
            _trybyNieZalogowanych.AddRange(pobierzTrybyListyZwStringa(SolexBllCalosc.PobierzInstancje.Konfiguracja.AktywneWidokiListyProduktow(false))  );
        }
    }
}