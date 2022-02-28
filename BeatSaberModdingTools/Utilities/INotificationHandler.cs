using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeatSaberModdingTools.Utilities
{
    public interface INotificationHandler
    {
        void ShowError(string title, string message);

        Task ShowErrorAsync(string title, string message);
    }
}
