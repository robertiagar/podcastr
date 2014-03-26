using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PodcastR.WindowsStore.ViewModel
{
    public class StickyAppBarViewModel : ViewModelBase
    {
        private bool _isOpen;
        public bool IsOpen
        {
            get { return _isOpen; }
            set
            {
                Set<bool>(() => IsOpen, ref _isOpen, value);
            }
        }
    }
}
