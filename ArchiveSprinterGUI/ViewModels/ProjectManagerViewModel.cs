using ArchiveSprinterGUI.Views;
using AS.ProjectManager;
using AS.Utilities;
using Microsoft.WindowsAPICodePack.Dialogs;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using System.Windows.Input;

namespace ArchiveSprinterGUI.ViewModels
{
    public class ProjectManagerViewModel : ViewModelBase
    {
        public ProjectManagerViewModel()
        {
            _model = new ProjectControl();
            _setupProjectTree();
            BrowseResultsStorage = new RelayCommand(_browseResultsStorage);
            AddAProject = new RelayCommand(_addAASProject);
            DeleteProject = new RelayCommand(_deleteAProject);
            AddATask = new RelayCommand(_addATask);
            DeleteATask = new RelayCommand(_deleteATask);
        }
        private ProjectControl _model;
        public string ResultsStoragePath
        {
            get { return _model.ResultsStoragePath; }
            set
            {
                _model.ResultsStoragePath = value;
                OnPropertyChanged();
                _setupProjectTree();
            }
        }

        private void _setupProjectTree()
        {
            if (_model.Projects.Count > 0)
            {
                Projects = new ObservableCollection<ProjectViewModel>();
                foreach (var prj in _model.Projects)
                {
                    var newprj = new ProjectViewModel(prj);
                    newprj.ProjectSelected += _onProjectSelected;
                    Projects.Add(newprj);
                }
            }
        }
        private void _onProjectSelected(object sender, ProjectViewModel e)
        {
            SelectedProject = e;
            foreach (var prj in Projects)
            {
                if (prj != e)
                {
                    prj.IsSelected = false;
                }
            }
            //SelectedRun = e.SelectedRun;
            if (e.SelectedRun != null)
            {
                OnRunSelected(e);
            }
        }
        public event EventHandler<ProjectViewModel> RunSelected;
        protected virtual void OnRunSelected(ProjectViewModel e)
        {
            RunSelected?.Invoke(this, e);
        }
        public ICommand BrowseResultsStorage { get; set; }
        private void _browseResultsStorage(object obj)
        {
            using (var fbd = new CommonOpenFileDialog())
            {
                fbd.InitialDirectory = ResultsStoragePath;
                fbd.IsFolderPicker = true;
                fbd.AddToMostRecentlyUsedList = true;
                fbd.AllowNonFileSystemItems = false;
                fbd.DefaultDirectory = ResultsStoragePath;
                fbd.EnsureFileExists = true;
                fbd.EnsurePathExists = true;
                fbd.EnsureReadOnly = false;
                fbd.EnsureValidNames = true;
                fbd.Multiselect = false;
                fbd.ShowPlacesList = true;
                fbd.RestoreDirectory = true;
                CommonFileDialogResult result = fbd.ShowDialog();

                if (result == CommonFileDialogResult.Ok && !string.IsNullOrWhiteSpace(fbd.FileName))
                {
                    try
                    {
                        ResultsStoragePath = fbd.FileName;
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error open project folder. Original error: " + ex.Message, "Error!", MessageBoxButton.OK);
                    }
                }
            }
        }
        private ObservableCollection<ProjectViewModel> _projects;
        public ObservableCollection<ProjectViewModel> Projects
        {
            get { return _projects; }
            set
            {
                _projects = value;
                OnPropertyChanged();
            }
        }
        private AddAProjectPopup _addProjectdialogbox;
        public ICommand AddAProject { get; set; }
        private void _addAASProject(object obj)
        {
            var _addProjectVM = new AddProjectViewModel();
            _addProjectVM.NameAccepted += _generateNewProjectStructureOnDisk;
            _addProjectVM.NewTaskCancelled += _newProjectCancelled;
            _addProjectdialogbox = new AddAProjectPopup
            {
                Owner = System.Windows.Application.Current.MainWindow,
                DataContext = _addProjectVM
            };
            _addProjectdialogbox.ShowDialog();
        }

        private void _newProjectCancelled(object sender, EventArgs e)
        {
            _addProjectdialogbox.Close();
        }

        private void _generateNewProjectStructureOnDisk(object sender, EventArgs e)
        {
            _addProjectdialogbox.Close();
            var newProjectName = ((AddProjectViewModel)sender).NewProjectName;
            var nameExistsFlag = false;
            foreach (var prj in _projects)
            {
                if (prj.ProjectName == newProjectName)
                {
                    nameExistsFlag = true;
                    break;
                }
            }
            if (nameExistsFlag)
            {
                MessageBox.Show("Project exists, please give a new name!", "ERROR!", MessageBoxButton.OK);
                var _addProjectVM = new AddProjectViewModel();
                _addProjectVM.NameAccepted += _generateNewProjectStructureOnDisk;
                _addProjectVM.NewTaskCancelled += _newProjectCancelled;
                _addProjectdialogbox = new AddAProjectPopup
                {
                    Owner = System.Windows.Application.Current.MainWindow,
                    DataContext = _addProjectVM
                };
                _addProjectdialogbox.ShowDialog();
            }
            else
            {
                var newProjectDir = ResultsStoragePath + "\\Project_" + newProjectName;
                Directory.CreateDirectory(newProjectDir);
                _model.GenerateProjectTree();
                _setupProjectTree();
            }
        }

        public ICommand DeleteProject { get; set; }
        private void _deleteAProject(object obj)
        {
            var tobeDeleted = (ProjectViewModel)obj;
            var dialogResult = MessageBox.Show("Are you sure you want to delete the whole project: " + tobeDeleted.ProjectName + " ?", "Warning!", MessageBoxButton.YesNo);
            if (dialogResult == MessageBoxResult.Yes)
            {
                foreach (var prj in _projects)
                {
                    if (prj.ProjectName == tobeDeleted.ProjectName)
                    {
                        _projects.Remove(prj);
                        break;
                    }
                }
                try
                {
                    Directory.Delete(tobeDeleted.Projectpath, true);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error deleting the task directories. Error message: " + ex.Message, "Error!", MessageBoxButton.OK);
                }
                _model.GenerateProjectTree();
                _setupProjectTree();
            }
        }
        private AddATaskPopup _addataskdialogbox;
        public ICommand AddATask { get; set; }
        private void _addATask(object obj)
        {
            var thisProject = (ProjectViewModel)obj;
            _showAddTaskPopupWindow(thisProject);
            //if (obj != null && !File.Exists(_generatedNewRun.Model.ConfigFilePath))
            //{
            //    FileStream fs = File.Create(_generatedNewRun.Model.ConfigFilePath);
            //    fs.Close();
            //    var wr = new ConfigFileWriter(new SettingsViewModel(), _generatedNewRun.Model);
            //    wr.WriteXmlConfigFile(_generatedNewRun.Model.ConfigFilePath);
            //}
        }

        private void _showAddTaskPopupWindow(ProjectViewModel e)
        {
            var _addTaskVM = new AddTaskViewModel(_projects);
            _addTaskVM.NameAccepted += _newTaskNameAccepted;
            _addTaskVM.NewTaskCancelled += _newTaskCancelled;
            _addTaskVM.SelectedProject = e;
            _addataskdialogbox = new AddATaskPopup
            {
                Owner = System.Windows.Application.Current.MainWindow,
                DataContext = _addTaskVM
            };
            _addataskdialogbox.ShowDialog();
        }

        private void _newTaskCancelled(object sender, ProjectViewModel e)
        {
            _addataskdialogbox.Close();
        }

        private void _newTaskNameAccepted(object sender, ProjectViewModel e)
        {
            _addataskdialogbox.Close();
            var newtaskName = ((AddTaskViewModel)sender).NewTaskName;
            var TaskExistsFlag = false;
            foreach (var task in e.Tasks)
            {
                if (task.TaskName == newtaskName)
                {
                    TaskExistsFlag = true;
                    break;
                }
            }
            if (TaskExistsFlag)
            {
                MessageBox.Show("Task exists, please give a new name!", "ERROR!", MessageBoxButton.OK);
                _showAddTaskPopupWindow(e);
            }
            else
            {
                e.AddANewTask(newtaskName);
                //_model.GenerateProjectTree();
                //_setupProjectTree();
            }
        }
        public ICommand DeleteATask { get; set; }
        private void _deleteATask(object obj)
        {
            var runToDelete = (ASTaskViewModel)obj;
            runToDelete.Parent.DeleteATask(runToDelete);
        }
        private ASTaskViewModel _selectedRun;
        public ASTaskViewModel SelectedRun
        {
            get { return _selectedRun; }
            set
            {
                _selectedRun = value;
//#if !DEBUG
//                CanRun = !_findRunGeneratedFile(value.Model.RunPath);
//#endif
                OnPropertyChanged();
            }
        }
        private ProjectViewModel _selectedProject;
        public ProjectViewModel SelectedProject
        {
            get { return _selectedProject; }
            set
            {
                _selectedProject = value;
                OnPropertyChanged();
            }
        }
    }

    public class AddProjectViewModel : ViewModelBase
    {
        public AddProjectViewModel()
        {
            _newProjectName = "";
            AcceptName = new RelayCommand(_nameOK);
            CancelNewTask = new RelayCommand(_cancelNewTask);
        }
        private string _newProjectName;
        public string NewProjectName
        {
            get { return _newProjectName; }
            set
            {
                _newProjectName = value;
                OnPropertyChanged();
            }
        }
        public ICommand AcceptName { get; set; }
        private void _nameOK(object obj)
        {
            OnNameAccepted();
        }

        public ICommand CancelNewTask { get; set; }
        private void _cancelNewTask(object obj)
        {
            NewProjectName = "";
            OnNewTaskCancelled();
        }
        public event EventHandler NameAccepted;
        protected virtual void OnNameAccepted()
        {
            NameAccepted?.Invoke(this, EventArgs.Empty);
        }
        public event EventHandler NewTaskCancelled;
        protected virtual void OnNewTaskCancelled()
        {
            NewTaskCancelled?.Invoke(this, EventArgs.Empty);
        }
    }

    public class ProjectViewModel : ViewModelBase
    {
        private Project _model;
        //public Project Model { get { return _model; } }
        public ProjectViewModel()
        {
        }
        public ProjectViewModel(Project m) : this()
        {
            _model = m;
            _tasks = new ObservableCollection<ASTaskViewModel>();
            foreach (var tsk in m.Tasks)
            {
                var newTask = new ASTaskViewModel(tsk);
                newTask.RunSelected += _onOneOfTheRunSelected;
                _tasks.Add(newTask);
                newTask.Parent = this;
            }
        }
        private ASTaskViewModel _selectedRun;
        public ASTaskViewModel SelectedRun
        {
            get { return _selectedRun; }
            set
            {
                _selectedRun = value;
                OnPropertyChanged();
            }
        }
        private void _onOneOfTheRunSelected(object sender, ASTaskViewModel e)
        {
            SelectedRun = e;
            foreach (var t in Tasks)
            {
                if (t != e)
                {
                    t.IsSelected = false;
                }
            }
            //OnProjectSelected(this);
        }

        private ObservableCollection<ASTaskViewModel> _tasks;
        public ObservableCollection<ASTaskViewModel> Tasks
        {
            set
            {
                _tasks = value;
                OnPropertyChanged();
            }
            get
            {
                return _tasks;
            }
        }
        public string ProjectName
        {
            get { return _model.ProjectName; }
        }

        public string Projectpath { get { return _model.Projectpath; } }
        private bool _isSelected;
        public bool IsSelected
        {
            get { return _isSelected; }
            set
            {
                if (_isSelected != value)
                {
                    _isSelected = value;
                    OnPropertyChanged();
                }
                if (value)
                {
                    OnProjectSelected(this);
                }
            }
        }

        public event EventHandler<ProjectViewModel> ProjectSelected;
        protected virtual void OnProjectSelected(ProjectViewModel e)
        {
            ProjectSelected?.Invoke(this, e);
        }

        public void AddANewTask(string newtaskName)
        {

            ASTask newTask = _model.CreatTaskDir(newtaskName);

            var newTaskVieModel = new ASTaskViewModel(newTask);
            newTaskVieModel.RunSelected += _onOneOfTheRunSelected;
            Tasks.Add(newTaskVieModel);
            newTaskVieModel.Parent = this;
            Tasks = new ObservableCollection<ASTaskViewModel>(Tasks);
        }

        public void DeleteATask(ASTaskViewModel e)
        {
            var dialogResult = MessageBox.Show("Are you sure you want to delete this task: " + e.TaskName + " ?", "Warning!", MessageBoxButton.YesNo);
            if (dialogResult == MessageBoxResult.Yes)
            {
                foreach (var task in _model.Tasks)
                {
                    if (task.TaskName == e.TaskName)
                    {
                        _model.Tasks.Remove(task);
                        _model.RemoveTaskDirRecursively(task.TaskPath);
                        break;
                    }
                }
                Tasks.Remove(e);
            }
        }
    }
    public class AddTaskViewModel : ViewModelBase
    {
        private ObservableCollection<ProjectViewModel> _projects;
        public ObservableCollection<ProjectViewModel> Projects
        {
            get { return _projects; }
            set
            {
                _projects = value;
                OnPropertyChanged();
            }
        }
        public AddTaskViewModel(ObservableCollection<ProjectViewModel> projects)
        { 
            _projects = projects;
            AcceptName = new RelayCommand(_nameOK);
            CancelNewTask = new RelayCommand(_cancelNewTask);
        }
        public event EventHandler<ProjectViewModel> NameAccepted;
        protected virtual void OnNameAccepted(ProjectViewModel e)
        {
            NameAccepted?.Invoke(this, e);
        }
        public event EventHandler<ProjectViewModel> NewTaskCancelled;
        protected virtual void OnNewTaskCancelled(ProjectViewModel e)
        {
            NewTaskCancelled?.Invoke(this, e);
        }
        private ProjectViewModel _selectedProject;
        public ProjectViewModel SelectedProject
        {
            get { return _selectedProject; }
            set
            {
                _selectedProject = value;
                OnPropertyChanged();
            }
        }
        private string _newTaskName;
        public string NewTaskName
        {
            get { return _newTaskName; }
            set
            {
                _newTaskName = value;
                OnPropertyChanged();
            }
        }
        public ICommand AcceptName { get; set; }
        private void _nameOK(object obj)
        {
            OnNameAccepted(SelectedProject);
        }

        public ICommand CancelNewTask { get; set; }
        private void _cancelNewTask(object obj)
        {
            NewTaskName = "";
            OnNewTaskCancelled(SelectedProject);
        }
    }

    public class ASTaskViewModel : ViewModelBase
    {
        private ASTask _model;

        public ASTaskViewModel(ASTask tsk)
        {
            _model = tsk;
        }
        public string TaskName { get { return _model.TaskName; } }
        private bool _isSelected;
        public bool IsSelected
        {
            get { return _isSelected; }
            set
            {
                if (_isSelected != value)
                {
                    _isSelected = value;
                    OnPropertyChanged();
                    if (value)
                    {
                        OnRunSelected(this);
                        Parent.IsSelected = value;
                    }
                }
            }
        }
        private ProjectViewModel _parent;
        public ProjectViewModel Parent 
        {
            get { return _parent; }
            set { _parent = value; }
        }

        public string ConfigFilePath { get { return _model.ConfigFilePath; } }

        public event EventHandler<ASTaskViewModel> RunSelected;
        protected virtual void OnRunSelected(ASTaskViewModel e)
        {
            RunSelected?.Invoke(this, e);
        }
    }
}
