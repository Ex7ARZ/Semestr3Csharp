using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace lab2lib
{
    public class firewatch
    {
        
           public static object obj=new object(); 

            private ArrayList Countryballs;

            public readonly string Root;

            public firewatch(string root)
            {

                Countryballs = new ArrayList();
                Root = root;
            }

            public string  Decryptf(string tpath)
            {
                var i = 0;
                string text;
                lock (obj)
                {

                    text = File.ReadAllText(tpath);
                }
                byte[] buf = Encoding.Unicode.GetBytes(text);
                for (i = 0; i < buf.Length; i++)
                {
                    buf[i] = (byte)(buf[i] ^ 1);
                }
                text = Encoding.Unicode.GetString(buf);
             tpath = tpath.Remove(tpath.Length - 4) + ".crypt";
             File.WriteAllText(tpath, text);
             return tpath;
            }

            public string ArchivizeCrypt(string tpath)
            {
                string stpath = tpath.Remove(tpath.Length - 6) + ".gz";
                using (FileStream sourceStream = new FileStream(tpath, FileMode.Open))
                {
                    using (FileStream targetStream = File.Create(stpath))
                    {
                        using (GZipStream compressionStream = new GZipStream(targetStream, CompressionMode.Compress))
                        {
                            sourceStream.CopyTo(compressionStream);
                        }
                    }
                }
                return stpath;
            }

            public string Copystr(string stpath)
            {
                char w = 'S';
                var i = stpath.Length - 1;
                while (w != stpath[i]) i--;
                string name = stpath.Substring(i);
                string newpath = Path.Combine(@"D:\\csharp\Target", name);
                FileInfo fileInf = new FileInfo(stpath);
                if (fileInf.Exists)
                {
                    fileInf.CopyTo(newpath, true);
                }
                return newpath;
            }

            public string GetFileName(string tpath)
            {
                var i = 0;
                string Name;
                i = tpath.Length - 1;
                while (tpath[i] != 'S') i--;
                Name = tpath.Substring(i);
                return Name;
            }

            public string DearchivizeCrypt(string newpath)
            {
                string newtargetpath = newpath.Remove(newpath.Length - 3) + ".crypt";
                using (FileStream sourceStream = new FileStream(newpath, FileMode.OpenOrCreate))
                {
                    using (FileStream targetStream = File.Create(newtargetpath))
                    {
                        using (GZipStream decompressionStream =
                            new GZipStream(sourceStream, CompressionMode.Decompress))
                        {
                            decompressionStream.CopyTo(targetStream);
                        }
                    }
                    File.Delete(newpath);
                   
                }

                return newtargetpath;
            }

            public string Encryptf(string newtargetpath)
            {
                string text = File.ReadAllText(newtargetpath);
                
               byte[] buf = Encoding.Unicode.GetBytes(text);
                for (var i = 0; i < buf.Length; i++)
                {
                    buf[i] = (byte)(buf[i] ^ 1);
                }
                text = Encoding.Unicode.GetString(buf);
                string delfile = newtargetpath;
                newtargetpath = newtargetpath.Remove(newtargetpath.Length - 6) + ".txt";
                File.WriteAllText(newtargetpath, text);
                File.Delete(delfile);

                return newtargetpath;
            }

            public string SaveToArchive(string newtargetpath)
            {
                string Name = GetFileName(newtargetpath);
                File.Copy(newtargetpath, Path.Combine(@"D:\\csharp\Target\Archive", Name));

               string  stpath = Path.Combine(@"D:\\csharp\Target\Archive", Name).Remove(Path.Combine(@"D:\\csharp\Target\Archive", Name).Length - 4) + ".gz";
                using (FileStream sourceStream = new FileStream(Path.Combine(@"D:\\csharp\Target\Archive", Name), FileMode.Open))
                {
                    using (FileStream targetStream = File.Create(stpath))
                    {
                        using (GZipStream compressionStream = new GZipStream(targetStream, CompressionMode.Compress))
                        {
                            sourceStream.CopyTo(compressionStream);
                            Console.WriteLine("Сжатие файла завершено.");
                        }
                    }
                }
                File.Delete(Path.Combine(@"D:\\csharp\Target\Archive", Name));
                return stpath;
            }

    }

    public  class Logger
    {
       
        FileSystemWatcher watcher;
        object obj = new object();
        bool enabled = true;
        public Logger()
        {
            string path = @"D:\\csharp\Source";
            string str = DateTime.Now.ToString();

            string day = str.Substring(0, 2);
            string month = str.Substring(3, 2);
            string year = str.Substring(6, 4);
            string subdir = Path.Combine(path, year, month, day);
            watcher = new FileSystemWatcher(subdir);
            watcher.Created += Watcher_Created;
            watcher.Filter = "*.txt";
        }

        public void Start()
        {
            string str = DateTime.Now.ToString();
            
            string day = str.Substring(0, 2);
            string month = str.Substring(3, 2);
            string year = str.Substring(6, 4);
            
            string path = Path.Combine(year, month, day);
            string subdir = path;
            DirectoryInfo dirInfo = new DirectoryInfo(@"D:\\csharp\Source");
            if (!dirInfo.Exists)
            {
                dirInfo.Create();
            }
            dirInfo.CreateSubdirectory(subdir ?? throw new Exception("null"));
            watcher.EnableRaisingEvents = true;
            while (enabled)
            {
                Thread.Sleep(1000);
            }
        }
        public void Stop()
        {
            watcher.EnableRaisingEvents = false;
            enabled = false;
        }
        
        private void Watcher_Created(object sender, FileSystemEventArgs e)
        {
            
            string fileEvent = "New file:" + e.Name + ",Task:" + e.ChangeType + ",path:" + e.FullPath + "\n";
            string filePath = e.FullPath;
            RecordEntry(fileEvent, filePath);
        }
        

        private void RecordEntry(string fileEvent, string filePath)
        { 
            lock (obj)
            {
                using (StreamWriter writer = new StreamWriter("D:\\templog.txt", true))
                {
                    writer.WriteLine( fileEvent);
                    writer.Flush();
                }
            }
            firewatch firewatcher=new firewatch(filePath);
            string string1 = firewatcher.Decryptf(filePath);
            string1 = firewatcher.ArchivizeCrypt(string1);
            string1 = firewatcher.Copystr(string1);
            string1 = firewatcher.DearchivizeCrypt(string1);
            string1 = firewatcher.Encryptf(string1);
            string1 = firewatcher.SaveToArchive(string1);
        }
    }
}
