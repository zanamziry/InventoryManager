using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace InventoryManager.Core.Models
{
    public class GivenAway : INotifyPropertyChanged
    {
        public int ID { get; set; }    
        public int ProductID { get; set; }
        public int Amount { get; set; }
        public string Event { get; set; }
        public DateTime Date { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged()
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ID)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ProductID)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Amount)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Event)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Date)));
        }
    }
}
