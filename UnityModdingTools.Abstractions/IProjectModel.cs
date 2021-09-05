using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace UnityModdingTools.Abstractions
{
    public interface IProjectModel : IProjectComponent, IEnumerable<IProjectElement>
    {
        string ProjectName { get; }
        bool IsUMTProject { get; }
        bool IsDirty { get; }
        bool ModelChanged { get; }
        Task LoadAsync();
        Task UnloadAsync();
        Task SaveAsync();
        Task<IProjectModel> RefreshModel();

        event EventHandler<RefreshingEventArgs> ProjectRefreshing;
        event EventHandler<ProjectLoadingEventArgs> ProjectLoading;
        event EventHandler<ProjectUnloadingEventArgs> ProjectUnloading;
    }
}
