using ServiceStack.DataAnnotations;
using SolEx.Hurt.Helpers;
using SolEx.Hurt.Model;
using System.Collections.Generic;
using System.Text;
using SolEx.Hurt.Core.BLL;
using SolEx.Hurt.Model.AtrybutyKlas;
using SolEx.Hurt.Model.Interfaces;

namespace SolEx.Hurt.Core
{
    [Alias("KategoriaProduktu")]
    [FriendlyName("Kategorie produktów")]
    public class KategorieBLL : KategoriaProduktu, IPoleJezyk
    {
        private IList<KategorieBLL> _dzieci;

        [Ignore]
        public IList<KategorieBLL> Dzieci
        {
            get
            {
                if (_dzieci == null)
                {
                    _dzieci = SolexBllCalosc.PobierzInstancje.KategorieDostep.PobierzDzieci(this);
                }
                return _dzieci;
            }
        }

        //autouzupelnienie przy selecie
        [Ignore]
        public GrupaBLL Grupa { get; set; }

        private IObrazek _obrazek = null;

        [Ignore]
        public IObrazek Obrazek
        {
            get
            {
                if (ObrazekId == null)
                {
                    return null;
                }
                if (_obrazek == null)
                {
                    _obrazek = SolexBllCalosc.PobierzInstancje.Pliki.PobierzObrazek(ObrazekId.Value);
                }
                return _obrazek;
            }
        }

        private IObrazek _miniatura = null;

        [Ignore]
        public IObrazek Miniatura
        {
            get
            {
                if (this.MiniaturaId == null)
                {
                    return null;
                }
                if (_miniatura == null)
                {
                    _miniatura = SolexBllCalosc.PobierzInstancje.Pliki.PobierzObrazek(this.MiniaturaId.Value);
                }
                return _miniatura;
            }
        }

        private long[] _wszystkieDzieci = null;

        public long[] PobierzIdWszystkichDzieci()
        {
            if (_wszystkieDzieci == null)
            {
                var ids = new List<long>(5);

                if (this.Dzieci == null)
                {
                    _wszystkieDzieci = new long[0];
                }
                else
                {
                    foreach (KategorieBLL k in Dzieci)
                    {
                        ids.Add(k.Id);
                        ids.AddRange(k.PobierzIdWszystkichDzieci());
                    }
                    _wszystkieDzieci = ids.ToArray();
                }
            }
            return _wszystkieDzieci;
        }

        private long[] _wszystkieNadrzedne = null;
        public long[] PobierzIdWszystkichNadrzednych()
        {
            if (_wszystkieNadrzedne == null && this.ParentId != null)
            {
                var ids = new List<long>(5);

                KategorieBLL aktualna = this;
                while (aktualna != null)
                {
                    aktualna = SolexBllCalosc.PobierzInstancje.KategorieDostep.KategoriaNadrzedna(aktualna);
                    if (aktualna != null)
                    {
                        ids.Add(aktualna.Id);
                    }
                }
                _wszystkieNadrzedne = ids.ToArray();
            }
            return _wszystkieNadrzedne;
        }


        private string _sciezka;

        [Ignore]
        public string Sciezka
        {
            get
            {
                if (_sciezka == null)
                {
                    var sb = new StringBuilder(500);
                    KategorieBLL aktualna = this;
                    while (aktualna != null)
                    {
                        sb.Insert(0, string.Format("{0} / ", aktualna.Nazwa));
                        aktualna = SolexBllCalosc.PobierzInstancje.KategorieDostep.KategoriaNadrzedna(aktualna);
                    }
                    _sciezka= sb.ToString().Trim('/').Trim().Trim('/');
                }
                return _sciezka;
            }
        }

        [Ignore]
        public int JezykId { get; set; }

        [Ignore]
        public IObrazek Zdjecie1
        {
            get
            {
                if (ZdjecieId1 == null) return null;
                return SolexBllCalosc.PobierzInstancje.Pliki.PobierzObrazek(ZdjecieId1.Value);
            }
        }

        [Ignore]
        public IObrazek Zdjecie2
        {
            get
            {
                if (ZdjecieId2 == null) return null;
                return SolexBllCalosc.PobierzInstancje.Pliki.PobierzObrazek(ZdjecieId2.Value);
            }
        }

        [Ignore]
        public IObrazek Zdjecie3
        {
            get
            {
                if (ZdjecieId3 == null) return null;
                return SolexBllCalosc.PobierzInstancje.Pliki.PobierzObrazek(ZdjecieId3.Value);
            }
        }

        [Ignore]
        public IObrazek Zdjecie4
        {
            get
            {
                if (ZdjecieId4 == null) return null;
                return SolexBllCalosc.PobierzInstancje.Pliki.PobierzObrazek(ZdjecieId4.Value);
            }
        }

        [Ignore]
        public IObrazek Zdjecie5
        {
            get
            {
                if (ZdjecieId5 == null) return null;
                return SolexBllCalosc.PobierzInstancje.Pliki.PobierzObrazek(ZdjecieId5.Value);
            }
        }

        public override string ToString()
        {
            return Nazwa;
        }

        //autowyliczane w pobieraniu z bazy
        [Ignore]
        public virtual string FriendlyLinkURL { get; set; }
    }


}
