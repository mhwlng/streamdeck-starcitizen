using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BarRaider.SdTools;
using p4ktest.SC;
using SCJMapper_V2.SC;

namespace starcitizen
{
    class Program
    {
        public static DProfileReader dpReader = new DProfileReader(); // we may read a profile

        static void Main(string[] args)
        {
            Logger.Instance.LogMessage(TracingLevel.INFO, "Init Star Citizen");


            try
            {

                SCFiles.Instance.UpdatePack(); // update game files

                var profile = SCDefaultProfile.DefaultProfile();

                var actionmaps = SCDefaultProfile.ActionMaps();

                dpReader.fromXML(profile);

                dpReader.fromActionProfile(actionmaps);

                dpReader.Actions();

                dpReader.CreateDropdownTemplate();

                dpReader.CreateCsv();

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
