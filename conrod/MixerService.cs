using System.Threading;

namespace conrod
{
    public class MixerService: SetIfDifferentHelper
    {
        private readonly CommandStack _commandStack;

        public MixerService(CommandStack commandStack)
        {
            _commandStack = commandStack;
        }

        public void ProcessNewCommandsForever()
        {
            while (true)
            {
                Thread.Sleep(10000);
                ProcessCommandsFromStack();
            }
        }

        private void ProcessCommandsFromStack()
        {
            Command[] commands = _commandStack.WaitAndPopAll();
            foreach (Command command in commands)
                ProcessCommand(command);
        }

        private void ProcessCommand(Command command)
        {

        }
    }
}
