using System;
using System.Diagnostics;
using SolEx.Hurt.Helpers;
using Xunit;
namespace SolEx.Hurt.Model.Helpers.Tests
{
    public class RefleksjaTests
    {
        [Fact()]
        public void PobierzWartoscTestWydajnosciowy()
        {
            //01-08-2014 16:05 dla 100000 iteracji czas wynosi 0,005424438ms dla jednej co jest 5 razy więcej niż wg testu powinno być

            Produkt testObiekt = new Produkt(){DataDodania =  DateTime.Now, Dostawa = "testowa", Nazwa="nazwaaaa"};

            int ileIteracji = 100000;
            Stopwatch stopwatch = Stopwatch.StartNew();

            for (int i = 0; i < ileIteracji; i++)
            {
                Refleksja.PobierzWartosc(testObiekt, "dostawa");
            }
            stopwatch.Stop();
            Assert.True(stopwatch.Elapsed.TotalMilliseconds / ileIteracji < 0.005, "Czas jednej iteracji jest za dlugi! W teście wyszło: " + stopwatch.Elapsed.TotalMilliseconds / ileIteracji);

        }
    }
}
