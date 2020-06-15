using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace AS.ProjectManager
{
    public class ProjectControl
    {
        public ProjectControl()
        {
            _resultsStoragePath = AS.ProjectManager.Properties.Settings.Default.ResultsStoragePath;
            if (!Directory.Exists(_resultsStoragePath))
            {
                _resultsStoragePath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            }
            try
            {
                _generateProjectTree(_resultsStoragePath);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private List<Project> _projects;
        public List<Project> Projects
        {
            get { return _projects; }
            //set { _projects = value; }
        }
        private string _resultsStoragePath;
        public string ResultsStoragePath
        {
            get { return _resultsStoragePath; }
            set
            {
                if (_resultsStoragePath != value)
                {
                    _resultsStoragePath = value;
                    AS.ProjectManager.Properties.Settings.Default.ResultsStoragePath = value;
                    AS.ProjectManager.Properties.Settings.Default.Save();
                    try
                    {
                        _generateProjectTree(_resultsStoragePath);
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                }
            }
        }

        private void _generateProjectTree(string resultsStoragePath)
        {
            if (Directory.Exists(ResultsStoragePath))
            {
                var projects = new List<Project>();
                var dirs = new DirectoryInfo(ResultsStoragePath).GetDirectories().OrderBy(x => x.CreationTime).ToList();
                foreach (var dir in dirs)
                {
                    var dirName = dir.FullName;
                    var projectNameFrac = Path.GetFileName(dirName).Split('_');
                    if (projectNameFrac[0] == "Project" && Directory.Exists(dirName))
                    {
                        var aNewPoject = new Project(dirName);
                        //aNewPoject.ProjectSelected += _onProjectSelected;
                        projects.Add(aNewPoject);
                    }
                }
                _projects = projects;
            }
            else
            {
                throw new Exception("Directory does not exists.");
            }
        }
        public void GenerateProjectTree()
        {
            _generateProjectTree(_resultsStoragePath);
        }
    }
}
