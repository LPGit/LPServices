using LPWpf.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesktopGui.ViewModels.Items
{
    public class ItemBaseVM : BindableBase
    {
        public virtual DateTime CreatedDate { get; }

        public bool IsActive { get { return isActive; } set { SetProperty(ref isActive, value); } }
        private bool isActive;

        public ObservableCollection<string> Tags { get; protected set; }

        public ItemBaseVM()
        {
        }
    }
}
