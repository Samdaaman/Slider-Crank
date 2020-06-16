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
    public class MixerService : SetIfDifferentHelper
    {
        private readonly CommandStack _commandStack;
        public readonly LibraryService libraryService = new LibraryService();
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
                // Thread.Sleep(10000);
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
            if (command.Location == "ss-up")
                if (libraryService.SelectedIndex > 0)
                    libraryService.SelectedIndex--;
            if (command.Location == "ss-down")
                if (libraryService.SelectedIndex < libraryService.LibraryItems.Count - 1)
                    libraryService.SelectedIndex++;
            if (command.Location == "ss-select")
                DeckAFilename = libraryService.LibraryItems[libraryService.SelectedIndex].Filepath;
        } 
    }
}
