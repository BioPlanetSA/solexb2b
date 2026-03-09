namespace SolEx.Hurt.Sync.App.Modules_.Provider
{
    using SolEx.Hurt.Sync.App.Modules_;
    using SolEx.Hurt.Model;
    using System;
    using System.Collections.Specialized;
    using System.Threading;

    public class OpiekunowieNaPodstawieGrupyProvider : IImportDataModule
    {
        public event ProgressChangedEventHandler ProgresChanged;

        public void DoWork(NameValueCollection configuration, SourceDB db)
        {
            if (this.ProgresChanged != null)
            {
                this.ProgresChanged(this, new ProgressChangedEventArgs("Opiekunowie na podstawie grupy"));
            }
            //for (int i = 0; i < db.Customers.Count; i++)
            //{
            //    db.Customers[i].OpiekunEmail = string.IsNullOrEmpty(db.Customers[i].Category) ? null : db.Customers[i].Category;
            //}
            //if (this.ProgresChanged != null)
            //{
            //    this.ProgresChanged(this, new ProgressChangedEventArgs("Zakończenie opiekunowie na podstawie grupy"));
            //}
        }
    }
}

