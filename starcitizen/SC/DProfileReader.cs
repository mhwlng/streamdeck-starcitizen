﻿using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using BarRaider.SdTools;
using p4ktest;
using p4ktest.SC;
using starcitizen;
using WindowsInput;
using Action = System.Action;

namespace SCJMapper_V2.SC
{
    public class DProfileReader
    {
        public class ActivationMode
        {
            public string Name { get; set; }
            public string OnPress { get; set; }
            public string OnHold { get; set; }
            public string OnRelease { get; set; }
            public string MultiTap { get; set; }
            public string MultiTapBlock { get; set; }
            public string PressTriggerThreshold { get; set; }
            public string ReleaseTriggerThreshold { get; set; }
            public string ReleaseTriggerDelay { get; set; }
            public string Retriggerable { get; set; }

            // --------

            public string Always { get; set; }
            public string NoModifiers { get; set; }
            public string HoldTriggerDelay { get; set; }

        };

        public class Action
        {
            public string MapName { get; set; }
            public string MapUILabel { get; set; }
            public string MapUICategory { get; set; }

            public string Name { get; set; }
            public string UILabel { get; set; }
            public string UIDescription { get; set; }

            public string Keyboard { get; set; }

            public string Mouse { get; set; }
            public string Joystick { get; set; }
            public string Gamepad { get; set; }

            public bool KeyboardOverRule { get; set; }
            public string JoystickOverRule { get; set; }
            public bool MouseOverRule { get; set; }

            public ActivationMode ActivationMode { get; set; }
        };


        public class ActionMap
        {
            public string Name { get; set; }
            public string UILabel { get; set; }
            public string UICategory { get; set; }

            public Dictionary<string, Action> Actions { get; set; } = new Dictionary<string, Action>();
        };

        private Dictionary<string, ActionMap> maps = new Dictionary<string, ActionMap>();
        private Dictionary<string, Action> actions = new Dictionary<string, Action>();
        private Dictionary<string, ActivationMode> activationmodes = new Dictionary<string, ActivationMode>();

        private Dictionary<string, string> joysticks = new Dictionary<string, string>();

        private void ReadAction(XElement action, ActionMap actionMap)
        {
            string name = (string)action.Attribute("name");
            string uiLabel = (string)action.Attribute("UILabel");

            if (string.IsNullOrEmpty(uiLabel))
                return;

            string uiDescription = (string)action.Attribute("UIDescription");
            //if (string.IsNullOrEmpty(uiDescription))
            //  uiDescription = name;

            uiLabel = SCUiText.Instance.Text(uiLabel, uiLabel);
            uiDescription = SCUiText.Instance.Text(uiDescription, "");

            string keyboard = (string)action.Attribute("keyboard");

            string mouse = (string)action.Attribute("mouse");

            string joystick = (string)action.Attribute("joystick");

            string gamepad = (string)action.Attribute("gamepad");

            string activationMode = (string)action.Attribute("ActivationMode");

            ActivationMode currentActivationMode = null;

            if (!string.IsNullOrEmpty(activationMode))
            {
                currentActivationMode = activationmodes[activationMode];

                string onPress = (string)action.Attribute("onPress");
                if (!string.IsNullOrEmpty(onPress))
                {
                    currentActivationMode.OnPress = onPress;
                }

                string onHold = (string)action.Attribute("onHold");
                if (!string.IsNullOrEmpty(onHold))
                {
                    currentActivationMode.OnHold = onHold;
                }

                string onRelease = (string)action.Attribute("onRelease");
                if (!string.IsNullOrEmpty(onRelease))
                {
                    currentActivationMode.OnRelease = onRelease;
                }

                string always = (string)action.Attribute("always");
                if (!string.IsNullOrEmpty(always))
                {
                    currentActivationMode.Always = always;
                }

                string noModifiers = (string)action.Attribute("noModifiers");
                if (!string.IsNullOrEmpty(noModifiers))
                {
                    currentActivationMode.NoModifiers = noModifiers;
                }

                string holdTriggerDelay = (string)action.Attribute("holdTriggerDelay");
                if (!string.IsNullOrEmpty(holdTriggerDelay))
                {
                    currentActivationMode.HoldTriggerDelay = holdTriggerDelay;
                }
            }

            if (string.IsNullOrWhiteSpace(keyboard))
            {
                keyboard = null;
            }
            else
            {
                if (keyboard.StartsWith("HMD_"))
                {
                    keyboard = null;
                }
            }

            var m_currentAction = new Action
            {
                MapName = actionMap.Name,
                MapUICategory = actionMap.UICategory,
                MapUILabel = actionMap.UILabel,

                Name = actionMap.Name + "-" + name,
                UILabel = uiLabel,
                UIDescription = uiDescription,
                ActivationMode = currentActivationMode,
                Keyboard = keyboard,
        
                Mouse  = mouse,
                Joystick = joystick,
                Gamepad = gamepad
            };

            if (!actionMap.Actions.ContainsKey(name))
                actionMap.Actions.Add(name, m_currentAction);
        }

        public void Actions()
        {
            actions = maps
                .SelectMany(x => x.Value.Actions)
                .Where(x => !string.IsNullOrWhiteSpace(x.Value.Keyboard) &&
                            x.Value.Keyboard != "lalt" &&
                            x.Value.Keyboard != "ralt" &&
                            x.Value.Keyboard != "lshift" &&
                            x.Value.Keyboard != "rshift" &&
                            x.Value.Keyboard != "lctrl" &&
                            x.Value.Keyboard != "rctrl"
                            )
                .ToDictionary(x => x.Value.Name, x => x.Value);
        }

        private void ReadActionmap(XElement actionmap)
        {
            string mapName = (string)actionmap.Attribute("name");
            string uiLabel = (string)actionmap.Attribute("UILabel");
            string uiCategory = (string)actionmap.Attribute("UICategory");

            if (!maps.ContainsKey(mapName))
            {
                if (string.IsNullOrEmpty(uiLabel))
                    return;

                if (string.IsNullOrEmpty(uiCategory))
                    uiCategory = mapName;

                uiLabel = SCUiText.Instance.Text(uiLabel, uiLabel);
                uiCategory = SCUiText.Instance.Text(uiCategory, uiCategory);

                var m_currentMap = new ActionMap { Name = mapName, UILabel = uiLabel, UICategory = uiCategory };

                IEnumerable<XElement> actions = actionmap.Elements().Where(x => x.Name == "action");
                foreach (XElement action in actions)
                {
                    ReadAction(action, m_currentMap);
                }

                maps.Add(mapName, m_currentMap);
            }
            else // from actionmaps.xml
            {
                var map = maps[mapName];

                IEnumerable<XElement> actions = actionmap.Elements().Where(x => x.Name == "action");
                foreach (XElement action in actions)
                {
                    string actionName = (string)action.Attribute("name");

                    if (map.Actions.ContainsKey(actionName))
                    {
                        XElement rebind = action.Elements().FirstOrDefault(x => x.Name == "rebind");
                        if (rebind != null)
                        {
                            string input = (string)rebind.Attribute("input");
                            if (input != null && input.StartsWith("kb"))
                            {
                                input = input.Substring(input.IndexOf("_", StringComparison.Ordinal) + 1).Trim();

                                if (!string.IsNullOrEmpty(input))
                                {
                                    map.Actions[actionName].Keyboard = input;
                                    map.Actions[actionName].KeyboardOverRule = true;
                                }

                            } else
                            if (input != null && input.StartsWith("js"))
                            {
                                var instance = input.Substring(2, input.IndexOf("_", StringComparison.Ordinal)-2);

                                input = input.Substring(input.IndexOf("_", StringComparison.Ordinal) + 1).Trim();

                                if (!string.IsNullOrEmpty(input))
                                {
                                    map.Actions[actionName].Joystick = input;

                                    if (joysticks.ContainsKey(instance))
                                    {
                                        instance = joysticks[instance];
                                    }

                                    map.Actions[actionName].JoystickOverRule = instance;
                                }
                            }
                            else
                            if (input != null && input.StartsWith("mo"))
                            {
                                input = input.Substring(input.IndexOf("_", StringComparison.Ordinal) + 1).Trim();

                                if (!string.IsNullOrEmpty(input))
                                {
                                    map.Actions[actionName].Mouse = input;
                                    map.Actions[actionName].MouseOverRule = true;
                                }
                            }

                        }
                    }
                    else
                    {
                        Logger.Instance.LogMessage(TracingLevel.INFO, actionName + "??????????????????" + mapName);

                        // do something ?????????????
                    }

                }
            }

        }

        private void ReadActivationMode(XElement actionmap)
        {
            string name = (string)actionmap.Attribute("name");
            string onPress = (string)actionmap.Attribute("onPress");
            string onHold = (string)actionmap.Attribute("onHold");
            string onRelease = (string)actionmap.Attribute("onRelease");
            string multiTap = (string)actionmap.Attribute("multiTap");
            string multiTapBlock = (string)actionmap.Attribute("multiTapBlock");
            string pressTriggerThreshold = (string)actionmap.Attribute("pressTriggerThreshold");
            string releaseTriggerThreshold = (string)actionmap.Attribute("releaseTriggerThreshold");
            string releaseTriggerDelay = (string)actionmap.Attribute("releaseTriggerDelay");
            string retriggerable = (string)actionmap.Attribute("retriggerable");

            var m_currenActivationMode = new ActivationMode()
            {
                Name = name,
                OnPress = onPress,
                OnHold = onHold,
                OnRelease = onRelease,
                MultiTap = multiTap,
                MultiTapBlock = multiTapBlock,
                PressTriggerThreshold = pressTriggerThreshold,
                ReleaseTriggerThreshold = releaseTriggerThreshold,
                ReleaseTriggerDelay = releaseTriggerDelay,
                Retriggerable = retriggerable
            };

            if (!activationmodes.ContainsKey(name))
                activationmodes.Add(name, m_currenActivationMode);

        }


        public void fromActionProfile(string xml)
        {
            XmlReaderSettings settings = new XmlReaderSettings
            {
                ConformanceLevel = ConformanceLevel.Fragment,
                IgnoreWhitespace = true,
                IgnoreComments = true
            };

            using (XmlReader reader = XmlReader.Create(new StringReader(xml), settings))
            {
                reader.MoveToContent();
                if (XNode.ReadFrom(reader) is XElement el)
                {
                    IEnumerable<XElement> actionProfiles = el.Elements().Where(x => x.Name == "ActionProfiles");
                    foreach (XElement actionProfile in actionProfiles)
                    {
                        string profileName = (string)actionProfile.Attribute("profileName");

                        if (profileName == "default")
                        {
                            fromXML(actionProfile.ToString());
                            break;
                        }
                    }
                }
            }
        }

        public void fromXML(string xml)
        {
            XmlReaderSettings settings = new XmlReaderSettings
            {
                ConformanceLevel = ConformanceLevel.Fragment,
                IgnoreWhitespace = true,
                IgnoreComments = true
            };

            using (XmlReader reader = XmlReader.Create(new StringReader(xml), settings))
            {
                reader.MoveToContent();
                if (XNode.ReadFrom(reader) is XElement el)
                {
                    XElement ams = el.Elements().FirstOrDefault(x => x.Name == "ActivationModes");
                    if (ams != null)
                    {
                        IEnumerable<XElement> activationModes = ams.Elements().Where(x => x.Name == "ActivationMode");
                        foreach (XElement activationMode in activationModes)
                        {
                            ReadActivationMode(activationMode);
                        }
                    }

                    /*
                    //Modifiers.Instance.Clear();
                    IEnumerable<XElement> modifiers = from x in el.Elements()
                                                      where (x.Name == "modifiers")
                                                      select x;
                    foreach (XElement modifier in modifiers)
                    {
                        //ValidContent &= Modifiers.Instance.FromXML(modifier, true);
                    }

                    // only in defaultProfile.xml
                    //OptionTree.InitOptionReader( );
                    
                    IEnumerable<XElement> optiontrees = from x in el.Elements( )
                                                      where ( x.Name == "optiontree" )
                                                      select x;

                    foreach ( XElement optiontree in optiontrees ) {
                     // ValidContent &= OptionTree.fromProfileXML( optiontree );
                    }
                    */

                    IEnumerable<XElement> options = el.Elements().Where(x => x.Name == "options");

                    foreach (XElement option in options)
                    {
                        string type = (string)option.Attribute("type");

                        string instance = (string)option.Attribute("instance");

                        string product = (string)option.Attribute("Product");

                        if (type == "joystick")
                        {
                            joysticks.Add(instance,product);
                        }
                    }

                    IEnumerable<XElement> actionmaps = el.Elements().Where(x => x.Name == "actionmap");

                    foreach (XElement actionmap in actionmaps)
                    {
                        ReadActionmap(actionmap);
                    }

                }
            }
        }

        public Action GetBinding(string key)
        {
            if (string.IsNullOrEmpty(key))
            {
                return null;
            }

            if (actions.ContainsKey(key))
            {
                return actions[key];
            }

            return null;
        }

        public void CreateCsv()
        {
            try
            {
                using (StreamWriter outputFile =
                    new StreamWriter(Path.Combine(TheUser.FileStoreDir, "keybindings.csv")))
                {
                    outputFile.WriteLine("sep=\t");
                    var headerline = "map_UICategory" + "\t" + "map_UILabel" + "\t" + "map_Name" + "\t";

                    headerline += "UILabel" + "\t" + "UIDescription" + "\t" + "Name" + "\t" + "Keyboard" + "\t" + "Overrule" + "\t";

                    headerline += "Name" + "\t" +
                                  "OnPress" + "\t" +
                                  "OnHold" + "\t" +
                                  "OnRelease" + "\t" +
                                  "MultiTap" + "\t" +
                                  "MultiTapBlock" + "\t" +
                                  "PressTriggerThreshold" + "\t" +
                                  "ReleaseTriggerThreshold" + "\t" +
                                  "ReleaseTriggerDelay" + "\t" +
                                  "Retriggerable";


                    outputFile.WriteLine(headerline);

                    foreach (var action in actions.OrderBy(x => x.Value.MapUILabel)
                        .ThenBy(x => x.Value.UILabel))
                    {
                        var csvline = action.Value.MapUICategory + "\t" + action.Value.MapUILabel + "\t" +
                                      action.Value.MapName + "\t";

                        csvline += action.Value.UILabel + "\t" + action.Value.UIDescription + "\t" + action.Value.Name +
                                   "\t" + action.Value.Keyboard + "\t" + (action.Value.KeyboardOverRule ? "YES" : "") + "\t";

                        csvline += action.Value.ActivationMode?.Name + "\t" +
                                   action.Value.ActivationMode?.OnPress + "\t" +
                                   action.Value.ActivationMode?.OnHold + "\t" +
                                   action.Value.ActivationMode?.OnRelease + "\t" +
                                   action.Value.ActivationMode?.MultiTap + "\t" +
                                   action.Value.ActivationMode?.MultiTapBlock + "\t" +
                                   action.Value.ActivationMode?.PressTriggerThreshold + "\t" +
                                   action.Value.ActivationMode?.ReleaseTriggerThreshold + "\t" +
                                   action.Value.ActivationMode?.ReleaseTriggerDelay + "\t" +
                                   action.Value.ActivationMode?.Retriggerable;


                        outputFile.WriteLine(csvline);
                    }
                }

                using (StreamWriter outputFile =
                    new StreamWriter(Path.Combine(TheUser.FileStoreDir, "mousebindings.csv")))
                {
                    outputFile.WriteLine("sep=\t");
                    var headerline = "map_UICategory" + "\t" + "map_UILabel" + "\t" + "map_Name" + "\t";

                    headerline += "UILabel" + "\t" + "UIDescription" + "\t" + "Name" + "\t" + "Mouse" + "\t" + "Overrule" + "\t";

                    outputFile.WriteLine(headerline);

                    var unboundActions = maps
                        .SelectMany(x => x.Value.Actions)
                        .Where(x => !string.IsNullOrWhiteSpace(x.Value.Mouse)
                        )
                        .ToDictionary(x => x.Value.Name, x => x.Value);


                    foreach (var action in unboundActions.OrderBy(x => x.Value.MapUILabel)
                        .ThenBy(x => x.Value.UILabel))
                    {
                        var csvline = action.Value.MapUICategory + "\t" + action.Value.MapUILabel + "\t" +
                                      action.Value.MapName + "\t";

                        csvline += action.Value.UILabel + "\t" + action.Value.UIDescription + "\t" + action.Value.Name +
                                   "\t" + action.Value.Mouse + "\t" + (action.Value.MouseOverRule ? "YES" : "") + "\t";


                        outputFile.WriteLine(csvline);
                    }
                }

                using (StreamWriter outputFile =
                    new StreamWriter(Path.Combine(TheUser.FileStoreDir, "joystickbindings.csv")))
                {
                    outputFile.WriteLine("sep=\t");
                    var headerline = "map_UICategory" + "\t" + "map_UILabel" + "\t" + "map_Name" + "\t";

                    headerline += "UILabel" + "\t" + "UIDescription" + "\t" + "Name" + "\t" + "Joystick" + "\t" + "Overrule" + "\t";

                    outputFile.WriteLine(headerline);

                    var unboundActions = maps
                        .SelectMany(x => x.Value.Actions)
                        .Where(x => !string.IsNullOrWhiteSpace(x.Value.Joystick) 
                        )
                        .ToDictionary(x => x.Value.Name, x => x.Value);


                    foreach (var action in unboundActions.OrderBy(x => x.Value.MapUILabel)
                        .ThenBy(x => x.Value.UILabel))
                    {
                        var csvline = action.Value.MapUICategory + "\t" + action.Value.MapUILabel + "\t" +
                                      action.Value.MapName + "\t";

                        csvline += action.Value.UILabel + "\t" + action.Value.UIDescription + "\t" + action.Value.Name +
                        "\t" + action.Value.Joystick + "\t" + action.Value.JoystickOverRule  + "\t";

                        outputFile.WriteLine(csvline);
                    }
                }
                using (StreamWriter outputFile =
                    new StreamWriter(Path.Combine(TheUser.FileStoreDir, "unboundactions.csv")))
                {
                    outputFile.WriteLine("sep=\t");
                    var headerline = "map_UICategory" + "\t" + "map_UILabel" + "\t" + "map_Name" + "\t";

                    headerline += "UILabel" + "\t" + "UIDescription" + "\t" + "Name" ;

                    outputFile.WriteLine(headerline);

                    var unboundActions = maps
                        .SelectMany(x => x.Value.Actions)
                        .Where(x => string.IsNullOrWhiteSpace(x.Value.Keyboard) &&
                                                        string.IsNullOrWhiteSpace(x.Value.Mouse) &&
                                                        string.IsNullOrWhiteSpace(x.Value.Joystick) //&&
                                                        //string.IsNullOrWhiteSpace(x.Value.Gamepad)
                        )
                        .ToDictionary(x => x.Value.Name, x => x.Value);


                    foreach (var action in unboundActions.OrderBy(x => x.Value.MapUILabel)
                        .ThenBy(x => x.Value.UILabel))
                    {
                        var csvline = action.Value.MapUICategory + "\t" + action.Value.MapUILabel + "\t" +
                                      action.Value.MapName + "\t";

                        csvline += action.Value.UILabel + "\t" + action.Value.UIDescription + "\t" + action.Value.Name ;

                        outputFile.WriteLine(csvline);
                    }
                }

            }

            catch (Exception ex)
            {
                Logger.Instance.LogMessage(TracingLevel.ERROR, $"CreateCsv {ex}");
            }
        }


        public void CreateStaticHtml(string statictemplate)
        {
            try
            {
                var keyboard = KeyboardLayouts.GetThreadKeyboardLayout();

                CultureInfo culture;

                try
                {
                    culture = new CultureInfo(keyboard.KeyboardId);
                }
                catch
                {
                    culture = new CultureInfo("en-US");
                }

                Logger.Instance.LogMessage(TracingLevel.INFO, $"Keyboard Detected, language : {keyboard.LanguageId:X} keyboard : {keyboard.KeyboardId:X} culture : {culture.Name}");

                var dropdownHtml = new StringBuilder();

                var mapsList =
                    actions
                        .Where(x => !string.IsNullOrWhiteSpace(x.Value.Keyboard))
                        .OrderBy(x => x.Value.MapUILabel)
                        .GroupBy(x => x.Value.MapUILabel)
                        .Select(x => x.Key);


                foreach (var map in mapsList)
                {
                    var options = actions
                        .Where(x => x.Value.MapUILabel == map && !string.IsNullOrWhiteSpace(x.Value.Keyboard))
                        .OrderBy(x => x.Value.MapUICategory)
                        .ThenBy(x => x.Value.MapUILabel)
                        .ThenBy(x => x.Value.UILabel);

                    if (options.Any())
                    {
                        var htmlline = $"<optgroup label=\"{map}\">";

                        dropdownHtml.AppendLine(htmlline);

                        foreach (var action in options)
                        {
                            var keyString = CommandTools.ConvertKeyStringToLocale(action.Value.Keyboard, culture.Name);

                            var key = keyString.Replace("Dik","").Replace("}{","+").Replace("}", "").Replace("{", "");

                            htmlline = $"   <option value=\"{action.Value.Name}\">{action.Value.UILabel} [{key}]</option>";

                            dropdownHtml.AppendLine(htmlline);
                        }

                        htmlline = $"</optgroup>";

                        dropdownHtml.AppendLine(htmlline);
                    }

                }

                File.WriteAllText(Path.Combine(@"PropertyInspector\StarCitizen", "Static.html"),
                    statictemplate.Replace("[DROPDOWN]", dropdownHtml.ToString()));
            }
            catch (Exception ex)
            {
                Logger.Instance.LogMessage(TracingLevel.ERROR, $"CreateStaticHtml {ex}");
            }

        }


        public void CreateDialHtml(string dialtemplate)
        {
            try
            {
                var keyboard = KeyboardLayouts.GetThreadKeyboardLayout();

                CultureInfo culture;

                try
                {
                    culture = new CultureInfo(keyboard.KeyboardId);
                }
                catch
                {
                    culture = new CultureInfo("en-US");
                }

                Logger.Instance.LogMessage(TracingLevel.INFO, $"Keyboard Detected, language : {keyboard.LanguageId:X} keyboard : {keyboard.KeyboardId:X} culture : {culture.Name}");

                var dropdownHtml = new StringBuilder();

                var mapsList =
                    actions
                        .Where(x => !string.IsNullOrWhiteSpace(x.Value.Keyboard))
                        .OrderBy(x => x.Value.MapUILabel)
                        .GroupBy(x => x.Value.MapUILabel)
                        .Select(x => x.Key);


                foreach (var map in mapsList)
                {
                    var options = actions
                        .Where(x => x.Value.MapUILabel == map && !string.IsNullOrWhiteSpace(x.Value.Keyboard))
                        .OrderBy(x => x.Value.MapUICategory)
                        .ThenBy(x => x.Value.MapUILabel)
                        .ThenBy(x => x.Value.UILabel);

                    if (options.Any())
                    {
                        var htmlline = $"<optgroup label=\"{map}\">";

                        dropdownHtml.AppendLine(htmlline);

                        foreach (var action in options)
                        {
                            var keyString = CommandTools.ConvertKeyStringToLocale(action.Value.Keyboard, culture.Name);

                            var key = keyString.Replace("Dik", "").Replace("}{", "+").Replace("}", "").Replace("{", "");

                            htmlline = $"   <option value=\"{action.Value.Name}\">{action.Value.UILabel} [{key}]</option>";

                            dropdownHtml.AppendLine(htmlline);
                        }

                        htmlline = $"</optgroup>";

                        dropdownHtml.AppendLine(htmlline);
                    }

                }

                File.WriteAllText(Path.Combine(@"PropertyInspector\StarCitizen", "Dial.html"),
                    dialtemplate.Replace("[DROPDOWN]", dropdownHtml.ToString()));
            }
            catch (Exception ex)
            {
                Logger.Instance.LogMessage(TracingLevel.ERROR, $"CreateDialHtml {ex}");
            }

        }

        public void CreateMacroHtml(string macrotemplate)
        {
            try
            {
                var keyboard = KeyboardLayouts.GetThreadKeyboardLayout();

                CultureInfo culture;

                try
                {
                    culture = new CultureInfo(keyboard.KeyboardId);
                }
                catch
                {
                    culture = new CultureInfo("en-US");
                }

                Logger.Instance.LogMessage(TracingLevel.INFO, $"Keyboard Detected, language : {keyboard.LanguageId:X} keyboard : {keyboard.KeyboardId:X} culture : {culture.Name}");

                var dropdownHtml = new StringBuilder();

                var mapsList =
                    actions
                        .Where(x => !string.IsNullOrWhiteSpace(x.Value.Keyboard))
                        .OrderBy(x => x.Value.MapUILabel)
                        .GroupBy(x => x.Value.MapUILabel)
                        .Select(x => x.Key);


                foreach (var map in mapsList)
                {
                    var options = actions
                        .Where(x => x.Value.MapUILabel == map && !string.IsNullOrWhiteSpace(x.Value.Keyboard))
                        .OrderBy(x => x.Value.MapUICategory)
                        .ThenBy(x => x.Value.MapUILabel)
                        .ThenBy(x => x.Value.UILabel);

                    if (options.Any())
                    {
                        var htmlline = $"<optgroup label=\"{map}\">";

                        dropdownHtml.AppendLine(htmlline);

                        foreach (var action in options)
                        {
                            var keyString = CommandTools.ConvertKeyStringToLocale(action.Value.Keyboard, culture.Name);

                            var key = keyString.Replace("Dik", "").Replace("}{", "+").Replace("}", "").Replace("{", "");

                            htmlline = $"   <option value=\"{action.Value.Name}\">{action.Value.UILabel} [{key}]</option>";

                            dropdownHtml.AppendLine(htmlline);
                        }

                        htmlline = $"</optgroup>";

                        dropdownHtml.AppendLine(htmlline);
                    }

                }

                File.WriteAllText(Path.Combine(@"PropertyInspector\StarCitizen", "Macro.html"),
                    macrotemplate.Replace("[DROPDOWN]", dropdownHtml.ToString()));
            }
            catch (Exception ex)
            {
                Logger.Instance.LogMessage(TracingLevel.ERROR, $"CreateMacroHtml {ex}");
            }

        }

    }
}
