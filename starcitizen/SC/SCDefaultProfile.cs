using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using BarRaider.SdTools;
using SCJMapper_V2.CryXMLlib;

namespace SCJMapper_V2.SC
{

    /// <summary>
    /// Finds and returns the DefaultProfile from SC GameData.pak
    /// it is located in GameData.pak \Libs\Config
    /// </summary>
    class SCDefaultProfile
    {
        private static string m_defProfileCached = ""; // cache...

        /// <summary>
        /// Returns a list of files found that match 'defaultProfile*.xml'
        /// 20151220BM: return only the single defaultProfile name
        /// </summary>
        /// <returns>A list of filenames - can be empty</returns>
        static public string DefaultProfileName
        {
            get { return "defaultProfile.xml"; }
        }

        /// <summary>
        /// Returns the sought default profile as string from various locations
        /// SC Alpha 2.2: Have to find the new one in E:\G\StarCitizen\StarCitizen\Public\Data\DataXML.pak (contains the binary XML now)
        /// </summary>
        /// <returns>A string containing the file contents</returns>
        static public string DefaultProfile()
        {
            //Logger.Instance.LogMessage(TracingLevel.DEBUG,"DefaultProfile - Entry");

            string retVal = m_defProfileCached;
            if (!string.IsNullOrEmpty(retVal)) return retVal; // Return cached defaultProfile

            retVal = SCFiles.Instance.DefaultProfile;
            if (!string.IsNullOrEmpty(retVal))
            {
                m_defProfileCached = retVal;
                return retVal; // EXIT
            }


            return retVal; // EXIT
        }

        public static string ActionMaps()
        {
            string mFile = Path.Combine(SCPath.SCClientProfilePath, "actionmaps.xml");

            Logger.Instance.LogMessage(TracingLevel.INFO, mFile);

            if (File.Exists(mFile))
            {
                using (var sr = new StreamReader(mFile))
                {
                    return sr.ReadToEnd();
                }
            }

            return "";

        }


    }
}
