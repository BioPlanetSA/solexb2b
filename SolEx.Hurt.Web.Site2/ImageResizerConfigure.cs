using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Web;
using ImageResizer.Configuration;
using ImageResizer.Plugins.Watermark;
using Config = ImageResizer.Configuration.Config;

namespace SolEx.Hurt.Web.Site2
{
    public static class ImageResizerConfigure
    {
        public static void UstawWatermark()
        {
            if ( !string.IsNullOrEmpty(SolEx.Hurt.Core.BLL.SolexBllCalosc.PobierzInstancje.Konfiguracja.ZnakWodnyTekst) || !string.IsNullOrEmpty(SolEx.Hurt.Core.BLL.SolexBllCalosc.PobierzInstancje.Konfiguracja.ZnakWodnyObrazek))
            {
                Config c = Config.Current;
                WatermarkPlugin wp = c.Plugins.Get<WatermarkPlugin>();
                if (wp == null)
                {
                    //Install it if it's missing
                    wp = new WatermarkPlugin();
                    wp.Install(c);
                }

                List<Layer> warstwy = new List<Layer>();

                if (!string.IsNullOrEmpty(SolEx.Hurt.Core.BLL.SolexBllCalosc.PobierzInstancje.Konfiguracja.ZnakWodnyObrazek))
                {
                    ImageLayer i = new ImageLayer(c); //ImageLayer needs a Config instance so it knows where to locate images
                    i.Path = SolEx.Hurt.Core.BLL.SolexBllCalosc.PobierzInstancje.Konfiguracja.ZnakWodnyObrazek; //obrazek
                    i.Align = ContentAlignment.MiddleCenter;
                    i.Fill = true;
                    warstwy.Add(i);
                }

                if (!string.IsNullOrEmpty(SolEx.Hurt.Core.BLL.SolexBllCalosc.PobierzInstancje.Konfiguracja.ZnakWodnyTekst))
                {
                    TextLayer t = new TextLayer();
                    t.Text = SolEx.Hurt.Core.BLL.SolexBllCalosc.PobierzInstancje.Konfiguracja.ZnakWodnyTekst; //tekst
                    t.Align = ContentAlignment.BottomLeft;
                    t.GlowColor = Color.GhostWhite;
                    t.FontSize = 18;
                    t.TextColor = Color.LightSkyBlue;
                    t.Fill = true; //Fill the image with the text
                    t.OutlineWidth = 1;
                    warstwy.Add(t);
                }

                wp.NamedWatermarks.Add("wm", warstwy);

                //handler watermarkow - dodanei watermark do kazdego requestu

                Config.Current.Pipeline.PostRewrite += delegate(IHttpModule sender, HttpContext context, IUrlEventArgs ev)
                {
                    string folder = VirtualPathUtility.ToAbsolute("~/Zasoby/import");
                    if (ev.VirtualPath.StartsWith(folder, StringComparison.OrdinalIgnoreCase))
                    {
                        if (!ev.QueryString.AllKeys.Any(x => x == "w"))    //watermark tylko jak nie ma qs w
                        {
                            ev.QueryString["watermark"] = "wm";
                        }
                    }
                };
            }
        }

        public static void UstawPresety()
        {
            
        }
    }
}
