namespace kowder
{
    using System;
    using YamlDotNet.Serialization;
    using YamlDotNet.Serialization.NamingConventions;
    using System.IO;
    using System.Collections.Generic;

    using scanCodeMap = System.Collections.Generic.Dictionary<int, string>;

    enum ScanCodes
    {
        LeftShift = 42,
        RightShift = 54,
        RightAlt = 312,
    }

    class KeyboardLayout
    {
        public string defines;
        public scanCodeMap scancodes;
        public Dictionary<string, Dictionary<int, string>> modifiers;
    }

    class KeyboardLayouts
    {
        public static string currentLayout = "azerty";
        public static Dictionary<string, KeyboardLayout> scanCodeMaps = new Dictionary<string, KeyboardLayout>();
        public static void Init()
        {
            var deserializer = new DeserializerBuilder()
               .WithNamingConvention(UnderscoredNamingConvention.Instance)  // see height_in_inches in sample yml 
               .Build();

            foreach (string file in Directory.EnumerateFiles("res/scancode", "*.yaml"))
            {
                string contents = File.ReadAllText(file);
                var p = deserializer.Deserialize<KeyboardLayout>(contents);
                scanCodeMaps.Add(p.defines, p);
            }
        }

        public static void AddScancodeMappings(string root, List<string> paths)
        {
            var deserializer = new DeserializerBuilder()
               .WithNamingConvention(UnderscoredNamingConvention.Instance)  // see height_in_inches in sample yml 
               .Build();

            foreach (var path in paths)
            {
                string scancodeMapping = File.ReadAllText($"{root}/scancodes/{path}");
                var p = deserializer.Deserialize<KeyboardLayout>(scancodeMapping);
                scanCodeMaps.Add(p.defines, p);
            }
        }

        /// <summary>
        /// Function that gives the pressed keys's equivalent
        /// in string (space= , 1=1 etc...) according to
        /// the current keyboard's mapping
        /// </summary>
        public static string GetKey(int scanCode)
        {
            var def = "undefined";
            scanCodeMaps[currentLayout].scancodes.TryGetValue(scanCode, out def);

            #region shiftMap
            var shiftPressed = Window.GetKeyPressed((int)ScanCodes.LeftShift, (int)ScanCodes.RightShift);

            if (shiftPressed)
            {
               scanCodeMap shiftMap;
               var exists = scanCodeMaps[currentLayout].modifiers.TryGetValue("shift", out shiftMap);

                
               if (!exists) return def;

               string val = "";
               exists = shiftMap.TryGetValue(scanCode, out val);

               if (exists) return val;
            }
            #endregion shiftMap
            #region rightAltMap
            var rightAltPressed = Window.GetKeyPressed((int)ScanCodes.RightAlt);

            if (rightAltPressed)
            {
               scanCodeMap rightAltMap;
               var exists = scanCodeMaps[currentLayout].modifiers.TryGetValue("rightalt", out rightAltMap);

               if (!exists) return def;
               
               string val = "";
               exists = rightAltMap.TryGetValue(scanCode, out val);

               if(exists) return val;
            }
            #endregion rightAltMap

            return def;
        }
    }
}