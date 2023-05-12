using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using KTrie;
using TreeDictionary.Annotations;

namespace TreeDictionary.ViewModels
{
    class MainViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private string _path;
        private Trie<char, string> _trie;

        private string requestText;
        private string currentWord;
        private string currentDescription;
        private string selectedWord;

        public ICommand SelectedWordChanged { get; set; }
        public ICommand DeleteCommand { get; set; }
        public ICommand AddCommand { get; set; }
        public ICommand SaveCommand { get; set; }

        public string RequestText
        {
            get => requestText;
            set
            {
                requestText = value ?? "";
                AvaibleWords.Clear();

                foreach (var entry in _trie.GetByPrefix(requestText).ToArray())
                {
                    AvaibleWords.Add(new string(entry.Key.ToArray()));
                }

                OnPropertyChanged(nameof(RequestText));
            }
        }

        public string CurrentWord
        {
            get => currentWord;
            set
            {
                currentWord = value;
                OnPropertyChanged(nameof(CurrentWord));
            }
        }

        public string CurrentDescription
        {
            get => currentDescription;
            set
            {
                currentDescription = value;
                OnPropertyChanged(nameof(CurrentDescription));
            }
        }

        public string SelectedWord
        {
            get => selectedWord;
            set
            {
                selectedWord = value;
                OnPropertyChanged(nameof(SelectedWord));
            }
        }

        public ObservableCollection<string> AvaibleWords { get; set; }

        public MainViewModel(string path)
        {
            _path = path;

            SelectedWordChanged = new Command((obj) =>
            {
                if (obj != null)
                {
                    var temp = (string) obj;
                    _trie.TryGetValue(temp, out var res);

                    CurrentWord = temp;
                    CurrentDescription = res;
                }
            });

            DeleteCommand = new Command((obj) =>
            {
                if (obj != null && (string)obj != "")
                {
                    var temp = (string)obj;
                    Debug.WriteLine(_trie.Remove(temp));

                    SelectedWord = "";
                    CurrentWord = "";
                    CurrentDescription = "";

                    RequestText = RequestText;
                }
            });

            AddCommand = new Command(() =>
            {
                if (!String.IsNullOrEmpty(CurrentWord))
                {
                    if (_trie.ContainsKey(CurrentWord))
                    {
                        _trie[CurrentWord] = CurrentDescription;
                    }
                    else
                    {
                        _trie.Add(CurrentWord, CurrentDescription);
                        RequestText = RequestText;
                    }
                }
            });

            SaveCommand = new Command(() =>
            {
                File.WriteAllLines(path, 
                    _trie.GetByPrefix("")
                        .ToArray()
                        .Select(x => $"{new string(x.Key.ToArray())}:{x.Value}"));
            });

            if (!File.Exists(path))
            {
                throw new FileLoadException();
            }

            var temp = File.ReadAllLines(path).Select(x => x.Split(':'));

            if (temp.Any(x => x.Length != 2))
            {
                throw new FileFormatException();
            }

            _trie = new Trie<char, string>();

            AvaibleWords = new ObservableCollection<string>();

            requestText = "";
            currentWord = "";
            currentDescription = "";

            foreach (var record in temp)
            {
                _trie.Add(record[0], record[1]);
            }

            foreach (var entry in _trie.GetByPrefix(""))
            {
                AvaibleWords.Add(new string(entry.Key.ToArray()));
            }
        }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
