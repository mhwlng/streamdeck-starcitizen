using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Linq;
using System.Text;
using System.IO;
using BarRaider.SdTools;
using Microsoft.Win32;
using p4ktest;
using TheUser = p4ktest.SC.TheUser;

//using SCJMapper_V2.Translation;

namespace SCJMapper_V2.SC
{
    /// <summary>
    /// Find the SC pathes and folders
    /// </summary>
    class SCPath
    {
        /// <summary>
        /// Try to locate the launcher from Alpha 3.0.0 public - e.g. E:\G\StarCitizen\RSI Launcher
        /// Alpha 3.6.0 PTU launcher 1.2.0 has the same entry (but PTU location changed)
        /// </summary>
        static private string SCLauncherDir6
        {
            get
            {
                //Logger.Instance.LogMessage(TracingLevel.DEBUG,"SCLauncherDir6 - Entry");

                RegistryKey localKey;
                if (Environment.Is64BitOperatingSystem)
                    localKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64);
                else
                    localKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry32);

                string scLauncher = localKey.OpenSubKey(@"SOFTWARE\81bfc699-f883-50c7-b674-2483b6baae23")
                    .GetValue("InstallLocation").ToString();

                if (scLauncher != null)
                {
                    //Logger.Instance.LogMessage(TracingLevel.DEBUG,"SCLauncherDir6 - Found HKLM -InstallLocation");
                    if (Directory.Exists(scLauncher))
                    {
                        return scLauncher;
                    }
                    else
                    {
                        //Logger.Instance.LogMessage(TracingLevel.DEBUG,"SCLauncherDir6 - directory does not exist: {0}", scLauncher);
                        return "";
                    }
                }

                //Logger.Instance.LogMessage(TracingLevel.DEBUG,"SCLauncherDir6 - did not found HKLM - InstallLocation");
                return "";
            }
        }

        /// <summary>
        /// Try to locate the launcher from Alpha 3.0.0 PTU - e.g. E:\G\StarCitizen\RSI PTU Launcher
        /// </summary>
        static private string SCLauncherDir5
        {
            get
            {
                //Logger.Instance.LogMessage(TracingLevel.DEBUG,"SCLauncherDir5 - Entry");

                RegistryKey localKey;
                if (Environment.Is64BitOperatingSystem)
                    localKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64);
                else
                    localKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry32);

                string scLauncher = localKey.OpenSubKey(@"SOFTWARE\94a6df8a-d3f9-558d-bb04-097c192530b9")
                    .GetValue("InstallLocation").ToString();

                if (scLauncher != null)
                {
                    //Logger.Instance.LogMessage(TracingLevel.DEBUG,"SCLauncherDir5 - Found HKLM -InstallLocation (PTU)");
                    if (Directory.Exists(scLauncher))
                    {
                        return scLauncher;
                    }
                    else
                    {
                        //Logger.Instance.LogMessage(TracingLevel.DEBUG,"SCLauncherDir5 - directory does not exist: {0}", scLauncher);
                        return "";
                    }
                }

                //Logger.Instance.LogMessage(TracingLevel.DEBUG,"SCLauncherDir5 - did not found HKLM - InstallLocation");
                return "";
            }
        }

        /// <summary>
        /// Returns the base SC install path from something like "E:\G\StarCitizen"
        /// </summary>
        static private string SCBasePath
        {
            get
            {
                //Logger.Instance.LogMessage(TracingLevel.DEBUG,"SCBasePath - Entry");

                        string scp = "";

                // start the registry search - sequence  5..1 to get the newest method first

                scp = SCLauncherDir6; // 3.0 Public Launcher
#if DEBUG
                //***************************************
                //scp = ""; // TEST not found (COMMENT OUT FOR PRODUCTIVE BUILD)
                //***************************************
#endif
                if (!string.IsNullOrEmpty(scp))
                {
                    scp = Path.GetDirectoryName(scp); // "E:\G\StarCitizen"
                    return scp;
                }

                scp = SCLauncherDir5; // 3.0 PTU Launcher
#if DEBUG
                //***************************************
                //scp = ""; // TEST not found (COMMENT OUT FOR PRODUCTIVE BUILD)
                //***************************************
#endif
                if (!string.IsNullOrEmpty(scp))
                {
                    scp = Path.GetDirectoryName(scp); // "E:\G\StarCitizen"
                    return scp;
                }

                // nothing found
                //Logger.Instance.LogMessage(TracingLevel.DEBUG,"SCBasePath - cannot find any valid SC path");
                // Issue a warning here to let the user know
                //string issue = "SCBasePath - cannot find any valid SC path";
                //string.Format( "Cannot find the SC Installation Path !!\nUse Settings to provide the path manually" );


                return ""; // sorry did not found a thing..
            } // get

        }

        /// <summary>
        /// Returns the SC Client path  
        /// SC 3.0.0: search path like  E:\G\StarCitizen\StarCitizen\LIVE 
        /// </summary>
        static public string SCClientPath
        {
            get
            {
                //Logger.Instance.LogMessage(TracingLevel.DEBUG,"SCClientPath - Entry");
                string scp = SCBasePath;
#if DEBUG
                //***************************************
                // scp += "X"; // TEST not found (COMMENT OUT FOR PRODUCTIVE BUILD)
                //***************************************
#endif
                string issue = "";

                if (string.IsNullOrEmpty(scp)) return ""; // no valid one can be found


                // 20180321 New PTU 3.1 another change in setup path - Testing for PTU first 
                // 20190711 Lanuncher 1.2 - PTU has moved - change detection to find this one first.
                if (TheUser.UsePTU)
                {
                    if (Directory.Exists(Path.Combine(scp, @"StarCitizen\PTU")))
                    {
                        scp = Path.Combine(scp, @"StarCitizen\PTU");
                    }
                    else if (Directory.Exists(Path.Combine(scp, @"StarCitizenPTU\LIVE")))
                    {
                        // this would be old style
                        scp = Path.Combine(scp, @"StarCitizenPTU\LIVE");
                    }
                }
                else
                {
                    // no PTU ..
                    scp = Path.Combine(scp, @"StarCitizen\LIVE");
                }

                if (Directory.Exists(scp)) return scp; // EXIT Success

                //Logger.Instance.LogMessage(TracingLevel.DEBUG,"SCClientPath - StarCitizen\\LIVE or PTU subfolder does not exist: {0}", scp);
                // Issue a warning here to let the user know
                issue = string.Format("SCClientPath - StarCitizen\\LIVE or PTU subfolder does not exist: {0}", scp);

                Logger.Instance.LogMessage(TracingLevel.ERROR, $"{issue}");

                //"Cannot find the SC Client Directory !!\n\nTried to look for:\n{0} \n\nPlease adjust the path in Settings\n"
                // Issue a warning here to let the user know

                return "";
            }
        }

        /// <summary>
        /// Returns the SC ClientData path
        /// AC 3.0: E:\G\StarCitizen\StarCitizen\LIVE\USER
        /// AC 3.13: E:\G\StarCitizen\StarCitizen\LIVE\USER\Client\0
        /// </summary>
        static public string SCClientUSERPath
        {
            get
            {
                //Logger.Instance.LogMessage(TracingLevel.DEBUG,"SCClientUSERPath - Entry");
                string scp = SCClientPath;
                if (string.IsNullOrEmpty(scp)) return "";
                //
                string scpu = Path.Combine(scp, "USER", "Client", "0"); // 20210404 new path
                if (!Directory.Exists(scpu))
                {
                    scpu = Path.Combine(scp, "USER"); // 20210404 old path
                }

#if DEBUG
                //***************************************
                // scp += "X"; // TEST not found (COMMENT OUT FOR PRODUCTIVE BUILD)
                //***************************************
#endif
                if (Directory.Exists(scpu)) return scpu;

                //Logger.Instance.LogMessage(TracingLevel.DEBUG,@"SCClientUSERPath - StarCitizen\\LIVE\\USER[\Client\0] subfolder does not exist: {0}",scpu);
                return "";
            }
        }

        static public string SCClientProfilePath
        {
            get
            {
                if (File.Exists("appSettings.config") &&
                    ConfigurationManager.GetSection("appSettings") is NameValueCollection appSection)
                {
                    if ((!string.IsNullOrEmpty(appSection["SCClientProfilePath"]) && !string.IsNullOrEmpty(Path.GetDirectoryName(appSection["SCClientProfilePath"]))))
                    {
                        return appSection["SCClientProfilePath"];
                    }
                }

                //Logger.Instance.LogMessage(TracingLevel.DEBUG,"SCClientProfilePath - Entry");
                string scp = SCClientUSERPath; 
                if (string.IsNullOrEmpty(scp)) return "";
                //
                scp = Path.Combine(scp, "Profiles", "default");

                if (Directory.Exists(scp)) return scp;

                //Logger.Instance.LogMessage(TracingLevel.DEBUG,@"SCClientProfilePath - StarCitizen\LIVE\USER\[Client\0\]Profiles\default subfolder does not exist: {0}",scp);
                return "";
            }
        }



        /// <summary>
        /// Returns the SC Data.p4k file path
        /// SC Alpha 3.0: E:\G\StarCitizen\StarCitizen\LIVE\Data.p4k (contains the binary XML now)
        /// </summary>
        static public string SCData_p4k
        {
            get
            {
                if (File.Exists("appSettings.config") &&
                    ConfigurationManager.GetSection("appSettings") is NameValueCollection appSection)
                {
                    if ((!string.IsNullOrEmpty(appSection["SCData_p4k"]) && File.Exists(appSection["SCData_p4k"])))
                    {
                        return appSection["SCData_p4k"];
                    }
                }

                //Logger.Instance.LogMessage(TracingLevel.DEBUG,"SCDataXML_p4k - Entry");
                string scp = SCClientPath;
                if (string.IsNullOrEmpty(scp)) return "";
                //
                scp = Path.Combine(scp, "Data.p4k");
#if DEBUG
                //***************************************
                // scp += "X"; // TEST not found (COMMENT OUT FOR PRODUCTIVE BUILD)
                //***************************************
#endif
                if (File.Exists(scp)) return scp;

                //Logger.Instance.LogMessage(TracingLevel.DEBUG,@"SCData_p4k - StarCitizen\LIVE or PTU\Data\Data.p4k file does not exist: {0}", scp);
                return "";
            }
        }



    }
}
