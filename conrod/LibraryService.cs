using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace conrod
{
    public class LibraryService: SetIfDifferentHelper
    {
        const string LIBRARY_PATH = "C:\\Users\\Sam\\Documents\\Coding\\Slider-Crank\\cylinder\\library";
        public List<LibraryItem> LibraryItems { get; } = new List<LibraryItem>();
        private int _selectedIndex = -1;
        public int SelectedIndex { get => _selectedIndex; set => SetIfDifferent(ref _selectedIndex, value); }
        public LibraryService()
        {
            SelectedIndex = -1;
            RefreshLibaray();
        }

        private void RefreshLibaray()
        {
            LibraryItems.Clear();
            foreach (string filepath in Directory.GetFiles(LIBRARY_PATH))
            {
                try
                {
                    LibraryItems.Add(new LibraryItem(filepath));
                }
                catch
                {
                    Console.WriteLine($"Error loading library item {filepath}");
                }
            }
            NotifyPropertyChange("LibraryItems");
        }
    }

    public class LibraryItem
    {
        public string Title { get; }
        public string Artist { get; }
        public string Filepath { get; }

        public LibraryItem(string filepath)
        {
            FileInfo fileInfo = new FileInfo(filepath);
            Filepath = fileInfo.FullName;
            string[] filenameParts = fileInfo.Name.Split('.')[0].Split('_');
            Title = filenameParts[0];
            Artist = filenameParts[1];
        }
    }
}
