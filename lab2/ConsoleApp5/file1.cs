using System;
using System.Collections;
using System.IO;
using System.Text;

namespace ConsoleApp5
{
    public class file1
    {

        private ArrayList Countryballs;

        public readonly string Root;

        public file1()
        {

            Countryballs = new ArrayList();
            Root = @"D:\\csharp\Source";
        }


        public void AddToList()
        {
            int n;
            Console.WriteLine("input num of elements :");

            n = int.Parse(Console.ReadLine() ?? throw new Exception("fuck u"));

            for (int i = 1; i <= n; i++)
            {

                var obj = Console.ReadLine();
                Countryballs.Add(obj);
            }
        }



        public void Write()
        {
            try
            {
                AddToList();
            }
            catch (Exception)
            {
                Console.WriteLine("error");
                return;
            }

            try
            {
                string str = DateTime.Now.ToString();
                StringBuilder str1 = null;
                // Console.WriteLine(str);
                string day = str.Substring(0, 2);
                string month = str.Substring(3, 2);
                string year = str.Substring(6, 4);
                str = str.Replace(' ', '_');
                str = str.Replace('.', '_');
                str = str.Replace(':', '_');

                string path = Path.Combine(year, month, day);
                string subdir = path;
                DirectoryInfo dirInfo = new DirectoryInfo(Root);
                if (!dirInfo.Exists)
                {
                    dirInfo.Create();
                }
                dirInfo.CreateSubdirectory(subdir ?? throw new Exception("null"));
                path = Path.Combine(Root, path);
                
                string name = "Sales_" +str;
                name = Path.Combine(path, name);
               
                using (StreamWriter sw = new StreamWriter(name, true, System.Text.Encoding.Default))
                {
                    foreach (var variable in Countryballs)
                    {
                        sw.WriteLine(variable);
                    }
                }

                Countryballs.Clear();
            }
            catch (Exception)
            {
                Console.WriteLine("Error with saving in file");
            }
        }

       


    
    }

}