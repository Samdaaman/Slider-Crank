using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;

namespace conrod
{
    public class Command
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
            // Console.WriteLine($"Parsing json {rawJson}");
            if (rawJson == "")
                return new Command[0];
            List<Command> commands = new List<Command>();
            try
            {
                JArray jsonArray = JArray.Parse(rawJson);
                foreach (JObject jsonItem in jsonArray)
                {
                    try
                    {
                        string location = jsonItem.Value<string>("location");
                        int value = jsonItem.Value<int>("value");
                        commands.Add(new Command(location, value));
                    }
                    catch
                    {
                        Console.WriteLine($"Error adding parsing command from array \"{jsonItem}\"");
                    }
                }
            }
            catch
            {
                Console.WriteLine($"Error loading JSON array {rawJson}");
            }
            
            return commands.ToArray();
        }
    }

    public class CommandStack: INotifyPropertyChanged
    {
        private readonly object _lockObjectStack = new object();
        private readonly List<Command> _stack = new List<Command>();
        public Command[] Stack { get => GetAll(); }
        private AutoResetEvent _newCommandsARE = new AutoResetEvent(false);

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnStackModified()
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Stack"));
        }

        public void PushAll(Command[] commands)
        {
            lock (_lockObjectStack)
            {
                foreach (Command command in commands)
                    _stack.Add(command);
                if (commands.Length > 0)
                {
                    OnStackModified();
                    _newCommandsARE.Set();
                }
            }
        }

        public void Push(Command command)
        {
            lock (_lockObjectStack)
            {
                _stack.Add(command);
                OnStackModified();
                _newCommandsARE.Set();
            }
        }

        public Command[] WaitAndPopAll()
        {
            _newCommandsARE.WaitOne();
            lock (_lockObjectStack)
            {
                Command[] commands = _stack.ToArray();
                _stack.Clear();
                OnStackModified();
                return commands;
            }
        }

        private Command[] GetAll()
        {
            lock (_lockObjectStack)
                return _stack.ToArray();
        }

        public bool LoadCommandsToStack(string rawJson)
        {
            Command[] commands = Command.LoadCommands(rawJson);
            PushAll(commands);
            return commands.Length > 0;
        }
    }
}
