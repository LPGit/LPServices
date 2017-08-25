using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesktopGui.ViewModels
{
    public class DirectoryVM : ReactiveObject
    {
        public string Fullpath { get; set; }

        public bool IncludeSubfolders { get { return includeSubfolders; } set { this.RaiseAndSetIfChanged(ref includeSubfolders, value); } }
        private bool includeSubfolders = false;

    }
}
