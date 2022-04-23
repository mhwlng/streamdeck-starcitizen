using BarRaider.SdTools;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using WindowsInput.Native;

namespace starcitizen
{
    internal static class CommandTools
    {
        internal const char MACRO_START_CHAR = '{';
        internal const string MACRO_END = "}}";
        internal const string REGEX_MACRO = @"^\{(\{[^\{\}]+\})+\}$";
        internal const string REGEX_SUB_COMMAND = @"(\{[^\{\}]+\})";

        public static string ConvertKeyString(string keyboard)
        {
            var keys = keyboard.Split('+');
            keyboard = "";
            foreach (var key in keys)
            {
                keyboard += "{" + FromSCKeyboardCmd(key) + "}";
            }

            return keyboard;
        }
        
        public static string ConvertKeyStringToLocale(string keyboard, string language)
        {
            var keys = keyboard.Split('+');
            keyboard = "";
            foreach (var key in keys)
            {
                var dikKey = FromSCKeyboardCmd(key);

                var dikKeyOut = dikKey.ToString();

                switch (language)
                {
                    case "en-GB":
                        // http://kbdlayout.info/kbduk/shiftstates+scancodes/base

                        switch (dikKey)
                        {
                            // FIRST ROW
                            case DirectInputKeyCode.DikGrave:

                                dikKeyOut = "Dik`";
                                break;

                            case DirectInputKeyCode.DikMinus:
                                dikKeyOut = "Dik-";
                                break;

                            case DirectInputKeyCode.DikEquals:
                                dikKeyOut = "Dik=";
                                break;

                            // SECOND ROW 

                            case DirectInputKeyCode.DikLbracket:
                                dikKeyOut = "Dik[";
                                break;

                            case DirectInputKeyCode.DikRbracket:
                                dikKeyOut = "Dik]";
                                break;

                            case DirectInputKeyCode.DikBackslash:
                                dikKeyOut = "Dik#";
                                break;

                            // THIRD ROW
                            case DirectInputKeyCode.DikSemicolon:
                                dikKeyOut = "Dik:";
                                break;

                            case DirectInputKeyCode.DikApostrophe:
                                dikKeyOut = "Dik'";
                                break;

                            // FOURTH ROW

                            case DirectInputKeyCode.DikComma:
                                dikKeyOut = "Dik,";
                                break;

                            case DirectInputKeyCode.DikPeriod:
                                dikKeyOut = "Dik.";
                                break;

                            case DirectInputKeyCode.DikSlash:
                                dikKeyOut = "Dik/";
                                break;

                        }
                        break;

                    case "de-CH":

                        // http://kbdlayout.info/kbdsg/shiftstates+scancodes/base

                        switch (dikKey)
                        {
                            // FIRST ROW
                            case DirectInputKeyCode.DikGrave:
                                dikKeyOut = "Dik§";
                                break;

                            case DirectInputKeyCode.DikMinus:
                                dikKeyOut = "Dik'";
                                break;

                            case DirectInputKeyCode.DikEquals:
                                dikKeyOut = "Dik^";
                                break;

                            // SECOND ROW 
                            case DirectInputKeyCode.DikY:
                                dikKeyOut = "DikZ";
                                break;

                            case DirectInputKeyCode.DikLbracket:
                                dikKeyOut = "DikÜ";
                                break;

                            case DirectInputKeyCode.DikRbracket:
                                dikKeyOut = "Dik¨";
                                break;

                            case DirectInputKeyCode.DikBackslash:
                                dikKeyOut = "Dik$";
                                break;

                            // THIRD ROW
                            case DirectInputKeyCode.DikSemicolon:
                                dikKeyOut = "DikÖ";
                                break;

                            case DirectInputKeyCode.DikApostrophe:
                                dikKeyOut = "DikÄ";
                                break;

                            // FOURTH ROW
                            case DirectInputKeyCode.DikZ:
                                dikKeyOut = "DikY";
                                break;

                            case DirectInputKeyCode.DikComma:
                                dikKeyOut = "Dik,";
                                break;

                            case DirectInputKeyCode.DikPeriod:
                                dikKeyOut = "Dik.";
                                break;

                            case DirectInputKeyCode.DikSlash:
                                dikKeyOut = "Dik-";
                                break;

                        }
                        break;


                    case "es-ES":

                        // http://kbdlayout.info/kbdsp/shiftstates+scancodes/base
                        
                        switch (dikKey)
                        {
                            // FIRST ROW
                            case DirectInputKeyCode.DikGrave:
                                dikKeyOut = "Dikº";
                                break;

                            case DirectInputKeyCode.DikMinus:
                                dikKeyOut = "Dik'";
                                break;

                            case DirectInputKeyCode.DikEquals:
                                dikKeyOut = "Dik¡";
                                break;

                            // SECOND ROW 

                            case DirectInputKeyCode.DikLbracket:
                                dikKeyOut = "Dik`";
                                break;

                            case DirectInputKeyCode.DikRbracket:
                                dikKeyOut = "Dik+";
                                break;

                            case DirectInputKeyCode.DikBackslash:
                                dikKeyOut = "Dikç";
                                break;

                            // THIRD ROW
                            case DirectInputKeyCode.DikSemicolon:
                                dikKeyOut = "Dikñ";
                                break;

                            case DirectInputKeyCode.DikApostrophe:
                                dikKeyOut = "Dik´";
                                break;

                            // FOURTH ROW

                            case DirectInputKeyCode.DikComma:
                                dikKeyOut = "Dik,";
                                break;

                            case DirectInputKeyCode.DikPeriod:
                                dikKeyOut = "Dik.";
                                break;

                            case DirectInputKeyCode.DikSlash:
                                dikKeyOut = "Dik-";
                                break;

                        }
                        break;

                    case "da-DK":

                        // http://kbdlayout.info/kbdda/shiftstates+scancodes/base

                        switch (dikKey)
                        {
                            // FIRST ROW
                            case DirectInputKeyCode.DikGrave:
                                dikKeyOut = "Dik½";
                                break;

                            case DirectInputKeyCode.DikMinus:
                                dikKeyOut = "Dik+";
                                break;

                            case DirectInputKeyCode.DikEquals:
                                dikKeyOut = "Dik´";
                                break;


                            // SECOND ROW 
                            case DirectInputKeyCode.DikLbracket:
                                dikKeyOut = "DikÅ";
                                break;

                            case DirectInputKeyCode.DikRbracket:
                                dikKeyOut = "Dik¨";
                                break;

                            case DirectInputKeyCode.DikBackslash:
                                dikKeyOut = "Dik'";
                                break;


                            // THIRD ROW
                            case DirectInputKeyCode.DikSemicolon:
                                dikKeyOut = "DikÆ";
                                break;

                            case DirectInputKeyCode.DikApostrophe:
                                dikKeyOut = "DikØ";
                                break;

                            // FOURTH ROW

                            case DirectInputKeyCode.DikComma:
                                dikKeyOut = "Dik,";
                                break;

                            case DirectInputKeyCode.DikPeriod:
                                dikKeyOut = "Dik.";
                                break;

                            case DirectInputKeyCode.DikSlash:
                                dikKeyOut = "Dik-";
                                break;

                        }
                        break;

                    case "it-IT":

                        // http://kbdlayout.info/kbdit/shiftstates+scancodes/base

                        switch (dikKey)
                        {
                            // FIRST ROW
                            case DirectInputKeyCode.DikGrave:
                                dikKeyOut = "Dik\\";
                                break;

                            case DirectInputKeyCode.DikMinus:
                                dikKeyOut = "Dik'";
                                break;

                            case DirectInputKeyCode.DikEquals:
                                dikKeyOut = "DikÌ";
                                break;

                            // SECOND ROW 
                            case DirectInputKeyCode.DikLbracket:
                                dikKeyOut = "DikÈ";
                                break;

                            case DirectInputKeyCode.DikRbracket:
                                dikKeyOut = "Dik+";
                                break;

                            case DirectInputKeyCode.DikBackslash:
                                dikKeyOut = "DikÙ";
                                break;


                            // THIRD ROW
                            case DirectInputKeyCode.DikSemicolon:
                                dikKeyOut = "DikÒ";
                                break;

                            case DirectInputKeyCode.DikApostrophe:
                                dikKeyOut = "DikÀ";
                                break;


                            // FOURTH ROW

                            case DirectInputKeyCode.DikComma:
                                dikKeyOut = "Dik,";
                                break;

                            case DirectInputKeyCode.DikPeriod:
                                dikKeyOut = "Dik.";
                                break;

                            case DirectInputKeyCode.DikSlash:
                                dikKeyOut = "Dik-";
                                break;

                        }
                        break;

                    case "pt-PT":

                        // http://kbdlayout.info/kbdpo/shiftstates+scancodes/base

                        switch (dikKey)
                        {
                            // FIRST ROW
                            case DirectInputKeyCode.DikGrave:
                                dikKeyOut = "Dik\\";
                                break;

                            case DirectInputKeyCode.DikMinus:
                                dikKeyOut = "Dik'";
                                break;

                            case DirectInputKeyCode.DikEquals:
                                dikKeyOut = "Dik«";
                                break;

                            // SECOND ROW 
                            case DirectInputKeyCode.DikLbracket:
                                dikKeyOut = "Dik+";
                                break;

                            case DirectInputKeyCode.DikRbracket:
                                dikKeyOut = "Dik´";
                                break;

                            case DirectInputKeyCode.DikBackslash:
                                dikKeyOut = "Dik~";
                                break;

                            // THIRD ROW
                            case DirectInputKeyCode.DikSemicolon:
                                dikKeyOut = "DikÇ";
                                break;

                            case DirectInputKeyCode.DikApostrophe:
                                dikKeyOut = "Dikº";
                                break;

                            // FOURTH ROW

                            case DirectInputKeyCode.DikComma:
                                dikKeyOut = "Dik,";
                                break;

                            case DirectInputKeyCode.DikPeriod:
                                dikKeyOut = "Dik.";
                                break;

                            case DirectInputKeyCode.DikSlash:
                                dikKeyOut = "Dik-";
                                break;

                        }
                        break;


                    case "de-DE":
                        // http://kbdlayout.info/kbdgr/shiftstates+scancodes/base

                        switch (dikKey)
                        {
                            // FIRST ROW
                            case DirectInputKeyCode.DikGrave:
                                dikKeyOut = "Dik^";
                                break;

                            case DirectInputKeyCode.DikMinus:
                                dikKeyOut = "Dikß";
                                break;

                            case DirectInputKeyCode.DikEquals:
                                dikKeyOut = "Dik´";
                                break;

                            // SECOND ROW 
                            case DirectInputKeyCode.DikY:
                                dikKeyOut = "DikZ";
                                break;

                            case DirectInputKeyCode.DikLbracket:
                                dikKeyOut = "DikÜ";
                                break;

                            case DirectInputKeyCode.DikRbracket:
                                dikKeyOut = "Dik+";
                                break;

                            case DirectInputKeyCode.DikBackslash:
                                dikKeyOut = "Dik#";
                                break;

                            // THIRD ROW
                            case DirectInputKeyCode.DikSemicolon:
                                dikKeyOut = "DikÖ";
                                break;

                            case DirectInputKeyCode.DikApostrophe:
                                dikKeyOut = "DikÄ";
                                break;

                            // FOURTH ROW
                            case DirectInputKeyCode.DikZ:
                                dikKeyOut = "DikY";
                                break;

                            case DirectInputKeyCode.DikComma:
                                dikKeyOut = "Dik,";
                                break;

                            case DirectInputKeyCode.DikPeriod:
                                dikKeyOut = "Dik.";
                                break;

                            case DirectInputKeyCode.DikSlash:
                                dikKeyOut = "Dik-";
                                break;
                        }

                        break;
                    case "fr-FR":
                        // http://kbdlayout.info/kbdfr/shiftstates+scancodes/base
                        switch (dikKey)
                        {
                            // FIRST ROW
                            case DirectInputKeyCode.DikGrave:
                                dikKeyOut = "Dik²";
                                break;

                            case DirectInputKeyCode.Dik1:
                                dikKeyOut = "Dik&";
                                break;

                            case DirectInputKeyCode.Dik2:
                                dikKeyOut = "DikÉ";
                                break;

                            case DirectInputKeyCode.Dik3:
                                dikKeyOut = "Dik\"";
                                break;

                            case DirectInputKeyCode.Dik4:
                                dikKeyOut = "Dik'";
                                break;

                            case DirectInputKeyCode.Dik5:
                                dikKeyOut = "Dik(";
                                break;

                            case DirectInputKeyCode.Dik6:
                                dikKeyOut = "Dik-";
                                break;

                            case DirectInputKeyCode.Dik7:
                                dikKeyOut = "DikÈ";
                                break;

                            case DirectInputKeyCode.Dik8:
                                dikKeyOut = "Dik_";
                                break;

                            case DirectInputKeyCode.Dik9:
                                dikKeyOut = "DikÇ";
                                break;

                            case DirectInputKeyCode.Dik0:
                                dikKeyOut = "DikÀ";
                                break;

                            case DirectInputKeyCode.DikMinus:
                                dikKeyOut = "Dik)";
                                break;

                            case DirectInputKeyCode.DikEquals:
                                dikKeyOut = "Dik=";
                                break;

                            // SECOND ROW
                            case DirectInputKeyCode.DikQ:
                                dikKeyOut = "DikA";
                                break;

                            case DirectInputKeyCode.DikW:
                                dikKeyOut = "DikZ";
                                break;

                            case DirectInputKeyCode.DikLbracket:
                                dikKeyOut = "Dik^";
                                break;

                            case DirectInputKeyCode.DikRbracket:
                                dikKeyOut = "Dik$";
                                break;

                            case DirectInputKeyCode.DikBackslash:
                                dikKeyOut = "Dik*";
                                break;

                            // THIRD ROW
                            case DirectInputKeyCode.DikA:
                                dikKeyOut = "DikQ";
                                break;

                            case DirectInputKeyCode.DikSemicolon:
                                dikKeyOut = "DikM";
                                break;

                            case DirectInputKeyCode.DikApostrophe:
                                dikKeyOut = "DikÙ";
                                break;

                            // FOURTH ROW
                            case DirectInputKeyCode.DikZ:
                                dikKeyOut = "DikW";
                                break;

                            case DirectInputKeyCode.DikM:
                                dikKeyOut = "Dik,";
                                break;

                            case DirectInputKeyCode.DikComma:
                                dikKeyOut = "Dik;";
                                break;

                            case DirectInputKeyCode.DikPeriod:
                                dikKeyOut = "Dik:";
                                break;

                            case DirectInputKeyCode.DikSlash:
                                dikKeyOut = "Dik!";
                                break;

                        }

                        break;
                    default:

                        switch (dikKey)
                        {
                            // FIRST ROW
                            case DirectInputKeyCode.DikGrave:

                                dikKeyOut = "Dik`";
                                break;

                            case DirectInputKeyCode.DikMinus:
                                dikKeyOut = "Dik-";
                                break;

                            case DirectInputKeyCode.DikEquals:
                                dikKeyOut = "Dik=";
                                break;

                            // SECOND ROW 

                            case DirectInputKeyCode.DikLbracket:
                                dikKeyOut = "Dik[";
                                break;

                            case DirectInputKeyCode.DikRbracket:
                                dikKeyOut = "Dik]";
                                break;

                            case DirectInputKeyCode.DikBackslash:
                                dikKeyOut = "Dik\\";
                                break;

                            // THIRD ROW
                            case DirectInputKeyCode.DikSemicolon:
                                dikKeyOut = "Dik:";
                                break;

                            case DirectInputKeyCode.DikApostrophe:
                                dikKeyOut = "Dik'";
                                break;

                            // FOURTH ROW

                            case DirectInputKeyCode.DikComma:
                                dikKeyOut = "Dik,";
                                break;

                            case DirectInputKeyCode.DikPeriod:
                                dikKeyOut = "Dik.";
                                break;

                            case DirectInputKeyCode.DikSlash:
                                dikKeyOut = "Dik/";
                                break;
                        }

                        break;
                }

                keyboard += "{" + dikKeyOut + "}";
            }

            return keyboard;
        }

        private static DirectInputKeyCode FromSCKeyboardCmd(string scKey)
        {
            switch (scKey)
            {
                // handle modifiers first
                case "lalt": return DirectInputKeyCode.DikLalt;
                case "ralt": return DirectInputKeyCode.DikRalt;
                case "lshift": return DirectInputKeyCode.DikLshift;
                case "rshift": return DirectInputKeyCode.DikRshift;
                case "lctrl": return DirectInputKeyCode.DikLcontrol;
                case "rctrl": return DirectInputKeyCode.DikRcontrol;

                // function keys first 
                case "f1": return DirectInputKeyCode.DikF1;
                case "f2": return DirectInputKeyCode.DikF2;
                case "f3": return DirectInputKeyCode.DikF3;
                case "f4": return DirectInputKeyCode.DikF4;
                case "f5": return DirectInputKeyCode.DikF5;
                case "f6": return DirectInputKeyCode.DikF6;
                case "f7": return DirectInputKeyCode.DikF7;
                case "f8": return DirectInputKeyCode.DikF8;
                case "f9": return DirectInputKeyCode.DikF9;
                case "f10": return DirectInputKeyCode.DikF10;
                case "f11": return DirectInputKeyCode.DikF11;
                case "f12": return DirectInputKeyCode.DikF12;
                case "f13": return DirectInputKeyCode.DikF13;
                case "f14": return DirectInputKeyCode.DikF14;
                case "f15": return DirectInputKeyCode.DikF15;

                // all keys where the DX name does not match the SC name
                // Numpad
                case "numlock": return DirectInputKeyCode.DikNumlock;

                case "np_divide": return DirectInputKeyCode.DikDivide;
                case "np_multiply": return DirectInputKeyCode.DikMultiply;
                case "np_subtract": return DirectInputKeyCode.DikSubtract;
                case "np_add": return DirectInputKeyCode.DikAdd;
                case "np_period": return DirectInputKeyCode.DikDecimal;
                case "np_enter": return DirectInputKeyCode.DikNumpadenter;
                case "np_0": return DirectInputKeyCode.DikNumpad0;
                case "np_1": return DirectInputKeyCode.DikNumpad1;
                case "np_2": return DirectInputKeyCode.DikNumpad2;
                case "np_3": return DirectInputKeyCode.DikNumpad3;
                case "np_4": return DirectInputKeyCode.DikNumpad4;
                case "np_5": return DirectInputKeyCode.DikNumpad5;
                case "np_6": return DirectInputKeyCode.DikNumpad6;
                case "np_7": return DirectInputKeyCode.DikNumpad7;
                case "np_8": return DirectInputKeyCode.DikNumpad8;
                case "np_9": return DirectInputKeyCode.DikNumpad9;
                // Digits
                case "0": return DirectInputKeyCode.Dik0;
                case "1": return DirectInputKeyCode.Dik1;
                case "2": return DirectInputKeyCode.Dik2;
                case "3": return DirectInputKeyCode.Dik3;
                case "4": return DirectInputKeyCode.Dik4;
                case "5": return DirectInputKeyCode.Dik5;
                case "6": return DirectInputKeyCode.Dik6;
                case "7": return DirectInputKeyCode.Dik7;
                case "8": return DirectInputKeyCode.Dik8;
                case "9": return DirectInputKeyCode.Dik9;
                // navigation
                case "insert": return DirectInputKeyCode.DikInsert;
                case "home": return DirectInputKeyCode.DikHome;
                case "delete": return DirectInputKeyCode.DikDelete;
                case "end": return DirectInputKeyCode.DikEnd;
                case "pgup": return DirectInputKeyCode.DikPageUp;
                case "pgdown": return DirectInputKeyCode.DikPageDown;
                case "pgdn": return DirectInputKeyCode.DikPageDown;
                case "print": return DirectInputKeyCode.DikPrintscreen;
                case "scrolllock": return DirectInputKeyCode.DikScroll;
                case "pause": return DirectInputKeyCode.DikPause;
                // Arrows
                case "up": return DirectInputKeyCode.DikUp;
                case "down": return DirectInputKeyCode.DikDown;
                case "left": return DirectInputKeyCode.DikLeft;
                case "right": return DirectInputKeyCode.DikRight;
                // non letters
                case "escape": return DirectInputKeyCode.DikEscape;
                case "minus": return DirectInputKeyCode.DikMinus;
                case "equals": return DirectInputKeyCode.DikEquals;
                case "grave": return DirectInputKeyCode.DikGrave;
                case "underline": return DirectInputKeyCode.DikUnderline;
                case "backspace": return DirectInputKeyCode.DikBackspace;
                case "tab": return DirectInputKeyCode.DikTab;
                case "lbracket": return DirectInputKeyCode.DikLbracket;
                case "rbracket": return DirectInputKeyCode.DikRbracket;
                case "enter": return DirectInputKeyCode.DikReturn;
                case "capslock": return DirectInputKeyCode.DikCapital;
                case "colon": return DirectInputKeyCode.DikColon;
                case "backslash": return DirectInputKeyCode.DikBackslash;
                case "comma": return DirectInputKeyCode.DikComma;
                case "period": return DirectInputKeyCode.DikPeriod;
                case "slash": return DirectInputKeyCode.DikSlash;
                case "space": return DirectInputKeyCode.DikSpace;
                case "semicolon": return DirectInputKeyCode.DikSemicolon;
                case "apostrophe": return DirectInputKeyCode.DikApostrophe;

                // all where the lowercase DX name matches the SC name
                default:
                    var letter = "Dik" + scKey.ToUpperInvariant();
                    if (Enum.TryParse(letter, out DirectInputKeyCode dxKey))
                    {
                        return dxKey;
                    }
                    else
                    {
                        return DirectInputKeyCode.DikEscape;
                    }
            }

        }


        internal static string ExtractMacro(string text, int position)
        {
            try
            {
                var endPosition = text.IndexOf(MACRO_END, position);

                // Found an end, let's verify it's actually a macro
                if (endPosition > position)
                {
                    // Use Regex to verify it's really a macro
                    var match = Regex.Match(text.Substring(position, endPosition - position + MACRO_END.Length), REGEX_MACRO);
                    if (match.Length > 0)
                    {
                        return match.Value;
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Instance.LogMessage(TracingLevel.FATAL, $"ExtractMacro Exception: {ex}");
            }

            return null;
        }

        internal static List<DirectInputKeyCode> ExtractKeyStrokes(string macroText)
        {
            var keyStrokes = new List<DirectInputKeyCode>();

            try
            {
                var matches = Regex.Matches(macroText, REGEX_SUB_COMMAND);
                foreach (var match in matches)
                {
                    var matchText = match.ToString().ToUpperInvariant().Replace("{", "").Replace("}", "");

                    var stroke = (DirectInputKeyCode)Enum.Parse(typeof(DirectInputKeyCode), matchText, true);

                    keyStrokes.Add(stroke);
                }
            }
            catch (Exception ex)
            {
                Logger.Instance.LogMessage(TracingLevel.FATAL, $"ExtractKeyStrokes Exception: {ex}");
            }

            return keyStrokes;
        }
    }
}
