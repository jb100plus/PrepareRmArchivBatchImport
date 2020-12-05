using PrepareRmArchivBatchImport;
using System;
using System.IO;
using System.Threading;

namespace PrepareRmArchivBatchImportConsole
{
    class Program
    {


        static void Main(string[] args)
        {
            string[] fileEntries = Directory.GetFiles(@"D:\temp\", "*.csv");
            foreach (string fileName in fileEntries)
            {

                Mover mv = new Mover(fileName, @"d:\temp\4", new Mover.MoverCallback(Callback));
                Thread tws = new Thread(new ThreadStart(mv.MoveCSV));
                tws.Start();
                Console.WriteLine(fileName);
            }
            //Console.WriteLine(fileName);
            Console.WriteLine(fileEntries.Length);
            Console.ReadLine();
        }

        public static void Callback(string sourcefilnename)
        {
            //Logger.Log(Logger.LogLevel.DEBUG, "callback" + sourcefilnename);
            Console.WriteLine("callback" + sourcefilnename);
        }
    }
}

