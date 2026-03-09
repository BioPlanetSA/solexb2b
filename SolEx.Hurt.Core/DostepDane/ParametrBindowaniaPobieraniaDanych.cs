using System;
using System.Linq.Expressions;
using SolEx.Hurt.Model.Interfaces;

namespace SolEx.Hurt.Core.DostepDane
{
    public interface IParametrBindowaniaPobieraniaDanych
    {
        /// <summary>
        /// Sposób cachowania danych
        /// </summary>
        SposobCachowania SposobCachowania { get; set; }

        /// <summary>
        /// Typ do pobierania lokalizacji
        /// </summary>
        Type TypDoLokalizacji { get; }

        Delegate ZwracanieDanych { get; }

        /// <summary>
        /// Czy filtrowanie ma być na poziomie SQL
        /// </summary>
        bool FiltrySql { get; }

        Delegate MetodaPrzetwarzajacaPoSelect { get; }
    }

    /// <summary>
    /// Klasa reprezentująca opis bindowania typu
    /// </summary>
    public class ParametrBindowaniaPobieraniaDanych<TDane> : IParametrBindowaniaPobieraniaDanych
    {
        public ParametrBindowaniaPobieraniaDanych(SposobCachowania cache, Expression<Func<TDane, IKlient, bool>> walidatorDanych, Delegate metodaZwracajacaDane, Type typLokalizacje, bool filtrySql, Delegate metodaPrzetwarzajacaPoSelect)
        {
            _filtrySql = filtrySql;
            _typLokalizacje = typLokalizacje;

            if (walidatorDanych != null)
            {
                _walidator = walidatorDanych;
                WalidatorSkompilowany = _walidator.Compile();

                //pozwalamy na kLIENTA - to jest wyjatek, mozna zrobic atrybut dla walidatora ale juz nie chcialem dlatego ze TYLKO klientowi pozwalamy na ominiecie zadajacego
                if (typeof(TDane) == typeof(Klient) || typeof(TDane) == typeof(Model.Klient))
                {
                    Walidator_WymuszajPodanieZadajacego = false;
                }

            }

            _sposobCachowania = cache;
            _zwracanieDanych = metodaZwracajacaDane;
            _metodaPrzetwarzajacaPoSelect = metodaPrzetwarzajacaPoSelect;
        }


        private readonly bool _filtrySql;
        private readonly Type _typLokalizacje;
        private readonly Expression<Func<TDane, IKlient, bool>> _walidator;

        private readonly Delegate _zwracanieDanych;
        private readonly Delegate _metodaPrzetwarzajacaPoSelect;
        private SposobCachowania _sposobCachowania;

        /// <summary>
        /// Sposób cachowania danych
        /// </summary>
        public SposobCachowania SposobCachowania
        {
            get { return _sposobCachowania; }
            set { _sposobCachowania = value; }
        }
        /// <summary>
        /// Typ do pobierania lokalizacji
        /// </summary>
        public Type TypDoLokalizacji
        {
            get { return _typLokalizacje; }
        }
        public Expression<Func<TDane, IKlient, bool>> Walidator
        {
            get { return _walidator; }
        }

        public bool Walidator_WymuszajPodanieZadajacego = true;

        public Func<TDane, IKlient, bool> WalidatorSkompilowany { get; private set; }

        public Delegate ZwracanieDanych
        {
            get { return _zwracanieDanych; }
        }
        /// <summary>
        /// Czy filtrowanie ma być na poziomie SQL
        /// </summary>
        public bool FiltrySql
        {
            get { return _filtrySql; }
        }

        public Delegate MetodaPrzetwarzajacaPoSelect
        {
            get { return _metodaPrzetwarzajacaPoSelect; }
        }
    }
}
