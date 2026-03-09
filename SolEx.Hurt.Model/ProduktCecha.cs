using System;
using ServiceStack.DesignPatterns.Model;
using SolEx.Hurt.Model.AtrybutyKlas;
using SolEx.Hurt.Model.Helpers;
using SolEx.Hurt.Model.Interfaces;

namespace SolEx.Hurt.Model
{
    public class ProduktCecha : IHasLongId, IPolaIDentyfikujaceRecznieDodanyObiekt
    {
        public ProduktCecha()
        {
            
        }
        /// <summary>
        /// Tworzy nowy łącznik
        /// </summary>
        /// <param name="produkt">ID produktu</param>
        /// <param name="cechaId">Id cechy</param>
        public ProduktCecha(long produkt, long cechaId)
        {
            this.CechaId = cechaId;
            this.ProduktId = produkt;
        }
        public override string ToString()
        {
            return $"CechaId: '{CechaId}', ProduktId: '{ProduktId}'";
        }
        public ProduktCecha(ProduktCecha c)
        {
            if (c == null) return;
            CechaId = c.CechaId;
            ProduktId = c.ProduktId;
        }

        public long Id
        {
            get
            {
                try
                {
                    //return ProduktId + "-" + CechaId;
                    // kiedyś tu był klucz stringowy - ale zmieniam na LONGowy żeby sprawdzic czy bedzie lepiej - 
                    string klucz = ProduktId + "||" + CechaId;
                    return klucz.WygenerujIDObiektuSHAWersjaLong();
                }
                catch (Exception e)
                {
                    throw new Exception(string.Format("Błąd generowania ID dla ProduktCecha. Składowe: produkt id: {0}, cecha id: {1} ", ProduktId, CechaId),e);
                }
            }
        }

        public long CechaId{get;set;}

       [FriendlyName("Numer id produkt")]
      public long ProduktId {get;set;}

      public bool RecznieDodany()
      {
          return CechaId < 0 || ProduktId < 0;
      }
    }
}
