using System.Collections.Generic;
using SolEx.Hurt.Model.Web;

namespace SolEx.Hurt.Model
{
    /// <summary>
    /// Model podsumowania nale¿nosci dla historii dokumentów
    /// </summary>
    public class DocumentSummary
    {
        private readonly Dictionary<string, DaneDoWykresu> _dane=new Dictionary<string, DaneDoWykresu>();

        public Dictionary<string, DaneDoWykresu> Dane
        {
            get { return _dane; }
        }

        private const string KluczPrzetermionowane = "przeterminowane";

        public DaneDoWykresu Przeterminowane
        {
            get
            {
                if (_dane.ContainsKey(KluczPrzetermionowane))
                {
                    return _dane[KluczPrzetermionowane];
                }
                return null;
            }
            set
            {
                if (_dane.ContainsKey(KluczPrzetermionowane))
                {
                    _dane[KluczPrzetermionowane] = value;
                }
                else
                {
                    _dane.Add(KluczPrzetermionowane, value);
                }
            }
        }

        private const string KluczNiezaplacone = "niezap³acone";

        public DaneDoWykresu Niezaplacone
        {
            get
            {
                if (_dane.ContainsKey(KluczNiezaplacone))
                {
                    return _dane[KluczNiezaplacone];
                }
                return null;
            }
            set
            {
                if (_dane.ContainsKey(KluczNiezaplacone))
                {
                    _dane[KluczNiezaplacone] = value;
                }
                else
                {
                    _dane.Add(KluczNiezaplacone, value);
                }
            }
        }

        private const string KluczZaplacone = "zap³acone";

        public DaneDoWykresu Zaplacone
        {
            get
            {
                if (_dane.ContainsKey(KluczZaplacone))
                {
                    return _dane[KluczZaplacone];
                }
                return null;
            }
            set
            {
                if (_dane.ContainsKey(KluczZaplacone))
                {
                    _dane[KluczZaplacone] = value;
                }
                else
                {
                    _dane.Add(KluczZaplacone, value);
                }
            }
        }

        private const string KluczNiezrealizowane = "niezrealizowane";

        public DaneDoWykresu Niezrealizowane
        {
            get
            {
                if (_dane.ContainsKey(KluczNiezrealizowane))
                {
                    return _dane[KluczNiezrealizowane];
                }
                return null;
            }
            set
            {
                if (_dane.ContainsKey(KluczNiezrealizowane))
                {
                    _dane[KluczNiezrealizowane] = value;
                }
                else
                {
                    _dane.Add(KluczNiezrealizowane, value);
                }
            }
        }
        private const string KluczZrealizowane = "zrealizowane";

        public DaneDoWykresu Zrealizowane
        {
            get
            {
                if (_dane.ContainsKey(KluczZrealizowane))
                {
                    return _dane[KluczZrealizowane];
                }
                return null;
            }
            set
            {
                if (_dane.ContainsKey(KluczZrealizowane))
                {
                    _dane[KluczZrealizowane] = value;
                }
                else
                {
                    _dane.Add(KluczZrealizowane, value);
                }
            }
        }
    }
}
