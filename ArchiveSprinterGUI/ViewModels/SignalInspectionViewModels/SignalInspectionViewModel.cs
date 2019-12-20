using AS.Core.ViewModels;
using AS.Utilities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace ArchiveSprinterGUI.ViewModels.SignalInspectionViewModels
{
    public class SignalInspectionViewModel : ViewModelBase
    {
        public SignalInspectionViewModel()
        {
            SampleDataMngr = new SampleDataManagerViewModel();
            SampleDataMngr.SignalCheckStatusChanged += _signalCheckStatusChanged;
            AddPlot = new RelayCommand(_addAPlot);
            PlotSelected = new RelayCommand(_plotSelectedToEdit);
            DeleteAPlot = new RelayCommand(_deleteAPlot);
            AllPlotsDeSelected = new RelayCommand(DeSelectAllPlots);
            _signalPlots = new ObservableCollection<SignalPlotPanel>();
        }

        private void _signalCheckStatusChanged(SignalTree e)
        {
            MessageBox.Show("Draw signals now!");
        }

        public SampleDataManagerViewModel SampleDataMngr { get; set; }
        private ObservableCollection<SignalPlotPanel> _signalPlots;
        public ObservableCollection<SignalPlotPanel> SignalPlots
        {
            set
            {
                _signalPlots = value;
                OnPropertyChanged();
            }
            get { return _signalPlots; }
        }
        private SignalPlotPanel _selectedSignalPlotPanel;
        public SignalPlotPanel SelectedSignalPlotPanel
        {
            get { return _selectedSignalPlotPanel; }
            set
            {
                _selectedSignalPlotPanel = value;
                //figure out the sampling rate of the current plot selected, 
                //if no signals on this plot, do not change sampling rate of the inspection analysis parameter,
                //if there's any signals, reflect it in the inspection analysis parameter
                //if (value != null && _selectedSignalPlotPanel.Signals.Any())
                //{
                //    InspectionAnalysisParams.Fs = _selectedSignalPlotPanel.Signals.FirstOrDefault().SamplingRate;
                //}
                OnPropertyChanged();
            }
        }
        public ICommand AddPlot { get; set; }
        private void _addAPlot(object obj)
        {
            var newPlot = new SignalPlotPanel();
            SignalPlots.Add(newPlot);
            _plotSelectedToEdit(newPlot);
        }
        public ICommand PlotSelected { get; set; }
        private void _plotSelectedToEdit(object obj)
        {
            var selection = obj as SignalPlotPanel;
            if (SelectedSignalPlotPanel != selection)
            {
                if (SelectedSignalPlotPanel != null)
                {
                    SelectedSignalPlotPanel.IsPlotSelected = false;
                    foreach (var s in SelectedSignalPlotPanel.Signals)
                    {
                        s.IsChecked = false;
                    }
                }
                SelectedSignalPlotPanel = selection;
                SelectedSignalPlotPanel.IsPlotSelected = true;
                foreach (var s in SelectedSignalPlotPanel.Signals)
                {
                    s.IsChecked = true;
                }
                //_determineParentGroupedByTypeNodeStatus(GroupedSignalsWithDataByPMU);
                //_determineParentGroupedByTypeNodeStatus(GroupedSignalsWithDataByType);
            }
        }
        public ICommand DeleteAPlot { set; get; }
        private void _deleteAPlot(object obj)
        {
            var toBeDeleted = obj as SignalPlotPanel;
            foreach (var s in toBeDeleted.Signals)
            {
                s.IsChecked = false;
            }
            //_determineParentGroupedByTypeNodeStatus(GroupedSignalsWithDataByPMU);
            //_determineParentGroupedByTypeNodeStatus(GroupedSignalsWithDataByType);
            //toBeDeleted.IsPlotSelected = false;
            //SelectedSignalPlotPanel = null;
            //if (SignalPlots.Contains(toBeDeleted))
            //{
            //    SignalPlots.Remove(toBeDeleted);
            //}
        }
        public ICommand AllPlotsDeSelected { set; get; }
        public void DeSelectAllPlots(object obj)
        {
            //if (SelectedSignalPlotPanel != null)
            //{
            //    foreach (var s in SelectedSignalPlotPanel.Signals)
            //    {
            //        s.IsChecked = false;
            //    }
            //    _determineParentGroupedByTypeNodeStatus(GroupedSignalsWithDataByPMU);
            //    _determineParentGroupedByTypeNodeStatus(GroupedSignalsWithDataByType);
            //    SelectedSignalPlotPanel.IsPlotSelected = false;
            //    SelectedSignalPlotPanel = null;
            //}
        }
    }
}
