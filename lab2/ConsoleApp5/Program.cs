using System;
using System.IO;
using System.IO.Compression;
using System.Text;
using System.Threading;

namespace ConsoleApp5
{
    class Program
    {
        private static bool Iswatched = false;
        private static string tpath;
        public static object obj=new object();
       
        public static file1 qq = new file1();

        static void Main(string[] args)
        {

            do
            {
               
                Run();
                // qq.Write();
                Console.WriteLine("Input space to exit");
            } 
            while (Console.Read() != ' ');
        }

        public static void Run()
        {
            string path = @"D:\\csharp\Source";
            string str = DateTime.Now.ToString();
           
            string day = str.Substring(0, 2);
            string month = str.Substring(3, 2);
            string year = str.Substring(6, 4);
            string subdir = Path.Combine(path,year, month, day);

            using (FileSystemWatcher watcher = new FileSystemWatcher() )
            {
                watcher.Path = @"D:\\csharp\Source"; //subdir;
                watcher.NotifyFilter = NotifyFilters.LastAccess | NotifyFilters.FileName | NotifyFilters.DirectoryName;
                watcher.Filter = "*.txt";
                watcher.Created += /*new FileSystemEventHandler*/(OnChanged);
                watcher.EnableRaisingEvents = true;
               Console.WriteLine("create file");
                qq.Write();
                
            }

            if (Iswatched)
            {
                Dpsunpen();
            }
       

        }

        private static void OnChanged(object sender,FileSystemEventArgs e)
        {
            Console.WriteLine("New file:"+e.Name+",Task:"+e.ChangeType+",path:"+e.FullPath+"\n");
            Iswatched = true;
            tpath = e.FullPath;
            Console.WriteLine(e.FullPath);
           
        }

        public static void Dpsunpen()
        {
            var i = 0;
            string Name;
            i = tpath.Length - 1;
            while (tpath[i] != 'S') i--;
            Name = tpath.Substring(i);
            string text;
            lock (obj)
            {
                
                text = File.ReadAllText(tpath);
            }
            byte[] buf = Encoding.Unicode.GetBytes(text);
            for ( i = 0; i < buf.Length; i++)
            {
                buf[i] = (byte)(buf[i] ^ 1);
            }
            text = Encoding.Unicode.GetString(buf);
        
            tpath = tpath.Remove(tpath.Length - 4) + ".crypt";
            File.WriteAllText(tpath, text);
           
            string stpath = tpath.Remove(tpath.Length - 6) + ".gz";
            using (FileStream sourceStream = new FileStream(tpath, FileMode.Open))
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


            char w = 'S';
            i = stpath.Length - 1;
            while (w != stpath[i]) i--;
            string name = stpath.Substring(i);
            string newpath = Path.Combine(@"D:\\csharp\Target", name).Remove(Path.Combine(@"D:\\csharp\Target", name).Length - 3) + ".gz";
            FileInfo fileInf = new FileInfo(stpath);
            if (fileInf.Exists)
            {
                fileInf.CopyTo(newpath, true);
            }
            
            
            Thread.Sleep(60);

            
            
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
                Console.WriteLine("File is dearchivized");
            }


            text = File.ReadAllText(newtargetpath);
            buf = Encoding.Unicode.GetBytes(text);
            for (i = 0; i < buf.Length; i++)
            {
                buf[i] = (byte)(buf[i] ^ 1);
            }
            text = Encoding.Unicode.GetString(buf);
            string delfile = newtargetpath;
            newtargetpath = newtargetpath.Remove(newtargetpath.Length - 6) + ".txt";
            File.WriteAllText(newtargetpath, text);
            File.Delete(delfile);
           
            File.Copy(newtargetpath,Path.Combine(@"D:\\csharp\Target\Archive",Name));

             stpath = Path.Combine(@"D:\\csharp\Target\Archive", Name).Remove(Path.Combine(@"D:\\csharp\Target\Archive", Name).Length - 4) + ".gz";
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
        }

    }
}
