using AS.Core.Models;
using AS.Core.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace AS.SampleDataManager
{
    public class SampleDataMngr
    {

        private static SampleDataMngr _instance = null;
        public static SampleDataMngr Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new SampleDataMngr();
                }
                return _instance;
            }
        }
        private SampleDataMngr()
        {
            _signalsVM = new ObservableCollection<SignalViewModel>();
        }

        private List<Signal> _signals;
        private ObservableCollection<SignalViewModel> _signalsVM;
        public ObservableCollection<SignalViewModel> SignalsVM
        {
            get { return _signalsVM; }
            set
            {
                _signalsVM = value;
            }
        }
        public void AddSampleSignals(List<Signal> signals)
        {
            _signals = signals;
            _signalsVM.Clear();
            foreach (var item in signals)
            {
                var newitem = new SignalViewModel(item);
                _signalsVM.Add(newitem);
            }
            GroupedSignalsByType = SortSignalsByType(_signalsVM);
            GroupedSignalsByPMU = SortSignalByPMU(_signalsVM);
        }
        private ObservableCollection<SignalTree> _groupedSignalsByType;
        public ObservableCollection<SignalTree> GroupedSignalsByType
        {
            get { return _groupedSignalsByType; }
            set
            {
                _groupedSignalsByType = value;
            }
        }
        private ObservableCollection<SignalTree> _groupedSignalsByPMU;
        public ObservableCollection<SignalTree> GroupedSignalsByPMU
        {
            get { return _groupedSignalsByPMU; }
            set
            {
                _groupedSignalsByPMU = value;
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
                    newTree.SignalCheckStatusChanged += _signalTreeCheckStatusChanged;
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
                                                    newHierachy.Parent = newTree;
                                                    //newHierachy.Signal.TypeAbbreviation = "S";
                                                    foreach (var signal in group)
                                                    {
                                                        var s = new SignalTree(signal);
                                                        s.Parent = newHierachy;
                                                        newHierachy.SignalList.Add(s);
                                                    }
                                                    signalTypeTree.Add(newHierachy);
                                                    break;
                                                }

                                            case "SC":
                                                {
                                                    var newHierachy = new SignalTree("Scalar");
                                                    newHierachy.Parent = newTree;
                                                    //newHierachy.Signal.TypeAbbreviation = "SC";
                                                    foreach (var signal in group)
                                                    {
                                                        var s = new SignalTree(signal);
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
                                    newHierachy.Parent = newTree;
                                    //newHierachy.Signal.TypeAbbreviation = "OTHER";
                                    foreach (var signal in signalType.Value)
                                    {
                                        var s = new SignalTree(signal);
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
                                                    newHierachy.Parent = newTree;
                                                    //newHierachy.Signal.TypeAbbreviation = "C";
                                                    foreach (var signal in group)
                                                    {
                                                        var s = new SignalTree(signal);
                                                        s.Parent = newHierachy;
                                                        newHierachy.SignalList.Add(s);
                                                    }
                                                    signalTypeTree.Add(newHierachy);
                                                    break;
                                                }

                                            case "CP":
                                                {
                                                    var newHierachy = new SignalTree("Complex");
                                                    newHierachy.Parent = newTree;
                                                    //newHierachy.Signal.TypeAbbreviation = "CP";
                                                    foreach (var signal in group)
                                                    {
                                                        var s = new SignalTree(signal);
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
                                    newHierachy.Parent = newTree;
                                    //newHierachy.Signal.TypeAbbreviation = "D";
                                    foreach (var signal in signalType.Value)
                                    {
                                        var s = new SignalTree(signal);
                                        s.Parent = newHierachy;
                                        newHierachy.SignalList.Add(s);
                                    }
                                    signalTypeTree.Add(newHierachy);
                                    break;
                                }

                            case "F":
                                {
                                    var newHierachy = new SignalTree("Frequency");
                                    newHierachy.Parent = newTree;
                                    //newHierachy.Signal.TypeAbbreviation = "F";
                                    foreach (var signal in signalType.Value)
                                    {
                                        var s = new SignalTree(signal);
                                        s.Parent = newHierachy;
                                        newHierachy.SignalList.Add(s);
                                    }
                                    signalTypeTree.Add(newHierachy);
                                    break;
                                }

                            case "R":
                                {
                                    var newHierachy = new SignalTree("Rate of Change of Frequency");
                                    newHierachy.Parent = newTree;
                                    //newHierachy.Signal.TypeAbbreviation = "R";
                                    foreach (var signal in signalType.Value)
                                    {
                                        var s = new SignalTree(signal);
                                        s.Parent = newHierachy;
                                        newHierachy.SignalList.Add(s);
                                    }
                                    signalTypeTree.Add(newHierachy);
                                    break;
                                }

                            case "Q":
                                {
                                    var newHierachy = new SignalTree("Reactive Power");
                                    newHierachy.Parent = newTree;
                                    //newHierachy.Signal.TypeAbbreviation = "Q";
                                    foreach (var signal in signalType.Value)
                                    {
                                        var s = new SignalTree(signal);
                                        s.Parent = newHierachy;
                                        newHierachy.SignalList.Add(s);
                                    }
                                    signalTypeTree.Add(newHierachy);
                                    break;
                                }

                            case "P":
                                {
                                    var newHierachy = new SignalTree("Active Power");
                                    newHierachy.Parent = newTree;
                                    //newHierachy.Signal.TypeAbbreviation = "P";
                                    foreach (var signal in signalType.Value)
                                    {
                                        var s = new SignalTree(signal);
                                        s.Parent = newHierachy;
                                        newHierachy.SignalList.Add(s);
                                    }
                                    signalTypeTree.Add(newHierachy);
                                    break;
                                }

                            case "V":
                                {
                                    var newHierachy = new SignalTree("Voltage");
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
                                                                    positiveGroup.Parent = mGroup;
                                                                    //positiveGroup.Signal.TypeAbbreviation = "VMP";
                                                                    foreach (var signal in phase)
                                                                    {
                                                                        var s = new SignalTree(signal);
                                                                        s.Parent = positiveGroup;
                                                                        positiveGroup.SignalList.Add(s);
                                                                    }
                                                                    mGroup.SignalList.Add(positiveGroup);
                                                                    break;
                                                                }

                                                            case "A":
                                                                {
                                                                    var AGroup = new SignalTree("Phase A");
                                                                    AGroup.Parent = mGroup;
                                                                    //AGroup.Signal.TypeAbbreviation = "VMA";
                                                                    foreach (var signal in phase)
                                                                    {
                                                                        var s = new SignalTree(signal);
                                                                        s.Parent = AGroup;
                                                                        AGroup.SignalList.Add(s);
                                                                    }
                                                                    mGroup.SignalList.Add(AGroup);
                                                                    break;
                                                                }

                                                            case "B":
                                                                {
                                                                    var BGroup = new SignalTree("Phase B");
                                                                    BGroup.Parent = mGroup;
                                                                    //BGroup.Signal.TypeAbbreviation = "VMB";
                                                                    foreach (var signal in phase)
                                                                    {
                                                                        var s = new SignalTree(signal);
                                                                        s.Parent = BGroup;
                                                                        BGroup.SignalList.Add(s);
                                                                    }
                                                                    mGroup.SignalList.Add(BGroup);
                                                                    break;
                                                                }

                                                            case "C":
                                                                {
                                                                    var CGroup = new SignalTree("Phase C");
                                                                    CGroup.Parent = mGroup;
                                                                    CGroup.Signal.TypeAbbreviation = "VMC";
                                                                    foreach (var signal in phase)
                                                                    {
                                                                        var s = new SignalTree(signal);
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
                                                                    positiveGroup.Parent = aGroup;
                                                                    //positiveGroup.Signal.TypeAbbreviation = "VAP";
                                                                    foreach (var signal in phase)
                                                                    {
                                                                        var s = new SignalTree(signal);
                                                                        s.Parent = positiveGroup;
                                                                        positiveGroup.SignalList.Add(s);
                                                                    }
                                                                    aGroup.SignalList.Add(positiveGroup);
                                                                    break;
                                                                }

                                                            case "A":
                                                                {
                                                                    var GroupA = new SignalTree("Phase A");
                                                                    GroupA.Parent = aGroup;
                                                                    //GroupA.Signal.TypeAbbreviation = "VAA";
                                                                    foreach (var signal in phase)
                                                                    {
                                                                        var s = new SignalTree(signal);
                                                                        s.Parent = GroupA;
                                                                        GroupA.SignalList.Add(s);
                                                                    }
                                                                    aGroup.SignalList.Add(GroupA);
                                                                    break;
                                                                }

                                                            case "B":
                                                                {
                                                                    var GroupB = new SignalTree("Phase B");
                                                                    GroupB.Parent = aGroup;
                                                                    //GroupB.Signal.TypeAbbreviation = "VAB";
                                                                    foreach (var signal in phase)
                                                                    {
                                                                        var s = new SignalTree(signal);
                                                                        s.Parent = GroupB;
                                                                        GroupB.SignalList.Add(s);
                                                                    }
                                                                    aGroup.SignalList.Add(GroupB);
                                                                    break;
                                                                }

                                                            case "C":
                                                                {
                                                                    var GroupC = new SignalTree("Phase C");
                                                                    GroupC.Parent = aGroup;
                                                                    //GroupC.Signal.TypeAbbreviation = "VAC";
                                                                    foreach (var signal in phase)
                                                                    {
                                                                        var s = new SignalTree(signal);
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
                                                                    positiveGroup.Parent = aGroup;
                                                                    //positiveGroup.Signal.TypeAbbreviation = "VPP";
                                                                    foreach (var signal in phase)
                                                                    {
                                                                        var s = new SignalTree(signal);
                                                                        s.Parent = positiveGroup;
                                                                        positiveGroup.SignalList.Add(s);
                                                                    }
                                                                    aGroup.SignalList.Add(positiveGroup);
                                                                    break;
                                                                }

                                                            case "A":
                                                                {
                                                                    var GroupA = new SignalTree("Phase A");
                                                                    GroupA.Parent = aGroup;
                                                                    //GroupA.Signal.TypeAbbreviation = "VPA";
                                                                    foreach (var signal in phase)
                                                                    {
                                                                        var s = new SignalTree(signal);
                                                                        s.Parent = GroupA;
                                                                        GroupA.SignalList.Add(s);
                                                                    }
                                                                    aGroup.SignalList.Add(GroupA);
                                                                    break;
                                                                }

                                                            case "B":
                                                                {
                                                                    var GroupB = new SignalTree("Phase B");
                                                                    GroupB.Parent = aGroup;
                                                                    //GroupB.Signal.TypeAbbreviation = "VPB";
                                                                    foreach (var signal in phase)
                                                                    {
                                                                        var s = new SignalTree(signal);
                                                                        s.Parent = GroupB;
                                                                        GroupB.SignalList.Add(s);
                                                                    }
                                                                    aGroup.SignalList.Add(GroupB);
                                                                    break;
                                                                }

                                                            case "C":
                                                                {
                                                                    var GroupC = new SignalTree("Phase C");
                                                                    GroupC.Parent = aGroup;
                                                                    //GroupC.Signal.TypeAbbreviation = "VPC";
                                                                    foreach (var signal in phase)
                                                                    {
                                                                        var s = new SignalTree(signal);
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
                                                                    AGroup.Parent = mGroup;
                                                                    //AGroup.Signal.TypeAbbreviation = "VWA";
                                                                    foreach (var signal in phase)
                                                                    {
                                                                        var s = new SignalTree(signal);
                                                                        s.Parent = AGroup;
                                                                        AGroup.SignalList.Add(s);
                                                                    }
                                                                    mGroup.SignalList.Add(AGroup);
                                                                    break;
                                                                }

                                                            case "B":
                                                                {
                                                                    var BGroup = new SignalTree("Phase B");
                                                                    BGroup.Parent = mGroup;
                                                                    //BGroup.Signal.TypeAbbreviation = "VWB";
                                                                    foreach (var signal in phase)
                                                                    {
                                                                        var s = new SignalTree(signal);
                                                                        s.Parent = BGroup;
                                                                        BGroup.SignalList.Add(s);
                                                                    }
                                                                    mGroup.SignalList.Add(BGroup);
                                                                    break;
                                                                }

                                                            case "C":
                                                                {
                                                                    var CGroup = new SignalTree("Phase C");
                                                                    CGroup.Parent = mGroup;
                                                                    //CGroup.Signal.TypeAbbreviation = "VWC";
                                                                    foreach (var signal in phase)
                                                                    {
                                                                        var s = new SignalTree(signal);
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
                                                                    positiveGroup.Parent = mGroup;
                                                                    //positiveGroup.Signal.TypeAbbreviation = "IMP";
                                                                    foreach (var signal in phase)
                                                                    {
                                                                        var s = new SignalTree(signal);
                                                                        s.Parent = positiveGroup;
                                                                        positiveGroup.SignalList.Add(s);
                                                                    }
                                                                    mGroup.SignalList.Add(positiveGroup);
                                                                    break;
                                                                }

                                                            case "A":
                                                                {
                                                                    var AGroup = new SignalTree("Phase A");
                                                                    AGroup.Parent = mGroup;
                                                                    //AGroup.Signal.TypeAbbreviation = "IMA";
                                                                    foreach (var signal in phase)
                                                                    {
                                                                        var s = new SignalTree(signal);
                                                                        s.Parent = AGroup;
                                                                        AGroup.SignalList.Add(s);
                                                                    }
                                                                    mGroup.SignalList.Add(AGroup);
                                                                    break;
                                                                }

                                                            case "B":
                                                                {
                                                                    var BGroup = new SignalTree("Phase B");
                                                                    BGroup.Parent = mGroup;
                                                                    //BGroup.Signal.TypeAbbreviation = "IMB";
                                                                    foreach (var signal in phase)
                                                                    {
                                                                        var s = new SignalTree(signal);
                                                                        s.Parent = BGroup;
                                                                        BGroup.SignalList.Add(s);
                                                                    }
                                                                    mGroup.SignalList.Add(BGroup);
                                                                    break;
                                                                }

                                                            case "C":
                                                                {
                                                                    var CGroup = new SignalTree("Phase C");
                                                                    CGroup.Parent = mGroup;
                                                                    //CGroup.Signal.TypeAbbreviation = "IMC";
                                                                    foreach (var signal in phase)
                                                                    {
                                                                        var s = new SignalTree(signal);
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
                                                    aGroup.Parent = newHierachy;
                                                    //aGroup.Signal.TypeAbbreviation = "IA";
                                                    var aGroupHierachy = group.GroupBy(z => z.TypeAbbreviation.ToArray()[2].ToString());
                                                    foreach (var phase in aGroupHierachy)
                                                    {
                                                        switch (phase.Key)
                                                        {
                                                            case "P":
                                                                {
                                                                    var positiveGroup = new SignalTree("Positive Sequence");
                                                                    positiveGroup.Parent = aGroup;
                                                                    //positiveGroup.Signal.TypeAbbreviation = "IAP";
                                                                    foreach (var signal in phase)
                                                                    {
                                                                        var s = new SignalTree(signal);
                                                                        s.Parent = positiveGroup;
                                                                        positiveGroup.SignalList.Add(s);
                                                                    }
                                                                    aGroup.SignalList.Add(positiveGroup);
                                                                    break;
                                                                }

                                                            case "A":
                                                                {
                                                                    var GroupA = new SignalTree("Phase A");
                                                                    GroupA.Parent = aGroup;
                                                                    //GroupA.Signal.TypeAbbreviation = "IAA";
                                                                    foreach (var signal in phase)
                                                                    {
                                                                        var s = new SignalTree(signal);
                                                                        s.Parent = GroupA;
                                                                        GroupA.SignalList.Add(s);
                                                                    }
                                                                    aGroup.SignalList.Add(GroupA);
                                                                    break;
                                                                }

                                                            case "B":
                                                                {
                                                                    var GroupB = new SignalTree("Phase B");
                                                                    GroupB.Parent = aGroup;
                                                                    //GroupB.Signal.TypeAbbreviation = "IAB";
                                                                    foreach (var signal in phase)
                                                                    {
                                                                        var s = new SignalTree(signal);
                                                                        s.Parent = GroupB;
                                                                        GroupB.SignalList.Add(s);
                                                                    }
                                                                    aGroup.SignalList.Add(GroupB);
                                                                    break;
                                                                }

                                                            case "C":
                                                                {
                                                                    var GroupC = new SignalTree("Phase C");
                                                                    GroupC.Parent = aGroup;
                                                                    //GroupC.Signal.TypeAbbreviation = "IAC";
                                                                    foreach (var signal in phase)
                                                                    {
                                                                        var s = new SignalTree(signal);
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
                                                    aGroup.Parent = newHierachy;
                                                    //aGroup.Signal.TypeAbbreviation = "IP";
                                                    var aGroupHierachy = group.GroupBy(z => z.TypeAbbreviation.ToArray()[2].ToString());
                                                    foreach (var phase in aGroupHierachy)
                                                    {
                                                        switch (phase.Key)
                                                        {
                                                            case "P":
                                                                {
                                                                    var positiveGroup = new SignalTree("Positive Sequence");
                                                                    positiveGroup.Parent = aGroup;
                                                                    //positiveGroup.Signal.TypeAbbreviation = "IPP";
                                                                    foreach (var signal in phase)
                                                                    {
                                                                        var s = new SignalTree(signal);
                                                                        s.Parent = aGroup;
                                                                        aGroup.SignalList.Add(s);
                                                                    }
                                                                    aGroup.SignalList.Add(positiveGroup);
                                                                    break;
                                                                }

                                                            case "A":
                                                                {
                                                                    var GroupA = new SignalTree("Phase A");
                                                                    GroupA.Parent = aGroup;
                                                                    //GroupA.Signal.TypeAbbreviation = "IPA";
                                                                    foreach (var signal in phase)
                                                                    {
                                                                        var s = new SignalTree(signal);
                                                                        s.Parent = GroupA;
                                                                        GroupA.SignalList.Add(s);
                                                                    }
                                                                    aGroup.SignalList.Add(GroupA);
                                                                    break;
                                                                }

                                                            case "B":
                                                                {
                                                                    var GroupB = new SignalTree("Phase B");
                                                                    GroupB.Parent = aGroup;
                                                                    //GroupB.Signal.TypeAbbreviation = "IPB";
                                                                    foreach (var signal in phase)
                                                                    {
                                                                        var s = new SignalTree(signal);
                                                                        s.Parent = GroupB;
                                                                        GroupB.SignalList.Add(s);
                                                                    }
                                                                    aGroup.SignalList.Add(GroupB);
                                                                    break;
                                                                }

                                                            case "C":
                                                                {
                                                                    var GroupC = new SignalTree("Phase C");
                                                                    GroupC.Parent = aGroup;
                                                                    //GroupC.Signal.TypeAbbreviation = "IPC";
                                                                    foreach (var signal in phase)
                                                                    {
                                                                        var s = new SignalTree(signal);
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
                                                    mGroup.Parent = newHierachy;
                                                    //mGroup.Signal.TypeAbbreviation = "IM";
                                                    var mGroupHierachky = group.GroupBy(z => z.TypeAbbreviation.ToArray()[2].ToString());
                                                    foreach (var phase in mGroupHierachky)
                                                    {
                                                        switch (phase.Key)
                                                        {
                                                            case "A":
                                                                {
                                                                    var AGroup = new SignalTree("Phase A");
                                                                    AGroup.Parent = mGroup;
                                                                    //AGroup.Signal.TypeAbbreviation = "IWA";
                                                                    foreach (var signal in phase)
                                                                    {
                                                                        var s = new SignalTree(signal);
                                                                        s.Parent = AGroup;
                                                                        AGroup.SignalList.Add(s);
                                                                    }
                                                                    mGroup.SignalList.Add(AGroup);
                                                                    break;
                                                                }

                                                            case "B":
                                                                {
                                                                    var BGroup = new SignalTree("Phase B");
                                                                    BGroup.Parent = mGroup;
                                                                    //BGroup.Signal.TypeAbbreviation = "IWB";
                                                                    foreach (var signal in phase)
                                                                    {
                                                                        var s = new SignalTree(signal);
                                                                        s.Parent = BGroup;
                                                                        BGroup.SignalList.Add(s);
                                                                    }
                                                                    mGroup.SignalList.Add(BGroup);
                                                                    break;
                                                                }

                                                            case "C":
                                                                {
                                                                    var CGroup = new SignalTree("Phase C");
                                                                    CGroup.Parent = mGroup;
                                                                    //CGroup.Signal.TypeAbbreviation = "IWC";
                                                                    foreach (var signal in phase)
                                                                    {
                                                                        var s = new SignalTree(signal);
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
                    //newSig.SamplingRate = rate;
                    //var a = new SignalTree(newSig);
                    newTree.SignalList = signalTypeTree;
                    signalTypeTreeGroupedBySamplingRate.Add(newTree);
                }
            }
            return signalTypeTreeGroupedBySamplingRate;
        }

        private void _signalTreeCheckStatusChanged(SignalTree e)
        {
            OnSignalTreeCheckStatusChanged(e);
        }
        public event SignalTreeCheckStatusChangedEventHandler SignalCheckStatusChanged;
        public delegate void SignalTreeCheckStatusChangedEventHandler(SignalTree e);
        private void OnSignalTreeCheckStatusChanged(SignalTree e)
        {
            SignalCheckStatusChanged?.Invoke(e);
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
                        newGroup.Parent = newTree;
                        foreach (var signal in subgroup.Value)
                        {
                            var aTree = new SignalTree(signal);
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
    }
}
