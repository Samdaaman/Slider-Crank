using CSCore;
using CSCore.Codecs;
using CSCore.SoundOut;
using CSCore.Streams;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.IO;

namespace conrod
{
    public class MixerService: SetIfDifferentHelper
    {
        private readonly CommandStack _commandStack;
        private bool _deckALoading = false;
        public bool DeckALoading { get => _deckALoading; private set => SetIfDifferent(ref _deckALoading, value); }
        private string _deckAFilename = null;
        public string DeckAFilename { get => _deckAFilename; set
            {
                DeckALoading = true;
                SetIfDifferent(ref _deckAFilename, value);
                IWaveSource waveSource = CodecFactory.Instance.GetCodec(_deckAFilename);
                _deckASoundOut?.Dispose();
                _deckASoundOut = new DirectSoundOut();
                _deckASoundOut.Initialize(waveSource);
                _deckASoundOut.Play();
            } }

        private DirectSoundOut _deckASoundOut = null;
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
            if (command.Location == "load")
            {
                string filename = Directory.GetFiles($"C:\\Users\\Sam\\Documents\\Coding\\Slider-Crank\\cylinder\\library\\")[command.Value];
                Console.WriteLine($"Trying to load from {filename}");
                DeckAFilename = filename;
            }
        }
    }
}
