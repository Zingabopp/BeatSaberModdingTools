using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Xml.Linq;
using UnityModdingTools.Abstractions;

namespace UnityModdingTools.Projects
{
    public class ProjectModel : IProjectModel
    {
        public static async Task<ProjectModel> CreateAsync(ICOMProject comProject, string projectPath)
        {
            string userProjectPath = projectPath + ".user";
            ProjectModel model = new ProjectModel(comProject, projectPath, userProjectPath);

            var created = await model.RefreshModel().ConfigureAwait(false);
            return (ProjectModel)created;
        }

        public static async Task<ProjectModel> CreateAsync(ICOMProject comProject, XDocument project, string projectPath)
        {
            string userProjectPath = projectPath + ".user";
            ProjectModel model = new ProjectModel(comProject, projectPath, userProjectPath)
            {
                Project = project
            };

            var created = await model.RefreshModel().ConfigureAwait(false);
            return (ProjectModel)created;
        }
        protected ICOMProject COMProject;
        protected XDocument Project;
        public readonly string ProjectPath;
        protected XDocument UserProject;
        public readonly string UserProjectPath;

        public event EventHandler<RefreshingEventArgs>? ProjectRefreshing;
        public event EventHandler<ProjectLoadingEventArgs>? ProjectLoading;
        public event EventHandler<ProjectUnloadingEventArgs>? ProjectUnloading;

        public ProjectModel(ICOMProject comProject, XDocument project, string projectPath, XDocument userProject, string userProjectPath)
        {
            COMProject = comProject ?? throw new ArgumentNullException(nameof(comProject));
            Project = project;
            ProjectPath = projectPath;
            UserProject = userProject;
            UserProjectPath = userProjectPath;
        }

        /// <summary>
        /// Private constructor using just the paths. Project and UserProject MUST be set outside constructor.
        /// </summary>
        /// <param name="projectPath"></param>
        /// <param name="userProjectPath"></param>
        private ProjectModel(ICOMProject comProject, string projectPath, string? userProjectPath = null)
        {
            COMProject = comProject ?? throw new ArgumentNullException(nameof(comProject));
            // Set in CreateAsync
            Project = null!;
            UserProject = null!;

            ProjectPath = projectPath ?? throw new ArgumentNullException(nameof(projectPath));
            UserProjectPath = userProjectPath ?? projectPath + ".user";
        }

        #region IProjectModel
        public bool IsUMTProject => throw new NotImplementedException();

        public bool IsDirty => COMProject.IsDirty;

        public bool ModelChanged => throw new NotImplementedException();

        public IProjectComponent Parent => throw new NotImplementedException();

        public string Condition { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public IEnumerable<IProjectComponent> Content => throw new NotImplementedException();

        public XObject XObject => Project;

        public ComponentState State { get; protected set; }

        public string ProjectName => throw new NotImplementedException();

        public bool IsConditioned => Condition != null && Condition.Length > 0;

        public IElementAttribute this[string s] 
        {
            get => throw new NotImplementedException(); 
            set => throw new NotImplementedException();
        }

        public async Task LoadAsync()
        {
            var eventArgs = new ProjectLoadingEventArgs(this);
            ProjectLoading?.Invoke(this, eventArgs);
            Task? waitTask = eventArgs.WaitTask;
            if (waitTask != null)
                await waitTask.ConfigureAwait(false);
            COMProject.Load();
        }

        public async Task UnloadAsync()
        {
            var eventArgs = new ProjectUnloadingEventArgs(this);
            ProjectUnloading?.Invoke(this, eventArgs);
            Task? waitTask = eventArgs.WaitTask;
            if (waitTask != null)
                await waitTask.ConfigureAwait(false);
            COMProject.Unload();
        }

        public Task SaveAsync()
        {
            throw new NotImplementedException();
        }

        public async Task<IProjectModel> RefreshModel()
        {
            var eventArgs = new RefreshingEventArgs(this);
            ProjectRefreshing?.Invoke(this, eventArgs);
            Task? waitTask = eventArgs.WaitTask;
            if (waitTask != null)
                await waitTask.ConfigureAwait(false);

            Project = XDocument.Load(ProjectPath);
            XDocument? userProject;
            if (File.Exists(UserProjectPath))
                userProject = XDocument.Load(UserProjectPath);
            else
                userProject = Utilities.GenerateUserProject();
            UserProject = userProject;
            return this;
        }


        public void SetState(ComponentState state)
        {
            State = state;
            throw new NotImplementedException();
        }

        public IEnumerator<IProjectElement> GetEnumerator()
        {
            throw new NotImplementedException();
        }

        IEnumerator IEnumerable.GetEnumerator()
            => GetEnumerator();

        #endregion

    }
}
