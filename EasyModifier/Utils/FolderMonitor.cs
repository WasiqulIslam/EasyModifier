using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Permissions;
using System.Text;

//Developing by Wasiqul Islam at 1st June, 2013

namespace EasyModifier.Utils
{

    public delegate void NewFileDelegate(string filePath);

    public class FolderMonitor
    {

        private bool monitorCreation = false;
        private bool monitorModification = false;
        private bool monitorRename = false;
        private FileSystemWatcher watcher;
        public NewFileDelegate NewFileCreated = null;
        private string monitoredFolderPath = null;

        //constructor
        public FolderMonitor(string monitoredFolderPath, bool monitorCreation, bool monitorModification, bool monitorRename)
        {
            this.monitoredFolderPath = monitoredFolderPath;
            this.monitorCreation = monitorCreation;
            this.monitorModification = monitorModification;
            this.monitorRename = monitorRename;
            Run();
        }
        
        //destructor
        ~FolderMonitor()
        {
            StopMonitoring();
        }

        /// <summary>
        /// Call this to stop monitoring
        /// </summary>
        public void StopMonitoring()
        {
            watcher.Dispose();
        }

        [PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
        public void Run()
        {

            // Create a new FileSystemWatcher and set its properties.
            watcher = new FileSystemWatcher();
            watcher.Path = this.monitoredFolderPath;
            /* Watch for changes in LastAccess and LastWrite times, and 
               the renaming of files or directories. */
            watcher.NotifyFilter = NotifyFilters.LastAccess | NotifyFilters.LastWrite
               | NotifyFilters.FileName | NotifyFilters.DirectoryName;
            // Only watch text files.
            watcher.Filter = "*.txt";

            // Add event handlers.
            watcher.Changed += new FileSystemEventHandler(OnChanged);
            watcher.Created += new FileSystemEventHandler(OnChanged);
            watcher.Deleted += new FileSystemEventHandler(OnChanged);
            watcher.Renamed += new RenamedEventHandler(OnRenamed);

            // Begin watching.
            watcher.EnableRaisingEvents = true;

            //// Wait for the user to quit the program.
            //Console.WriteLine("Press \'q\' to quit the sample.");
            //while (Console.Read() != 'q') ;
        }

        // Define the event handlers.
        private void OnChanged(object source, FileSystemEventArgs e)
        {
            // Specify what is done when a file is changed, created, or deleted.
            if (
                    (e.ChangeType == WatcherChangeTypes.Created && this.monitorCreation )
                    || 
                    (e.ChangeType == WatcherChangeTypes.Changed && this.monitorModification)
               )
            {
                    NewFileCreated(e.FullPath);
            }
        }

        private void OnRenamed(object source, RenamedEventArgs e)
        {
            if (monitorRename)
            {
                NewFileCreated(e.FullPath);
            }
            // Specify what is done when a file is renamed.
            //MessageBox.Show(String.Format("File: {0} renamed to {1}", e.OldFullPath, e.FullPath));
        }

    }
}
