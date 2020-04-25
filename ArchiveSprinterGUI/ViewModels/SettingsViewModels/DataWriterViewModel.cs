using AS.Config;
using AS.Core.ViewModels;
using AS.Utilities;
using Microsoft.WindowsAPICodePack.Dialogs;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ArchiveSprinterGUI.ViewModels.SettingsViewModels
{
    public class DataWriterViewModel : StepViewModel
    {
        public DataWriterViewModel()
        {
            _model = new DataWriter();
            BrowseSavePath = new RelayCommand(_browseSavePath);
        }

        public DataWriterViewModel(string name) : this()
        {
            this.name = name;
        }

        private DataWriter _model;
        [JsonIgnore]
        public DataWriter Model { get { return _model; } }
        public string Name { get { return _model.Name; } }
        public string SavePath 
        { 
            get { return _model.SavePath; } 
            set { _model.SavePath = value;
                OnPropertyChanged();
            }
        }
        public bool SeparatePMUs 
        { 
            get { return _model.SeparatePMUs; }
            set
            {
                _model.SeparatePMUs = value;
                OnPropertyChanged();
            }
        }
        public string Mnemonic 
        { 
            get { return _model.Mnemonic; }
            set
            {
                _model.Mnemonic = value;
                OnPropertyChanged();
            }
        }
        private string _lastSavePath;
        private string name;

        [JsonIgnore]
        public ICommand BrowseSavePath { get; set; }
        private void _browseSavePath(object obj)
        {
            using (var fbd = new CommonOpenFileDialog())
            {
                if (string.IsNullOrEmpty(_lastSavePath))
                {
                    fbd.InitialDirectory = Environment.CurrentDirectory;
                }
                else
                {
                    fbd.InitialDirectory = _lastSavePath;
                }
                fbd.IsFolderPicker = true;
                fbd.AddToMostRecentlyUsedList = true;
                fbd.AllowNonFileSystemItems = false;
                fbd.DefaultDirectory = Environment.CurrentDirectory;
                fbd.EnsureFileExists = true;
                fbd.EnsurePathExists = true;
                fbd.EnsureReadOnly = false;
                fbd.EnsureValidNames = true;
                fbd.Multiselect = false;
                fbd.ShowPlacesList = true;
                fbd.RestoreDirectory = true;
                fbd.Title = "Please Select Signature Output Directory.";
                //fbd.Filters.Add(new CommonFileDialogFilter("CSV files", "*.csv"));
                //fbd.Filters.Add(new CommonFileDialogFilter("All files", "*.*"));
                CommonFileDialogResult result = fbd.ShowDialog();
                if (result == CommonFileDialogResult.Ok && !string.IsNullOrWhiteSpace(fbd.FileName))
                {
                    SavePath = fbd.FileName;
                    _lastSavePath = fbd.FileName;
                }
            }

        }
    }
}
