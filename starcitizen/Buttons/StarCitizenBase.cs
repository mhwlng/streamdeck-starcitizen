using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using WindowsInput;
using WindowsInput.Native;
using BarRaider.SdTools;

namespace starcitizen.Buttons
{
    public abstract class StarCitizenBase : PluginBase
    {
        [DllImport("user32.dll")]
        private static extern uint MapVirtualKeyEx(uint uCode, uint uMapType, IntPtr dwhkl);

        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        private static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        private static extern int GetWindowThreadProcessId(IntPtr handleWindow, out int lpdwProcessID);

        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        private static extern IntPtr GetKeyboardLayout(int WindowsThreadProcessID);

        private static Dictionary<string,string> _lastStatus = new Dictionary<string, string>();

        protected bool InputRunning;
        protected bool ForceStop = false;
        protected StarCitizenBase(SDConnection connection, InitialPayload payload) : base(connection, payload)
        {
            //Logger.Instance.LogMessage(TracingLevel.INFO, "aa");
        }

        public override void Dispose()
        {
            //Logger.Instance.LogMessage(TracingLevel.INFO, "bb");
        }

        public override void KeyReleased(KeyPayload payload) { }


        public override void OnTick()
        {
            //Logger.Instance.LogMessage(TracingLevel.INFO, "dd");

            //var deviceInfo = Connection.DeviceInfo();

        }

        public override void ReceivedGlobalSettings(ReceivedGlobalSettingsPayload payload) { }

        private void SendInput(string inputText, int delay)
        {
            var text = inputText;

            for (var idx = 0; idx < text.Length && !ForceStop; idx++)
            {
                var macro = CommandTools.ExtractMacro(text, idx);
                idx += macro.Length - 1;
                macro = macro.Substring(1, macro.Length - 2);

                HandleMacro(macro, delay);
            }
        }
        private void SendInputDown(string inputText)
        {
            var text = inputText;

            for (var idx = 0; idx < text.Length && !ForceStop; idx++)
            {
                var macro = CommandTools.ExtractMacro(text, idx);
                idx += macro.Length - 1;
                macro = macro.Substring(1, macro.Length - 2);

                HandleMacroDown(macro);
            }
        }

        private void SendInputUp(string inputText)
        {
            var text = inputText;

            for (var idx = 0; idx < text.Length && !ForceStop; idx++)
            {
                var macro = CommandTools.ExtractMacro(text, idx);
                idx += macro.Length - 1;
                macro = macro.Substring(1, macro.Length - 2);

                HandleMacroUp(macro);
            }
        }

        public static void HandleMacro(string macro, int delay)
        {
            var keyStrokes = CommandTools.ExtractKeyStrokes(macro);

            // Actually initiate the keystrokes
            if (keyStrokes.Count > 0)
            {
                var iis = new InputSimulator();
                var keyCode = keyStrokes.Last();
                keyStrokes.Remove(keyCode);

                if (keyStrokes.Count > 0)
                {
                    //iis.Keyboard.ModifiedKeyStroke(keyStrokes.Select(ks => ks).ToArray(), keyCode);

                    iis.Keyboard.DelayedModifiedKeyStroke(keyStrokes.Select(ks => ks), keyCode, delay);

                }
                else // Single Keycode
                {
                    //iis.Keyboard.KeyPress(keyCode);

                    iis.Keyboard.DelayedKeyPress(keyCode, delay);
                }
            }
        }

        private void HandleMacroDown(string macro)
        {
            var keyStrokes = CommandTools.ExtractKeyStrokes(macro);

            // Actually initiate the keystrokes
            if (keyStrokes.Count > 0)
            {
                var iis = new InputSimulator();
                var keyCode = keyStrokes.Last();
                keyStrokes.Remove(keyCode);

                if (keyStrokes.Count > 0)
                {
                    iis.Keyboard.ModifiedKeyStrokeDown(keyStrokes.Select(ks => ks), keyCode);

                }
                else // Single Keycode
                {
                    iis.Keyboard.DelayedKeyPressDown(keyCode);
                }
            }
        }


        private void HandleMacroUp(string macro)
        {
            var keyStrokes = CommandTools.ExtractKeyStrokes(macro);

            // Actually initiate the keystrokes
            if (keyStrokes.Count > 0)
            {
                var iis = new InputSimulator();
                var keyCode = keyStrokes.Last();
                keyStrokes.Remove(keyCode);

                if (keyStrokes.Count > 0)
                {
                    iis.Keyboard.ModifiedKeyStrokeUp(keyStrokes.Select(ks => ks), keyCode);

                }
                else // Single Keycode
                {
                    iis.Keyboard.DelayedKeyPressUp(keyCode);
                }
            }
        }

        protected void SendKeypress(string keyInfo, int delay)
        {
            if (!string.IsNullOrEmpty(keyInfo))
            {
                SendInput("{" + keyInfo + "}", delay);
            }
        }

        protected void SendKeypressDown(string keyInfo)
        {
            if (!string.IsNullOrEmpty(keyInfo))
            {
                SendInputDown("{" + keyInfo + "}");
            }
        }


        protected void SendKeypressUp(string keyInfo)
        {
            if (!string.IsNullOrEmpty(keyInfo))
            {
                SendInputUp("{" + keyInfo + "}");
            }
        }



    }
}
