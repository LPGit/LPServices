using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesktopGui.ViewModels
{
    public class FileResult
    {
        public string Title => this.Name;
        public string Directory => Path.GetDirectoryName(this.FullName);

        public string Name { get; set; }
        public string FullName { get; set; }
    }
}
