using Newtonsoft.Json;
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

    public class CommandStack: INotifyPropertyChanged
    {
        private object _lockObject = new object();
        private List<Command> _stack = new List<Command>();
        public Command[] Stack { get => GetAll(); }
        private AutoResetEvent _newCommandsARE = new AutoResetEvent(false);

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnStackModified()
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Stack"));
        }

        public void PushAll(Command[] commands)
        {
            lock (_lockObject)
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
            lock (_lockObject)
            {
                _stack.Add(command);
                OnStackModified();
                _newCommandsARE.Set();
            }
        }

        public Command[] WaitAndPopAll()
        {
            _newCommandsARE.WaitOne();
            lock (_lockObject)
            {
                Command[] commands = _stack.ToArray();
                _stack.Clear();
                OnStackModified();
                return commands;
            }
        }

        private Command[] GetAll()
        {
            lock (_lockObject)
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
