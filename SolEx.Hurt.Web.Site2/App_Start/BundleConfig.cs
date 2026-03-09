using System;
using System.IO;
using System.Web.Optimization;
using BundleTransformer.Core.Resolvers;
using BundleTransformer.Core.Transformers;
using SolEx.Hurt.Core.BLL;

namespace SolEx.Hurt.Web.Site2
{
    public class BundleConfig
    {

        public static string linkDoCss
        {
            get
            {
                return "~/Bundles/Css";
            }
        }

        public static string linkScript
        {
            get
            {
                return "~/Bundles/Scripts";
            }
        }



        public static string linkCssMaile
        {
            get
            {
                return "~/Bundles/CssMaile";
            }
        }

        public static string linkScriptMaile
        {
            get
            {
                return "~/Bundles/ScriptsMaile";
            }
        }

        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {        
            bundles.UseCdn = false;

            string szablonNiestandardowy = SolexBllCalosc.PobierzInstancje.Konfiguracja.SzablonNiestandardowyNazwa;
            string sciezkaDoPliku = $"/TEMPLATES/{szablonNiestandardowy}/";
            string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
            // var nullBuilder = new NullBuilder();
            var styleTransformer = new StyleTransformer();
            var scriptTransformer = new ScriptTransformer();

            // var nullOrderer = new NullOrderer();

            // Replace a default bundle resolver in order to the debugging HTTP-handler
            // can use transformations of the corresponding bundle
            BundleResolver.Current = new CustomBundleResolver();

            //zwykle style            
            var commonStylesBundle = new StyleBundle( linkDoCss );

            //jako pierwszy msui być @import url(https://fonts.googleapis.com/css?family=Open+Sans+Condensed:400|Open+Sans:400,700|Roboto+Condensed:400,700|Roboto:400,700|Caveat&subset=latin,latin-ext);
            commonStylesBundle.Include("~/Layout/css/_fonty.css");

            commonStylesBundle.IncludeDirectory("~/Content/", "*.css");
            commonStylesBundle.IncludeDirectory("~/Layout/css/", "*.css");
            //commonStylesBundle.IncludeDirectory("~/Layout/css/pieczatki/", "*.css");
            commonStylesBundle.IncludeDirectory("~/Content/tether/", "*.css");

            commonStylesBundle.Include("~/Layout/css/hoverEffects/hover.scss");

            commonStylesBundle.Include("~/Layout/css/font-awesome.min.css");
            commonStylesBundle.Include("~/Layout/jquery/token-input.css");
            commonStylesBundle.Include("~/Layout/jquery/token-input-facebook.css");

            commonStylesBundle.Include("~/Content/themes/default/style.css");

            commonStylesBundle.Include("~/Scripts/slider/jquery.bxslider.css");

            commonStylesBundle.Include("~/Content/bootstrap/bootstrap-flex.scss");

            commonStylesBundle.Include("~/Layout/css/main/main.scss");
            commonStylesBundle.Include("~/Layout/css/main/customCSS.scss");

            commonStylesBundle.Include("~/Layout/css/perfect-scroolbar/main.scss");

            /*Dodane z powodu, ze przy publishu automatycznie pliki scss kompilowane są do css i w publishu nie ma ich, nawet gdy ustawiy aby były kopiowane*/
            //commonStylesBundle.Include("~/Content/bootstrap/bootstrap-flex.css");

            //commonStylesBundle.Include("~/Layout/css/customCSS.css");
            //commonStylesBundle.Include("~/Layout/css/main/main.css");

            if (!string.IsNullOrEmpty(szablonNiestandardowy))
            {
                //czy plik istnieje
                string sciezka = sciezkaDoPliku + "custom.scss";
                
                if (!File.Exists(baseDirectory + sciezka))
                {
                    Directory.CreateDirectory(baseDirectory + sciezkaDoPliku);
                    File.Create(baseDirectory + sciezka);
                }
                else
                {
                    //czy jest jakas zawartosc
                    if (new FileInfo(baseDirectory + sciezka).Length != 0)
                    {
                        commonStylesBundle.Include($"~{sciezka}");
                    }
                }
            }

            commonStylesBundle.Transforms.Add(styleTransformer);
            bundles.Add(commonStylesBundle);

            var scriptsBundle = new Bundle(linkScript);

            scriptsBundle.Include("~/Scripts/fullPageSlider/jquery.fullpage.min.js");
            scriptsBundle.Include("~/Scripts/fullPageSlider/vendors/*.min.js");
            scriptsBundle.Include("~/Scripts/tether/tether.js");

            //na koncu ladujemy pozostale skrypty z worka
            scriptsBundle.IncludeDirectory("~/Scripts/", "*.js");
            scriptsBundle.IncludeDirectory("~/Scripts/ug-gallery/", "*.js");

            scriptsBundle.IncludeDirectory("~/Scripts/Solex", "*.js");
        //    scriptsBundle.IncludeDirectory("~/Scripts/admin/", "*.js");
            //    scriptsBundle.IncludeDirectory("~/Scripts/polyfill/", "*.js");

            scriptsBundle.IncludeDirectory("~/Scripts/JSKompilowaneZTS/","*.js");


            scriptsBundle.Transforms.Add(scriptTransformer);
         //   scriptsBundle.Transforms.Add(typoTransformer);
            
            bundles.Add(scriptsBundle);

            //  var typoScriptsBundle = new Bundle(linkTypoScript);


            // -------- Bundle dla podglądu maila -------------
            var styleMaileBundle = new StyleBundle(linkCssMaile);
            styleMaileBundle.Include("~/Content/bootstrap/bootstrap-flex.css");
            styleMaileBundle.Include("~/Layout/css/maile/ink.css");
            styleMaileBundle.Include("~/Layout/css/maile/maile.css");


            if (string.IsNullOrEmpty(szablonNiestandardowy))
            {
                styleMaileBundle.Include($"~{sciezkaDoPliku}maile.css");
            }
            bundles.Add(styleMaileBundle);

            var scriptMaileBundle = new StyleBundle(linkScriptMaile);
            scriptMaileBundle.Include("~/Scripts/tether/tether.js");
            scriptMaileBundle.Include("~/Scripts/bootstrap.js");
            scriptMaileBundle.Include("~/Scripts/Solex/Helpers.js");
            scriptMaileBundle.Include("~/Scripts/jquery-2.2.4.js");
            scriptMaileBundle.Include("~/Scripts/Typo/_helpers.ts");
            scriptMaileBundle.Include("~/Scripts/Typo/_ajax.ts");

            scriptMaileBundle.Transforms.Add(scriptTransformer);

            bundles.Add(scriptMaileBundle);
        }
    }
}
