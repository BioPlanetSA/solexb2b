using ServiceStack.DesignPatterns.Model;
using System;
using SolEx.Hurt.Model.Helpers;
using SolEx.Hurt.Model.Interfaces;

namespace SolEx.Hurt.Model
{
    [TworzDynamicznieTabeleAttribute]
    public class ProduktKategoria :   IHasLongId, IPolaIDentyfikujaceRecznieDodanyObiekt
    {
        public long KategoriaId { get; set; }
        public long ProduktId { get; set; }
        public int Rodzaj { get; set; }

        public ProduktKategoria(ProduktKategoria bazowy)
        {
           this.KopiujPola(bazowy);
        }
        public ProduktKategoria(){}

        /// <summary>
        /// Generuje nowy łacznik produkt do kategorii
        /// </summary>
        /// <param name="produkt">id produktu</param>
        /// <param name="kategoria">id kategorii</param>
        public ProduktKategoria(int produkt, int kategoria)
        {
            ProduktId = produkt;
            KategoriaId = kategoria;
        }
        public bool RecznieDodany()
        {
            return KategoriaId < 0 || ProduktId < 0;
        }

        public long Id
        {
            get { return (ProduktId + "||" + KategoriaId).WygenerujIDObiektuSHAWersjaLong(); }
        }
    }
}
