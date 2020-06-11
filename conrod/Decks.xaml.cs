using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace conrod
{
    /// <summary>
    /// Interaction logic for Decks.xaml
    /// </summary>
    public partial class Decks : Window
    {
        private readonly CommandStack _commandStack;
        private readonly CrankService _crankService;
        private readonly MixerService _mixerService;
        public Decks()
        {
            _commandStack = new CommandStack();
            _crankService = new CrankService(_commandStack);
            _mixerService = new MixerService(_commandStack);
            DataContext = new DecksDataContext(_commandStack, _crankService, _mixerService);
            InitializeComponent();
        }

        private void ButtonInitialise_Click(object sender, RoutedEventArgs e)
        {
            _crankService.Initialise();
            Task.Run(() => _crankService.AcceptNewCommandsForever());
            Task.Run(() => _mixerService.ProcessNewCommandsForever());
        }
    }

    public class DecksDataContext
    {
        public CommandStack CurrentCommandStack;
        public CrankService CurrentCrankService;
        public MixerService CurrentMixerService;
        public DecksDataContext(CommandStack commandStack, CrankService crankService, MixerService mixerService)
        {
            CurrentCommandStack = commandStack;
            CurrentCrankService = crankService;
            CurrentMixerService = mixerService;
        }
    }
}


