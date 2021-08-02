using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Input;
using WindowsInput.Native;
using BarRaider.SdTools;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

// ReSharper disable StringLiteralTypo

namespace starcitizen.Buttons
{

    [PluginActionId("com.mhwlng.starcitizen.static")]
    public class Static : StarCitizenBase
    {
        protected class PluginSettings
        {
            public static PluginSettings CreateDefaultSettings()
            {
                var instance = new PluginSettings
                {
                    Function = string.Empty,
                };

                return instance;
            }

            [JsonProperty(PropertyName = "function")]
            public string Function { get; set; }

            [FilenameProperty]
            [JsonProperty(PropertyName = "clickSound")]
            public string ClickSoundFilename { get; set; }

        }


        PluginSettings settings;
        private CachedSound _clickSound = null;


        public Static(SDConnection connection, InitialPayload payload) : base(connection, payload)
        {
            if (payload.Settings == null || payload.Settings.Count == 0)
            {
                //Logger.Instance.LogMessage(TracingLevel.DEBUG, "Repeating Static Constructor #1");

                settings = PluginSettings.CreateDefaultSettings();
                Connection.SetSettingsAsync(JObject.FromObject(settings)).Wait();

            }
            else
            {
                //Logger.Instance.LogMessage(TracingLevel.DEBUG, "Repeating Static Constructor #2");

                settings = payload.Settings.ToObject<PluginSettings>();
                HandleFileNames();
            }

        }


        public override void KeyPressed(KeyPayload payload)
        {
            if (InputRunning || Program.dpReader == null)
            {
                ForceStop = true;
                return;
            }

            ForceStop = false;

            var action = Program.dpReader.GetBinding(settings.Function);
            if (action != null)
            {
                Logger.Instance.LogMessage(TracingLevel.INFO, CommandTools.ConvertKeyString(action.Keyboard));

                SendKeypressDown(CommandTools.ConvertKeyString(action.Keyboard));
            }

            if (_clickSound != null)
            {
                try
                {
                    AudioPlaybackEngine.Instance.PlaySound(_clickSound);
                }
                catch (Exception ex)
                {
                    Logger.Instance.LogMessage(TracingLevel.FATAL, $"PlaySound: {ex}");
                }

            }

        }

        public override void KeyReleased(KeyPayload payload)
		{

            if (InputRunning || Program.dpReader == null)
            {
                ForceStop = true;
                return;
            }

            ForceStop = false;

            var action = Program.dpReader.GetBinding(settings.Function);
            if (action != null)
            {
                Logger.Instance.LogMessage(TracingLevel.INFO, CommandTools.ConvertKeyString(action.Keyboard));

                SendKeypressUp(CommandTools.ConvertKeyString(action.Keyboard));
            }

        }

        public override void Dispose()
        {
            base.Dispose();

            //Logger.Instance.LogMessage(TracingLevel.DEBUG, "Destructor called #1");
        }


        public override void ReceivedSettings(ReceivedSettingsPayload payload)
        {
            //Logger.Instance.LogMessage(TracingLevel.DEBUG, "ReceivedSettings");

            // New in StreamDeck-Tools v2.0:
            BarRaider.SdTools.Tools.AutoPopulateSettings(settings, payload.Settings);
            HandleFileNames();
        }

        private void HandleFileNames()
        {
            _clickSound = null;
            if (File.Exists(settings.ClickSoundFilename))
            {
                try
                {
                    _clickSound = new CachedSound(settings.ClickSoundFilename);
                }
                catch (Exception ex)
                {
                    Logger.Instance.LogMessage(TracingLevel.FATAL, $"CachedSound: {settings.ClickSoundFilename} {ex}");

                    _clickSound = null;
                    settings.ClickSoundFilename = null;
                }
            }

            Connection.SetSettingsAsync(JObject.FromObject(settings)).Wait();
        }
    }
}
