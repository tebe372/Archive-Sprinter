using AS.Config;
using AS.Core.Models;
using AS.IO;
using AS.SampleDataManager;
using AS.Utilities;
using Microsoft.WindowsAPICodePack.Dialogs;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace ArchiveSprinterGUI.ViewModels.SettingsViewModels
{
    public class DataSourceSettingViewModel : ViewModelBase
    {
        public DataSourceSettingViewModel()
        {
            _model = new DataSourceSetting();
            BrowseInputFileDir = new RelayCommand(_browseInputFile);
        }
        private DataSourceSetting _model;
        [JsonIgnore]
        public DataSourceSetting Model
        {
            get { return _model; }
        }
        public DataFileType FileType 
        {
            get { return _model.FileType; }
            set
            {
                _model.FileType = value;
                OnPropertyChanged();
            }
        }
        public string FileDirectory
        {
            get { return _model.FileDirectory; }
            set
            {
                _model.FileDirectory = value;
                OnPropertyChanged();
            }
        }
        public string ExampleFile
        {
            get { return _model.ExampleFile; }
            set
            {
                if (_model.ExampleFile != value)
                {
                    try
                    {
                        _model.ExampleFile = value;
                        OnPropertyChanged();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message, "Error!", MessageBoxButton.OK);
                    }
                    var reader = DataFileReaderFactory.Create(FileType);
                    List<Signal> signals = reader.Read(value);
                    NumberOfDataPointInFile = reader.GetNumberOfDataPointInFile();
                    SamplingRate = reader.GetSamplingRate();
                    if (signals != null && signals.Count() > 0)
                    {
                        SampleDataMngr sdm = SampleDataMngr.Instance;
                        sdm.AddSampleSignals(signals);
                    }
                    //if (!string.IsNullOrEmpty(value))
                    //{
                    //    if (File.Exists(value) && _model.CheckDataFileMatch())
                    //    {
                    //        var filename = "";
                    //        try
                    //        {
                    //            filename = Path.GetFileNameWithoutExtension(value);
                    //        }
                    //        catch (ArgumentException ex)
                    //        {
                    //            MessageBox.Show("Data file path contains one or more of the invalid characters. Original message: " + ex.Message, "Error!", MessageBoxButton.OK);
                    //        }
                    //        if (FileType == DataFileType.PI || FileType == DataFileType.OpenHistorian || FileType == DataFileType.OpenPDC)
                    //        {
                    //            Mnemonic = "";
                    //            //this try block need to stay so the change would show up in the GUI, even though it's duplicating the work in DataConfigModel.cs tryi block on line 268 to 279.
                    //            try
                    //            {
                    //                FileDirectory = Path.GetDirectoryName(value);
                    //                var type = Path.GetExtension(value);
                    //                if (type == ".xml")
                    //                {
                    //                    //PresetList = _model.GetPresets(value);
                    //                }
                    //            }
                    //            catch (Exception ex)
                    //            {
                    //                MessageBox.Show("Error extracting file directory from selected file. Original message: " + ex.Message, "Error!", MessageBoxButton.OK);
                    //            }
                    //        }
                    //        else
                    //        {
                    //            try
                    //            {
                    //                Mnemonic = filename.Substring(0, filename.Length - 16);
                    //            }
                    //            catch (Exception ex)
                    //            {
                    //                MessageBox.Show("Error extracting Mnemonic from selected data file. Original message: " + ex.Message, "Error!", MessageBoxButton.OK);
                    //            }
                    //            //this try block need to stay so the change would show up in the GUI, even though it's duplicating the work in DataConfigModel.cs tryi block on line 268 to 279.
                    //            try
                    //            {
                    //                var fullPath = Path.GetDirectoryName(value);
                    //                var oneLevelUp = fullPath.Substring(0, fullPath.LastIndexOf(@"\"));
                    //                var twoLevelUp = oneLevelUp.Substring(0, oneLevelUp.LastIndexOf(@"\"));
                    //                FileDirectory = twoLevelUp;
                    //            }
                    //            catch (Exception ex)
                    //            {
                    //                MessageBox.Show("Error extracting file directory from selected file. Original message: " + ex.Message, "Error!", MessageBoxButton.OK);
                    //            }
                    //        }
                    //    }
                    //    else
                    //    {
                    //        MessageBox.Show("The example file  " + Path.GetFileName(value) + "  could not be found in the directory  " + Path.GetDirectoryName(value) + ".\n"
                    //                        + "Please go to the 'Data Source' tab, update the location of the example file, and click the 'Read File' button.", "Warning!", MessageBoxButton.OK);
                    //    }
                    //}
                }
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
        [JsonIgnore]
        public ICommand BrowseInputFileDir { get; set; }
        private void _browseInputFile(object obj)
        {
            using (var fbd = new CommonOpenFileDialog())
            {
                fbd.InitialDirectory = FileDirectory;
                fbd.IsFolderPicker = false;
                fbd.AddToMostRecentlyUsedList = true;
                fbd.AllowNonFileSystemItems = false;
                fbd.DefaultDirectory = FileDirectory;
                fbd.EnsureFileExists = true;
                fbd.EnsurePathExists = true;
                fbd.EnsureReadOnly = false;
                fbd.EnsureValidNames = true;
                fbd.Multiselect = false;
                fbd.ShowPlacesList = true;
                fbd.RestoreDirectory = true;
                fbd.Title = "Please Select Input data file.";
                if (FileType == DataFileType.csv)
                {
                    fbd.Filters.Add(new CommonFileDialogFilter("CSV files", "*.csv"));
                }
                if (FileType == DataFileType.pdat)
                {
                    fbd.Filters.Add(new CommonFileDialogFilter("pdat files", "*.pdat"));
                }
                if (FileType == DataFileType.powHQ)
                {
                    fbd.Filters.Add(new CommonFileDialogFilter("HQ Point on Wave", "*.mat"));
                }
                if (FileType == DataFileType.OpenHistorian)
                {
                    fbd.Filters.Add(new CommonFileDialogFilter("openHistorian Preset", "*.xml"));
                }
                if (FileType == DataFileType.PI)
                {
                    fbd.Filters.Add(new CommonFileDialogFilter("PI Reader Preset", "*.xml"));
                }
                if (FileType == DataFileType.OpenPDC)
                {
                    fbd.Filters.Add(new CommonFileDialogFilter("openPDC Preset", "*.xml"));
                }
                fbd.Filters.Add(new CommonFileDialogFilter("All files", "*.*"));
                CommonFileDialogResult result = fbd.ShowDialog();

                if (result == CommonFileDialogResult.Ok && !string.IsNullOrWhiteSpace(fbd.FileName))
                {
                    ExampleFile = fbd.FileName;
                }
            }
        }
        [JsonIgnore]
        public int SamplingRate 
        {
            get { return _model.SamplingRate; }
            set { _model.SamplingRate = value; }
        }
        [JsonIgnore]
        public int NumberOfDataPointInFile
        {
            get { return _model.NumberOfDataPointInFile; }
            set { _model.NumberOfDataPointInFile = value; }
        }
    }
}
