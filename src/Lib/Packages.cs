namespace kowder
{
    using System;
    using System.IO;
    using System.Collections.Generic;
    using YamlDotNet.Serialization;
    using YamlDotNet.Serialization.NamingConventions;

    using ExtendingList = System.Collections.Generic.Dictionary<string, dynamic>;
    class Package
    {
        public string name;
        public string author = "";
        public ExtendingList extends;
    }

    class Packages
    {
        public static List<Package> packages;
        public static void Load()
        {
            var deserializer = new DeserializerBuilder()
               .WithNamingConvention(UnderscoredNamingConvention.Instance)  // see height_in_inches in sample yml 
               .Build();


            Output.PrintLine(ConsoleColor.Blue, "Loading packages ...");

            foreach (string dir in Directory.EnumerateDirectories("packages"))
            {
                string contents = File.ReadAllText($"{dir}/package.yaml");
                var p = deserializer.Deserialize<Package>(contents);
                
                if(p.extends != null) 
                {
                    foreach(KeyValuePair<string, dynamic> entry in p.extends) {
                        switch(entry.Key) {
                            case "kowder.scancodes":
                                KeyboardLayouts.AddScancodeMappings(dir, entry.Value);
                                break;
                            case "kowder.themes":
                                break;
                        }
                    }
                }

                Output.PrintLine(ConsoleColor.Green, $"{p.name} by {p.author} successfully loaded!");
            }

            Output.PrintLine(ConsoleColor.Blue, "Packages loaded.");
        }
    }
}