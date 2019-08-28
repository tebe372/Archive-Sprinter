using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AS.Core.Models;

namespace AS.IO
{
    public interface IDataFileReader
    {
        List<Signal> Read(string filename);
    }
}
