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
        public CommandStack CurrentCommandStack { get; } = new CommandStack();
        public CrankService CurrentCrankService { get; }
        public MixerService CurrentMixerService { get; }
        public LibraryService CurrentLibraryService { get; }

        public Decks()
        {
            InitializeComponent();
            CurrentCrankService = new CrankService(CurrentCommandStack);
            CurrentMixerService = new MixerService(CurrentCommandStack);
            CurrentLibraryService = CurrentMixerService.libraryService;
            DataContext = this;

            CurrentCrankService.Initialise();
            CurrentCrankService.AcceptNewCommandsForever();
            CurrentMixerService.ProcessNewCommandsForever();
        }

        private void ListViewSongSelect_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ListViewSongSelect.ScrollIntoView(ListViewSongSelect.SelectedItem);
        }
    }
}


