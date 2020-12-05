using System;
using System.IO;
using System.Text;


namespace PrepareRmArchivBatchImport
{
    // Für jedes zu archivierende Dokumen wird vom Rechnunsgmanager eine csv-Datei mit dem gleichen Dateinamen .csv wie die TIFF erstellt.
    // Die csv Datei enthält genau eine Zeile, mit durch Semikolen separierten Werten
    // Der 1. Eintrag (Index 0) ist der vollst. Dateiname der TIF , der siebente (Index 6) die Nummer des Mandanten (1,2,3).
    // Der Kendox-Batch-Importer kann die Mandanten nicht auswerten, somit können im Archiv keine separaten Sicherheitsbereiche etc. genutzt werden.
    // Wir umgehen das, indem wir die csv-Dateien mandantenweise in separate Ordner sortieren und für jeden Ordner einen Batch-Job anlegen.
    // Die Ordner werden entprechnend der Mandatennummer benannt (1,2,3).
    // Die Dateinamen für csv und TIF sind achtstellige Hexadezimalzahlen, wir wissen nicht, nach welcher Logig die Namen vergeben werden.
    // Es reicht, nur die csv in die Mandantenordner zu verschieben, der Pfad+Dateiname der TIF bleibt gleich. Unklar ist aber, ob u.U. die TIF 
    // überschrieben werden könnte, wenn die csv im Standardordner fehlt. 
    //
    // Werden beide Dateien in den Mandantenorner verschoben würden bei Namensdopplungen die nachfolgenden nicht verschoben, weil hier in diesem Modul
    // auf Existenz geprüft wird. Somit findet aber auch keine Weiterverarbeitung statt, beide Dateien würden im Standardordner verbleiben.
    // Bei diesem Vorgehen muss aber der vollständige Dateiname der TIF Datei in der csv angepasst werden
    //
    // Da der Batchimport über die Aufgabensteuerung einmal täglich läuft, ist die Einrichtung eines Windows Service mit FileSystemWatcher nicht erforderlich.
    // Dieser Service eurde eher als Proof of Concept und als Beispiel für Multitheading programmiert.
    // Die Klassen finden Verwendung bei der Consolenanwedung welche dann kurz vor dem Batchimport ebenfalls über die Aufgabensteuerung gestartet wird.
    // Da der Batchimprt in der Regel außerhalb der Arbeitszeit erfolgt, wird diese Konsolenanwendung wie folgt ablaufen:
    // Duchsuchen des Standardverzeichnisses und nur Verschieben der csv-Dateien, die TIFF Dateien bleiben im Standardordner, der Batchimport läuft 5 Minuten später
    // und nur während dieser 5 Minuten könnten ein Namenskonflikt durch den Rechnungsmanager entstehen, was aber sehr unwahrscheinlich ist (weil außerhalb der Arbeitszeit).
    class RMCSVFile
    {
        private readonly string fileName = null;
        private string line = null;

        public RMCSVFile(string FileName)
        {
            this.fileName = FileName;
        }

        private string GetLine()
        {
            if (line == null)
            {
                try
                {
                    using (StreamReader sr = new StreamReader(fileName))
                    {
                        while (sr.Peek() >= 0)
                        {
                            line = sr.ReadLine();
                        }
                    }
                }
                catch (Exception e)
                {
                    Logger.Log(Logger.LogLevel.WARNING, String.Format("Kann Zeile nicht lesen {0} {1} ", fileName, e.Message));
                }
            }
            return line;
        }

        private string GetOldTIFFullFileNameCSV()
        {
            string tiffFileName = null;
            try
            {
                var entries = GetLine().Split(';');
                tiffFileName = entries[0];
            }
            catch (Exception e)
            {
                Logger.Log(Logger.LogLevel.INFO, String.Format("keinen TIF Dateineamen gefunden  {0} {1}", fileName, e.Message));
            }
            return tiffFileName;
        }

        private string GetNewTIFFullFileNameCSV()
        {
            return Path.GetDirectoryName(GetOldTIFFullFileNameCSV()) + "\\" + GetMandant() + "\\" + Path.GetFileName(GetOldTIFFullFileNameCSV());
        }
        
        private string GetNewCSVDirectory()
        {
            return Path.GetDirectoryName(fileName) + "\\" + GetMandant() + "\\";
        }

        private string GetNewLineCSV()
        {
            string newLine = null;
            try
            {
                var entries = GetLine().Split(';');
                string[] newEntries = (string[])entries.Clone();
                newEntries[0] = GetNewTIFFullFileNameCSV();
                StringBuilder sb = new StringBuilder("", 200);
                foreach (string entry in newEntries)
                {
                    sb.Append(entry);
                    sb.Append(";");
                }
                newLine = sb.ToString();
            }
            catch (Exception e)
            {
                Logger.Log(Logger.LogLevel.INFO, String.Format("kann keine neue Zeile erzeugen {0} {1} ", fileName, e.Message));
            }
            return newLine;
        }

        public string GetMandant()
        {
            string mandantNr = null;
            try
            {
                var entries = GetLine().Split(';');
                mandantNr = entries[6];
            }
            catch (Exception e)
            {
                Logger.Log(Logger.LogLevel.INFO, String.Format("Keinen Mandant gefunden  {0} {1}", fileName, e.Message));
            }
            return mandantNr;
        }

        public string GetTIFFileName()
        {
            return Path.GetFileName(GetOldTIFFullFileNameCSV());
        }

        public bool SaveAsNewFile()
        {
            bool succes = false;
            try
            {
                string newFileName = GetNewCSVDirectory() + Path.GetFileName(fileName);
                if (!File.Exists(newFileName))
                {
                    using (StreamWriter outputFile = new StreamWriter(newFileName))
                    {
                        outputFile.WriteLine(GetNewLineCSV());
                    }
                    succes = true;
                }
                else
                { 
                    Logger.Log(Logger.LogLevel.INFO, String.Format("SaveAsNewFile Datei existiert bereits {0}", newFileName));
                }
            }
            catch (Exception e)
            {
                Logger.Log(Logger.LogLevel.INFO, String.Format("SaveAsNewFile fehlgeschlagen {0}", e.Message));
            }
            return succes;
        }

        public bool MoveTIF()
        {
            bool succes = false;
            try
            {
                string newFileName = GetNewCSVDirectory() + Path.GetFileName(GetTIFFileName());
                if (!File.Exists(newFileName))
                {
                    File.Move(Path.GetDirectoryName(fileName) + "\\" + GetTIFFileName(), newFileName);
                    succes = true;
                }
                else
                {
                    Logger.Log(Logger.LogLevel.INFO, String.Format("MoveTIF Datei existiert bereits {0}", newFileName));
                }
            }
            catch (Exception e)
            {
                Logger.Log(Logger.LogLevel.INFO, String.Format("MoveTIF fehlgeschlagen {0} {1}", GetTIFFileName(), e.Message));
            }
            return succes;
        }

        public bool DeleteOldCSV()
        {
            bool succes = false;
            try
            {
                File.Delete(fileName);
                succes = true;
            }
            catch (Exception e)
            {
                Logger.Log(Logger.LogLevel.INFO, String.Format("Fehler beim Löschen {0} {1}", fileName, e.Message));
            }
            return succes;
        }
    }
}
