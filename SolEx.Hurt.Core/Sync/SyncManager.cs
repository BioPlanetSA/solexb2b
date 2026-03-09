using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using log4net;
using SolEx.Hurt.Core.BLL;
using SolEx.Hurt.Core.BLL.Interfejsy;
using SolEx.Hurt.Helpers;
using SolEx.Hurt.Model;
using SolEx.Hurt.Model.Enums;
using SolEx.Hurt.Model.Interfaces;
using SolEx.Hurt.Model.Interfaces.Sync;

namespace SolEx.Hurt.Core.Sync
{
    public interface ISyncManager
    {
        IConfigBLL Konfiguracja { get; set; }
        ILog Log { get; }
        ISyncProvider GetProvider(ERPProviderzy provider, IConfigSynchro config);
    }

    public class SyncManager: BllBaza<SyncManager>, ISyncManager
    {
        public SyncManager()
        {
            Konfiguracja = new ConfigBLL(null, new ApiConfigProvider());
        }

        private KlientKategoriaKlienta[] _lacznikiKategoriiKlientow = null;

        public KlientKategoriaKlienta[] PobierzLacznikiKategoriiKlientow()
        {
            if (_lacznikiKategoriiKlientow != null)
            {
                return _lacznikiKategoriiKlientow;
            }

            _lacznikiKategoriiKlientow =   APIWywolania.PobierzInstancje.PobierzKlienciKategorie(new Dictionary<string, object>()).Values.ToArray();
            if (_lacznikiKategoriiKlientow == null || !_lacznikiKategoriiKlientow.Any())
            {
                throw new Exception("Nie ma pobranych ³¹czników.");
            }
            return _lacznikiKategoriiKlientow;
        }

        public virtual IConfigBLL Konfiguracja { get; set; } 

        private readonly log4net.ILog log = log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public ISyncProvider GetProvider(ERPProviderzy provider, IConfigSynchro config)
        {
            if (provider == ERPProviderzy.Brak)
            {
                return null;
            }

            //tu musi byc tray catch - jesli nie mozna zaladowac providera to w ogach musi byc info
            try
            {
                string path = AppDomain.CurrentDomain.BaseDirectory + "SolEx.Hurt.Sync.Provider" + provider + ".dll";
                log.Info("dll path: " + path);

                if (File.Exists(path))
                {
                    Assembly ass = Assembly.LoadFile(path);
                    log.Info("dll load: " + path);
                    if (ass == null)
                        return null;

                    foreach (Type t in ass.GetTypes())
                    {
                        if (t.GetInterface("ISyncProvider") != null)
                        {
                            ISyncProvider prov= (ISyncProvider)Activator.CreateInstance(t);
                            prov.UstawParametryLaczenia(config);
                            return prov;
                        }
                    }

                }
            }
            catch (ReflectionTypeLoadException ex)
            {
                log.Error("b³¹d przy ³adowaniu providera", ex);
                if (ex.LoaderExceptions != null)
                    foreach (var e in ex.LoaderExceptions)
                        log.Error("b³¹d", e);
            }
            catch (Exception ex) 
            {
                log.Error("b³¹d przy ³adowaniu providera", ex);
            }
            return null;
        }

    }
}
