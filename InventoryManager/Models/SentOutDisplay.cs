using ControlzEx.Standard;
using InventoryManager.Core.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventoryManager.Models
{
    public class SentOutDisplay : INotifyPropertyChanged
    {

        public Product Product { get; set; }
        public SentOutside Outside { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged()
        {
            Outside.OnPropertyChanged();
        }
    }
}
