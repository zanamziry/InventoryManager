using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace InventoryManager.Core.Models
{
    public class SentOutside : INotifyPropertyChanged
    {
        public int ID { get; set; }    
        public int InventoryID { get; set; }
        public int AmountSent { get; set; }
        public int AmountSold { get; set; }
        public string Location { get; set; }
        public int Remaining => AmountSent - AmountSold;

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged()
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ID)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(InventoryID)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(AmountSent)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(AmountSold)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Location)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Remaining)));
        }
    }
}
