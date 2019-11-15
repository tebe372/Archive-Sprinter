using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using AS.IO;
using AS.Core.Models;
using System.Collections.Generic;

namespace AS.Test
{
    [TestClass]
    public class CSVReaderTest
    {
        [TestMethod]
        public void TestCreate()
        {
            var reader = DataFileReaderFactory.Create(DataFileType.csv);
            Assert.IsNotNull(reader, "Factory created null csv reader");
        }

        [TestMethod]
        public void TestOpen()
        {
            var reader = DataFileReaderFactory.Create(DataFileType.csv);
            List<Signal> signals = reader.Read("Data\\ExData_20170203_000800.csv");

            Assert.IsNotNull(signals);
        }

        [TestMethod]
        public void TestRead()
        {
            // Read in file and compare with known results
            var reader = DataFileReaderFactory.Create(DataFileType.csv);
            List<Signal> signals = reader.Read("Data\\ExData_20170203_000800.csv");

            Assert.IsNotNull(signals);
            Assert.AreEqual(signals.Count, 8);
            Assert.AreEqual(signals[0].Data.Count, 1800);
        }

        [TestMethod]
        public void TestRead2()
        {
            // Read in file and compare with known results
            CSVFileReader reader = (CSVFileReader)DataFileReaderFactory.Create(DataFileType.csv);
            List<Signal> signals = reader.Read2("Data\\ExData_20170203_000800.csv");

            Assert.IsNotNull(signals);
            Assert.AreEqual(signals.Count, 8);
            Assert.AreEqual(signals[0].Data.Count, 1800);
        }
    }
}
