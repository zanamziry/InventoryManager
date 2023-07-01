using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace InventoryManager.Core.Models
{
    public class LocalInventory : INotifyPropertyChanged
    {
        public int ID { get; set; }
        public string ProductID { get; set; }
        public int Inventory { get; set; }
        public int Open { get; set; }
        public DateTime? ExpireDate { get; set; }
        public string Note { get; set; }
        public int Total => Inventory + Open;

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnRealChanged()
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Inventory))); 
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Open))); 
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ExpireDate))); 
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Note))); 
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Total))); 
        }
    
    }
}
