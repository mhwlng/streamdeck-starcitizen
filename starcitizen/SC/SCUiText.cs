using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using p4ktest.SC;
using SCJMapper_V2.p4kFile;

namespace SCJMapper_V2.SC
{
    sealed class SCUiText
    {
        public enum Languages
        {
            profile = 0, // use profile texts
            english // must be the one used in the game assets.. Data\Localization\<lang>
        }


        private SCLocale[] m_locales =
        {
            new SCLocale(Languages.profile
                .ToString()), // creates an empty one and will return the default(profile string) later
            new SCLocale(Languages.english.ToString())
        }; // add supported languages

        private Languages m_language = Languages.english;

        /// <summary>
        /// Set the language to be used
        /// </summary>
        public Languages Language
        {
            get => m_language;
            set => m_language = value;
        }

        public IList<string> LanguagesS
        {
            get
            {
                List<string> list = new List<string>();
                foreach (SCLocale l in m_locales)
                {
                    list.Add(l.Language);
                }

                return list;
            }
        }

        // Singleton
        private static readonly Lazy<SCUiText> m_lazy = new Lazy<SCUiText>(() => new SCUiText());

        public static SCUiText Instance
        {
            get => m_lazy.Value;
        }

        /// <summary>
        /// Load all languages from Assets
        ///  like:  dfm_crusader_port_olisar=Port Olisar
        /// </summary>
        public SCUiText()
        {
            foreach (string fileKey in SCFiles.Instance.LangFiles)
            {
                string lang = Path.GetFileNameWithoutExtension(fileKey);
                // check if it is a valid language
                if (Enum.TryParse(lang, out Languages fileLang))
                {
                    string fContent = SCFiles.Instance.LangFile(fileKey);

                    using (TextReader sr = new StringReader(fContent))
                    {
                        string line = sr.ReadLine();
                        while (line != null)
                        {
                            int epo = line.IndexOf('=');
                            string tag = "";
                            string content = "";
                            if (epo >= 0)
                            {
                                tag = line.Substring(0, epo);
                                if (line.Length >= (epo + 1))
                                {
                                    content = line.Substring(epo + 1);
                                }

                                if (tag.StartsWith("ui_", StringComparison.InvariantCultureIgnoreCase))
                                {
                                    // seems all strings we may need are ui_Cxyz
                                    m_locales[(int) fileLang].Add("@" + tag, content); // cAT is prepending the tags
                                }
                            }

                            line = sr.ReadLine();
                        } // while
                    }
                }
            } // all files
        }

        /// <summary>
        /// Returns the content from the UILabel in the set Language
        /// </summary>
        /// <param name="UILabel">The UILabel from defaultProfile</param>
        /// <param name="defaultS">A default string to return if the label cannot be found</param>
        /// <returns>A text string</returns>
        public string Text(string UILabel, string defaultS)
        {
            try
            {
                string retVal = "";
                if (m_locales[(int) m_language].ContainsKey(UILabel))
                {
                    retVal = m_locales[(int) m_language][UILabel];
                }

                //if ( string.IsNullOrEmpty( retVal ) )
                //  if ( m_locales[(int)Languages.english].ContainsKey( UILabel ) ) {
                //    retVal = m_locales[(int)Languages.english][UILabel]; // fallback to english
                //  }
                if (string.IsNullOrEmpty(retVal))
                    retVal = defaultS; // final fallback to default
                return retVal.Replace("Â", "").Trim();
            }
            catch
            {
                //Logger.Instance.LogMessage(TracingLevel.DEBUG, "SCLocale - Language not valid ??!!" );
            }

            return defaultS;
        }


    }
}
