﻿using AS.Core.ViewModels;
using AS.SampleDataManager;
using AS.Utilities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ArchiveSprinterGUI.ViewModels
{
    public class SampleDataManagerViewModel : ViewModelBase
    {
        private SampleDataMngr _model;
        public SampleDataMngr Model
        {
            get { return _model; }
        }
        //private static SampleDataManagerViewModel _instance = null;
        //public static SampleDataManagerViewModel Instance
        //{
        //    get
        //    {
        //        if (_instance == null)
        //        {
        //            _instance = new SampleDataManagerViewModel();
        //        }
        //        return _instance;
        //    }
        //}
        public SampleDataManagerViewModel()
        {
            _model = SampleDataMngr.Instance;
            _model.SignalsUpdated += SampleSignalsUpdated;
            DataviewGroupMethods = new List<string>() { "Input Signals by Type", "Input Signals by PMU" };
            SelectedDataViewingGroupMethod = "Input Signals by Type";
            _signalsVM = new ObservableCollection<SignalViewModel>();
        }

        private ObservableCollection<SignalViewModel> _signalsVM;
        public ObservableCollection<SignalViewModel> SignalsVM
        {
            get { return _signalsVM; }
            set
            {
                _signalsVM = value;
            }
        }

        public void SampleSignalsUpdated(object sender, EventArgs e)
        {
            _signalsVM.Clear();
            foreach (AS.Core.Models.Signal item in _model.Signals)
            {
              //  SignalView newitem = new SignalViewModel(item);
                _signalsVM.Add(new SignalViewModel(item));
            }
            GroupedRawSignalsByType = SortSignalsByType(_signalsVM);
            GroupedRawSignalsByPMU = SortSignalByPMU(_signalsVM);
        }

        private void _signalCheckStatusChanged(SignalTree e)
        {
            OnSignalTreeCheckStatusChanged(e);
        }
        public event SignalTreeCheckStatusChangedEventHandler SignalCheckStatusChanged;
        public delegate void SignalTreeCheckStatusChangedEventHandler(SignalTree e);
        private void OnSignalTreeCheckStatusChanged(SignalTree e)
        {
            SignalCheckStatusChanged?.Invoke(e);
        }
        public List<string> DataviewGroupMethods { get; set; }
        private string _selectedDataViewingGroupMethod;
        public string SelectedDataViewingGroupMethod 
        {
            get { return _selectedDataViewingGroupMethod; }
            set
            {
                _selectedDataViewingGroupMethod = value;
                OnPropertyChanged();
            }
        }
        #region Raw signals
        private ObservableCollection<SignalTree> _groupedRawSignalsByType;
        public ObservableCollection<SignalTree> GroupedRawSignalsByType
        {
            get
            {
                return _groupedRawSignalsByType;
            }
            set
            {
                _groupedRawSignalsByType = value;
                OnPropertyChanged();
            }
        }
        private ObservableCollection<SignalTree> _groupedRawSignalsByPMU;
        public ObservableCollection<SignalTree> GroupedRawSignalsByPMU
        {
            get
            {
                return _groupedRawSignalsByPMU;
            }
            set
            {
                _groupedRawSignalsByPMU = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region pre process related
        private ObservableCollection<SignalTree> _groupedSignalByPreProcessStepsInput = new ObservableCollection<SignalTree>();
        public ObservableCollection<SignalTree> GroupedSignalByPreProcessStepsInput
        {
            get
            {
                return _groupedSignalByPreProcessStepsInput;
            }
            set
            {
                _groupedSignalByPreProcessStepsInput = value;
                OnPropertyChanged();
            }
        }
        private ObservableCollection<SignalTree> _groupedSignalByPreProcessStepsOutput = new ObservableCollection<SignalTree>();
        public ObservableCollection<SignalTree> GroupedSignalByPreProcessStepsOutput
        {
            get
            {
                return _groupedSignalByPreProcessStepsOutput;
            }
            set
            {
                _groupedSignalByPreProcessStepsOutput = value;
                OnPropertyChanged();
            }
        }
        private ObservableCollection<SignalTree> _allPreProcessOutputGroupedByType = new ObservableCollection<SignalTree>();
        public ObservableCollection<SignalTree> AllPreProcessOutputGroupedByType
        {
            get
            {
                return _allPreProcessOutputGroupedByType;
            }
            set
            {
                _allPreProcessOutputGroupedByType = value;
                OnPropertyChanged();
            }
        }
        private ObservableCollection<SignalTree> _allPreProcessOutputGroupedByPMU = new ObservableCollection<SignalTree>();
        public ObservableCollection<SignalTree> AllPreProcessOutputGroupedByPMU
        {
            get
            {
                return _allPreProcessOutputGroupedByPMU;
            }
            set
            {
                _allPreProcessOutputGroupedByPMU = value;
                OnPropertyChanged();
            }
        }
        #endregion

        private ObservableCollection<SignalTree> _groupedSignalBySignatureStepsInput = new ObservableCollection<SignalTree>();
        public ObservableCollection<SignalTree> GroupedSignalBySignatureStepsInput
        {
            get
            {
                return _groupedSignalBySignatureStepsInput;
            }
            set
            {
                _groupedSignalBySignatureStepsInput = value;
                OnPropertyChanged();
            }
        }
        private ObservableCollection<SignalTree> _groupedSignalByDataWriterStepsInput = new ObservableCollection<SignalTree>();
        public ObservableCollection<SignalTree> GroupedSignalByDataWriterStepsInput
        {
            get
            {
                return _groupedSignalByDataWriterStepsInput;
            }
            set
            {
                _groupedSignalByDataWriterStepsInput = value;
                OnPropertyChanged();
            }
        }

        public void DetermineCheckStatusOfGroupedSignals()
        {
            if (GroupedRawSignalsByPMU != null)
            {
                foreach (var item in GroupedRawSignalsByPMU)
                {
                    item.DetermineIsCheckedStatusFromTopToBottom();
                }
            }
            if (GroupedRawSignalsByType != null)
            {
                foreach (var item in GroupedRawSignalsByType)
                {
                    item.DetermineIsCheckedStatusFromTopToBottom();
                }
            }
            if (GroupedSignalByPreProcessStepsInput != null)
            {
                foreach (var item in GroupedSignalByPreProcessStepsInput)
                {
                    item.DetermineIsCheckedStatusFromTopToBottom();
                }
            }
            if (GroupedSignalByPreProcessStepsOutput != null)
            {
                foreach (var item in GroupedSignalByPreProcessStepsOutput)
                {
                    item.DetermineIsCheckedStatusFromTopToBottom();
                }
            }
            if (AllPreProcessOutputGroupedByType != null)
            {
                foreach (var item in AllPreProcessOutputGroupedByType)
                {
                    item.DetermineIsCheckedStatusFromTopToBottom();
                }
            }
            if (AllPreProcessOutputGroupedByPMU != null)
            {
                foreach (var item in AllPreProcessOutputGroupedByPMU)
                {
                    item.DetermineIsCheckedStatusFromTopToBottom();
                }
            }
            if (GroupedSignalBySignatureStepsInput != null)
            {
                foreach (var item in GroupedSignalBySignatureStepsInput)
                {
                    item.DetermineIsCheckedStatusFromTopToBottom();
                }
            }
            if (GroupedSignalByDataWriterStepsInput != null)
            {
                foreach (var item in GroupedSignalByDataWriterStepsInput)
                {
                    item.DetermineIsCheckedStatusFromTopToBottom();
                }
            }
        }
        public ObservableCollection<SignalTree> SortSignalsByType(ObservableCollection<SignalViewModel> signalsVM)
        {
            ObservableCollection<SignalTree> signalTypeTreeGroupedBySamplingRate = new ObservableCollection<SignalTree>();
            if (signalsVM != null && signalsVM.Count > 0)
            {
                var signalTypeGroupBySamplingRate = signalsVM.GroupBy(x => x.SamplingRate);
                foreach (var rateGroup in signalTypeGroupBySamplingRate)
                {
                    var rate = rateGroup.Key;
                    var newTree = new SignalTree("Sampling Rate: " + rate.ToString() + "/Second");
                    newTree.SignalCheckStatusChanged += _signalCheckStatusChanged;
                    var subSignalGroup = rateGroup.ToList();
                    ObservableCollection<SignalTree> signalTypeTree = new ObservableCollection<SignalTree>();
                    var signalTypeDictionary = subSignalGroup.GroupBy(x => x.TypeAbbreviation.ToArray()[0].ToString())/*.OrderBy(x=>x.Key)*/.ToDictionary(x => x.Key, x => new ObservableCollection<SignalViewModel>(x.ToList()));
                    foreach (var signalType in signalTypeDictionary)
                    {
                        switch (signalType.Key)
                        {
                            case "S":
                                {
                                    var groups = signalType.Value.GroupBy(x => x.TypeAbbreviation);
                                    foreach (var group in groups)
                                    {
                                        switch (group.Key)
                                        {
                                            case "S":
                                                {
                                                    var newHierachy = new SignalTree("Apparent");
                                                    newHierachy.SignalCheckStatusChanged += _signalCheckStatusChanged;
                                                    newHierachy.Parent = newTree;
                                                    //newHierachy.Signal.TypeAbbreviation = "S";
                                                    foreach (var signal in group)
                                                    {
                                                        var s = new SignalTree(signal);
                                                        s.SignalCheckStatusChanged += _signalCheckStatusChanged;
                                                        s.Parent = newHierachy;
                                                        newHierachy.SignalList.Add(s);
                                                    }
                                                    signalTypeTree.Add(newHierachy);
                                                    break;
                                                }

                                            case "SC":
                                                {
                                                    var newHierachy = new SignalTree("Scalar");
                                                    newHierachy.SignalCheckStatusChanged += _signalCheckStatusChanged;
                                                    newHierachy.Parent = newTree;
                                                    //newHierachy.Signal.TypeAbbreviation = "SC";
                                                    foreach (var signal in group)
                                                    {
                                                        var s = new SignalTree(signal);
                                                        s.SignalCheckStatusChanged += _signalCheckStatusChanged;
                                                        s.Parent = newHierachy;
                                                        newHierachy.SignalList.Add(s);
                                                    }
                                                    signalTypeTree.Add(newHierachy);
                                                    break;
                                                }

                                            default:
                                                {
                                                    throw new Exception("Unknown signal type: " + group.Key + "found!");
                                                }
                                        }
                                    }
                                    break;
                                }

                            case "O":
                                {
                                    var newHierachy = new SignalTree("Other");
                                    newHierachy.SignalCheckStatusChanged += _signalCheckStatusChanged;
                                    newHierachy.Parent = newTree;
                                    //newHierachy.Signal.TypeAbbreviation = "OTHER";
                                    foreach (var signal in signalType.Value)
                                    {
                                        var s = new SignalTree(signal);
                                        s.SignalCheckStatusChanged += _signalCheckStatusChanged;
                                        s.Parent = newHierachy;
                                        newHierachy.SignalList.Add(s);
                                    }
                                    signalTypeTree.Add(newHierachy);
                                    break;
                                }

                            case "C":
                                {
                                    var groups = signalType.Value.GroupBy(x => x.TypeAbbreviation);
                                    foreach (var group in groups)
                                    {
                                        switch (group.Key)
                                        {
                                            case "C":
                                                {
                                                    var newHierachy = new SignalTree("CustomizedSignal");
                                                    newHierachy.SignalCheckStatusChanged += _signalCheckStatusChanged;
                                                    newHierachy.Parent = newTree;
                                                    //newHierachy.Signal.TypeAbbreviation = "C";
                                                    foreach (var signal in group)
                                                    {
                                                        var s = new SignalTree(signal);
                                                        s.SignalCheckStatusChanged += _signalCheckStatusChanged;
                                                        s.Parent = newHierachy;
                                                        newHierachy.SignalList.Add(s);
                                                    }
                                                    signalTypeTree.Add(newHierachy);
                                                    break;
                                                }

                                            case "CP":
                                                {
                                                    var newHierachy = new SignalTree("Complex");
                                                    newHierachy.SignalCheckStatusChanged += _signalCheckStatusChanged;
                                                    newHierachy.Parent = newTree;
                                                    //newHierachy.Signal.TypeAbbreviation = "CP";
                                                    foreach (var signal in group)
                                                    {
                                                        var s = new SignalTree(signal);
                                                        s.SignalCheckStatusChanged += _signalCheckStatusChanged;
                                                        s.Parent = newHierachy;
                                                        newHierachy.SignalList.Add(s);
                                                    }
                                                    signalTypeTree.Add(newHierachy);
                                                    break;
                                                }

                                            default:
                                                {
                                                    throw new Exception("Unknown signal type: " + group.Key + "found!");
                                                }
                                        }
                                    }
                                    break;
                                }

                            case "D":
                                {
                                    var newHierachy = new SignalTree("Digital");
                                    newHierachy.SignalCheckStatusChanged += _signalCheckStatusChanged;
                                    newHierachy.Parent = newTree;
                                    //newHierachy.Signal.TypeAbbreviation = "D";
                                    foreach (var signal in signalType.Value)
                                    {
                                        var s = new SignalTree(signal);
                                        s.SignalCheckStatusChanged += _signalCheckStatusChanged;
                                        s.Parent = newHierachy;
                                        newHierachy.SignalList.Add(s);
                                    }
                                    signalTypeTree.Add(newHierachy);
                                    break;
                                }

                            case "F":
                                {
                                    var newHierachy = new SignalTree("Frequency");
                                    newHierachy.SignalCheckStatusChanged += _signalCheckStatusChanged;
                                    newHierachy.Parent = newTree;
                                    //newHierachy.Signal.TypeAbbreviation = "F";
                                    foreach (var signal in signalType.Value)
                                    {
                                        var s = new SignalTree(signal);
                                        s.SignalCheckStatusChanged += _signalCheckStatusChanged;
                                        s.Parent = newHierachy;
                                        newHierachy.SignalList.Add(s);
                                    }
                                    signalTypeTree.Add(newHierachy);
                                    break;
                                }

                            case "R":
                                {
                                    var newHierachy = new SignalTree("Rate of Change of Frequency");
                                    newHierachy.SignalCheckStatusChanged += _signalCheckStatusChanged;
                                    newHierachy.Parent = newTree;
                                    //newHierachy.Signal.TypeAbbreviation = "R";
                                    foreach (var signal in signalType.Value)
                                    {
                                        var s = new SignalTree(signal);
                                        s.SignalCheckStatusChanged += _signalCheckStatusChanged;
                                        s.Parent = newHierachy;
                                        newHierachy.SignalList.Add(s);
                                    }
                                    signalTypeTree.Add(newHierachy);
                                    break;
                                }

                            case "Q":
                                {
                                    var newHierachy = new SignalTree("Reactive Power");
                                    newHierachy.SignalCheckStatusChanged += _signalCheckStatusChanged;
                                    newHierachy.Parent = newTree;
                                    //newHierachy.Signal.TypeAbbreviation = "Q";
                                    foreach (var signal in signalType.Value)
                                    {
                                        var s = new SignalTree(signal);
                                        s.SignalCheckStatusChanged += _signalCheckStatusChanged;
                                        s.Parent = newHierachy;
                                        newHierachy.SignalList.Add(s);
                                    }
                                    signalTypeTree.Add(newHierachy);
                                    break;
                                }

                            case "P":
                                {
                                    var newHierachy = new SignalTree("Active Power");
                                    newHierachy.SignalCheckStatusChanged += _signalCheckStatusChanged;
                                    newHierachy.Parent = newTree;
                                    //newHierachy.Signal.TypeAbbreviation = "P";
                                    foreach (var signal in signalType.Value)
                                    {
                                        var s = new SignalTree(signal);
                                        s.SignalCheckStatusChanged += _signalCheckStatusChanged;
                                        s.Parent = newHierachy;
                                        newHierachy.SignalList.Add(s);
                                    }
                                    signalTypeTree.Add(newHierachy);
                                    break;
                                }

                            case "V":
                                {
                                    var newHierachy = new SignalTree("Voltage");
                                    newHierachy.SignalCheckStatusChanged += _signalCheckStatusChanged;
                                    newHierachy.Parent = newTree;
                                    //newHierachy.Signal.TypeAbbreviation = "V";
                                    var voltageHierachy = signalType.Value.GroupBy(y => y.TypeAbbreviation.ToArray()[1].ToString());
                                    foreach (var group in voltageHierachy)
                                    {
                                        switch (group.Key)
                                        {
                                            case "M":
                                                {
                                                    var mGroup = new SignalTree("Magnitude");
                                                    mGroup.SignalCheckStatusChanged += _signalCheckStatusChanged;
                                                    mGroup.Parent = newHierachy;
                                                    //mGroup.Signal.TypeAbbreviation = "VM";
                                                    var mGroupHierachky = group.GroupBy(z => z.TypeAbbreviation.ToArray()[2].ToString());
                                                    foreach (var phase in mGroupHierachky)
                                                    {
                                                        switch (phase.Key)
                                                        {
                                                            case "P":
                                                                {
                                                                    var positiveGroup = new SignalTree("Positive Sequence");
                                                                    positiveGroup.SignalCheckStatusChanged += _signalCheckStatusChanged;
                                                                    positiveGroup.Parent = mGroup;
                                                                    //positiveGroup.Signal.TypeAbbreviation = "VMP";
                                                                    foreach (var signal in phase)
                                                                    {
                                                                        var s = new SignalTree(signal);
                                                                        s.SignalCheckStatusChanged += _signalCheckStatusChanged;
                                                                        s.Parent = positiveGroup;
                                                                        positiveGroup.SignalList.Add(s);
                                                                    }
                                                                    mGroup.SignalList.Add(positiveGroup);
                                                                    break;
                                                                }

                                                            case "A":
                                                                {
                                                                    var AGroup = new SignalTree("Phase A");
                                                                    AGroup.SignalCheckStatusChanged += _signalCheckStatusChanged;
                                                                    AGroup.Parent = mGroup;
                                                                    //AGroup.Signal.TypeAbbreviation = "VMA";
                                                                    foreach (var signal in phase)
                                                                    {
                                                                        var s = new SignalTree(signal);
                                                                        s.SignalCheckStatusChanged += _signalCheckStatusChanged;
                                                                        s.Parent = AGroup;
                                                                        AGroup.SignalList.Add(s);
                                                                    }
                                                                    mGroup.SignalList.Add(AGroup);
                                                                    break;
                                                                }

                                                            case "B":
                                                                {
                                                                    var BGroup = new SignalTree("Phase B");
                                                                    BGroup.SignalCheckStatusChanged += _signalCheckStatusChanged;
                                                                    BGroup.Parent = mGroup;
                                                                    //BGroup.Signal.TypeAbbreviation = "VMB";
                                                                    foreach (var signal in phase)
                                                                    {
                                                                        var s = new SignalTree(signal);
                                                                        s.SignalCheckStatusChanged += _signalCheckStatusChanged;
                                                                        s.Parent = BGroup;
                                                                        BGroup.SignalList.Add(s);
                                                                    }
                                                                    mGroup.SignalList.Add(BGroup);
                                                                    break;
                                                                }

                                                            case "C":
                                                                {
                                                                    var CGroup = new SignalTree("Phase C");
                                                                    CGroup.SignalCheckStatusChanged += _signalCheckStatusChanged;
                                                                    CGroup.Parent = mGroup;
                                                                    //CGroup.Signal.TypeAbbreviation = "VMC";
                                                                    foreach (var signal in phase)
                                                                    {
                                                                        var s = new SignalTree(signal);
                                                                        s.SignalCheckStatusChanged += _signalCheckStatusChanged;
                                                                        s.Parent = CGroup;
                                                                        CGroup.SignalList.Add(s);
                                                                    }
                                                                    mGroup.SignalList.Add(CGroup);
                                                                    break;
                                                                }

                                                            default:
                                                                {
                                                                    throw new Exception("Error! Invalid signal phase: " + phase.Key + " found in Voltage magnitude!");
                                                                }
                                                        }
                                                    }
                                                    newHierachy.SignalList.Add(mGroup);
                                                    break;
                                                }

                                            case "A":
                                                {
                                                    var aGroup = new SignalTree("Angle");
                                                    aGroup.SignalCheckStatusChanged += _signalCheckStatusChanged;
                                                    aGroup.Parent = newHierachy;
                                                    //aGroup.Signal.TypeAbbreviation = "VA";
                                                    var aGroupHierachy = group.GroupBy(z => z.TypeAbbreviation.ToArray()[2].ToString());
                                                    foreach (var phase in aGroupHierachy)
                                                    {
                                                        switch (phase.Key)
                                                        {
                                                            case "P":
                                                                {
                                                                    var positiveGroup = new SignalTree("Positive Sequence");
                                                                    positiveGroup.SignalCheckStatusChanged += _signalCheckStatusChanged;
                                                                    positiveGroup.Parent = aGroup;
                                                                    //positiveGroup.Signal.TypeAbbreviation = "VAP";
                                                                    foreach (var signal in phase)
                                                                    {
                                                                        var s = new SignalTree(signal);
                                                                        s.SignalCheckStatusChanged += _signalCheckStatusChanged;
                                                                        s.Parent = positiveGroup;
                                                                        positiveGroup.SignalList.Add(s);
                                                                    }
                                                                    aGroup.SignalList.Add(positiveGroup);
                                                                    break;
                                                                }

                                                            case "A":
                                                                {
                                                                    var GroupA = new SignalTree("Phase A");
                                                                    GroupA.SignalCheckStatusChanged += _signalCheckStatusChanged;
                                                                    GroupA.Parent = aGroup;
                                                                    //GroupA.Signal.TypeAbbreviation = "VAA";
                                                                    foreach (var signal in phase)
                                                                    {
                                                                        var s = new SignalTree(signal);
                                                                        s.SignalCheckStatusChanged += _signalCheckStatusChanged;
                                                                        s.Parent = GroupA;
                                                                        GroupA.SignalList.Add(s);
                                                                    }
                                                                    aGroup.SignalList.Add(GroupA);
                                                                    break;
                                                                }

                                                            case "B":
                                                                {
                                                                    var GroupB = new SignalTree("Phase B");
                                                                    GroupB.SignalCheckStatusChanged += _signalCheckStatusChanged;
                                                                    GroupB.Parent = aGroup;
                                                                    //GroupB.Signal.TypeAbbreviation = "VAB";
                                                                    foreach (var signal in phase)
                                                                    {
                                                                        var s = new SignalTree(signal);
                                                                        s.SignalCheckStatusChanged += _signalCheckStatusChanged;
                                                                        s.Parent = GroupB;
                                                                        GroupB.SignalList.Add(s);
                                                                    }
                                                                    aGroup.SignalList.Add(GroupB);
                                                                    break;
                                                                }

                                                            case "C":
                                                                {
                                                                    var GroupC = new SignalTree("Phase C");
                                                                    GroupC.SignalCheckStatusChanged += _signalCheckStatusChanged;
                                                                    GroupC.Parent = aGroup;
                                                                    //GroupC.Signal.TypeAbbreviation = "VAC";
                                                                    foreach (var signal in phase)
                                                                    {
                                                                        var s = new SignalTree(signal);
                                                                        s.SignalCheckStatusChanged += _signalCheckStatusChanged;
                                                                        s.Parent = GroupC;
                                                                        GroupC.SignalList.Add(s);
                                                                    }
                                                                    aGroup.SignalList.Add(GroupC);
                                                                    break;
                                                                }

                                                            default:
                                                                {
                                                                    throw new Exception("Error! Invalid signal phase: " + phase.Key + " found in Voltage Angle!");
                                                                }
                                                        }
                                                    }
                                                    newHierachy.SignalList.Add(aGroup);
                                                    break;
                                                }

                                            case "P":
                                                {
                                                    var aGroup = new SignalTree("Phasor");
                                                    aGroup.SignalCheckStatusChanged += _signalCheckStatusChanged;
                                                    aGroup.Parent = newHierachy;
                                                    //aGroup.Signal.TypeAbbreviation = "VP";
                                                    var aGroupHierachy = group.GroupBy(z => z.TypeAbbreviation.ToArray()[2].ToString());
                                                    foreach (var phase in aGroupHierachy)
                                                    {
                                                        switch (phase.Key)
                                                        {
                                                            case "P":
                                                                {
                                                                    var positiveGroup = new SignalTree("Positive Sequence");
                                                                    positiveGroup.SignalCheckStatusChanged += _signalCheckStatusChanged;
                                                                    positiveGroup.Parent = aGroup;
                                                                    //positiveGroup.Signal.TypeAbbreviation = "VPP";
                                                                    foreach (var signal in phase)
                                                                    {
                                                                        var s = new SignalTree(signal);
                                                                        s.SignalCheckStatusChanged += _signalCheckStatusChanged;
                                                                        s.Parent = positiveGroup;
                                                                        positiveGroup.SignalList.Add(s);
                                                                    }
                                                                    aGroup.SignalList.Add(positiveGroup);
                                                                    break;
                                                                }

                                                            case "A":
                                                                {
                                                                    var GroupA = new SignalTree("Phase A");
                                                                    GroupA.SignalCheckStatusChanged += _signalCheckStatusChanged;
                                                                    GroupA.Parent = aGroup;
                                                                    //GroupA.Signal.TypeAbbreviation = "VPA";
                                                                    foreach (var signal in phase)
                                                                    {
                                                                        var s = new SignalTree(signal);
                                                                        s.SignalCheckStatusChanged += _signalCheckStatusChanged;
                                                                        s.Parent = GroupA;
                                                                        GroupA.SignalList.Add(s);
                                                                    }
                                                                    aGroup.SignalList.Add(GroupA);
                                                                    break;
                                                                }

                                                            case "B":
                                                                {
                                                                    var GroupB = new SignalTree("Phase B");
                                                                    GroupB.SignalCheckStatusChanged += _signalCheckStatusChanged;
                                                                    GroupB.Parent = aGroup;
                                                                    //GroupB.Signal.TypeAbbreviation = "VPB";
                                                                    foreach (var signal in phase)
                                                                    {
                                                                        var s = new SignalTree(signal);
                                                                        s.SignalCheckStatusChanged += _signalCheckStatusChanged;
                                                                        s.Parent = GroupB;
                                                                        GroupB.SignalList.Add(s);
                                                                    }
                                                                    aGroup.SignalList.Add(GroupB);
                                                                    break;
                                                                }

                                                            case "C":
                                                                {
                                                                    var GroupC = new SignalTree("Phase C");
                                                                    GroupC.SignalCheckStatusChanged += _signalCheckStatusChanged;
                                                                    GroupC.Parent = aGroup;
                                                                    //GroupC.Signal.TypeAbbreviation = "VPC";
                                                                    foreach (var signal in phase)
                                                                    {
                                                                        var s = new SignalTree(signal);
                                                                        s.SignalCheckStatusChanged += _signalCheckStatusChanged;
                                                                        s.Parent = GroupC;
                                                                        GroupC.SignalList.Add(s);
                                                                    }
                                                                    aGroup.SignalList.Add(GroupC);
                                                                    break;
                                                                }

                                                            default:
                                                                {
                                                                    throw new Exception("Error! Invalid signal phase: " + phase.Key + " found in Voltage Angle!");
                                                                }
                                                        }
                                                    }
                                                    newHierachy.SignalList.Add(aGroup);
                                                    break;
                                                }

                                            case "W":
                                                {
                                                    var mGroup = new SignalTree("Wave");
                                                    mGroup.SignalCheckStatusChanged += _signalCheckStatusChanged;
                                                    mGroup.Parent = newHierachy;
                                                    //mGroup.Signal.TypeAbbreviation = "VM";
                                                    var mGroupHierachky = group.GroupBy(z => z.TypeAbbreviation.ToArray()[2].ToString());
                                                    foreach (var phase in mGroupHierachky)
                                                    {
                                                        switch (phase.Key)
                                                        {
                                                            case "A":
                                                                {
                                                                    var AGroup = new SignalTree("Phase A");
                                                                    AGroup.SignalCheckStatusChanged += _signalCheckStatusChanged;
                                                                    AGroup.Parent = mGroup;
                                                                    //AGroup.Signal.TypeAbbreviation = "VWA";
                                                                    foreach (var signal in phase)
                                                                    {
                                                                        var s = new SignalTree(signal);
                                                                        s.SignalCheckStatusChanged += _signalCheckStatusChanged;
                                                                        s.Parent = AGroup;
                                                                        AGroup.SignalList.Add(s);
                                                                    }
                                                                    mGroup.SignalList.Add(AGroup);
                                                                    break;
                                                                }

                                                            case "B":
                                                                {
                                                                    var BGroup = new SignalTree("Phase B");
                                                                    BGroup.SignalCheckStatusChanged += _signalCheckStatusChanged;
                                                                    BGroup.Parent = mGroup;
                                                                    //BGroup.Signal.TypeAbbreviation = "VWB";
                                                                    foreach (var signal in phase)
                                                                    {
                                                                        var s = new SignalTree(signal);
                                                                        s.SignalCheckStatusChanged += _signalCheckStatusChanged;
                                                                        s.Parent = BGroup;
                                                                        BGroup.SignalList.Add(s);
                                                                    }
                                                                    mGroup.SignalList.Add(BGroup);
                                                                    break;
                                                                }

                                                            case "C":
                                                                {
                                                                    var CGroup = new SignalTree("Phase C");
                                                                    CGroup.SignalCheckStatusChanged += _signalCheckStatusChanged;
                                                                    CGroup.Parent = mGroup;
                                                                    //CGroup.Signal.TypeAbbreviation = "VWC";
                                                                    foreach (var signal in phase)
                                                                    {
                                                                        var s = new SignalTree(signal);
                                                                        s.SignalCheckStatusChanged += _signalCheckStatusChanged;
                                                                        s.Parent = CGroup;
                                                                        CGroup.SignalList.Add(s);
                                                                    }
                                                                    mGroup.SignalList.Add(CGroup);
                                                                    break;
                                                                }

                                                            default:
                                                                {
                                                                    throw new Exception("Error! Invalid signal phase: " + phase.Key + " found in Voltage magnitude!");
                                                                }
                                                        }
                                                    }
                                                    newHierachy.SignalList.Add(mGroup);
                                                    break;
                                                }
                                            default:
                                                {
                                                    throw new Exception("Error! Invalid voltage signal type found: " + group.Key);
                                                }
                                        }
                                    }
                                    signalTypeTree.Add(newHierachy);
                                    break;
                                }

                            case "I":
                                {
                                    var newHierachy = new SignalTree("Current");
                                    newHierachy.SignalCheckStatusChanged += _signalCheckStatusChanged;
                                    newHierachy.Parent = newTree;
                                    //newHierachy.Signal.TypeAbbreviation = "I";
                                    var currentHierachy = signalType.Value.GroupBy(y => y.TypeAbbreviation.ToArray()[1].ToString());
                                    foreach (var group in currentHierachy)
                                    {
                                        switch (group.Key)
                                        {
                                            case "M":
                                                {
                                                    var mGroup = new SignalTree("Magnitude");
                                                    mGroup.SignalCheckStatusChanged += _signalCheckStatusChanged;
                                                    mGroup.Parent = newHierachy;
                                                    //mGroup.Signal.TypeAbbreviation = "IM";
                                                    var mGroupHierachky = group.GroupBy(z => z.TypeAbbreviation.ToArray()[2].ToString());
                                                    foreach (var phase in mGroupHierachky)
                                                    {
                                                        switch (phase.Key)
                                                        {
                                                            case "P":
                                                                {
                                                                    var positiveGroup = new SignalTree("Positive Sequence");
                                                                    positiveGroup.SignalCheckStatusChanged += _signalCheckStatusChanged;
                                                                    positiveGroup.Parent = mGroup;
                                                                    //positiveGroup.Signal.TypeAbbreviation = "IMP";
                                                                    foreach (var signal in phase)
                                                                    {
                                                                        var s = new SignalTree(signal);
                                                                        s.SignalCheckStatusChanged += _signalCheckStatusChanged;
                                                                        s.Parent = positiveGroup;
                                                                        positiveGroup.SignalList.Add(s);
                                                                    }
                                                                    mGroup.SignalList.Add(positiveGroup);
                                                                    break;
                                                                }

                                                            case "A":
                                                                {
                                                                    var AGroup = new SignalTree("Phase A");
                                                                    AGroup.SignalCheckStatusChanged += _signalCheckStatusChanged;
                                                                    AGroup.Parent = mGroup;
                                                                    //AGroup.Signal.TypeAbbreviation = "IMA";
                                                                    foreach (var signal in phase)
                                                                    {
                                                                        var s = new SignalTree(signal);
                                                                        s.SignalCheckStatusChanged += _signalCheckStatusChanged;
                                                                        s.Parent = AGroup;
                                                                        AGroup.SignalList.Add(s);
                                                                    }
                                                                    mGroup.SignalList.Add(AGroup);
                                                                    break;
                                                                }

                                                            case "B":
                                                                {
                                                                    var BGroup = new SignalTree("Phase B");
                                                                    BGroup.SignalCheckStatusChanged += _signalCheckStatusChanged;
                                                                    BGroup.Parent = mGroup;
                                                                    //BGroup.Signal.TypeAbbreviation = "IMB";
                                                                    foreach (var signal in phase)
                                                                    {
                                                                        var s = new SignalTree(signal);
                                                                        s.SignalCheckStatusChanged += _signalCheckStatusChanged;
                                                                        s.Parent = BGroup;
                                                                        BGroup.SignalList.Add(s);
                                                                    }
                                                                    mGroup.SignalList.Add(BGroup);
                                                                    break;
                                                                }

                                                            case "C":
                                                                {
                                                                    var CGroup = new SignalTree("Phase C");
                                                                    CGroup.SignalCheckStatusChanged += _signalCheckStatusChanged;
                                                                    CGroup.Parent = mGroup;
                                                                    //CGroup.Signal.TypeAbbreviation = "IMC";
                                                                    foreach (var signal in phase)
                                                                    {
                                                                        var s = new SignalTree(signal);
                                                                        s.SignalCheckStatusChanged += _signalCheckStatusChanged;
                                                                        s.Parent = CGroup;
                                                                        CGroup.SignalList.Add(s);
                                                                    }
                                                                    mGroup.SignalList.Add(CGroup);
                                                                    break;
                                                                }

                                                            default:
                                                                {
                                                                    throw new Exception("Error! Invalid signal phase: " + phase.Key + " found in Voltage magnitude!");
                                                                }
                                                        }
                                                    }
                                                    newHierachy.SignalList.Add(mGroup);
                                                    break;
                                                }

                                            case "A":
                                                {
                                                    var aGroup = new SignalTree("Angle");
                                                    aGroup.SignalCheckStatusChanged += _signalCheckStatusChanged;
                                                    aGroup.Parent = newHierachy;
                                                    var aGroupHierachy = group.GroupBy(z => z.TypeAbbreviation.ToArray()[2].ToString());
                                                    foreach (var phase in aGroupHierachy)
                                                    {
                                                        switch (phase.Key)
                                                        {
                                                            case "P":
                                                                {
                                                                    var positiveGroup = new SignalTree("Positive Sequence");
                                                                    positiveGroup.SignalCheckStatusChanged += _signalCheckStatusChanged;
                                                                    positiveGroup.Parent = aGroup;
                                                                    foreach (var signal in phase)
                                                                    {
                                                                        var s = new SignalTree(signal);
                                                                        s.SignalCheckStatusChanged += _signalCheckStatusChanged;
                                                                        s.Parent = positiveGroup;
                                                                        positiveGroup.SignalList.Add(s);
                                                                    }
                                                                    aGroup.SignalList.Add(positiveGroup);
                                                                    break;
                                                                }

                                                            case "A":
                                                                {
                                                                    var GroupA = new SignalTree("Phase A");
                                                                    GroupA.SignalCheckStatusChanged += _signalCheckStatusChanged;
                                                                    GroupA.Parent = aGroup;
                                                                    foreach (var signal in phase)
                                                                    {
                                                                        var s = new SignalTree(signal);
                                                                        s.SignalCheckStatusChanged += _signalCheckStatusChanged;
                                                                        s.Parent = GroupA;
                                                                        GroupA.SignalList.Add(s);
                                                                    }
                                                                    aGroup.SignalList.Add(GroupA);
                                                                    break;
                                                                }

                                                            case "B":
                                                                {
                                                                    var GroupB = new SignalTree("Phase B");
                                                                    GroupB.SignalCheckStatusChanged += _signalCheckStatusChanged;
                                                                    GroupB.Parent = aGroup;
                                                                    foreach (var signal in phase)
                                                                    {
                                                                        var s = new SignalTree(signal);
                                                                        s.SignalCheckStatusChanged += _signalCheckStatusChanged;
                                                                        s.Parent = GroupB;
                                                                        GroupB.SignalList.Add(s);
                                                                    }
                                                                    aGroup.SignalList.Add(GroupB);
                                                                    break;
                                                                }

                                                            case "C":
                                                                {
                                                                    var GroupC = new SignalTree("Phase C");
                                                                    GroupC.SignalCheckStatusChanged += _signalCheckStatusChanged;
                                                                    GroupC.Parent = aGroup;
                                                                    foreach (var signal in phase)
                                                                    {
                                                                        var s = new SignalTree(signal);
                                                                        s.SignalCheckStatusChanged += _signalCheckStatusChanged;
                                                                        s.Parent = GroupC;
                                                                        GroupC.SignalList.Add(s);
                                                                    }
                                                                    aGroup.SignalList.Add(GroupC);
                                                                    break;
                                                                }

                                                            default:
                                                                {
                                                                    throw new Exception("Error! Invalid signal phase: " + phase.Key + " found in Voltage Angle!");
                                                                }
                                                        }
                                                    }
                                                    newHierachy.SignalList.Add(aGroup);
                                                    break;
                                                }

                                            case "P":
                                                {
                                                    var aGroup = new SignalTree("Phasor");
                                                    aGroup.SignalCheckStatusChanged += _signalCheckStatusChanged;
                                                    aGroup.Parent = newHierachy;
                                                    var aGroupHierachy = group.GroupBy(z => z.TypeAbbreviation.ToArray()[2].ToString());
                                                    foreach (var phase in aGroupHierachy)
                                                    {
                                                        switch (phase.Key)
                                                        {
                                                            case "P":
                                                                {
                                                                    var positiveGroup = new SignalTree("Positive Sequence");
                                                                    positiveGroup.SignalCheckStatusChanged += _signalCheckStatusChanged;
                                                                    positiveGroup.Parent = aGroup;
                                                                    foreach (var signal in phase)
                                                                    {
                                                                        var s = new SignalTree(signal);
                                                                        s.SignalCheckStatusChanged += _signalCheckStatusChanged;
                                                                        s.Parent = positiveGroup;
                                                                        positiveGroup.SignalList.Add(s);
                                                                    }
                                                                    aGroup.SignalList.Add(positiveGroup);
                                                                    break;
                                                                }

                                                            case "A":
                                                                {
                                                                    var GroupA = new SignalTree("Phase A");
                                                                    GroupA.SignalCheckStatusChanged += _signalCheckStatusChanged;
                                                                    GroupA.Parent = aGroup;
                                                                    foreach (var signal in phase)
                                                                    {
                                                                        var s = new SignalTree(signal);
                                                                        s.SignalCheckStatusChanged += _signalCheckStatusChanged;
                                                                        s.Parent = GroupA;
                                                                        GroupA.SignalList.Add(s);
                                                                    }
                                                                    aGroup.SignalList.Add(GroupA);
                                                                    break;
                                                                }

                                                            case "B":
                                                                {
                                                                    var GroupB = new SignalTree("Phase B");
                                                                    GroupB.SignalCheckStatusChanged += _signalCheckStatusChanged;
                                                                    GroupB.Parent = aGroup;
                                                                    foreach (var signal in phase)
                                                                    {
                                                                        var s = new SignalTree(signal);
                                                                        s.SignalCheckStatusChanged += _signalCheckStatusChanged;
                                                                        s.Parent = GroupB;
                                                                        GroupB.SignalList.Add(s);
                                                                    }
                                                                    aGroup.SignalList.Add(GroupB);
                                                                    break;
                                                                }

                                                            case "C":
                                                                {
                                                                    var GroupC = new SignalTree("Phase C");
                                                                    GroupC.SignalCheckStatusChanged += _signalCheckStatusChanged;
                                                                    GroupC.Parent = aGroup;
                                                                    foreach (var signal in phase)
                                                                    {
                                                                        var s = new SignalTree(signal);
                                                                        s.SignalCheckStatusChanged += _signalCheckStatusChanged;
                                                                        s.Parent = GroupC;
                                                                        GroupC.SignalList.Add(s);
                                                                    }
                                                                    aGroup.SignalList.Add(GroupC);
                                                                    break;
                                                                }

                                                            default:
                                                                {
                                                                    throw new Exception("Error! Invalid signal phase: " + phase.Key + " found in Voltage Angle!");
                                                                }
                                                        }
                                                    }
                                                    newHierachy.SignalList.Add(aGroup);
                                                    break;
                                                }

                                            case "W":
                                                {
                                                    var mGroup = new SignalTree("Wave");
                                                    mGroup.SignalCheckStatusChanged += _signalCheckStatusChanged;
                                                    mGroup.Parent = newHierachy;
                                                    var mGroupHierachky = group.GroupBy(z => z.TypeAbbreviation.ToArray()[2].ToString());
                                                    foreach (var phase in mGroupHierachky)
                                                    {
                                                        switch (phase.Key)
                                                        {
                                                            case "A":
                                                                {
                                                                    var AGroup = new SignalTree("Phase A");
                                                                    AGroup.SignalCheckStatusChanged += _signalCheckStatusChanged;
                                                                    AGroup.Parent = mGroup;
                                                                    foreach (var signal in phase)
                                                                    {
                                                                        var s = new SignalTree(signal);
                                                                        s.SignalCheckStatusChanged += _signalCheckStatusChanged;
                                                                        s.Parent = AGroup;
                                                                        AGroup.SignalList.Add(s);
                                                                    }
                                                                    mGroup.SignalList.Add(AGroup);
                                                                    break;
                                                                }

                                                            case "B":
                                                                {
                                                                    var BGroup = new SignalTree("Phase B");
                                                                    BGroup.SignalCheckStatusChanged += _signalCheckStatusChanged;
                                                                    BGroup.Parent = mGroup;
                                                                    foreach (var signal in phase)
                                                                    {
                                                                        var s = new SignalTree(signal);
                                                                        s.SignalCheckStatusChanged += _signalCheckStatusChanged;
                                                                        s.Parent = BGroup;
                                                                        BGroup.SignalList.Add(s);
                                                                    }
                                                                    mGroup.SignalList.Add(BGroup);
                                                                    break;
                                                                }

                                                            case "C":
                                                                {
                                                                    var CGroup = new SignalTree("Phase C");
                                                                    CGroup.SignalCheckStatusChanged += _signalCheckStatusChanged;
                                                                    CGroup.Parent = mGroup;
                                                                    foreach (var signal in phase)
                                                                    {
                                                                        var s = new SignalTree(signal);
                                                                        s.SignalCheckStatusChanged += _signalCheckStatusChanged;
                                                                        s.Parent = CGroup;
                                                                        CGroup.SignalList.Add(s);
                                                                    }
                                                                    mGroup.SignalList.Add(CGroup);
                                                                    break;
                                                                }

                                                            default:
                                                                {
                                                                    throw new Exception("Error! Invalid signal phase: " + phase.Key + " found in Voltage magnitude!");
                                                                }
                                                        }
                                                    }
                                                    newHierachy.SignalList.Add(mGroup);
                                                    break;
                                                }
                                            default:
                                                {
                                                    throw new Exception("Error! Invalid voltage signal type found: " + group.Key);
                                                }
                                        }
                                    }
                                    signalTypeTree.Add(newHierachy);
                                    break;
                                }
                            default:
                                {
                                    throw new Exception("Error! Invalid signal type found: " + signalType.Key);
                                }
                        }
                    }
                    newTree.SignalList = signalTypeTree;
                    signalTypeTreeGroupedBySamplingRate.Add(newTree);
                }
            }
            return signalTypeTreeGroupedBySamplingRate;
        }
        public ObservableCollection<SignalTree> SortSignalByPMU(ObservableCollection<SignalViewModel> signalList)
        {
            var pmuSignalTreeGroupedBySamplingRate = new ObservableCollection<SignalTree>();
            if (signalList != null && signalList.Count > 0)
            {
                var groupBySamplingRate = signalList.GroupBy(x => x.SamplingRate);
                foreach (var group in groupBySamplingRate)
                {
                    var rate = group.Key;
                    var newTree = new SignalTree("Sampling Rate: " + rate.ToString() + "/Second");
                    newTree.SignalCheckStatusChanged += _signalCheckStatusChanged;
                    var subSignalList = group.ToList();
                    var PMUSignalDictionary = new Dictionary<string, List<SignalViewModel>>();
                    try
                    {
                        var pairs = subSignalList.GroupBy(x => x.PMUName);
                        PMUSignalDictionary = pairs.ToDictionary(x => x.Key, x => x.ToList());
                    }
                    catch (Exception ex)
                    {
                        throw new Exception("Missing PMU name. " + ex.Message);
                    }
                    var pmuSignalTree = new ObservableCollection<SignalTree>();
                    foreach (var subgroup in PMUSignalDictionary)
                    {
                        //var newPMUSignature = new SignalViewModel(subgroup.Key, subgroup.Key);
                        var newGroup = new SignalTree(subgroup.Key);
                        newGroup.SignalCheckStatusChanged += _signalCheckStatusChanged;
                        newGroup.Parent = newTree;
                        foreach (var signal in subgroup.Value)
                        {
                            var aTree = new SignalTree(signal);
                            aTree.SignalCheckStatusChanged += _signalCheckStatusChanged;
                            aTree.Parent = newGroup;
                            newGroup.SignalList.Add(aTree);
                        }
                        //newGroup.Signal.SamplingRate = subgroup.Value.FirstOrDefault().SamplingRate;
                        pmuSignalTree.Add(newGroup);
                    }
                    //newSig.SamplingRate = rate;
                    //var a = new SignalTree(newSig);
                    newTree.SignalList = pmuSignalTree;
                    pmuSignalTreeGroupedBySamplingRate.Add(newTree);
                }
            }
            return pmuSignalTreeGroupedBySamplingRate;
        }
        internal SignalViewModel FindSignal(string pMUName, string signalName)
        {
            if (SignalsVM != null)
            {
                foreach (var sig in SignalsVM)
                {
                    if (sig.PMUName == pMUName && sig.SignalName == signalName)
                    {
                        return sig;
                    }
                }
            }
            foreach (var group in GroupedSignalByPreProcessStepsOutput)
            {
                foreach (var samplingRateSubgroup in group.SignalList)
                {
                    foreach (var subgroup in samplingRateSubgroup.SignalList)
                    {
                        if (subgroup.Label == pMUName)
                        {
                            foreach (var subsubgroup in subgroup.SignalList)
                            {
                                if (subsubgroup.Signal.SignalName == signalName)
                                    return subsubgroup.Signal;
                            }
                        }
                    }
                }
            }           
            return null;            
        }

        private ObservableCollection<PMUWithSamplingRate> _allPMU;
        public ObservableCollection<PMUWithSamplingRate> AllPMUs
        {
            set { _allPMU = value; OnPropertyChanged(); }
            get
            {
                return _getAllPMU();
            }
        }
        private ObservableCollection<PMUWithSamplingRate> _getAllPMU()
        {
            List<PMUWithSamplingRate> allPMU = null;
            if (GroupedRawSignalsByPMU != null && GroupedRawSignalsByPMU.Count > 0)
            {
                allPMU = GroupedRawSignalsByPMU.SelectMany(x => x.SignalList).Distinct().SelectMany(r => r.SignalList).Distinct().Select(y => new PMUWithSamplingRate(y.Signal.PMUName, y.Signal.SamplingRate, y.Signal.Data.Count)).ToList();
            }
            if (AllPreProcessOutputGroupedByPMU != null && AllPreProcessOutputGroupedByPMU.Count > 0)
            {
                allPMU.AddRange(AllPreProcessOutputGroupedByPMU.SelectMany(x => x.SignalList).Distinct().SelectMany(r => r.SignalList).Distinct().Select(y => new PMUWithSamplingRate(y.Signal.PMUName, y.Signal.SamplingRate, y.Signal.Data.Count)).ToList());
            }
            if (allPMU != null)
            {
                return new ObservableCollection<PMUWithSamplingRate>(allPMU.Distinct());
            }
            else
            {
                return null;
            }
        }
        private Visibility _signalSelectionTreeViewVisibility;
        public Visibility SignalSelectionTreeViewVisibility 
        {
            get { return _signalSelectionTreeViewVisibility; }
            set
            {
                _signalSelectionTreeViewVisibility = value;
                OnPropertyChanged();
            }
        }
    }
}
    