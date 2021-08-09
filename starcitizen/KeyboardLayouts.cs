using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace starcitizen
{
    // from https://gist.github.com/vurdalakov/9cea795e82109fdacb7062dcb122b42e


    public class KeyboardLayout
    {
        public UInt32 Id { get; }

        public UInt16 LanguageId { get; }
        public UInt16 KeyboardId { get; }

        public String LanguageName { get; }
        public String KeyboardName { get; }

        internal KeyboardLayout(UInt32 id, UInt16 languageId, UInt16 keyboardId, String languageName, String keyboardName)
        {
            this.Id = id;
            this.LanguageId = languageId;
            this.KeyboardId = keyboardId;
            this.LanguageName = languageName;
            this.KeyboardName = keyboardName;
        }
    }

    public static class KeyboardLayouts
    {
        public static KeyboardLayout GetThreadKeyboardLayout(Int32 threadId = 0)
        {
            var keyboardLayoutId = (UInt32)GetKeyboardLayout((UInt32)threadId);
            return CreateKeyboardLayout(keyboardLayoutId);
        }

        public static KeyboardLayout GetProcessKeyboardLayout(Int32 processId = 0)
        {
            var threadId = GetProcessMainThreadId(processId);
            return GetThreadKeyboardLayout(threadId);
        }

        public static UInt32 SetThreadKeyboardLayout(UInt32 keyboardLayoutId)
        {
            return SetKeyboardLayout(keyboardLayoutId, 0);
        }

        public static UInt32 SetProcessKeyboardLayout(UInt32 keyboardLayoutId)
        {
            return SetKeyboardLayout(keyboardLayoutId, KLF_SETFORPROCESS);
        }

        public static KeyboardLayout[] GetSystemKeyboardLayouts()
        {
            var keyboardLayouts = new List<KeyboardLayout>();

            var count = GetKeyboardLayoutList(0, null);
            var keyboardLayoutIds = new IntPtr[count];
            GetKeyboardLayoutList(keyboardLayoutIds.Length, keyboardLayoutIds);

            foreach (var keyboardLayoutId in keyboardLayoutIds)
            {
                var keyboardLayout = CreateKeyboardLayout((UInt32)keyboardLayoutId);
                keyboardLayouts.Add(keyboardLayout);
            }

            return keyboardLayouts.ToArray();
        }

        private static Int32 GetProcessMainThreadId(Int32 processId = 0)
        {
            var process = 0 == processId ? Process.GetCurrentProcess() : Process.GetProcessById(processId);
            return process.Threads[0].Id;
        }

        private static KeyboardLayout CreateKeyboardLayout(UInt32 keyboardLayoutId)
        {
            var languageId = (UInt16)(keyboardLayoutId & 0xFFFF);
            var keyboardId = (UInt16)(keyboardLayoutId >> 16);
            
            return new KeyboardLayout(keyboardLayoutId, languageId, keyboardId,"","");//, GetCultureInfoName(languageId), GetCultureInfoName(keyboardId));

            //String GetCultureInfoName(UInt16 cultureId)
            //{
            //    return CultureInfo.GetCultureInfo(cultureId).DisplayName;
            //}
        }

        private static UInt32 SetKeyboardLayout(UInt32 keyboardLayoutId, UInt32 flags)
        {
            return ActivateKeyboardLayout((IntPtr)keyboardLayoutId, flags);
        }

        [DllImport("user32.dll")]
        private static extern IntPtr GetKeyboardLayout(UInt32 idThread);

        [DllImport("user32.dll")]
        private static extern UInt32 GetKeyboardLayoutList(Int32 nBuff, IntPtr[] lpList);

        private const UInt32 KLF_SETFORPROCESS = 0x00000100;

        [DllImport("user32.dll")]
        private static extern UInt32 ActivateKeyboardLayout(IntPtr hkl, UInt32 flags);

        private const uint SPI_GETDEFAULTINPUTLANG = 0x0059;
        private const uint SPI_SETDEFAULTINPUTLANG = 0x005A;
        private const uint SPIF_UPDATEINIFILE = 0x01;
        private const uint SPIF_SENDWININICHANGE = 0x02;
        private const uint DVORAK = 0xF0020409;
        private const uint QWERTY = 0x04090409;

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern bool SystemParametersInfo(uint uiAction, uint uiParam, ref uint pvParam, uint fWinIni);

    }
}
