using AS.ComputationManager.Calculations;
using AS.Core.Models;
using AS.DataManager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AS.Config
{
    public abstract class SignatureSetting
    {
        public abstract void Process(DataStore dataMngr);
        public int WindowSize { get; set; }
        public int WindowOverlap { get; set; }
    }
    public class Variance : SignatureSetting
    {
        public override void Process(DataStore dataMngr)
        {
            //if there are still data to be processed
            // two situation: 1, un-processed data available; 2, has to wait for data being read.
            // while waiting, keep checking back

            while (dataMngr.GetData() || !dataMngr.DataCompleted)
            {
                //according to the input channels in variance, take part of the e as input to the following function call.
                var variance = SignatureCalculations.Variance(null);
            }            
        }
    }
}
