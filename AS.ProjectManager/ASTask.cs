using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AS.ProjectManager
{
    public class ASTask
    {
        public ASTask(string dir)
        {
            _taskPath = dir + "\\";
            _taskName = Path.GetFileName(dir).Split(new[] { '_' }, 2)[1];
            foreach (var file in Directory.GetFiles(dir))
            {
                if (Path.GetFileNameWithoutExtension(file) == "Config" && Path.GetExtension(file).ToLower() == ".json")
                {
                    _configFilePath = file;
                }
                if (Path.GetFileNameWithoutExtension(file) == "Pause" && Path.GetExtension(file).ToLower() == ".json")
                {
                    _pauseFilePath = file;
                }
            }
            foreach (var fldr in Directory.GetDirectories(dir))
            {
                if (Path.GetFileName(fldr) == "Signatures")
                {
                    _signaturePath = fldr + "\\";
                }
                //if (Path.GetFileName(dir) == "Init")
                //{
                //    _initializationPath = dir + "\\";
                //}
                //if (Path.GetFileName(dir) == "ControlRun")
                //{
                //    _controlRunPath = dir + "\\";
                //    var runFlag = _controlRunPath + "RunFlag.txt";
                //    var pauseFlag = _controlRunPath + "PauseFlag.txt";
                //    if (!System.IO.File.Exists(runFlag) && System.IO.File.Exists(pauseFlag) && System.IO.File.Exists(_controlRunPath + "PauseData.mat"))
                //    {
                //        _isRunPaused = true;
                //    }
                //    else
                //    {
                //        _isRunPaused = false;
                //    }
                //}
                //if (Path.GetFileName(dir) == "ControlRerun")
                //{
                //    _controlRerunPath = dir + "\\";
                //}
            }
        }
        private string _taskPath;
        public string TaskPath { get { return _taskPath; } set { _taskPath = value; } }
        private string _taskName;
        public string TaskName { get { return _taskName; } set { _taskName = value; } }

        public bool FindRunGeneratedFile()
        {
            return _findRunGeneratedFile(_taskPath);
        }

        private bool _findRunGeneratedFile(string path)
        {
            foreach (var file in Directory.GetFiles(path))
            {
                var ext = Path.GetExtension(file).ToLower();
                var filename = Path.GetFileNameWithoutExtension(file);
                if (ext == ".csv")
                {
                    return true;
                }
            }
            foreach (var dir in Directory.GetDirectories(path))
            {
                var result = _findRunGeneratedFile(dir);
                if (result)
                {
                    return result;
                }
            }
            return false;
        }

        private string _configFilePath;
        public string ConfigFilePath { get { return _configFilePath; } set { _configFilePath = value; } }
        private string _pauseFilePath;
        public string PauseFilePath { get { return _pauseFilePath; } set { _pauseFilePath = value; } }
        private string _signaturePath;
        public string SignaturePath { get { return _signaturePath; } set { _signaturePath = value; } }
        public bool CheckTaskDirIntegrity()
        {
            bool integrity = true;
            var sigPath = TaskPath + "Signatures";
            if (!Directory.Exists(sigPath))
            {
                Directory.CreateDirectory(sigPath);
                SignaturePath = sigPath;
                integrity = false;
            }
            var configFilePath = TaskPath + "Config.json";
            if (!File.Exists(configFilePath))
            {
                var config = JsonConvert.SerializeObject(new object(), Formatting.Indented);
                using (StreamWriter outputFile = new StreamWriter(configFilePath))
                {
                    outputFile.WriteLine(config);
                }
                ConfigFilePath = configFilePath;
                integrity = false;
            }
            return integrity;
        }
    }
}
