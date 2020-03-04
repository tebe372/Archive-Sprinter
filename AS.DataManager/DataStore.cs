using AS.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AS.DataManager
{
    public class DataStore
    {
        //should be able to find the signal we need quickly
        //signal should be found by name and time
        public DataStore()
        {
            DataCompleted = false;
        }
        // when all data in the data source directory are read and preprocessed and put in the store
        public bool DataCompleted { get; set; }

        public void AddData(List<Signal> e)
        {

        }

        public bool GetData()
        {
            throw new NotImplementedException();
        }
    }
}
