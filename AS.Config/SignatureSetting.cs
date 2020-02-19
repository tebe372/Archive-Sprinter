using AS.ComputationManager.Calculations;
using AS.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AS.Config
{
    public abstract class SignatureSetting
    {
        public abstract void Process(List<Signal> e);
    }
    public class Variance : SignatureSetting
    {
        public override void Process(List<Signal> e)
        {
            //according to the input channels in variance, take part of the e as input to the following function call.
            var d = e.Take(5); // just an example
            var variance = SignatureCalculations.Variance(e);
        }
    }
}
