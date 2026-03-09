using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SolEx.Hurt.Model
{
    /// <summary>
    /// Błąd wyrzucany podczas importu zamówień
    /// </summary>
    public class SaveOrderException:Exception
    {
        private ZamowienieSynchronizacja _order = null;
        /// <summary>
        /// Zamówienie podczas importu którego nastąpił błąd
        /// </summary>
        public ZamowienieSynchronizacja Order
        {
            get
            {
                return _order;
            }
        }
        /// <summary>
        /// Konstruktor wyjątku
        /// </summary>
        /// <param name="message">Widomość</param>
        public SaveOrderException(string message) : this(message, null) { }
        /// <summary>
        /// Konstruktor wyjątku
        /// </summary>
        /// <param name="message">Widomość</param>
        /// <param name="order">Problematyczne zamówienie</param>
        public SaveOrderException(string message, ZamowienieSynchronizacja order) : this(message, order, null) { }
        /// <summary>
        /// Konstruktor wyjątku
        /// </summary>
        /// <param name="message">Widomość</param>
        /// <param name="order">Problematyczne zamówienie</param>
        /// <param name="inner">Wyjątek wewnętrzny</param>
        public SaveOrderException(string message, ZamowienieSynchronizacja order, Exception inner) : base(message, inner) { _order = order; }
    }
}
