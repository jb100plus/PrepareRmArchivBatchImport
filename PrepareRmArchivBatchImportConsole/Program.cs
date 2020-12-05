using PrepareRmArchivBatchImport;
using System;
using System.IO;


namespace PrepareRmArchivBatchImportConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            string[] fileEntries = Directory.GetFiles(@"D:\temp\", "*.csv");
            foreach (string fileName in fileEntries)
            {
                RMCSVFile kp = new RMCSVFile(fileName);
                Console.WriteLine(kp.GetTIFFileName());
                Console.WriteLine(kp.GetMandant());
                //if (kp.SaveAsNewFile())
                  //  kp.Delete();
                //Console.WriteLine(kp.GetMandant());
                /*Console.WriteLine(Path.GetDirectoryName(fileName));
                Console.WriteLine(Path.GetFileName(fileName));
                Console.WriteLine(Path.GetExtension(fileName));*/
            }
            //Console.WriteLine(fileName);
            Console.ReadLine();
        }

    }
}

