using ServiceStack.DesignPatterns.Model;
using SolEx.Hurt.Model.AtrybutyKlas;
using System;
using SolEx.Hurt.Model.Helpers;
using SolEx.Hurt.Model.Interfaces;

namespace SolEx.Hurt.Model
{
    /// <summary>
    /// Model poziomu cenowego
    /// </summary>
    [ApiTypeDescriptor(ApiGrupy.Produkty, "Model poziomu cenowego" )]
    public class CenaPoziomu :IHasLongId, Core.IDocumentApiTypeVisible, IPolaIDentyfikujaceRecznieDodanyObiekt
    {
        public static long wyliczKlucz(long produktId, int poziomId)
        {
            string klucz = produktId + "||" + poziomId;
            return klucz.WygenerujIDObiektuSHAWersjaLong();
        }

        public long Id => wyliczKlucz(this.ProduktId, this.PoziomId);

        [UpdateColumnKey]
        [FriendlyName("Id produktu")]
        public long ProduktId { get; set; }
        [UpdateColumnKey]
        [FriendlyName("Id poziomu cenowego")]
        public int PoziomId { get; set; }
        [FriendlyName("Waluta")]
        public long? WalutaId { get; set; }
        [FriendlyName("Cena netto produktu na tym poziomie bez waluty")]
        public decimal Netto { get; set; }
        
        public CenaPoziomu(int zrodloId,  decimal netto, long  productId,long? walutaId=null)
        {
            PoziomId = zrodloId;
            Netto = netto;
            ProduktId = productId;
            WalutaId = walutaId;
        }

        public CenaPoziomu()
        {
        }
        public CenaPoziomu(CenaPoziomu baza)
        {
            PoziomId = baza.PoziomId;
            Netto = baza.Netto;
            ProduktId = baza.ProduktId;
            WalutaId = baza.WalutaId;
        }
        public bool RecznieDodany()
        {
            return PoziomId < 0 || ProduktId < 0;
        }
    }
}
