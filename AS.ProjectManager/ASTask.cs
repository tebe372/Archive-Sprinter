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
            _taskPath = dir;
            _taskName = Path.GetFileName(dir).Split(new[] { '_' }, 2)[1];
            foreach (var file in Directory.GetFiles(dir))
            {
                if (Path.GetFileNameWithoutExtension(file) == "Config" && Path.GetExtension(file).ToLower() == ".json")
                {
                    _configFilePath = file;
                }
            }
        }
        private string _taskPath;
        public string TaskPath { get { return _taskPath; } set { _taskPath = value; } }
        private string _taskName;
        public string TaskName { get { return _taskName; } set { _taskName = value; } }
        private string _configFilePath;
        public string ConfigFilePath { get { return _configFilePath; } set { _configFilePath = value; } }
        //private string _signaturePath;
        //public string SignaturePath { get { return _signaturePath; } set { _signaturePath = value; } }
    }
}
