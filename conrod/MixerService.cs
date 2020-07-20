using System;
using System.Threading;
using System.Threading.Tasks;
using System.IO;
using System.CodeDom;

namespace conrod
{
    public class MixerService : SetIfDifferentHelper
    {
        private readonly CommandStack commandStack;
        public readonly LibraryService libraryService = new LibraryService();
        public Deck DeckA { get; } = new Deck();
        public Deck DeckB { get; } = new Deck();
        public MixerService(CommandStack commandStack)
        {
            this.commandStack = commandStack;
        }

        private Deck GetDeckForCommand(string deckKey)
        {
            if (deckKey == "a")
                return DeckA;
            if (deckKey == "b")
                return DeckB;
            else
                throw new Exception("Deck not found");
        }

        public void ProcessNewCommandsForever()
        {
            Task.Run(() =>
            {
                while (true)
                {
                    // Thread.Sleep(10000);
                    ProcessCommandsFromStack();
                }
            });
        }
        private void ProcessCommandsFromStack()
        {
            Command[] commands = commandStack.WaitAndPopAll();
            foreach (Command command in commands)
                ProcessCommand(command);
        }

        private void ProcessCommand(Command command)
        {
            string[] locationParts = command.Location.Split('-');
            switch (locationParts[0])
            {
                case "ss":
                    switch (locationParts[1])
                    {
                        case "up":
                            if (libraryService.SelectedIndex > 0)
                                libraryService.SelectedIndex--;
                            break;
                        case "down":
                            if (libraryService.SelectedIndex < libraryService.LibraryItems.Count - 1)
                                libraryService.SelectedIndex++;
                            break;
                        default:
                            Console.WriteLine($"Could not process location {command.Location}");
                            break;
                    }
                    break;
                case "d":
                    Deck deck = GetDeckForCommand(locationParts[1]);
                    switch (locationParts[2])
                    {
                        case "sselect":
                            if (libraryService.SelectedIndex != -1)
                                deck.LoadFromFile(libraryService.LibraryItems[libraryService.SelectedIndex].Filepath);
                            break;
                        case "splay":
                            deck.Play();
                            break;
                        case "spause":
                            deck.Pause();
                            break;
                        case "sstop":
                            deck.Stop();
                            break;
                        case "seekrb":
                            deck.RelativeSeekValueChange(command.Value, false);
                            break;
                        case "seekrf":
                            deck.RelativeSeekValueChange(command.Value, true);
                            break;
                        case "volume":
                            deck.VolumeAdjust(command.Value);
                            break;
                        case "treble":
                            deck.TrebleAdjust(command.Value);
                            break;
                        case "mid":
                            deck.MidAdjust(command.Value);
                            break;
                        case "bass":
                            deck.BassAdjust(command.Value);
                            break;
                        case "tempo":
                            deck.TempoAdjust(command.Value);
                            break;
                        default:
                            Console.WriteLine($"Could not process location {command.Location}");
                            break;
                    }
                    break;
                default:
                    Console.WriteLine($"Could not process location {command.Location}");
                    break;
            }
            
               
        } 
    }
}
