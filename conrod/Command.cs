using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace conrod
{
    class Command
    {
        public string Location { get; private set; }
        public int Value { get; private set; }
        public Command(string location, int value)
        {
            Location = location;
            Value = value;
            Console.WriteLine($"Loaded command {location}:{value}");
        }

        public static Command[] LoadCommands(string rawJson)
        {
            Dictionary<string, int> jsonData = JsonConvert.DeserializeObject<Dictionary<string, int>>(rawJson);
            List<Command> commands = new List<Command>();
            foreach (KeyValuePair<string, int> jsonCommand in jsonData)
            {
                try
                {
                    commands.Add(new Command(jsonCommand.Key, jsonCommand.Value));
                }
                catch
                {
                    Console.WriteLine($"Error adding command {jsonCommand.Key}:{jsonCommand.Value}");
                }
            }
            return commands.ToArray();
        }
    }
}
