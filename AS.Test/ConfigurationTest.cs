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
        public void TestDQFilterParse()
        {
            string json = @"{
                'Name': 'Test DQ Filter Name',
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

            DQFilter testfilter = JsonConvert.DeserializeObject<DQFilter>(json);
        }

        [TestMethod]
        public void TestDQFilterSerialize()
        {
            DQFilter testfilter = TestObjects.MakeDQFilter();
                
            string jsonOutput = JsonConvert.SerializeObject(testfilter, Formatting.None);

           Assert.IsNotNull(jsonOutput.Length);
        }


        [TestMethod]
        public void TestWrappingFilterParse()
        {
            string json = @"{
                'Name': 'Angle Wrapping',
                'Parameters': [
                    'AngleThreshold': 30
                ],
                'PMUs': [
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
                ]
            }";

            DQFilter testfilter = JsonConvert.DeserializeObject<DQFilter>(json);
        }

        [TestMethod]
        public void TestWrappingFilterSerialize()
        {
            WrappingFailureDQFilter testfilter = TestObjects.MakeWrappingFilter();

            string jsonOutput = JsonConvert.SerializeObject(testfilter, Formatting.None);

            Assert.IsNotNull(jsonOutput.Length);
        }
    }

    public static class TestObjects
    {
        public static DQFilter MakeDQFilter()
        {
            DQFilter testfilter = new DQFilter();
            testfilter.Name = "TestName";
            testfilter.PMUs = new List<SignalSignature>();
            testfilter.PMUs.Add(MakeSignalSignature());

            return testfilter;
        }
        public static WrappingFailureDQFilter MakeWrappingFilter()
        {
            WrappingFailureDQFilter testfilter = new WrappingFailureDQFilter();
            testfilter.PMUs = new List<SignalSignature>();
            testfilter.PMUs.Add(MakeSignalSignature());

            return testfilter;
        }
        public static SignalSignature MakeSignalSignature()
        {

            SignalSignature testObj = new SignalSignature();
            testObj.Name = "exData";
            testObj.Channels = new List<string> { "Sub1VA", "Sub2VA", "Sub3VA", "Sub4VA", "Sub5VA", "Sub6VA", "Sub7VA" };

            return testObj;
        }
    }

   
}
