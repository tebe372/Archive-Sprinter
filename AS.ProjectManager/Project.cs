using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AS.ProjectManager
{
    public class Project
    {
        public Project(string dir)
        {
            _projectpath = dir + "\\";
            _projectName = Path.GetFileName(dir).Split(new[] { '_' }, 2)[1];
            _tasks = new List<ASTask>();
            var dirs = new DirectoryInfo(dir).GetDirectories().OrderBy(x => x.CreationTime).ToList();
            foreach (var run in dirs)
            {
                var dirName = run.FullName;
                var runNameFrac = Path.GetFileName(dirName).Split(new[] { '_' }, 2);
                if (runNameFrac[0] == "Task" && Directory.Exists(dirName))
                {
                    _tasks.Add(new ASTask(dirName));
                }
            }
        }
        private string _projectName;
        public string ProjectName
        {
            get { return _projectName; }
        }
        private string _projectpath;
        public string Projectpath
        {
            get { return _projectpath; }
        }
        private List<ASTask> _tasks;
        public List<ASTask> Tasks
        {
            get { return _tasks; }
        }

        public void RemoveTaskDirRecursively(string path)
        {
            foreach (var dir in Directory.GetDirectories(path))
            {
                RemoveTaskDirRecursively(dir);
            }
            try
            {
                Directory.Delete(path, true);
            }
            catch (IOException ex)
            {
                Directory.Delete(path, true);
            }
            catch (UnauthorizedAccessException ex)
            {
                Directory.Delete(path, true);
            }
        }

        public ASTask CreatTaskDir(string name)
        {
            var taskDir = Projectpath + "Task_" + name;
            var signaturePath = taskDir + "\\Signatures\\";
            Directory.CreateDirectory(signaturePath);
            //DirectoryInfo dir = Directory.CreateDirectory(taskDir);
            var newTask = new ASTask(taskDir);
            newTask.ConfigFilePath = taskDir + "\\Config.json";
            if (Directory.Exists(taskDir) && !File.Exists(newTask.ConfigFilePath))
            {
                //FileStream fs = File.Create(newTask.ConfigFilePath);
                //fs.Close();
                var config = JsonConvert.SerializeObject(new object(), Formatting.Indented);
                using (StreamWriter outputFile = new StreamWriter(newTask.ConfigFilePath))
                {
                    outputFile.WriteLine(config);
                }
            }
            Tasks.Add(newTask);
            return newTask;
        }
    }
}
