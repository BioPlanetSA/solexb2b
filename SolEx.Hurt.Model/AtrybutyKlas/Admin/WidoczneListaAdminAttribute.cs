using System;
using System.Collections.Generic;
using System.Linq;

namespace SolEx.Hurt.Model.AtrybutyKlas
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Method,AllowMultiple = false)]
    public class WidoczneListaAdminAttribute:Attribute
    {

        private readonly bool _domyslnieWidoczne;
        private readonly bool _dostepneLista;
        private readonly bool _edytowalne;
        private readonly IList<Type> _widocznyGdzie;
        private readonly bool _edycjaInline;
        private readonly bool _edycjaWidoczny;

        /// <summary>
        /// Tworzy nowy atrybut opisujący pole w adminie
        /// </summary>
        /// <param name="dostepneLista">Czy pole może być pokazywane na listach w adminie</param>
        /// <param name="domyslnieWidoczneLista">Czy pole jeste domyślnie widoczne na listach w adminie</param>
        /// <param name="edytowalne">Czy pole jest edytowalne w adminie</param>
        /// <param name="edytowalneInline">Czy pole jest edytowalne na liście w  adminie</param>
        /// <param name="edycjaWidoczny">Czy pole ma być widoczne na edycji - domyślnie FALSE, chyba że jakikolwiek poprzedni parametr TRUE</param>
        public WidoczneListaAdminAttribute(bool dostepneLista, bool domyslnieWidoczneLista, bool edytowalne,bool edytowalneInline,bool edycjaWidoczny=true)
        {
            if (!dostepneLista && domyslnieWidoczneLista)
            {
                throw new ArgumentException("Pole ustawione jako niedostepne na liscie, ale domslnie widoczne na liście");
            }
            _domyslnieWidoczne = domyslnieWidoczneLista;
            _dostepneLista = dostepneLista;
            _edytowalne = edytowalne;
            _widocznyGdzie = new List<Type>();
            _edycjaInline = edytowalneInline;
            _edycjaWidoczny = edycjaWidoczny;
        }

        public WidoczneListaAdminAttribute():this(true,true,true,true,true)
        {
        }

        public WidoczneListaAdminAttribute(bool dostepneLista, bool domyslnieWidoczneLista, bool edytowalne, bool edytowalneInline,bool edycjaWidoczny, Type[] widocznosc)
            : this(dostepneLista, domyslnieWidoczneLista, edytowalne, edytowalneInline, edycjaWidoczny)
        {
       
            _widocznyGdzie = widocznosc;

        }

        public WidoczneListaAdminAttribute(bool dostepneLista, bool domyslnieWidoczneLista, bool edytowalne, bool edytowalneInline,bool edycjaWidoczny, string[] widocznoscTypy)
            : this(dostepneLista, domyslnieWidoczneLista, edytowalne, edytowalneInline,edycjaWidoczny, widocznoscTypy.Select(x=>Type.GetType(x,true)).ToArray())
        {
            
        }
        /// <summary>
        /// Czy pole jest domyślnie widoczne na liście w adminie
        /// </summary>
        public bool DomyslnieWidoczne
        {
            get { return _domyslnieWidoczne; }
        }
        /// <summary>
        /// Czy pole jest może być pokazane na listach w adminie
        /// </summary>
        public bool DostepneLista
        {
            get { return _dostepneLista; }
        }
        /// <summary>
        /// Czy pole można edytować w adminie
        /// </summary>
        public bool Edytowalne
        {
            get { return _edytowalne; }
        }

        /// <summary>
        /// W jakich lista widoczny
        /// </summary>
        public IList<Type> WidocznyGdzie
        {
            get { return _widocznyGdzie; }
        }

        public bool EdytowalneInline { get { return _edycjaInline; }}

        public bool EdycjaWidoczny
        {
            get { return _edycjaWidoczny; }
        }

        public string StyleCss { get; set; }
    }
}
