using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SolEx.Hurt.Core.ModelBLL;
using SolEx.Hurt.Model.AtrybutyKlas;

namespace SolEx.Hurt.Web.Site2.Models.KontrolkiTresci
{
    public class ZmianaJezyka :KontrolkaTresciBaza 
    {
        public override string Nazwa
        {
            get { return "Zmiana języka"; }
        }

        public override string Kontroler
        {
            get { return "Wyglad"; }
        }

        public override string Akcja
        {
            get { return "Jezyki"; }
        }

        [FriendlyName("Czy pokazywać języki jako lista rozwijana")]
        [WidoczneListaAdmin(true,true,true,true)]
        public bool ListaRozwijana { get; set; }

        public ZmianaJezyka()
        {
            ListaRozwijana = true;
        }
    }
}