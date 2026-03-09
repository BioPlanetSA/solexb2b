using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using SolEx.Hurt.Model;
using SolEx.Hurt.Helpers;

namespace SolEx.Hurt.Sync.App.Modules_.Provider
{
    public class FTPProvider : IImportDataModule
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        #region IImportDataModule Members
        bool isINWidnow = true;
        public void DoWork(System.Collections.Specialized.NameValueCollection configuration, SourceDB db)
        {
            if ((configuration["program_mode2"] == "-lite") || (configuration["program_mode2"] == "-prices")) return;
            if (this.ProgresChanged != null)
            {
                ProgresChanged(this, new ProgressChangedEventArgs("Aktualizacja plików na serwerze, operacja ta może zająć kilkadziesiąt minut - Czas " + DateTime.Now.ToString()));
            }
            int start = 0;
            int stop = 0;
            if (!int.TryParse(configuration["ftp_dontsend_start"], out start) || !int.TryParse(configuration["ftp_dontsend_stop"], out stop))
            {
                isINWidnow = true;
            }
            else
            {
                if (DateTime.Now.Hour >= start && DateTime.Now.Hour <= stop)
                {
                    isINWidnow = false;
                }
            }
            if (isINWidnow)
            {
                string patterns = configuration["ftp_file_name_patterns"];
                string[] pattern = new string[1];
                if (!string.IsNullOrEmpty(patterns))
                {
                    pattern = patterns.ToLower().Split(new string[] { ";;" }, StringSplitOptions.RemoveEmptyEntries);
                }
                else
                {
                    pattern[0] = ".";
                }
                string[] baseDirs2 = configuration["ftp_module_client_base_directory"].Split(new string[] { ";;" }, StringSplitOptions.RemoveEmptyEntries);
                string[,] baseDirs = new string[baseDirs2.Length, 2];
                List<string> files = new List<string>(1000);
                for (int i = 0; i < baseDirs2.Length; i++)
                {
                        string[] ss = baseDirs2[i].Split(new string[] { "^^" }, StringSplitOptions.RemoveEmptyEntries);
                        if (!Directory.Exists(ss[0])) throw new DirectoryNotFoundException("Nie znaleziono katalogu: " + ss[0]); 
                        string[] tmp = Directory.GetFiles(ss[0], "*.*", SearchOption.AllDirectories);
                        baseDirs[i, 0] = ss[0].ToLower();
                        baseDirs[i, 1] = ss[1].ToLower();
                        if (tmp != null && tmp.Length > 0)
                        {
                            files.AddRange(tmp);
                        }
                }

                for (int i = 0; i < files.Count; ++i)
                    files[i] = files[i].ToLower();
                files.RemoveAll(p => !pattern.Any(pp => p.Contains(pp)));

                List<FTPFile> remote_files = new List<FTPFile>(10000);
                MyWebRequest req = new MyWebRequest(configuration["ftp_module_url"], "GET");
                string response = req.GetResponse();

                if (response != null && response.Trim() != "")
                {
                    string[] remotefiles = response.Split(new string[] { "^^" }, StringSplitOptions.RemoveEmptyEntries);
                    for (int i = 0; i < remotefiles.Length; i++)
                    {
                        string[] dataItem = remotefiles[i].Split(new string[] { ";;" }, StringSplitOptions.RemoveEmptyEntries);
                        long size = 0;
                        if (!long.TryParse(dataItem[2], out size)) { size = 0; }
                        long tics = 0;
                        if (!long.TryParse(dataItem[1], out tics)) { tics = 0; }
                        DateTime modifyDate = DateTime.FromBinary(tics);
                        FTPFile tmp = new FTPFile(dataItem[0].ToLower(), modifyDate, size);
                        remote_files.Add(tmp);
                    }
                }

                if (this.ProgresChanged != null)
                {
                    ProgresChanged(this, new ProgressChangedEventArgs("Porównywanie plików  Czas " + DateTime.Now.ToString()));
                }

                List<string> filesToSend = new List<string>(5000);
                for (int i = 0; i < files.Count; i++)
                {
                    if (FileNeedToBeCopied(files[i], remote_files, baseDirs))
                    {
                        filesToSend.Add(files[i]);
                    }
                }
                if (this.ProgresChanged != null)
                {
                    ProgresChanged(this, new ProgressChangedEventArgs("Pliki do wysłania " + filesToSend.Count.ToString() + " Czas " + DateTime.Now.ToString()));
                }
                //files = files.Where(p => !filesToSend.Contains(p)).ToList();
                List<FTPFile> filesToDelete = new List<FTPFile>(1000);
                for (int i = 0; i < remote_files.Count; i++)
                {
                    if (remote_files[i].IsDelete) continue;
                    if (RemoteFileNeedToBeDeleted(remote_files[i], files, baseDirs, filesToSend))
                    {
                        filesToDelete.Add(remote_files[i]);
                    }
                }
                if (this.ProgresChanged != null)
                {
                    ProgresChanged(this, new ProgressChangedEventArgs("Pliki do usunięcia " + filesToDelete.Count.ToString() + " Czas " + DateTime.Now.ToString()));
                }
                int port = 0;
                if (!int.TryParse(configuration["ftp_module_serwer_port"], out port))
                {
                    port = 21;
                }
                log.Error(string.Format("Serwer {0}, port {1}, user {2} pass {3}", configuration["ftp_module_serwer"], port, configuration["ftp_module_user"], configuration["ftp_module_password"]));
                try
                {

                    using (SolEx.Hurt.Helpers.FTP ftp = new SolEx.Hurt.Helpers.FTP(configuration["ftp_module_serwer"], port, configuration["ftp_module_user"], configuration["ftp_module_password"]))
                    {
                        //ftp.Open();
                        //ftp.Login();
                        for (int i = 0; i < filesToDelete.Count; i++)
                        {
                            try
                            {
                               // ftp.RemoveFile(filesToDelete[i].FullPath);
                                ftp.DeleteFile(filesToDelete[i].FullPath);
                            }
                            catch (Exception e)
                            {
                                if (this.ProgresChanged != null)
                                {
                                    ProgresChanged(this, new ProgressChangedEventArgs("Usuwanie pliku " + filesToDelete[i].FullPath + " Błąd:" + e.Message));
                                }
                                log.Error(e.Message + " " + e.StackTrace);
                            }
                        }
                        if (this.ProgresChanged != null)
                        {
                            ProgresChanged(this, new ProgressChangedEventArgs("Koniec usuwania " + DateTime.Now.ToString()));
                        }
                        for (int i = 0; i < filesToSend.Count; i++)
                        {
                            try
                            {
                                string fileName = "";
                                string serwer_path = GetSerwerPath(filesToSend[i], baseDirs, out fileName);
                                try
                                {
                                    if (!string.IsNullOrEmpty(serwer_path) && !ftp.DirectoryExists(serwer_path))
                                    {
                                        ftp.CreateDirectory(serwer_path);
                                    }
                                }
                                catch (Exception ex) { }
                            //    ftp.PutFile(filesToSend[i], serwer_path + "\\" + fileName); /* upload c:\localfile.txt to the current ftp directory as file.txt */
                                ftp.UploadFTP(filesToSend[i], serwer_path + "\\" + fileName);
                            }
                            catch (Exception e)
                            {
                                if (this.ProgresChanged != null)
                                {
                                    ProgresChanged(this, new ProgressChangedEventArgs("Wysyłanie pliku " + filesToSend[i] + " Błąd:" + e.Message));
                                }
                                log.Error(e.Message + " " + e.StackTrace);
                            }
                        }
                        if (this.ProgresChanged != null)
                        {
                            ProgresChanged(this, new ProgressChangedEventArgs("Koniec wysłania " + DateTime.Now.ToString()));
                        }
                    }
                }
                catch (System.ComponentModel.Win32Exception ex)
                {

                     log.Error(string.Format("Błąd ftp {0} typ {1} stack {2} , errocode {3} , native {4} ",ex.Message,ex.GetType().Name,ex.StackTrace,ex.ErrorCode,ex.NativeErrorCode ));
                   // log.Error(string.Format("Błąd ftp {0} kod {1} stack {2}",ex.Message,ex.ErrorCode,ex.StackTrace ));
                    if (ex.InnerException != null)
                    {
                        log.Error(string.Format("Błąd ftp {0} stack {1}", ex.InnerException.Message, ex.InnerException.StackTrace));
                    }
                    
                    throw ex; }
                if (this.ProgresChanged != null)
                {
                    ProgresChanged(this, new ProgressChangedEventArgs(string.Format("Znalezionych plików: {0}, Wysłanych plików: {1} Plików usuniętych z serwera: {2}", files.Count, filesToSend.Count, filesToDelete.Count)));
                    ProgresChanged(this, new ProgressChangedEventArgs("Zakończono aktualizację plików na serwerze"));
                }
            }
        }
        private string GetSerwerPath(string fullPath, string[,] baseDirs, out string name)
        {
            name = Path.GetFileName(fullPath);
            string tmp = fullPath.Replace(name, "");
            //string[] baseDirs = pathToDelete.Split(new string[] { ";;" }, StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < baseDirs.GetLength(0); i++)
            {
                //string[] ss = s.ToLower().Split(new string[] { "^^" }, StringSplitOptions.RemoveEmptyEntries);
                if (fullPath.Contains(baseDirs[i,0]))
                {
                    return ( baseDirs[i, 1]+"\\"  + tmp.Replace(baseDirs[i, 0], ""));
                }
            }
            log.Error("------------------------------------------------------");
            log.Error("full path" + fullPath);
            for (int i = 0; i < baseDirs.GetLength(0); i++)
            {
                log.Error(" sciezka " + i.ToString() + " [i,0] " +baseDirs[i,0] + " [i,1] " +baseDirs[i,1]);
            }
            log.Error("------------------------------------------------------");
            return "\\domyslne";
        }
        private bool RemoteFileNeedToBeDeleted(FTPFile fTPFile, List<string> files, string[,] baseDirs, List<string> filesToSend)
        {
            string contains = null;

            for (int i = 0; i < baseDirs.GetLength(0); i++)
            {
                // string[] ss = s.Split(new string[] { "^^" }, StringSplitOptions.RemoveEmptyEntries);

                string containst = files.FirstOrDefault(p => p.Replace(baseDirs[i, 0], "") == (fTPFile.FullPath.Replace(baseDirs[i, 1], "")));
                if (containst != null)
                {
                    contains = containst;
                    break;
                }

            }
            long localsize = 0;
            if (contains != null)
            {
                FileInfo finfo = new FileInfo(contains); 
                DateTime compareLocal = finfo.LastWriteTime;
                localsize = finfo.Length;
                long remotesize = fTPFile.Size;
                DateTime compare = fTPFile.LastModyfication;
                if ((compareLocal <= compare) && (localsize == remotesize))
                {
                    return false;
                }
            }
            if (ProgresChanged != null)
            {
                ProgresChanged(this, new ProgressChangedEventArgs(String.Format("{0} {1} {2} ", (fTPFile != null ? (String.Format("{0} {1}", fTPFile.FullPath, fTPFile.Size)) : "remote null"), contains ?? "local null", localsize)));
            }
            return true;
        }

        private bool FileNeedToBeCopied(string local, List<FTPFile> remote_files, string[,] baseDirs)
        {
            FTPFile remote = null;

            for (int i = 0; i < baseDirs.GetLength(0); i++)
            {
                //  string[] ss = s.Split(new string[] { "^^" }, StringSplitOptions.RemoveEmptyEntries);
                string localPath = local.Replace(baseDirs[i, 0], "");

                //(p => p.Replace(ss[0],"").Contains(fTPFile.FullPath.Replace(ss[1] + "\\", "")));
                remote = remote_files.FirstOrDefault(p => p.FullPath.Replace(baseDirs[i, 1], "") == localPath);
                if (remote != null)
                {
                    break;
                }
            }
            FileInfo finfo = new FileInfo(local);
            DateTime compareLocal = finfo.LastWriteTime;
            long localsize = finfo.Length;

            if (remote != null)
            {
                remote.IsDelete = true;
                long remotesize = remote.Size;
                DateTime compare = remote.LastModyfication;
                if ((compareLocal <= compare) && (localsize == remotesize))
                {
                    return false;
                }
            }
            //ProgresChanged(this, new ProgressChangedEventArgs(String.Format("{0} {1} {2} ", (remote != null ? (String.Format("{0} {1}", remote.FullPath, remote.Size)) : "remote null"), local, localsize)));


            return true;
        }
        public event ProgressChangedEventHandler ProgresChanged;

        #endregion
    }
    public class FTPFile
    {
        public bool IsDelete { get; set; }
        public string Name { get; set; }
        public long Size { get; set; }
        public string Path { get; set; }
        public DateTime LastModyfication { get; set; }
        public string FullPath
        {
            get
            {
                return (Path + Name);
            }
        }
        public FTPFile(string path, string name, DateTime modyfication, long size)
        {
            Path = path;
            Name = name;
            LastModyfication = modyfication;
            Size = size;
        }
        public FTPFile(string name, DateTime modyfication, long size)
        {

            Name = System.IO.Path.GetFileName(name);
            Path = name.Replace(Name, "");
            LastModyfication = modyfication;
            Size = size;
        }
    }
}
