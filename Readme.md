this is a very special solution for a very special problem
what doe we have
a solution as an .exe or as an windows service (yes this is also an exe)
and it's a german problem:\
\
Für jedes zu archivierende Dokumen wird vom Rechnunsgmanager eine csv-Datei mit dem gleichen Dateinamen .csv wie die TIFF erstellt.
Die csv Datei enthält genau eine Zeile, mit durch Semikolen separierten Werten
Der 1. Eintrag (Index 0) ist der vollst. Dateiname der TIF , der siebente (Index 6) die Nummer des Mandanten (1,2,3).
Der Kendox-Batch-Importer kann die Mandanten nicht auswerten, somit können im Archiv keine separaten Sicherheitsbereiche etc. genutzt werden.
Wir umgehen das, indem wir die csv-Dateien mandantenweise in separate Ordner sortieren und für jeden Ordner einen Batch-Job anlegen.
Die Ordner werden entprechnend der Mandatennummer benannt (1,2,3).
Die Dateinamen für csv und TIF sind achtstellige Hexadezimalzahlen, wir wissen nicht, nach welcher Logig die Namen vergeben werden.
Es reicht, nur die csv in die Mandantenordner zu verschieben, der Pfad+Dateiname der TIF bleibt gleich. Unklar ist aber, ob u.U. die TIF 
überschrieben werden könnte, wenn die csv im Standardordner fehlt. \
Werden beide Dateien in den Mandantenorner verschoben würden bei Namensdopplungen die nachfolgenden nicht verschoben, weil hier in diesem Modul
auf Existenz geprüft wird. Somit findet aber auch keine Weiterverarbeitung statt, beide Dateien würden im Standardordner verbleiben.
Bei diesem Vorgehen muss aber der vollständige Dateiname der TIF Datei in der csv angepasst werden
Da der Batchimport über die Aufgabensteuerung einmal täglich läuft, ist die Einrichtung eines Windows Service mit FileSystemWatcher nicht erforderlich.
Dieser Service eurde eher als Proof of Concept und als Beispiel für Multitheading programmiert.
Die Klassen finden Verwendung bei der Consolenanwedung welche dann kurz vor dem Batchimport ebenfalls über die Aufgabensteuerung gestartet wird.
Da der Batchimprt in der Regel außerhalb der Arbeitszeit erfolgt, wird diese Konsolenanwendung wie folgt ablaufen:
Duchsuchen des Standardverzeichnisses und nur Verschieben der csv-Dateien, die TIFF Dateien bleiben im Standardordner, der Batchimport läuft 5 Minuten später
und nur während dieser 5 Minuten könnten ein Namenskonflikt durch den Rechnungsmanager entstehen, was aber sehr unwahrscheinlich ist (weil außerhalb der Arbeitszeit).
