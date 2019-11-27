using BeatSaberModdingTools.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeatSaberModdingTools.ViewModels
{
    public class ReferenceWindowViewModel : ViewModelBase
    {
        public ObservableCollection<ReferenceModel> AvailableReferences { get; }

        public ReferenceWindowViewModel()
        {
            AvailableReferences = new ObservableCollection<ReferenceModel>();
        }
    }
}