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
        public static void DropOutMissingFilt(Signal s)
        {
            for (int idx = 0; idx < s.Data.Count; idx++)
            {
                if (double.IsNaN(s.Data[idx]))
                {
                    s.Flags[idx] = false;
                }
            }
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

        public static void PMUflagFilt(Signal s)
        {
            for (int idx = 0; idx < s.Data.Count; idx++)
            {
                if (s.Stat[idx] >= 4096)
                {
                    s.Flags[idx] = false;
                }
            }
        }

        public static void VoltPhasorFilt(Signal s, double voltMax, double voltMin, double nomVoltage)
        {
        }

        public static void FreqFilt(Signal s)
        {
        }
    }
}
