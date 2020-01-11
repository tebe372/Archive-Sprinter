using AS.Core.Models;
using AS.Core.ViewModels;
using AS.Utilities;
using OxyPlot;
using OxyPlot.Axes;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
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
            //UpdatePlot = new RelayCommand(_updatePlot);
            _signalPlots = new ObservableCollection<SignalPlotPanel>();
            _inspectionAnalysisParams = new InspectionAnalysisParametersViewModel();
        }

        private void _signalCheckStatusChanged(SignalTree e)
        {
            //MessageBox.Show("Draw signals now!");
            _updatePlot(e);
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
                if (value != null && _selectedSignalPlotPanel.Signals.Any())
                {
                    InspectionAnalysisParams.Fs = _selectedSignalPlotPanel.Signals.FirstOrDefault().SamplingRate;
                }
                OnPropertyChanged();
            }
        }
        private InspectionAnalysisParametersViewModel _inspectionAnalysisParams;
        public InspectionAnalysisParametersViewModel InspectionAnalysisParams
        {
            get { return _inspectionAnalysisParams; }
            set
            {
                _inspectionAnalysisParams = value;
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
                        //foreach (var tr in s.SignalTreeContained)
                        //{
                        //    tr.ChangeIsCheckedStatus(false);
                        //    tr.CheckDirectParent();
                        //}
                    }
                }
                SelectedSignalPlotPanel = selection;
                SelectedSignalPlotPanel.IsPlotSelected = true;
                foreach (var s in SelectedSignalPlotPanel.Signals)
                {
                    s.IsChecked = true;
                    //foreach (var tr in s.SignalTreeContained)
                    //{
                    //    tr.ChangeIsCheckedStatus(true);
                    //    tr.CheckDirectParent();
                    //}
                }
                SampleDataMngr.DetermineCheckStatusOfGroupedSignals();
            }
        }
        public ICommand DeleteAPlot { set; get; }
        private void _deleteAPlot(object obj)
        {
            var toBeDeleted = obj as SignalPlotPanel;
            foreach (var s in toBeDeleted.Signals)
            {
                s.IsChecked = false;
                //foreach (var tr in s.SignalTreeContained)
                //{
                //    tr.ChangeIsCheckedStatus(false);
                //    tr.CheckDirectParent();
                //}
            }
            SampleDataMngr.DetermineCheckStatusOfGroupedSignals();
            toBeDeleted.IsPlotSelected = false;
            SelectedSignalPlotPanel = null;
            if (SignalPlots.Contains(toBeDeleted))
            {
                SignalPlots.Remove(toBeDeleted);
            }
        }
        public ICommand AllPlotsDeSelected { set; get; }
        public void DeSelectAllPlots(object obj)
        {
            if (SelectedSignalPlotPanel != null)
            {
                foreach (var s in SelectedSignalPlotPanel.Signals)
                {
                    s.IsChecked = false;
                }
                SelectedSignalPlotPanel.IsPlotSelected = false;
                SelectedSignalPlotPanel = null;
                SampleDataMngr.DetermineCheckStatusOfGroupedSignals();
            }
        }
        private void _updatePlot(SignalTree obj)
        {
            var tree = obj as SignalTree;
            if (SelectedSignalPlotPanel != null)
            {
                var needToUpdate = true;
                if ((bool)tree.IsChecked)
                {
                    //check to see if newly added signal has the same sampling rate as the already drawn ones.
                    if (SelectedSignalPlotPanel.Signals.Count > 0)
                    {
                        needToUpdate = _checkFreq(SelectedSignalPlotPanel.Signals[0].SamplingRate, tree);
                    }
                    if (needToUpdate)
                    {
                        //add signals to plot's signal list
                        _addSignalsToPlot(tree);
                    }
                }
                else
                {
                    //remove signals from plot's signal list
                    _removeSignalsFromPlot(tree);
                }
                if (needToUpdate)
                {
                    _drawSignals();
                    if (SelectedSignalPlotPanel.Signals.Count > 0 && InspectionAnalysisParams.Fs != SelectedSignalPlotPanel.Signals[0].SamplingRate)
                    {
                        InspectionAnalysisParams.Fs = SelectedSignalPlotPanel.Signals[0].SamplingRate;
                    }
                }
                else
                {
                    MessageBox.Show("Selected signal has a different sampling rate than the plotted ones.");
                }
            }
            else
            {
                MessageBox.Show("No plot is selected to add signal.");
                tree.ChangeIsCheckedStatus(false);
                tree.CheckDirectParent();
            }
        }
        private bool _checkFreq(int samplingRate, SignalTree tree)
        {
            if (tree.Signal != null)
            {
                return tree.Signal.SamplingRate == samplingRate;
            }
            else
            {
                if (tree.SignalList != null && tree.SignalList.Count > 0)
                {
                    return _checkFreq(samplingRate, tree.SignalList[0]);
                }
                else
                {
                    return false;
                }
            }
        }
        private void _removeSignalsFromPlot(SignalTree tree)
        {
            if (tree.Signal != null && SelectedSignalPlotPanel.Signals.Contains(tree.Signal))
            {
                SelectedSignalPlotPanel.Signals.Remove(tree.Signal);
            }
            else
            {
                if (tree.SignalList != null && tree.SignalList.Count > 0)
                {
                    foreach (var tr in tree.SignalList)
                    {
                        _removeSignalsFromPlot(tr);
                    }
                }
            }
        }
        private void _addSignalsToPlot(SignalTree tree)
        {
            if (tree.Signal != null)
            {
                SelectedSignalPlotPanel.Signals.Add(tree.Signal);
            }
            else
            {
                foreach (var tr in tree.SignalList)
                {
                    _addSignalsToPlot(tr);
                }
            }
        }
        private void _drawSignals()
        {
            var AsignalPlot = new ViewResolvingPlotModel() { PlotAreaBackground = OxyColors.WhiteSmoke };
            var legends = new ObservableCollection<Legend>();
            OxyPlot.Axes.DateTimeAxis timeXAxis = new OxyPlot.Axes.DateTimeAxis()
            {
                Position = OxyPlot.Axes.AxisPosition.Bottom,
                MinorIntervalType = OxyPlot.Axes.DateTimeIntervalType.Auto,
                MajorGridlineStyle = LineStyle.Dot,
                MinorGridlineStyle = LineStyle.Dot,
                MajorGridlineColor = OxyColor.FromRgb(44, 44, 44),
                TicklineColor = OxyColor.FromRgb(82, 82, 82),
                IsZoomEnabled = true,
                IsPanEnabled = true,
            };
            timeXAxis.AxisChanged += TimeXAxis_AxisChanged;
            AsignalPlot.Axes.Add(timeXAxis);
            OxyPlot.Axes.LinearAxis yAxis = new OxyPlot.Axes.LinearAxis()
            {
                Position = OxyPlot.Axes.AxisPosition.Left,
                Title = _getUnitFromSignals(SelectedSignalPlotPanel.Signals),
                //Unit = SelectedSignalToBeViewed.Unit,
                MajorGridlineStyle = LineStyle.Dot,
                MinorGridlineStyle = LineStyle.Dot,
                MajorGridlineColor = OxyColor.FromRgb(44, 44, 44),
                TicklineColor = OxyColor.FromRgb(82, 82, 82),
                IsZoomEnabled = true,
                IsPanEnabled = true
            };
            AsignalPlot.Axes.Add(yAxis);
            var signalCounter = 0;
            foreach (var signal in SelectedSignalPlotPanel.Signals)
            {
                var newSeries = new OxyPlot.Series.LineSeries() { LineStyle = LineStyle.Solid, StrokeThickness = 2 };
                for (int i = 0; i < signal.Data.Count; i++)
                {
                    newSeries.Points.Add(new DataPoint(signal.TimeStampNumber[i], signal.Data[i]));
                }
                newSeries.Title = signal.SignalName;
                var c = string.Format("#{0:x6}", Color.FromName(Utility.SaturatedColors[signalCounter % 20]).ToArgb());
                newSeries.Color = OxyColor.Parse(c);
                legends.Add(new Legend(signal.SignalName, newSeries.Color));
                AsignalPlot.Series.Add(newSeries);
                signalCounter++;
            }
            AsignalPlot.LegendPlacement = LegendPlacement.Outside;
            AsignalPlot.LegendPosition = LegendPosition.RightMiddle;
            AsignalPlot.LegendPadding = 0.0;
            AsignalPlot.LegendSymbolMargin = 0.0;
            AsignalPlot.LegendMargin = 0;
            AsignalPlot.IsLegendVisible = false;
            foreach (var ax in SelectedSignalPlotPanel.SignalViewPlotModel.Axes)
            {
                if (ax.IsHorizontal())
                {
                    foreach (var nax in AsignalPlot.Axes)
                    {
                        if (nax.IsHorizontal() && (ax.ActualMaximum != nax.ActualMaximum || ax.ActualMinimum != nax.ActualMinimum))
                        {
                            nax.Zoom(ax.ActualMinimum, ax.ActualMaximum);
                            break;
                        }
                    }
                }
                if (ax.IsVertical())
                {
                    foreach (var nax in AsignalPlot.Axes)
                    {
                        if (nax.IsVertical() && (ax.ActualMaximum != nax.ActualMaximum || ax.ActualMinimum != nax.ActualMinimum))
                        {
                            nax.Zoom(ax.ActualMinimum, ax.ActualMaximum);
                            break;
                        }
                    }
                }
            }

            SelectedSignalPlotPanel.SignalViewPlotModel = AsignalPlot;
            SelectedSignalPlotPanel.Legends = legends;
        }
        private string _getUnitFromSignals(ObservableCollection<SignalViewModel> signals)
        {
            var unit = "";
            foreach (var s in signals)
            {
                if (string.IsNullOrEmpty(unit))
                {
                    unit = s.Unit;
                }
                else
                {
                    if (unit != s.Unit)
                    {
                        unit = "Mixed";
                        break;
                    }
                }
            }
            return unit;
        }
        private void TimeXAxis_AxisChanged(object sender, AxisChangedEventArgs e)
        {
            var xAxis = sender as DateTimeAxis;
            foreach (var plot in SignalPlots)
            {
                foreach (var ax in plot.SignalViewPlotModel.Axes)
                {
                    if (ax.IsHorizontal() && (ax.ActualMaximum != xAxis.ActualMaximum || ax.ActualMinimum != xAxis.ActualMinimum))
                    {
                        ax.Zoom(xAxis.ActualMinimum, xAxis.ActualMaximum);
                        plot.SignalViewPlotModel.InvalidatePlot(false);
                        break;
                    }
                }
            }
        }
    }
}
