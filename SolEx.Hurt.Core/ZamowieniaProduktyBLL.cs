using System;
using System.Linq;
using System.Runtime.InteropServices;
using SolEx.Hurt.Core.BLL;
using SolEx.Hurt.Core.BLL.Interfejsy;
using SolEx.Hurt.Core.ModelBLL;
using SolEx.Hurt.Core.ModelBLL.Interfejsy;
using SolEx.Hurt.Model;
using SolEx.Hurt.Model.AtrybutyKlas;
using SolEx.Hurt.Model.Core;
using ServiceStack.DataAnnotations;

namespace SolEx.Hurt.Core
{
    [Alias("ZamowienieProdukt")]
    public class ZamowieniaProduktyBLL : ZamowienieProdukt, IPozycjaDokumentuBll
    {    
        private IConfigBLL _configbll =  SolexBllCalosc.PobierzInstancje.Konfiguracja;

        public ZamowieniaProduktyBLL()
        {
            
        }

        public ZamowieniaProduktyBLL(ZamowienieProdukt baza):base(baza)
        {
        }

        public ZamowieniaProduktyBLL(ZamowienieProdukt baza, string walutaB2B = null) : base(baza)
        {
            if (!string.IsNullOrEmpty(walutaB2B))
            {
                this.walutaB2b = walutaB2B;
            }
        }


        public ZamowieniaProduktyBLL(ZamowienieProdukt baza,IKoszykPozycja pozycja):this(baza)
        {
            ProduktId = pozycja.ProduktId;
            ProduktIdBazowy = pozycja.ProduktBazowyId;
            Ilosc = pozycja.Ilosc;
            this.JednostkaMiary = pozycja.Jednostka().Id;
            this.UstawJednostke(pozycja.Jednostka().Nazwa);
            base.JednostkaPrzelicznik = pozycja.Jednostka().Przelicznik;
            CenaNetto = pozycja.DoklanaCenaNetto();
            CenaBrutto = pozycja.DoklanaCenaBruto();
       
            DataZmiany = pozycja.DataZmiany;
            PrzedstawicielId = pozycja.PrzedstawicielId;
            
            this.walutaB2b = pozycja.Waluta();
        }

        /// <summary>
        /// ustawiane przy wyciaganiu automatycznie
        /// </summary>
        [Ignore]
        public virtual ProduktBazowy ProduktBazowy { get; set; }
    }
}
