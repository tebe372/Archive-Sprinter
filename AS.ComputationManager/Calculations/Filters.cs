using AS.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AS.ComputationManager.Calculations
{
    public static class Filters
    {
        public static void DropOutMissingFilt()
        {

        }

        public static void DropOutZeroFilt(Signal s)
        {
            for (int idx = 0; idx < s.Data.Count; idx++)
            {
                if (s.Data[idx] < 1e-15)
                {
                    s.Flags[idx] = false;
                }
            }
        }
    }
}
