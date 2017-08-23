using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LPCloudAPI.Models
{
    public class ItemBase
    {
        /// <summary>
        /// The date when the item was added.
        /// </summary>
        public DateTime AddedDate { get; set; }
        public string RawText { get; set; }
        public virtual string Text { get { return this.RawText; } }

        public ItemBase(string rawText)
        {
            this.AddedDate = DateTime.Now;
            this.RawText = rawText;
        }
    }
}
