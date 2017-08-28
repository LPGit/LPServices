using LPCloudCore.Models.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace LPCloudCore.Models.Primitives
{
    public class TodoItem : ItemBase
    {
        public string Task { get; set; }
        public string Description { get; set; }
    }
}
