using ServiceStack.DataAnnotations;
using SolEx.Hurt.Core.BLL;
using System.Collections.Generic;
using System.Linq;
using ServiceStack.Common;
using SolEx.Hurt.Core.ModelBLL.Interfejsy;
using SolEx.Hurt.Model;
using SolEx.Hurt.Model.AtrybutyKlas;
using SolEx.Hurt.Model.Enums;
using SolEx.Hurt.Model.Interfaces;

namespace SolEx.Hurt.Core
{
    [Alias("Zamowienie")]
    [FriendlyName("Zamowienie")]
    [NieSprawdzajCzyIsnieje]
    public class ZamowieniaBLL : Zamowienie, IDokument
    {   
        public ZamowieniaBLL()
        {
        }

        [Ignore]
        [WidoczneListaAdmin(true, true, false, false)]
        [FriendlyName("Nazwa klienta")]
        public string NazwaKlienta
        {
            get { return Klient.Nazwa; }
        }
        
        //[Ignore]
        //public virtual IKlient Klient
        //{
        //    get
        //    {
        //        if (_klient == null)
        //        {
        //            _klient =SolexBllCalosc.PobierzInstancje.DostepDane.PobierzPojedynczy<Klient>(KlientId);
        //        }
        //        return _klient;
        //    }
        //}


        public ZamowieniaBLL(Zamowienie baza)
            : base(baza)
        {

        }


        [FriendlyName("Nazwa dokumentu")]
        [Ignore]
        [WidoczneListaAdmin(true, true, false, false)]
        public virtual string DokumentNazwa
        {
            get
            {
                var dokNazwa = new HashSet<string>( SolexBllCalosc.PobierzInstancje.DostepDane.Pobierz<ZamowienieDokumenty>(null,x => x.IdZamowienia == Id).Select(x => x.NazwaERP) ) ;
                if (dokNazwa != null && dokNazwa.Any())
                {
                    return string.Join(" ", dokNazwa);
                }

                return NumerTymczasowyZamowienia;
            }
        }

        [FriendlyName("Nazwa pracownika składającego zamówienie")]
        [Ignore]
        [WidoczneListaAdmin(true, true, false, false)]  
        public string ZlozonePrzezPracownikaNazwa
        {
            get
            {
                if (PracownikSkladajacyId != null )
                {
                    var k = SolexBllCalosc.PobierzInstancje.DostepDane.PobierzPojedynczy<Klient>(PracownikSkladajacyId.Value);
                    return k == null ? "" : k.Nazwa;
                }
                return null;
            }
        }
        
        [Ignore]
        public IKlient DokumentPlatnik
        {
            get { return SolexBllCalosc.PobierzInstancje.DostepDane.PobierzPojedynczy<Klient>(KlientId); }
        }
        
        [Ignore]
        public IKlient DokumentOdbiorca
        {
            get { return SolexBllCalosc.PobierzInstancje.DostepDane.PobierzPojedynczy<Klient>(KlientId); }
        }

        public virtual IEnumerable<ZamowieniaProduktyBLL> PobierzPozycjeDokumentu()
        {
            if (this.Id < 0)
            {
                return SolexBllCalosc.PobierzInstancje.DostepDane.Pobierz<ZamowieniaProduktyBLL>(this.Klient, x => x.DokumentId == this.Id, this).ToList();
            }
            return null;
        }

        public virtual decimal WagaZamowienia()
        {
            var dok = PobierzPozycjeDokumentu().Where(x => x.ProduktBazowy != null);
            decimal wagaCalkowita = 0m;
            foreach (var zamowieniaProduktyBll in dok)
            {
                var waga = zamowieniaProduktyBll.ProduktBazowy.Waga;
                if (waga.HasValue)
                {
                    wagaCalkowita += waga.Value* zamowieniaProduktyBll.Ilosc;
                }
            }
            return wagaCalkowita;
        }

        public virtual decimal ObjetoscZamowienia()
        {
            return this.PobierzPozycjeDokumentu().Where(x => x.ProduktBazowy != null).Where(x => x.ProduktBazowy.Objetosc.HasValue).Sum(x=> x.ProduktBazowy.Objetosc.Value);
        }

    }
}
