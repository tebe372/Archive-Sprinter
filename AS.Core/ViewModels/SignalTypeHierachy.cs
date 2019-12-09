using AS.Core.Models;
using AS.Utilities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AS.Core.ViewModels
{
    public class SignalTypeHierachy : ViewModelBase
    {
        public SignalTypeHierachy()
        {
            _signalSignature = new Signal();
            _signalList = new ObservableCollection<SignalTypeHierachy>();
        }
        public SignalTypeHierachy(Signal signature)
        {
            _signalSignature = signature;
            _signalList = new ObservableCollection<SignalTypeHierachy>();
        }
        public SignalTypeHierachy(Signal signature, ObservableCollection<SignalTypeHierachy> list)
        {
            _signalSignature = signature;
            _signalList = list;
        }
        private Signal _signalSignature;
        public Signal SignalSignature
        {
            get
            {
                return _signalSignature;
            }
            set
            {
                _signalSignature = value;
                OnPropertyChanged();
            }
        }
        private ObservableCollection<SignalTypeHierachy> _signalList;
        public ObservableCollection<SignalTypeHierachy> SignalList
        {
            get
            {
                return _signalList;
            }
            set
            {
                _signalList = value;
                OnPropertyChanged();
            }
        }
    }
}
