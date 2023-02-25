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
        public LocalInventory Inventory { get; set; }
        public SentOutside Outside { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged()
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Product.ID)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Product.Name)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Product.Price)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Inventory.ID)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Outside.AmountSold)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Outside.AmountSent)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Outside.Remaining)));
        }
    }
}
