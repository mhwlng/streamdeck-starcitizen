using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Serialization;
using BarRaider.SdTools;
using p4ktest.SC;
using SCJMapper_V2.SC;

namespace starcitizen
{
    public class KeyBindingFileEvent : EventArgs
    {

    }

    public class KeyBindingWatcher : FileSystemWatcher
    {
        public event EventHandler KeyBindingUpdated;

        protected KeyBindingWatcher()
        {

        }

        public KeyBindingWatcher(string path, string fileName)
        {
            Filter = fileName;
            NotifyFilter = NotifyFilters.CreationTime | NotifyFilters.LastWrite;
            Path = path;
        }

        public virtual void StartWatching()
        {
            if (EnableRaisingEvents)
            {
                return;
            }

            Changed -= UpdateStatus;
            Changed += UpdateStatus;

            EnableRaisingEvents = true;
        }

        public virtual void StopWatching()
        {
            try
            {
                if (EnableRaisingEvents)
                {
                    Changed -= UpdateStatus;

                    EnableRaisingEvents = false;
                }
            }
            catch (Exception e)
            {
                Trace.TraceError($"Error while stopping Status watcher: {e.Message}");
                Trace.TraceInformation(e.StackTrace);
            }
        }

        protected void UpdateStatus(object sender, FileSystemEventArgs e)
        {
            Thread.Sleep(50);

            KeyBindingUpdated?.Invoke(this, EventArgs.Empty);
        }


    }
    class Program
    {
        public static FifoExecution keywatcherjob = new FifoExecution();

        public static KeyBindingWatcher KeyBindingWatcher;

        public static DProfileReader dpReader = new DProfileReader(); 

        public static string profile;

        public static string statictemplate;

        public static string macrotemplate;

        public static void HandleKeyBindingEvents(object sender, object evt)
        {
            Logger.Instance.LogMessage(TracingLevel.INFO, $"Reloading Key Bindings");

            keywatcherjob.QueueUserWorkItem(GetKeyBindings, null);
        }


        private static void GetKeyBindings(Object threadContext)
        {
            if (KeyBindingWatcher != null)
            {
                KeyBindingWatcher.StopWatching();
                KeyBindingWatcher.Dispose();
                KeyBindingWatcher = null;
            }

            // load stuff
            var actionmaps = SCDefaultProfile.ActionMaps();

            dpReader = new DProfileReader();

            dpReader.fromXML(profile);

            dpReader.fromActionProfile(actionmaps);

            dpReader.Actions();

            dpReader.CreateStaticHtml(statictemplate);

            dpReader.CreateMacroHtml(macrotemplate);

            dpReader.CreateCsv();

            Logger.Instance.LogMessage(TracingLevel.INFO, "monitoring key binding file" );
            KeyBindingWatcher = new KeyBindingWatcher(SCPath.SCClientProfilePath, "actionmaps.xml");
            KeyBindingWatcher.KeyBindingUpdated += HandleKeyBindingEvents;
            KeyBindingWatcher.StartWatching();
        }


        static void Main(string[] args)
        {
            Logger.Instance.LogMessage(TracingLevel.INFO, "Init Star Citizen");

            try
            {
                SCFiles.Instance.UpdatePack(); // update game files

                statictemplate = File.ReadAllText("statictemplate.html");

                macrotemplate = File.ReadAllText("macrotemplate.html");

                profile = SCDefaultProfile.DefaultProfile();

                GetKeyBindings(null);

            }
            catch (Exception ex)
            {
                Logger.Instance.LogMessage(TracingLevel.FATAL, $"DProfileReader: {ex}");
            }

            Logger.Instance.LogMessage(TracingLevel.INFO, "Finished Init Star Citizen");
            
            //var langFiles = SCFiles.Instance.LangFiles;
            //var langFile = SCFiles.Instance.LangFile(langFiles[0]);
            //var txt = SCUiText.Instance.Text("@ui_COMiningThrottle", "???");

            // Write the string array to a new file named "WriteLines.txt".


            // Uncomment this line of code to allow for debugging
            //while (!System.Diagnostics.Debugger.IsAttached) { System.Threading.Thread.Sleep(100); }

            SDWrapper.Run(args);


        }
    }
}
