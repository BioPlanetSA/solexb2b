using System;
using ServiceStack.DataAnnotations;
using ServiceStack.DesignPatterns.Model;
using SolEx.Hurt.Model.AtrybutyKlas;
using SolEx.Hurt.Model.Helpers;
using SolEx.Hurt.Model.Interfaces;

namespace SolEx.Hurt.Model
{
   public class Konfekcje: IHasLongId
    {
        [PrimaryKey]
        [WidoczneListaAdmin(true, true,false,false)]
        public long Id
        {
            get
            {
                return (string.Format("{0}||{1}||{2}||{3}||{4}||{5}||{6}||{7}", KategoriaKlientowId, KlientId,
                    ProduktId, Ilosc.ToString("F2"), RabatKwota != null ? RabatKwota.Value.ToString("F2") : "",
                    Rabat != null ? Rabat.Value.ToString("F2") : "", WalutaId, CechaId)).WygenerujIDObiektuSHAWersjaLong();
            }
        }
        [WidoczneListaAdmin(true, true, false, false)]
        [FriendlyName("Klient id")]
        public long? KlientId{ get; set; }
        [WidoczneListaAdmin(true, true, false, false)]
        [FriendlyName("Kategoria klienta id")]
        [PobieranieSlownika("SolEx.Hurt.Core.Helper.SlownikKategoriiKlienta,SolEx.Hurt.Core")]
        public int? KategoriaKlientowId	{ get; set; }
        [WidoczneListaAdmin(true, true, false, false)]
        [FriendlyName("Produkt id")]
        [PobieranieSlownika("SolEx.Hurt.Core.Helper.SlownikProduktow,SolEx.Hurt.Core")]
        public long? ProduktId { get; set; }
        [WidoczneListaAdmin(true, true, false, false)]
        [FriendlyName("Ilość")]
        public decimal Ilosc { get; set; }
        [WidoczneListaAdmin(true, true, false, false)]
        [FriendlyName("Rabat")]
        public decimal? Rabat { get; set; }
        [WidoczneListaAdmin(true, true, false, false)]
        [FriendlyName("Kwota rabatu")]
        public decimal? RabatKwota { get; set; }

        [WidoczneListaAdmin(true, true, false, false)]
        [FriendlyName("Waluta")]
        [PobieranieSlownika("SolEx.Hurt.Core.Helper.SlownikWalut,SolEx.Hurt.Core")]
        public long WalutaId { get; set; }

        [WidoczneListaAdmin(true, true, false, false)]
        [PobieranieSlownika("SolEx.Hurt.Core.Helper.SlownikCech,SolEx.Hurt.Core")]
        [FriendlyName("Cecha")]
        public long? CechaId { get; set; }

        [WidoczneListaAdmin(true, true, false, false)]
        [FriendlyName("Id modułu na podstawie którego została wyliczona gradacja")]
        public int? WyliczonePrzezModul { get; set; }

       public Konfekcje()
       {
       }
       public Konfekcje(Konfekcje baza)
       {
           KlientId = baza.KlientId;
           KategoriaKlientowId = baza.KategoriaKlientowId;
           ProduktId = baza.ProduktId;
           Ilosc = baza.Ilosc;
           Rabat = baza.Rabat;
           RabatKwota = baza.RabatKwota;
           WalutaId = baza.WalutaId;
           CechaId = baza.CechaId;
           WyliczonePrzezModul = baza.WyliczonePrzezModul;
       }


       public decimal PoliczCeneZGradacji(decimal cenaNettoBazowa)
       {
            if (this.RabatKwota.HasValue)
            {
                return this.RabatKwota.GetValueOrDefault();
            }
           if (this.Rabat.HasValue)
           {
               return cenaNettoBazowa*(100 - this.Rabat.GetValueOrDefault())/100M;
           }
           return cenaNettoBazowa;
       }
        
    
    }
}
