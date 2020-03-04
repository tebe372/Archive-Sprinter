using AS.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AS.ComputationManager.Calculations
{
    public static class Customizations
    {
        public static List<double> SubtractionCustomization(List<double> subtrahend, List<double> minuend)
        {
            //process the subtraction and return the new signal.
            var result = new List<double>();
            for (int i = 0; i < subtrahend.Count; i++)
            {
                result.Add(subtrahend[i] - minuend[i]);
            }
            return result;
        }
    }
}
