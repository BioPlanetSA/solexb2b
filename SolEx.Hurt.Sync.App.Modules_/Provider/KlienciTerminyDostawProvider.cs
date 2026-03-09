namespace SolEx.Hurt.Sync.App.Modules_.Provider
{
    using SolEx.Hurt.Sync.App.Modules_;
    using SolEx.Hurt.Model;
    using System;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.Linq;
    using System.Threading;

    public class KlienciTerminyDostawProvider : IImportDataModule
    {
        public event ProgressChangedEventHandler ProgresChanged;
        public void DoWork(NameValueCollection configuration, SourceDB db)
        {
            List<string> days = new List<string> { "PONIEDZIAŁEK", "WTOREK", "ŚRODA", "CZWARTEK", "PIĄTEK", "SOBOTA", "NIEDZIELA" };
            string format = "Twoje dni dostaw to: {0}.";
            if (this.ProgresChanged != null)
            {
                this.ProgresChanged(this, new ProgressChangedEventArgs("Pobieranie termiów dostaw z cech"));
            }
            for (int i = 0; i < db.Customers.Count; i++)
            {
               
        
            }        
        }
    }
}

