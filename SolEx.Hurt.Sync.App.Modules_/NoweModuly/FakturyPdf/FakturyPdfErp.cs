using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Management;
using System.Runtime.InteropServices;
using System.Threading;
using SolEx.Hurt.Core.BLL;
using SolEx.Hurt.Core.ModelBLL;
using SolEx.Hurt.Core.ModelBLL.Interfejsy;
using SolEx.Hurt.Model;
using SolEx.Hurt.Model.Interfaces.Sync;
using System.IO;
using SolEx.Hurt.Core.Sync;
using SolEx.Hurt.Model.AtrybutyKlas;


namespace SolEx.Hurt.Sync.App.Modules_.NoweModuly.FakturyPdf
{
     public static class ProcessExtensions
    {
        /// <summary>
        /// Returns the Parent Process of a Process
        /// </summary>
        /// <param name="process">The Windows Process.</param>
        /// <returns>The Parent Process of the Process.</returns>
        public static Process ParentProcess(this Process process)
        {
            int parentPid = 0;
            int processPid = process.Id;
            uint TH32CS_SNAPPROCESS = 2;
 
            // Take snapshot of processes
            IntPtr hSnapshot = CreateToolhelp32Snapshot(TH32CS_SNAPPROCESS, 0);
 
            if (hSnapshot == IntPtr.Zero)
            {
                return null;
            }
 
            PROCESSENTRY32 procInfo = new PROCESSENTRY32();
 
            procInfo.dwSize = (uint)Marshal.SizeOf(typeof(PROCESSENTRY32));
 
            // Read first
            if (Process32First(hSnapshot, ref procInfo) == false)
            {
                return null;
            }
 
            // Loop through the snapshot
            do
            {
                // If it's me, then ask for my parent.
                if (processPid == procInfo.th32ProcessID)
                {
                    parentPid = (int)procInfo.th32ParentProcessID;
                }
            }
            while (parentPid == 0 && Process32Next(hSnapshot, ref procInfo)); // Read next
 
            if (parentPid > 0)
            {
                return Process.GetProcessById(parentPid);
            }
            else
            {
                return null;
            }
        }
 
        /// <summary>
        /// Takes a snapshot of the specified processes, as well as the heaps, 
        /// modules, and threads used by these processes.
        /// </summary>
        /// <param name="dwFlags">
        /// The portions of the system to be included in the snapshot.
        /// </param>
        /// <param name="th32ProcessID">
        /// The process identifier of the process to be included in the snapshot.
        /// </param>
        /// <returns>
        /// If the function succeeds, it returns an open handle to the specified snapshot.
        /// If the function fails, it returns INVALID_HANDLE_VALUE.
        /// </returns>
        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern IntPtr CreateToolhelp32Snapshot(uint dwFlags, uint th32ProcessID);
 
        /// <summary>
        /// Retrieves information about the first process encountered in a system snapshot.
        /// </summary>
        /// <param name="hSnapshot">A handle to the snapshot.</param>
        /// <param name="lppe">A pointer to a PROCESSENTRY32 structure.</param>
        /// <returns>
        /// Returns TRUE if the first entry of the process list has been copied to the buffer.
        /// Returns FALSE otherwise.
        /// </returns>
        [DllImport("kernel32.dll")]
        private static extern bool Process32First(IntPtr hSnapshot, ref PROCESSENTRY32 lppe);
 
        /// <summary>
        /// Retrieves information about the next process recorded in a system snapshot.
        /// </summary>
        /// <param name="hSnapshot">A handle to the snapshot.</param>
        /// <param name="lppe">A pointer to a PROCESSENTRY32 structure.</param>
        /// <returns>
        /// Returns TRUE if the next entry of the process list has been copied to the buffer.
        /// Returns FALSE otherwise.</returns>
        [DllImport("kernel32.dll")]
        private static extern bool Process32Next(IntPtr hSnapshot, ref PROCESSENTRY32 lppe);
 
        /// <summary>
        /// Describes an entry from a list of the processes residing 
        /// in the system address space when a snapshot was taken.
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        private struct PROCESSENTRY32
        {
            public uint dwSize;
            public uint cntUsage;
            public uint th32ProcessID;
            public IntPtr th32DefaultHeapID;
            public uint th32ModuleID;
            public uint cntThreads;
            public uint th32ParentProcessID;
            public int pcPriClassBase;
            public uint dwFlags;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
            public string szExeFile;
        }
    }

    [FriendlyName("Drukowanie faktur pdf",FriendlyOpis = "Drukowanie faktur pdf z systemu erp")]
    public class FakturyPdfErp : SyncModul, Model.Interfaces.SyncModuly.IFakturyPdf
    {
        public ILogiFormatki Logi = LogiFormatki.PobierzInstancje;

        private void Nadzorca(StatusDokumentuPDF dokument)
        {
            try
            {
                Thread.Sleep(5000*60);
                Log.Error("Drukowanie trwa zbyt długo kończę");
                int id = Process.GetCurrentProcess().ParentProcess().Id;
                Log.Error($"Nazwa procesu: {Process.GetCurrentProcess()}");
                string email = SyncManager.PobierzInstancje.Konfiguracja.EmailCustomerError;
                if (string.IsNullOrEmpty(email))
                {
                    LogiFormatki.PobierzInstancje.WyslijMailaZBledami(email,"Drukowanie pdf do dokumentu " + dokument.IdDokumentu);
                }
                
                KillProcessAndChildren(id);

            }
            catch (ThreadAbortException )
            {
                Thread.ResetAbort();
            }
            catch (Exception ex)
            {

                Log.Error("Bład wydruku", ex);
                Log.Error("Nie da się wyłączyć normalnie, wyłączam problem dzieląc przez zero");
                int x = 1;
                int y = 0;
                int a = x/y;
                Log.ErrorFormat("Wynik dzielenie x ={0} y={1} wynik={2}",x,y,a);
            }
        }

        public void Przetworz(ref List<StatusDokumentuPDF> doDrukowania, ISyncProvider aktualnyProvider)
        {
            IDrukowanieDokumentowPdf providerPdf = aktualnyProvider as IDrukowanieDokumentowPdf;
            if (providerPdf == null)
            {
                return;
                
            }
            string path = AppDomain.CurrentDomain.BaseDirectory + @"\";

            string katalog = path + "temp";
            if (!Directory.Exists(katalog))
            {
                Directory.CreateDirectory(katalog);
            }
            int liczbaBledow = 0;
            Thread workerThread = null;
            foreach (var dokument in doDrukowania)
            {
                try
                {
                    string file = string.Format("{0}\\{1}.pdf", katalog, dokument.IdDokumentu);
                    FileInfo fi = new FileInfo(file);
                    if (fi.Exists)
                    {
                        if (fi.IsLocked())
                        {
                            Logi.LogujInfo("Pomijam drukowanie pdf do dokumentu, {0}, plik istnieje i jest otwarty w jakimś programie, usuń ręcznie ten plik", dokument.IdDokumentu);
                            continue;
                        }
                        fi.Delete();
                    }
                    workerThread = new Thread(() => Nadzorca(dokument));
                    workerThread.Start();
                    if (providerPdf.DrukujPdfDokument(dokument, ref file))
                    {
                        dokument.DaneBase64 = Hurt.Helpers.PlikiBase64.FileToBase64(file);
                        dokument.Rozszerzenie = Path.GetExtension(file);
                        fi.Delete();
                        LogiFormatki.PobierzInstancje.LogujInfo("Wydrukowano plik dla dokumentu: {0}", dokument.IdDokumentu);

                    }


                }

                catch (Exception ex)
                {
                    Logi.LogujError(new Exception("Błąd podczas drukowaniu dokumentu " + dokument.IdDokumentu));
                    Logi.LogujError(ex);

                    aktualnyProvider.CleanUp();

                    ++liczbaBledow;

                    if (liczbaBledow > 5)
                    {
                        Logi.LogujInfo("Przerywamy drukowanie dokumentów z powodu zbyd dużej ilości błędów. Skontaktuj się z informatykiem.");
                        break;
                    }
                }
                finally
                {
                    if (workerThread != null)
                    {
                        workerThread.Abort();

                        workerThread.Join();
                        workerThread = null;
                    }
                }

            }
        }
        private  void KillProcessAndChildren(int pid)
        {
            ManagementObjectSearcher searcher = new ManagementObjectSearcher
              ("Select * From Win32_Process Where ParentProcessID=" + pid);
            ManagementObjectCollection moc = searcher.Get();
            foreach (ManagementObject mo in moc)
            {
                KillProcessAndChildren(Convert.ToInt32(mo["ProcessID"]));
            }
            try
            {
                Process proc = Process.GetProcessById(pid);
                proc.Kill();
            }
            catch (ArgumentException)
            {
                // Process already exited.
            }
        }
    }
}
