using LPCloudCore.Models.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace LPCloudCore.Models.Primitives
{
    public class FileItem : ItemBase
    {
        public string Name { get; set; }
        public string Fullpath { get; set; }
    }
}
