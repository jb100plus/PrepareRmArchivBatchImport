using PrepareRmArchivBatchImport;
using System;
using System.Configuration;
using System.IO;
using System.Threading;

namespace PrepareRmArchivBatchImportConsole
{
    class Program
    {


        static void Main(string[] args)
        {
            System.Collections.Specialized.NameValueCollection appSettings = ConfigurationManager.AppSettings;
            string dirToWatch = appSettings["DirectoryToWatch"];
            string dirToMove = appSettings["DirectoryToMove"];
            string watchFilter = appSettings["WatchFilter"];

            Logger.Log(Logger.LogLevel.DEBUG, System.String.Format("{0} {1} {2}", dirToWatch, dirToMove, watchFilter));

            string[] fileEntries = Directory.GetFiles(dirToWatch, watchFilter);
            foreach (string fileName in fileEntries)
            {
                Mover mv = new Mover(fileName, dirToMove, new Mover.MoverCallback(Callback));
                Thread tws = new Thread(new ThreadStart(mv.MoveCSV));
                tws.Start();
            }
            Console.WriteLine(fileEntries.Length);
            Console.ReadLine();
        }

        public static void Callback(string sourcefilnename)
        {
            Console.WriteLine("callback" + sourcefilnename);
        }
    }
}

