using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AS.Core.Models;

namespace AS.IO
{
    public class DataFileReaderFactory
    {
        public static IDataFileReader Create(DataFileType type)
        {
            IDataFileReader newReader = null;
            switch (type)
            {
                case DataFileType.pdat:
                    newReader = new PDATFileReader();
                    break;
                case DataFileType.csv:
                    newReader = new CSVFileReader();
                    break;
                case DataFileType.powHQ:
                    break;
                case DataFileType.PI:
                    break;
                case DataFileType.OpenHistorian:
                    break;
                default:
                    break;
            }
            return newReader;
        }
    }
}
