using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ImageProcessor;
using ImageProcessor.Imaging;
using ImageProcessor.Imaging.Formats;
using SolEx.Hurt.Model;
using SolEx.Hurt.Model.Enums;

namespace SolEx.Hurt.Helpers
{
    public static class PlikiBase64
    {
        /// <summary>
        /// Convert given file to Base64 string.
        /// </summary>
        /// <param name="filename">Full path of the file to be converted. Ex: "C:\My Files\Kiba.jpg"</param>
        /// <returns>string</returns>
        public static string FileToBase64(string filename)
        {
            return Convert.ToBase64String(FileToByteArray(filename));
        }

        /// <summary>
        /// Convert given file to byte array.
        /// </summary>
        /// <param name="filename">Full path of the file to be converted. Ex: "C:\My Files\Kiba.jpg"</param>
        /// <returns>byte[]</returns>
        public static byte[] FileToByteArray(string filename)
        {
            byte[] returnByteArray = null;
            // Instantiate FileStream to read file
            System.IO.FileStream readFileStream = new System.IO.FileStream(filename, System.IO.FileMode.Open, System.IO.FileAccess.Read);

            // Instantiate BinaryReader and attach FileStream with the object
            System.IO.BinaryReader readBinaryReader = new System.IO.BinaryReader(readFileStream);

            // Get file's byte length
            long fileByteSize = new System.IO.FileInfo(filename).Length;

            // Read bytes from the file
            returnByteArray = readBinaryReader.ReadBytes((Int32)fileByteSize);

            // Clean up / disposal
            readFileStream.Close();
            readFileStream.Dispose();
            readBinaryReader.Close();
            return returnByteArray;
        }

        /// <summary>
        /// Convert given Base64 string to file based on given filename.
        /// </summary>
        /// <param name="base64String">String of Base64</param>
        /// <param name="filename">Full path of the file to be converted. Ex: "C:\My Files\Kiba.jpg"</param>
        public static void Base64ToFile(string base64String, string filename)
        {
            byte[] fileByteArray = Convert.FromBase64String(base64String);
            // Instantiate FileStream to create a new file
            System.IO.Directory.CreateDirectory( System.IO.Path.GetDirectoryName(filename) );
            System.IO.FileStream writeFileStream = new System.IO.FileStream(filename, System.IO.FileMode.Create, System.IO.FileAccess.Write);
            // Write converted base64String to newly created file
            writeFileStream.Write(fileByteArray, 0, fileByteArray.Length);
            // Clean up / disposal
            writeFileStream.Close();
            fileByteArray = null;
            writeFileStream.Dispose();
            writeFileStream = null;
        }


        /// <summary>
        /// Metoda mająca ne celu zmiane rozdzielczości zdjęcia
        /// </summary>
        /// <param name="p"></param>
        /// /// <param name="rozmiarRzeczywisty"></param>
        public static string ResizePhoto(Plik p, out double rozmiarRzeczywisty)
        {
            byte[] byteArray = FileToByteArray(Path.Combine(p.Sciezka, p.nazwaLokalna));

            //sprawdzamy czy plik jest mniejszy niż 100kb oraz czy jest on napewno zdjeciem
            if (p.Rozmiar < 100 * 1024 || (p.RodzajPliku != RodzajPliku.Zdjecie && p.RodzajPliku!=RodzajPliku.PlikDoPrzeniesieniaJedenNaJedenOdKlienta))
            {
                rozmiarRzeczywisty = byteArray.Length;
                return Convert.ToBase64String(byteArray);
            }

           //ustawiamy wymiary
            Size size = new Size(1280, 1024);
            
            byte[] wynik = null;

            //odczytujemy dane i zmieniamy wymiary
            using (var inStream = new MemoryStream(byteArray))
            {
                using (var outStream = new MemoryStream())
                {
                    using (var imageFactory = new ImageFactory(false))
                    {
                        imageFactory.Load(inStream);
                        var imageFormat = imageFactory.CurrentImageFormat;
                        if (Equals(imageFormat.ImageFormat, new BitmapFormat().ImageFormat))
                        {
                            imageFormat = new JpegFormat();
                            p.Nazwa = $"{p.NazwaBezRozszerzenia}.{imageFormat.ImageFormat}";
                        }
                        if (Equals(imageFormat.ImageFormat, new TiffFormat().ImageFormat))
                        {
                            imageFormat = new PngFormat();
                            p.Nazwa = $"{p.NazwaBezRozszerzenia}.{imageFormat.ImageFormat}";
                        }
                        if (imageFactory.Image.Height > imageFactory.Image.Width)
                        {
                            size = new Size(1024, 1280);
                        }

                        ResizeLayer resizeLayer = new ResizeLayer(size, ResizeMode.Max, AnchorPosition.Center, false);

                        imageFactory.Resize(resizeLayer)
                            .Format(imageFormat)
                            .Quality(80)
                            .Save(outStream);
                    }
                    wynik = outStream.ToArray();
                }
            }
            //Jeżeli otrzymany wynik jest większy od orginalnego rozmiary wysyłamy orginalny
            if (wynik.Length > byteArray.Length)
            {
                wynik = byteArray;
            }
            rozmiarRzeczywisty = wynik.Length;
            return Convert.ToBase64String(wynik);
        }
    }
}
