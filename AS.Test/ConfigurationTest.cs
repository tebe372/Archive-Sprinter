using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using AS.Config;
using Newtonsoft.Json;
using AS.Core.Models;
using System.Collections.Generic;

namespace AS.Test
{
    [TestClass]
    public class ConfigurationTest
    {
        [TestMethod]
        public void TestFilterParse()
        {
            string json = @"{
                'Name': 'Test Filter Name',
                'PMUs': [
                  {
                  'Name': 'ExData',
                  'Channels' : [
                       'Sub1VA',
                       'Sub2VA',
                       'Sub3VA',
                       'Sub4VA',
                       'Sub5VA',
                       'Sub6VA',
                       'Sub7VA'
                   ]
                }
               ]
            }";

            Filter testfilter = JsonConvert.DeserializeObject<Filter>(json);
        }

        [TestMethod]
        public void TestFilterSerialize()
        {
            Filter testfilter = TestObjects.MakeDQFilter();
                
            string jsonOutput = JsonConvert.SerializeObject(testfilter, Formatting.None);

           Assert.IsNotNull(jsonOutput.Length);
        }


        [TestMethod]
        public void TestWrappingFilterParse()
        {
            string json = @"{
                'Name': 'Angle Wrapping',
                'AngleThreshold': '30',
                'PMUs': [
                  {
                    'Name': 'ExData',
                  'Channels' : [
                       'Sub1VA',
                       'Sub2VA',
                       'Sub3VA',
                       'Sub4VA',
                       'Sub5VA',
                       'Sub6VA',
                       'Sub7VA'
                   ]
                }]
            }";

            Filter testfilter = JsonConvert.DeserializeObject<Filter>(json);
        }

        [TestMethod]
        public void TestWrappingFilterSerialize()
        {
            Filter testfilter = TestObjects.MakeWrappingFilter();

            string jsonOutput = JsonConvert.SerializeObject(testfilter, Formatting.None);

            Assert.IsFalse(jsonOutput.Length <= 0);
        }

        [TestMethod]
        public void TestConfigurationSerialize()
        {
            Configuration testconfig = TestObjects.MakeConfiguration();

            string jsonOutput = JsonConvert.SerializeObject(testconfig, Formatting.None);

            Assert.IsFalse(jsonOutput.Length <= 0);
        }
    }

    public static class TestObjects
    {
        public static Filter MakeDQFilter()
        {
            Filter testfilter = new Filter("TestName");
            testfilter.PMUs = new List<SignalSignature>();
            testfilter.PMUs.Add(MakeSignalSignature());

            return testfilter;
        }
        public static Filter MakeWrappingFilter()
        {
            /*   WrappingFailureDQFilter testfilter = new WrappingFailureDQFilter();
               testfilter.PMUs = new List<SignalSignature>();
               testfilter.PMUs.Add(MakeSignalSignature());

               testfilter.AddParameter("AngleThresh", "30");

               return testfilter;
               */
            Filter testfilter = new Filter("Angle Wrapping");
            testfilter.PMUs = new List<SignalSignature>();
            testfilter.PMUs.Add(MakeSignalSignature());

            testfilter.AddParameter("AngleThresh", "30");

            return testfilter;
        }
        public static SignalSignature MakeSignalSignature()
        {

            SignalSignature testObj = new SignalSignature();
            testObj.Name = "exData";
            testObj.Channels = new List<string> { "Sub1VA", "Sub2VA", "Sub3VA", "Sub4VA", "Sub5VA", "Sub6VA", "Sub7VA" };

            return testObj;
        }

        public static Configuration MakeConfiguration()
        {
            Configuration testObj = new Configuration();
            testObj.InputFiles.Add(MakeInputFile());
            testObj.PreProcessSteps.Add(MakeDQFilter());
            testObj.PreProcessSteps.Add(MakeWrappingFilter());

            return testObj;
        }

        public static InputFileInfo MakeInputFile(DataFileType type = DataFileType.csv)
        {
            InputFileInfo testFile = new InputFileInfo(".\\TestDir\\");
            testFile.FileType = type;

            return testFile;
        }
    }

   
}
