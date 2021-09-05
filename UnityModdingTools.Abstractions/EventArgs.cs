using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace UnityModdingTools.Abstractions
{
    public abstract class WaitableEventArgs : EventArgs
    {
        private object _waitLock = new object();
        private List<Task>? _waitList;
        public Task? WaitTask
        {
            get
            {
                lock (_waitLock)
                {
                    if (_waitList != null && _waitList.Count > 0)
                        return Task.WhenAll(_waitList);
                    else
                        return null;
                }
            }
        }
        public WaitableEventArgs()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="waitTask"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public void AddWait(Task waitTask)
        {
            if (waitTask == null)
                throw new ArgumentNullException(nameof(waitTask));
            lock (_waitLock)
            {
                if (_waitList == null)
                    _waitList = new List<Task>() { waitTask };
                else
                    _waitList.Add(waitTask);
            }
        }
    }

    public abstract class ProjectWaitableEventArgs : WaitableEventArgs
    {
        public string ProjectName { get => ProjectModel.ProjectName; }
        public IProjectModel ProjectModel { get; }
        public ProjectWaitableEventArgs(IProjectModel model)
            : base()
        {
            ProjectModel = model ?? throw new ArgumentNullException(nameof(model));
        }
    }
    public class RefreshingEventArgs : ProjectWaitableEventArgs
    {
        public RefreshingEventArgs(IProjectModel model)
            : base(model)
        {

        }
    }

    public class ProjectLoadingEventArgs : ProjectWaitableEventArgs
    {
        public ProjectLoadingEventArgs(IProjectModel model)
            : base(model)
        {

        }
    }

    public class ProjectUnloadingEventArgs : ProjectWaitableEventArgs
    {
        public ProjectUnloadingEventArgs(IProjectModel model)
            : base(model)
        {

        }
    }
}
