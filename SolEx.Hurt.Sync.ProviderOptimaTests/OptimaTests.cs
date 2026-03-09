using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using FakeItEasy;
using SolEx.Hurt.Core.BLL.Interfejsy;
using SolEx.Hurt.Model.Enums;
using SolEx.Hurt.Model.Interfaces;
using SolEx.Hurt.Sync.ProviderOptima;
using Xunit;

namespace SolEx.Hurt.Sync.ProviderOptimaTests
{
    public class OptimaTests
    {
        static string folderPath = @"C:\Program Files (x86)\Comarch ERP Optima\";

        private Optima _optima = null;
        public Optima PobierzInstancjeTestowaOptima
        {
            get
            {
                try
                {
                    if (_optima == null)
                    {
                        //katalog odpowiedni

                        AppDomain currentDomain = AppDomain.CurrentDomain;
                        currentDomain.AssemblyResolve += new ResolveEventHandler(LoadFromSameFolder);


                        _optima = Optima.PobierzInstancje;
                        IConfigBLL config = A.Fake<IConfigBLL>();
                        _optima.ConfigBll = config;

                        A.CallTo(() => _optima.ConfigBll.KatalogProgramuKsiegowego).Returns(folderPath);
                        A.CallTo(() => _optima.ConfigBll.JakieModulyOptima).Returns(new HashSet<ModulyOptima>());

                        A.CallTo(() => _optima.ConfigBll.ERPLogin).Returns("Admin");
                        A.CallTo(() => _optima.ConfigBll.ERPHaslo).Returns("");
                        A.CallTo(() => _optima.ConfigBll.OptimaNazwaFirmy).Returns("test-optima");

                        //czy dziala
                        if (_optima.Sesja == null)
                        {
                            throw new Exception("Nie działa");
                        }
                    }
                    return _optima;
                }
                catch (Exception e)
                {
                    throw e;
                }
            }
        }


        static Assembly LoadFromSameFolder(object sender, ResolveEventArgs args)
        {
            string assemblyPath = Path.Combine(folderPath, new AssemblyName(args.Name).Name + ".dll");
            if (File.Exists(assemblyPath) == false)
            {
                //dla genrapa
                assemblyPath = Path.Combine(folderPath + @"\GenRap", new AssemblyName(args.Name).Name + ".dll");
                if (File.Exists(assemblyPath) == false)
                {
                  //  throw new Exception("Nie mozna zaladowac plikow dla optimy");
                    return null;
                }
            }
            Assembly assembly = Assembly.LoadFrom(assemblyPath);
            return assembly;
        }


        [Fact()]
        public void DrukujTest()
        {
            //crystal standardowy
            A.CallTo(() => PobierzInstancjeTestowaOptima.ConfigBll.OptimaIdSzablonuWydrukuDoPdf).Returns(1834);
            PobierzInstancjeTestowaOptima.Drukuj(1, "c:/test-crystal.pdf");

            //genrap standardowy
            //A.CallTo(() => PobierzInstancjeTestowaOptima.ConfigBll.OptimaIdSzablonuWydrukuDoPdf).Returns(372);
            //PobierzInstancjeTestowaOptima.Drukuj(1, "c:/test-genrap.pdf");

            Assert.True(true, "Sprawdz czy plik się wydrukował poprawnie - sciezka : c:/test-genrap i crystal.pdf");
        }
    }
}
